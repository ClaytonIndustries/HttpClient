using System;
using System.IO;

namespace CI.HttpClient
{
    public class StreamContent : IHttpContent
    {
        private readonly Stream _stream;
        private readonly string _mediaType;

        public StreamContent(Stream stream, string mediaType)
        {
            _stream = stream;
            _mediaType = mediaType;
        }

        public int GetContentLength()
        {
            return (int)_stream.Length;
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