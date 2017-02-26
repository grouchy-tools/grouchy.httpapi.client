namespace Burble
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Events;

   public class InstrumentingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpClientEventCallback _callback;

      public InstrumentingHttpClient(
         IHttpClient httpClient,
         IHttpClientEventCallback callback)
      {
         _httpClient = httpClient;
         _callback = callback;
      }

      public Uri BaseAddress => _httpClient.BaseAddress;

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (BaseAddress == null && !request.RequestUri.IsAbsoluteUri)
         {
            throw new ArgumentException("requestUri cannot be UriKind.Relative if BaseAddress has not been specified", nameof(request));
         }

         _callback.Invoke(HttpClientRequestInitiated.Create(request, BaseAddress));

         var stopwatch = Stopwatch.StartNew();

         HttpResponseMessage response;
         try
         {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
         }
         catch (TaskCanceledException)
         {
            _callback.Invoke(HttpClientTimedOut.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds));
            throw new HttpClientTimeoutException(request.Method, request.AbsoluteRequestUri(BaseAddress));
         }
         catch (HttpRequestException e) when (IsServerUnavailable(e))
         {
            _callback.Invoke(HttpClientServerUnavailable.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds));
            throw new HttpClientServerUnavailableException(request.Method, request.AbsoluteRequestUri(BaseAddress));
         }
         catch (Exception e)
         {
            _callback.Invoke(HttpClientExceptionThrown.Create(request, BaseAddress, stopwatch.ElapsedMilliseconds, e));
            throw new HttpClientException(request.Method, request.AbsoluteRequestUri(BaseAddress), e);
         }

         _callback.Invoke(HttpClientResponseReceived.Create(response, BaseAddress, stopwatch.ElapsedMilliseconds));

         return response;
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