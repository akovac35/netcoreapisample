using Infrastructure.Diagnostics;

namespace WebApi.Middleware
{
    public class ActivityMiddleware : IMiddleware
    {
        public ActivityMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var (isNew, activity) = ActivityDiagnostics.CreateActivityOrReuse(
                operationName: $"HTTP {context.Request.Method} {context.Request.Path}");
            try
            {
                await next(context);

                _ = (activity?.SetStatus(
                    code: System.Diagnostics.ActivityStatusCode.Ok));
            }
            catch (Exception e)
            {
                _ = (activity?.SetStatus(
                    code: System.Diagnostics.ActivityStatusCode.Error,
                    description: e.Message
                    ));
                throw;
            }
            finally
            {
                if (isNew)
                {
                    activity?.Stop();
                    activity?.Dispose();
                }
            }
        }
    }
}
