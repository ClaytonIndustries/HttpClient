using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpGet : HttpBase
    {
        public void GetString(Action<HttpResponseMessage<string>> responseCallback, HttpWebRequest request)
        {
            request.Method = HttpAction.Get.ToString().ToUpper();           
            HandleStringResponseRead(responseCallback, request);
        }

        public void GetByteArray(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, HttpWebRequest request, int blockSize)
        {
            request.Method = HttpAction.Get.ToString();
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}