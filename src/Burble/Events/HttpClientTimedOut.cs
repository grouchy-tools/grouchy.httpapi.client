namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientTimedOut : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientTimedOut);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public long DurationMs { get; set; }

      public static HttpClientTimedOut Create(string requestId, HttpRequestMessage request, Uri baseAddress, long durationMs)
      {
         return new HttpClientTimedOut
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.AbsoluteRequestUri(baseAddress).ToString(),
            Method = request.Method.Method,
            DurationMs = durationMs
         };
      }
   }
}