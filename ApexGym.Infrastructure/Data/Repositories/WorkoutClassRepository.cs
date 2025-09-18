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
    public class WorkoutClassRepository : IWorkoutClassRepository
    {
        private readonly AppDbContext _dbContext;

        public WorkoutClassRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WorkoutClass> GetByIdAsync(int id)
        {
            return await _dbContext.WorkoutClasses.FindAsync(id);
        }

        public async Task<List<WorkoutClass>> GetAllAsync()
        {
            return await _dbContext.WorkoutClasses.ToListAsync();
        }

        // Special method to get class with trainer details included
        public async Task<WorkoutClass> GetByIdWithDetailsAsync(int id)
        {
            return await _dbContext.WorkoutClasses
                .Include(wc => wc.Trainer) // Include trainer details
                .FirstOrDefaultAsync(wc => wc.Id == id);
        }

        public async Task<WorkoutClass> AddAsync(WorkoutClass workoutClass)
        {
            _dbContext.WorkoutClasses.Add(workoutClass);
            await _dbContext.SaveChangesAsync();
            return workoutClass;
        }
        
        public async Task UpdateAsync(WorkoutClass workoutClass)
        {
            _dbContext.Entry(workoutClass).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(WorkoutClass workoutClass)
        {
            _dbContext.WorkoutClasses.Remove(workoutClass);
            await _dbContext.SaveChangesAsync();
        }
    }
}
