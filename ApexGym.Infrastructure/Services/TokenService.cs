using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<User> _userManager; // Add UserManager

    // Inject UserManager through constructor
    public TokenService(IConfiguration config, UserManager<User> userManager)
    {
        _config = config;
        _userManager = userManager;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
    }

    public async Task<string> CreateToken(User user) // Made async
    {
        // 1. GET USER ROLES FROM DATABASE - This is the correct way
        var roles = await _userManager.GetRolesAsync(user);

        // 2. CREATE CLAIMS - Include actual roles from database
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        // 3. ADD EACH ROLE AS A SEPARATE CLAIM
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 4. CREATE SIGNING CREDENTIALS
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // 5. DESCRIBE THE TOKEN
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_config["JwtSettings:ExpiryMinutes"]!)
            ),
            SigningCredentials = creds,
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        // 6. CREATE AND WRITE THE TOKEN
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}