using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.HttpClients;
using Grouchy.HttpApi.Client.Testing;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;

namespace Grouchy.HttpApi.Client.Tests.Extensions
{
   public static class StubWebApiHostExtensions
   {
      public static DefaultHttpClient CreateClient(this StubHttpApi api, int timeoutMs = 3000)
      {
         var configuration = new HttpApiConfiguration { Uri = api.BaseUri, TimeoutMs = timeoutMs };
         return new DefaultHttpClient(configuration);
      }

      public static DisposableHttpClient CreateClientWithInstrumenting(this StubHttpApi api, IHttpClientEventCallback callback, int timeoutMs = 3000)
      {
         var configuration = new InstrumentingConfiguration { Uri = api.BaseUri,TimeoutMs = timeoutMs };
         var httpClient = new DefaultHttpClient(configuration);
                    
         return new DisposableHttpClient(httpClient, httpClient.AddInstrumenting(configuration, new []{callback}));
      }

      public static DisposableHttpClient CreateClientWithTagging(this StubHttpApi api, ISessionIdAccessor sessionIdAccessor, ICorrelationIdAccessor correlationIdAccessor, IOutboundRequestIdAccessor outboundRequestIdAccessor, IGenerateGuids guidGenerator, IApplicationInfo applicationInfo, int timeoutMs = 3000)
      {
         var configuration = new InstrumentingConfiguration { Uri = api.BaseUri,TimeoutMs = timeoutMs };
         var httpClient = new DefaultHttpClient(configuration);
                    
         return new DisposableHttpClient(httpClient, httpClient.AddTagging(sessionIdAccessor, correlationIdAccessor, outboundRequestIdAccessor, guidGenerator, applicationInfo));
      }

      public static DisposableHttpClient CreateClientWithRetrying(this StubHttpApi api, IHttpClientEventCallback callback, int retries, int delayMs, int timeoutMs = 3000)
      {
         var retryManager = new StubRetryManager {Delay = new StubRetryDelay(delayMs), Predicate = new StubRetryPredicate(retries)}; 

         var configuration = new RetryingConfiguration {Uri = api.BaseUri,TimeoutMs = timeoutMs, RetryPolicy = "default"};
         var httpClient = new DefaultHttpClient(configuration);
                    
         return new DisposableHttpClient(httpClient, httpClient.AddRetrying(configuration, retryManager, new []{callback}));
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