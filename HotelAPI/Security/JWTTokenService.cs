using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelAPI.Models;
using HotelAPI.Security.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace HotelAPI.Security
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly JWTTokenOptions _tokenOptions;
        private readonly SymmetricSecurityKey _key;
        private readonly SymmetricSecurityKey _refreshKey;
        public JWTTokenService(IOptions<JWTTokenOptions> options)
        {
            _tokenOptions = options.Value;
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.Secret));
            _refreshKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.RefreshTokenSecret));
        }

        public string GenerateJwtToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpirationMinutes);
            var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);


            var securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: GetClaims(user),
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials
            );
            return handler.WriteToken(securityToken);
        }

        private IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id+""),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth+"")
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role,"" + userRole));
            }

            return claims;
        }

        public User? ValidateJwtToken(string token)
        {
            if (token == null) return null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = false,
                    ValidIssuer = _tokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _tokenOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var user =  new User();            
                user.Id = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
                user.Email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                user.DateOfBirth =  DateTime.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Birthdate).Value);
                var a = jwtToken.Claims.Where(r=> r.Type == ClaimTypes.Role);
                user.UserRoles = a
                .Select(claim => (Role)Enum.Parse(typeof(Role),claim.Value))
                .ToList();                
                
                // return user id from JWT token if validation successful
                return user;
            }
            catch(Exception e)
            {

                // return null if validation fails
                return null;
            }

        }
        public string GenerateRefreshToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenExpiration = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpirationDays);
            var signingCredentials = new SigningCredentials(_refreshKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id+""),
            };

            var securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims,
                expires: tokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials
            );
            return handler.WriteToken(securityToken);
        }

         public int? ValidateRefreshToken(string token)
        {
            if (token == null) return null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _refreshKey,
                    ValidateIssuer = false,
                    ValidIssuer = _tokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _tokenOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }

        }
    }
}