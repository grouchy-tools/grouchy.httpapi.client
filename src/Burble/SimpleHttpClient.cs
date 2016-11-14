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
         catch (HttpRequestException e) when (IsServerUnavailable(e))
         {
            throw new HttpClientServerUnavailableException(request);
         }
         catch (Exception e)
         {
            throw new HttpClientException(request, e);
         }
      }

      private static bool IsServerUnavailable(HttpRequestException e)
      {
#if NET451
         return e.InnerException.Message.StartsWith("The remote name could not be resolved: ") ||
            e.InnerException.Message == "Unable to connect to the remote server";
#else
         return e.InnerException.Message == "The server name or address could not be resolved" ||
            e.InnerException.Message == "The connection with the server was terminated abnormally" ||
            e.InnerException.Message == "A connection with the server could not be established";
#endif
      }
   }
}