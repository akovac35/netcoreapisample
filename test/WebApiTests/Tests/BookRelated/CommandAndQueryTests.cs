using System.Text;
using Domain;
using Domain.BookRelated;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebApi.BookRelated.CommandAndQuery;

namespace WebApiTests.Tests.BookRelated
{
    public class CommandAndQueryTests : TestBase
    {
        [Test]
        public async Task CreateBook_CreatesBook()
        {
            using var scope = TestHelper.CreateTestServices(out var bookRepositoryMock).CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new CreateBook.Command
            {
                Title = "Test book",
                Author = "Test Author",
                Year = 2000,
                Publisher = "MK"
            };

            var bookDto = await mediator.Send(command);

            _ = bookDto.Should().NotBeNull();
            _ = bookDto.Should().BeEquivalentTo(command);
            bookRepositoryMock.Verify(x => x.AddAsync(
                It.IsAny<Book>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        // TODO HIGH add tests for other CQs
    }

    public static class TestHelper
    {
        public static ServiceProvider CreateTestServices(out Mock<IBookRepository> bookRepositoryMock)
        {
            var services = new ServiceCollection();

            _ = services.AddTestConfig();
            _ = services.CreateTestMocks(out bookRepositoryMock);
            _ = services.AddWebApiBasic(useInMemoryDb: false);

            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static IServiceCollection CreateTestMocks(this IServiceCollection services, out Mock<IBookRepository> bookRepositoryMock)
        {
            bookRepositoryMock = new Mock<IBookRepository>();
            _ = bookRepositoryMock.Setup(_ => _.AddAsync(
                It.IsAny<Book>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book value, CancellationToken cancellationToken) => value
                );
            var bookRepository = bookRepositoryMock.Object;
            _ = services.AddScoped(f => bookRepository);

            return services;
        }

        private static IServiceCollection AddTestConfig(this IServiceCollection services)
        {
            var appSettings = """
            {
              "SampleConfig": {
                "DbConnectionString": "N/A",

                "JwtConfig": {
                  "TokenValidityInMinutes": "10"
                }
              }
            }
            """;

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
