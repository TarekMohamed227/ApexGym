using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new User { UserName = registerDto.Email, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // DEFAULT ROLE: Every new user gets "Member" role
            if(registerDto.Email.ToLower().Contains("Member"))
            {
                await _userManager.AddToRoleAsync(user, "Member");
            }   
            

            // SPECIAL CASE: If email contains "admin", also give "Admin" role
            if (registerDto.Email.ToLower().Contains("admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return Ok(new { message = "Registration successful. Please log in." });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                // Check if user exists and password is correct
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Generate JWT token
                var token = _tokenService.CreateToken(user);

                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    user = new { user.Email, user.Id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }
    }
}
