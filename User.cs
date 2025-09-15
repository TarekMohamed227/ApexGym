using Microsoft.AspNetCore.Identity;

namespace ApexGym.Domain.Entities;

public class User : IdentityUser<int> // <-- 'int' means we want to use an integer as the ID
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}