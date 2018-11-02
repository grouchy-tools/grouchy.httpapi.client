using System;
using Grouchy.HttpApi.Client.Abstractions.Configuration;

namespace Grouchy.HttpApi.Client.Tests.Configuration
{
   public class RetryingConfiguration : IHttpApiWithRetrying
   {
      public string Name { get; set; }

      public Uri Uri { get; set; } = new Uri("http://exception-host");
      
      public int? TimeoutMs { get; set; }
      
      public string RetryPolicy { get; set; }
   }
}