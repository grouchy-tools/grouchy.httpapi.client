using System;
using System.Linq;
using Burble.Abstractions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.retrying_scenarios
{
   public class retry_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var baseHttpClient = new ExceptionHttpClient(new Exception());
         var httpClient = baseHttpClient.AddRetrying(
            new StubRetryPredicate(1),
            new StubRetryDelay(10),
            _callback);

         try
         {
            httpClient.GetAsync("/ping").Wait();
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
