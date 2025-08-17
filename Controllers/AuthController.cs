using Microsoft.AspNetCore.Mvc;
using EcommerceApi.Data;
using EcommerceApi.DTOs;
using EcommerceApi.Models;
using EcommerceApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _jwtService = jwtService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest("Email is already taken.");
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email
                };

                // Hash password
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

                // Save user
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "User registered successfully!" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while creating the user.");
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized("Invalid credentials.");
                }

                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Save refresh token to database
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during login.");
            }
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && !rt.IsRevoked);

                if (refreshToken == null || refreshToken.ExpiresAt <= DateTime.UtcNow)
                {
                    return Unauthorized("Invalid or expired refresh token.");
                }

                // Revoke old refresh token
                refreshToken.IsRevoked = true;

                // Generate new tokens
                var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Save new refresh token
                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = refreshToken.UserId,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during token refresh.");
            }
        }

        // Get logged in user details
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try 
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(new {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                });
            } catch (Exception) {
                return StatusCode(500, "An error occurred while getting the user.");
            }
        }
    }
}
