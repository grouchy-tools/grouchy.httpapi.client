using System;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.adapter_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class absolute_uri
   {
      private Exception _requestException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var host = new StubWebApiHost())
         using (var httpClient = host.CreateClient())
         {
            try
            {
               await httpClient.GetAsync(host.BaseUri + "/ping");
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
         exception.Message.ShouldBe($"RequestUri cannot be UriKind.Absolute{Environment.NewLine}Parameter name: request");
      }
   }
}
