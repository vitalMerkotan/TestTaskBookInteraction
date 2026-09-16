using BookLibrary;

namespace BookLibraryUnitTests
{
    public class SortTests
    {
        [Fact]
        public void Sort_OrdersByAuthorAscending()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Dune", author: "Herbert"));
            service.Add(TestHelpers.Book(title: "The Hobbit", author: "Tolkien"));
            service.Add(TestHelpers.Book(title: "Foundation", author: "Asimov"));

            service.Sort();

            Assert.Equal(new[] { "Asimov", "Herbert", "Tolkien" },
                TestHelpers.AllBooks(service).Select(b => b.Author).ToArray());
        }

        [Fact]
        public void Sort_SameAuthor_OrdersByTitleAscending()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "The Lord of the Rings", author: "Tolkien"));
            service.Add(TestHelpers.Book(title: "The Hobbit", author: "Tolkien"));
            service.Add(TestHelpers.Book(title: "The Silmarillion", author: "Tolkien"));

            service.Sort();

            Assert.Equal(
                new[] { "The Hobbit", "The Lord of the Rings", "The Silmarillion" },
                TestHelpers.AllBooks(service).Select(b => b.Title).ToArray());
        }

        [Fact]
        public void Sort_UsesAuthorAsPrimaryAndTitleAsSecondaryKey()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Zeta", author: "Brown"));
            service.Add(TestHelpers.Book(title: "Alpha", author: "Brown"));
            service.Add(TestHelpers.Book(title: "Beta", author: "Adams"));

            service.Sort();

            var books = TestHelpers.AllBooks(service);
            Assert.Equal("Adams", books[0].Author);
            Assert.Equal("Beta", books[0].Title);
            Assert.Equal("Brown", books[1].Author);
            Assert.Equal("Alpha", books[1].Title);
            Assert.Equal("Brown", books[2].Author);
            Assert.Equal("Zeta", books[2].Title);
        }

        [Fact]
        public void Sort_EmptyLibrary_RemainsEmpty()
        {
            var service = new BookService();

            service.Sort();

            Assert.Empty(TestHelpers.AllBooks(service));
        }

        [Fact]
        public void Sort_SingleBook_RemainsUnchanged()
        {
            var service = new BookService();
            var book = TestHelpers.Book(title: "Solo", author: "Author");
            service.Add(book);

            service.Sort();

            var books = TestHelpers.AllBooks(service);
            Assert.Single(books);
            Assert.Same(book, books[0]);
        }

        [Fact]
        public void Sort_AlreadySorted_PreservesOrder()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Foundation", author: "Asimov"));
            service.Add(TestHelpers.Book(title: "Dune", author: "Herbert"));

            service.Sort();

            var books = TestHelpers.AllBooks(service);
            Assert.Equal("Asimov", books[0].Author);
            Assert.Equal("Herbert", books[1].Author);
        }
    }
}
