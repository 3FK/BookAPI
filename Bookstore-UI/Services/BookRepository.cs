using System;
using System.Net.Http;
using Blazored.LocalStorage;
using Bookstore_UI.Contracts;
using Bookstore_UI.Models;

namespace Bookstore_UI.Services
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;

        public BookRepository(IHttpClientFactory client, ILocalStorageService localStorage) : base(client, localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }
    }
}
