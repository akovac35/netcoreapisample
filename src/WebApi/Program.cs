using Microsoft.AspNetCore;
using NLog;
using NLog.Web;

namespace WebApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Needed for tests")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting host");
                using var host = CreateHostBuilder(args).Build();
                await host.Services.InitializeWebApiAsync();
                host.Run();
                logger.Info("Stopping host");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw;
            }
            finally
            {
                try
                {
                    LogManager.Flush();
                }
                catch
                {
                }

                try
                {
                    LogManager.Shutdown();
                }
                catch
                {
                }
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider((context, options) =>
                {
#if DEBUG
                    options.ValidateScopes = true;
#endif
                });
        }
    }
}

