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

      public string Method => Request.Method.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public HttpRequestMessage Request { get; set; }

      public long DurationMs { get; set; }

      public static HttpClientTimedOut Create(HttpRequestMessage request, Uri baseAddress, long durationMs)
      {
         return new HttpClientTimedOut
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.AbsoluteRequestUri(baseAddress).ToString(),
            Request = request,
            DurationMs = durationMs
         };
      }
   }
}