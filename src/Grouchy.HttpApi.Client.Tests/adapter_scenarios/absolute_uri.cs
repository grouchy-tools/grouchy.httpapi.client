using System;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Testing;
using Grouchy.HttpApi.Client.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.adapter_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class absolute_uri
   {
      private Exception _requestException;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var host = new StubHttpApi())
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
