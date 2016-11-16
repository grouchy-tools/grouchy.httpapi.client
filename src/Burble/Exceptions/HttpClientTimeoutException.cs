using System;

namespace Burble.Exceptions
{
   using System.Net.Http;

   public class HttpClientTimeoutException : Exception
   {
      public HttpClientTimeoutException(HttpMethod method, Uri requestUri)
         : base($"Request timed-out, {method} {requestUri}")
      {
         Method = method;
         RequestUri = requestUri;
      }

      public HttpMethod Method { get; }

      public Uri RequestUri { get; }
   }
}
