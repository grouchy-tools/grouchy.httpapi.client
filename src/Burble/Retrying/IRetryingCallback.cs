namespace Burble.Retrying
{
   using Burble.Events;

   public interface IRetryingCallback
   {
      void OnRetry(HttpClientRetryAttempt @event);
   }
}