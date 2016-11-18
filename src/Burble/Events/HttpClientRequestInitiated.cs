namespace Burble.Events
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using Burble.Abstractions;

   public class HttpClientRequestInitiated : IHttpClientEvent
   {
      public string EventType => nameof(HttpClientRequestInitiated);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public HttpRequestMessage Request { get; set; }

      public static HttpClientRequestInitiated Create(HttpRequestMessage request, Uri baseAddress)
      {
         return new HttpClientRequestInitiated
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = request.AbsoluteRequestUri(baseAddress).ToString(),
            Request = request
         };
      }
   }
}