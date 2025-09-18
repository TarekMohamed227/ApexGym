using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces
{
    public interface IWorkoutClassRepository
    {
        Task<WorkoutClass> GetByIdAsync(int id);
        Task<List<WorkoutClass>> GetAllAsync();
        Task<WorkoutClass> AddAsync(WorkoutClass workoutClass);
        Task UpdateAsync(WorkoutClass workoutClass);
        Task DeleteAsync(WorkoutClass workoutClass);

        // Special method to get classes with included details (for better performance)
        Task<WorkoutClass> GetByIdWithDetailsAsync(int id);
    }
}
