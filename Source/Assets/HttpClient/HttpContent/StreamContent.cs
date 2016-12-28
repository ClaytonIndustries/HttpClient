using System;
using System.IO;

namespace CI.HttpClient
{
    public class StreamContent : IHttpContent
    {
        private readonly Stream _stream;
        private readonly string _mediaType;

        public ContentReadAction ContentReadAction
        {
            get { return ContentReadAction.Stream; }
        }

        /// <summary>
        /// Send content based on a stream, the specified mediaType sets the Content Type header
        /// </summary>
        /// <param name="stream">The stream that identifies the content</param>
        /// <param name="mediaType">The media type</param>
        public StreamContent(Stream stream, string mediaType)
        {
            _stream = stream;
            _mediaType = mediaType;
        }

        public long GetContentLength()
        {
            return _stream.Length;
        }

        public string GetContentType()
        {
            return _mediaType;
        }

        public byte[] ReadAsByteArray()
        {
            throw new NotImplementedException();
        }

        public Stream ReadAsStream()
        {
            return _stream;
        }
    }
}