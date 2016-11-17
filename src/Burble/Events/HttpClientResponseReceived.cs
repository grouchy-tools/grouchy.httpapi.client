namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientResponseReceived : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientResponseReceived);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public long DurationMs { get; set; }

      public int StatusCode { get; set; }

      public static HttpClientResponseReceived Create(string requestId, HttpResponseMessage response, Uri baseAddress, long durationMs)
      {
         return new HttpClientResponseReceived
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Uri = response.RequestMessage.AbsoluteRequestUri(baseAddress).ToString(),
            Method = response.RequestMessage.Method.Method,
            DurationMs = durationMs,
            StatusCode = (int)response.StatusCode
         };
      }
   }
}