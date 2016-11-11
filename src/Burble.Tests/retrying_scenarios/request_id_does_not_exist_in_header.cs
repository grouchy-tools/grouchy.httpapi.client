namespace Burble.Tests.retrying_scenarios
{
   using System;
   using System.Linq;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class request_id_does_not_exist_in_header
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly string _requestIdFromHeader;

      public request_id_does_not_exist_in_header()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(10) })
         {
            var httpClient = baseHttpClient.AddRetrying(
               new StubRetryPredicate(1),
               new StubRetryDelay(10),
               _callback);

            var response = httpClient.GetAsync("/get-request-id").Result;
            _requestIdFromHeader = response.Content.ReadAsStringAsync().Result;
         }
      }

      [Test]
      public void should_add_non_empty_request_id_to_headers()
      {
         Guid requestId;
         Guid.TryParse(_requestIdFromHeader, out requestId).ShouldBe(true);
         requestId.ShouldNotBe(Guid.Empty);
      }

      [Test]
      public void should_log_retry_with_matching_request_id()
      {
         var lastRetry = _callback.RetryAttempts.Last();

         lastRetry.RequestId.ShouldBe(_requestIdFromHeader);
      }
      
      private class PingWebApi : StubWebApiHost
      {
         private bool _slowInitially = true;

         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/get-request-id")
            {
               if (_slowInitially)
               {
                  _slowInitially = false;
                  await Task.Delay(100);
               }

               context.Response.StatusCode = (int)HttpStatusCode.OK;
               await context.Response.WriteAsync(context.Request.Headers["request-id"].ToString());
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
