namespace Burble.Exceptions
{
   using System;
   using System.Net.Http;

   public class HttpClientException : Exception
   {
      public HttpClientException(HttpMethod method, Uri requestUri, Exception innerException)
         : base($"Unexpected exception, {method} {requestUri}", innerException)
      {
         Method = method;
         RequestUri = requestUri;
      }

      public HttpMethod Method { get; }

      public Uri RequestUri { get; }
   }
}