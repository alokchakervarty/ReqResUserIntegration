namespace ReqResUserApi.Configuration
{
    public class ApiConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; } = "reqres-free-v1"; // Default API key, can be overridden in appsettings.json
        public int UserCacheExpirationMinutes { get; set; }
    }
}
