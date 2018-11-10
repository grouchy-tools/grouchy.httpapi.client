using System.Collections.Generic;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Abstractions.Tagging;
using Grouchy.HttpApi.Client.HttpClients;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Grouchy.Resilience.Abstractions.Retrying;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.HttpApi.Client.Extensions
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
         ICircuitBreakerManager circuitBreakerManager)
      {
         return new CircuitBreakingHttpClient(
            httpClient,
            circuitBreakerManager.GetState(httpApiWithCircuitBreaking.CircuitBreakerPolicy));
      }

      public static IHttpClient AddThrottling(
         this IHttpClient httpClient,
         IHttpApiWithThrottling httpApiWithThrottling,
         IThrottleManager throttleManager)
      {
         return new ThrottlingHttpClient(
            httpClient,
            throttleManager.GetSync(httpApiWithThrottling.ThrottlePolicy));
      }

      public static IHttpClient AddRetrying(
         this IHttpClient httpClient,
         IHttpApiWithRetrying httpApiWithRetrying,
         IRetryManager retryManager,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         return new RetryingHttpClient(
            httpClient,
            httpApiWithRetrying,
            retryManager.GetPredicate(httpApiWithRetrying.RetryPolicy),
            retryManager.GetDelay(httpApiWithRetrying.RetryPolicy),
            callbacks);
      }

      public static IHttpClient AddTagging(
         this IHttpClient httpClient,
         ISessionIdAccessor sessionIdAccessor,
         ICorrelationIdAccessor correlationIdAccessor,
         IOutboundRequestIdAccessor outboundRequestIdAccessor,
         IGenerateGuids guidGenerator,
         IApplicationInfo applicationInfo)
      {
         return new TaggingHttpClient(
            httpClient,
            sessionIdAccessor,
            correlationIdAccessor,
            outboundRequestIdAccessor,
            guidGenerator,
            applicationInfo);
      }
   }
}
