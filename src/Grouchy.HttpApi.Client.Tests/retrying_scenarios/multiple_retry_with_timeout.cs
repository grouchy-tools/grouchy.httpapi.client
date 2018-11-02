using System;
using System.Net;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Testing;
using Grouchy.HttpApi.Client.Tests.Extensions;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Grouchy.HttpApi.Client.Tests.retrying_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class multiple_retry_with_timeout : scenario_base
   {
      private const int ExpectedRetries = 3;

      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private Exception _exception;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var webApi = new PingHttpApi())
         using (var httpClient = webApi.CreateClientWithRetrying(_callback, retries: ExpectedRetries, delayMs: 10, timeoutMs: 10))
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            try
            {
               await httpClient.GetAsync("/ping");
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
         _callback.RetryAttempts.Length.ShouldBe(ExpectedRetries);
         _callback.RetryAttempts[0].Attempt.ShouldBe(1);
         _callback.RetryAttempts[1].Attempt.ShouldBe(2);
         _callback.RetryAttempts[2].Attempt.ShouldBe(3);
      }

      [Test]
      public void should_log_retry_attempt()
      {
         _callback.RetryAttempts[0].EventType.ShouldBe("HttpClientRetryAttempt");
         _callback.RetryAttempts[0].Uri.ShouldBe(_eventUri);
         _callback.RetryAttempts[0].Method.ShouldBe("GET");
      }
      
      [Test]
      public void should_throw_task_cancelled_exception()
      {
         _exception.ShouldBeOfType<TaskCanceledException>();
      }

      private class PingHttpApi : StubHttpApi
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
