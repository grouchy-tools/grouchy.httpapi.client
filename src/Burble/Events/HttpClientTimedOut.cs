namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;

   public class HttpClientTimedOut
   {
      public string EventType => GetType().Name;

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public long DurationMs { get; set; }

      public IDictionary<string, object> Tags { get; set; }

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