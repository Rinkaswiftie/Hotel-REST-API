using System.Net.Http.Headers;
using AutoMapper;
using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.ErrorHandling;
using HotelAPI.Models;
using HotelAPI.Models.DTO;
using HotelAPI.Security.Core;
using HotelAPI.Services.iServices;

namespace HotelAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        private readonly IJWTTokenService _tokenService;
        private readonly IUserService _userService;
        public AuthService(IUnitOfWork db, IMapper mapper, IJWTTokenService tokenService, IUserService userService)
        {
            _db = db;
            _mapper = mapper;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<TokenResponse?> SignUpUser(SignUpRequest signUp)
        {
            var userInput = _mapper.Map<User>(signUp);
            if (await _userService.UserExists(userInput.Email)) throw new AppException("Email exists already");
            var user = await _userService.CreateANewUser(userInput);
            return GenerateTokensForUser(user);
        }

        public async Task<TokenResponse?> LoginUser(UserCredentials credentials)
        {
            var user = await _db.UserRepository.FindByEmailAsync(credentials.Email);
            if (user == null) throw new AppException("Email not found");
            var passwordsMatch = _userService.verifyPasswords(user, credentials);
            if (!passwordsMatch) throw new AppException("Incorrect Password");
            return GenerateTokensForUser(user);
        }
       

        private TokenResponse GenerateTokensForUser(User user)
        {
            if (user.Id == null) throw new AppException("Token generation failed");;

            return new TokenResponse
            {
                User = _mapper.Map<NewUserResponse>(user),
                AccessToken = _tokenService.GenerateJwtToken(user),
                RefreshToken = _tokenService.GenerateRefreshToken(user)
            };
        }

        public async Task<TokenResponse> RefreshAccessToken(string refreshToken)
        {
            var userId = _tokenService.ValidateRefreshToken(refreshToken);
            if (userId == null) throw new AppException("Invalid Refresh token");
            var user = await _userService.GetById((int)userId);
            if (user == null) throw new AppException("Invalid Refresh token");
            return new TokenResponse
            {
                User = _mapper.Map<NewUserResponse>(user),
                AccessToken = _tokenService.GenerateJwtToken(user),
                RefreshToken = refreshToken
            };
        }


    }
}