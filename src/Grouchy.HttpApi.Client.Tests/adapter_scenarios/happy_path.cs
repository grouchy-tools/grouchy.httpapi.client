using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Client.Tests.adapter_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class happy_path
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         using (var webApi = new PingHttpApi())
         using (var httpClient = webApi.CreateClient())
         {
            _response = await httpClient.GetAsync("/ping");
         }
      }
      
      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public async Task should_return_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         content.ShouldBe("pong");
      }
   }
}
