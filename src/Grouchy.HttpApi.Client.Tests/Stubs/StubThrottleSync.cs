using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubThrottleSync : IThrottleSync
   {
      private readonly SemaphoreSlim _semaphore;

      public StubThrottleSync(int concurrentRequests)
      {
         _semaphore = new SemaphoreSlim(concurrentRequests);
      }

      public Task WaitAsync()
      {
         return _semaphore.WaitAsync();
      }

      public void Release()
      {
         _semaphore.Release();
      }
   }
}