using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;

namespace Burble.HttpClients
{
   public class DefaultHttpClient : IHttpClient, IDisposable
   {
      private const int DefaultTimeoutMs = 3000;
      
      private readonly HttpClient _httpClient;

      private bool _isDisposed;

      public DefaultHttpClient(IHttpApiConfiguration configuration)
      {
         if (configuration.Uri == null || !configuration.Uri.IsAbsoluteUri)
         {
            throw new ArgumentException("Uri is required and must be absolute");
         }
         
         _httpClient = new HttpClient
         {
            BaseAddress = configuration.Uri,
            Timeout = TimeSpan.FromMilliseconds(configuration.TimeoutMs ?? DefaultTimeoutMs)
         };
      }

      ~DefaultHttpClient()
      {
         Dispose(false);
      }
      
      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (request.RequestUri.IsAbsoluteUri)
         {
            throw new ArgumentException("RequestUri cannot be UriKind.Absolute", nameof(request));
         }

         var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
         return response;
      }
      
      void IDisposable.Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      
      private void Dispose(bool disposing)
      {
         if (_isDisposed)
         {
            return;
         }

         if (disposing)
         {
            _httpClient?.Dispose();
         }

         _isDisposed = true;
      }
   }
}