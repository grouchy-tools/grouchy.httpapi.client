using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
{
   public class CircuitBreakingHttpClientDecorator : IHttpClientDecorator
   {
      private readonly ICircuitBreakerManager _circuitBreakerManager;

      public CircuitBreakingHttpClientDecorator(ICircuitBreakerManager circuitBreakerManager)
      {
         _circuitBreakerManager = circuitBreakerManager;
      }
      
      public IHttpClient Decorate(
         IHttpClient httpClient,
         IHttpApiConfiguration httpApiConfiguration)
      {
         var httpApiWithCircuitBreaking = httpApiConfiguration as IHttpApiWithCircuitBreaking;

         if (httpApiWithCircuitBreaking is null) return httpClient;
         
         return httpClient.AddCircuitBreaking(httpApiWithCircuitBreaking, _circuitBreakerManager);
      }
   }
}