using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.HttpApi.Client.HttpClients
{
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

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         try
         {
            await _throttleSync.WaitAsync().ConfigureAwait(false);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response;
         }
         finally
         {
            _throttleSync.Release();
         }
      }
   }
}