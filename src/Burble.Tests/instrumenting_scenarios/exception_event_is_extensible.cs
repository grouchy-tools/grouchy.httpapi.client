namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Linq;
   using Burble.Abstractions;
   using Xunit;
   using Shouldly;

   public class exception_event_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      public exception_event_is_extensible()
      {
         var baseHttpClient = new ExceptionHttpClient(new Exception());
         var httpClient = baseHttpClient.AddInstrumenting(_callback);

         try
         {
            httpClient.GetAsync("/ping").Wait();
         }
         catch
         {
            // Ignore the exception
         }
      }

      [Fact]
      public void should_log_exception_thrown()
      {
         var lastRequest = _callback.ExceptionsThrown.Last();
         lastRequest.Tags.Count.ShouldBe(1);
         lastRequest.Tags["Key"].ShouldBe("HttpClientExceptionThrown");
      }
   }
}
