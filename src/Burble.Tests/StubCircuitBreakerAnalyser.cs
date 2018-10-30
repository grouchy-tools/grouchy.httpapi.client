using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Burble.Abstractions.CircuitBreaking;
using Burble.CircuitBreaking;

namespace Burble.Tests
{
   public class StubCircuitBreakerAnalyser : ICircuitBreakerAnalyser<HttpStatusCode>
   {
      public ConcurrentQueue<CircuitBreakerAnalysis> AnalysisResponses { get; }
      
      public StubCircuitBreakerAnalyser(params CircuitBreakerAnalysis[] analysisResponses)
      {
         AnalysisResponses = new ConcurrentQueue<CircuitBreakerAnalysis>(analysisResponses);
      }
      
      public CircuitBreakerAnalysis Analyse(IEnumerable<Metrics<HttpStatusCode>> lastMetrics)
      {
         if (AnalysisResponses.TryDequeue(out var response))
         {
            return response;
         }
         
         throw new InvalidOperationException("No more responses queued");
      }
   }
}