namespace Burble.Events
{
   using System;
   using System.Net.Http;

   public class HttpClientRequestInitiated
   {
      public string EventType => GetType().Name;

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

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