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

   public class multiple_retry_with_timeout
   {
      private const int ExpectedRetries = 3;

      private readonly string _eventUri;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _exception;

      public multiple_retry_with_timeout()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(10) })
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            var httpClient = baseHttpClient.AddRetrying(
               new StubRetryPredicate(ExpectedRetries),
               new StubRetryDelay(10),
               _callback);

            try
            {
               httpClient.GetAsync("/ping").Wait();
            }
            catch (Exception e)
            {
               _exception = e;
            }
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
      public void should_throw_task_cancelled_exception()
      {
         _exception.ShouldBeOfType<AggregateException>();

         var innerException = _exception.InnerException;
         innerException.ShouldBeOfType<TaskCanceledException>();
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
               await Task.Delay(100);

               context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
