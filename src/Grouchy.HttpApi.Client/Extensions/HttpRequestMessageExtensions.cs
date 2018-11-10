using System.Net.Http;

namespace Grouchy.HttpApi.Client.Extensions
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
   }
}
