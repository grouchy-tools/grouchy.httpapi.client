namespace Burble.Tests
{
   using System.Net.Http;
   using Burble.Abstractions;

   public static class HttpContextExtensions
   {
      public static IHttpClient AddInstrumenting(this HttpClient httpClient, IHttpClientEventCallback callback)
      {
         return new HttpClientAdapter(httpClient).AddInstrumenting(callback);
      }

      public static IHttpClient AddRetrying(this HttpClient httpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         IHttpClientEventCallback callback)
      {
         return new HttpClientAdapter(httpClient).AddRetrying(retryPredicate, retryDelay, callback);
      }
   }
}
