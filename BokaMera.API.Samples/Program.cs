using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Text;
using System.Net;

namespace BokaMera.API.Samples
{
    class Program
    {
        const string ApiUrlTest = "https://testapi.bokamera.se/";
        //const string ApiUrlProd = "https://api.bokamera.se/";


        static void Main(string[] args)
        {

            AuthExamples();

            // Create and configure client
            // BokaMera API supports MsgPack, prefer that for efficient communication with the API.
            // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format
            // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
            var client = new JsonServiceClient(ApiUrlTest);
            client.Headers.Add("x-api-key", "YOUR_API_KEY_HERE");
            
            //Authenticate the user
            var response = client.Post<AuthenticateResponse>("authenticate", new
            {
                UserName = "username",
                Password = "password",
            });

            
            client.AddHeader("x-ss-id", response.SessionId);

            
            client.BearerToken = response.BearerToken;           

            // Call service, this uses Dto from BokaMera's nuget package to return typed responses.
            // Check the namespace BokaMera.API.ServiceModel.Dto.* for request and response classes
            Console.WriteLine("Calling service GetApiVersion...");
            var versionResponse = client.Get(new ApiVersionQuery());


            //Create request object for retrieving bookings
            var request = new BookingQuery
            {
               Id = 370476
            };

            //Retrieve the bookings. To see what return type see the https://testapi.bokamera.se/swagger-ui/#!/bookings/BookingQuery  
            QueryResponse<BookingQueryResponse> bookings = client.Get<QueryResponse<BookingQueryResponse>>(request);

            // Print response
            Console.WriteLine("Response received, printing output...");
            response.PrintDump();

            // Logout session
            Console.WriteLine("Logging out...press any key to quit");
            client.Post(new Authenticate { provider = "logout" });

            // Wait for keypress
            Console.ReadKey();
        }

        static void AuthExamples()
        {
           
            var credentials = new { UserName = "username", Password = "password" };
            var key = "YOUR_API_KEY_HERE";


            Action<HttpWebRequest> apiKey = r => r.Headers.Add("x-api-key", key);

            Console.WriteLine("Retrive authentication data...");
            var authResponce = 
                (ApiUrlTest + "/authenticate")
                    .PostJsonToUrl(credentials, apiKey)
                    .FromJson<AuthenticateResponse>();
           
            authResponce.PrintDump();

            Action<HttpWebRequest> bearerTokenAuth = r => r.AddBearerToken(authResponce.BearerToken);
            Action<HttpWebRequest> sessionAuth = r => r.Headers.Add("x-ss-id", authResponce.SessionId);

            //authentication via bearer token
            Console.WriteLine("Send authenticated request with bearer token...");
            (ApiUrlTest + "/users")
                .GetJsonFromUrl(apiKey + bearerTokenAuth)
                .PrintDump();

            //authentication via session id
            Console.WriteLine("Send authenticated request with session id...");
            (ApiUrlTest + "/users")
                .GetJsonFromUrl(apiKey + sessionAuth)
                .PrintDump();

            //anonymous request
            Console.WriteLine("Send anonymous request...");
            (ApiUrlTest + "/version")
               .GetJsonFromUrl(apiKey)
               .PrintDump();

            //client with authentication on demand
            var client = new JsonServiceClient(ApiUrlTest) { RequestFilter = apiKey };
            client.OnAuthenticationRequired = () => 
            {
                client.BearerToken =
                    (ApiUrlTest + "/authenticate")
                    .PostJsonToUrl(new { UserName = "demo@bokamera.se", Password = "demo12", }, apiKey)
                    .FromJson<AuthenticateResponse>().BearerToken;
            };

            Console.WriteLine("Send request with authentication on demand...");
            client.Get(new CurrentUserQuery()).PrintDump(); 
        }
    }
}
