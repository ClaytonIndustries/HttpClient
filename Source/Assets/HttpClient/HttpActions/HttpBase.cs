using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CI.HttpClient.Core
{
    public abstract class HttpBase
    {
        protected HttpWebRequest _request;
        protected HttpWebResponse _response;
        protected IDispatcher _dispatcher;

        protected void SetMethod(HttpAction httpAction)
        {
            _request.Method = httpAction.ToString().ToUpper();
        }

#if NETFX_CORE
        protected void SetContentHeaders(IHttpContent content)
        {
            _request.Headers[HttpRequestHeader.ContentLength] = content.GetContentLength().ToString();
            _request.ContentType = content.GetContentType();
        }
#else
        protected void SetContentHeaders(IHttpContent content)
        {
            _request.ContentLength = content.GetContentLength();
            _request.ContentType = content.GetContentType();
        }
#endif

#if NETFX_CORE
        protected void HandleRequestWrite(IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize)
        {
            using (Stream stream = _request.GetRequestStreamAsync().Result)
            {
                if (content.ContentReadAction == ContentReadAction.Multi)
                {
                    WriteMultipleContent(stream, content, uploadStatusCallback, blockSize);
                }
                else
                {
                    WriteSingleContent(stream, content, uploadStatusCallback, blockSize, content.GetContentLength(), 0);
                }
            }
        }
#else
        protected void HandleRequestWrite(IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize)
        {
            using (Stream stream = _request.GetRequestStream())
            {
                if (content.ContentReadAction == ContentReadAction.Multi)
                {
                    WriteMultipleContent(stream, content, uploadStatusCallback, blockSize);
                }
                else
                {
                    WriteSingleContent(stream, content, uploadStatusCallback, blockSize, content.GetContentLength(), 0);
                }
            }
        }
#endif

        private void WriteMultipleContent(Stream stream, IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize)
        {
            long contentLength = content.GetContentLength();
            long totalContentUploaded = 0;
            int singleContentCount = 0;
            MultipartContent multipartContent = content as MultipartContent;

            foreach (IHttpContent singleContent in multipartContent)
            {
                byte[] contentHeader = Encoding.UTF8.GetBytes("Content-Type: " + singleContent.GetContentType());

                stream.Write(multipartContent.BoundaryStartBytes, 0, multipartContent.BoundaryStartBytes.Length);
                totalContentUploaded += multipartContent.BoundaryStartBytes.Length;

                stream.Write(contentHeader, 0, contentHeader.Length);
                totalContentUploaded += contentHeader.Length;

                stream.Write(multipartContent.CRLFBytes, 0, multipartContent.CRLFBytes.Length);
                totalContentUploaded += multipartContent.CRLFBytes.Length;
                stream.Write(multipartContent.CRLFBytes, 0, multipartContent.CRLFBytes.Length);
                totalContentUploaded += multipartContent.CRLFBytes.Length;

                totalContentUploaded += WriteSingleContent(stream, singleContent, uploadStatusCallback, blockSize, contentLength, totalContentUploaded);

                stream.Write(multipartContent.CRLFBytes, 0, multipartContent.CRLFBytes.Length);
                totalContentUploaded += multipartContent.CRLFBytes.Length;

                singleContentCount++;
            }

            if(singleContentCount == 0)
            {
                stream.Write(multipartContent.BoundaryStartBytes, 0, multipartContent.BoundaryStartBytes.Length);
                totalContentUploaded += multipartContent.BoundaryStartBytes.Length;
            }

            stream.Write(multipartContent.BoundaryEndBytes, 0, multipartContent.BoundaryEndBytes.Length);
            totalContentUploaded += multipartContent.BoundaryEndBytes.Length;

            RaiseUploadStatusCallback(uploadStatusCallback, contentLength, (multipartContent.CRLFBytes.Length * 2) + multipartContent.BoundaryEndBytes.Length, totalContentUploaded);
        }

        private long WriteSingleContent(Stream stream, IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize, long overallContentLength, long totalContentUploadedOverall)
        {
            long contentLength = content.GetContentLength();
            int contentUploadedThisRound = 0;
            int totalContentUploaded = 0;
            byte[] requestBuffer = null;
            Stream contentStream = null;

            if (content.ContentReadAction == ContentReadAction.Stream)
            {
                requestBuffer = new byte[blockSize];
                contentStream = content.ReadAsStream();
            }
            else
            {
                requestBuffer = content.ReadAsByteArray();
            }

            while (totalContentUploaded != contentLength)
            {
                contentUploadedThisRound = 0;

                if (content.ContentReadAction == ContentReadAction.Stream)
                {
                    int read = 0;
                    while ((read = contentStream.Read(requestBuffer, read, blockSize - read)) > 0)
                    {
                        contentUploadedThisRound += read;
                    }

                    if (contentUploadedThisRound > 0)
                    {
                        stream.Write(requestBuffer, 0, contentUploadedThisRound);
                    }
                }
                else
                {
                    contentUploadedThisRound = blockSize > (requestBuffer.Length - totalContentUploaded) ? (requestBuffer.Length - totalContentUploaded) : blockSize;

                    stream.Write(requestBuffer, totalContentUploaded, contentUploadedThisRound);
                }

                totalContentUploaded += contentUploadedThisRound;
                totalContentUploadedOverall += contentUploadedThisRound;

                RaiseUploadStatusCallback(uploadStatusCallback, overallContentLength, contentUploadedThisRound, totalContentUploadedOverall);
            }

            return totalContentUploaded;
        }

        private void RaiseUploadStatusCallback(Action<UploadStatusMessage> uploadStatusCallback, long contentLength, long contentUploadedThisRound, long totalContentUploaded)
        {
            if (uploadStatusCallback != null)
            {
                _dispatcher.Enqueue(() =>
                {
                    uploadStatusCallback(new UploadStatusMessage()
                    {
                        ContentLength = contentLength,
                        ContentUploadedThisRound = contentUploadedThisRound,
                        TotalContentUploaded = totalContentUploaded
                    });
                });
            }
        }

#if NETFX_CORE
        protected void HandleStringResponseRead(Action<HttpResponseMessage<string>> responseCallback)
        {
            HttpWebResponse response = (HttpWebResponse)_request.GetResponseAsync().Result;

            _response = response;

            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                if (responseCallback == null)
                {
                    return;
                }

                RaiseResponseCallback(responseCallback, streamReader.ReadToEnd(), response.ContentLength, response.ContentLength);
            }
        }
#else
        protected void HandleStringResponseRead(Action<HttpResponseMessage<string>> responseCallback)
        {
            HttpWebResponse response = (HttpWebResponse)_request.GetResponse();

            _response = response;

            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                if (responseCallback == null)
                {
                    return;
                }

                RaiseResponseCallback(responseCallback, streamReader.ReadToEnd(), response.ContentLength, response.ContentLength);
            }
        }
