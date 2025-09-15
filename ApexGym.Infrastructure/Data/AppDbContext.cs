using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data;

//public class AppDbContext : IdentityDbContext<User>
//{
//    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // Call base constructor
//    {
//    }

//    public DbSet<Member> Members => Set<Member>();

//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder); // <-- CRITICAL: This calls Identity's model configuration
//        // Your other model configurations here...
//    }
//}
