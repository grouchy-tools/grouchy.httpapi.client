using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Tests.Extensions;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class instrumenting_post_request
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task  setup_scenario()
      {
         using (var webApi = new PingHttpApi { Method = "POST" })
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback))
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, "/ping");

            _response = await httpClient.SendAsync(request, CancellationToken.None);
         }
      }
      
      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public async Task  should_return_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

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

   }
}
