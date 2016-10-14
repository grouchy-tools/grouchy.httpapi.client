namespace Burble.Tests.retrying_scenarios
{
   using System.Net.Http;
   using Burble.Abstractions;

   public class StubRetryPredicate : IRetryPredicate
   {
      private readonly int _retries;

      public StubRetryPredicate(int retries)
      {
         _retries = retries;
      }

      public bool ShouldRetry(int retryAttempt, HttpResponseMessage response)
      {
         if (response != null && response.IsSuccessStatusCode)
         {
            return false;
         }

         return retryAttempt <= _retries;
      }
   }
}
