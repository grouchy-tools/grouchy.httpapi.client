using System;
using System.Collections.Generic;
using Grouchy.Abstractions;

namespace Grouchy.HttpApi.Client.Tests.Stubs
{
   public class StubGuidGenerator : IGenerateGuids
   {
      private readonly Queue<Guid> _guids;

      public StubGuidGenerator(params Guid[] guids)
      {
         _guids = new Queue<Guid>(guids);
      }

      public Guid Generate()
      {
         return _guids.Dequeue();
      }

      public void Add(params Guid[] guids)
      {
         foreach (var guid in guids)
         {
            _guids.Enqueue(guid);
         }
      }
   }
}