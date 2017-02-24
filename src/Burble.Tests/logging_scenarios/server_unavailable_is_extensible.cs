namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using Burble.Abstractions;
   using NUnit.Framework;
   using Shouldly;

   public class server_unavailable_is_extensible
   {
      private readonly CustomisingHttpClientEventCallback _callback = new CustomisingHttpClientEventCallback();

      public server_unavailable_is_extensible()
      {
         var exception = new HttpClientServerUnavailableException(HttpMethod.Get, new Uri("http://something-not-found"));
         var baseHttpClient = new ExceptionHttpClient(exception);
         var httpClient = baseHttpClient.AddLogging(_callback);

         try
         {
            httpClient.GetAsync("/ping").Wait();
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
   }
}
