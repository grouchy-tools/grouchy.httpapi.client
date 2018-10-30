using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;
using Burble.CircuitBreaking;
using NUnit.Framework;
using Shouldly;
using Cba = Burble.Abstractions.CircuitBreaking.CircuitBreakerAnalysis;

namespace Burble.Tests.circuit_breaking_scenarios.state_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class progressive_shutdown_tests
   {
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip, Cba.Trip }, 5)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip, Cba.Trip, Cba.Trip }, 0)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Hold }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip, Cba.Reset }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip, Cba.Hold }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Trip, Cba.Trip, Cba.Reset, Cba.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Reset, Cba.Trip }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Reset, Cba.Trip, Cba.Trip }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Reset, Cba.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Cba.Reset, Cba.Reset, Cba.Trip }, 50)]
      [TestCase(new int[] { }, new[] { Cba.Trip }, 0)]
      [TestCase(new int[] { }, new[] { Cba.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Cba.Hold }, 100)]
      [TestCase(new int[] { }, new[] { Cba.Trip, Cba.Trip, Cba.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Cba.Trip, Cba.Reset, Cba.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Cba.Reset, Cba.Trip }, 0)]
      [TestCase(new int[] { }, new[] { Cba.Reset, Cba.Reset }, 100)]
      public async Task state_should_be_closed_appropriately(int[] openingRates, CircuitBreakerAnalysis[] analyseResponses, double expectedClosedPct)
      {
         var configuration = new CircuitBreakingConfiguration
         {
            MonitoringPeriodMs = 10,
            ProgressiveOpeningRates = openingRates,
            Analyser = new StubCircuitBreakerAnalyser(analyseResponses)
         };

         var state = await RunScenario(configuration);
         
         state.ClosedPct.ShouldBe(expectedClosedPct);
      }
      
      private async Task<CircuitBreakingState<HttpStatusCode>> RunScenario(IHttpApiWithCircuitBreaking configuration)
      {
         var testSubject = new CircuitBreakingState<HttpStatusCode>(configuration);
         var monitorTask = Task.Run(() => testSubject.MonitorAsync(CancellationToken.None));

         // Wait long enough for the monitor task to throw an exception, due to no more analyser responses
         await Task.Delay(100);

         try
         {
            await monitorTask;
         }
         catch (InvalidOperationException)
         {
         }

         return testSubject;
      }
   }
}