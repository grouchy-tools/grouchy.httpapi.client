namespace Burble.Tests
{
   using System.Collections.Generic;
   using Burble.Events;
   using Burble.Logging;
   using Burble.Retrying;

   public class StubLoggingCallback : ILoggingCallback, IRetryingCallback
   {
      public List<HttpClientRequestInitiated> RequestsInitiated { get; } = new List<HttpClientRequestInitiated>();
      public List<HttpClientResponseReceived> ResponsesReceived { get; } = new List<HttpClientResponseReceived>();
      public List<HttpClientTimedOut> TimeOuts { get; } = new List<HttpClientTimedOut>();
      public List<HttpClientExceptionThrown> ExceptionsThrown { get; } = new List<HttpClientExceptionThrown>();
      public List<HttpClientRetryAttempt> RetryAttempts { get; } = new List<HttpClientRetryAttempt>();

      public virtual void OnInitiated(HttpClientRequestInitiated @event)
      {
         RequestsInitiated.Add(@event);
      }

      public virtual void OnReceived(HttpClientResponseReceived @event)
      {
         ResponsesReceived.Add(@event);
      }

      public virtual void OnTimeout(HttpClientTimedOut @event)
      {
         TimeOuts.Add(@event);
      }

      public virtual void OnException(HttpClientExceptionThrown @event)
      {
         ExceptionsThrown.Add(@event);
      }

      public virtual void OnRetry(HttpClientRetryAttempt @event)
      {
         RetryAttempts.Add(@event);
      }
   }
}