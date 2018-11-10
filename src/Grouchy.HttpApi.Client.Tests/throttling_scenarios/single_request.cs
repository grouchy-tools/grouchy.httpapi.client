using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.throttling_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class single_request
   {
      private const int ConcurrentRequests = 3;

      private HttpResponseMessage _expectedResponse;
      private HttpResponseMessage _actualResponse;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var throttleManager = new StubThrottleManager { Sync = new StubThrottleSync(ConcurrentRequests) }; 
         _expectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
         var baseHttpClient = new StubHttpClient(_expectedResponse);
         var configuration = new ThrottlingConfiguration { ThrottlePolicy = "default" };
         var httpClient = baseHttpClient.AddThrottling(configuration, throttleManager);

         _actualResponse = await httpClient.GetAsync("/ping");
      }
      
      [Test]
      public void should_return_response_from_inner_httpclient()
      {
         _actualResponse.ShouldBeSameAs(_expectedResponse);
      }
   }
}
