using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Text;

namespace BokaMera.API.Samples
{
    class Program
    {
        const string ApiUrl = "http://localhost:59600/";

        static void Main(string[] args)
        {
            // Create and configure client
            // BokaMera API supports MsgPack, prefer that for efficient communication with the API.
            // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format
            // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
            //var client = new JsonServiceClient(apiUrl);
            var client = new MsgPackServiceClient(ApiUrl);

            // Authenticate
            client.Headers.Add("x-api-key", "Somelongrandoapikey"); //You will get this API from the support@bokamera.se
            //client.UserName = "demo@bokamera.se";
            //client.Password = "demo12";

            //Authenticate the user
            client.Post(new Authenticate() { provider = "login", UserName = "demo@bokamera.se", Password = "demo12" });


            // Call service, this uses Dto from BokaMera's nuget package to return typed responses.
            // Check the namespace BokaMera.API.ServiceModel.Dto.* for request and response classes
            Console.WriteLine("Calling service GetApiVersion...");
            var response = client.Get(new ApiVersionQuery());


            //Create request object for retrieving bookings
            var request = new BookingQuery
            {
                BookingStart = new DateTime(2017, 01, 01), //From what date to collect bookings
                AllCompanyBookings = true,  //If retrieve all bookings for the logged in user (the customer) AND for the company he is admin for (requires the user is a admin, otherwise set false here)
                IncludeServiceInformation = true, //Will also retrive the Service information on the Booking. If included = false the property will be null (theBooking.Service)
                IncludeCustomFieldValues = true, //Will also retrive the custom field and values on the Booking (if any exists). If included = false the property will be null (theBooking.CustomFieldValues)
                StatusIds = null //What statuses to include, if null or empty array only active bookings will be included
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
