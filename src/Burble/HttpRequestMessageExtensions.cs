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

      public static string LocalRequestUri(this HttpRequestMessage request)
      {
         if (request.RequestUri.IsAbsoluteUri)
         {
            return request.RequestUri.LocalPath;
         }

         var uri = request.RequestUri.OriginalString;

         if (uri.StartsWith("/"))
         {
            return uri;
         }

         return "/" + uri;
      }

      public static Uri AbsoluteRequestUri(this HttpRequestMessage request, Uri baseAddress)
      {
         if (request.RequestUri.IsAbsoluteUri)
         {
            return request.RequestUri;
         }

         var uri = new Uri(baseAddress, request.RequestUri);
         return uri;
      }
   }
}
