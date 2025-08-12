using FlashcardApp.Core.Auth.Models;
using FlashcardApp.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FlashcardApp.Core.Auth.DTO;

namespace FlashcardApp.Core.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            var newUser = _mapper.Map<User>(registerDto);
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);
            await _userRepository.AddAsync(newUser);

            return GenerateJwtToken(newUser.Id, newUser.Email);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid email or password");

            return GenerateJwtToken(user.Id, user.Email);
        }

        private string GenerateJwtToken(string userId, string email)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}