namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientExceptionThrown : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientExceptionThrown);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public long DurationMs { get; set; }

      public Exception Exception { get; set; }

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