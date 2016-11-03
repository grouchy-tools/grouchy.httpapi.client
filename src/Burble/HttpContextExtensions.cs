namespace Burble
{
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Logging;
   using Burble.Retrying;

   public static class HttpContextExtensions
   {
      public static Task<HttpResponseMessage> GetAsync(
         this IHttpClient httpClient,
         string requestUri)
      {
         var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

         return httpClient.SendAsync(request);
      }

      public static IHttpClient AddLogging(
         this HttpClient baseHttpClient,
         ILoggingCallback callback)
      {
         return new LoggingHttpClient(
            new SimpleHttpClient(baseHttpClient),
            callback);
      }

      public static IHttpClient AddLogging(
         this IHttpClient httpClient,
         ILoggingCallback callback)
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
         IRetryingCallback callback)
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
         IRetryingCallback callback)
      {
         return new RetryingHttpClient(
            httpClient,
            retryPredicate,
            retryDelay,
            callback);
      }
   }
}
