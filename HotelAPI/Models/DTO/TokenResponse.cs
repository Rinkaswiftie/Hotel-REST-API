using System.ComponentModel.DataAnnotations;


namespace HotelAPI.Models.DTO{

    public class TokenResponse
    {
        public NewUserResponse User { get; set; }
        public string AccessToken { get; set; }

        [StringLength(200)]
        public string RefreshToken { get; set; }

    }
}