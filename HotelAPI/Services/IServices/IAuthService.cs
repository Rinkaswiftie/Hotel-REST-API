using HotelAPI.Models;
using HotelAPI.Models.DTO;

namespace HotelAPI.Services.iServices
{

    public interface IAuthService
    {
        public Task<TokenResponse?> SignUpUser(SignUpRequest userInput);

        public Task<TokenResponse?> LoginUser(UserCredentials credentials);

        public Task<TokenResponse> RefreshAccessToken(string refreshToken);
    }
}