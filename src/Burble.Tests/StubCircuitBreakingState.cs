using System;
using System.Net;
using System.Net.Http;
using Burble.Abstractions.CircuitBreaking;

namespace Burble.Tests
{
   public class StubCircuitBreakingState : ICircuitBreakingState<HttpStatusCode>
   {
      public int LogSuccessResponseCalledCount { get; private set; }

      public int LogFailureResponseCalledCount { get; private set; }

      public int LogTimeoutCalledCount { get; private set; }

      public int LogExceptionCalledCount { get; private set; }

      public int LogRejectedCalledCount { get; private set; }

      public HttpStatusCode LastSuccessResponse { get; private set; }
      
      public HttpStatusCode LastFailureResponse { get; private set; }

      public bool ShouldAcceptResponse { get; set; }

      public double ClosedPct { get; set; }

      public bool ShouldAccept()
      {
         return ShouldAcceptResponse;
      }

      public void LogSuccessResponse(HttpStatusCode response)
      {
         LogSuccessResponseCalledCount++;
         LastSuccessResponse = response;
      }

      public void LogFailureResponse(HttpStatusCode response)
      {
         LogFailureResponseCalledCount++;
         LastFailureResponse = response;
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