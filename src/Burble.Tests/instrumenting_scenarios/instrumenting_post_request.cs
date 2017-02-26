namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Linq;
   using System.Net;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Banshee;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class instrumenting_post_request
   {
      private readonly string _eventUri;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly HttpResponseMessage _response;

      public instrumenting_post_request()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            var httpClient = baseHttpClient.AddInstrumenting(_callback);

            var request = new HttpRequestMessage(HttpMethod.Post, "/ping");

            _response = httpClient.SendAsync(request, CancellationToken.None).Result;
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
         lastRequest.Uri.ShouldBe(_eventUri);
         lastRequest.Method.ShouldBe("POST");
      }

      [Test]
      public void should_log_response_received()
      {
         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.ShouldNotBeNull();
         lastResponse.EventType.ShouldBe("HttpClientResponseReceived");
         lastResponse.Uri.ShouldBe(_eventUri);
         lastResponse.Method.ShouldBe("POST");
         lastResponse.StatusCode.ShouldBe(200);
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "POST" && context.Request.Path.ToString() == "/ping")
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
