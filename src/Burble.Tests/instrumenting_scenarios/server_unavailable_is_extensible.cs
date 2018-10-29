using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class server_unavailable_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var exception = CreateHttpRequestException();
         var baseHttpClient = new StubHttpClient(exception);
         var configuration = new InstrumentingConfiguration { Uri = new Uri("http://localhost")};
         var httpClient = baseHttpClient.AddInstrumenting(configuration, new[]{_callback});

         try
         {
            await httpClient.GetAsync("/ping");
         }
         catch
         {
            // Ignore the exception;
         }
      }

      [Test]
      public void should_log_server_unavailable()
      {
         var lastRequest = _callback.ServersUnavailable.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientServerUnavailable");
      }

      private static Exception CreateHttpRequestException()
      {
#if NET451
         var inner = new HttpRequestException("Unable to connect to the remote server");
#else
         var inner = new HttpRequestException("A connection with the server could not be established");
#endif

         return new HttpRequestException("Ooops", inner);
      }
   }
}
