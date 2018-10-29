using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Banshee;
using Burble.Abstractions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

#if NET451
   using HttpContext = Microsoft.Owin.IOwinContext;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Burble.Tests.retrying_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_has_content_and_content_headers
   {
      private readonly StubHttpClientEventCallback _callback = new StubHttpClientEventCallback();
      
      private string _content;

      [OneTimeSetUp]
      public async Task setup_scenario()
      { 
         using (var webApi = new PingWebApi())
         using (var httpClient = webApi.CreateClientWithRetrying(_callback, retries: 1, delayMs: 10, timeoutMs: 100000))
         {
            var message = new HttpRequestMessage(HttpMethod.Post, "/get-content");

            message.Content = new StringContent("{\"contentA\":\"valueA\",\"contentB\":\"valueB\"}");
            message.Content.Headers.Add("headerC", "valueC");
            message.Content.Headers.Add("headerD", "valueD");

            var response = await httpClient.SendAsync(message);
            _content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(_content);
         }
      }

      [Test]
      public void response_should_be_json()
      {
         var json = JObject.Parse(_content);
         json.ShouldNotBeNull();
      }

      [Test]
      public void body_values_are_returned()
      {
         var json = JObject.Parse(_content);
         json.ShouldContainKeyAndValue("contentA", "valueA");
         json.ShouldContainKeyAndValue("contentB", "valueB");
      }

      [Test]
      public void header_values_are_returned()
      {
         var json = JObject.Parse(_content);
         json.ShouldContainKeyAndValue("headerC", "valueC");
         json.ShouldContainKeyAndValue("headerD", "valueD");
      }
      
      private class PingWebApi : StubWebApiHost
      {
         private bool _slowInitially = true;

         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "POST" && context.Request.Path.ToString() == "/get-content")
            {
               if (_slowInitially)
               {
                  _slowInitially = false;
                  await Task.Delay(100);
               }

               JObject body;
               using (var reader = new StreamReader(context.Request.Body))
               using (var jsonReader = new JsonTextReader(reader))
               {
                  body = JObject.Load(jsonReader);
               } 

               var response = new JObject();

               foreach (var j in body)
               {
                  response.Add(j.Key, j.Value);
               }

               foreach (var j in context.Request.Headers)
               {
                  response.Add(j.Key, string.Join(",", j.Value));
               }

               context.Response.StatusCode = (int)HttpStatusCode.OK;
               var json = JsonConvert.SerializeObject(response);

               await context.Response.WriteAsync(json);
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
