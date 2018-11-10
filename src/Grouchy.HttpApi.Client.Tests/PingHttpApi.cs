using System.Net;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Testing;

#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Grouchy.HttpApi.Client.Tests
{
   public class PingHttpApi : StubHttpApi
   {
      public string Method { get; set; } = "GET";

      public int Latency { get; set; } = 0;
      
      protected override async Task Handler(HttpContext context)
      {
         if (context.Request.Method == Method && context.Request.Path.ToString() == "/ping")
         {
            if (Latency != 0)
            {
               await Task.Delay(Latency);
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync("pong");
         }
         else
         {
            await base.Handler(context);
         }
      }
   }
}