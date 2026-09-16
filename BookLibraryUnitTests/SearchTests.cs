using BookLibrary;

namespace BookLibraryUnitTests
{
    public class SearchTests
    {
        private static BookService ServiceWithSampleBooks()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "The Hobbit", author: "Tolkien"));
            service.Add(TestHelpers.Book(title: "The Lord of the Rings", author: "Tolkien"));
            service.Add(TestHelpers.Book(title: "Dune", author: "Herbert"));
            return service;
        }

        [Fact]
        public void Search_ExactTitle_ReturnsMatchingBook()
        {
            var service = ServiceWithSampleBooks();

            var results = service.Search("Dune").ToList();

            Assert.Single(results);
            Assert.Equal("Dune", results[0].Title);
        }

        [Fact]
        public void Search_PartialTitle_ReturnsAllMatches()
        {
            var service = ServiceWithSampleBooks();

            var results = service.Search("The").ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, b => b.Title == "The Hobbit");
            Assert.Contains(results, b => b.Title == "The Lord of the Rings");
        }

        [Theory]
        [InlineData("hobbit")]
        [InlineData("HOBBIT")]
        [InlineData("HoBBiT")]
        public void Search_IsCaseInsensitive(string term)
        {
            var service = ServiceWithSampleBooks();

            var results = service.Search(term).ToList();

            Assert.Single(results);
            Assert.Equal("The Hobbit", results[0].Title);
        }

        [Fact]
        public void Search_EmptyString_ReturnsAllBooks()
        {
            var service = ServiceWithSampleBooks();

            var results = service.Search(string.Empty).ToList();

            Assert.Equal(3, results.Count);
        }

        [Fact]
        public void Search_NoMatch_ReturnsEmpty()
        {
            var service = ServiceWithSampleBooks();

            var results = service.Search("Nonexistent").ToList();

            Assert.Empty(results);
        }

        [Fact]
        public void Search_OnEmptyLibrary_ReturnsEmpty()
        {
            var service = new BookService();

            var results = service.Search("anything").ToList();

            Assert.Empty(results);
        }

        [Fact]
        public void Search_NullTerm_ThrowsArgumentNullException()
        {
            var service = ServiceWithSampleBooks();

            Assert.Throws<ArgumentNullException>(() => service.Search(null!).ToList());
        }
    }
}
