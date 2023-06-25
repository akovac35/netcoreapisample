using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace WebApiTests.Integration
{
    public class SqlServerDbWebApplicationFactory :
        WebApplicationFactory<TestProgram>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("TestingWithSqlServerDb");

            builder.UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = true;
            });

            // Mocks can be introduced here
            builder.ConfigureServices(services => { });

            var buildMode = "Release";

#if DEBUG
            buildMode = "Debug";
#endif

            builder.UseSolutionRelativeContentRoot($"test/WebApiTests.Integration/bin/{buildMode}/net7.0");

        }
    }
}
