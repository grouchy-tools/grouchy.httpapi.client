namespace Burble.Tests.retrying_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Abstractions;
   using Xunit;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class multiple_retry_without_timeout
   {
      private const int ExpectedRetries = 3;

      private readonly string _eventUri;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly HttpResponseMessage _response;

      public multiple_retry_without_timeout()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            var httpClient = baseHttpClient.AddRetrying(
               new StubRetryPredicate(ExpectedRetries),
               new StubRetryDelay(10),
               _callback);

            _response = httpClient.GetAsync("/ping").Result;
         }
      }

      [Fact]
      public void should_log_three_attempts()
      {
         _callback.RetryAttempts.Length.ShouldBe(ExpectedRetries);
         _callback.RetryAttempts[0].Attempt.ShouldBe(1);
         _callback.RetryAttempts[1].Attempt.ShouldBe(2);
         _callback.RetryAttempts[2].Attempt.ShouldBe(3);
      }

      [Fact]
      public void should_log_retry_attempt()
      {
         _callback.RetryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _callback.RetryAttempts[0].Uri.ShouldBe(_eventUri);
         _callback.RetryAttempts[0].Method.ShouldBe("GET");
      }

      [Fact]
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
