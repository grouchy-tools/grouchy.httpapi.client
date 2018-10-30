using System.Net.Http;
using Burble.Abstractions.Retrying;

namespace Burble.Retrying
{
   public class DefaultRetryPredicate : IRetryPredicate
   {
      private readonly int _maxAttempts;

      public DefaultRetryPredicate(int maxAttempts)
      {
         _maxAttempts = maxAttempts;
      }

      public bool ShouldRetry(int retryAttempt, HttpResponseMessage response)
      {
         if (retryAttempt > _maxAttempts)
         {
            return false;
         }

         if (response == null)
         {
            return true;
         }

         return (int)response.StatusCode >= 400 && (int)response.StatusCode < 600;
      }
   }
}
