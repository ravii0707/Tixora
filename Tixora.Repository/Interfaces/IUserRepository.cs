using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.Entities;

namespace Tixora.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<TbUser> AddAsync(TbUser user);
        Task<TbUser?> GetByIdAsync(int id);
        Task<TbUser?> GetByEmailAsync(string email);
        Task<IEnumerable<TbUser>> GetAllAsync();
        Task<bool> EmailExistsAsync(string email);

        Task<TbUser> GetByPhoneAsync(string phone);
        Task<bool> PhoneExistsAsync(string phone);

    }
}
