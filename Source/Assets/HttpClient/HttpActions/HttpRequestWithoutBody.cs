using System;
using System.Net;

namespace CI.HttpClient.Core
{
    public class HttpRequestWithoutBody : HttpBase
    {
        public HttpRequestWithoutBody(HttpAction httpAction, HttpWebRequest request, IDispatcher dispatcher)
        {
            _request = request;
            _dispatcher = dispatcher;

            SetMethod(httpAction);
        }

        public void Execute(Action<HttpResponseMessage<string>> responseCallback)
        {
            try
            {
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void Execute(HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, int blockSize)
        {
            try
            {
                HandleByteArrayResponseRead(responseCallback, completionOption, blockSize);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }
    }
}