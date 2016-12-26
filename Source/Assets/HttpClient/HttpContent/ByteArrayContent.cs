
using System;
using System.IO;

namespace CI.HttpClient
{
    public class ByteArrayContent : IHttpContent
    {
        private readonly byte[] _content;
        private readonly string _mediaType;

        public ByteArrayContent(byte[] content, string mediaType)
        {
            _content = content;
            _mediaType = mediaType;
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