using System;
using Burble.Abstractions.Configuration;
using Burble.Abstractions.Throttling;
using Burble.Throttling;

namespace Burble.Tests.throttling_scenarios
{
   public class ThrottlingConfiguration : IHttpApiWithThrottling
   {
      public ThrottlingConfiguration(int concurrentRequests)
      {
         ThrottleSync = new SemaphoreThrottleSync(concurrentRequests);
      }
      
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
      
      public IThrottleSync ThrottleSync { get; }
   }
}