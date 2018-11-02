using System;
using System.Linq;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.retrying_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class retry_event_is_extensible : scenario_base
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var retryManager = new StubRetryManager {Delay = new StubRetryDelay(10), Predicate = new StubRetryPredicate(1)}; 
         var baseHttpClient = new StubHttpClient(new Exception());
         var configuration = new RetryingConfiguration {RetryPolicy = "default"};
         var httpClient = baseHttpClient.AddRetrying(
            configuration,
            retryManager,
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
