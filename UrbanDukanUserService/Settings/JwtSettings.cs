namespace UrbanDukanUserService.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = "UrbanDukan";
        public string Audience { get; set; } = "UrbanDukanClients";
        public int ExpirationMinutes { get; set; } = 60;
    }
}