﻿namespace Burble.Tests.throttling_scenarios
{
   using System.Net;
   using System.Threading.Tasks;
   using System.Net.Http;
   using Burble.Abstractions;
   using NUnit.Framework;
   using Shouldly;

   public class single_request
   {
      private const int ConcurrentRequests = 3;

      private readonly HttpResponseMessage _expectedResponse;
      private readonly HttpResponseMessage _actualResponse;

      public single_request()
      {
         _expectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
         var baseHttpClient = new StubHttpClient(_expectedResponse);
         var httpClient = baseHttpClient.AddThrottling(ConcurrentRequests);

         _actualResponse = httpClient.GetAsync("/ping").Result;
      }
      
      [Test]
      public void should_return_response_from_inner_httpclient()
      {
         _actualResponse.ShouldBeSameAs(_expectedResponse);
      }

      private class StubHttpClient : IHttpClient
      {
         private readonly HttpResponseMessage _response;

         public StubHttpClient(HttpResponseMessage response)
         {
            _response = response;
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            return Task.FromResult(_response);
         }
      }
   }
}