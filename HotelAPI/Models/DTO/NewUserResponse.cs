using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Models
{

    public class NewUserResponse
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(30)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime DateOfBirth { get; set; }

        public ICollection<Role> UserRoles { get; set; } = new List<Role>();

    }
}