#endif


#if NETFX_CORE
        protected void HandleByteArrayResponseRead(Action<HttpResponseMessage<byte[]>> responseCallback, HttpCompletionOption completionOption, int blockSize)
        {
            HttpWebResponse response = (HttpWebResponse)_request.GetResponseAsync().Result;

            _response = response;

            using (Stream stream = response.GetResponseStream())
            {
                if (responseCallback == null)
                {
                    return;
                }

                long totalContentRead = 0;
                int contentReadThisRound = 0;

                int readThisLoop = 0;
                List<byte> allContent = new List<byte>();
                byte[] buffer = new byte[blockSize];

                do
                {
                    readThisLoop = stream.Read(buffer, contentReadThisRound, blockSize - contentReadThisRound);

                    contentReadThisRound += readThisLoop;

                    if (contentReadThisRound == blockSize || readThisLoop == 0)
                    {
                        totalContentRead += contentReadThisRound;

                        byte[] responseData = null;

                        if (buffer.Length > contentReadThisRound)
                        {
                            responseData = new byte[contentReadThisRound];

                            Array.Copy(buffer, responseData, contentReadThisRound);
                        }
                        else
                        {
                            responseData = buffer;
                        }

                        if (completionOption == HttpCompletionOption.AllResponseContent)
                        {
                            allContent.AddRange(responseData);
                        }

                        if (completionOption == HttpCompletionOption.StreamResponseContent || readThisLoop == 0)
                        {
                            RaiseResponseCallback(responseCallback, completionOption == HttpCompletionOption.AllResponseContent ? allContent.ToArray() : responseData,
                                completionOption == HttpCompletionOption.AllResponseContent ? totalContentRead : contentReadThisRound, totalContentRead);
                        }

                        contentReadThisRound = 0;
                    }
                } while (readThisLoop > 0);
            }
        }
