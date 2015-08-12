using System;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Text;

namespace BokaMera.API.Samples
{
   class Program
   {
      const string ApiUrl = "http://testapi.bokamera.se/";

      static void Main(string[] args)
      {
         // Create and configure client
         // BokaMera API supports MsgPack, prefer that for efficient communication with the API.
         // See https://github.com/ServiceStack/ServiceStack/wiki/MessagePack-Format
         // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
         //var client = new JsonServiceClient(apiUrl);
         var client = new MsgPackServiceClient(ApiUrl);

         // Authenticate
         client.Headers.Add("x-api-key", "somelongrandomkey");
         client.UserName = "demo@bokamera.se";
         client.Password = "demo12";

         // Call service, this uses Dto from BokaMera's nuget package to return typed responses.
         // Check the namespace BokaMera.API.ServiceModel.Dto.* for request and response classes
         Console.WriteLine("Calling service FindBookings...");
         var response = client.Get(new BokaMera.API.ServiceModel.Dto.GetApiVersion());
         
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
