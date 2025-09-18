using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ApexGym.Infrastructure.Data.Configurations
{
    public class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
    {
        public void Configure(EntityTypeBuilder<MembershipPlan> builder)
        {
            builder.HasKey(mp => mp.Id);

            builder.Property(mp => mp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mp => mp.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(mp => mp.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(mp => mp.DurationDays)
                .IsRequired();

            builder.Property(mp => mp.IsActive)
                .HasDefaultValue(true);

            // Seed initial data
            builder.HasData(
                new MembershipPlan
                {
                    Id = 1,
                    Name = "Basic",
                    Description = "Gym access during business hours (8 AM - 8 PM)",
                    Price = 49.99m,
                    DurationDays = 30
                },
                new MembershipPlan
                {
                    Id = 2,
                    Name = "Premium",
                    Description = "24/7 gym access + locker storage",
                    Price = 99.99m,
                    DurationDays = 30
                },
                new MembershipPlan
                {
                    Id = 3,
                    Name = "VIP",
                    Description = "24/7 access + locker + 2 personal training sessions per month",
                    Price = 199.99m,
                    DurationDays = 30
                }
            );
        }
    }
}
