namespace Burble.Tests.instrumenting_scenarios
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
         var exception = CreateHttpRequestException();
         var baseHttpClient = new ExceptionHttpClient(exception);
         var httpClient = baseHttpClient.AddInstrumenting(_callback);

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
