using System.Linq;
using System.Net;
using Burble.CircuitBreaking;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.circuit_breaking_scenarios.state_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class state_tests
   {
      [Test]
      public void circuit_should_be_accepting_all_requests_on_creation()
      {
         var state = new CircuitBreakingState<HttpStatusCode>(new CircuitBreakingConfiguration());

         var accepts = Enumerable.Range(0, 100).Select(c => state.ShouldAccept()).ToArray();
         
         Assert.That(accepts.All(c => c), Is.True);
      }
      
      [Test]
      public void circuit_should_be_100_pct_closed_on_creation()
      {
         var state = new CircuitBreakingState<HttpStatusCode>(new CircuitBreakingConfiguration());
         
         state.ClosedPct.ShouldBe(100);
      }
   }
}