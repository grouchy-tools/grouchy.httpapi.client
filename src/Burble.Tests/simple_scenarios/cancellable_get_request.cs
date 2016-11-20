namespace Burble.Tests.simple_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using System.Threading;
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

   public class cancellable_get_request
   {
      private readonly Exception _timeoutException;

      public cancellable_get_request()
      {
         using (var webApi = new SlowWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
            try
            {
               httpClient.GetAsync("/unknown", cancellationTokenSource.Token).Wait();
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

      private class SlowWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            await Task.Delay(1000);

            await base.Handler(context);
         }
      }
   }
}
