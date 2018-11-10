using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Exceptions;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_is_cancelled
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private Exception _timeoutException;

      [OneTimeSetUp]
      public async Task  setup_scenario()
      {
         using (var webApi = new PingHttpApi { Latency = 2000 })
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback))
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            try
            {
               await httpClient.GetAsync("/ping", new CancellationTokenSource(TimeSpan.FromMilliseconds(20)).Token);
            }
            catch (Exception e)
            {
               _timeoutException = e;
            }
         }
      }

      [Test]
      public void should_log_request_initiated()
      {
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe(_eventUri);
         lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_not_log_response()
      {
         _callback.ResponsesReceived.ShouldBeEmpty();
      }

      [Test]
      public void should_log_timeout_received()
      {
         var lastTimeout = _callback.TimeOuts.Last();
         lastTimeout.ShouldNotBeNull();
         lastTimeout.EventType.ShouldBe("HttpClientTimedOut");
         lastTimeout.Uri.ShouldBe(_eventUri);
         lastTimeout.Method.ShouldBe("GET");
      }
      
      [Test]
      public void should_throw_http_client_timeout_exception()
      {
         _timeoutException.ShouldBeOfType<HttpClientTimeoutException>();
      }
   }
}
