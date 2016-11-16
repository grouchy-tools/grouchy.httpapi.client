namespace Burble.Tests.simple_scenarios
{
   using System;
   using System.Net.Http;
   using Banshee;
   using NUnit.Framework;
   using Shouldly;

   public class base_address_missing_and_relative_uri
   {
      private readonly Exception _requestException;

      public base_address_missing_and_relative_uri()
      {
         Console.WriteLine("aarr");
         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient())
         {
            var httpClient = new SimpleHttpClient(baseHttpClient);

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
      
      [Test]
      public void should_throw_argument_exception()
      {
         _requestException.ShouldBeOfType<AggregateException>();

         _requestException.InnerException.ShouldBeOfType<ArgumentException>();
      }

      [Test]
      public void should_populate_argument_exception_message()
      {
         var exception = (ArgumentException)_requestException.InnerException;

         exception.InnerException.ShouldBeNull();
         exception.Message.ShouldBe("requestUri cannot be UriKind.Relative if BaseAddress has not been specified\r\nParameter name: request");
      }
   }
}
