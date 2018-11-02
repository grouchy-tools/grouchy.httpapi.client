using System;
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
   public class multiple_retry_with_exception : scenario_base
   {
      private const int ExpectedRetries = 3;

      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      
      private Exception _exceptionToThrow;
      private Exception _caughtException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _exceptionToThrow = new Exception();
         var retryManager = new StubRetryManager {Delay = new StubRetryDelay(10), Predicate = new StubRetryPredicate(ExpectedRetries)}; 
         var baseHttpClient = new StubHttpClient(_exceptionToThrow);
         var configuration = new RetryingConfiguration {RetryPolicy = "default"};
         var httpClient = baseHttpClient.AddRetrying(configuration, retryManager, new[]{_callback});

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
