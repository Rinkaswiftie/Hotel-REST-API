using System.ComponentModel.DataAnnotations;
using HotelAPI.Models.DTO;

namespace HotelAPI.Models
{

    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public Boolean IsActive { get; set; }

        public String ImageName { get; set; }

        public void updateWithDTO(HotelDTO hotel){
           this.Name =  hotel.Name;
           this.Description = hotel.Description;
        }
    }
}