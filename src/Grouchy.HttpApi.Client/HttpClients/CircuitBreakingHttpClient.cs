using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Exceptions;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Client.HttpClients
{
   public class CircuitBreakingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly ICircuitBreakerState _circuitBreakerState;

      public CircuitBreakingHttpClient(
         IHttpClient httpClient,
         ICircuitBreakerState circuitBreakerState)
      {
         _httpClient = httpClient;
         _circuitBreakerState = circuitBreakerState;
      }
      
      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (_circuitBreakerState.ShouldAccept())
         {
            HttpResponseMessage response;
            try
            {
               response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is TaskCanceledException || e is HttpClientTimeoutException)
            {
               _circuitBreakerState.LogTimeout();
               throw;
            }
            catch (Exception e)
            {
               _circuitBreakerState.LogException(e);
               throw;
            }

            if ((int)response.StatusCode < 400)
            {
               _circuitBreakerState.LogSuccessResponse(response.StatusCode.ToString());
            }
            else
            {
               _circuitBreakerState.LogFailureResponse(response.StatusCode.ToString());
            }

            return response;
         }
         
         // TODO: Log circuit open event
         
         _circuitBreakerState.LogWithheld();
         
         return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
      }
   }
}