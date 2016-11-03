namespace Burble.Logging
{
   using Burble.Events;

   public interface ILoggingCallback
   {
      void OnInitiated(HttpClientRequestInitiated @event);

      void OnReceived(HttpClientResponseReceived @event);

      void OnTimeout(HttpClientTimedOut @event);

      void OnException(HttpClientExceptionThrown @event);
   }
}
