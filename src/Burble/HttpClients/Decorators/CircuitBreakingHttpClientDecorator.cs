using System.Net;
using Burble.Abstractions;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;
using Burble.Extensions;

namespace Burble.HttpClients.Decorators
{
   public class CircuitBreakingHttpClientDecorator : IHttpClientDecorator
   {
      private readonly ICircuitBreakingStateManager<HttpStatusCode> _circuitBreakingStateManager;

      public CircuitBreakingHttpClientDecorator(ICircuitBreakingStateManager<HttpStatusCode> circuitBreakingStateManager)
      {
         _circuitBreakingStateManager = circuitBreakingStateManager;
      }
      
      public IHttpClient Decorate(
         IHttpClient httpClient,
         IHttpApiConfiguration httpApiConfiguration)
      {
         var httpApiWithCircuitBreaking = httpApiConfiguration as IHttpApiWithCircuitBreaking;

         if (httpApiWithCircuitBreaking is null) return httpClient;
         
         return httpClient.AddCircuitBreaking(httpApiWithCircuitBreaking, _circuitBreakingStateManager);
      }
   }
}