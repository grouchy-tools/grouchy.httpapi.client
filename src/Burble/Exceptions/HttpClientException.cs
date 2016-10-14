namespace Burble.Exceptions
{
   using System;

   public class HttpClientException : Exception
   {
      public HttpClientException(Exception innerException)
         : base(null, innerException)
      {
      }

      public Uri RequestUri { get; set; }
   }
}