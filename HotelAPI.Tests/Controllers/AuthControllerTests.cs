using Xunit;
using HotelAPI.Security;
using AutoMapper;
using HotelAPI.Mapping;
using HotelAPI.Services.iServices;
using HotelAPI.Services;
using HotelAPI.Tests.Helpers;
using HotelAPI.Security.Core;
using Microsoft.Extensions.Options;
using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Models.DTO;
using System.Linq;
using System;
using HotelAPI.ErrorHandling;
using HotelAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Tests.Controllers;

public class AuthControllerTests
{
    private static IMapper? _mapper;

    private static IUnitOfWork? _db;
    private static IAuthService? _authService;
    private static IJWTTokenService? _tokenService;
    private static IUserService? _userService;
    private static AuthController _authController;

    public AuthControllerTests()
    {

        //dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
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

        if (_tokenService == null)
        {
            var tokenOptions = Options.Create(new JWTTokenOptions()
            {
                Audience = "SampleAudience",
                Issuer = "HotelAPI",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 30,
                Secret = "very_long_but_insecure_token_here_be_sure_to_use_env_var",
                RefreshTokenSecret = "Rinka's refresh token secret"
            });
            _tokenService = new JWTTokenService(tokenOptions);
        }

        if (_userService == null)
        {
            _userService = new UserService(_db);
        }

        if (_authService == null)
        {
            _authService = new AuthService(_db, _mapper, _tokenService, _userService);
        }

        _authController = new AuthController(_authService);
    }

    [Fact]
    public async void SignUp_OnValidInput_ShouldSignUpTheUser()
    {
        //Arrange
        var signUp = new SignUpRequest()
        {
            Name = "Test User",
            DateOfBirth = new System.DateTime(2000, 11, 11),
            Email = "test@test.com",
            Password = "Pass123",
            ConfirmPassword = "Pass123"
        };

        //Act
        var output = await _authController.SignUp(signUp);


        //Assert
        Assert.Equal(output.Value.User.Name, signUp.Name);
        Assert.Equal(output.Value.User.DateOfBirth, signUp.DateOfBirth);
        Assert.Equal(output.Value.User.Email, signUp.Email);
        Assert.Equal(output.Value.User.UserRoles.FirstOrDefault(), Models.Role.User);
    }

    [Fact]
    public async void SignUp_OnValidInput_ShouldGenerateAccessTokens()
    {
        //Arrange
        var signUp = new SignUpRequest()
        {
            Name = "Test User",
            DateOfBirth = new System.DateTime(2000, 11, 11),
            Email = "accesscheck@test.com",
            Password = "Pass123",
            ConfirmPassword = "Pass123"
        };

        //Act
        var output = await _authController.SignUp(signUp);

        //Assert
        Assert.IsType<string>(output.Value.AccessToken);
        Assert.IsType<string>(output.Value.RefreshToken);
    }

    [Fact]
    public async void SignUp_WhenInputInvalidEmail_ShouldThowAnError()
    {
        //Arrange
        var signUp = new SignUpRequest()
        {
            Name = "Test User",
            DateOfBirth = new System.DateTime(2000, 11, 11),
            Email = "common@common.com",
            Password = "Pass123",
            ConfirmPassword = "Pass123"
        };

        //Act
        var ex = await Assert.ThrowsAsync<AppException>(async () => await _authService.SignUpUser(signUp));
        Assert.Equal(ex.Message, "Email exists already");

    }

    [Fact]
    public async void Login_WhenInputValidCredentials_ShouldLoginUser()
    {
        //Arrange
        var login = new UserCredentials()
        {
            Email = "common@common.com",
            Password = "12345678"
        };
        var authControl = new AuthController(_authService);

        //Act
        var output = await authControl.Login(login);

        //Assert
        Assert.Equal(output.Value.User.Name, "Jane Doe");
        Assert.Equal(output.Value.User.DateOfBirth, new DateTime(2000, 04, 11));
        Assert.Equal(output.Value.User.Email, login.Email);
        Assert.Equal(output.Value.User.UserRoles.FirstOrDefault(), Models.Role.User);
    }

    [Fact]
    public async void Login_OnValidInput_ShouldGenerateAccessTokens()
    {
        //Arrange
        var login = new UserCredentials()
        {
            Email = "common@common.com",
            Password = "12345678"
        };

        //Act
        var output = await _authController.Login(login);

        //Assert
        Assert.NotNull(output.Value.AccessToken);
        Assert.IsType<string>(output.Value.AccessToken);
        Assert.NotNull(output.Value.RefreshToken);
        Assert.IsType<string>(output.Value.RefreshToken);
    }


    [Fact]
    public async void Login_WhenInValidEmail_ShouldThrowAnError()
    {
        //Arrange
        var login = new UserCredentials()
        {
            Email = "invalid@common.com",
            Password = "12345678"
        };

        var ex = await Assert.ThrowsAsync<AppException>(async () => await _authController.Login(login));
        Assert.Equal(ex.Message, "Email not found");

    }

    [Fact]
    public async void Login_WhenInValidPassword_ShouldThrowAnError()
    {
        //Arrange
        var login = new UserCredentials()
        {
            Email = "common@common.com",
            Password = "wrongpassword"
        };


        var ex = await Assert.ThrowsAsync<AppException>(async () => await _authController.Login(login));
        Assert.Equal(ex.Message, "Incorrect Password");
    }

    [Fact]
    public async void RefreshToken_OnValidTokenInput_ShouldGenerateNewAccessTokens()
    {
        //Arrange
        var login = new UserCredentials()
        {
            Email = "common@common.com",
            Password = "12345678"
        };
        var output = await _authController.Login(login);

        //Act
        var accessToken = await _authController.RefreshToken(output.Value.RefreshToken);

        //Assert
        Assert.Equal(output.Value.User.Name, accessToken.Value.User.Name);
        Assert.Equal(output.Value.User.DateOfBirth, accessToken.Value.User.DateOfBirth);
        Assert.Equal(output.Value.User.Email, accessToken.Value.User.Email);
        Assert.Equal(output.Value.User.UserRoles.FirstOrDefault(), accessToken.Value.User.UserRoles.FirstOrDefault());
        Assert.NotNull(accessToken.Value.AccessToken);
        Assert.IsType<string>(accessToken.Value.AccessToken);
        Assert.NotNull(accessToken.Value.RefreshToken);
        Assert.IsType<string>(accessToken.Value.RefreshToken);
    }

    [Fact]
    public void Logout()
    {
        var output = _authController.LogOut();
        Assert.IsType<OkResult>(output);

    }
}