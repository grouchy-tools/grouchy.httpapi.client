namespace Burble.Tests.logging_scenarios
{
   using System.Linq;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class logging_get_request
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly HttpResponseMessage _response;

      public logging_get_request()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddLogging(_callback);

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
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe("/ping");
         lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_log_response_received()
      {
         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.ShouldNotBeNull();
         lastResponse.EventType.ShouldBe("HttpClientResponseReceived");
         lastResponse.Uri.ShouldBe("/ping");
         lastResponse.Method.ShouldBe("GET");
         lastResponse.StatusCode.ShouldBe(200);
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
