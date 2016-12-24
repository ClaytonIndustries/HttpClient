using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPost : HttpBase
    {
        public void Post(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, HttpWebRequest request, int blockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Post.ToString().ToUpper();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, blockSize);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Post(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            HttpWebRequest request, int blockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Post.ToString();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, blockSize);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}