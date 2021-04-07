using System;

namespace BokaMera.API.Samples.Model
{
    public class Config
    {
        // System configuration
        public Uri ApiBaseUrl { get; set; }
        public Uri TokenUrl { get; set; }
        public Uri EndSessionUrl { get; set; }
        public string ClientId { get; set; }
        
        // User configuration
        public Guid ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        // Predefined environments
        public static Config TestEnvironment => new()
        {
            ApiBaseUrl = new Uri("https://testapi.bokamera.se/"), 
            TokenUrl = new Uri("https://identity.bookmore.dev/auth/realms/bookmore/protocol/openid-connect/token"),
            EndSessionUrl = new Uri("https://identity.bookmore.dev/auth/realms/bookmore/protocol/openid-connect/logout"),
            ClientId = "bm-external-api-users"
        };
        
        public static Config ProductionEnvironment => new()
        {
            ApiBaseUrl = new Uri("https://api.bokamera.se/"), 
            TokenUrl = new Uri("https://identity.bookmore.com/auth/realms/bookmore/protocol/openid-connect/token"),
            EndSessionUrl = new Uri("https://identity.bookmore.com/auth/realms/bookmore/protocol/openid-connect/logout"),
            ClientId = "bm-external-api-users"
        };
    }
}