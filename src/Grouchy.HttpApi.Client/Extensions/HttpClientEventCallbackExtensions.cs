using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.Events;

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
                catch (Exception)
                {
                    // Just in case callback handler doesn't catch exceptions
                }
            }
        }
    }
}