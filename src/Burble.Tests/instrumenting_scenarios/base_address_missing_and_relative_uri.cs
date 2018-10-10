using System;
using System.Net.Http;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.instrumenting_scenarios
{
   public class base_address_missing_and_relative_uri
   {
      private Exception _requestException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var callback = new StubHttpClientEventCallback();

         using (new StubWebApiHost())
         using (var baseHttpClient = new HttpClient())
         {
            var httpClient = baseHttpClient.AddInstrumenting(callback);

            try
            {
               await httpClient.GetAsync("/ping");
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
         _requestException.ShouldBeOfType<ArgumentException>();
      }

      [Test]
      public void should_populate_argument_exception_message()
      {
         var exception = (ArgumentException)_requestException;

         exception.InnerException.ShouldBeNull();
         exception.Message.ShouldBe($"requestUri cannot be UriKind.Relative if BaseAddress has not been specified{Environment.NewLine}Parameter name: request");
      }
   }
}
