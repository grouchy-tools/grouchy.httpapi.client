using System.Collections.Generic;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
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

        public IHttpClient Decorate(
            IHttpClient httpClient,
            IHttpApiConfiguration httpApiConfiguration)
        {
            var httpApiWithInstrumenting = httpApiConfiguration as IHttpApiWithInstrumenting;

            if (httpApiWithInstrumenting is null) return httpClient;

            return httpClient.AddInstrumenting(httpApiWithInstrumenting, _callbacks);
        }
    }
}