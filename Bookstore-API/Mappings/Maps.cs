using System;
using AutoMapper;
using Bookstore_API.Data;
using Bookstore_API.DTOs;

namespace Bookstore_API.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap(); 
        }
    }
}
