namespace Burble
{
   using System.Net.Http;
   using Burble.Abstractions;

   public static class HttpContextExtensions
   {
      public static IHttpClient AddInstrumenting(
         this HttpClient baseHttpClient,
         IHttpClientEventCallback callback)
      {
         return new InstrumentingHttpClient(
            new SimpleHttpClient(baseHttpClient),
            callback);
      }

      public static IHttpClient AddInstrumenting(
         this IHttpClient httpClient,
         IHttpClientEventCallback callback)
      {
         return new InstrumentingHttpClient(
            httpClient,
            callback);
      }

      public static IHttpClient AddThrottling(
         this IHttpClient httpClient,
         IThrottleSync throttleSync)
      {
         return new ThrottlingHttpClient(httpClient, throttleSync);
      }

      public static IHttpClient AddRetrying(
         this HttpClient baseHttpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         IHttpClientEventCallback callback)
      {
         return new RetryingHttpClient(
            new SimpleHttpClient(baseHttpClient),
            retryPredicate,
            retryDelay,
            callback);
      }

      public static IHttpClient AddRetrying(
         this IHttpClient httpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         IHttpClientEventCallback callback)
      {
         return new RetryingHttpClient(
            httpClient,
            retryPredicate,
            retryDelay,
            callback);
      }
   }
}
