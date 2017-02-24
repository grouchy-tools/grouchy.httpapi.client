namespace Burble.Tests.simple_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using Burble.Abstractions;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class time_out_request
   {
      private readonly Exception _timeoutException;
      private readonly Uri _exceptionUrl;

      public time_out_request()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(50) })
         {
            _exceptionUrl = new Uri(webApi.BaseUri, "/ping");

            var httpClient = new SimpleHttpClient(baseHttpClient);

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
      public void should_throw_http_client_timeout_exception()
      {
         _timeoutException.ShouldBeOfType<AggregateException>();

         var innerException = _timeoutException.InnerException;
         innerException.ShouldBeOfType<HttpClientTimeoutException>();
      }

      [Test]
      public void should_populate_http_client_timeout_exception()
      {
         var timeoutException = (HttpClientTimeoutException)_timeoutException.InnerException;

         timeoutException.InnerException.ShouldBeNull();
         timeoutException.RequestUri.ShouldBe(_exceptionUrl);
         timeoutException.Method.ShouldBe(HttpMethod.Get);
         timeoutException.Message.ShouldBe($"Request timed-out, GET {_exceptionUrl}");
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
