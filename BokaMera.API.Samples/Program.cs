using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Text;

namespace BokaMera.API.Samples
{
    class Program
    {
        const string ApiUrlTest = "https://testapi.bokamera.se/";
        //const string ApiUrlProd = "https://api.bokamera.se/";


        static void Main(string[] args)
        {
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
    }
}
