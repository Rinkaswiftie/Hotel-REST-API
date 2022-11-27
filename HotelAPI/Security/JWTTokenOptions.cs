namespace HotelAPI.Security
{

    public class JWTTokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public long AccessTokenExpirationMinutes { get; set; }
        public long RefreshTokenExpirationDays { get; set; }
        public string Secret { get; set; }
        public string RefreshTokenSecret { get; set; }
    }
}