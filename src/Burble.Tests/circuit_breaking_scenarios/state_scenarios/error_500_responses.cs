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
   public class error_500_responses
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

         for (var j = 0; j <= 1; j++)
         {
            for (var i = 500; i < 600; i++)
            {
               _closedPcts.Add(_testSubject.ClosedPct);
               _shouldAccepts.Add(_testSubject.ShouldAccept());
               _testSubject.LogFailureResponse((HttpStatusCode) i);
               await Task.Delay(5);
            }
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
      public void state_should_end_fully_open()
      {
         Assert.That(_closedPcts.Last(), Is.EqualTo(0));
      }

      [Test]
      public void should_start_by_accepting_requests()
      {
         _shouldAccepts.First().ShouldBe(true);
      }

      [Test]
      public void should_end_by_not_accepting_requests()
      {
         _shouldAccepts.Last().ShouldBe(false);
      }

      [Test]
      public void state_should_progressively_get_more_open()
      {
         var groups = _closedPcts.Batch(20).ToArray();

         for (var i = 1; i < groups.Length; i++)
         {
            Assert.That(groups[i].Count(b => false), Is.GreaterThanOrEqualTo(groups[0].Count(b => false)));
         }
      }
   }
}