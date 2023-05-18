using System.Globalization;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseWebApi(this WebApplication app)
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

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapControllers();

            return app;
        }

        public static async Task InitializeWebApi(this WebApplication app, bool useInMemoryDb)
        {
            using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var config = serviceScope.ServiceProvider.GetRequiredService<IOptions<SampleConfig>>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (!useInMemoryDb)
            {
                logger.LogInformation("Migrating database ...");

                var context = serviceScope.ServiceProvider.GetRequiredService<SampleDbContext>();
                var hostApplicationLifetime = serviceScope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

                await context.Database.MigrateAsync(cancellationToken: hostApplicationLifetime.ApplicationStopping)
                    .ConfigureAwait(true);
            }
        }
    }
}
