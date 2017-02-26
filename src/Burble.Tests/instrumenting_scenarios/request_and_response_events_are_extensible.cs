namespace Burble.Tests.instrumenting_scenarios
{
   using System.Linq;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Abstractions;
   using Microsoft.AspNetCore.Http;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else

#endif

   public class request_and_response_events_are_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      public request_and_response_events_are_extensible()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddInstrumenting(_callback);

            httpClient.GetAsync("/ping").Wait();
         }
      }
      
      [Test]
      public void should_log_request_initiated()
      {
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientRequestInitiated");
      }

      [Test]
      public void should_log_response_received()
      {
         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.Tags.Count.ShouldBe(1);
         lastResponse.Tags["Key"].ShouldBe("HttpClientResponseReceived");
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
