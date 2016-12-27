using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPost : HttpBase
    {
        public HttpPost(HttpWebRequest request)
        {
            _request = request;
        }

        public void Post(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, int uploadBlockSize)
        {
            try
            {
                SetMethod(HttpAction.Post);
                SetContentHeaders(content);

                HandleRequestWrite(content, uploadStatusCallback, uploadBlockSize);
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void Post(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            int downloadBlockSize, int uploadBlockSize)
        {
            try
            {
                SetMethod(HttpAction.Post);
                SetContentHeaders(content);

                HandleRequestWrite(content, uploadStatusCallback, uploadBlockSize);
                HandleByteArrayResponseRead(responseCallback, completionOption, downloadBlockSize);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }
    }
}