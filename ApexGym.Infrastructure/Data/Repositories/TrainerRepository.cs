using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
// REMOVE using System.Data.Entity; - This is the main cause of your error!

namespace ApexGym.Infrastructure.Data.Repositories
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _context;

        public TrainerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Trainer> AddAsync(Trainer trainer)
        {
            // Use Add instead of AddAsync for better performance in most cases
            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();
            return trainer;
        }

        public async Task DeleteAsync(Trainer trainer)
        {
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Trainer>> GetAllAsync()
        {
            return await _context.Trainers.ToListAsync();
        }

        public async Task<Trainer> GetByIdAsync(int id)
        {
            return await _context.Trainers.FindAsync(id);
        }

        public async Task<Trainer> GetTrainerWithDetailsAsync(int id) // Fixed method name to match interface
        {
            return await _context.Trainers
                .Include(t => t.WorkoutClasses) // EF Core Include
                .FirstOrDefaultAsync(t => t.Id == id); // lowercase 'f' in FirstOrDefaultAsync
        }

        public async Task UpdateAsync(Trainer trainer) // Fixed typo in method name (UpdateAsyc -> UpdateAsync)
        {
            _context.Entry(trainer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}