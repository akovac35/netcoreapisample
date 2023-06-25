using System.Text;
using Domain;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddTestConfig(this IServiceCollection services, string appSettings)
        {
            var builder = new ConfigurationBuilder();

            _ = builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            var configSection = configuration.GetRequiredSection(nameof(SampleConfig));

            _ = services
                .AddOptions<SampleConfig>()
                .Bind(configSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
