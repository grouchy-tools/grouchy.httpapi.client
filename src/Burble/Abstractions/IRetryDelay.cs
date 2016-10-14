namespace Burble.Abstractions
{
   public interface IRetryDelay
   {
      int DelayMs(int retryAttempt);
   }
}