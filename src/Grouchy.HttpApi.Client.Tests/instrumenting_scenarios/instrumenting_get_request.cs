using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class instrumenting_get_request
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private HttpResponseMessage _response;

      private async Task act(string uri, string eventUri)
      {
         using (var webApi = new PingHttpApi())
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback))
         {
            _eventUri = new Uri(webApi.BaseUri, eventUri).ToString();

            _response = await httpClient.GetAsync(uri);
         }
      }

      public static IEnumerable<object[]> TestData { get; } = new[]
      {
         new object[] { "ping", "/ping" },
         new object[] { "/ping", "/ping" }
      };

      [TestCaseSource(nameof(TestData))]
      public async Task should_return_status_code_200(string uri, string eventUri)
      {
         await act(uri, eventUri);

         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [TestCaseSource(nameof(TestData))]
      public async Task should_return_content(string uri, string eventUri)
      {
         await act(uri, eventUri);

         var content = await _response.Content.ReadAsStringAsync();

         content.ShouldBe("pong");
      }

      [TestCaseSource(nameof(TestData))]
      public async Task should_log_request_initiated(string uri, string eventUri)
      {
         await act(uri, eventUri);

         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe(_eventUri);
         lastRequest.Method.ShouldBe("GET");
      }

      [TestCaseSource(nameof(TestData))]
      public async Task should_log_response_received(string uri, string eventUri)
      {
         await act(uri, eventUri);

         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.ShouldNotBeNull();
         lastResponse.EventType.ShouldBe("HttpClientResponseReceived");
         lastResponse.Uri.ShouldBe(_eventUri);
         lastResponse.Method.ShouldBe("GET");
         lastResponse.StatusCode.ShouldBe(200);
      }
   }
}
