using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using BokaMera.API.Samples.Bootstrap;
using BokaMera.API.Samples.Model;
using Config = BokaMera.API.Samples.Model.Config;

namespace BokaMera.API.Samples
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Setup credentials
            // TODO: ENTER YOUR CREDENTIALS AND API KEY HERE
            var config = Config.TestEnvironment;
            config.ApiKey = Guid.Parse("FAF84D4B-58E0-435C-A4C3-536A00CAFABD");
            config.Username = "demo@bokamera.se";
            config.Password = "demo12";

            // Create client and login
            var (client, session) = BokaMeraClientFactory.GetAuthorizedClient(config);

            // Execute sample requests
            GetApiVersionExample(client);
            GetBookingsQueryExample(client);

            // Notable other request classes to checkout:
            // - CustomerQuery
            // - CreateBooking
            // - DeleteBooking

            // End session on identity server
            LogoutExample(config, session);

            // Wait for keypress
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void GetApiVersionExample(JsonServiceClient client)
        {
            // Call service, this uses Dto from BokaMera's nuget package to return typed responses.
            // Check the namespace BokaMera.API.ServiceModel.Dto.* for request and response classes
            Console.WriteLine("Calling service GetApiVersion...");
            var versionResponse = client.Get(new ApiVersionQuery());
            versionResponse.PrintDump("Response");
        }
        
        private static void GetBookingsQueryExample(JsonServiceClient client)
        {
            Console.WriteLine("Get bookings request...");

            //Create request object for retrieving bookings.
            // If no properties are set, all bookings for the current users company is returned
            var request = new BookingQuery
            {
                // If your user is an administer and want to list all bookings, set this to true
                // otherwise the bookings returned will be only the current users bookings
                // CompanyBookings = false
                
                // Use id if you want to retrieve one specific booking
                //Id = 404042 
                
                // Use interval if you want a range
                // BookingStart = DateTime.Now,
                // BookingEnd = DateTime.Now,
                
                // Use skip and take for paging
                // Skip = 5,
                // Take = 10
            };

            //Retrieve the bookings. To see what return type see the https://testapi.bokamera.se/swagger-ui/#!/bookings/BookingQuery  
            var response = client.Get(request);
            response.PrintDump("Response");
        }

        private static void LogoutExample(Config config, TokenResponse session)
        {
            // Logout session
            Console.WriteLine("Logout request...");
            BokaMeraClientFactory.Logout(config, session);
            Console.WriteLine("Done.");
        }
    }
}