using System;

namespace Burble.Exceptions
{
   using System.Net.Http;

   public class HttpClientTimeoutException : Exception
   {
      public HttpClientTimeoutException(HttpRequestMessage request)
         : base($"Request timed-out, {request.Method} {request.RequestUri}")
      {
         RequestUri = request.RequestUri;
      }

      public Uri RequestUri { get; }
   }
}
