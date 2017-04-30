﻿namespace Burble.Tests.throttling_scenarios
{
   using System;
   using System.Linq;
   using System.Net;
   using System.Threading.Tasks;
   using System.Net.Http;
   using System.Threading;
   using Burble.Abstractions;
   using Burble.Throttling;
   using Xunit;
   using Shouldly;

   public class limit_concurrent_requests
   {
      private const int ExpectedMaxConcurrentRequests = 3;
      private const int ExpectedTotalRequests = 7;

      private readonly CountingHttpClient _baseHttpClient;

      public limit_concurrent_requests()
      {
         _baseHttpClient = new CountingHttpClient();
         var httpClient = _baseHttpClient.AddThrottling(new SemaphoneThrottleSync(ExpectedMaxConcurrentRequests));

         var tasks = Enumerable.Range(1, ExpectedTotalRequests).Select(c => httpClient.GetAsync("/ping")).ToArray();
         Task.WhenAll(tasks).Wait(2000);
      }

      [Fact]
      public void should_not_exceed_concurrent_requests()
      {
         _baseHttpClient.MaxConcurrentRequests.ShouldBeLessThanOrEqualTo(ExpectedMaxConcurrentRequests);
      }

      [Fact]
      public void should_handle_all_requests()
      {
         _baseHttpClient.TotalRequests.ShouldBe(ExpectedTotalRequests);
      }

      private class CountingHttpClient : IHttpClient
      {
         private readonly object _lock = new object();

         private int _currentRequests;

         public int MaxConcurrentRequests { get; private set; }

         public int TotalRequests { get; private set; }

         public Uri BaseAddress
         {
            get { throw new NotImplementedException(); }
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            return SendAsync(request, CancellationToken.None);
         }

         public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            lock (_lock)
            {
               _currentRequests++;
               TotalRequests++;
               MaxConcurrentRequests = _currentRequests > MaxConcurrentRequests ? _currentRequests : MaxConcurrentRequests;
            }

            await Task.Delay(10, cancellationToken);

            lock (_lock)
            {
               _currentRequests--;
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
         }
      }
   }
}
