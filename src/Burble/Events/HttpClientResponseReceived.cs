using System;
using System.Collections.Generic;
using System.Net.Http;
using Burble.Abstractions;

namespace Burble.Events
{
   public class HttpClientResponseReceived : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientResponseReceived);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public HttpRequestMessage Request { get; set; }

      public long DurationMs { get; set; }

      public int StatusCode { get; set; }

      public static HttpClientResponseReceived Create(HttpResponseMessage response, Uri baseAddress, long durationMs)
      {
         return new HttpClientResponseReceived
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = new Uri(baseAddress, response.RequestMessage.RequestUri).ToString(),
            Request = response.RequestMessage,
            DurationMs = durationMs,
            StatusCode = (int)response.StatusCode
         };
      }
   }
}