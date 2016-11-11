namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using NUnit.Framework;
   using Shouldly;

   public class get_throws_exception
   {
      private readonly string _existingRequestId;
      private readonly Exception _exceptionThrown;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _exception;

      public get_throws_exception()
      {
         _existingRequestId = Guid.NewGuid().ToString();

         _exceptionThrown = new Exception();
         var baseHttpClient = new ExceptionHttpClient(_exceptionThrown);
         var httpClient = baseHttpClient.AddLogging(_callback);

         var message = new HttpRequestMessage(HttpMethod.Get, "/ping");
         message.Headers.Add("request-id", _existingRequestId);

         try
         {
            httpClient.SendAsync(message).Wait();
         }
         catch (Exception e)
         {
            _exception = e;
         }
      }
      
      [Test]
      public void should_log_request_initiated()
      {
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.ShouldNotBeNull();
         lastRequest.EventType.ShouldBe("HttpClientRequestInitiated");
         lastRequest.Uri.ShouldBe("/ping");
         lastRequest.Method.ShouldBe("GET");
      }

      [Test]
      public void should_not_log_response()
      {
         _callback.ResponsesReceived.ShouldBeEmpty();
      }

      [Test]
      public void should_not_log_timeout_received()
      {
         _callback.TimeOuts.ShouldBeEmpty();
      }

      [Test]
      public void should_log_exception_received()
      {
         var lastException = _callback.ExceptionsThrown.Last();
         lastException.ShouldNotBeNull();
         lastException.EventType.ShouldBe("HttpClientExceptionThrown");
         lastException.Uri.ShouldBe("/ping");
         lastException.Method.ShouldBe("GET");
         lastException.Exception.ShouldBeSameAs(_exceptionThrown);
      }

      [Test]
      public void should_log_exception_with_matching_request_id()
      {
         var lastException = _callback.ExceptionsThrown.Last();

         lastException.RequestId.ShouldBe(_existingRequestId);
      }

      [Test]
      public void should_throw_http_client_exception()
      {
         _exception.ShouldBeOfType<AggregateException>();
         _exception.InnerException.ShouldBeSameAs(_exceptionThrown);
      }
   }
}
