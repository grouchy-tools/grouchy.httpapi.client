using System;
using System.Collections.Generic;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Events;

namespace Grouchy.HttpApi.Client.Events
{
   public class HttpClientServerUnavailable : IHttpClientExceptionEvent
   {
      public string EventType => nameof(HttpClientServerUnavailable);

      public DateTimeOffset Timestamp { get; set; }

      public string Method { get; set; }

      public string TargetService { get; set; }

      public string Uri { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public long DurationMs { get; set; }

      public Exception Exception { get; } = null;

      public static HttpClientServerUnavailable Create(HttpRequestMessage request, string targetService, Uri baseAddress, long durationMs)
      {
         return new HttpClientServerUnavailable
         {
            Timestamp = DateTimeOffset.UtcNow,
            Method = request.Method.Method,
            TargetService = targetService,
            Uri = new Uri(baseAddress, request.RequestUri).ToString(),
            DurationMs = durationMs
         };
      }
   }
}