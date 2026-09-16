using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BookLibrary.DTOs
{
    [XmlRoot("books")]
    public class BooksDto
    {
        [XmlElement("book")]
        public List<BookDto> Items { get; set; } = null!;
    }
}
