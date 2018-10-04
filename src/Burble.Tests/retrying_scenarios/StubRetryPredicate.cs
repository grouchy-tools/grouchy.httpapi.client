using System.Net.Http;
using Burble.Abstractions;

namespace Burble.Tests.retrying_scenarios
{
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
