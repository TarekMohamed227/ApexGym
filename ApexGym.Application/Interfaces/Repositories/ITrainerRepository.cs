using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces.Repositories
{
    public interface ITrainerRepository : IGenericRepository<Trainer>
    {
        Task<Trainer> GetTrainerWithDetailsAsync(int id);
    }
}
