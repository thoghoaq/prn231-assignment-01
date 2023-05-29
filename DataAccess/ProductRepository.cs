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
    public class ProductRepository : IRepository<Product>
    {
        private readonly FStoreDBContext _context;

        public ProductRepository(FStoreDBContext context)
        {
            _context = context;
        }

        public async Task<Product> Add(Product _object)
        {
            _context.Products.Add(_object);
            return null;
        }

        public async Task<int> Remove(Product _object)
        {
            _context.Products.Remove(_object);
            return 1;
        }

        public async Task<ActionResult<IEnumerable<Product>>> List()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> FindAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task<int> Update(int id, Product _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            return 1;
        }

        public bool Exists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
