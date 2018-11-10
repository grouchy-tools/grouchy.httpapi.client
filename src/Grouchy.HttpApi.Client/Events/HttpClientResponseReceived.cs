using System;
using System.Collections.Generic;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Events;

namespace Grouchy.HttpApi.Client.Events
{
   public class HttpClientResponseReceived : IHttpClientResponseEvent
   {
      public string EventType => nameof(HttpClientResponseReceived);

      public DateTimeOffset Timestamp { get; set; }

      public string Method { get; set; }

      public string TargetService { get; set; }

      public string Uri { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public long DurationMs { get; set; }

      public int StatusCode { get; set; }

      public static HttpClientResponseReceived Create(HttpResponseMessage response, string targetService, Uri baseAddress, long durationMs)
      {
         return new HttpClientResponseReceived
         {
            Timestamp = DateTimeOffset.UtcNow,
            Method = response.RequestMessage.Method.Method,
            TargetService = targetService,
            Uri = new Uri(baseAddress, response.RequestMessage.RequestUri).ToString(),
            DurationMs = durationMs,
            StatusCode = (int)response.StatusCode
         };
      }
   }
}