using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPut : HttpBase
    {
        public void Put(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, HttpWebRequest request, int uploadBlockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Put.ToString().ToUpper();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Put(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            HttpWebRequest request, int downloadBlockSize, int uploadBlockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Put.ToString();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, downloadBlockSize);
        }
    }
}