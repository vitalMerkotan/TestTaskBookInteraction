using System.Xml.Serialization;

namespace BookLibrary.DTOs
{
    public class BookDto
    {
        [XmlElement("title")]
        public string Title { get; set; } = null!;

        [XmlElement("author")]
        public string Author { get; set; } = null!;

        [XmlElement("numberOfPages")]
        public string NumberOfPages { get; set; } = null!;
    }
}
