using Grouchy.HttpApi.Client.Tests.Stubs;

namespace Grouchy.HttpApi.Client.Tests
{
   using Grouchy.HttpApi.Client.Abstractions;

   public class CustomisingHttpClientEventCallback : StubHttpClientEventCallback
   {
      public override void Invoke(IHttpClientEvent @event)
      {
         base.Invoke(@event);
         @event.Tags.Add("Key", @event.EventType);
      }
   }
}