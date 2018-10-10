using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Burble.Abstractions;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using Burble.Throttling;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.throttling_scenarios
{
   public class single_request
   {
      private const int ConcurrentRequests = 3;

      private HttpResponseMessage _expectedResponse;
      private HttpResponseMessage _actualResponse;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _expectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
         var baseHttpClient = new StubHttpClient(_expectedResponse);
         var httpClient = baseHttpClient.AddThrottling(new SemaphoneThrottleSync(ConcurrentRequests));

         _actualResponse = httpClient.GetAsync("/ping").Result;
      }
      
      [Test]
      public void should_return_response_from_inner_httpclient()
      {
         _actualResponse.ShouldBeSameAs(_expectedResponse);
      }

      private class StubHttpClient : IHttpClient
      {
         private HttpResponseMessage _response;

         public StubHttpClient(HttpResponseMessage response)
         {
            _response = response;
         }

         public Uri BaseAddress
         {
            get { throw new NotImplementedException(); }
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            return Task.FromResult(_response);
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            return Task.FromResult(_response);
         }
      }
   }
}
