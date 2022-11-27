using Xunit;
using AutoMapper;
using HotelAPI.Mapping;
using HotelAPI.Tests.Helpers;
using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models.DTO;
using System.Linq;
using HotelAPI.ErrorHandling;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace HotelAPI.Tests.Controllers;

public class HotelControllerTests
{
    private static IMapper? _mapper;

    private static IUnitOfWork? _db;

    private static HotelsController _hotelController;

    public HotelControllerTests()
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DTOToModelProfile());
                mc.AddProfile(new ModelToDTOProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
        }

        if (_db == null)
        {
            _db = new InMemoryDBHelper().GetUnitOfWork();
        }

        _hotelController = new HotelsController(_db, _mapper);
    }

    [Fact]
    public async void GetHotels_ShouldReturnAllHotels()
    {
        //Act
        var result = await _hotelController.GetHotels();

        //Assert
        Assert.InRange(result.Value.Count(), 2, 10);

    }

    [Fact]
    public async void GetHotel_WithExistingHotelIdInput_ShouldReturnCorrectHotel()
    {
        //Act
        var response = await _hotelController.GetHotel(1);

        //Assert
        Assert.Equal(1, response.Value.Id);
    }

    [Fact]
    public async void GetHotel_InvalidIdInput_ShouldNotFoundResult()
    {
        //Act
        var response = await _hotelController.GetHotel(400);

        //Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async void CreateNewHotel_WithExistingHotelIDInput_ShouldThrowAnException()
    {
        //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 1,
            Name = "Test Hotel",
            Description = "Invalid Hotel"
        };

        //Assert 
        var result = await Record.ExceptionAsync(async () => await _hotelController.CreateNewHotel(newHotel));
        Assert.IsType<AppException>(result);
        Assert.Equal(result.Message, "Hotel ID already exists");
    }

    [Fact]
    public async void CreateNewHotel_ValidInput_CreateAHotel()
    {
        //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 300,
            Name = "Test Hotel",
            Description = "Valid Hotel"
        };

        //Act
        var response = await _hotelController.CreateNewHotel(newHotel);

        //Assert
        Assert.IsType<CreatedAtActionResult>(response.Result);

    }

    [Fact]
    public async void UpdateHotel_MismatchingHotelID_ThrowAnError()
    {
        //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 200,
            Name = "Test Hotel",
            Description = "Valid Hotel"
        };

        //Act
        //Assert 
        var result = await Record.ExceptionAsync(async () => await _hotelController.UpdateHotel(1, newHotel));
        Assert.IsType<AppException>(result);
        Assert.Equal(result.Message, "Hotel ID Doesn't match");
    }

    [Fact]
    public async void UpdateHotel_OnNonExistentHotel_ShouldReturnNotFound()
    {
        //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 201,
            Name = "Test Hotel",
            Description = "Valid Hotel"
        };

        //Act
        var response = await _hotelController.UpdateHotel(201, newHotel);

        //Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void UpdateHotel_OnValidInput_UpdateTheHotel()
    {   //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 1,
            Name = "Test Hotel",
            Description = "Valid Hotel"
        };

        //Act
        var response = await _hotelController.UpdateHotel(1, newHotel);

        //Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async void DeleteHotel_OnValidInput_ShouldDeleteHotel()
    {
        //Arrange
        var newHotel = new HotelDTO()
        {
            Id = 117,
            Name = "Test Hotel",
            Description = "Valid Hotel"
        };
        var createdHotel = await _hotelController.CreateNewHotel(newHotel);

        //Act
        var response = await _hotelController.DeleteHotel(117);

        //Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async void DeleteHotel_OnNonExistantInput_ShouldReturnNotFound()
    {
        //Act
        var response = await _hotelController.DeleteHotel(999);

        //Assert
        Assert.IsType<NotFoundResult>(response);

    }

    [Fact]
    public async void Upload_OnInvalidId_ShouldReturnNotFound()
    {
        //Arrange
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, 100, "hotel image", fileName);
        
        //Act
        var response = await _hotelController.Upload(731, file);

        //Assert
        Assert.IsType<NotFoundResult>(response);

    }
}