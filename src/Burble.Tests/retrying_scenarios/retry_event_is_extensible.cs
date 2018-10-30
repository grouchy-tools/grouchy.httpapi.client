using System;
using System.Linq;
using System.Threading.Tasks;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.retrying_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class retry_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var baseHttpClient = new StubHttpClient(new Exception());
         var configuration = new RetryingConfiguration(retries: 1, delayMs: 10);
         var httpClient = baseHttpClient.AddRetrying(
            configuration,
            new[]{_callback});

         try
         {
            await httpClient.GetAsync("/ping");
         }
         catch
         {
            // Ignore the exception
         }
      }

      [Test]
      public void should_log_retry_attempt()
      {
         var lastRequest = _callback.RetryAttempts.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientRetryAttempt");
      }
   }
}
