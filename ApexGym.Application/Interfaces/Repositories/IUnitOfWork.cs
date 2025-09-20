using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Generic Repositories (for basic CRUD)
        IGenericRepository<Member> Members { get; }
        IGenericRepository<Trainer> Trainers { get; }
        IGenericRepository<WorkoutClass> WorkoutClasses { get; }
        IGenericRepository<MembershipPlan> MembershipPlans { get; }

        // Specific Repositories (for custom methods)
        IMemberRepository MemberRepository { get; } // NEW
        ITrainerRepository TrainerRepository { get; } // NEW
        IWorkoutClassRepository WorkoutClassRepository { get; } // NEW
        IAttendanceRepository Attendances { get; }

        // Transaction management
        Task<int> CompleteAsync();
        Task<bool> SaveChangesAsync();

        // Optional: Advanced transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}