namespace Burble.Events
{
   public interface IHttpClientEventCallback
   {
      void Invoke(IHttpClientEvent @event);
   }
}
