namespace Burble.Tests
{
   using Burble.Abstractions;

   public class CustomisingHttpClientEventCallback : StubHttpClientEventCallback
   {
      public override void Invoke(IHttpClientEvent @event)
      {
         base.Invoke(@event);
         @event.Tags.Add("Key", @event.EventType);
      }
   }
}