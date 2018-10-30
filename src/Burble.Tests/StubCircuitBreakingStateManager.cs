using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;

namespace Burble.Tests
{
   public class StubCircuitBreakingStateManager : ICircuitBreakingStateManager<HttpStatusCode>
   {
      public ICircuitBreakingState<HttpStatusCode> State { get; set; }
      
      public ICircuitBreakingState<HttpStatusCode> Get(ICircuitBreakingConfiguration<HttpStatusCode> configuration)
      {
         return State;
      }

      public Task StopMonitoringAsync(CancellationToken cancellationToken)
      {
         throw new NotSupportedException();
      }
   }
}