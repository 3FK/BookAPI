using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore_UI.Contracts
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> Get(String url, int id);
        Task<IList<T>> Get(String url);
        Task<bool> Create(String url, T obj);
        Task<bool> Update(String url, T obj, int id);
        Task<bool> Delete(String url, int id);

    }
}
