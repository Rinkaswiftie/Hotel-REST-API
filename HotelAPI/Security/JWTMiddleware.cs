using Microsoft.Extensions.Options;
using HotelAPI.Services.iServices;
using HotelAPI.Security.Core;

namespace HotelAPI.Security
{

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJWTTokenService tokenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = tokenService.ValidateJwtToken(token);
            if (user != null)
            {
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}