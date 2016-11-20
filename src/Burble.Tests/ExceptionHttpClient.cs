namespace Burble.Tests
{
   using System;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class ExceptionHttpClient : IHttpClient
   {
      private readonly Exception _exception;

      public ExceptionHttpClient(Exception exception)
      {
         _exception = exception;
      }

      public Uri BaseAddress { get; } = new Uri("http://exception-host");

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         throw _exception;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         throw _exception;
      }
   }
}