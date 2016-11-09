namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientTimedOut : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientTimedOut);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public long DurationMs { get; set; }

      public static HttpClientTimedOut Create(HttpRequestMessage request, long durationMs)
      {
         return new HttpClientTimedOut
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.RequestUri.LocalPath,
            Method = request.Method.Method,
            DurationMs = durationMs
         };
      }
   }
}