using System.Net;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Grouchy.HttpApi.Client.Tests.tagging_scenarios
{
   public class GetIdsFromHeadersHttpApi : StubHttpApi
   {
      protected override async Task Handler(HttpContext context)
      {
         if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/get-ids-from-headers")
         {
            var response = new
            {
               requestId = context.Request.Headers["request-id"].ToString(),
               correlationId = context.Request.Headers["correlation-id"].ToString(),
               sessionId = context.Request.Headers["session-id"].ToString(),
               userAgent = context.Request.Headers[HeaderNames.UserAgent].ToString()
            };

            var json = JsonConvert.SerializeObject(response);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(json);
         }
         else
         {
            await base.Handler(context);
         }
      }
   }
}