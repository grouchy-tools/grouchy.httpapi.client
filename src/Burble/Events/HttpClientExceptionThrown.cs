namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;

   public class HttpClientExceptionThrown
   {
      public string EventType => GetType().Name;

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public long DurationMs { get; set; }

      public Exception Exception { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public static HttpClientExceptionThrown Create(HttpRequestMessage request, long durationMs, Exception exception)
      {
         return new HttpClientExceptionThrown
         {
            Timestamp = DateTimeOffset.UtcNow,
            Method = request.Method.Method,
            Uri = request.RequestUri.ToString(),
            DurationMs = durationMs,
            Exception = exception
         };
      }
   }
}