using System;
using System.Linq;
using Burble.Abstractions.Extensions;
using Burble.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.instrumenting_scenarios
{
   public class exception_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var baseHttpClient = new ExceptionHttpClient(new Exception());
         var httpClient = baseHttpClient.AddInstrumenting(new[]{_callback});

         try
         {
            httpClient.GetAsync("/ping").Wait();
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
