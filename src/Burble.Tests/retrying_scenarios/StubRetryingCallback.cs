namespace Burble.Tests.retrying_scenarios
{
   using System.Collections.Generic;
   using Burble.Events;
   using Burble.Retrying;

   public class StubRetryingCallback : IRetryingCallback
   {
      public List<HttpClientRetryAttempt> RetryAttempts { get; } = new List<HttpClientRetryAttempt>();

      public void OnRetry(HttpClientRetryAttempt @event)
      {
         RetryAttempts.Add(@event);
      }
   }
}