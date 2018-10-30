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
      private readonly ConcurrentDictionary<ICircuitBreakingConfiguration<TResponse>, Entry> _cache = new ConcurrentDictionary<ICircuitBreakingConfiguration<TResponse>, Entry>();

      private bool _isDisposed;
      
      public ICircuitBreakingState<TResponse> Get(ICircuitBreakingConfiguration<TResponse> configuration)
      {
         var foo = _cache.GetOrAdd(configuration, Create);

         return foo.CircuitBreakingState;
      }

      private Entry Create(ICircuitBreakingConfiguration<TResponse> configuration)
      {
         var circuitBreakingState = new CircuitBreakingState<TResponse>(configuration);
         var task = Task.Run(() => circuitBreakingState.MonitorAsync(_stoppingCts.Token));

         return new Entry(circuitBreakingState, task);
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
            var waitForAllTasks = Task.WhenAll(_cache.Select(c => c.Value.Task));
            
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

      // Implicit tuple does not seem to be available for net451
      private class Entry
      {
         public Entry(ICircuitBreakingState<TResponse> circuitBreakingState, Task task)
         {
            CircuitBreakingState = circuitBreakingState;
            Task = task;
         }

         public readonly ICircuitBreakingState<TResponse> CircuitBreakingState;

         public readonly Task Task;
      }
   }
}