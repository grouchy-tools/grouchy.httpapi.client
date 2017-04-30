namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Linq;
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

   public class timeout_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      public timeout_event_is_extensible()
      {
         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient {BaseAddress = webApi.BaseUri, Timeout = TimeSpan.FromMilliseconds(50)})
         {
            var httpClient = baseHttpClient.AddInstrumenting(_callback);

            try
            {
               httpClient.GetAsync("/ping").Wait();
            }
            catch
            {
               // Ignore the exception;
            }
         }
      }

      [Fact]
      public void should_log_timed_out()
      {
         var lastRequest = _callback.TimeOuts.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientTimedOut");
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
