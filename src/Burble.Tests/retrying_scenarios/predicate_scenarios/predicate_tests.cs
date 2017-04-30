namespace Burble.Tests.retrying_scenarios.predicate_scenarios
{
   using System.Net;
   using System.Net.Http;
   using Burble.Retrying;
   using Xunit;
   using Shouldly;

   public class predicate_tests
   {
      [Theory]
      [InlineData(HttpStatusCode.Continue)]
      [InlineData(HttpStatusCode.SwitchingProtocols)]
      [InlineData(199)]
      public void should_return_false_for_informational_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      [Theory]
      [InlineData(HttpStatusCode.OK)]
      [InlineData(HttpStatusCode.Accepted)]
      [InlineData(299)]
      public void should_return_false_for_success_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      [Theory]
      [InlineData(HttpStatusCode.Ambiguous)]
      [InlineData(HttpStatusCode.Moved)]
      [InlineData(399)]
      public void should_return_false_for_redirection_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      [Theory]
      [InlineData(HttpStatusCode.BadRequest)]
      [InlineData(HttpStatusCode.Unauthorized)]
      [InlineData(499)]
      public void should_return_true_for_client_error_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(true);
      }

      [Theory]
      [InlineData(HttpStatusCode.InternalServerError)]
      [InlineData(HttpStatusCode.NotImplemented)]
      [InlineData(599)]
      public void should_return_true_for_server_error_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(true);
      }

      [Theory]
      [InlineData(0)]
      [InlineData(99)]
      [InlineData(600)]
      [InlineData(999)]
      public void should_return_false_for_other_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }
      
      [Theory]
      [InlineData(1, 1, true)]
      [InlineData(2, 2, true)]
      [InlineData(2, 1, false)]
      [InlineData(3, 1, false)]
      [InlineData(1, 2, true)]
      [InlineData(1, 3, true)]
      public void should_return_false_after_max_attempts(int attempt, int maxAttempts, bool expectedResult)
      {
         var testSubject = new DefaultRetryPredicate(maxAttempts);
         var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

         testSubject.ShouldRetry(attempt, response).ShouldBe(expectedResult);
      }

      [Theory]
      [InlineData(2, 3, true)]
      [InlineData(2, 2, true)]
      [InlineData(2, 1, false)]
      public void should_return_true_if_response_not_specified(int attempt, int maxAttempts, bool expectedResult)
      {
         var testSubject = new DefaultRetryPredicate(maxAttempts);

         testSubject.ShouldRetry(attempt, null).ShouldBe(expectedResult);
      }
   }
}
