namespace Burble
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Events;

   public static class HttpContextExtensions
   {
      public static Task<HttpResponseMessage> GetAsync(this IHttpClient httpClient, string requestUri)
      {
         var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

         return httpClient.SendAsync(request);
      }

      public static IHttpClient AddLogging(
         this HttpClient baseHttpClient,
         Action<HttpClientRequestInitiated> onInitiated,
         Action<HttpClientResponseReceived> onReceived,
         Action<HttpClientTimedOut> onTimeout,
         Action<HttpClientExceptionThrown> onException)
      {
         return new LoggingHttpClient(
            new SimpleHttpClient(baseHttpClient),
            onInitiated,
            onReceived,
            onTimeout,
            onException);
      }

      public static IHttpClient AddLogging(
         this IHttpClient httpClient,
         Action<HttpClientRequestInitiated> onInitiated,
         Action<HttpClientResponseReceived> onReceived,
         Action<HttpClientTimedOut> onTimeout,
         Action<HttpClientExceptionThrown> onException)
      {
         return new LoggingHttpClient(
            httpClient,
            onInitiated,
            onReceived,
            onTimeout,
            onException);
      }
   }
}
