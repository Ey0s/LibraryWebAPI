using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BCrypt.Net; // For password hashing
using Microsoft.Extensions.Configuration; // Ensure this is present for IConfiguration
using System; // Ensure this is present for DateTime and Convert

namespace LibraryWebAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // To access appsettings.json for JWT settings

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> Register(string username, string password)
        {
            // Check if username already exists
            if (_context.Users.Any(u => u.Username == username))
            {
                return null; // Username already taken
            }

            // Hash the password before storing
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User { Username = username, PasswordHash = passwordHash };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // User not found or password incorrect
            }

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Get the JWT secret from appsettings.json
            // CRITICAL CHANGE: Use Encoding.UTF8.GetBytes for consistency with validation
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            // Temporary Debugging Line: Check the key being used for generation
            Console.WriteLine($"--- AuthService: JWT Generation Key Bytes (UTF8): {Convert.ToBase64String(key)}");
            Console.WriteLine($"--- AuthService: Raw Key String: {_configuration["Jwt:Key"]}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                // Token expiration set based on appsettings.json
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])), 
                
                // Use the key for signing the token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                
                // Issuer and Audience from appsettings.json
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}