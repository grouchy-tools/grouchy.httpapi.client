namespace Burble.Tests.retrying_scenarios
{
   using System;
   using System.Linq;
   using NUnit.Framework;
   using Shouldly;

   public class retry_event_is_extensible
   {
      private readonly CustomisingLoggingCallback _callback = new CustomisingLoggingCallback();

      public retry_event_is_extensible()
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
         lastRequest.Tags["Key"].ShouldBe("RetryAttempt");
      }
   }
}
