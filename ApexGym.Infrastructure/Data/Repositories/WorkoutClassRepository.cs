using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data.Repositories
{
    public class WorkoutClassRepository : GenericRepository<WorkoutClass>, IWorkoutClassRepository
{
    public WorkoutClassRepository(AppDbContext context) : base(context)
    {
    }

   

        public async Task<WorkoutClass> GetByIdAsync(int id)
        {
            return await _context.WorkoutClasses.FindAsync(id);
        }

        public async Task<List<WorkoutClass>> GetAllAsync()
        {
            return await _context.WorkoutClasses.Include(w=>w.Trainer).ToListAsync();
        }

        // Special method to get class with trainer details included
        public async Task<WorkoutClass> GetByIdWithDetailsAsync(int id)
        {
            return await _context.WorkoutClasses
                .Include(wc => wc.Trainer) // Include trainer details
                .FirstOrDefaultAsync(wc => wc.Id == id);
        }

        public async Task<WorkoutClass> AddAsync(WorkoutClass workoutClass)
        {
            _context.WorkoutClasses.Add(workoutClass);
            await _context.SaveChangesAsync();
            return workoutClass;
        }
        
        public async Task UpdateAsync(WorkoutClass workoutClass)
        {
            _context.Entry(workoutClass).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(WorkoutClass workoutClass)
        {
            _context.WorkoutClasses.Remove(workoutClass);
            await _context.SaveChangesAsync();
        }
    }
}
