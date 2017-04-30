﻿namespace Burble.Tests.instrumenting_scenarios
{
   using System;
   using System.Net.Http;
   using Banshee;
   using Burble.Abstractions;
   using Xunit;
   using Shouldly;

   public class base_address_missing_and_relative_uri
   {
      private readonly Exception _requestException;

      public base_address_missing_and_relative_uri()
      {
         var callback = new StubHttpClientEventCallback();

         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient())
         {
            var httpClient = baseHttpClient.AddInstrumenting(callback);

            try
            {
               httpClient.GetAsync("/ping").Wait();
            }
            catch (Exception e)
            {
               _requestException = e;
            }
         }
      }
      
      [Fact]
      public void should_throw_argument_exception()
      {
         _requestException.ShouldBeOfType<AggregateException>();

         _requestException.InnerException.ShouldBeOfType<ArgumentException>();
      }

      [Fact]
      public void should_populate_argument_exception_message()
      {
         var exception = (ArgumentException)_requestException.InnerException;

         exception.InnerException.ShouldBeNull();
         exception.Message.ShouldBe("requestUri cannot be UriKind.Relative if BaseAddress has not been specified\r\nParameter name: request");
      }
   }
}
