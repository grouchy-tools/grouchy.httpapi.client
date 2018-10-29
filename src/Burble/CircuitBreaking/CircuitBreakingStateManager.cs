using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;

namespace Burble.CircuitBreaking
{
   public class CircuitBreakingStateManager<TResponse> : ICircuitBreakingStateManager<TResponse>, IDisposable
   {
      private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
      private readonly ConcurrentDictionary<ICircuitBreakingConfiguration<TResponse>, (ICircuitBreakingState<TResponse>, Task)> _cache = new ConcurrentDictionary<ICircuitBreakingConfiguration<TResponse>, (ICircuitBreakingState<TResponse>, Task)>();

      private bool _isDisposed;
      
      public ICircuitBreakingState<TResponse> Get(ICircuitBreakingConfiguration<TResponse> configuration)
      {
         var (circuitBreakingState, _) = _cache.GetOrAdd(configuration, Create);

         return circuitBreakingState;
      }

      private (ICircuitBreakingState<TResponse>, Task) Create(ICircuitBreakingConfiguration<TResponse> configuration)
      {
         var circuitBreakingState = new CircuitBreakingState<TResponse>(configuration);
         var task = Task.Run(() => circuitBreakingState.MonitorAsync(_stoppingCts.Token));

         return (circuitBreakingState, task);
      }

      public async Task StopMonitoringAsync(CancellationToken cancellationToken)
      {
         try
         {
            // Signal cancellation to all tasks monitoring state 
            _stoppingCts.Cancel();
         }
         finally
         {
            var waitForAllTasks = Task.WhenAll(_cache.Select(c => c.Value.Item2));
            
            // Wait until all tasks complete or the stop token triggers
            await Task.WhenAny(waitForAllTasks, Task.Delay(Timeout.Infinite, cancellationToken));
         }
      }

      void IDisposable.Dispose()
      {
         if (_isDisposed) return;

         _stoppingCts?.Dispose();
         _isDisposed = true;
      }
   }
}