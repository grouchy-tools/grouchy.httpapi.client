namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Net.Http;
   using Banshee;
   using Burble.Abstractions;
   using Xunit;
   using Shouldly;

   public class get_server_not_found
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _method;
      private string _absoluteUri;
      private string _exceptionMessage;
      private Exception _requestException;

      private void act(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         _method = method;
         _absoluteUri = absoluteUri;
         _exceptionMessage = exceptionMessage;

         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient { BaseAddress = baseAddress != null ? new Uri(baseAddress) : null })
         {
            var httpClient = baseHttpClient.AddInstrumenting(_callback);
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

      public static IEnumerable<object[]> TestData { get; } = new[]
      {
         new object[] { "GET", "http://fail", "/ping", "http://fail/ping", "Server unavailable, GET http://fail/ping" },
         new object[] { "GET", null, "http://fail/ping", "http://fail/ping", "Server unavailable, GET http://fail/ping" },
         new object[] { "HEAD", "http://fail", "/ping", "http://fail/ping", "Server unavailable, HEAD http://fail/ping" },
         new object[] { "HEAD", null, "http://fail/ping", "http://fail/ping", "Server unavailable, HEAD http://fail/ping" },
         new object[] { "GET", "http://localhost:9010", "/status", "http://localhost:9010/status", "Server unavailable, GET http://localhost:9010/status" }
      };

      [Theory, MemberData(nameof(TestData))]
      public void should_throw_http_client_connection_exception(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         act(method, baseAddress, requestUri, absoluteUri, exceptionMessage);

         _requestException.ShouldBeOfType<AggregateException>();

         _requestException.InnerException.ShouldBeOfType<HttpClientServerUnavailableException>();
      }

      [Theory, MemberData(nameof(TestData))]
      public void should_populate_http_client_exception_message(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         act(method, baseAddress, requestUri, absoluteUri, exceptionMessage);

         var httpClientConnectionException = _requestException.InnerException;

         httpClientConnectionException.InnerException.ShouldBeNull();
         httpClientConnectionException.Message.ShouldBe(_exceptionMessage);
      }

      [Theory, MemberData(nameof(TestData))]
      public void should_populate_http_client_exception_request_id(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         act(method, baseAddress, requestUri, absoluteUri, exceptionMessage);

         var httpClientConnectionException = (HttpClientServerUnavailableException)_requestException.InnerException;

         httpClientConnectionException.RequestUri.ShouldBe(new Uri(_absoluteUri));
         httpClientConnectionException.Method.ShouldBe(new HttpMethod(_method));
      }

      [Theory, MemberData(nameof(TestData))]
      public void should_not_log_exception_thrown(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         act(method, baseAddress, requestUri, absoluteUri, exceptionMessage);

         _callback.ExceptionsThrown.ShouldBeEmpty();
      }

      [Theory, MemberData(nameof(TestData))]
      public void should_log_server_unavailable(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         act(method, baseAddress, requestUri, absoluteUri, exceptionMessage);

         var lastException = _callback.ServersUnavailable.Last();
         lastException.ShouldNotBeNull();
         lastException.EventType.ShouldBe("HttpClientServerUnavailable");
         lastException.Uri.ShouldBe(_absoluteUri);
         lastException.Method.ShouldBe(_method);
      }
   }
}
