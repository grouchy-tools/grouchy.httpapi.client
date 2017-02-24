namespace Burble.Tests.event_callback_scenarios
{
   using System;
   using System.Net;
   using System.Net.Http;
   using Burble.Abstractions;
   using Burble.EventCallbacks;
   using Burble.Events;
   using NUnit.Framework;
   using Shouldly;

   public class json_client_event_callback
   {
      private readonly Uri _baseAddress;
      private readonly HttpRequestMessage _request;
      private readonly HttpResponseMessage _response;
      private readonly IHttpClientEventCallback _testSubject;

      private string _json;

      public json_client_event_callback()
      {
         _baseAddress = new Uri("http://localhost:8080");
         _request = new HttpRequestMessage(HttpMethod.Get, "/ping");
         _response = new HttpResponseMessage(HttpStatusCode.Accepted) { RequestMessage = _request };

         Action<string> jsonCallback = c => { _json = c; };

         _testSubject = new JsonHttpClientEventCallback(jsonCallback);
      }

      [Test]
      public void serialise_client_request_initiated()
      {
         var clientRequest = HttpClientRequestInitiated.Create(_request, _baseAddress);
         clientRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(clientRequest);

         _json.ShouldBe("{\"eventType\":\"HttpClientRequestInitiated\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"http://localhost:8080/ping\",\"method\":\"GET\"}");
      }

      [Test]
      public void serialise_client_request_initiated_with_tag()
      {
         var clientRequest = HttpClientRequestInitiated.Create(_request, _baseAddress);
         clientRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);
         clientRequest.Tags.Add("key", "value");

         _testSubject.Invoke(clientRequest);

         _json.ShouldBe("{\"eventType\":\"HttpClientRequestInitiated\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"http://localhost:8080/ping\",\"method\":\"GET\",\"tags\":{\"key\":\"value\"}}");
      }

      [Test]
      public void serialise_client_response_received()
      {
         var clientRequest = HttpClientResponseReceived.Create(_response, _baseAddress, 7);
         clientRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(clientRequest);

         _json.ShouldBe("{\"eventType\":\"HttpClientResponseReceived\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"http://localhost:8080/ping\",\"method\":\"GET\",\"durationMs\":7,\"statusCode\":202}");
      }

      [Test]
      public void serialise_client_response_received_with_tag()
      {
         var clientRequest = HttpClientResponseReceived.Create(_response, _baseAddress, 7);
         clientRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);
         clientRequest.Tags.Add("key", "value");

         _testSubject.Invoke(clientRequest);

         _json.ShouldBe("{\"eventType\":\"HttpClientResponseReceived\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"http://localhost:8080/ping\",\"method\":\"GET\",\"tags\":{\"key\":\"value\"},\"durationMs\":7,\"statusCode\":202}");
      }
   }
}
