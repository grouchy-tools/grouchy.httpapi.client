using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Burble.CircuitBreaking
{
   public class CircuitBreakingState<TResponse> : ICircuitBreakingState<TResponse>
   {
      private const int DefaultMonitoringPeriodMs = 10000;
      
      private readonly ICircuitBreakingConfiguration<TResponse> _configuration;
      private readonly LinkedList<Metrics<TResponse>> _historicMetrics = new LinkedList<Metrics<TResponse>>();
      private readonly object _metricsSync = new object();
      private readonly Random _random = new Random();

      private Metrics<TResponse> _currentMetrics = new Metrics<TResponse>();
      private int _tripLevel;

      public CircuitBreakingState(ICircuitBreakingConfiguration<TResponse> configuration)
      {
         _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      }

      public double ClosedPct
      {
         get
         {
            if (_tripLevel == 0) return 100;

            if (_tripLevel > OpeningRates.Length) return 0;

            return 100f / OpeningRates[_tripLevel - 1];
         }
      }

      public bool ShouldAccept()
      {
         // 100%
         if (_tripLevel == 0) return true;
         
         // 0%
         if (_tripLevel > OpeningRates.Length) return false;
         
         return _random.Next(OpeningRates[_tripLevel - 1]) == 0;
      }

      public void LogSuccessResponse(TResponse response)
      {
         lock (_metricsSync)
         {
            _currentMetrics.SuccessResponses.TryGetValue(response, out var value); 
            _currentMetrics.SuccessResponses[response] = value + 1;
         }
      }

      public void LogFailureResponse(TResponse response)
      {
         lock (_metricsSync)
         {
            _currentMetrics.FailureResponses.TryGetValue(response, out var value); 
            _currentMetrics.FailureResponses[response] = value + 1;
         }
      }

      public void LogTimeout()
      {
         lock (_metricsSync)
         {
            _currentMetrics.Timeouts++;
         }
      }

      public void LogException(Exception exception)
      {
         lock (_metricsSync)
         {
            _currentMetrics.Exceptions++;
         }
      }

      public void LogWithheld()
      {
         lock (_metricsSync)
         {
            _currentMetrics.Rejections++;
         }
      }

      public async Task MonitorAsync(CancellationToken cancellationToken)
      {
         // TODO: Ensure only single thread in here
         
         while (!cancellationToken.IsCancellationRequested)
         {
            // TODO: Get last n metrics, based on config
            
            Metrics<TResponse> lastMetrics;
            lock (_metricsSync)
            {
               _historicMetrics.AddFirst(_currentMetrics);
               lastMetrics = _currentMetrics;
               _currentMetrics = new Metrics<TResponse>();
            }
            
            var analysis = _configuration.Analyser.Analyse(new[] { lastMetrics });
            
            switch (analysis)
            {
               case CircuitBreakerAnalysis.Trip when CanTrip():
                  _tripLevel++;
                  break;
               case CircuitBreakerAnalysis.Reset when CanReset():
                  _tripLevel--;
                  break;
            }

            try
            {
               await Task.Delay(_configuration.MonitoringPeriodMs ?? DefaultMonitoringPeriodMs, cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }

            bool CanTrip() => _tripLevel < OpeningRates.Length + 1;
            bool CanReset() => _tripLevel > 0;
         }
      }
      
      private int[] OpeningRates => _configuration.ProgressiveOpeningRates ?? new int[] { };
   }
}
