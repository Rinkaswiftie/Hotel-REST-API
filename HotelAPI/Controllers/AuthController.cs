using HotelAPI.Models.DTO;
using HotelAPI.Security.Filters;
using HotelAPI.Services.iServices;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("/signup")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponse>> SignUp(SignUpRequest signUp)
        {
            var res = await _authService.SignUpUser(signUp);
            return res;
        }

        [HttpPost("/login")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponse>> Login(UserCredentials credentials)
        {
            var res = await _authService.LoginUser(credentials);

            return res;
        }

        [HttpPost("/refresh-token")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<TokenResponse>> RefreshToken(string refreshToken)
        {
            var response = await _authService.RefreshAccessToken(refreshToken);
            return response;
        }

        [HttpPost("/logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult LogOut()
        {
            return Ok();
        }

    }
}