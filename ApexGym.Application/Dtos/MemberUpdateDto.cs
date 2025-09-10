using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class MemberUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        // We do NOT include Id or RegistrationDate here.
        // The client should not be allowed to update these.
    }
}
