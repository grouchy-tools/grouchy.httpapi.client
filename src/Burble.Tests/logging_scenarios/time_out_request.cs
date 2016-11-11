namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Linq;
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

   public class time_out_request
   {
      private readonly string _existingRequestId;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _timeoutException;

      public time_out_request()
      {
         _existingRequestId = Guid.NewGuid().ToString();

         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient {BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(50)})
         {
            var httpClient = baseHttpClient.AddLogging(_callback);

            var message = new HttpRequestMessage(HttpMethod.Get, "/ping");
            message.Headers.Add("request-id", _existingRequestId);

            try
            {
               httpClient.SendAsync(message).Wait();
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
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe("/ping");
         lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_not_log_response()
      {
         _callback.ResponsesReceived.ShouldBeEmpty();
      }

      [Test]
      public void should_log_timeout_received()
      {
         var lastTimeout = _callback.TimeOuts.Last();
         lastTimeout.ShouldNotBeNull();
         lastTimeout.EventType.ShouldBe("HttpClientTimedOut");
         lastTimeout.Uri.ShouldBe("/ping");
         lastTimeout.Method.ShouldBe("GET");
      }

      [Test]
      public void should_log_timeout_with_matching_request_id()
      {
         var lastTimeout = _callback.TimeOuts.Last();

         lastTimeout.RequestId.ShouldBe(_existingRequestId);
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
