using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions;
using Burble.Extensions;
using Burble.HttpClients;
using Burble.Tests.adapter_scenarios;
using Burble.Tests.instrumenting_scenarios;
using Burble.Tests.retrying_scenarios;

namespace Burble.Tests
{
   public static class StubWebApiHostExtensions
   {
      public static DefaultHttpClient CreateClient(this StubWebApiHost host)
      {
         var configuration = new HttpApiConfiguration { Uri = host.BaseUri };
         return new DefaultHttpClient(configuration);
      }

      public static DisposableHttpClient CreateClientWithInstrumenting(this StubWebApiHost host, IHttpClientEventCallback callback, int timeoutMs = 3000)
      {
         var configuration = new InstrumentingConfiguration { Uri = host.BaseUri,TimeoutMs = timeoutMs };
         var httpClient = new DefaultHttpClient(configuration);
                    
         return new DisposableHttpClient(httpClient, httpClient.AddInstrumenting(configuration, new []{callback}));
      }
      
      public static DisposableHttpClient CreateClientWithRetrying(this StubWebApiHost host, IHttpClientEventCallback callback, int retries, int delayMs, int timeoutMs = 3000)
      {
         var configuration = new RetryingConfiguration(retries, delayMs) { Uri = host.BaseUri,TimeoutMs = timeoutMs };
         var httpClient = new DefaultHttpClient(configuration);
                    
         return new DisposableHttpClient(httpClient, httpClient.AddRetrying(configuration, new []{callback}));
      }

      public class DisposableHttpClient : IHttpClient, IDisposable
      {
         private readonly IDisposable _disposable;
         private readonly IHttpClient _httpClient;

         private bool _isDisposed;

         public DisposableHttpClient(IDisposable disposable, IHttpClient httpClient)
         {
            _disposable = disposable;
            _httpClient = httpClient;
         }

         ~DisposableHttpClient()
         {
            Dispose(false);
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            return _httpClient.SendAsync(request, cancellationToken);
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
               _disposable?.Dispose();
            }

            _isDisposed = true;
         }
      }
   }
}