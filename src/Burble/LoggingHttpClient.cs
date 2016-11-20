namespace Burble
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading;
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

      public Uri BaseAddress => _httpClient.BaseAddress;

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         return SendAsync(request, CancellationToken.None);
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         _callback.Invoke(HttpClientRequestInitiated.Create(request, BaseAddress));

         var stopwatch = Stopwatch.StartNew();

         HttpResponseMessage response;
         try
         {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
         }
         catch (HttpClientTimeoutException)
         {
            _callback.Invoke(HttpClientTimedOut.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds));
            throw;
         }
         catch (HttpClientServerUnavailableException)
         {
            _callback.Invoke(HttpClientServerUnavailable.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds));
            throw;
         }
         catch (Exception e)
         {
            _callback.Invoke(HttpClientExceptionThrown.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds, e));
            throw new HttpClientException(request.Method, request.AbsoluteRequestUri(BaseAddress), e);
         }

         _callback.Invoke(HttpClientResponseReceived.Create(response, BaseAddress, stopwatch.ElapsedMilliseconds));

         return response;
      }
   }
}