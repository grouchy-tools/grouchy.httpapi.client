namespace Burble.Tests.instrumenting_scenarios
{
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Abstractions;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class base_address_missing_and_absolute_uri
   {
      private readonly HttpResponseMessage _response;

      public base_address_missing_and_absolute_uri()
      {
         var callback = new StubHttpClientEventCallback();

         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient())
         {
            var httpClient = baseHttpClient.AddInstrumenting(callback);

            _response = httpClient.GetAsync(webApi.BaseUri + "ping").Result;
         }
      }
      
      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public void should_return_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         content.ShouldBe("pong");
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
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
}
