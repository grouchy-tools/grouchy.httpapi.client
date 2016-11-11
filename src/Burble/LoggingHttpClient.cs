namespace Burble
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Events;
   using Burble.Exceptions;

   public class LoggingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpClientEventCallback _callback;

      public LoggingHttpClient(
         IHttpClient httpClient,
         IHttpClientEventCallback callback)
      {
         _httpClient = httpClient;
         _callback = callback;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         var requestId = request.EnsureRequestIdIsInHeaders();
         var stopwatch = Stopwatch.StartNew();

         try
         {
            _callback.Invoke(HttpClientRequestInitiated.Create(requestId, request));
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            _callback.Invoke(HttpClientResponseReceived.Create(requestId, response, stopwatch.ElapsedMilliseconds));

            return response;
         }
         catch (HttpClientTimeoutException)
         {
            _callback.Invoke(HttpClientTimedOut.Create(requestId, request, stopwatch.ElapsedMilliseconds));
            throw;
         }
         catch (Exception e)
         {
            _callback.Invoke(HttpClientExceptionThrown.Create(requestId, request, stopwatch.ElapsedMilliseconds, e));
            throw;
         }
      }
   }
}