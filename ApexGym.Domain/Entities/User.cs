using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Entities
{
    // Inherit from IdentityUser which has properties like Email, UserName, PasswordHash
    public class User : IdentityUser<int> // <-- 'int' means we want to use an integer as the ID
    {
        // We can add custom properties here later
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
