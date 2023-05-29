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
    public class MemberRepository : IRepository<Member>, IAuthRepository
    {
        private readonly FStoreDBContext _context;

        public MemberRepository(FStoreDBContext context)
        {
            _context = context;
        }

        public async Task<Member> Add(Member _object)
        {
            _context.Members.Add(_object);
            return null;
        }

        public async Task<int> Remove(Member member)
        {
            _context.Members.Remove(member);
            return 1;
        }

        public async Task<ActionResult<IEnumerable<Member>>> List()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<Member> FindAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);
            return member;
        }

        public async Task<int> Update(int id, Member _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            return 1;
        }

        public bool Exists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Member CheckUserInDatabase(LoginModel loginModel)
        {
            var user = _context.Members.Where(u => u.Email == loginModel.Email && u.Password == loginModel.Password).FirstOrDefault();
            return user;
        }
    }
}
