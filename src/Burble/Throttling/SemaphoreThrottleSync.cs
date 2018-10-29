using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.Throttling;

namespace Burble.Throttling
{
   // TODO: Should support IDisposable
   public class SemaphoreThrottleSync : IThrottleSync
   {
      private readonly SemaphoreSlim _semaphore;

      public SemaphoreThrottleSync(int concurrentRequests)
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
