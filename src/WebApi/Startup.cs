using Domain;

namespace WebApi
{
    /// <summary>
    /// We need this class for the dotnet swagger tool to work.
    /// See also: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2290
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var sampleConfigSection = Configuration.GetRequiredSection(nameof(SampleConfig));

            _ = services.AddWebApi(sampleConfigSection);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseWebApi();
        }
    }
}
