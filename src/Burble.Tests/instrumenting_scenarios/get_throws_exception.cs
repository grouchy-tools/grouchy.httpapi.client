using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Burble.Abstractions.Exceptions;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class get_throws_exception
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private Exception _exceptionThrown;
      private Exception _exception;

      [OneTimeSetUp]
      public async Task  setup_scenario()
      {
         _exceptionThrown = new Exception();
         var baseHttpClient = new StubHttpClient(_exceptionThrown);
         var configuration = new InstrumentingConfiguration {Uri = new Uri("http://exception-host")};
         var httpClient = baseHttpClient.AddInstrumenting(configuration, new []{_callback});

         try
         {
            await httpClient.GetAsync("/ping");
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
         lastRequest.Uri.ShouldBe("http://exception-host/ping");
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
         lastException.Uri.ShouldBe("http://exception-host/ping");
         lastException.Method.ShouldBe("GET");
         lastException.Exception.ShouldBeSameAs(_exceptionThrown);
      }
      
      [Test]
      public void should_throw_http_client_exception()
      {
         _exception.ShouldBeOfType<HttpClientException>();
      }

      [Test]
      public void should_populate_http_client_exception()
      {
         var httpClientException = (HttpClientException)_exception;

         httpClientException.InnerException.ShouldBeSameAs(_exceptionThrown);
         httpClientException.RequestUri.ShouldBe(new Uri("http://exception-host/ping"));
         httpClientException.Method.ShouldBe(HttpMethod.Get);
         httpClientException.Message.ShouldBe("Unexpected exception, GET http://exception-host/ping");
      }
   }
}
