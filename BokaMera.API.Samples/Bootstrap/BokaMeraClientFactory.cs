using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BokaMera.API.Samples.Model;
using ServiceStack;
using ServiceStack.Text;
using Config = BokaMera.API.Samples.Model.Config;

namespace BokaMera.API.Samples.Bootstrap
{
    public static class BokaMeraClientFactory
    {
        /// <summary>
        /// Get an authorized ServiceStack client using the Client Credentials Grant method
        /// </summary>
        public static (JsonServiceClient client, TokenResponse session) GetAuthorizedClient(Config config)
        {
            // Request a token from the identity server
            Console.WriteLine($"Requesting token from {config.TokenUrl}...");
            var session = GetToken(config);
            session.PrintDump();
            
            // Create a client that always adds the bearer token and api key to headers
            var client = new JsonServiceClient(config.ApiBaseUrl.ToString())
            {
                
                // Access token is short lived, see session.ExpiresIn
                BearerToken = session.AccessToken,
                
                // When access token expires, the refresh token is used to retrieve a new one,
                // the refresh token can also expire, see session.RefreshTokenExpiresIn 
                
                // When this happens, it will throw a RefreshTokenExpiredException that can 
                // be caught in order to re-authenticate.
                // See: https://docs.servicestack.net/jwt-authprovider#handling-refresh-tokens-expiring
                RefreshToken = session.RefreshToken
            };

            // Access headers
            client.AddHeader("x-api-key", config.ApiKey.ToString());
            
            // Other configuration headers
            client.AddHeader("x-language", "sv"); // sv = swedish, en = english (default)
            //client.AddHeader("baseUri", bokaMeraConfig.ApiBaseUrl);

            return (client, session);
        }

        private static TokenResponse GetToken(Config config)
        {
            //Build data string, eg: "name=john&age=20&city=Uganda";
            var dict = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", config.ClientId},
                {"username", config.Username},
                {"password", config.Password}
            };

            var data = string
                .Join("&", dict
                    .Select(kvp => $"{kvp.Key}={kvp.Value}"));

            // Post as url encoded to token end point
            using var client = new WebClient
            {
                Headers = {[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded"}
            };

            var resultJson = client.UploadString(config.TokenUrl, data);
                
            // Deserialize and return result
            try
            {
                    
                return resultJson.FromJson<TokenResponse>();
            }
            catch
            {
                throw new UnauthorizedAccessException($"Unauthorized: {resultJson}");
            }
        }
        
        public static void Logout(Config config, TokenResponse session)
        {
            //Build data string, eg: "name=john&age=20&city=Uganda";
            var dict = new Dictionary<string, string>
            {
                {"client_id", config.ClientId},
                {"refresh_token", session.RefreshToken}
            };

            var data = string
                .Join("&", dict
                    .Select(kvp => $"{kvp.Key}={kvp.Value}"));

            using var client = new WebClient
            {
                Headers =
                {
                    [HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded",
                    [HttpRequestHeader.Authorization] = $"Bearer {session.AccessToken}"
                }
            };

            // Post as url encoded to end session endpoint
            client.UploadString(config.EndSessionUrl, data);
        }
    }
}