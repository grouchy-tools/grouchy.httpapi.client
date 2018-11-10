using System.Collections.Generic;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
{
   public class RetryingHttpClientDecorator : IHttpClientDecorator
   {
      private readonly IRetryManager _retryManager;
      private readonly IEnumerable<IHttpClientEventCallback> _callbacks;

      public RetryingHttpClientDecorator(
         IRetryManager retryManager,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         _retryManager = retryManager;
         _callbacks = callbacks;
      }
      
      public IHttpClient Decorate(
         IHttpClient httpClient,
         IHttpApiConfiguration httpApiConfiguration)
      {
         var httpApiWithRetrying = httpApiConfiguration as IHttpApiWithRetrying;

         if (httpApiWithRetrying is null) return httpClient;

         return httpClient.AddRetrying(httpApiWithRetrying, _retryManager, _callbacks);
      }
   }
}