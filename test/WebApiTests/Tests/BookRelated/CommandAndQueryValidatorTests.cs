using FluentAssertions;
using FluentValidation.TestHelper;
using WebApi.BookRelated.CommandAndQuery;

namespace WebApiTests.Tests.BookRelated
{
    public class CommandAndQueryValidatorTests
    {
        private static IEnumerable<TestCaseData> GetPageOfBooksValidator_Works_Data()
        {
            var caseIndex = 0;
            // Valid case
            yield return new TestCaseData(++caseIndex, new GetPageOfBooks.Query
            {
                ItemsPerPage = 10,
                PageIndex = 0,
                SearchMode = GetPageOfBooks.SearchMode.Guids,
                Guids = new List<Guid>() { Guid.NewGuid() }
            }, true, null);

            // Valid case
            yield return new TestCaseData(++caseIndex, new GetPageOfBooks.Query
            {
                ItemsPerPage = 10,
                PageIndex = 0,
                SearchMode = GetPageOfBooks.SearchMode.TitleContains,
                TitleContains = "test"
            }, true, null);

            // Invalid case
            yield return new TestCaseData(++caseIndex, new GetPageOfBooks.Query
            {
                ItemsPerPage = 10,
                PageIndex = 0,
                SearchMode = GetPageOfBooks.SearchMode.Guids
            }, false, nameof(GetPageOfBooks.Query.Guids));

            // Invalid case
            yield return new TestCaseData(++caseIndex, new GetPageOfBooks.Query
            {
                ItemsPerPage = 10,
                PageIndex = 0,
                SearchMode = GetPageOfBooks.SearchMode.TitleContains
            }, false, nameof(GetPageOfBooks.Query.TitleContains));
        }

        [TestCaseSource(nameof(GetPageOfBooksValidator_Works_Data))]
        public void GetPageOfBooksValidator_Works(int caseIndex, GetPageOfBooks.Query query, bool valid, string? propertyName)
        {
            var validator = new GetPageOfBooks.Validator();

            var result = validator.TestValidate(query);

            if (!valid)
            {
                result.ShouldHaveValidationErrorFor(propertyName);
            }
            else
            {
                result.IsValid.Should().BeTrue();
            }
        }
    }
}
