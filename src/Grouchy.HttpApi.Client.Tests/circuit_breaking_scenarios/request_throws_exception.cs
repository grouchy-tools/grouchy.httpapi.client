using System;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.circuit_breaking_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_throws_exception
   {
      private Exception _exceptionToThrow;
      private Exception _caughtException;
      private StubCircuitBreakerState _state;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _exceptionToThrow = new Exception();
         _state = new StubCircuitBreakerState { ShouldAcceptResponse = true };

         var baseHttpClient = new StubHttpClient(_exceptionToThrow);
         var stateCache = new StubCircuitBreakerManager { State = _state };
         var configuration = new CircuitBreakingConfiguration();
         var httpClient = baseHttpClient.AddCircuitBreaking(configuration, stateCache);

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
      public void should_throw_exception()
      {
         _caughtException.ShouldBeSameAs(_exceptionToThrow);
      }

      [Test]
      public void should_log_exception_once()
      {
         _state.LogExceptionCalledCount.ShouldBe(1);
      }

      [Test]
      public void should_not_log_timeout()
      {
         _state.LogTimeoutCalledCount.ShouldBe(0);
      }

      [Test]
      public void should_not_log_success_response()
      {
         _state.LogSuccessResponseCalledCount.ShouldBe(0);
      }

      [Test]
      public void should_not_log_failure_response()
      {
         _state.LogFailureResponseCalledCount.ShouldBe(0);
      }

      [Test]
      public void should_not_log_rejected()
      {
         _state.LogRejectedCalledCount.ShouldBe(0);
      }
   }
}
