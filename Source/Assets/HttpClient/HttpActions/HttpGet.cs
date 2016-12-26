using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpGet : HttpBase
    {
        public void GetString(Action<HttpResponseMessage<string>> responseCallback, HttpWebRequest request)
        {
            SetMethod(request, HttpAction.Get, responseCallback);
            HandleStringResponseRead(responseCallback, request);
        }

        public void GetByteArray(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, HttpWebRequest request, int blockSize)
        {
            SetMethod(request, HttpAction.Get, responseCallback);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}