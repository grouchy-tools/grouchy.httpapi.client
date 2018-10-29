using System.Collections.Generic;
using System.Net;
using Burble.Abstractions;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;
using Burble.Abstractions.Identifying;
using Burble.HttpClients;

namespace Burble.Extensions
{
   public static class HttpClientExtensions
   {
      public static IHttpClient AddInstrumenting(
         this IHttpClient httpClient,
         IHttpApiWithInstrumenting httpApiWithInstrumenting,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         return new InstrumentingHttpClient(
            httpClient,
            httpApiWithInstrumenting,
            callbacks);
      }

      public static IHttpClient AddCircuitBreaking(
         this IHttpClient httpClient,
         IHttpApiWithCircuitBreaking httpApiWithCircuitBreaking,
         ICircuitBreakingStateManager<HttpStatusCode> circuitBreakingStateManager)
      {
         return new CircuitBreakingHttpClient(
            httpClient,
            circuitBreakingStateManager.Get(httpApiWithCircuitBreaking));
      }
      
      public static IHttpClient AddThrottling(
         this IHttpClient httpClient,
         IHttpApiWithThrottling httpApiWithThrottling)
      {
         return new ThrottlingHttpClient(httpClient, httpApiWithThrottling);
      }

      public static IHttpClient AddRetrying(
         this IHttpClient httpClient,
         IHttpApiWithRetrying httpApiWithRetrying,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         return new RetryingHttpClient(
            httpClient,
            httpApiWithRetrying,
            callbacks);
      }
      
      public static IHttpClient AddIdentifyingHeaders(
         this IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator,
         IApplicationInfo applicationInfo)
      {
         return new IdentifyingHttpClient(
            httpClient,
            correlationIdGetter,
            guidGenerator,
            applicationInfo);
      }
   }
}
