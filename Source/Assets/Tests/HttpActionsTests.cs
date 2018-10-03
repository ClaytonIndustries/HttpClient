using System;
using System.Threading;
using CI.HttpClient;
using CI.TestRunner;

namespace Assets.TestRunner
{
    [TestFixture]
    public class HttpActionsTests
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
        public void HttpClient_Get()
        {
            // Arrange
            HttpResponseMessage<string> response = null;

            // Act
            _sut.GetString(new Uri("https://httpbin.org/get"), r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public void HttpClient_Post()
        {
            // Arrange
            HttpResponseMessage<string> response = null;

            // Act
            _sut.Post(new Uri("https://httpbin.org/post"), new StringContent("Some Content"), r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public void HttpClient_Put()
        {
            // Arrange
            HttpResponseMessage<string> response = null;

            // Act
            _sut.Put(new Uri("https://httpbin.org/put"), new StringContent("Some Content"), r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public void HttpClient_Patch()
        {
            // Arrange
            HttpResponseMessage<string> response = null;

            // Act
            _sut.Patch(new Uri("https://httpbin.org/patch"), new StringContent("Some Content"), r =>
            {
                response = r;

                _manualResetEvent.Set();
            });

            // Assert
            Assert.IsTrue(_manualResetEvent.WaitOne(TIMEOUT_IN_MILLISECONDS));

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public void HttpClient_Delete()
        {
            // Arrange
            HttpResponseMessage<string> response = null;

            // Act
            _sut.Delete(new Uri("https://httpbin.org/delete"), r =>
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