using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Burble.Events;
using Burble.Extensions;
using Burble.Retrying;

namespace Burble.HttpClients
{
   public class RetryingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpApiWithRetrying _httpApiWithRetrying;
      private readonly IEnumerable<IHttpClientEventCallback> _callbacks;

      public RetryingHttpClient(
         IHttpClient httpClient,
         IHttpApiWithRetrying httpApiWithRetrying,
         IEnumerable<IHttpClientEventCallback> callbacks)
      {
         _httpClient = httpClient;
         _httpApiWithRetrying = httpApiWithRetrying;
         _callbacks = callbacks;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         var retryAttempts = 0;

         try
         {
            while (true)
            {
               HttpResponseMessage response = null;
               Exception exception = null;

               try
               {
                  var clonedRequest = await CloneRequestAsync(request).ConfigureAwait(false);
                  response = await _httpClient.SendAsync(clonedRequest, cancellationToken).ConfigureAwait(false);
               }
               catch (Exception e)
               {
                  exception = e;
               }

               retryAttempts++;

               if (!_httpApiWithRetrying.RetryPredicate.ShouldRetry(retryAttempts, response))
               {
                  if (exception != null)
                  {
                     throw exception;
                  }

                  return response;
               }

               var delayMs = _httpApiWithRetrying.RetryDelay.DelayMs(retryAttempts);
               await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);

               _callbacks.Invoke(HttpClientRetryAttempt.Create(request, _httpApiWithRetrying.Uri, retryAttempts));
            }
         }
         finally
         {
            // Preserve compatibility with HttpClient.SendAsync by disposing of the original request
            request.Content?.Dispose();
         }
      }

      // From http://stackoverflow.com/questions/25044166/how-to-clone-a-httprequestmessage-when-the-original-request-has-content
      private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
      {
         var clone = new HttpRequestMessage(request.Method, request.RequestUri) { Version = request.Version };

         if (request.Content != null)
         {
            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();

            await request.Content.CopyToAsync(ms).ConfigureAwait(false);

            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            if (request.Content.Headers != null)
            {
               // Copy the content headers
               foreach (var header in request.Content.Headers)
               {
                  clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
               }
            }
         }

         // Copy the properties
         foreach (var property in request.Properties)
         {
            clone.Properties.Add(property);
         }

         // Copy the request headers
         foreach (var header in request.Headers)
         {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
         }

         return clone;
      }
   }
}