using Grouchy.HttpApi.Client.Abstractions;

namespace Grouchy.HttpApi.Client.EventCallbacks
{
   public class NoOpHttpClientEventCallback : IHttpClientEventCallback
   {
      public void Invoke(IHttpClientEvent @event)
      {
      }
   }
}
