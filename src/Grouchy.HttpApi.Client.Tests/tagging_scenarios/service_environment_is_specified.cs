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
   public class service_environment_is_specified
   {
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var sessionIdAccessor = new StubSessionIdAccessor();
         var correlationIdAccessor = new StubCorrelationIdAccessor();
         var outboundRequestIdAccessor = new StubOutboundRequestIdAccessor();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());
         var applicationInfo = new StubApplicationInfo { Name = "my-service", Environment = "Production", OperatingSystem = "my-os"};

         using (var webApi = new GetIdsFromHeadersHttpApi())
         using (var httpClient = webApi.CreateClientWithTagging(sessionIdAccessor, correlationIdAccessor, outboundRequestIdAccessor, guidGenerator, applicationInfo))
         {
            var response = await httpClient.GetAsync("/get-ids-from-headers");
            var content = await response.Content.ReadAsStringAsync();
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe("my-service Production (my-os)");
      }
   }
}
