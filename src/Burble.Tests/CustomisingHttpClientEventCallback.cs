namespace Burble.Tests
{
   using System.Collections.Generic;
   using Burble.Abstractions;

   public class CustomisingHttpClientEventCallback : StubHttpClientEventCallback
   {
      public override void Invoke(IHttpClientEvent @event)
      {
         base.Invoke(@event);
         @event.Tags = new Dictionary<string, object> { { "Key", @event.EventType } };
      }
   }
}