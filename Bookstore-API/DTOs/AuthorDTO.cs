using System;
using System.Collections.Generic;

namespace Bookstore_API.DTOs
{
    public class AuthorDTO
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Bio { get; set; }

        public IList<BookDTO> Books { get; set; }
    }
}
