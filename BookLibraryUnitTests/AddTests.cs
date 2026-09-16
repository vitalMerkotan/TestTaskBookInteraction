using BookLibrary;

namespace BookLibraryUnitTests
{
    public class AddTests
    {
        [Fact]
        public void Add_ValidBook_AppendsToBooks()
        {
            var service = new BookService();
            var book = TestHelpers.Book();

            service.Add(book);

            var books = TestHelpers.AllBooks(service);
            Assert.Single(books);
            Assert.Same(book, books[0]);
        }

        [Fact]
        public void Add_MultipleBooks_PreservesInsertionOrder()
        {
            var service = new BookService();
            var first = TestHelpers.Book(title: "A");
            var second = TestHelpers.Book(title: "B");
            var third = TestHelpers.Book(title: "C");

            service.Add(first);
            service.Add(second);
            service.Add(third);

            var books = TestHelpers.AllBooks(service);
            Assert.Equal(3, books.Count);
            Assert.Same(first, books[0]);
            Assert.Same(second, books[1]);
            Assert.Same(third, books[2]);
        }

        [Fact]
        public void Add_NullBook_ThrowsArgumentNullException()
        {
            var service = new BookService();

            var ex = Assert.Throws<ArgumentNullException>(() => service.Add(null!));

            Assert.Equal("book", ex.ParamName);
            Assert.Empty(TestHelpers.AllBooks(service));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Add_EmptyOrWhitespaceTitle_ThrowsArgumentException(string? title)
        {
            var service = new BookService();
            var book = TestHelpers.Book(title: title!);

            var ex = Assert.Throws<ArgumentException>(() => service.Add(book));

            Assert.Equal("book", ex.ParamName);
            Assert.StartsWith("Book title cannot be empty.", ex.Message);
            Assert.Empty(TestHelpers.AllBooks(service));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Add_EmptyOrWhitespaceAuthor_ThrowsArgumentException(string? author)
        {
            var service = new BookService();
            var book = TestHelpers.Book(author: author!);

            var ex = Assert.Throws<ArgumentException>(() => service.Add(book));

            Assert.Equal("book", ex.ParamName);
            Assert.StartsWith("Book author cannot be empty.", ex.Message);
            Assert.Empty(TestHelpers.AllBooks(service));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Add_EmptyOrWhitespaceNumberOfPages_ThrowsArgumentException(string? numberOfPages)
        {
            var service = new BookService();
            var book = TestHelpers.Book(numberOfPages: numberOfPages!);

            var ex = Assert.Throws<ArgumentException>(() => service.Add(book));

            Assert.Equal("book", ex.ParamName);
            Assert.StartsWith("Book number of pages cannot be empty.", ex.Message);
            Assert.Empty(TestHelpers.AllBooks(service));
        }
    }
}
