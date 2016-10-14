namespace Burble.Exceptions
{
   using System;

   public class HttpClientException : Exception
   {
      public HttpClientException(string message, Exception innerException)
         : base(message, innerException)
      {
      }
   }
}