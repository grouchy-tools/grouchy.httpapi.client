using System;
using System.Threading.Tasks;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.retrying_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class multiple_retry_with_exception
   {
      private const int ExpectedRetries = 3;

      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      
      private Exception _exceptionToThrow;
      private Exception _caughtException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _exceptionToThrow = new Exception();
         var baseHttpClient = new StubHttpClient(_exceptionToThrow);
         var configuration = new RetryingConfiguration(retries: ExpectedRetries, delayMs: 10);
         var httpClient = baseHttpClient.AddRetrying(
            configuration,
            new[]{_callback});

         try
         {
            await httpClient.GetAsync("/ping");
         }
         catch (Exception e)
         {
            _caughtException = e;
         }
      }

      [Test]
      public void should_log_three_attempts()
      {
         _callback.RetryAttempts.Length.ShouldBe(ExpectedRetries);
         _callback.RetryAttempts[0].Attempt.ShouldBe(1);
         _callback.RetryAttempts[1].Attempt.ShouldBe(2);
         _callback.RetryAttempts[2].Attempt.ShouldBe(3);
      }

      [Test]
      public void should_log_retry_attempt()
      {
         _callback.RetryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _callback.RetryAttempts[0].Uri.ShouldBe("http://exception-host/ping");
         _callback.RetryAttempts[0].Method.ShouldBe("GET");
      }

      [Test]
      public void should_throw_exception()
      {
         _caughtException.ShouldBeSameAs(_exceptionToThrow);
      }
   }
}
