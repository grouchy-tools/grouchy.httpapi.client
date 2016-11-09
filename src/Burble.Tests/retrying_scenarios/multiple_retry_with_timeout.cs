namespace Burble.Tests.retrying_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Exceptions;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class multiple_retry_with_timeout
   {
      private const int ExpectedRetries = 3;

      private readonly StubLoggingCallback _callback = new StubLoggingCallback();
      private readonly Exception _exception;

      public multiple_retry_with_timeout()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient {BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(10)})
         {
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

      [Test]
      public void should_log_three_attempts()
      {
         _callback.RetryAttempts.Count.ShouldBe(ExpectedRetries);
         _callback.RetryAttempts[0].Attempt.ShouldBe(1);
         _callback.RetryAttempts[1].Attempt.ShouldBe(2);
         _callback.RetryAttempts[2].Attempt.ShouldBe(3);
      }

      [Test]
      public void should_log_retry_attempt()
      {
         _callback.RetryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _callback.RetryAttempts[0].Uri.ShouldBe("/ping");
         _callback.RetryAttempts[0].Method.ShouldBe("GET");
      }

      [Test]
      public void should_throw_http_client_timeout_exception()
      {
         _exception.ShouldBeOfType<AggregateException>();

         var innerException = _exception.InnerException;
         innerException.ShouldBeOfType<HttpClientTimeoutException>();
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
