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

        public int DownloadBlockSize { get; set; }
        public int UploadBlockSize { get; set; }
        public int Timeout { get; set; }
        public int ReadWriteTimoeut { get; set; }
        public RequestCachePolicy Cache { get; set; }
        public ICredentials Credentials { get; set; }
        public IDictionary<HttpRequestHeader, string> Headers { get; set; }
        public IWebProxy Proxy { get; set; }

        private readonly List<HttpWebRequest> _requests;
        private readonly object _lock;

        public HttpClient()
        {
            DownloadBlockSize = DEFAULT_BLOCK_SIZE;
            UploadBlockSize = DEFAULT_BLOCK_SIZE;
            Timeout = DEFAULT_TIMEOUT;
            ReadWriteTimoeut = DEFAULT_READ_WRITE_TIMEOUT;
            _requests = new List<HttpWebRequest>();
            _lock = new object();
        }

        public void Abort()
        {
            lock(_lock)
            {
                foreach(HttpWebRequest request in _requests)
                {
                    request.Abort();
                }
            }
        }

        public void Delete(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpDelete().Delete(responseCallback, request);
                RemoveRequest(request);
            }).Start();
        }

        public void Delete(Uri uri, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpDelete().Delete(completionOption, responseCallback, request, DownloadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void GetString(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpGet().GetString(responseCallback, request);
                RemoveRequest(request);
            }).Start();
        }

        public void GetByteArray(Uri uri, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpGet().GetByteArray(completionOption, responseCallback, request, DownloadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Patch(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPatch().Patch(content, responseCallback, uploadStatusCallback, request, UploadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Patch(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPatch().Patch(content, completionOption, responseCallback, uploadStatusCallback, request, DownloadBlockSize, UploadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Post(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPost().Post(content, responseCallback, uploadStatusCallback, request, UploadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Post(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPost().Post(content, completionOption, responseCallback, uploadStatusCallback, request, DownloadBlockSize, UploadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Put(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPut().Put(content, responseCallback, uploadStatusCallback, request, UploadBlockSize);
                RemoveRequest(request);
            }).Start();
        }

        public void Put(Uri uri, IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback,
            Action<UploadStatusMessage> uploadStatusCallback = null)
        {
            new Thread(() =>
            {
                HttpWebRequest request = CreateRequest(uri);
                new HttpPut().Put(content, completionOption, responseCallback, uploadStatusCallback, request, DownloadBlockSize, UploadBlockSize);
                RemoveRequest(request);
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
            lock(_lock)
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
    }
}