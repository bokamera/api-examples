using System;
using BokaMera.API.ServiceModel.Dtos;
using ServiceStack;
using ServiceStack.Text;

namespace BokaMera.API.Samples
{
   class Program
   {
      static void Main(string[] args)
      {
         // See https://github.com/ServiceStack/ServiceStack/wiki/C%23-client
         var client = new JsonServiceClient("http://localhost:59600/");
         client.Headers.Add("x-api-key", "somelongrandomkey");
         client.AlwaysSendBasicAuthHeader = true;
         client.UserName = "demo@bokamera.se";
         client.Password = "demo12";

         var response = client.Get(new FindBookings { From = DateTime.UtcNow.AddYears(-1), To = DateTime.UtcNow });
         response.PrintDump();
         Console.ReadKey();
      }
   }
}
