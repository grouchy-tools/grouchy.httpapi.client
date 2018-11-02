using System.Net.Http;
using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.HttpApi.Client.Tests.Stubs
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
