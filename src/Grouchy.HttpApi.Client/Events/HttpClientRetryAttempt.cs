using System;
using System.Collections.Generic;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Events;

namespace Grouchy.HttpApi.Client.Events
{
   public class HttpClientRetryAttempt : IHttpClientRetryEvent
   {
      public string EventType => nameof(HttpClientRetryAttempt);

      public DateTimeOffset Timestamp { get; set; }

      public string Method { get; set; }

      public string TargetService { get; set; }

      public string Uri { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public int Attempt { get; set; }

      public static HttpClientRetryAttempt Create(HttpRequestMessage request, string targetService, Uri baseAddress, int attempt)
      {
         return new HttpClientRetryAttempt
         {
            Timestamp = DateTimeOffset.UtcNow,
            Method = request.Method.Method,
            TargetService = targetService,
            Uri = new Uri(baseAddress, request.RequestUri).ToString(),
            Attempt = attempt
         };
      }
   }
}