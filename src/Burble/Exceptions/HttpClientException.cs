namespace Burble.Exceptions
{
   using System;
   using System.Net.Http;

   public class HttpClientException : Exception
   {
      public HttpClientException(HttpRequestMessage request, Exception innerException)
         : base($"An error occurred invoking {request.Method} {request.RequestUri}", innerException)
      {
         RequestUri = request.RequestUri;
      }

      public Uri RequestUri { get; }
   }
}