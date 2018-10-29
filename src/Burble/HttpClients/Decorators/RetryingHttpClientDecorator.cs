using System.Collections.Generic;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Burble.Extensions;

namespace Burble.HttpClients.Decorators
{
   public class RetryingHttpClientDecorator : IHttpClientDecorator
   {
      private readonly IEnumerable<IHttpClientEventCallback> _callbacks;

      public RetryingHttpClientDecorator(IEnumerable<IHttpClientEventCallback> callbacks)
      {
         _callbacks = callbacks;
      }
      
      public IHttpClient Decorate(
         IHttpClient httpClient,
         IHttpApiConfiguration httpApiConfiguration)
      {
         var httpApiWithRetrying = httpApiConfiguration as IHttpApiWithRetrying;

         if (httpApiWithRetrying is null) return httpClient;

         return httpClient.AddRetrying(httpApiWithRetrying, _callbacks);
      }
   }
}