using System;
using Burble.Abstractions.Configuration;

namespace Burble.Tests.adapter_scenarios
{
   public class HttpApiConfiguration : IHttpApiConfiguration
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
   }
}