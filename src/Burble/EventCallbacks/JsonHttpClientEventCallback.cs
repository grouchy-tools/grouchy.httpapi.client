namespace Burble.EventCallbacks
{
   using System;
   using System.Linq;
   using System.Reflection;
   using Burble.Abstractions;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Serialization;

   public class JsonHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly Action<string> _jsonCallback;
      private readonly JsonSerializerSettings _jsonSettings;

      public JsonHttpClientEventCallback(Action<string> jsonCallback)
      {
         _jsonCallback = jsonCallback;
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new ClientEventResolver(),
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpClientEvent @event)
      {
         _jsonCallback(JsonConvert.SerializeObject(@event, _jsonSettings));
      }

      private class ClientEventResolver : CamelCasePropertyNamesContractResolver
      {
         protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
         {
            var property = base.CreateProperty(member, memberSerialization);

            var implementsHttpClientEvent = property.DeclaringType.GetInterfaces().Contains(typeof(IHttpClientEvent));

            if (implementsHttpClientEvent && property.PropertyName == "tags")
            {
               property.ShouldSerialize = value =>
               {
                  var @event = (IHttpClientEvent)value;
                  return @event.Tags != null && @event.Tags.Count != 0;
               };
            }
            else if (implementsHttpClientEvent && property.PropertyName == "request")
            {
               property.ShouldSerialize = value => false;
            }

            return property;
         }
      }
   }
}