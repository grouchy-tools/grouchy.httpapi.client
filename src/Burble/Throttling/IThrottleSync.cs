namespace Burble.Abstractions
{
   using System.Threading.Tasks;

   public interface IThrottleSync
   {
      Task WaitAsync();

      void Release();
   }
}
