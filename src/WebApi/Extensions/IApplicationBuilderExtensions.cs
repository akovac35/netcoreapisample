using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WebApi.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebApi(this IApplicationBuilder app)
        {
            _ = app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("sl") },
                SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("sl") }
            });

            _ = app.UseMiddleware<ActivityMiddleware>();
            _ = app.UseMiddleware<ExceptionHandlingMiddleware>();

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();

            _ = app.UseRouting();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
            });

            return app;
        }
    }
}
