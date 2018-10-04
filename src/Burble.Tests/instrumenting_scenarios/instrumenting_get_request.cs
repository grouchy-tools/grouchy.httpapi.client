using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Burble.Tests.instrumenting_scenarios
{
   public class instrumenting_get_request
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private HttpResponseMessage _response;

      private void act(string uri, string eventUri)
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            _eventUri = new Uri(webApi.BaseUri, eventUri).ToString();

            var httpClient = baseHttpClient.AddInstrumenting(_callback);

            _response = httpClient.GetAsync(uri).Result;
         }
      }

      public static IEnumerable<object[]> TestData { get; } = new[]
      {
         new object[] { "ping", "/ping" },
         new object[] { "/ping", "/ping" }
      };

      [TestCaseSource(nameof(TestData))]
      public void should_return_status_code_200(string uri, string eventUri)
      {
         act(uri, eventUri);

         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [TestCaseSource(nameof(TestData))]
      public void should_return_content(string uri, string eventUri)
      {
         act(uri, eventUri);

         var content = _response.Content.ReadAsStringAsync().Result;

         content.ShouldBe("pong");
      }

      [TestCaseSource(nameof(TestData))]
      public void should_log_request_initiated(string uri, string eventUri)
      {
         act(uri, eventUri);

         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe(_eventUri);
         lastRequest.Method.ShouldBe("GET");
      }

      [TestCaseSource(nameof(TestData))]
      public void should_log_response_received(string uri, string eventUri)
      {
         act(uri, eventUri);

         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.ShouldNotBeNull();
         lastResponse.EventType.ShouldBe("HttpClientResponseReceived");
         lastResponse.Uri.ShouldBe(_eventUri);
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
