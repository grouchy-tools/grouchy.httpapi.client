namespace Burble.Retrying
{
   using System;
   using Burble.Abstractions;

   public class ExponentialRetryDelay : IRetryDelay
   {
      private readonly int _initialDelayMs;

      public ExponentialRetryDelay(int initialDelayMs)
      {
         _initialDelayMs = initialDelayMs;
      }

      public int DelayMs(int retryAttempt)
      {
         return _initialDelayMs * (int)Math.Pow(2, retryAttempt - 1);
      }
   }
}
