using System;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class StringContent : IHttpContent
    {
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        private readonly string _content;
        private readonly Encoding _encoding;
        private readonly string _mediaType;

        private byte[] _serialisedContent;

        public ContentReadAction ContentReadAction
        {
            get { return ContentReadAction.ByteArray; }
        }

        public StringContent(string content)
        {
            _content = content;
            _encoding = Encoding.UTF8;
            _mediaType = DEFAULT_MEDIA_TYPE;
        }

        public StringContent(string content, Encoding encoding)
        {
            _content = content;
            _encoding = encoding;
            _mediaType = DEFAULT_MEDIA_TYPE;
        }

        public StringContent(string content, Encoding encoding, string mediaType)
        {
            _content = content;
            _encoding = encoding;
            _mediaType = mediaType;
        }

        public long GetContentLength()
        {
            return ReadAsByteArray().Length;
        }

        public string GetContentType()
        {
            return _mediaType;
        }

        public byte[] ReadAsByteArray()
        {
            if(_serialisedContent == null)
            {
                _serialisedContent = _encoding.GetBytes(_content);
            }

            return _serialisedContent;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }
    }
}