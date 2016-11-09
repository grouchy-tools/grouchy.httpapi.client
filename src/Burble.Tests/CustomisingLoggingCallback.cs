namespace Burble.Tests
{
   using System.Collections.Generic;
   using Burble.Events;

   public class CustomisingLoggingCallback : StubLoggingCallback
   {
      public override void OnInitiated(HttpClientRequestInitiated @event)
      {
         base.OnInitiated(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", "Initiated" } };
      }

      public override void OnReceived(HttpClientResponseReceived @event)
      {
         base.OnReceived(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", "Received" } };
      }

      public override void OnException(HttpClientExceptionThrown @event)
      {
         base.OnException(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", "ExceptionThrown" } };
      }

      public override void OnTimeout(HttpClientTimedOut @event)
      {
         base.OnTimeout(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", "TimedOut" } };
      }

      public override void OnRetry(HttpClientRetryAttempt @event)
      {
         base.OnRetry(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", "RetryAttempt" } };
      }
   }
}