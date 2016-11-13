namespace Burble.Exceptions
{
   using System;
   using System.Net.Http;

   public class HttpClientServerUnavailableException : Exception
   {
      public HttpClientServerUnavailableException(HttpRequestMessage request)
         : base($"Server unavailable, {request.Method} {request.RequestUri}")
      {
         RequestUri = request.RequestUri;
      }

      public Uri RequestUri { get; }
   }
}