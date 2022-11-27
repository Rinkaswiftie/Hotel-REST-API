using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Models{

    public class Roles
    {

        public Role Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }

    public enum Role{
        Admin = 1,
        User = 2
    }
}