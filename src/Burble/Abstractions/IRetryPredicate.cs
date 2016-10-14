namespace Burble.Abstractions
{
   using System.Net.Http;

   public interface IRetryPredicate
   {
      bool ShouldRetry(int retryAttempt, HttpResponseMessage response);
   }
}
