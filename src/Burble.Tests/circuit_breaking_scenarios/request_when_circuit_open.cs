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
   public class request_when_circuit_open
   {
      private HttpResponseMessage _response;
      private StubHttpClient _baseHttpClient;
      private StubCircuitBreakingState _state;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _baseHttpClient = new StubHttpClient(new HttpResponseMessage());
         _state = new StubCircuitBreakingState { ShouldAcceptResponse = false };
         var stateCache = new StubCircuitBreakingStateManager { State = _state };
         var configuration = new CircuitBreakingConfiguration();
         var httpClient = _baseHttpClient.AddCircuitBreaking(configuration, stateCache);

         _response = await httpClient.GetAsync("/ping");
      }

      [Test]
      public void should_return_service_unavailable()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
      }
      
      [Test]
      public void should_not_call_base_http_client()
      {
         _baseHttpClient.SendAsyncCalledCount.ShouldBe(0);
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
      public void should_log_rejected()
      {
         _state.LogRejectedCalledCount.ShouldBe(1);
      }
      
      [Test]
      public void should_not_log_timeout()
      {
         _state.LogTimeoutCalledCount.ShouldBe(0);
      }
      
      [Test]
      public void should_not_log_exception()
      {
         _state.LogExceptionCalledCount.ShouldBe(0);
      }
   }
}
