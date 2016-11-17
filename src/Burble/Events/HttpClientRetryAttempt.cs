namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientRetryAttempt : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientRetryAttempt);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public int Attempt { get; set; }

      public static HttpClientRetryAttempt Create(string requestId, HttpRequestMessage request, Uri baseAddress, int attempt)
      {
         return new HttpClientRetryAttempt
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.AbsoluteRequestUri(baseAddress).ToString(),
            Method = request.Method.Method,
            Attempt = attempt
         };
      }
   }
}