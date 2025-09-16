using ApexGym.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data;

// Ensure it inherits from IdentityDbContext<User, IdentityRole<int>, int>
public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    // The constructor parameter must be DbContextOptions<AppDbContext>, not string
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // This must be the FIRST line to call the base Identity configuration
        base.OnModelCreating(builder);

        // Add your own custom model configurations AFTER the base call
        // builder.Entity<Member>(entity => { ... });
    }
}