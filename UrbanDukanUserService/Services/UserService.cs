using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UrbanDukanUserService.DTOs;
using UrbanDukanUserService.Interfaces;
using UrbanDukanUserService.Models;
using UrbanDukanUserService.Security;
using UrbanDukanUserService.Settings;

namespace UrbanDukanUserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly JwtSettings _jwt;

        public UserService(IUserRepository repo, IOptions<JwtSettings> jwtOptions)
        {
            _repo = repo;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            ValidateEmail(request.Email);
            ValidatePasswordStrength(request.Password);

            var existing = await _repo.GetByEmailAsync(request.Email);
            if (existing is not null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                CreatedAt = DateTime.UtcNow,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                Phone = request.Phone
            };

            // Role assignment: use provided role or default to "Buyer"
            var desiredRoleName = string.IsNullOrWhiteSpace(request.Role) ? "Buyer" : request.Role.Trim();
            var role = await _repo.GetRoleByNameAsync(desiredRoleName);
            if (role is null)
                throw new ArgumentException($"Role '{desiredRoleName}' does not exist.");

            user.Roles.Add(role);

            await _repo.CreateAsync(user);

            var token = CreateJwtToken(user);

            return new AuthResponse
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt,
                UserId = user.Id,
                Email = user.Email
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            ValidateEmail(request.Email);

            var user = await _repo.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = CreateJwtToken(user);

            return new AuthResponse
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt,
                UserId = user.Id,
                Email = user.Email
            };
        }

        public async Task<UserDetailsResponse?> GetUserDetailsAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user is null) return null;

            return new UserDetailsResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.Roles?.Select(r => r.Name).ToArray() ?? Array.Empty<string>()
            };
        }

        public async Task<UserDetailsResponse?> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user is null) return null;

            // Update allowed profile fields
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Address = request.Address;
            user.Phone = request.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user);

            return new UserDetailsResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.Roles?.Select(r => r.Name).ToArray() ?? Array.Empty<string>()
            };
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
        }

        private static void ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.");

            var hasUpper = false;
            var hasLower = false;
            var hasDigit = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
            }

            if (!hasUpper || !hasLower || !hasDigit)
                throw new ArgumentException("Password must include upper, lower and a digit.");
        }

        private (string Token, DateTime ExpiresAt) CreateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);

            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString() ?? string.Empty),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenStr, expires);
        }
    }
}