namespace Burble.Events
{
   using System;
   using System.Collections.Generic;

   public interface IHttpClientEvent
   {
      string EventType { get; }

      DateTimeOffset Timestamp { get; }

      string Uri { get; }

      string Method { get; }

      IDictionary<string, object> Tags { get; set; }
   }
}