using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Burble.Extensions;

namespace Burble.HttpClients.Decorators
{
    public class ThrottlingHttpClientDecorator : IHttpClientDecorator
    {
        public IHttpClient Decorate(
            IHttpClient httpClient,
            IHttpApiConfiguration httpApiConfiguration)
        {
            var httpApiWithThrottling = httpApiConfiguration as IHttpApiWithThrottling;

            if (httpApiWithThrottling is null) return httpClient;

            return httpClient.AddThrottling(httpApiWithThrottling);
        }
    }
}