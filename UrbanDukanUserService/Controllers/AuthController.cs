using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanDukanUserService.DTOs;
using UrbanDukanUserService.Interfaces;

namespace UrbanDukanUserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _users;

        public AuthController(IUserService users) => _users = users;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _users.RegisterAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _users.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "Invalid credentials." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/auth/userdetails
        // Returns the currently authenticated user's details (tries several claim keys)
        [HttpGet("userdetails")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Try common claim names used by JWT handlers:
            // - http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier (ClaimTypes.NameIdentifier)
            // - "sub" (JwtRegisteredClaimNames.Sub)
            // - raw "sub"
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var userId))
                return Unauthorized();

            var details = await _users.GetUserDetailsAsync(userId);
            if (details is null)
                return NotFound(new { error = "User not found." });

            return Ok(details);
        }
    }
}