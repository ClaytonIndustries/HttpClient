using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpDelete : HttpBase
    {
        public void Delete(Action<HttpResponseMessage<string>> responseCallback, HttpWebRequest request)
        {
            SetMethod(request, HttpAction.Delete, responseCallback);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Delete(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, HttpWebRequest request, int blockSize)
        {
            SetMethod(request, HttpAction.Delete, responseCallback);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}