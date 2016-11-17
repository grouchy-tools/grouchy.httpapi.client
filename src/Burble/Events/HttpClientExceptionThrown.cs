namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientExceptionThrown : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientExceptionThrown);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public long DurationMs { get; set; }

      public Exception Exception { get; set; }

      public static HttpClientExceptionThrown Create(string requestId, HttpRequestMessage request, Uri baseAddress, long durationMs, Exception exception)
      {
         return new HttpClientExceptionThrown
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.AbsoluteRequestUri(baseAddress).ToString(),
            Method = request.Method.Method,
            DurationMs = durationMs,
            Exception = exception
         };
      }
   }
}