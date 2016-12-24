using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class MultipartContent : IHttpContent
    {
        private readonly List<IHttpContent> _content;
        private readonly string _boundary;

        private byte[] _serialisedContent;

        public MultipartContent()
        {
            _content = new List<IHttpContent>();
            _boundary = Guid.NewGuid().ToString();
        }

        public MultipartContent(string boundary)
        {
            _content = new List<IHttpContent>();
            _boundary = boundary;
        }

        public void Add(IHttpContent content)
        {
            _content.Add(content);
        }

        public int GetContentLength()
        {
            return ReadAsByteArray().Length;
        }

        public string GetContentType()
        {
            return "multipart/form-data; boundary=" + _boundary;
        }

        public byte[] ReadAsByteArray()
        {
            if(_serialisedContent == null)
            {
                List<byte> bytes = new List<byte>();

                if (_content.Count == 0)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes("--" + _boundary + "\r\n"));
                }

                foreach(IHttpContent content in _content)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes("--" + _boundary + "\r\n"));
                    bytes.AddRange(Encoding.UTF8.GetBytes("Content-Type: " + content.GetContentType() + "\r\n"));
                    bytes.AddRange(Encoding.UTF8.GetBytes("\r\n"));
                    bytes.AddRange(content.ReadAsByteArray());
                    bytes.AddRange(Encoding.UTF8.GetBytes("\r\n"));
                }

                bytes.AddRange(Encoding.UTF8.GetBytes("--" + _boundary + "--\r\n"));

                _serialisedContent = bytes.ToArray();
            }

            return _serialisedContent;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }
    }
}