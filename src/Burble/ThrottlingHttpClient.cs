namespace Burble
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class ThrottlingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IThrottleSync _throttleSync;

      public ThrottlingHttpClient(
         IHttpClient httpClient,
         IThrottleSync throttleSync)
      {
         _httpClient = httpClient;
         _throttleSync = throttleSync;
      }

      public Uri BaseAddress => _httpClient.BaseAddress;

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         try
         {
            await _throttleSync.WaitAsync().ConfigureAwait(false);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            return response;
         }
         finally
         {
            _throttleSync.Release();
         }
      }
   }
}