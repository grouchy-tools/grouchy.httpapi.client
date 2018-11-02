using Grouchy.Abstractions;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Extensions;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
{
    public class IdentifyingHttpClientDecorator : IHttpClientDecorator
    {
        private readonly IGetCorrelationId _correlationIdGetter;
        private readonly IGenerateGuids _guidGenerator;
        private readonly IApplicationInfo _applicationInfo;

        public IdentifyingHttpClientDecorator(
            IGetCorrelationId correlationIdGetter,
            IGenerateGuids guidGenerator,
            IApplicationInfo applicationInfo)
        {
            _correlationIdGetter = correlationIdGetter;
            _guidGenerator = guidGenerator;
            _applicationInfo = applicationInfo;
        }

        public IHttpClient Decorate(
            IHttpClient httpClient,
            IHttpApiConfiguration httpApiConfiguration)
        {
            var httpApiWithIdentification = httpApiConfiguration as IHttpApiWithIdentification;

            if (httpApiWithIdentification is null) return httpClient;

            return httpClient.AddIdentifyingHeaders(_correlationIdGetter, _guidGenerator, _applicationInfo);
        }
    }
}