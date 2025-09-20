using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IGenericRepository<Member> _memberRepository; // Use generic repository
        private readonly ITokenService _tokenService;

        // Updated constructor: Use IGenericRepository<Member> instead of IMemberRepository
        public AuthController(
            UserManager<User> userManager,
            ITokenService tokenService,
            IGenericRepository<Member> memberRepository) // Changed parameter type
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _memberRepository = memberRepository;
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
            if (registerDto.Email.ToLower().Contains("member"))
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
                var token = await _tokenService.CreateToken(user); // Make sure this is async!

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