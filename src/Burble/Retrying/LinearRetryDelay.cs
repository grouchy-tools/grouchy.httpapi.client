using Burble.Abstractions.Retrying;

namespace Burble.Retrying
{
   public class LinearRetryDelay : IRetryDelay
   {
      private readonly int _delayMs;

      public LinearRetryDelay(int delayMs)
      {
         _delayMs = delayMs;
      }

      public int DelayMs(int retryAttempt)
      {
         return _delayMs * retryAttempt;
      }
   }
}