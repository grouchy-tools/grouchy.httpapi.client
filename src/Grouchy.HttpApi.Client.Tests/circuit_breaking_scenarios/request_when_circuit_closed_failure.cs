using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.circuit_breaking_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_when_circuit_closed_failure
   {
      private HttpResponseMessage _expectedResponse;
      private HttpResponseMessage _actualResponse;
      private StubHttpClient _baseHttpClient;
      private StubCircuitBreakerState _state;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
         _baseHttpClient = new StubHttpClient(_expectedResponse);
         _state = new StubCircuitBreakerState { ShouldAcceptResponse = true };
         var stateCache = new StubCircuitBreakerManager { State = _state };
         var configuration = new CircuitBreakingConfiguration();
         var httpClient = _baseHttpClient.AddCircuitBreaking(configuration, stateCache);

         _actualResponse = await httpClient.GetAsync("/ping");
      }

      [Test]
      public void should_return_response_from_base_http_client()
      {
         _actualResponse.ShouldBeSameAs(_expectedResponse);
      }
      
      [Test]
      public void should_call_base_http_client_once()
      {
         _baseHttpClient.SendAsyncCalledCount.ShouldBe(1);
      }

      [Test]
      public void should_log_failure_response()
      {
         _state.LogFailureResponseCalledCount.ShouldBe(1);
      }

      [Test]
      public void should_not_log_success_response()
      {
         _state.LogSuccessResponseCalledCount.ShouldBe(0);
      }
      
      [Test]
      public void should_log_response_from_base_http_client()
      {
         _state.LastFailureResponse.ShouldBe(_expectedResponse.StatusCode.ToString());
      }
      
      [Test]
      public void should_not_log_rejected()
      {
         _state.LogRejectedCalledCount.ShouldBe(0);
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
