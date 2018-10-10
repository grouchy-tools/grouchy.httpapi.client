using System;
using System.Net.Http;

namespace Burble.Extensions
{
   public static class HttpRequestMessageExtensions
   {
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
