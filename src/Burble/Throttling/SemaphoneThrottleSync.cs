namespace Burble.Throttling
{
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class SemaphoneThrottleSync : IThrottleSync
   {
      private readonly SemaphoreSlim _semaphore;

      public SemaphoneThrottleSync(int concurrentRequests)
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
