using System.Diagnostics;
using System.Text.Json;
using Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
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
        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
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
        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                SampleNotFoundException => StatusCodes.Status404NotFound,
                SampleException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitle(Exception exception)
        {
            return exception switch
            {
                ValidationException => "Data validation error",
                SampleException => "Business logic error",
                UnauthorizedAccessException => "Unauthorized",
                _ => "Server error"
            };
        }
    }
}
