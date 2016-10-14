namespace Burble
{
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;

   public class ThrottlingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly SemaphoreSlim _semaphore;

      public ThrottlingHttpClient(
         IHttpClient httpClient,
         int concurrentRequests)
      {
         _httpClient = httpClient;
         _semaphore = new SemaphoreSlim(concurrentRequests);
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         try
         {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            return response;
         }
         finally
         {
            _semaphore.Release();
         }
      }
   }
}