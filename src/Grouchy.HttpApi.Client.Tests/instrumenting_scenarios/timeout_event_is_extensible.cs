using System.Linq;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class timeout_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var webApi = new PingHttpApi { Latency = 2000 })
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback, timeoutMs: 50))
         {
            try
            {
               await httpClient.GetAsync("/ping");
            }
            catch
            {
               // Ignore the exception;
            }
         }
      }

      [Test]
      public void should_log_timed_out()
      {
         var lastRequest = _callback.TimeOuts.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientTimedOut");
      }
   }
}
