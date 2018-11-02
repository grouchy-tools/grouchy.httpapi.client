using System;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubCircuitBreakerManager : ICircuitBreakerManager
   {
      public ICircuitBreakerState State { get; set; }
      
      public void Register(
         string policy,
         ICircuitBreakerAnalyser circuitBreakerAnalyser,
         ICircuitBreakerOpeningRates circuitBreakerOpeningRates,
         ICircuitBreakerPeriod circuitBreakerPeriod)
      {
         throw new NotImplementedException();
      }

      public ICircuitBreakerState GetState(string policy)
      {
         return State;
      }

      public Task StopMonitoringAsync(CancellationToken cancellationToken)
      {
         throw new NotSupportedException();
      }
   }
}