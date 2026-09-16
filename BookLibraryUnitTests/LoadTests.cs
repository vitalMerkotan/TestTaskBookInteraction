using BookLibrary;

namespace BookLibraryUnitTests
{
    public class LoadTests
    {
        [Fact]
        public void Load_Path_NonExistentFile_ThrowsFileNotFoundException()
        {
            var service = new BookService();
            var missingPath = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.xml");

            var ex = Assert.Throws<FileNotFoundException>(() => service.Load(missingPath));

            Assert.Equal(missingPath, ex.FileName);
        }

        [Fact]
        public void Load_Path_ValidFile_PopulatesBooks()
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "Dune", author: "Herbert", numberOfPages: "412"));
            var path = Path.Combine(Path.GetTempPath(), $"books_{Guid.NewGuid():N}.xml");
            File.WriteAllText(path, xml);

            try
            {
                service.Load(path);

                var books = TestHelpers.AllBooks(service);
                Assert.Single(books);
                Assert.Equal("Dune", books[0].Title);
                Assert.Equal("Herbert", books[0].Author);
                Assert.Equal("412", books[0].NumberOfPages);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_Stream_NonReadableStream_ThrowsArgumentException()
        {
            var service = new BookService();
            var stream = new MemoryStream();
            stream.Dispose();

            var ex = Assert.Throws<ArgumentException>(() => service.Load(stream));

            Assert.Equal("stream", ex.ParamName);
        }

        [Fact]
        public void Load_Stream_MalformedXml_ThrowsInvalidDataException()
        {
            var service = new BookService();
            using var stream = TestHelpers.AsStream("<books><book>not closed properly");

            var ex = Assert.Throws<InvalidDataException>(() => service.Load(stream));

            Assert.Equal("The XML file has an invalid structure.", ex.Message);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Load_Stream_ValidXml_SetsBooks()
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "1984", author: "Orwell", numberOfPages: "328"),
                TestHelpers.BookElement(title: "Animal Farm", author: "Orwell", numberOfPages: "112"));
            using var stream = TestHelpers.AsStream(xml);

            service.Load(stream);

            var books = TestHelpers.AllBooks(service);
            Assert.Equal(2, books.Count);
            Assert.Equal("1984", books[0].Title);
            Assert.Equal("Animal Farm", books[1].Title);
        }

        [Fact]
        public void Load_Stream_ValidXml_ReplacesExistingBooks()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Stale"));

            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "Fresh", author: "Author", numberOfPages: "100"));
            using var stream = TestHelpers.AsStream(xml);

            service.Load(stream);

            var books = TestHelpers.AllBooks(service);
            Assert.Single(books);
            Assert.Equal("Fresh", books[0].Title);
        }

        [Fact]
        public void Load_Stream_EmptyBooksRoot_LoadsEmptyCollection()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Stale"));
            using var stream = TestHelpers.AsStream("<books />");

            service.Load(stream);

            Assert.Empty(TestHelpers.AllBooks(service));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Load_Stream_EmptyOrWhitespaceTitle_ThrowsInvalidDataException(string title)
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: title, author: "Author", numberOfPages: "100"));
            using var stream = TestHelpers.AsStream(xml);

            var ex = Assert.Throws<InvalidDataException>(() => service.Load(stream));

            Assert.Equal("Book at index 0 has an empty title.", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Load_Stream_EmptyOrWhitespaceAuthor_ThrowsInvalidDataException(string author)
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "Title", author: author, numberOfPages: "100"));
            using var stream = TestHelpers.AsStream(xml);

            var ex = Assert.Throws<InvalidDataException>(() => service.Load(stream));

            Assert.Equal("Book at index 0 has an empty author.", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Load_Stream_EmptyOrWhitespaceNumberOfPages_ThrowsInvalidDataException(string pages)
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "Title", author: "Author", numberOfPages: pages));
            using var stream = TestHelpers.AsStream(xml);

            var ex = Assert.Throws<InvalidDataException>(() => service.Load(stream));

            Assert.Equal("Book at index 0 has an empty numberOfPages.", ex.Message);
        }

        [Fact]
        public void Load_Stream_ReportsIndexOfInvalidBook()
        {
            var service = new BookService();
            var xml = TestHelpers.BooksXml(
                TestHelpers.BookElement(title: "Valid", author: "Author", numberOfPages: "100"),
                TestHelpers.BookElement(title: "", author: "Author", numberOfPages: "100"));
            using var stream = TestHelpers.AsStream(xml);

            var ex = Assert.Throws<InvalidDataException>(() => service.Load(stream));

            Assert.Equal("Book at index 1 has an empty title.", ex.Message);
        }
    }
}
