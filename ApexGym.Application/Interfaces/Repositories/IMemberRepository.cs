using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces.Repositories
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        // Only include SPECIAL methods, not basic CRUD
        Task<bool> IsEmailUniqueAsync(string email);

    }



}
