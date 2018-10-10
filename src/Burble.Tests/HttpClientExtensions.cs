using System.Net.Http;
using Burble.Abstractions;
using Burble.Extensions;
using Burble.HttpClients;
using Burble.Retrying;

namespace Burble.Tests
{
   public static class HttpClientExtensions
   {
      public static IHttpClient AddInstrumenting(this HttpClient httpClient, IHttpClientEventCallback callback)
      {
         return new HttpClientAdapter(httpClient).AddInstrumenting(new[] {callback});
      }
      
      public static IHttpClient AddRetrying(this HttpClient httpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         IHttpClientEventCallback callback)
      {
         return new HttpClientAdapter(httpClient).AddRetrying(retryPredicate, retryDelay, new[] {callback});
      }
   }
}
