using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Threading;

namespace CI.HttpClient
{
    public class HttpClient
    {
        private const int DEFAULT_BLOCK_SIZE = 10000;
        private const int DEFAULT_TIMEOUT = 100000;
        private const int DEFAULT_READ_WRITE_TIMEOUT = 300000;

        /// <summary>
        /// Chunk size when downloading data. Default is 10,000 bytes (10 kilobytes)
        /// </summary>
        public int DownloadBlockSize { get; set; }

        /// <summary>
        /// Chunk size when uploading data. Default is 10,000 bytes (10 kilobytes)
        /// </summary>
        public int UploadBlockSize { get; set; }

        /// <summary>
        /// Timeout value in milliseconds for opening read / write streams to the server. The default value is 100,000 milliseconds (100 seconds)
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Timeout value in milliseconds when reading or writing data to / from the server. The default value is 300,000 milliseconds (5 minutes)
        /// </summary>
        public int ReadWriteTimoeut { get; set; }

        /// <summary>
        /// The cache policy
        /// </summary>
        public RequestCachePolicy Cache { get; set; }

        /// <summary>
        /// Authentication information 
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Specifies a collection of the name/value pairs that make up the HTTP headers
        /// </summary>
        public IDictionary<HttpRequestHeader, string> Headers { get; set; }

        /// <summary>
        /// Proxy information 
        /// </summary>
        public IWebProxy Proxy { get; set; }

        private readonly List<HttpWebRequest> _requests;
        private readonly object _lock;

        /// <summary>
        /// Provides a class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI
        /// </summary>
        public HttpClient()
        {
            DownloadBlockSize = DEFAULT_BLOCK_SIZE;
            UploadBlockSize = DEFAULT_BLOCK_SIZE;
            Timeout = DEFAULT_TIMEOUT;
            ReadWriteTimoeut = DEFAULT_READ_WRITE_TIMEOUT;
            _requests = new List<HttpWebRequest>();
            _lock = new object();
        }

        /// <summary>
        /// Aborts all requests on this instance
        /// </summary>
        public void Abort()
        {
            lock (_lock)
            {
                foreach (HttpWebRequest request in _requests)
                {
                    request.Abort();
                }
            }
        }

        /// <summary>
        /// Sends a DELETE request to the specified Uri and returns the response body as a string
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        public void Delete(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpDelete(request).Delete(responseCallback);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a DELETE request to the specified Uri and returns the response body as a byte array. A completion option specifies if download progress should be reported
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="completionOption">Determines how he response should be read</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        public void Delete(Uri uri, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpDelete(request).Delete(completionOption, responseCallback, DownloadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a GET request to the specified Uri and returns the response body as a string 
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        public void GetString(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpGet(request).GetString(responseCallback);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a GET request to the specified Uri and returns the response body as a byte array. A completion option specifies if download progress should be reported
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="completionOption">Determines how he response should be read</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        public void GetByteArray(Uri uri, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpGet(request).GetByteArray(completionOption, responseCallback, DownloadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a PATCH request to the specified Uri and returns the response body as a string. An uploadStatusCallback can be specified to report upload progress
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Patch(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPatch(request).Patch(content, responseCallback, uploadStatusCallback, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a PATCH request to the specified Uri and returns the response body as a byte array. An uploadStatusCallback can be specified to report upload progress 
        /// and a completion option specifies if download progress should be reported
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="completionOption">Determines how he response should be read</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Patch(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPatch(request).Patch(content, completionOption, responseCallback, uploadStatusCallback, DownloadBlockSize, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a POST request to the specified Uri and returns the response body as a string. An uploadStatusCallback can be specified to report upload progress
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Post(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPost(request).Post(content, responseCallback, uploadStatusCallback, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a POST request to the specified Uri and returns the response body as a byte array. An uploadStatusCallback can be specified to report upload progress 
        /// and a completion option specifies if download progress should be reported
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="completionOption">Determines how he response should be read</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Post(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPost(request).Post(content, completionOption, responseCallback, uploadStatusCallback, DownloadBlockSize, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a PUT request to the specified Uri and returns the response body as a string. An uploadStatusCallback can be specified to report upload progress
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Put(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPut(request).Put(content, responseCallback, uploadStatusCallback, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        /// <summary>
        /// Sends a PUT request to the specified Uri and returns the response body as a byte array. An uploadStatusCallback can be specified to report upload progress 
        /// and a completion option specifies if download progress should be reported
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="content">Data to send</param>
        /// <param name="completionOption">Determines how he response should be read</param>
        /// <param name="responseCallback">Callback raised once the request completes</param>
        /// <param name="uploadStatusCallback">Callback that reports upload progress</param>
        public void Put(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                try
                {
                    HttpWebRequest request = CreateRequest(uri);
                    new HttpPut(request).Put(content, completionOption, responseCallback, uploadStatusCallback, DownloadBlockSize, UploadBlockSize);
                    RemoveRequest(request);
                }
                catch (Exception e)
                {
                    RaiseErrorResponse(responseCallback, e);
                }
            }).Start();
        }

        private HttpWebRequest CreateRequest(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            AddCache(request);
            AddCredentials(request);
            AddHeaders(request);
            AddProxy(request);
            AddTimeouts(request);
            AddRequest(request);
            return request;
        }

        private void AddCache(HttpWebRequest request)
        {
            if (Cache != null)
            {
                request.CachePolicy = Cache;
            }
        }

        private void AddCredentials(HttpWebRequest request)
        {
            if (Credentials != null)
            {
                request.Credentials = Credentials;
            }
        }

        private void AddHeaders(HttpWebRequest request)
        {
            if (Headers != null)
            {
                foreach (KeyValuePair<HttpRequestHeader, string> header in Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private void AddProxy(HttpWebRequest request)
        {
            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }
        }

        private void AddTimeouts(HttpWebRequest request)
        {
            request.Timeout = Timeout;
            request.ReadWriteTimeout = ReadWriteTimoeut;
        }

        private void AddRequest(HttpWebRequest request)
        {
            lock (_lock)
            {
                _requests.Add(request);
            }
        }

        private void RemoveRequest(HttpWebRequest request)
        {
            lock (_lock)
            {
                _requests.Remove(request);
            }
        }

        private void RaiseErrorResponse<T>(Action<HttpResponseMessage<T>> action, Exception exception)
        {
            if (action != null)
            {
                action(new HttpResponseMessage<T>()
                {
                    Exception = exception,
                });
            }
        }
    }
}