namespace Burble.Tests.retrying_scenarios
{
   using System;
   using NUnit.Framework;
   using Shouldly;

   public class multiple_retry_with_exception
   {
      private const int ExpectedRetries = 3;

      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _exceptionToThrow;
      private readonly Exception _caughtException;

      public multiple_retry_with_exception()
      {
         _exceptionToThrow = new Exception();
         var baseHttpClient = new ExceptionHttpClient(_exceptionToThrow);
         var httpClient = baseHttpClient.AddRetrying(
            new StubRetryPredicate(ExpectedRetries),
            new StubRetryDelay(10),
            _callback);

         try
         {
            httpClient.GetAsync("/ping").Wait();
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
         _caughtException.ShouldBeOfType<AggregateException>();
         _caughtException.InnerException.ShouldBeSameAs(_exceptionToThrow);
      }
   }
}
