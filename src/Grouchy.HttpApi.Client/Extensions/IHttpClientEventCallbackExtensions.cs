using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Client.Abstractions;

namespace Grouchy.HttpApi.Client.Extensions
{
    public static class HttpClientEventCallbackExtensions
    {
        public static void Invoke(this IEnumerable<IHttpClientEventCallback> callbacks, IHttpClientEvent @event)
        {
            foreach (var callback in callbacks)
            {
                try
                {
                    callback.Invoke(@event);
                }
                catch (Exception e)
                {
                    // TODO: remove
                    var m = e.Message;
                    // Just in case callback handler doesn't catch exceptions
                }
            }
        }
    }
}