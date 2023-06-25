using System.Linq.Expressions;
using Domain.AuthorRelated;
using Domain.BookRelated;
using FluentAssertions;
using Infrastructure.BookRelated.CommandAndQuery;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace WebApiTests.Tests.BookRelated
{
    public class CommandAndQueryTests : TestBase
    {
        [Test]
        public async Task CreateBook_CreatesBook()
        {
            using var scope = TestHelper.CreateTestServices(out var bookRepositoryMock, out var authorRepositoryMock).CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var createBookCommand = new CreateBook.Command
            {
                Title = "Test book",
                AuthorUniqueId = Guid.NewGuid(),
                Year = 2000,
                Publisher = "MK"
            };

            var bookDbo = await mediator.Send(createBookCommand);

            _ = bookDbo.Should().NotBeNull();
            _ = bookDbo.Should().BeEquivalentTo(createBookCommand, options => options.Excluding(o => o.AuthorUniqueId));
            bookRepositoryMock.Verify(x => x.AddOrUpdateAsync(
                It.IsAny<BookDbo>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        // TODO HIGH add tests for other CQs
    }

    public static class TestHelper
    {
        public static ServiceProvider CreateTestServices(out Mock<IBookRepository> bookRepositoryMock, out Mock<IAuthorRepository> authorRepositoryMock)
        {
            var services = new ServiceCollection();

            var appSettings = """
            {
              "SampleConfig": {
                "DbConnectionString": "N/A",
                "UseInMemoryDb": null,

                "JwtConfig": {
                  "TokenValidityInMinutes": "10"
                }
              }
            }
            """;

            _ = services.AddLogging();
            _ = services.AddTestConfig(appSettings);
            _ = services.CreateTestMocks(out bookRepositoryMock, out authorRepositoryMock);
            _ = services.AddWebApiEssentials();

            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static IServiceCollection CreateTestMocks(this IServiceCollection services, out Mock<IBookRepository> bookRepositoryMock, out Mock<IAuthorRepository> authorRepositoryMock)
        {
            bookRepositoryMock = new Mock<IBookRepository>();
            _ = bookRepositoryMock.Setup(_ => _.AddOrUpdateAsync(
                It.IsAny<BookDbo>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
            var bookRepository = bookRepositoryMock.Object;
            _ = services.AddScoped(f => bookRepository);

            authorRepositoryMock = new Mock<IAuthorRepository>();
            _ = authorRepositoryMock.Setup(_ => _.GetAsync(
                    It.IsAny<Expression<Func<AuthorDbo, bool>>>(),
                    It.IsAny<CancellationToken>()
                )).Returns(Task.FromResult(new AuthorDbo { FirstName = "George", LastName = "Orwell", UniqueId = Guid.NewGuid() })!);
            var authorRepository = authorRepositoryMock.Object;
            _ = services.AddScoped(f => authorRepository);

            return services;
        }
    }
}
