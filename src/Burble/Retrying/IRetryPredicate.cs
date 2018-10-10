using System.Net.Http;

namespace Burble.Retrying
{
   public interface IRetryPredicate
   {
      bool ShouldRetry(int retryAttempt, HttpResponseMessage response);
   }
}
