namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using Banshee;
   using Burble.Exceptions;
   using NUnit.Framework;
   using Shouldly;

   [TestFixture("GET", "http://fail", "/ping", "http://fail/ping", "Server unavailable, GET http://fail/ping")]
   [TestFixture("GET", null, "http://fail/ping", "http://fail/ping", "Server unavailable, GET http://fail/ping")]
   [TestFixture("HEAD", "http://fail", "/ping", "http://fail/ping", "Server unavailable, HEAD http://fail/ping")]
   [TestFixture("HEAD", null, "http://fail/ping", "http://fail/ping", "Server unavailable, HEAD http://fail/ping")]
   [TestFixture("GET", "http://localhost:9010", "/status", "http://localhost:9010/status", "Server unavailable, GET http://localhost:9010/status")]
   public class get_server_not_found
   {
      private readonly string _method;
      private readonly string _absoluteUri;
      private readonly string _exceptionMessage;
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      private readonly Exception _requestException;

      public get_server_not_found(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         _method = method;
         _absoluteUri = absoluteUri;
         _exceptionMessage = exceptionMessage;

         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient { BaseAddress = baseAddress != null ? new Uri(baseAddress) : null })
         {
            var httpClient = baseHttpClient.AddLogging(_callback);
            var message = new HttpRequestMessage(new HttpMethod(method), requestUri);

            try
            {
               httpClient.SendAsync(message).Wait();
            }
            catch (Exception e)
            {
               _requestException = e;
            }
         }
      }
      
      [Test]
      public void should_throw_http_client_connection_exception()
      {
         _requestException.ShouldBeOfType<AggregateException>();

         _requestException.InnerException.ShouldBeOfType<HttpClientServerUnavailableException>();
      }

      [Test]
      public void should_populate_http_client_exception_message()
      {
         var httpClientConnectionException = _requestException.InnerException;

         httpClientConnectionException.InnerException.ShouldBeNull();
         httpClientConnectionException.Message.ShouldBe(_exceptionMessage);
      }

      [Test]
      public void should_populate_http_client_exception_request_id()
      {
         var httpClientConnectionException = (HttpClientServerUnavailableException)_requestException.InnerException;

         httpClientConnectionException.RequestUri.ShouldBe(new Uri(_absoluteUri));
         httpClientConnectionException.Method.ShouldBe(new HttpMethod(_method));
      }

      [Test]
      public void should_not_log_exception_thrown()
      {
         _callback.ExceptionsThrown.ShouldBeEmpty();
      }

      [Test]
      public void should_log_server_unavailable()
      {
         var lastException = _callback.ServersUnavailable.Last();
         lastException.ShouldNotBeNull();
         lastException.EventType.ShouldBe("HttpClientServerUnavailable");
         lastException.Uri.ShouldBe(_absoluteUri);
         lastException.Method.ShouldBe(_method);
      }
   }
}
