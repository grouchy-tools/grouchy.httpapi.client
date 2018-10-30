using System.Collections.Concurrent;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Burble.HttpClients;

namespace Burble
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