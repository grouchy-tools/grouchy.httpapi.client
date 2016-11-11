namespace Burble
{
   using System;
   using System.Linq;
   using System.Collections.Generic;
   using System.Net.Http;

   public static class HttpRequestMessageExtensions
    {
      public static string EnsureRequestIdIsInHeaders(this HttpRequestMessage request)
      {
         IEnumerable<string> values;
         if (request.Headers.TryGetValues("request-id", out values))
         {
            return values.First();
         }

         var requestId = Guid.NewGuid().ToString();
         request.Headers.Add("request-id", requestId);

         return requestId;
      }
   }
}
