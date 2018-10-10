namespace Burble.Retrying
{
   public interface IRetryDelay
   {
      int DelayMs(int retryAttempt);
   }
}