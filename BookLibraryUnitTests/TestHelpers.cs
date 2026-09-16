using System.Text;
using BookLibrary;
using BookLibrary.DTOs;

namespace BookLibraryUnitTests
{
    internal static class TestHelpers
    {
        public static BookDto Book(
            string title = "The Hobbit",
            string author = "Tolkien",
            string numberOfPages = "310")
        {
            return new BookDto
            {
                Title = title,
                Author = author,
                NumberOfPages = numberOfPages,
            };
        }

        public static string BooksXml(params string[] bookElements)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<books>");
            foreach (var element in bookElements)
            {
                sb.Append(element);
            }

            sb.Append("</books>");
            return sb.ToString();
        }

        public static string BookElement(
            string title = "The Hobbit",
            string author = "Tolkien",
            string numberOfPages = "310")
        {
            return "<book>" +
                   $"<title>{title}</title>" +
                   $"<author>{author}</author>" +
                   $"<numberOfPages>{numberOfPages}</numberOfPages>" +
                   "</book>";
        }

        public static MemoryStream AsStream(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        /// Reads the service's current contents through the public API only.
        /// Search("") matches every non-null title and preserves list order, so it
        /// faithfully reflects the (now private) backing collection.
        /// </summary>
        public static List<BookDto> AllBooks(IBookService service)
        {
            return service.Search(string.Empty).ToList();
        }
    }
}
