using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPatch : HttpBase
    {
        public void Patch(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, HttpWebRequest request, int uploadBlockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Patch.ToString().ToUpper();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Patch(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            HttpWebRequest request, int downloadBlockSize, int uploadBlockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Patch.ToString();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, uploadBlockSize);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, downloadBlockSize);
        }
    }
}