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
    public class OrderDetailRepository : IRepository<OrderDetail>
    {
        private readonly FStoreDBContext _context;

        public OrderDetailRepository(FStoreDBContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail> Add(OrderDetail _object)
        {
            _context.OrderDetails.Add(_object);
            return null;
        }

        public async Task<int> Remove(OrderDetail _object)
        {
            _context.OrderDetails.Remove(_object);
            return 1;
        }

        public async Task<ActionResult<IEnumerable<OrderDetail>>> List()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        public async Task<OrderDetail> FindAsync(int id)
        {
            var product = await _context.OrderDetails.FindAsync(id);
            return product;
        }

        public async Task<int> Update(int id, OrderDetail _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            return 1;
        }

        public bool Exists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderId == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
