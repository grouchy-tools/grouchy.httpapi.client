using System;
using Grouchy.Abstractions.Tagging;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubSessionIdAccessor : ISessionIdAccessor
   {
      public string Response { get; set; }

      public Exception Exception { get; set; }

      public string SessionId
      {
         get
         {
            if (Exception != null)
            {
               throw Exception;
            }

            return Response;
         }
      }
   }
}