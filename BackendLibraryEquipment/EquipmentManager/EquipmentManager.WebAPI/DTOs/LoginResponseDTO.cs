namespace EquipmentManager.WebAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string HashedId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
