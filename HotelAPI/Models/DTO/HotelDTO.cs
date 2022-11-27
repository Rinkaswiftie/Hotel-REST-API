using System.ComponentModel.DataAnnotations;


namespace HotelAPI.Models.DTO{

    public class HotelDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

    }
}