using BookLibrary.DTOs;
using System.Xml.Serialization;

namespace BookLibrary
{
    public class BookService : IBookService
    {
        private List<BookDto> Books = [];

        public void Add(BookDto book)
        {
            ArgumentNullException.ThrowIfNull(book);

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                throw new ArgumentException("Book title cannot be empty.", nameof(book));
            }

            if (string.IsNullOrWhiteSpace(book.Author))
            {
                throw new ArgumentException("Book author cannot be empty.", nameof(book));
            }

            if (string.IsNullOrWhiteSpace(book.NumberOfPages))
            {
                throw new ArgumentException("Book number of pages cannot be empty.", nameof(book));
            }

            Books.Add(book);
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Book file was not found.", filePath);
            }

            using var stream = File.OpenRead(filePath);
            Load(stream);
        }

        public void Load(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException(
                    "The stream must be readable.",
                    nameof(stream));
            }

            var serializer = new XmlSerializer(typeof(BooksDto));

            BooksDto data;

            try
            {
                data = (BooksDto)serializer.Deserialize(stream)!;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidDataException(
                    "The XML file has an invalid structure.",
                    ex);
            }

            Validate(data);

            Books = data.Items;
        }

        public void Save(string savePath)
        {
            var directory = Path.GetDirectoryName(savePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = File.Create(savePath);
            Save(stream);
        }

        public void Save(Stream stream)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("The stream must be writable.", nameof(stream));
            }
            var serializer = new XmlSerializer(typeof(BooksDto));
            var data = new BooksDto { Items = Books };
            serializer.Serialize(stream, data);
        }

        public IEnumerable<BookDto> Search(string searchTerm)
        {
            return Books.Where(s => s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void Sort()
        {
            Books = Books.OrderBy(s => s.Author).ThenBy(s => s.Title).ToList(); 
        }

        private static void Validate(BooksDto data)
        {
            if (data.Items is null)
            {
                throw new InvalidDataException(
                    "The books collection cannot be null.");
            }

            for (var i = 0; i < data.Items.Count; i++)
            {
                var book = data.Items[i];

                if (book is null)
                {
                    throw new InvalidDataException(
                        $"Book at index {i} cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(book.Title))
                {
                    throw new InvalidDataException(
                        $"Book at index {i} has an empty title.");
                }

                if (string.IsNullOrWhiteSpace(book.Author))
                {
                    throw new InvalidDataException(
                        $"Book at index {i} has an empty author.");
                }

                if (string.IsNullOrWhiteSpace(book.NumberOfPages))
                {
                    throw new InvalidDataException(
                        $"Book at index {i} has an empty numberOfPages.");
                }
            }
        }
    }
}
