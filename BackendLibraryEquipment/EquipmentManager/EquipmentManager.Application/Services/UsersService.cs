using BCrypt.Net;
using EquipmentManager.Application.Contracts;
using EquipmentManager.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EquipmentManager.Application.Services
{
    public class UsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<Users> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty.", nameof(email));

                return await _usersRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the user with email {email}.", ex);
            }
        }

        public async Task<Users> GetByIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

                return await _usersRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the user with ID {userId}.", ex);
            }
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            try
            {
                if (_usersRepository == null)
                    throw new InvalidOperationException("Users repository is not initialized.");

                return await _usersRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all users.", ex);
            }
        }

        public async Task AddAsync(Users user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");

                await _usersRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new user.", ex);
            }
        }

        public async Task UpdateAsync(Users user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");

                await _usersRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the user with ID {user.UserId}.", ex);
            }
        }

        public async Task DeleteAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

                await _usersRepository.DeleteAsync(userId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the user with ID {userId}.", ex);
            }
        }

        public async Task<string> GenerateJwtToken(Users user, IConfiguration configuration)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");

                if (configuration == null)
                    throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

                var tokenHandler = new JwtSecurityTokenHandler();

                string secret = configuration["Jwt:Key"]
                    ?? configuration["JWT_SECRET"]
                    ?? throw new InvalidOperationException("JWT Key is not configured.");
                var key = Encoding.ASCII.GetBytes(secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                        new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the JWT token.", ex);
            }
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty.", nameof(email));

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be null or empty.", nameof(password));

                var user = await _usersRepository.GetByEmailAsync(email);

                if (user == null)
                    return false;

                return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash ?? string.Empty);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while verifying the password for email {email}.", ex);
            }
        }

        public async Task<ClaimsPrincipal> ValidateToken(string token, IConfiguration configuration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token cannot be null or empty.", nameof(token));

                if (configuration == null)
                    throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                string secret = configuration["Jwt:Key"]
                    ?? configuration["JWT_SECRET"]
                    ?? throw new InvalidOperationException("JWT Key is not configured.");
                byte[] key = Encoding.ASCII.GetBytes(secret);

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while validating the JWT token.", ex);
            }
        }
    }
}
