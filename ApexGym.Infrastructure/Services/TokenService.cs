using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Infrastructure.Services
{
   

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // Create the security key once during construction for better performance
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        }

        public string CreateToken(User user)
        {
            // 1. CREATE CLAIMS - These become the payload of the JWT
            // Claims are pieces of information about the user that we "claim" to be true
            var claims = new List<Claim>
        {
            // The user's unique ID. This is the most important claim.
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            
            // The user's email. Useful for display purposes.
            new Claim(ClaimTypes.Email, user.Email),
            
            // The username. Identity requires this.
            new Claim(ClaimTypes.Name, user.UserName),
            
            // You can add custom claims here later:
            // new Claim("MembershipLevel", "Premium"),
            // new Claim("GymBranchId", "5")
        };

            // 2. CREATE SIGNING CREDENTIALS - How we will sign the token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 3. DESCRIBE THE TOKEN - All the token's properties
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // The payload (claims) for the token
                Expires = DateTime.UtcNow.AddMinutes( // When the token expires
                    double.Parse(_config["JwtSettings:ExpiryMinutes"]!)
                ),
                SigningCredentials = creds, // How to sign it
                Issuer = _config["JwtSettings:Issuer"], // Who issued it
                Audience = _config["JwtSettings:Audience"] // Who it's for
            };

            // 4. CREATE AND WRITE THE TOKEN
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
