using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions.Extensions;
using NUnit.Framework;
using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Burble.Tests.adapter_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class happy_path
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var webApi = new PingWebApi())
         using (var httpClient = webApi.CreateClient())
         {
            _response = await httpClient.GetAsync("/ping");
         }
      }
      
      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public async Task should_return_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

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
