using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Common;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        private readonly AppDbContext _dbContext;

        // Constructor: Dependency Injection provides the AppDbContext
        public MemberRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Members.AnyAsync(m => m.Email == email);
        }
        public async Task<Member> GetByIdAsync(int id)
        {
            // Use EF Core to find the member by their primary key (Id)
            var member= await _dbContext.Members.FindAsync(id);

            if(member == null)
            {
                throw new NotFoundException(nameof(Member), id);
            }
            return member;

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
           
            // Return the member (its Id will now be populated by the database)
            return member;
        }

     
        public async Task UpdateAsync(Member member) // Fixed typo in method name (UpdateAsyc -> UpdateAsync)
        {
            _context.Entry(member).State = EntityState.Modified;
           
        }
        public async Task DeleteAsync(Member member)
        {
            // Remove the member from the DbContext's tracker
            _dbContext.Members.Remove(member);
            // Save the changes to the database
            

        }

       
    }
}
