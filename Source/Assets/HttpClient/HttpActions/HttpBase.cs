using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CI.HttpClient
{
    public abstract class HttpBase
    {
        protected void HandleRequestWrite<T>(Action<HttpResponseMessage<T>> responseCallback, HttpWebRequest request, IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback,
            int blockSize)
        {
            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    if(content.ContentReadAction == ContentReadAction.Multi)
                    {
                        WriteMultipleContent(stream, content, uploadStatusCallback, blockSize);
                    }
                    else
                    {
                        WriteSingleContent(stream, content, uploadStatusCallback, blockSize, content.GetContentLength(), 0);
                    }
                }
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e, request, null);
            }
        }

        private void WriteMultipleContent(Stream stream, IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize)
        {
            long contentLength = content.GetContentLength();
            long totalContentUploaded = 0;
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

                WriteSingleContent(stream, singleContent, uploadStatusCallback, blockSize, contentLength, totalContentUploaded);

                stream.Write(multipartContent.CRLFBytes, 0, multipartContent.CRLFBytes.Length);
                totalContentUploaded += multipartContent.CRLFBytes.Length;
            }

            stream.Write(multipartContent.BoundaryEndBytes, 0, multipartContent.BoundaryEndBytes.Length);
            totalContentUploaded += multipartContent.BoundaryEndBytes.Length;

            if (uploadStatusCallback != null)
            {
                uploadStatusCallback(new UploadStatusMessage()
                {
                    ContentLength = contentLength,
                    ContentUploadedThisRound = (multipartContent.CRLFBytes.Length * 2) + multipartContent.BoundaryEndBytes.Length,
                    TotalContentUploaded = totalContentUploaded
                });
            }
        }

        private void WriteSingleContent(Stream stream, IHttpContent content, Action<UploadStatusMessage> uploadStatusCallback, int blockSize, long overallContentLength, long totalContentUploadedOverall)
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
                    else
                    {
                        stream.Close();
                    }
                }
                else
                {
                    contentUploadedThisRound = blockSize > (requestBuffer.Length - totalContentUploaded) ? (requestBuffer.Length - totalContentUploaded) : blockSize;

                    stream.Write(requestBuffer, totalContentUploaded, contentUploadedThisRound);
                }

                totalContentUploaded += contentUploadedThisRound;
                totalContentUploadedOverall += contentUploadedThisRound;

                if (uploadStatusCallback != null)
                {
                    uploadStatusCallback(new UploadStatusMessage()
                    {
                        ContentLength = overallContentLength,
                        ContentUploadedThisRound = contentUploadedThisRound,
                        TotalContentUploaded = totalContentUploadedOverall
                    });
                }
            }
        }


        protected void HandleStringResponseRead(Action<HttpResponseMessage<string>> responseCallback, HttpWebRequest request)
        {
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    if (responseCallback == null)
                    {
                        return;
                    }

                    responseCallback(new HttpResponseMessage<string>()
                    {
                        OriginalRequest = request,
                        OriginalResponse = response,
                        Data = streamReader.ReadToEnd(),
                        ContentLength = response.ContentLength,
                        ContentReadThisRound = response.ContentLength,
                        TotalContentRead = response.ContentLength,
                        StatusCode = response.StatusCode,
                        ReasonPhrase = response.StatusDescription
                    });
                }
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e, request, response);
            }
        }

        protected void HandleByteArrayResponseRead(Action<HttpResponseMessage<byte[]>> responseCallback, HttpCompletionOption completionOption, HttpWebRequest request, int blockSize)
        {
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    if (responseCallback != null)
                    {
                        return;
                    }

                    long totalContentRead = 0;
                    int contentReadThisRound = 0;

                    int readThisLoop = 0;
                    List<byte> allContent = new List<byte>();

                    do
                    {
                        byte[] buffer = new byte[blockSize];

                        readThisLoop = stream.Read(buffer, contentReadThisRound, blockSize - contentReadThisRound);

                        if(completionOption == HttpCompletionOption.AllResponseContent)
                        {
                            allContent.AddRange(buffer);
                        }

                        contentReadThisRound += readThisLoop;

                        if ((completionOption == HttpCompletionOption.StreamResponseContent && contentReadThisRound == blockSize) || readThisLoop == 0)
                        {
                            totalContentRead += contentReadThisRound;

                            responseCallback(new HttpResponseMessage<byte[]>()
                            {
                                OriginalRequest = request,
                                OriginalResponse = response,
                                Data = completionOption == HttpCompletionOption.AllResponseContent ? allContent.ToArray() : buffer,
                                ContentLength = response.ContentLength,
                                ContentReadThisRound = contentReadThisRound,
                                TotalContentRead = totalContentRead,
                                StatusCode = response.StatusCode,
                                ReasonPhrase = response.StatusDescription
                            });

                            contentReadThisRound = 0;
                        }
                    } while (readThisLoop > 0);
                }
            }
            catch (Exception e)
            {
                RaiseErrorResponse(responseCallback, e, request, response);
            }
        }

        private void RaiseErrorResponse<T>(Action<HttpResponseMessage<T>> action, Exception exception, HttpWebRequest request, HttpWebResponse response)
        {
            if (action != null)
            {
                action(new HttpResponseMessage<T>()
                {
                    OriginalRequest = request,
                    OriginalResponse = response,
                    Exception = exception,
                    StatusCode = GetStatusCode(exception, response),
                    ReasonPhrase = GetReasonPhrase(exception, response)
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