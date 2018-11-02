using System;
using System.Linq;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using Grouchy.HttpApi.Client.Tests.Configuration;
using Grouchy.HttpApi.Client.Tests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.instrumenting_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class exception_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var baseHttpClient = new StubHttpClient(new Exception());
         var configuration = new InstrumentingConfiguration {Uri =new Uri("http://localhost")};
         var httpClient = baseHttpClient.AddInstrumenting(configuration, new[]{_callback});

         try
         {
            await httpClient.GetAsync("/ping");
         }
         catch
         {
            // Ignore the exception
         }
      }

      [Test]
      public void should_log_exception_thrown()
      {
         var lastRequest = _callback.ExceptionsThrown.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientExceptionThrown");
      }
   }
}
