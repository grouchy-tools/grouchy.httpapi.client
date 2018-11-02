using System;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubThrottleManager : IThrottleManager
   {
      public IThrottleSync Sync { get; set; }

      public void Register(string policy, IThrottleSync throttleSync)
      {
         throw new NotImplementedException();
      }

      public IThrottleSync GetSync(string policy)
      {
         if (policy != "default") throw new InvalidOperationException("ThrottlePolicy not found");

         return Sync;
      }
   }
}