#else
        protected void HandleByteArrayResponseRead(Action<HttpResponseMessage<byte[]>> responseCallback, HttpCompletionOption completionOption, int blockSize)
        {
            HttpWebResponse response = (HttpWebResponse)_request.GetResponse();

            _response = response;

            using (Stream stream = response.GetResponseStream())
            {
                if (responseCallback == null)
                {
                    return;
                }

                long totalContentRead = 0;
                int contentReadThisRound = 0;

                int readThisLoop = 0;
                List<byte> allContent = new List<byte>();
                byte[] buffer = new byte[blockSize];

                do
                {
                    readThisLoop = stream.Read(buffer, contentReadThisRound, blockSize - contentReadThisRound);

                    contentReadThisRound += readThisLoop;

                    if (contentReadThisRound == blockSize || readThisLoop == 0)
                    {
                        totalContentRead += contentReadThisRound;

                        byte[] responseData = new byte[contentReadThisRound];

                        Array.Copy(buffer, responseData, contentReadThisRound);

                        if (completionOption == HttpCompletionOption.AllResponseContent)
                        {
                            allContent.AddRange(responseData);
                        }

                        if (completionOption == HttpCompletionOption.StreamResponseContent || readThisLoop == 0)
                        {
                            RaiseResponseCallback(responseCallback, completionOption == HttpCompletionOption.AllResponseContent ? allContent.ToArray() : responseData,
                                completionOption == HttpCompletionOption.AllResponseContent ? totalContentRead : contentReadThisRound, totalContentRead);
                        }

                        contentReadThisRound = 0;
                    }
                } while (readThisLoop > 0);
            }
        }
#endif

        private void RaiseResponseCallback<T>(Action<HttpResponseMessage<T>> responseCallback, T data, long contentReadThisRound, long totalContentRead)
        {
            _dispatcher.Enqueue(() =>
            {
                responseCallback(new HttpResponseMessage<T>()
                {
                    OriginalRequest = _request,
                    OriginalResponse = _response,
                    Data = data,
                    ContentLength = _response.ContentLength,
                    ContentReadThisRound = contentReadThisRound,
                    TotalContentRead = totalContentRead,
                    StatusCode = _response.StatusCode,
                    ReasonPhrase = _response.StatusDescription
                });
            });
        }

        protected void RaiseErrorResponse<T>(Action<HttpResponseMessage<T>> action, Exception exception)
        {
            if (action != null)
            {
                _dispatcher.Enqueue(() =>
                {
                    action(new HttpResponseMessage<T>()
                    {
                        OriginalRequest = _request,
                        OriginalResponse = _response,
                        Exception = exception,
                        StatusCode = GetStatusCode(exception, _response),
                        ReasonPhrase = GetReasonPhrase(exception, _response)
                    });
                });
            }
        }

        private HttpStatusCode GetStatusCode(Exception exception, HttpWebResponse response)
        {
            if(response != null)
            {
                return response.StatusCode;
            }

            if(exception.Message.Contains("The remote server returned an error:"))
            {
                int statusCode = 0;

                Match match = Regex.Match(exception.Message, "\\(([0-9]+)\\)");

                if (match.Groups.Count == 2 && int.TryParse(match.Groups[1].Value, out statusCode))
                {
                    return (HttpStatusCode)statusCode;
                }
            }

            return HttpStatusCode.InternalServerError;
        }

        private string GetReasonPhrase(Exception exception, HttpWebResponse response)
        {
            if (response != null)
            {
                return response.StatusDescription;
            }

            if (exception.Message.Contains("The remote server returned an error:"))
            {
                Match match = Regex.Match(exception.Message, "\\([0-9]+\\) (.+)");

                if (match.Groups.Count == 2)
                {
                    return match.Groups[1].Value;
                }
            }

            return "Unknown";
        }
    }
}