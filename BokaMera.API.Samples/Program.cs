using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Text;

namespace BokaMera.API.Samples
{
   class Program
   {
      static void Main(string[] args)
      {
         // Create and configure client
         // BokaMera API supports MsgPack, prefer that for efficient communication
         // with the API.
         // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
         // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format

         var apiUrl = "http://testapi.bokamera.se/";
         //var client = new JsonServiceClient(apiUrl);
         var client = new MsgPackServiceClient(apiUrl);
         client.AlwaysSendBasicAuthHeader = true;

         // Authenticate
         client.Headers.Add("x-api-key", "somelongrandomkey");
         client.UserName = "demo@bokamera.se";
         client.Password = "demo12";

         // Call service, this uses Dto from BokaMera's nuget package to return typed responses
         Console.WriteLine("Calling service FindBookings...");
         var response = client.Get(new FindBookings { From = DateTime.UtcNow.AddYears(-1), To = DateTime.UtcNow });
         
         Console.WriteLine("Response received, printing output...");
         response.PrintDump();

         // Logout session
         Console.WriteLine("Logging out...press any key to quit");
         client.Post(new Authenticate { provider = "logout" });
         
         // In ServiceStack v4 you can use
         // See: http://stackoverflow.com/questions/14116882/how-to-logout-authenticated-user-in-servicestack
         //client.Post(new Authenticate { provider = AuthenticateService.LogoutAction });

         Console.ReadKey();
      }
   }
}
