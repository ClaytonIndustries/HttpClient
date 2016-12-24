using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpDelete : HttpBase
    {
        public void Delete(Action<HttpResponseMessage<string>> responseCallback, HttpWebRequest request)
        {
            request.Method = HttpAction.Delete.ToString().ToUpper();
            HandleStringResponseRead(responseCallback, request);
        }

        public void Delete(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, HttpWebRequest request, int blockSize)
        {
            request.Method = HttpAction.Delete.ToString();
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}