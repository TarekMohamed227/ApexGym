using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces.Repositories
{
    public interface ITrainerRepository
    {
        Task<Trainer> GetByIdAsync(int id);
        Task<List<Trainer>> GetAllAsync();
        Task<Trainer> AddAsync(Trainer trainer);
        Task UpdateAsync(Trainer trainer); // Fixed typo here too if needed
        Task DeleteAsync(Trainer trainer);
        Task<Trainer> GetTrainerWithDetailsAsync(int id); // Async suffix added
    }
}
