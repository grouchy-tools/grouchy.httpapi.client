namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientRetryAttempt : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientRetryAttempt);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public int Attempt { get; set; }

      public static HttpClientRetryAttempt Create(HttpRequestMessage request, int attempt)
      {
         return new HttpClientRetryAttempt
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.RequestUri.ToString(),
            Method = request.Method.Method,
            Attempt = attempt
         };
      }
   }
}