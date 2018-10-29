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
   // ReSharper disable once InconsistentNaming
   public class request_has_properties
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      
      private StubHttpClient _baseHttpClient;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _baseHttpClient = new StubHttpClient();

         var configuration = new RetryingConfiguration(retries: 2, delayMs: 10);
         var httpClient = _baseHttpClient.AddRetrying(
            configuration,
            new []{_callback});

         var message = new HttpRequestMessage(HttpMethod.Get, "/get-content");
         message.Properties.Add("propertyA", "valueA");
         message.Properties.Add("propertyB", 12358);

         try
         {
            await httpClient.SendAsync(message);
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

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            Requests.Add(request);
            throw new Exception();
         }
      }
   }
}
