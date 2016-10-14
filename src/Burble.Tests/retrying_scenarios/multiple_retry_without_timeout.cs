namespace Burble.Tests.retrying_scenarios
{
   using System.Collections.Generic;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Events;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class multiple_retry_without_timeout
   {
      private const int ExpectedRetries = 3;

      private readonly List<HttpClientRetryAttempt> _retryAttempts = new List<HttpClientRetryAttempt>();
      private readonly HttpResponseMessage _response;

      public multiple_retry_without_timeout()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddRetrying(
               new StubRetryPredicate(ExpectedRetries),
               new StubRetryDelay(10),
               e => { _retryAttempts.Add(e); });

            _response = httpClient.GetAsync("/ping").Result;
         }
      }

      [Test]
      public void should_log_three_attempts()
      {
         _retryAttempts.Count.ShouldBe(ExpectedRetries);
         _retryAttempts[0].Attempt.ShouldBe(1);
         _retryAttempts[1].Attempt.ShouldBe(2);
         _retryAttempts[2].Attempt.ShouldBe(3);
      }

      [Test]
      public void should_log_retry_attempt()
      {
         _retryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _retryAttempts[0].Uri.ShouldBe("/ping");
         _retryAttempts[0].Method.ShouldBe("GET");
      }

      [Test]
      public void should_return_status_code_400()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
               context.Response.StatusCode = 400;
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
