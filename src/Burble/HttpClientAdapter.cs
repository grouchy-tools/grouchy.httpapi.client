namespace Burble
{
   using System;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class HttpClientAdapter : IHttpClient
   {
      private readonly HttpClient _baseHttpClient;

      public HttpClientAdapter(HttpClient baseHttpClient)
      {
         _baseHttpClient = baseHttpClient;
      }

      public Uri BaseAddress => _baseHttpClient.BaseAddress;

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         var response = await _baseHttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
         return response;
      }
   }
}