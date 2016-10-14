namespace Burble.Tests.retrying_scenarios
{
   using Burble.Abstractions;

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