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

   public class success_so_no_retry
   {
      private readonly List<HttpClientRetryAttempt> _retryAttempts = new List<HttpClientRetryAttempt>();
      private readonly HttpResponseMessage _response;

      public success_so_no_retry()
      {
         using (var webApi = new success_so_no_retry.PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddRetrying(
               new StubRetryPredicate(3),
               new StubRetryDelay(10),
               e => { _retryAttempts.Add(e); });

            _response = httpClient.GetAsync("/ping").Result;
         }
      }

      [Test]
      public void should_not_log_retry_attempts()
      {
         _retryAttempts.Count.ShouldBe(0);
      }

      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public void should_return_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         content.ShouldBe("pong");
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
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
