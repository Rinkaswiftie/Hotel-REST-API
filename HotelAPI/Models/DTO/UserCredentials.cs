using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace HotelAPI.Models.DTO
{

    public class UserCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}