namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;

   public class HttpClientRetryAttempt
   {
      public string EventType => GetType().Name;

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public int Attempt { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public static HttpClientRetryAttempt Create(HttpRequestMessage request, int attempt)
      {
         return new HttpClientRetryAttempt
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.RequestUri.LocalPath,
            Method = request.Method.Method,
            Attempt = attempt
         };
      }
   }
}