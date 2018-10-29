using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Exceptions;

namespace Burble.HttpClients
{
   public class CircuitBreakingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly ICircuitBreakingState<HttpStatusCode> _circuitBreakingState;

      public CircuitBreakingHttpClient(
         IHttpClient httpClient,
         ICircuitBreakingState<HttpStatusCode> circuitBreakingState)
      {
         _httpClient = httpClient;
         _circuitBreakingState = circuitBreakingState;
      }
      
      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (_circuitBreakingState.ShouldAccept())
         {
            HttpResponseMessage response;
            try
            {
               response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is TaskCanceledException || e is HttpClientTimeoutException)
            {
               _circuitBreakingState.LogTimeout();
               throw;
            }
            catch (Exception e)
            {
               _circuitBreakingState.LogException(e);
               throw;
            }

            if ((int)response.StatusCode < 400)
            {
               _circuitBreakingState.LogSuccessResponse(response.StatusCode);
            }
            else
            {
               _circuitBreakingState.LogFailureResponse(response.StatusCode);
            }

            return response;
         }
         
         // TODO: Log circuit open event
         
         _circuitBreakingState.LogWithheld();
         
         return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
      }
   }
}