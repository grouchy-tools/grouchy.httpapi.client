using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Abstractions.Tagging;

namespace Grouchy.HttpApi.Client.HttpClients
{
   /// <summary>
   /// Add session-id, correlation-id and request-id to the request header
   /// </summary>
   public class TaggingHttpClient : IHttpClient
   {
      private const string UserAgentHeader = "User-Agent";

      private readonly IHttpClient _httpClient;
      private readonly ISessionIdAccessor _sessionIdAccessor;
      private readonly ICorrelationIdAccessor _correlationIdAccessor;
      private readonly IOutboundRequestIdAccessor _outboundRequestIdAccessor;
      private readonly IGenerateGuids _guidGenerator;
      private readonly string _userAgent;

      public TaggingHttpClient(
         IHttpClient httpClient,
         ISessionIdAccessor sessionIdGetter,
         ICorrelationIdAccessor correlationIdGetter,
         IOutboundRequestIdAccessor outboundRequestIdAccessor,
         IGenerateGuids guidGenerator,
         IApplicationInfo applicationInfo)
      {
         _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
         _sessionIdAccessor = sessionIdGetter ?? throw new ArgumentNullException(nameof(sessionIdGetter));
         _correlationIdAccessor = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _outboundRequestIdAccessor = outboundRequestIdAccessor ?? throw new ArgumentNullException(nameof(outboundRequestIdAccessor));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));

         if (applicationInfo == null) throw new ArgumentNullException(nameof(applicationInfo));

         _userAgent = BuildUserAgent(applicationInfo);
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         // TODO: Does this even work with multiple outbound requests?
         var outboundRequestId = _guidGenerator.Generate().ToString();
         _outboundRequestIdAccessor.OutboundRequestId = outboundRequestId;

         request.Headers.Add(UserAgentHeader, _userAgent);
         request.Headers.Add("session-id", _sessionIdAccessor.SessionId);
         request.Headers.Add("correlation-id", _correlationIdAccessor.CorrelationId);
         request.Headers.Add("request-id", outboundRequestId);
         
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