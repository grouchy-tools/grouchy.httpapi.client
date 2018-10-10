using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.retrying_scenarios
{
   public class request_has_properties
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      
      private StubHttpClient _baseHttpClient;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _baseHttpClient = new StubHttpClient();

         var httpClient = _baseHttpClient.AddRetrying(
            new StubRetryPredicate(2),
            new StubRetryDelay(10),
            new []{_callback});

         var message = new HttpRequestMessage(HttpMethod.Get, "/get-content");
         message.Properties.Add("propertyA", "valueA");
         message.Properties.Add("propertyB", 12358);

         try
         {
            httpClient.SendAsync(message).Wait();
         }
         catch
         {
            // Ignore the exception
         }
      }

      [Test]
      public void properties_should_exist_in_first_request()
      {
         _baseHttpClient.Requests[0].Properties.ShouldContainKeyAndValue("propertyA", "valueA");
         _baseHttpClient.Requests[0].Properties.ShouldContainKeyAndValue("propertyB", 12358);
      }

      [Test]
      public void properties_should_exist_in_second_request()
      {
         _baseHttpClient.Requests[1].Properties.ShouldContainKeyAndValue("propertyA", "valueA");
         _baseHttpClient.Requests[1].Properties.ShouldContainKeyAndValue("propertyB", 12358);
      }

      private class StubHttpClient : IHttpClient
      {
         public IList<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

         public Uri BaseAddress => new Uri("http://stub-host");

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            return SendAsync(request, CancellationToken.None);
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            Requests.Add(request);
            throw new Exception();
         }
      }
   }
}
