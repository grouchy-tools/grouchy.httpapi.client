namespace Burble
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Exceptions;

   public class SimpleHttpClient : IHttpClient
   {
      private readonly HttpClient _baseHttpClient;

      public SimpleHttpClient(HttpClient baseHttpClient)
      {
         _baseHttpClient = baseHttpClient;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         try
         {
            var response = await _baseHttpClient.SendAsync(request).ConfigureAwait(false);
            return response;
         }
         catch (TaskCanceledException)
         {
            throw new HttpClientTimeoutException(request);
         }
         catch (HttpRequestException e) when (e.InnerException.Message == "The server name or address could not be resolved")
         {
            throw new HttpClientServerUnavailableException(request);
         }
         catch (Exception e)
         {
            throw new HttpClientException(request, e);
         }
      }
   }
}