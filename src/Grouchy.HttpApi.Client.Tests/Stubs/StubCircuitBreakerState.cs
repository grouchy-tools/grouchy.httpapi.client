using System;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubCircuitBreakerState : ICircuitBreakerState
   {
      public int LogSuccessResponseCalledCount { get; private set; }

      public int LogFailureResponseCalledCount { get; private set; }

      public int LogTimeoutCalledCount { get; private set; }

      public int LogExceptionCalledCount { get; private set; }

      public int LogRejectedCalledCount { get; private set; }

      public string LastSuccessResponse { get; private set; }
      
      public string LastFailureResponse { get; private set; }

      public bool ShouldAcceptResponse { get; set; }

      public string Policy { get; set; }
      
      public double ClosedPct { get; set; }

      public bool ShouldAccept()
      {
         return ShouldAcceptResponse;
      }

      public void LogSuccessResponse(string key)
      {
         LogSuccessResponseCalledCount++;
         LastSuccessResponse = key;
      }

      public void LogFailureResponse(string key)
      {
         LogFailureResponseCalledCount++;
         LastFailureResponse = key;
      }

      public void LogTimeout()
      {
         LogTimeoutCalledCount++;
      }

      public void LogException(Exception exception)
      {
         LogExceptionCalledCount++;
      }

      public void LogWithheld()
      {
         LogRejectedCalledCount++;
      }
   }
}