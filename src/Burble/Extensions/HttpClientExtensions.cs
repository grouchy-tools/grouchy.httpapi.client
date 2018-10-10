using System.Collections.Generic;
using Burble.Abstractions;
using Burble.HttpClients;
using Burble.Retrying;
using Burble.Throttling;

namespace Burble.Extensions
{
   public static class HttpClientExtensions
   {
      public static IHttpClient AddInstrumenting(
         this IHttpClient httpClient,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         return new InstrumentingHttpClient(
            httpClient,
            callbacks);
      }

      public static IHttpClient AddThrottling(
         this IHttpClient httpClient,
         IThrottleSync throttleSync)
      {
         return new ThrottlingHttpClient(httpClient, throttleSync);
      }

      public static IHttpClient AddRetrying(
         this IHttpClient httpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         return new RetryingHttpClient(
            httpClient,
            retryPredicate,
            retryDelay,
            callbacks);
      }
   }
}
