﻿namespace Burble.Tests.logging_scenarios
{
   using System;
   using System.Linq;
   using NUnit.Framework;
   using Shouldly;

   public class exception_event_is_extensible
   {
      private readonly CustomisingLoggingCallback _callback = new CustomisingLoggingCallback();

      public exception_event_is_extensible()
      {
         var baseHttpClient = new ExceptionHttpClient(new Exception());
         var httpClient = baseHttpClient.AddLogging(_callback);

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
         lastRequest.Tags["Key"].ShouldBe("ExceptionThrown");
      }
   }
}
