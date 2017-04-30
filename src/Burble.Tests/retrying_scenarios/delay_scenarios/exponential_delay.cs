namespace Burble.Tests.retrying_scenarios.delay_scenarios
{
   using Burble.Retrying;
   using Xunit;
   using Shouldly;

   public class exponential_delay
   {
      [Theory]
      [InlineData(2, 2, 1)]
      [InlineData(2, 4, 2)]
      [InlineData(2, 8, 3)]
      [InlineData(2, 16, 4)]
      [InlineData(2, 1024, 10)]
      [InlineData(100, 100, 1)]
      [InlineData(100, 200, 2)]
      [InlineData(100, 400, 3)]
      [InlineData(100, 800, 4)]
      public void returns_increasing_larger_delay(int initialDelayMs, int expectedDelayMs, int retryAttempt)
      {
         var testSubject = new ExponentialRetryDelay(initialDelayMs);

         testSubject.DelayMs(retryAttempt).ShouldBe(expectedDelayMs);
      }
   }
}
