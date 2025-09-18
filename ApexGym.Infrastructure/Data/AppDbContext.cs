using ApexGym.Domain.Entities;
using ApexGym.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data;

// Ensure it inherits from IdentityDbContext<User, IdentityRole<int>, int>
public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Existing DbSet
    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>(); // Added for completeness

    // New DbSets for our new entities
    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<WorkoutClass> WorkoutClasses => Set<WorkoutClass>();
    public DbSet<Attendance> Attendances => Set<Attendance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // This is necessary for Identity configurations

        // Apply configurations
        builder.ApplyConfiguration(new MembershipPlanConfiguration());

        // --- NEW CONFIGURATIONS START HERE ---

        // 1. Configure the composite PRIMARY KEY for Attendance
        // This is the most important configuration for this step.
        // It says: "The primary key of the Attendance table is made up of BOTH MemberId AND WorkoutClassId."
        builder.Entity<Attendance>()
            .HasKey(a => new { a.MemberId, a.WorkoutClassId }); // <-- Composite Key

        // 2. Configure the relationship between Attendance and Member
        // One Member can have many Attendance records
        builder.Entity<Attendance>()
            .HasOne(a => a.Member) // One Attendance record has ONE Member
            .WithMany() // One Member can have MANY Attendance records
            .HasForeignKey(a => a.MemberId) // The foreign key in Attendance is MemberId
            .OnDelete(DeleteBehavior.Cascade); // If a Member is deleted, delete all their Attendance records

        // 3. Configure the relationship between Attendance and WorkoutClass
        // One WorkoutClass can have many Attendance records
        builder.Entity<Attendance>()
            .HasOne(a => a.WorkoutClass) // One Attendance record has ONE WorkoutClass
            .WithMany(wc => wc.Attendances) // One WorkoutClass can have MANY Attendance records
            .HasForeignKey(a => a.WorkoutClassId) // The foreign key in Attendance is WorkoutClassId
            .OnDelete(DeleteBehavior.Cascade); // If a WorkoutClass is deleted, delete all its Attendance records

        // 4. Configure the relationship between WorkoutClass and Trainer
        // This enforces the rule: "A WorkoutClass MUST HAVE a Trainer"
        builder.Entity<WorkoutClass>()
            .HasOne(wc => wc.Trainer) // One WorkoutClass has ONE Trainer
            .WithMany(t => t.WorkoutClasses) // One Trainer can have MANY WorkoutClasses
            .HasForeignKey(wc => wc.TrainerId) // The foreign key in WorkoutClass is TrainerId
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Trainer if they have classes assigned

        // 5. (Optional) Seed some initial Trainer data
        builder.Entity<Trainer>().HasData(
            new Trainer { Id = 1, FirstName = "John", LastName = "Doe", Specialization = "Yoga", YearsOfExperience = 5, Bio = "Expert in Vinyasa and Hatha yoga." },
            new Trainer { Id = 2, FirstName = "Sarah", LastName = "Smith", Specialization = "Weightlifting", YearsOfExperience = 8, Bio = "National level weightlifting coach." },
            new Trainer { Id = 3, FirstName = "Mike", LastName = "Johnson", Specialization = "HIIT", YearsOfExperience = 4, Bio = "Passionate about high-intensity interval training." }
        );
    }
}