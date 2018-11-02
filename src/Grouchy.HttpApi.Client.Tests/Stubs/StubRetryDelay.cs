using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubRetryDelay : IRetryDelay
   {
      private readonly int _delayMs;

      public StubRetryDelay(int delayMs)
      {
         _delayMs = delayMs;
      }

      public int DelayMs(int retryAttempt)
      {
         return _delayMs;
      }
   }
}