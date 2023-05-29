using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRepository<T>
    {
        public Task<T> Add(T _object);
        public Task<int> Remove(T _object);
        public Task<int> Update(int id, T _object);
        public Task<ActionResult<IEnumerable<T>>> List();
        public Task<T> FindAsync(int Id);
        public bool Exists(int id);
        public Task<int> SaveChangesAsync();
    }
}
