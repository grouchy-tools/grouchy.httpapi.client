using System.Collections.Generic;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Extensions;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
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