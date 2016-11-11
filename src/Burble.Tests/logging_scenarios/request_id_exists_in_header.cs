namespace Burble.Tests.logging_scenarios
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

   public class request_id_exists_in_header
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly string _existingRequestId;
      private readonly string _requestIdFromHeader;

      public request_id_exists_in_header()
      {
         _existingRequestId = Guid.NewGuid().ToString();

         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = baseHttpClient.AddLogging(_callback);

            var message = new HttpRequestMessage(HttpMethod.Get, "/get-request-id");
            message.Headers.Add("request-id", _existingRequestId);

            var response = httpClient.SendAsync(message).Result;
            _requestIdFromHeader = response.Content.ReadAsStringAsync().Result;
         }
      }

      [Test]
      public void should_preserve_request_id_in_headers()
      {
         _requestIdFromHeader.ShouldBe(_existingRequestId);
      }

      [Test]
      public void should_log_request_with_matching_request_id()
      {
         var lastRequest = _callback.RequestsInitiated.Last();

         lastRequest.RequestId.ShouldBe(_existingRequestId);
      }

      [Test]
      public void should_log_response_with_matching_request_id()
      {
         var lastResponse = _callback.ResponsesReceived.Last();

         lastResponse.RequestId.ShouldBe(_existingRequestId);
      }

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/get-request-id")
            {
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
