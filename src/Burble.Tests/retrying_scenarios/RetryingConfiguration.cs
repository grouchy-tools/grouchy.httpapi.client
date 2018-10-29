using System;
using Burble.Abstractions.Configuration;
using Burble.Abstractions.Retrying;

namespace Burble.Tests.retrying_scenarios
{
   public class RetryingConfiguration : IHttpApiWithRetrying
   {
      public RetryingConfiguration(int retries, int delayMs)
      {
         RetryPredicate = new StubRetryPredicate(retries);
         RetryDelay = new StubRetryDelay(delayMs);
      }
      
      public string Name { get; set; }

      public Uri Uri { get; set; } = new Uri("http://exception-host");
      
      public int? TimeoutMs { get; set; }
      
      public IRetryPredicate RetryPredicate { get; }
      
      public IRetryDelay RetryDelay { get; }
   }
}