namespace Burble
{
   using System;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class SimpleHttpClient : IHttpClient
   {
      private readonly HttpClient _baseHttpClient;

      public SimpleHttpClient(HttpClient baseHttpClient)
      {
         _baseHttpClient = baseHttpClient;
      }

      public Uri BaseAddress => _baseHttpClient.BaseAddress;

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         return SendAsync(request, CancellationToken.None);
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (BaseAddress == null && !request.RequestUri.IsAbsoluteUri)
         {
            throw new ArgumentException("requestUri cannot be UriKind.Relative if BaseAddress has not been specified", nameof(request));
         }

         try
         {
            var response = await _baseHttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response;
         }
         catch (TaskCanceledException)
         {
            throw new HttpClientTimeoutException(request.Method, request.AbsoluteRequestUri(BaseAddress));
         }
         catch (HttpRequestException e) when (IsServerUnavailable(e))
         {
            throw new HttpClientServerUnavailableException(request.Method, request.AbsoluteRequestUri(BaseAddress));
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