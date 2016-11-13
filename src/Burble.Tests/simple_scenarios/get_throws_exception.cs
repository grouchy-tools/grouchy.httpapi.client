namespace Burble.Tests.simple_scenarios
{
   using System;
   using System.Net.Http;
   using Banshee;
   using Burble.Exceptions;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class get_throws_exception
   {
      private readonly Exception _requestException;

      public get_throws_exception()
      {
         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient { BaseAddress = new Uri("http://fail") })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient);

            try
            {
               httpClient.GetAsync("/ping").Wait();
            }
            catch (Exception e)
            {
               _requestException = e;
            }
         }
      }
      
      [Test]
      public void should_throw_http_client_exception()
      {
         _requestException.ShouldBeOfType<AggregateException>();

         _requestException.InnerException.ShouldBeOfType<HttpClientException>();
      }

      [Test]
      public void should_populate_http_client_exception()
      {
         var httpClientException = (HttpClientException)_requestException.InnerException;

         httpClientException.InnerException.ShouldNotBeNull();         
         httpClientException.RequestUri.ShouldBe(new Uri("http://fail/ping"));         
         httpClientException.Message.ShouldBe("An error occurred invoking GET http://fail/ping");
      }
   }
}
