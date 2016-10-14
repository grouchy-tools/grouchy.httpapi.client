namespace Burble
{
   using System.Net.Http;
   using System.Threading.Tasks;

   public class SimpleHttpClient : IHttpClient
   {
      private readonly HttpClient _baseHttpClient;

      public SimpleHttpClient(HttpClient baseHttpClient)
      {
         _baseHttpClient = baseHttpClient;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         // Nothing to await so just return the Task
         return _baseHttpClient.SendAsync(request);
      }
   }
}