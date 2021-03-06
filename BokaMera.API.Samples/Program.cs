﻿using System;
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
            //Language settings sv = swedish, en = english (default)
            client.Headers.Add("x-language", "sv");


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

        public void PostBookingAsAdminExample()
        {
            // Create and configure client
            // BokaMera API supports MsgPack, prefer that for efficient communication with the API.
            // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format
            // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
            var client = new JsonServiceClient(ApiUrlTest);
            client.Headers.Add("x-api-key", "YOUR_API_KEY_HERE");
            //Language settings sv = swedish, en = english (default)
            client.Headers.Add("x-language", "sv");

            //Authenticate the user
            var response = client.Post<AuthenticateResponse>("authenticate", new
            {
                UserName = "demo@bokamera.se", //The user is administrator for Company Demo Hårsalong with CompanyId: 00000000-0000-0000-0000-000000000001 (see https://www.bokamera.se/Demo1)
                Password = "demo12",
            });

            client.AddHeader("x-ss-id", response.SessionId);
            client.BearerToken = response.BearerToken;

            //Create request object for create a new booking
            var request = new CreateBooking
            {
               CompanyId = new Guid("00000000-0000-0000-0000-000000000001"),
               CustomerId = new Guid("00000000-0000-0000-0000-000000000010"),
               From = DateTime.Now,
               To = DateTime.Now.AddMinutes(60),
               AllowBookingOutsideSchedules = true,
               ServiceId = 1, //Klippning , kort hår - Kortbetalning (see https://www.bokamera.se/Demo1/BookTime?event=1)
               Resources = new System.Collections.Generic.List<ResourceToBook>()
               {
                    new ResourceToBook() { ResourceId = 1, ResourceTypeId = 1 },
                    new ResourceToBook() { ResourceId = 5, ResourceTypeId = 2 },
               }



            };


        }
        static void GetBookingsExample()
        {
            // Create and configure client
            // BokaMera API supports MsgPack, prefer that for efficient communication with the API.
            // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format
            // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
            var client = new JsonServiceClient(ApiUrlTest);
            client.Headers.Add("x-api-key", "YOUR_API_KEY_HERE");
            //Language settings sv = swedish, en = english (default)
            client.Headers.Add("x-language", "sv");

            //Authenticate the user
            var response = client.Post<AuthenticateResponse>("authenticate", new
            {
                UserName = "demo@bokamera.se",
                Password = "demo12",
            });

            client.AddHeader("x-ss-id", response.SessionId);
            client.BearerToken = response.BearerToken;

            //Create request object for retrieving bookings
            //See https://github.com/ServiceStack/ServiceStack/wiki/AutoQuery-RDBMS How to query on columns
            var request = new BookingQuery
            {
                CompanyBookings = true //Get All bookings for the company you are administrator for (the authenticated user)
                //Id = 370476, //Get a specific booking with the BookingId
                //IncludeBookedResources = true, //Include all booked resource information
                //IncludeCompanyInformation = true,  //Include the company information for the booking
                //IncludeCustomerInformation = true,  //Include the customer information for the booking
                //IncludeCustomFields = true,  //Include the custom fields for the booking if any exists
                //IncludeLog = true,  //Include the booking log (only allowed for adminstrators
                //IncludeCustomFieldValues = true,  //Include the selected values for the custom fields, if any exists
                //IncludeServiceInformation = true, //Include the service information for the booking
                //BookingStart = new DateTime(2017, 01, 01), //Will query bookings that are between BookingStart and BookingEnd (Default for BookingStart is DateTime.MinValue)
                //BookingEnd = new DateTime(2017, 01, 01), //Will query bookings that are between BookingStart and BookingEnd (Default for BookingStart is DateTime.MaxValue)
                //StatusIds = new int[] { 1, 2, 3 }, //Will get all bookings with the status 1,2 or 3
                //Take = 10, //Will get at most 10 bookings (user for paging)
                //Skip = 5, //Will skip 5 bookings  (user for paging)
                //OrderBy = "Id", //will order bookings by the property Id Asceding
                //OrderByDesc = "Id" //Will order bookings by the property Id Descending
            };

            // Call service, this uses Dto from BokaMera's nuget package to return typed responses.
            // Check the namespace BokaMera.API.ServiceModel.Dto.* for request and response classes
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

            var userName = "username";
            var password = "password";          
            var key = "secret_key";
            var oauth_token = "_";


            Action<HttpWebRequest> apiKey = r => r.Headers.Add("x-api-key", key);

            Console.WriteLine("Retrive authentication data...");
            var authResponce = 
                (ApiUrlTest + "/authenticate")
                    .PostJsonToUrl(new Authenticate { UserName = userName, Password = password }, apiKey)
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

            //client with credentials authentication on demand
            var clientWithCredentials = new JsonServiceClient(ApiUrlTest) { RequestFilter = apiKey };
            clientWithCredentials.OnAuthenticationRequired = () => 
            {
                clientWithCredentials.BearerToken =
                    (ApiUrlTest + "/authenticate")
                    .PostJsonToUrl(new Authenticate { UserName = userName, Password = password, }, apiKey)
                    .FromJson<AuthenticateResponse>().BearerToken;
            };

            Console.WriteLine("Send request with authentication on demand...");
            clientWithCredentials.Get(new CurrentUserQuery()).PrintDump();


            //client with fb token authentication on demand
            var clientWithFbToken = new JsonServiceClient(ApiUrlTest) { RequestFilter = apiKey };
            clientWithFbToken.OnAuthenticationRequired = () =>
            {
                clientWithFbToken.BearerToken =
                    (ApiUrlTest + "/authenticate")
                    .PostJsonToUrl(new Authenticate { provider = "facebook", oauth_token = oauth_token }, apiKey)
                    .FromJson<AuthenticateResponse>().BearerToken;
            };

            Console.WriteLine("Send request with fb token authentication on demand...");
            clientWithFbToken.Get(new CurrentUserQuery()).PrintDump();
        }
    }
}
