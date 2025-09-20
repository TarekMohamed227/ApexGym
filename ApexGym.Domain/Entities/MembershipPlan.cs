using ApexGym.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Entities
{
    public class MembershipPlan: BaseEntity
    {
        

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 1000)]
        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
