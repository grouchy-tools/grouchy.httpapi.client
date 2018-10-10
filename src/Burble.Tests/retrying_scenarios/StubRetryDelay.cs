using Burble.Retrying;

namespace Burble.Tests.retrying_scenarios
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