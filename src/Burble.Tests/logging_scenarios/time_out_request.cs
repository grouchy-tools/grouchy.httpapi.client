namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Events;
   using Burble.Exceptions;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;

#endif

   public class time_out_request
   {
      private HttpClientRequestInitiated _lastRequest;
      private HttpClientResponseReceived _lastResponse;
      private HttpClientTimedOut _lastTimeout;
      private Exception _timeoutException;

      public time_out_request()
      {
         using (var webApi = new PingWebApi())
         using (
            var baseHttpClient = new HttpClient {BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(50)})
         {
            var httpClient = baseHttpClient.AddLogging(
               e => { _lastRequest = e; },
               e => { _lastResponse = e; },
               e => { _lastTimeout = e; },
               e => { });

            try
            {
               httpClient.GetAsync("/ping").Wait();
            }
            catch (Exception e)
            {
               _timeoutException = e;
            }
         }
      }
      
      [Test]
      public void should_log_request_initiated()
      {
         _lastRequest.ShouldNotBeNull();
         _lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         _lastRequest.Uri.ShouldBe("/ping");
         _lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_not_log_response()
      {
         _lastResponse.ShouldBeNull();
      }

      [Test]
      public void should_log_timeout_received()
      {
         _lastTimeout.ShouldNotBeNull();
         _lastTimeout.EventType.ShouldBe("HttpClientTimedOut");
         _lastTimeout.Uri.ShouldBe("/ping");
         _lastTimeout.Method.ShouldBe("GET");
      }

      [Test]
      public void should_throw_http_client_timeout_exception()
      {
         _timeoutException.ShouldBeOfType<AggregateException>();

         var innerException = _timeoutException.InnerException;
         innerException.ShouldBeOfType<HttpClientTimeoutException>();
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
               await Task.Delay(2000);
               context.Response.StatusCode = (int)HttpStatusCode.OK;
               await context.Response.WriteAsync("pong");
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
