using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using WebApiClient;

namespace WebApiTests.Integration.Tests.BookRelated
{
    public class BooksControllerTestsWithInMemoryDb : BooksControllerTests<InMemoryDbWebApplicationFactory>
    {
    }

    public class BooksControllerTestsWithSqlServer : BooksControllerTests<SqlServerDbWebApplicationFactory>
    {
    }

    public class BooksControllerTestsWithOracle : BooksControllerTests<OracleDbWebApplicationFactory>
    {
    }

    public abstract class BooksControllerTests<T> : TestBase where T : WebApplicationFactory<TestProgram>, new()
    {
        private T
            factory = null!;

        private HttpClient httpClient = null!;

        private Client client = null!;

        private Guid bookUniqueId;
        private Guid authorUniqueId;

        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            factory = new T();
            httpClient = factory.CreateClient();

            client = Client.CreateTestClient(httpClient: httpClient);
            var token = client.CreateAuthenticationTokenAsync(new CreateAuthenticationTokenCommand { Username = "admin", Password = "password" }).Result;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        }

        public override void OneTimeTearDown()
        {
            base.OneTimeTearDown();

            httpClient?.Dispose();
            factory?.Dispose();
        }

        [Test, Order(1)]
        public async Task CreateBook_Works()
        {
            var createAuthorCommand = new CreateAuthorCommand
            {
                FirstName = "George",
                LastName = "Orwell"
            };

            var author = await client.CreateAuthorAsync(createAuthorCommand);

            var createBookCommand = new CreateBookCommand
            {
                AuthorUniqueId = author.Id,
                Publisher = "Publisher",
                Title = "Test title",
                Year = 2000
            };

            var book = await client.CreateBookAsync(createBookCommand);

            _ = book.Should().NotBeNull();
            _ = book.Author.Should().BeEquivalentTo(author);
            _ = book.Publisher.Should().Be(createBookCommand.Publisher);
            _ = book.Title.Should().Be(createBookCommand.Title);
            _ = book.Year.Should().Be(createBookCommand.Year);

            bookUniqueId = book.Id;
            authorUniqueId = author.Id;
        }

        [Test, Order(2)]
        public async Task GetBook_Works()
        {
            var book = await client.GetBookAsync(bookUniqueId);

            _ = book.Should().NotBeNull();
        }

        [Test, Order(3)]
        public async Task UpdateBook_Works()
        {
            var updateBookCommand = new UpdateBookCommand
            {
                AuthorUniqueId = authorUniqueId,
                Publisher = "Publisher",
                Title = "Test title",
                Year = 3000,
                Id = bookUniqueId
            };

            var book = await client.UpdateBookAsync(updateBookCommand);

            _ = book.Should().NotBeNull();
            _ = book.Author.Should().NotBeNull();
            _ = book.Publisher.Should().Be(updateBookCommand.Publisher);
            _ = book.Title.Should().Be(updateBookCommand.Title);
            _ = book.Year.Should().Be(updateBookCommand.Year);
        }

        [Test, Order(4)]
        public async Task DeletingAuthor_SetsAuthorToNull()
        {
            var deleteAuthorResult = await client.DeleteAuthorAsync(authorUniqueId);

            _ = deleteAuthorResult.Should().Be(1);

            var book = await client.GetBookAsync(bookUniqueId);

            _ = book.Should().NotBeNull();
            _ = book.Author.Should().BeNull();
        }

        [Test, Order(5)]
        public async Task DeleteBook_Works()
        {
            var deleteBookResult = await client.DeleteBookAsync(bookUniqueId);

            _ = deleteBookResult.Should().Be(1);
        }
    }
}
