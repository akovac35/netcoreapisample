using System.Diagnostics;
using System.Text.Json;
using Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApi.Resources;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly IStringLocalizer<WebApiText> localizer;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(IStringLocalizer<WebApiText> localizer, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.localizer = localizer;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                await HandleExceptionAsync(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var response = new ProblemDetails()
            {
                Title = GetTitle(exception),
                Status = GetStatusCode(exception),
                Detail = exception.Message
            };
            response.Extensions.Add(nameof(Activity.Current.SpanId), Activity.Current?.SpanId.ToString());
            if (Activity.Current?.Parent != null)
            {
                response.Extensions.Add(nameof(Activity.Current.ParentSpanId), Activity.Current?.ParentSpanId.ToString());
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = response.Status.Value;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                SampleNotFoundException => StatusCodes.Status404NotFound,
                SampleUnauthorizedException => StatusCodes.Status401Unauthorized,
                SampleForbiddenException => StatusCodes.Status403Forbidden,
                SampleException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private string GetTitle(Exception exception)
        {
            return exception switch
            {
                ValidationException => localizer["TitleForDataValidationError"].Value,
                SampleNotFoundException => localizer["TitleForNotFoundError"].Value,
                SampleUnauthorizedException => localizer["TitleForUnauthorizedError"].Value,
                SampleForbiddenException => localizer["TitleForForbiddenError"].Value,
                SampleException => localizer["TitleForBusinessError"].Value,
                _ => localizer["TitleForTechnicalError"].Value
            };
        }
    }
}
