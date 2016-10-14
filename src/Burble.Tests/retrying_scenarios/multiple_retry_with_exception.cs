namespace Burble.Tests.retrying_scenarios
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Events;
   using NUnit.Framework;
   using Shouldly;

   public class multiple_retry_with_exception
   {
      private const int ExpectedRetries = 3;

      private readonly List<HttpClientRetryAttempt> _retryAttempts = new List<HttpClientRetryAttempt>();
      private readonly Exception _exceptionToThrow;
      private readonly Exception _caughtException;

      public multiple_retry_with_exception()
      {
         _exceptionToThrow = new Exception();
         var baseHttpClient = new ExceptionHttpClient(_exceptionToThrow);
         var httpClient = baseHttpClient.AddRetrying(
            new StubRetryPredicate(ExpectedRetries),
            new StubRetryDelay(10),
            e => { _retryAttempts.Add(e); });

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
         _retryAttempts.Count.ShouldBe(ExpectedRetries);
         _retryAttempts[0].Attempt.ShouldBe(1);
         _retryAttempts[1].Attempt.ShouldBe(2);
         _retryAttempts[2].Attempt.ShouldBe(3);
      }

      [Test]
      public void should_log_retry_attempt()
      {
         _retryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _retryAttempts[0].Uri.ShouldBe("/ping");
         _retryAttempts[0].Method.ShouldBe("GET");
      }

      [Test]
      public void should_throw_exception()
      {
         _caughtException.ShouldBeOfType<AggregateException>();
         _caughtException.InnerException.ShouldBeSameAs(_exceptionToThrow);
      }

      private class ExceptionHttpClient : IHttpClient
      {
         private readonly Exception _exception;

         public ExceptionHttpClient(Exception exception)
         {
            _exception = exception;
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            throw _exception;
         }
      }
   }
}
