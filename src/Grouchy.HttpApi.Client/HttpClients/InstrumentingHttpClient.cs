using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.Exceptions;
using Grouchy.HttpApi.Client.Events;
using Grouchy.HttpApi.Client.Extensions;

namespace Grouchy.HttpApi.Client.HttpClients
{
   public class InstrumentingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpApiWithInstrumenting _httpApiWithInstrumenting;
      private readonly IEnumerable<IHttpClientEventCallback> _callbacks;

      public InstrumentingHttpClient(
         IHttpClient httpClient,
         IHttpApiWithInstrumenting httpApiWithInstrumenting,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         _httpClient = httpClient;
         _httpApiWithInstrumenting = httpApiWithInstrumenting;
         _callbacks = callbacks;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         _callbacks.Invoke(HttpClientRequestInitiated.Create(request, _httpApiWithInstrumenting.Uri));

         var stopwatch = Stopwatch.StartNew();

         HttpResponseMessage response;
         try
         {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
         }
         catch (TaskCanceledException)
         {
            _callbacks.Invoke(HttpClientTimedOut.Create(request, _httpApiWithInstrumenting.Uri, stopwatch.ElapsedMilliseconds));
            throw new HttpClientTimeoutException(request.Method, new Uri(_httpApiWithInstrumenting.Uri, request.RequestUri));
         }
         catch (HttpRequestException e) when (IsServerUnavailable(e))
         {
            _callbacks.Invoke(HttpClientServerUnavailable.Create(request, _httpApiWithInstrumenting.Uri, stopwatch.ElapsedMilliseconds));
            throw new HttpClientServerUnavailableException(request.Method, new Uri(_httpApiWithInstrumenting.Uri, request.RequestUri));
         }
         catch (Exception e)
         {
            _callbacks.Invoke(HttpClientExceptionThrown.Create(request, _httpApiWithInstrumenting.Uri, stopwatch.ElapsedMilliseconds, e));
            throw new HttpClientException(request.Method, new Uri(_httpApiWithInstrumenting.Uri, request.RequestUri), e);
         }

         _callbacks.Invoke(HttpClientResponseReceived.Create(response, _httpApiWithInstrumenting.Uri, stopwatch.ElapsedMilliseconds));

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
            e.InnerException.Message == "A connection with the server could not be established" ||
            e.InnerException.Message == "Couldn't resolve host name" ||
            e.InnerException.Message == "Couldn't connect to server";
#endif
      }
   }
}