using System;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using Grouchy.HttpApi.Client.Tests.Stubs;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.tagging_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class happy_path
   {
      private string _currentRequestId;
      private string _newRequestId;
      private string _correlationId;
      private string _sessionId;
      private string _service;
      private string _version;
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _currentRequestId = Guid.NewGuid().ToString();
         _newRequestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();
         _sessionId = Guid.NewGuid().ToString();
         _service = "my-service";
         _version = "1.0.1-client";

         var sessionIdAccessor = new StubSessionIdAccessor { Response = _sessionId };
         var correlationIdAccessor = new StubCorrelationIdAccessor { Response = _correlationId };
         var outboundRequestIdAccessor = new StubOutboundRequestIdAccessor { OutboundRequestId  = _currentRequestId };
         var guidGenerator = new StubGuidGenerator(Guid.Parse(_newRequestId));
         var applicationInfo = new StubApplicationInfo { Name = _service, Version = _version, OperatingSystem = "my-os"};

         using (var webApi = new GetIdsFromHeadersHttpApi())
         using (var httpClient = webApi.CreateClientWithTagging(sessionIdAccessor, correlationIdAccessor, outboundRequestIdAccessor, guidGenerator, applicationInfo))
         {
            var response = await httpClient.GetAsync("/get-ids-from-headers");
            var content = await response.Content.ReadAsStringAsync();
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      

      [Test]
      public void new_request_id_is_added_to_the_headers()
      {
         _idsFromHeaders["requestId"].Value<string>().ShouldBe(_newRequestId);
      }

      [Test]
      public void correlation_id_is_added_to_the_headers()
      {
         _idsFromHeaders["correlationId"].Value<string>().ShouldBe(_correlationId);
      }

      [Test]
      public void session_id_is_added_to_the_headers()
      {
         _idsFromHeaders["sessionId"].Value<string>().ShouldBe(_sessionId);
      }

      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe("my-service/1.0.1-client (my-os)");
      }
   }
}
