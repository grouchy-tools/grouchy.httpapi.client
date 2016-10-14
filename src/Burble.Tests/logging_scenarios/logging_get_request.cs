namespace Burble.Tests.logging_scenarios
{
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Events;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class logging_get_request
   {
      private readonly HttpResponseMessage _response;

      private HttpClientRequestInitiated _lastRequest;
      private HttpClientResponseReceived _lastResponse;

      public logging_get_request()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddLogging(
               e => { _lastRequest = e; },
               e => { _lastResponse = e; },
               e => { },
               e => { });

            _response = httpClient.GetAsync("/ping").Result;
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

      [Test]
      public void should_log_request_initiated()
      {
         _lastRequest.ShouldNotBeNull();
         _lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         _lastRequest.Uri.ShouldBe("/ping");
         _lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_log_response_received()
      {
         _lastResponse.ShouldNotBeNull();
         _lastResponse.EventType.ShouldBe("HttpClientResponseReceived");
         _lastResponse.Uri.ShouldBe("/ping");
         _lastResponse.Method.ShouldBe("GET");
         _lastResponse.StatusCode.ShouldBe(200);
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
