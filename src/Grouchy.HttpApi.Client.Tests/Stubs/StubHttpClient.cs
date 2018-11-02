using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubHttpClient : IHttpClient
   {
      private readonly HttpResponseMessage _response;
      private readonly Exception _exception;

      public int SendAsyncCalledCount { get; private set; }
      
      public StubHttpClient(HttpResponseMessage response)
      {
         _response = response;
      }

      public StubHttpClient(Exception exception)
      {
         _exception = exception;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         SendAsyncCalledCount++;

         if (_exception != null)
         {
            throw _exception;
         }
         
         return Task.FromResult(_response);
      }
   }
}
