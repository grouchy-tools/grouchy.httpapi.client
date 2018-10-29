using System;
using System.Net;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;

namespace Burble.Tests.circuit_breaking_scenarios
{
   public class CircuitBreakingConfiguration : IHttpApiWithCircuitBreaking
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
      
      public int? MonitoringPeriodMs { get; set; }
      
      public ICircuitBreakerAnalyser<HttpStatusCode> Analyser { get; set; }
      
      public int[] ProgressiveOpeningRates { get; set; }
   }
}