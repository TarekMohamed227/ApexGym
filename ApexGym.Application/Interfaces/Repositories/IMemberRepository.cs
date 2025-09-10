using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces.Repositories
{
    public interface IMemberRepository
    {
        Task<Member> GetByIdAsync(int id);
        Task<List<Member>> GetAllAsync();
        Task<Member> AddAsync(Member member);
        Task UpdateAsync(Member member);
        Task DeleteAsync(Member member);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
