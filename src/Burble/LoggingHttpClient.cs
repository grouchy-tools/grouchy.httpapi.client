namespace Burble
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Events;
   using Burble.Exceptions;
   using Burble.Logging;

   public class LoggingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly ILoggingCallback _callback;

      public LoggingHttpClient(
         IHttpClient httpClient,
         ILoggingCallback callback)
      {
         _httpClient = httpClient;
         _callback = callback;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         var stopwatch = Stopwatch.StartNew();

         try
         {
            _callback.OnInitiated(HttpClientRequestInitiated.Create(request));
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            _callback.OnReceived(HttpClientResponseReceived.Create(response, stopwatch.ElapsedMilliseconds));
            return response;
         }
         catch (HttpClientTimeoutException)
         {
            _callback.OnTimeout(HttpClientTimedOut.Create(request, stopwatch.ElapsedMilliseconds));
            throw;
         }
         catch (Exception e)
         {
            _callback.OnException(HttpClientExceptionThrown.Create(request, stopwatch.ElapsedMilliseconds, e));
            throw;
         }
      }
   }
}