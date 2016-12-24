using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPut : HttpBase
    {
        public void Put(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, HttpWebRequest request, int blockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Put.ToString().ToUpper();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, blockSize);
            HandleStringResponseRead(responseCallback, request);
        }

        public void Put(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback,
            HttpWebRequest request, int blockSize)
        {
            request.ContentLength = content.GetContentLength();
            request.ContentType = content.GetContentType();
            request.Method = HttpAction.Put.ToString();

            HandleRequestWrite(responseCallback, request, content, uploadStatusCallback, blockSize);
            HandleByteArrayResponseRead(responseCallback, completionOption, request, blockSize);
        }
    }
}