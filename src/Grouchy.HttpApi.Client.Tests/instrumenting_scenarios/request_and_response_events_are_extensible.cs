using System.Linq;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_and_response_events_are_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task  setup_scenario()
      {
         using (var webApi = new PingHttpApi())
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback))
         {
            await httpClient.GetAsync("/ping");
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
   }
}
