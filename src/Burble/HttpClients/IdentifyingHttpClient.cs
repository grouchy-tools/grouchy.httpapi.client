using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.Identifying;

namespace Burble.HttpClients
{
   /// <summary>
   /// Add correlation-id and request-id to the request header, creating a new id if necessary
   /// </summary>
   public class IdentifyingHttpClient : IHttpClient
   {
      private const string UserAgentHeader = "User-Agent";

      private readonly IHttpClient _httpClient;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGenerateGuids _guidGenerator;
      private readonly string _userAgent;

      public IdentifyingHttpClient(
         IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator,
         IApplicationInfo applicationInfo)
      {
         _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
         _correlationIdGetter = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
         
         if (applicationInfo == null) throw new ArgumentNullException(nameof(applicationInfo));

         _userAgent = BuildUserAgent(applicationInfo);
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         request.Headers.Add(UserAgentHeader, _userAgent);
         request.Headers.Add("correlation-id", _correlationIdGetter.Get());
         request.Headers.Add("request-id", _guidGenerator.Generate().ToString());

         return _httpClient.SendAsync(request, cancellationToken);
      }

      private static string BuildUserAgent(IApplicationInfo applicationInfo)
      {
         var userAgent = applicationInfo.Name;
         var version = applicationInfo.Version;

         if (!string.IsNullOrWhiteSpace(version))
         {
            userAgent += $"/{version}";
         }

         if (!string.IsNullOrWhiteSpace(applicationInfo.Environment))
         {
            userAgent += $" {applicationInfo.Environment}";
         }

         userAgent += $" ({applicationInfo.OperatingSystem})";

         return userAgent;
      }
   }
}