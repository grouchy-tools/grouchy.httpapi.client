using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions.Exceptions;
using Burble.Abstractions.Extensions;
using NUnit.Framework;
using Shouldly;

#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Burble.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class time_out_request
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _eventUri;
      private Exception _timeoutException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var webApi = new PingWebApi())
         using (var httpClient = webApi.CreateClientWithInstrumenting(_callback, timeoutMs: 50))
         {
            _eventUri = new Uri(webApi.BaseUri, "/ping").ToString();

            try
            {
               await httpClient.GetAsync("/ping");
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
         lastRequest.Uri.ShouldBe(_eventUri);
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
         lastTimeout.Uri.ShouldBe(_eventUri);
         lastTimeout.Method.ShouldBe("GET");
      }
      
      [Test]
      public void should_throw_http_client_timeout_exception()
      {
         _timeoutException.ShouldBeOfType<HttpClientTimeoutException>();
      }

      [Test]
      public void should_populate_http_client_timeout_exception()
      {
         var timeoutException = (HttpClientTimeoutException)_timeoutException;

         timeoutException.InnerException.ShouldBeNull();
         timeoutException.RequestUri.ToString().ShouldBe(_eventUri);
         timeoutException.Method.ShouldBe(HttpMethod.Get);
         timeoutException.Message.ShouldBe($"Request timed-out, GET {_eventUri}");
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
