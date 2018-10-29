using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.CircuitBreaking;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.circuit_breaking_scenarios.state_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class errors_then_successes
   {
      private IList<double> _closedPcts;
      private IList<bool> _shouldAccepts;
      private CircuitBreakingState<HttpStatusCode> _testSubject;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var cancellationTokenSource = new CancellationTokenSource();
         _testSubject = new CircuitBreakingState<HttpStatusCode>(new CircuitBreakingConfiguration { MonitoringPeriodMs = 100, Analyser = new HttpApiCircuitBreakerAnalyser() });
         var monitorTask = Task.Run(() => _testSubject.MonitorAsync(cancellationTokenSource.Token));

         _closedPcts = new List<double>();
         _shouldAccepts = new List<bool>();

         for (var i = 0; i < 100; i++)
         {
            _closedPcts.Add(_testSubject.ClosedPct);
            _shouldAccepts.Add(_testSubject.ShouldAccept());
            _testSubject.LogFailureResponse(HttpStatusCode.InternalServerError);
            await Task.Delay(5);
         }

         for (var i = 0; i < 100; i++)
         {
            _closedPcts.Add(_testSubject.ClosedPct);
            _shouldAccepts.Add(_testSubject.ShouldAccept());
            _testSubject.LogSuccessResponse(HttpStatusCode.OK);
            await Task.Delay(5);
         }

         cancellationTokenSource.Cancel();
         await monitorTask;
      }

      [Test]
      public void state_should_start_fully_closed()
      {
         Assert.That(_closedPcts.First(), Is.EqualTo(100));
      }

      [Test]
      public void state_should_end_fully_closed()
      {
         Assert.That(_closedPcts.Last(), Is.EqualTo(100));
      }

      [Test]
      public void should_start_by_accepting_requests()
      {
         _shouldAccepts.First().ShouldBe(true);
      }

      [Test]
      public void should_end_by_accepting_requests()
      {
         _shouldAccepts.Last().ShouldBe(true);
      }

      [Test]
      public void fully_opened_should_be_reached()
      {
         Assert.That(_closedPcts.Any(c => c.Equals(0)), Is.True);
      }
   }
}