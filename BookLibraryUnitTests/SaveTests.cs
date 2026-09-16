using BookLibrary;

namespace BookLibraryUnitTests
{
    public class SaveTests
    {
        [Fact]
        public void Save_Stream_NonWritableStream_ThrowsArgumentException()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book());

            using var stream = new MemoryStream(new byte[16], writable: false);

            var ex = Assert.Throws<ArgumentException>(() => service.Save(stream));

            Assert.Equal("stream", ex.ParamName);
        }

        [Fact]
        public void Save_Stream_WritesSerializedContent()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book(title: "Neuromancer", author: "Gibson", numberOfPages: "271"));

            using var stream = new MemoryStream();
            service.Save(stream);

            var xml = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            Assert.Contains("<books", xml);
            Assert.Contains("<title>Neuromancer</title>", xml);
            Assert.Contains("<author>Gibson</author>", xml);
            Assert.Contains("<numberOfPages>271</numberOfPages>", xml);
        }

        [Fact]
        public void Save_Stream_ThenLoad_RoundTripsBooks()
        {
            var saver = new BookService();
            saver.Add(TestHelpers.Book(title: "A", author: "AuthorA", numberOfPages: "10"));
            saver.Add(TestHelpers.Book(title: "B", author: "AuthorB", numberOfPages: "20"));

            using var stream = new MemoryStream();
            saver.Save(stream);
            stream.Position = 0;

            var loader = new BookService();
            loader.Load(stream);

            var books = TestHelpers.AllBooks(loader);
            Assert.Equal(2, books.Count);
            Assert.Equal("A", books[0].Title);
            Assert.Equal("AuthorA", books[0].Author);
            Assert.Equal("10", books[0].NumberOfPages);
            Assert.Equal("B", books[1].Title);
        }

        [Fact]
        public void Save_Stream_EmptyBooks_WritesValidDocument()
        {
            var service = new BookService();

            using var stream = new MemoryStream();
            service.Save(stream);

            var xml = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            Assert.Contains("<books", xml);
        }

        [Fact]
        public void Save_Path_BareFileName_WritesFileWithoutDirectory()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book());
            var fileName = $"books_{Guid.NewGuid():N}.xml";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            try
            {
                service.Save(fileName);

                Assert.True(File.Exists(fullPath));
                Assert.Contains("<title>The Hobbit</title>", File.ReadAllText(fullPath));
            }
            finally
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        [Fact]
        public void Save_Path_NestedDirectory_CreatesDirectoryAndWritesFile()
        {
            var service = new BookService();
            service.Add(TestHelpers.Book());
            var root = Path.Combine(Path.GetTempPath(), $"booktests_{Guid.NewGuid():N}");
            var savePath = Path.Combine(root, "nested", "books.xml");

            try
            {
                service.Save(savePath);

                Assert.True(File.Exists(savePath));
                Assert.Contains("<title>The Hobbit</title>", File.ReadAllText(savePath));
            }
            finally
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, recursive: true);
                }
            }
        }

        [Fact]
        public void Save_Path_ThenLoad_RoundTripsBooks()
        {
            var saver = new BookService();
            saver.Add(TestHelpers.Book(title: "Roundtrip", author: "Author", numberOfPages: "42"));
            var path = Path.Combine(Path.GetTempPath(), $"books_{Guid.NewGuid():N}.xml");

            try
            {
                saver.Save(path);

                var loader = new BookService();
                loader.Load(path);

                var books = TestHelpers.AllBooks(loader);
                Assert.Single(books);
                Assert.Equal("Roundtrip", books[0].Title);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
