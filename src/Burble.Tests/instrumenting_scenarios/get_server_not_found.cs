using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Banshee;
using Burble.Abstractions.Exceptions;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using Burble.HttpClients;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.instrumenting_scenarios
{
   // TODO: These are really slow - they were taking over 2 minutes for both frameworks before this hacky approach
   // ReSharper disable once InconsistentNaming
   public class get_server_not_found
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();

      private string _method;
      private string _absoluteUri;
      private string _exceptionMessage;
      private Exception _requestException;

      public static IEnumerable<object[]> TestData { get; } = new[]
      {
         new object[] { "GET", "http://fail", "/ping", "http://fail/ping", "Server unavailable, GET http://fail/ping" },
         new object[] { "HEAD", "http://fail", "/ping", "http://fail/ping", "Server unavailable, HEAD http://fail/ping" },
         new object[] { "GET", "http://localhost:9010", "/status", "http://localhost:9010/status", "Server unavailable, GET http://localhost:9010/status" }
      };

      // TODO: Can't see to get TestFixtureSource and OneTimeSetup working together
      [TestCaseSource(nameof(TestData))]
      public async Task aggregate_tests(string method, string baseAddress, string requestUri, string absoluteUri, string exceptionMessage)
      {
         _method = method;
         _absoluteUri = absoluteUri;
         _exceptionMessage = exceptionMessage;

         var configuration = new InstrumentingConfiguration {Uri = new Uri(baseAddress)};
         
         using (var baseHttpClient = new DefaultHttpClient(configuration))
         {
            var httpClient = baseHttpClient.AddInstrumenting(configuration, new[]{_callback});
            var message = new HttpRequestMessage(new HttpMethod(method), requestUri);

            try
            {
               await httpClient.SendAsync(message);
            }
            catch (Exception e)
            {
               _requestException = e;
            }
         }
         
         // TODO: bit nasty
         should_throw_http_client_connection_exception();
         should_populate_http_client_exception_message();
         should_populate_http_client_exception_request_id();
         should_not_log_exception_thrown();
         should_log_server_unavailable();
      }
      
      private void should_throw_http_client_connection_exception()
      {
         _requestException.ShouldBeOfType<HttpClientServerUnavailableException>();
      }

      private void should_populate_http_client_exception_message()
      {
         _requestException.InnerException.ShouldBeNull();
         _requestException.Message.ShouldBe(_exceptionMessage);
      }

      private void should_populate_http_client_exception_request_id()
      {
         var httpClientConnectionException = (HttpClientServerUnavailableException)_requestException;

         httpClientConnectionException.RequestUri.ShouldBe(new Uri(_absoluteUri));
         httpClientConnectionException.Method.ShouldBe(new HttpMethod(_method));
      }

      private void should_not_log_exception_thrown()
      {
         _callback.ExceptionsThrown.ShouldBeEmpty();
      }

      private void should_log_server_unavailable()
      {
         var lastException = _callback.ServersUnavailable.Last();
         lastException.ShouldNotBeNull();
         lastException.EventType.ShouldBe("HttpClientServerUnavailable");
         lastException.Uri.ShouldBe(_absoluteUri);
         lastException.Method.ShouldBe(_method);
      }
   }
}
