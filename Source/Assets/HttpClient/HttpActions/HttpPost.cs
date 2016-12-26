using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPost : HttpBase
    {
        public void Post(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, HttpWebRequest request, int uploadBlockSize)
        {
            SetContentHeaders(request, content, responseCallback);
            SetMethod(request, HttpAction.Post, responseCallback);

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Post(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            HttpWebRequest request, int downloadBlockSize, int uploadBlockSize)
        {
            SetContentHeaders(request, content, responseCallback);
            SetMethod(request, HttpAction.Post, responseCallback);

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, downloadBlockSize);
        }
    }
}