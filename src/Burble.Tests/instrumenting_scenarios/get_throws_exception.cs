namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using Burble.Abstractions;
   using Xunit;
   using Shouldly;

   public class get_throws_exception
   {
      private readonly Exception _exceptionThrown;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _exception;

      public get_throws_exception()
      {
         _exceptionThrown = new Exception();
         var baseHttpClient = new ExceptionHttpClient(_exceptionThrown);
         var httpClient = baseHttpClient.AddInstrumenting(_callback);

         try
         {
            httpClient.GetAsync("/ping").Wait();
         }
         catch (Exception e)
         {
            _exception = e;
         }
      }
      
      [Fact]
      public void should_log_request_initiated()
      {
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe("http://exception-host/ping");
         lastRequest.Method.ShouldBe("GET");
      }

      [Fact]
      public void should_not_log_response()
      {
         _callback.ResponsesReceived.ShouldBeEmpty();
      }

      [Fact]
      public void should_not_log_timeout_received()
      {
         _callback.TimeOuts.ShouldBeEmpty();
      }

      [Fact]
      public void should_log_exception_received()
      {
         var lastException = _callback.ExceptionsThrown.Last();
         lastException.ShouldNotBeNull();
         lastException.EventType.ShouldBe("HttpClientExceptionThrown");
         lastException.Uri.ShouldBe("http://exception-host/ping");
         lastException.Method.ShouldBe("GET");
         lastException.Exception.ShouldBeSameAs(_exceptionThrown);
      }
      
      [Fact]
      public void should_throw_http_client_exception()
      {
         _exception.ShouldBeOfType<AggregateException>();
         _exception.InnerException.ShouldBeOfType<HttpClientException>();
      }

      [Fact]
      public void should_populate_http_client_exception()
      {
         var httpClientException = (HttpClientException)_exception.InnerException;

         httpClientException.InnerException.ShouldBeSameAs(_exceptionThrown);
         httpClientException.RequestUri.ShouldBe(new Uri("http://exception-host/ping"));
         httpClientException.Method.ShouldBe(HttpMethod.Get);
         httpClientException.Message.ShouldBe("Unexpected exception, GET http://exception-host/ping");
      }
   }
}
