using System.Threading.Tasks;

namespace Burble.Throttling
{
   public interface IThrottleSync
   {
      Task WaitAsync();

      void Release();
   }
}
