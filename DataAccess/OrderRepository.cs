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
    public class OrderRepository : IRepository<Order>
    {
        private readonly FStoreDBContext _context;

        public OrderRepository(FStoreDBContext context)
        {
            _context = context;
        }

        public async Task<Order> Add(Order _object)
        {
            _context.Orders.Add(_object);
            return null;
        }

        public async Task<int> Remove(Order _object)
        {
            _context.Orders.Remove(_object);
            return 1;
        }

        public async Task<ActionResult<IEnumerable<Order>>> List()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> FindAsync(int id)
        {
            var product = await _context.Orders.FindAsync(id);
            return product;
        }

        public async Task<int> Update(int id, Order _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            return 1;
        }

        public bool Exists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
