using System;
using Burble.Abstractions.Configuration;

namespace Burble.Tests.instrumenting_scenarios
{
   public class InstrumentingConfiguration : IHttpApiWithInstrumenting
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
   }
}