using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Infrastructure.Data.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> GetByIdAsync(int memberId, int workoutClassId)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.MemberId == memberId && a.WorkoutClassId == workoutClassId);
        }

        public async Task<List<Attendance>> GetAllAsync()
        {
            return await _context.Attendances.Include(a => a.Member).Include(a => a.WorkoutClass).ToListAsync();
        }

        public async Task<Attendance> AddAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Entry(attendance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Attendance attendance)
        {
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsMemberRegisteredAsync(int memberId, int workoutClassId)
        {
            return await _context.Attendances
                .AnyAsync(a => a.MemberId == memberId && a.WorkoutClassId == workoutClassId);
        }

        public async Task<int> GetAttendanceCountForClassAsync(int workoutClassId)
        {
            return await _context.Attendances
                .CountAsync(a => a.WorkoutClassId == workoutClassId);
        }

        public async Task<List<Attendance>> GetAttendancesByMemberAsync(int memberId)
        {
            return await _context.Attendances
                .Where(a => a.MemberId == memberId)
                .Include(a => a.WorkoutClass)
                .ThenInclude(wc => wc.Trainer)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetAttendancesByClassAsync(int workoutClassId)
        {
            return await _context.Attendances
                .Where(a => a.WorkoutClassId == workoutClassId)
                .Include(a => a.Member)
                .ToListAsync();
        }
    }
}
