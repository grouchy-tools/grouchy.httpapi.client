namespace Burble.Retrying
{
   using Burble.Abstractions;

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