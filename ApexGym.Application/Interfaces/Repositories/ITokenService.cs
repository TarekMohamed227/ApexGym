using ApexGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Interfaces.Repositories
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
