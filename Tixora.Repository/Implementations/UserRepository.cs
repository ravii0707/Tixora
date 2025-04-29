using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Context;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;

namespace Tixora.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TbUser> AddAsync(TbUser user)
        {
            await _context.TbUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TbUser?> GetByIdAsync(int id)
        {
            return await _context.TbUsers.FindAsync(id);
        }

        public async Task<TbUser?> GetByEmailAsync(string email)
        {
    var normalizedEmail = email.Trim().ToLower();
    return await _context.TbUsers
        .FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == normalizedEmail);
        }

        public async Task<IEnumerable<TbUser>> GetAllAsync()
        {
            return await _context.TbUsers.ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.TbUsers.AnyAsync(u => u.Email == email);
        }

        public async Task<TbUser> GetByPhoneAsync(string phone) 
        {
            return await _context.TbUsers.FirstOrDefaultAsync(u => u.Phone == phone);
        }
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.TbUsers.AnyAsync(u => u.Phone == phone);
        }
    }
}