using System.Collections.Concurrent;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.HttpClients;

namespace Grouchy.HttpApi.Client
{
    public class HttpClientRegistry : IHttpClientRegistry
    {
        private readonly ConcurrentDictionary<IHttpApiConfiguration, IHttpClient> _cache = new ConcurrentDictionary<IHttpApiConfiguration, IHttpClient>();

        public IHttpClient Get<TConfiguration>(TConfiguration configuration) where TConfiguration : IHttpApiConfiguration
        {
            return _cache.GetOrAdd(configuration, Create);
        }

        private static IHttpClient Create(IHttpApiConfiguration configuration)
        {
            return new DefaultHttpClient(configuration);
        }
    }
}