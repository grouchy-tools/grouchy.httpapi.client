using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
{
    public class ThrottlingHttpClientDecorator : IHttpClientDecorator
    {
        private readonly IThrottleManager _throttleManager;

        public ThrottlingHttpClientDecorator(IThrottleManager throttleManager)
        {
            _throttleManager = throttleManager;
        }
        
        public IHttpClient Decorate(
            IHttpClient httpClient,
            IHttpApiConfiguration httpApiConfiguration)
        {
            var httpApiWithThrottling = httpApiConfiguration as IHttpApiWithThrottling;

            if (httpApiWithThrottling is null) return httpClient;

            return httpClient.AddThrottling(httpApiWithThrottling, _throttleManager);
        }
    }
}