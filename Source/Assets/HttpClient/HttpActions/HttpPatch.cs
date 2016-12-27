﻿using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpPatch : HttpBase
    {
        public HttpPatch(HttpWebRequest request)
        {
            _request = request;
        }

        public void Patch(IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, int uploadBlockSize)
        {
            try
            {
                SetMethod(HttpAction.Patch);
                SetContentHeaders(content);

                HandleRequestWrite(content, uploadStatusCallback, uploadBlockSize);
                HandleStringResponseRead(responseCallback);
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e);
            }
        }

        public void Patch(IHttpContent content, HttpCompletionOption completionOption, Action<HttpResponseMessage<byte[]>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback, 
            int downloadBlockSize, int uploadBlockSize)
        {
            try
            {
                SetMethod(HttpAction.Patch);
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