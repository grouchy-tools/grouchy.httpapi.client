using System;
using System.Collections.Generic;
using System.Net.Http;
using Burble.Abstractions;

namespace Burble.Events
{
   public class HttpClientRetryAttempt : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientRetryAttempt);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public HttpRequestMessage Request { get; set; }

      public int Attempt { get; set; }

      public static HttpClientRetryAttempt Create(HttpRequestMessage request, Uri baseAddress, int attempt)
      {
         return new HttpClientRetryAttempt
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = new Uri(baseAddress, request.RequestUri).ToString(),
            Request = request,
            Attempt = attempt
         };
      }
   }
}