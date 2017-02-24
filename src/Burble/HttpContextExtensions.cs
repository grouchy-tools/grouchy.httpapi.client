namespace Burble
{
   using System.Net.Http;
   using Burble.Abstractions;

   public static class HttpContextExtensions
   {
      public static IHttpClient AddLogging(
         this HttpClient baseHttpClient,
         IHttpClientEventCallback callback)
      {
         return new LoggingHttpClient(
            new SimpleHttpClient(baseHttpClient),
            callback);
      }

      public static IHttpClient AddLogging(
         this IHttpClient httpClient,
         IHttpClientEventCallback callback)
      {
         return new LoggingHttpClient(
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
