using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;

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
            var httpClient = new HttpClient
            {
                BaseAddress = configuration.Uri,
                Timeout = TimeSpan.FromMilliseconds(configuration.TimeoutMs)
            };
            
            return new HttpClientAdapter(httpClient);
        }
    }
}