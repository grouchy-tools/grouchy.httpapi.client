using System;
using Grouchy.HttpApi.Client.Abstractions.Configuration;

namespace Grouchy.HttpApi.Client.Tests.Configuration
{
   public class CircuitBreakingConfiguration : IHttpApiWithCircuitBreaking
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
      
      public string CircuitBreakerPolicy { get; set; }
   }
}