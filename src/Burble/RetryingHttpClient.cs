namespace Burble
{
   using System;
   using System.IO;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Burble.Events;

   public class RetryingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IRetryPredicate _retryPredicate;
      private readonly IRetryDelay _retryDelay;
      private readonly Action<HttpClientRetryAttempt> _onRetry;

      public RetryingHttpClient(
         IHttpClient httpClient,
         IRetryPredicate retryPredicate,
         IRetryDelay retryDelay,
         Action<HttpClientRetryAttempt> onRetry)
      {
         _httpClient = httpClient;
         _retryPredicate = retryPredicate;
         _retryDelay = retryDelay;
         _onRetry = onRetry;
      }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
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
                  response = await _httpClient.SendAsync(clonedRequest).ConfigureAwait(false);
               }
               catch (Exception e)
               {
                  exception = e;
               }

               retryAttempts++;

               if (!_retryPredicate.ShouldRetry(retryAttempts, response))
               {
                  if (exception != null)
                  {
                     throw exception;
                  }

                  return response;
               }

               var delayMs = _retryDelay.DelayMs(retryAttempts);
               await Task.Delay(delayMs).ConfigureAwait(false);

               _onRetry(new HttpClientRetryAttempt
               {
                  Timestamp = DateTimeOffset.UtcNow,
                  Method = request.Method.Method,
                  Uri = request.RequestUri.ToString(),
                  Attempt = retryAttempts
               });
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
         var clone = new HttpRequestMessage(request.Method, request.RequestUri);

         // Copy the request's content (via a MemoryStream) into the cloned object
         var ms = new MemoryStream();

         if (request.Content != null)
         {
            await request.Content.CopyToAsync(ms).ConfigureAwait(false);

            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            // Copy the content headers
            if (request.Content.Headers != null)
            {
               foreach (var header in request.Content.Headers)
               {
                  clone.Content.Headers.Add(header.Key, header.Value);
               }
            }
         }

         clone.Version = request.Version;

         foreach (var property in request.Properties)
         {
            clone.Properties.Add(property);
         }

         foreach (var header in request.Headers)
         {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
         }

         return clone;
      }
   }
}