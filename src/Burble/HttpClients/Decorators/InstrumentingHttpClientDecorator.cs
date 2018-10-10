using System.Collections.Generic;
using Burble.Abstractions;
using Burble.Extensions;

namespace Burble.HttpClients.Decorators
{
    public class InstrumentingHttpClientDecorator : IHttpClientDecorator
    {
        private readonly IEnumerable<IHttpClientEventCallback> _callbacks;

        public InstrumentingHttpClientDecorator(IEnumerable<IHttpClientEventCallback> callbacks)
        {
            _callbacks = callbacks;
        }

        public IHttpClient Decorate(IHttpClient httpClient)
        {
            return httpClient.AddInstrumenting(_callbacks);
        }
    }
}