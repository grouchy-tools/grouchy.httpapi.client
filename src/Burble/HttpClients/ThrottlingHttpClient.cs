using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;

namespace Burble.HttpClients
{
   public class ThrottlingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpApiWithThrottling _httpApiWithThrottling;

      public ThrottlingHttpClient(
         IHttpClient httpClient,
         IHttpApiWithThrottling httpApiWithThrottling)
      {
         _httpClient = httpClient;
         _httpApiWithThrottling = httpApiWithThrottling;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         try
         {
            await _httpApiWithThrottling.ThrottleSync.WaitAsync().ConfigureAwait(false);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response;
         }
         finally
         {
            _httpApiWithThrottling.ThrottleSync.Release();
         }
      }
   }
}