using HotelAPI.Models;

namespace HotelAPI.Security.Core
{
    public interface IJWTTokenService
    {
        public string GenerateJwtToken(User user);
        public User? ValidateJwtToken(string token);
        public string GenerateRefreshToken(User user);
        public int? ValidateRefreshToken(string token);
    }
}