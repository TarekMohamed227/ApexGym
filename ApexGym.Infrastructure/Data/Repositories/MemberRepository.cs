using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data.Repositories
{
    public class MemberRepository : IMemberRepository // Implement the interface
    {
        private readonly AppDbContext _dbContext;

        // Constructor: Dependency Injection provides the AppDbContext
        public MemberRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Member> GetByIdAsync(int id)
        {
            // Use EF Core to find the member by their primary key (Id)
            return await _dbContext.Members.FindAsync(id);
        }

        public async Task<List<Member>> GetAllAsync()
        {
            // Use EF Core to get all members from the database
            return await _dbContext.Members.ToListAsync();
        }

        public async Task<Member> AddAsync(Member member)
        {
            // Add the member to the DbContext's tracker
            _dbContext.Members.Add(member);
            // Save the changes to the database
            await _dbContext.SaveChangesAsync();
            // Return the member (its Id will now be populated by the database)
            return member;
        }

        public async Task UpdateAsync(Member member)
        {
           

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Member member)
        {
            // Remove the member from the DbContext's tracker
            _dbContext.Members.Remove(member);
            // Save the changes to the database
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            // Use EF Core to check if any member already has this email
            return !await _dbContext.Members.AnyAsync(m => m.Email == email);
        }
    }
}
