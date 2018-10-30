using Burble.Abstractions;

namespace Burble.EventCallbacks
{
   public class NoOpHttpClientEventCallback : IHttpClientEventCallback
   {
      public void Invoke(IHttpClientEvent @event)
      {
      }
   }
}
