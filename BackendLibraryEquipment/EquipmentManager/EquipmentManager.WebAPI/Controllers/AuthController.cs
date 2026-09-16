using EquipmentManager.Application.Services;
using EquipmentManager.Domain;
using EquipmentManager.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManager.WebAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly IConfiguration _configuration;

        public AuthController(UsersService usersService, IConfiguration configuration)
        {
            _usersService = usersService;
            _configuration = configuration;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            bool isValidPassword = await _usersService.VerifyPasswordAsync(request.Email, request.Password);

            if (!isValidPassword)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid credentials",
                    Detail = "The email address or password is incorrect."
                });
            }

            Users user = await _usersService.GetByEmailAsync(request.Email);

            string hashedId = BCrypt.Net.BCrypt.HashPassword(user.UserId.ToString());

            string jwt = await _usersService.GenerateJwtToken(user, _configuration);

            return Ok(new LoginResponseDTO
            {
                HashedId = hashedId,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Token = jwt
            });
        }
    }
}
