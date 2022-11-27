using AutoMapper;
using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.ErrorHandling;
using HotelAPI.Filters;
using HotelAPI.Models;
using HotelAPI.Models.DTO;
using HotelAPI.Security.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace HotelAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public HotelsController(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>Gets All the Hotels</summary>
        /// <returns>List of All Hotels</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Role.Admin, Role.User)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            var hotels = await _db.HotelRepository.GetAll();
            return hotels.Select(h => _mapper.Map<Hotel, HotelDTO>(h)).ToList();
        }

        /// <summary>Gets a Hotel</summary>
        /// <param name="id"></param>
        /// <returns>Hotel details</returns>
        /// <response code="200">Returns the Hotel details</response>
        /// <response code="400">If the hotel doesn't exist</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Role.Admin, Role.User)]
        public async Task<ActionResult<HotelDTO>> GetHotel(int id)
        {
            var hotel = await _db.HotelRepository.Get(id);
            if (hotel == null) return NotFound();
            return _mapper.Map<Hotel, HotelDTO>(hotel);
        }

        /// <summary> Creates a Hotel</summary>
        /// <param name="newHotel"></param>
        /// <returns>A newly created Hotel</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/hotels
        ///     {
        ///        "id": 1,
        ///        "name": "The Marbella",
        ///        "description": "The Marbella is a hotel on Jane the Virgin, located in Miami, Florida."
        ///     }
        /// </remarks>
        /// <response code="201">Newly created Hotel</response>
        /// <response code="400">If the hotel data isn't valid</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Role.Admin)]
        public async Task<ActionResult<HotelDTO>> CreateNewHotel(HotelDTO newHotel)
        {
            if (await _db.HotelRepository.Get(newHotel.Id) != null) throw new AppException("Hotel ID already exists");
            var hotel = _mapper.Map<HotelDTO, Hotel>(newHotel);
            hotel.IsActive = true;
            hotel.ImageName = "";
            _db.HotelRepository.Add(hotel);
            await _db.Save();
            return CreatedAtAction(
                nameof(CreateNewHotel),
                new { id = hotel.Id },
                _mapper.Map<Hotel, HotelDTO>(hotel)
                );
        }

        /// <summary>Updates a Hotel's data</summary>
        /// <param name="id"></param>
        /// <param name="hotel">Updated Hotel</param>
        /// <returns>Hotel details</returns>
        /// <response code="204">Successful update</response>
        /// <response code="400">If the hotel data isn't valid</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Role.Admin)]
        public async Task<ActionResult> UpdateHotel(int id, HotelDTO hotel)
        {
            if (hotel.Id != id) throw new AppException("Hotel ID Doesn't match");
            var hotelEntry = await _db.HotelRepository.Get(id);
            if (hotelEntry == null)
            {
                return NotFound();
            }
            hotelEntry.updateWithDTO(hotel);
            await _db.HotelRepository.Update(hotelEntry);
            return NoContent();
        }

        /// <summary>Updates a Hotel's data</summary>
        /// <param name="id"></param>
        /// <returns>Hotel details</returns>
        /// <response code="204">Successful delete</response>
        /// <response code="404">If the hotel doesn't exist already</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _db.HotelRepository.Get(id);

            if (hotel == null)
            {
                return NotFound();
            }
            if (System.IO.File.Exists(Path.Combine("wwwroot", "images", hotel.ImageName)))
            {
                // If file found, delete it    
                System.IO.File.Delete(Path.Combine("wwwroot", "images", hotel.ImageName));
            }
            _db.HotelRepository.Remove(hotel);
            await _db.Save();

            return NoContent();
        }

        [HttpPost("upload/image/{id}")]
        [Authorize(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateFile]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            var hotel = await _db.HotelRepository.Get(id);

            if (hotel == null)
            {
                return NotFound();
            }

            var fileName = Guid.NewGuid() + "-" + file.FileName;
            hotel.ImageName = fileName;
            var path = Path.Combine("wwwroot", "images", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                await _db.HotelRepository.Update(hotel);
                await _db.Save();
            };
            return Ok(new { length = file.Length, name = fileName });

        }

    }
}