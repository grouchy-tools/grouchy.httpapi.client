using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using Burble.Throttling;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.throttling_scenarios
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
         _expectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
         var baseHttpClient = new StubHttpClient(_expectedResponse);
         var configuration = new ThrottlingConfiguration(ConcurrentRequests);
         var httpClient = baseHttpClient.AddThrottling(configuration);

         _actualResponse = await httpClient.GetAsync("/ping");
      }
      
      [Test]
      public void should_return_response_from_inner_httpclient()
      {
         _actualResponse.ShouldBeSameAs(_expectedResponse);
      }
   }
}
