using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Burble.Abstractions.Extensions;
using Burble.CircuitBreaking;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.circuit_breaking_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_times_out
   {
      private Exception _exceptionToThrow;
      private Exception _caughtException;
      private StubCircuitBreakingState _state;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _exceptionToThrow = new TaskCanceledException();
         _state = new StubCircuitBreakingState { ShouldAcceptResponse = true };

         var baseHttpClient = new StubHttpClient(_exceptionToThrow);
         var stateCache = new StubCircuitBreakingStateManager { State = _state };
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
      public void should_log_timeout_once()
      {
         _state.LogTimeoutCalledCount.ShouldBe(1);
      }

      [Test]
      public void should_not_log_exception()
      {
         _state.LogExceptionCalledCount.ShouldBe(0);
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
