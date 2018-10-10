using System;
using System.Collections.Generic;
using Burble.Abstractions;

namespace Burble.Extensions
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