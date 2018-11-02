using System;
using System.Threading;
using CI.HttpClient;
using CI.HttpClient.Helpers;
using CI.TestRunner;

namespace Assets.Tests
{
    [TestFixture]
    public class AuthTests
    {
        private const int TIMEOUT_IN_MILLISECONDS = 5000;

        private HttpClient _sut;
        private ManualResetEvent _manualResetEvent;

        [Setup]
        public void Setup()
        {
            _sut = new HttpClient();
            _sut.SetDispatcher<TestDispatcher>();

            _manualResetEvent = new ManualResetEvent(false);
        }

        [Test]
        public void HttpClient_BasicAuth()
        {
            // Arrange
            const string username = "username";
            const string password = "password";

            HttpResponseMessage response = null;

            _sut.Headers.Add("Authorization", AuthHelper.CreateBasicAuthHeader(username, password));

            // Act
            _sut.Get(new Uri(string.Format("https://httpbin.org/basic-auth/{0}/{1}", username, password)), HttpCompletionOption.AllResponseContent, r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public void HttpClient_Bearer()
        {
            // Arrange
            const string token = "1234567890";

            HttpResponseMessage response = null;

            _sut.Headers.Add("Authorization", AuthHelper.CreateOAuth2Header(token));

            // Act
            _sut.Get(new Uri(string.Format("https://httpbin.org/bearer")), HttpCompletionOption.AllResponseContent, r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}