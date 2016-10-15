namespace Burble.Tests.retrying_scenarios.delay_scenarios
{
   using Burble.Retrying;
   using NUnit.Framework;
   using Shouldly;

   public class linear_delay
   {
      [TestCase(2, 2, 1)]
      [TestCase(2, 4, 2)]
      [TestCase(2, 6, 3)]
      [TestCase(2, 8, 4)]
      [TestCase(2, 20, 10)]
      [TestCase(100, 100, 1)]
      [TestCase(100, 200, 2)]
      [TestCase(100, 300, 3)]
      [TestCase(100, 400, 4)]
      public void returns_increasing_larger_delay(int initialDelayMs, int expectedDelayMs, int retryAttempt)
      {
         var testSubject = new LinearRetryDelay(initialDelayMs);

         testSubject.DelayMs(retryAttempt).ShouldBe(expectedDelayMs);
      }
   }
}