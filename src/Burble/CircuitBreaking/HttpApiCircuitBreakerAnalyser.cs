using System.Collections.Generic;
using System.Linq;
using System.Net;
using Burble.Abstractions.CircuitBreaking;

namespace Burble.CircuitBreaking
{
   // TODO: Add tests
   public class HttpApiCircuitBreakerAnalyser : ICircuitBreakerAnalyser<HttpStatusCode>
   {
      private bool ShouldTrip(Metrics<HttpStatusCode> metrics)
      {
         // TODO: Vary where client errors are considered or not

         // TODO: Vary the weighting based on exceptions, timeouts and server/client errors
         // use absolute values + percentages
         
         
         var errors = Sum(metrics.FailureResponses) + metrics.Timeouts + metrics.Exceptions;
         var total = errors + Sum(metrics.SuccessResponses);

         var errorRate = errors / (double) total;

         // TODO: What should the threshold be?
         return errorRate > 0.5;
      }

      public bool ShouldReset(Metrics<HttpStatusCode> metrics)
      {
         return Sum(metrics.SuccessResponses) > 0;
      }

      private static int Sum(IDictionary<HttpStatusCode, int> occurrences)
      {
         return occurrences.Sum(c => c.Value);
      }

      public CircuitBreakerAnalysis Analyse(IEnumerable<Metrics<HttpStatusCode>> recentMetrics)
      {
         // TODO: Vary weighting based on age of metrics

         var metrics = recentMetrics.First();

         if (ShouldTrip(metrics)) return CircuitBreakerAnalysis.Trip;

         if (ShouldReset(metrics)) return CircuitBreakerAnalysis.Reset;

         return CircuitBreakerAnalysis.Hold;
      }
   }
}