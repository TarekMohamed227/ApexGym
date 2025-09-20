using ApexGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance> GetByIdAsync(int memberId, int workoutClassId);
        Task<List<Attendance>> GetAllAsync();
        Task<Attendance> AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task DeleteAsync(Attendance attendance);

        // Special methods for business logic
        Task<bool> IsMemberRegisteredAsync(int memberId, int workoutClassId);
        Task<int> GetAttendanceCountForClassAsync(int workoutClassId);
        Task<List<Attendance>> GetAttendancesByMemberAsync(int memberId);
        Task<List<Attendance>> GetAttendancesByClassAsync(int workoutClassId);
    }
}
