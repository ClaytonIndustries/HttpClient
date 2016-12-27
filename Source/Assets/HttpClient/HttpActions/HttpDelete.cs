using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpDelete : HttpBase
    {
        public HttpDelete(HttpWebRequest request)
        {
            _request = request;
        }

        public void Delete(Action<HttpResponseMessage<string>> responseCallback)
        {
            try
            {
                SetMethod(HttpAction.Delete);
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void Delete(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, int blockSize)
        {
            try
            {
                SetMethod(HttpAction.Delete);
                HandleByteArrayResponseRead(responseCallback, completionOption, blockSize);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }
    }
}