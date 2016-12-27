using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpGet : HttpBase
    {
        public HttpGet(HttpWebRequest request)
        {
            _request = request;
        }

        public void GetString(Action<HttpResponseMessage<string>> responseCallback)
        {
            try
            {
                SetMethod(HttpAction.Get);
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void GetByteArray(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, int blockSize)
        {
            try
            {
                SetMethod(HttpAction.Get);
                HandleByteArrayResponseRead(responseCallback, completionOption, blockSize);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }
    }
}