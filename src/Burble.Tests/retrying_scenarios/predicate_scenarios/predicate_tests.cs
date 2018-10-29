using System.Net;
using System.Net.Http;
using Burble.Retrying;
using NUnit.Framework;
using Shouldly;

namespace Burble.Tests.retrying_scenarios.predicate_scenarios
{
   // ReSharper disable once InconsistentNaming
   public class predicate_tests
   {
      [TestCase(HttpStatusCode.Continue)]
      [TestCase(HttpStatusCode.SwitchingProtocols)]
      [TestCase(199)]
      public void should_return_false_for_informational_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      
      [TestCase(HttpStatusCode.OK)]
      [TestCase(HttpStatusCode.Accepted)]
      [TestCase(299)]
      public void should_return_false_for_success_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      
      [TestCase(HttpStatusCode.Ambiguous)]
      [TestCase(HttpStatusCode.Moved)]
      [TestCase(399)]
      public void should_return_false_for_redirection_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }

      
      [TestCase(HttpStatusCode.BadRequest)]
      [TestCase(HttpStatusCode.Unauthorized)]
      [TestCase(499)]
      public void should_return_true_for_client_error_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(true);
      }

      
      [TestCase(HttpStatusCode.InternalServerError)]
      [TestCase(HttpStatusCode.NotImplemented)]
      [TestCase(599)]
      public void should_return_true_for_server_error_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(true);
      }

      
      [TestCase(0)]
      [TestCase(99)]
      [TestCase(600)]
      [TestCase(999)]
      public void should_return_false_for_other_status_codes(HttpStatusCode statusCode)
      {
         var testSubject = new DefaultRetryPredicate(1);
         var response = new HttpResponseMessage(statusCode);

         testSubject.ShouldRetry(1, response).ShouldBe(false);
      }
      
      
      [TestCase(1, 1, true)]
      [TestCase(2, 2, true)]
      [TestCase(2, 1, false)]
      [TestCase(3, 1, false)]
      [TestCase(1, 2, true)]
      [TestCase(1, 3, true)]
      public void should_return_false_after_max_attempts(int attempt, int maxAttempts, bool expectedResult)
      {
         var testSubject = new DefaultRetryPredicate(maxAttempts);
         var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

         testSubject.ShouldRetry(attempt, response).ShouldBe(expectedResult);
      }

      
      [TestCase(2, 3, true)]
      [TestCase(2, 2, true)]
      [TestCase(2, 1, false)]
      public void should_return_true_if_response_not_specified(int attempt, int maxAttempts, bool expectedResult)
      {
         var testSubject = new DefaultRetryPredicate(maxAttempts);

         testSubject.ShouldRetry(attempt, null).ShouldBe(expectedResult);
      }
   }
}
