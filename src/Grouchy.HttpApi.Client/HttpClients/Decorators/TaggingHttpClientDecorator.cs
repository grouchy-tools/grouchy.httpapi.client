using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Extensions;

namespace Grouchy.HttpApi.Client.HttpClients.Decorators
{
    public class TaggingHttpClientDecorator : IHttpClientDecorator
    {
        private readonly ISessionIdAccessor _sessionIdAccessor;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private readonly IOutboundRequestIdAccessor _outboundRequestIdAccessor;
        private readonly IGenerateGuids _guidGenerator;
        private readonly IApplicationInfo _applicationInfo;

        public TaggingHttpClientDecorator(
            ISessionIdAccessor sessionIdAccessor,
            ICorrelationIdAccessor correlationIdAccessor,
            IOutboundRequestIdAccessor outboundRequestIdAccessor,
            IGenerateGuids guidGenerator,
            IApplicationInfo applicationInfo)
        {
            _sessionIdAccessor = sessionIdAccessor;
            _correlationIdAccessor = correlationIdAccessor;
            _outboundRequestIdAccessor = outboundRequestIdAccessor;
            _guidGenerator = guidGenerator;
            _applicationInfo = applicationInfo;
        }

        public IHttpClient Decorate(
            IHttpClient httpClient,
            IHttpApiConfiguration httpApiConfiguration)
        {
            var httpApiWithTagging = httpApiConfiguration as IHttpApiWithTagging;

            if (httpApiWithTagging is null) return httpClient;

            return httpClient.AddTagging(_sessionIdAccessor, _correlationIdAccessor, _outboundRequestIdAccessor, _guidGenerator, _applicationInfo);
        }
    }
}