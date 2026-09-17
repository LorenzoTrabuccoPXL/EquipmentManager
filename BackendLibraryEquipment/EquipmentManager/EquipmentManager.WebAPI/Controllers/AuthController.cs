using EquipmentManager.Application.Services;
using EquipmentManager.Domain;
using EquipmentManager.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly IConfiguration _configuration;

        public AuthController(UsersService usersService, IConfiguration configuration)
        {
            _usersService = usersService;
            _configuration = configuration;
        }

        [AllowAnonymous]
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

            string jwt = await _usersService.GenerateJwtToken(user, _configuration);

            return Ok(new LoginResponseDTO
            {
                Id = user.UserId,
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
