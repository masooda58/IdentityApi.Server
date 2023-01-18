namespace Jwt.Identity.BoursYarServer.Services.TokenServices
{
    public class JwtSettingModel
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationminutes { get; set; }
        public string RefreshSecret { get; set; }
    }
}