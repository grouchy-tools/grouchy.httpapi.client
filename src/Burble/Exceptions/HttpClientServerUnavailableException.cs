namespace Burble.Exceptions
{
   using System;
   using System.Net.Http;

   public class HttpClientServerUnavailableException : Exception
   {
      public HttpClientServerUnavailableException(HttpMethod method, Uri requestUri)
         : base($"Server unavailable, {method} {requestUri}")
      {
         Method = method;
         RequestUri = requestUri;
      }

      public HttpMethod Method { get; }

      public Uri RequestUri { get; }
   }
}