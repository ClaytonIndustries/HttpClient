using System;
using System.Collections.Generic;
using System.IO;

namespace CI.HttpClient
{
    public class ByteArrayContent : IHttpContent
    {
        private readonly byte[] _content;
        private readonly string _mediaType;

        /// <summary>
        /// Not currently implemented
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Send content encoded as a byte array, the specified mediaType sets the Content Type header
        /// </summary>
        /// <param name="content">The byte array to send</param>
        /// <param name="mediaType">The media type</param>
        public ByteArrayContent(byte[] content, string mediaType)
        {
            _content = content;
            _mediaType = mediaType;
            Headers = new Dictionary<string, string>();
        }

        public ContentReadAction ContentReadAction
        {
            get { return ContentReadAction.ByteArray; }
        }

        public long GetContentLength()
        {
            return _content.Length;
        }

        public string GetContentType()
        {
            return _mediaType;
        }

        public byte[] ReadAsByteArray()
        {
            return _content;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }
    }
}