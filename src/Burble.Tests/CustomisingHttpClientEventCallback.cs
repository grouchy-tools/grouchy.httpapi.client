namespace Burble.Tests
{
   using System.Collections.Generic;
   using Burble.Events;

   public class CustomisingHttpClientEventCallback : StubHttpClientEventCallback
   {
      public override void Invoke(IHttpClientEvent @event)
      {
         base.Invoke(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", @event.EventType } };
      }
   }
}