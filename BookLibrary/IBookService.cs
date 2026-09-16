using BookLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLibrary
{
    public interface IBookService
    {
        public void Add(BookDto book);

        public void Load(string filePath);

        public void Load(Stream stream);

        public void Save(string savePath);

        public void Save(Stream stream);

        public IEnumerable<BookDto> Search(string searchTerm);

        public void Sort();
    }
}
