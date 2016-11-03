namespace Burble.Tests.logging_scenarios
{
   using System.Collections.Generic;
   using Burble.Events;
   using Burble.Logging;

   public class StubLoggingCallback : ILoggingCallback
   {
      public List<HttpClientRequestInitiated> RequestsInitiated { get; } = new List<HttpClientRequestInitiated>();
      public List<HttpClientResponseReceived> ResponsesReceived { get; } = new List<HttpClientResponseReceived>();
      public List<HttpClientTimedOut> TimeOuts { get; } = new List<HttpClientTimedOut>();
      public List<HttpClientExceptionThrown> ExceptionsThrown { get; } = new List<HttpClientExceptionThrown>();

      public void OnInitiated(HttpClientRequestInitiated @event)
      {
         RequestsInitiated.Add(@event);
      }

      public void OnReceived(HttpClientResponseReceived @event)
      {
         ResponsesReceived.Add(@event);
      }

      public void OnTimeout(HttpClientTimedOut @event)
      {
         TimeOuts.Add(@event);
      }

      public void OnException(HttpClientExceptionThrown @event)
      {
         ExceptionsThrown.Add(@event);
      }
   }
}