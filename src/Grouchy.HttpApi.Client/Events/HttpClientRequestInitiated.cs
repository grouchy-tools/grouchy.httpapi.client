using System;
using System.Collections.Generic;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Events;

namespace Grouchy.HttpApi.Client.Events
{
   public class HttpClientRequestInitiated : IHttpClientRequestEvent
   {
      public string EventType => nameof(HttpClientRequestInitiated);

      public DateTimeOffset Timestamp { get; set; }

      public string Method { get; set; }

      public string TargetService { get; set; }

      public string Uri { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public static HttpClientRequestInitiated Create(HttpRequestMessage request, string targetService, Uri baseAddress)
      {
         return new HttpClientRequestInitiated
         {
            Timestamp = DateTimeOffset.UtcNow,
            Method = request.Method.Method,
            TargetService = targetService,
            Uri = new Uri(baseAddress, request.RequestUri).ToString()
         };
      }
   }
}