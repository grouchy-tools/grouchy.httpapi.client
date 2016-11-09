namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;

   public class HttpClientRequestInitiated : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientRequestInitiated);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public static HttpClientRequestInitiated Create(HttpRequestMessage request)
      {
         return new HttpClientRequestInitiated
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.RequestUri.ToString(),
            Method = request.Method.Method
         };
      }
   }
}