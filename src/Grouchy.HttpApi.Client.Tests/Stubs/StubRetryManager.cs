using System;
using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubRetryManager : IRetryManager
   {
      public IRetryDelay Delay { get; set; }
      
      public IRetryPredicate Predicate { get; set; }

      public void Register(string policy, IRetryDelay retryDelay, IRetryPredicate retryPredicate)
      {
         throw new NotImplementedException();
      }

      public IRetryDelay GetDelay(string policy)
      {
         if (policy != "default") throw new InvalidOperationException("RetryPolicy not found");

         return Delay;
      }

      public IRetryPredicate GetPredicate(string policy)
      {
         if (policy != "default") throw new InvalidOperationException("RetryPolicy not found");

         return Predicate;
      }
   }
}