using ApexGym.Application.Interfaces;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using ApexGym.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApexGym.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        // Generic Repository instances
        private IGenericRepository<Member> _members;
        private IGenericRepository<Trainer> _trainers;
        private IGenericRepository<WorkoutClass> _workoutClasses;
        private IGenericRepository<MembershipPlan> _membershipPlans;

        // Specific Repository instances
        private IMemberRepository _memberRepository;
        private ITrainerRepository _trainerRepository;
        private IWorkoutClassRepository _workoutClassRepository;
        private IAttendanceRepository _attendances;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Generic Repository Properties
        public IGenericRepository<Member> Members =>
            _members ??= new GenericRepository<Member>(_context);

        public IGenericRepository<Trainer> Trainers =>
            _trainers ??= new GenericRepository<Trainer>(_context);

        public IGenericRepository<WorkoutClass> WorkoutClasses =>
            _workoutClasses ??= new GenericRepository<WorkoutClass>(_context);

        public IGenericRepository<MembershipPlan> MembershipPlans =>
            _membershipPlans ??= new GenericRepository<MembershipPlan>(_context);

        // Specific Repository Properties (NEW)
        public IMemberRepository MemberRepository =>
            _memberRepository ??= new MemberRepository(_context);

        public ITrainerRepository TrainerRepository =>
            _trainerRepository ??= new TrainerRepository(_context);

        public IWorkoutClassRepository WorkoutClassRepository =>
            _workoutClassRepository ??= new WorkoutClassRepository(_context);

        public IAttendanceRepository Attendances =>
            _attendances ??= new AttendanceRepository(_context);

        // MAIN METHOD: Complete the unit of work (save all changes)
        public async Task<int> CompleteAsync()
        {
            try
            {
                // Save all changes to database
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                throw new Exception("Failed to save changes to database", ex);
            }
        }

        // Alternative method: Returns boolean for success
        public async Task<bool> SaveChangesAsync()
        {
            return await CompleteAsync() > 0;
        }

        // Advanced transaction support
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            
            GC.SuppressFinalize(this);
        }
    }
}