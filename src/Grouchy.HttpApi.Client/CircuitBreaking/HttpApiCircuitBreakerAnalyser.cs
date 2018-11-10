using System.Collections.Generic;
using System.Linq;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Client.CircuitBreaking
{
   // TODO: Add tests
   public class HttpApiCircuitBreakerAnalyser : ICircuitBreakerAnalyser
   {
      private bool ShouldTrip(Metrics metrics)
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

      public bool ShouldReset(Metrics metrics)
      {
         return Sum(metrics.SuccessResponses) > 0;
      }

      private static int Sum(IDictionary<string, int> occurrences)
      {
         return occurrences.Sum(c => c.Value);
      }

      public Rating Analyse(IEnumerable<Metrics> recentMetrics)
      {
         // TODO: Vary weighting based on age of metrics

         var metrics = recentMetrics.First();

         if (ShouldTrip(metrics)) return Rating.Trip;

         if (ShouldReset(metrics)) return Rating.Reset;

         return Rating.Hold;
      }
   }
}