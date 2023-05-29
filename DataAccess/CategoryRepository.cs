using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly FStoreDBContext _context;

        public CategoryRepository(FStoreDBContext context)
        {
            _context = context;
        }

        public async Task<Category> Add(Category _object)
        {
            _context.Categories.Add(_object);
            return null;
        }

        public async Task<int> Remove(Category _object)
        {
            _context.Categories.Remove(_object);
            return 1;
        }

        public async Task<ActionResult<IEnumerable<Category>>> List()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> FindAsync(int id)
        {
            var product = await _context.Categories.FindAsync(id);
            return product;
        }

        public async Task<int> Update(int id, Category _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            return 1;
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
