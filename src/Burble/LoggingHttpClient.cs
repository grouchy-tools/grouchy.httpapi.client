namespace Burble
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Events;
   using Burble.Exceptions;

   public class LoggingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly Action<HttpClientRequestInitiated> _onInitiated;
      private readonly Action<HttpClientResponseReceived> _onReceived;
      private readonly Action<HttpClientTimedOut> _onTimeout;
      private readonly Action<HttpClientExceptionThrown> _onException;

      public LoggingHttpClient(
         IHttpClient httpClient,
         Action<HttpClientRequestInitiated> onInitiated,
         Action<HttpClientResponseReceived> onReceived,
         Action<HttpClientTimedOut> onTimeout,
         Action<HttpClientExceptionThrown> onException)
      {
         _httpClient = httpClient;
         _onInitiated = onInitiated;
         _onReceived = onReceived;
         _onTimeout = onTimeout;
         _onException = onException;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         var stopwatch = Stopwatch.StartNew();

         try
         {
            _onInitiated(HttpClientRequestInitiated.Create(request));
            var response = await _httpClient.SendAsync(request);
            _onReceived(HttpClientResponseReceived.Create(response, stopwatch.ElapsedMilliseconds));
            return response;
         }
         catch (TaskCanceledException)
         {
            _onTimeout(HttpClientTimedOut.Create(request, stopwatch.ElapsedMilliseconds));
            throw new HttpClientTimeoutException();
         }
         catch (Exception e)
         {
            _onException(HttpClientExceptionThrown.Create(request, stopwatch.ElapsedMilliseconds, e));
            throw new HttpClientException($"Failed to call {request.RequestUri}", e);
         }
      }
   }
}