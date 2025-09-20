using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces
{
    public interface IWorkoutClassRepository : IGenericRepository<WorkoutClass>
    {
        Task<WorkoutClass> GetByIdWithDetailsAsync(int id);
    }
}
