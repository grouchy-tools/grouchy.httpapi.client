using System;
using Grouchy.HttpApi.Client.Abstractions.Configuration;

namespace Grouchy.HttpApi.Client.Tests.Configuration
{
   public class ThrottlingConfiguration : IHttpApiWithThrottling
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }
      
      public int? TimeoutMs { get; set; }
      
      public string ThrottlePolicy { get; set; }
   }
}