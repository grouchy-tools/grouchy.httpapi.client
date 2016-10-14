namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Events;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class get_throws_exception
   {
      private readonly Exception _exceptionThrown;
      private readonly Exception _exception;

      private HttpClientRequestInitiated _lastRequest;
      private HttpClientResponseReceived _lastResponse;
      private HttpClientTimedOut _lastTimeout;
      private HttpClientExceptionThrown _lastException;

      public get_throws_exception()
      {
         _exceptionThrown = new Exception();
         var baseHttpClient = new ExceptionHttpClient(_exceptionThrown);
         var httpClient = baseHttpClient.AddLogging(
            e => { _lastRequest = e; },
            e => { _lastResponse = e; },
            e => { _lastTimeout = e; },
            e => { _lastException = e; });

         try
         {
            httpClient.GetAsync("/ping").Wait();
         }
         catch (Exception e)
         {
            _exception = e;
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
      public void should_not_log_timeout_received()
      {
         _lastTimeout.ShouldBeNull();
      }

      [Test]
      public void should_log_exception_received()
      {
         _lastException.ShouldNotBeNull();
         _lastException.EventType.ShouldBe("HttpClientExceptionThrown");
         _lastException.Uri.ShouldBe("/ping");
         _lastException.Method.ShouldBe("GET");
         _lastException.Exception.ShouldBeSameAs(_exceptionThrown);
      }

      [Test]
      public void should_throw_http_client_exception()
      {
         _exception.ShouldBeOfType<AggregateException>();
         _exception.InnerException.ShouldBeSameAs(_exceptionThrown);
      }

      private class ExceptionHttpClient : IHttpClient
      {
         private readonly Exception _exception;

         public ExceptionHttpClient(Exception exception)
         {
            _exception = exception;
         }

         public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            throw _exception;
         }
      }
   }
}
