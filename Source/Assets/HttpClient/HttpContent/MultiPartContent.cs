using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class MultipartContent : IHttpContent, IEnumerable<IHttpContent>
    {
        private const string DEFAULT_SUBTYPE = "form-data";

        private readonly List<IHttpContent> _content;
        private readonly string _boundary;

        private string _contentType;
        private long _contentLength;

        public byte[] BoundaryStartBytes { get; private set; }
        public byte[] BoundaryEndBytes { get; private set; }
        public byte[] CRLFBytes { get; private set; }

        public ContentReadAction ContentReadAction
        {
            get { return ContentReadAction.Multi; }
        }

        public MultipartContent()
        {
            _content = new List<IHttpContent>();
            _boundary = Guid.NewGuid().ToString();
            CreateConentType(DEFAULT_SUBTYPE);
            CreateDelimiters();
        }

        public MultipartContent(string boundary)
        {
            _content = new List<IHttpContent>();
            _boundary = boundary;
            CreateConentType(DEFAULT_SUBTYPE);
            CreateDelimiters();
        }

        public MultipartContent(string boundary, string subtype)
        {
            _content = new List<IHttpContent>();
            _boundary = boundary;
            CreateConentType(subtype);
            CreateDelimiters();
        }

        private void CreateConentType(string subtype)
        {
            _contentType = "multipart/" + subtype + "; boundary=" + _boundary;
        }

        private void CreateDelimiters()
        {
            CRLFBytes = Encoding.UTF8.GetBytes("\r\n");
            BoundaryStartBytes = Encoding.UTF8.GetBytes("--" + _boundary + "\r\n");
            BoundaryEndBytes = Encoding.UTF8.GetBytes("--" + _boundary + "--\r\n");
        }

        public void Add(IHttpContent content)
        {
            _content.Add(content);
        }

        public long GetContentLength()
        {
            if (_contentLength == 0)
            {
                long length = 0;

                if (_content.Count == 0)
                {
                    length += BoundaryStartBytes.Length;
                }

                foreach (IHttpContent content in _content)
                {
                    length += BoundaryStartBytes.Length;
                    length += Encoding.UTF8.GetBytes("Content-Type: " + content.GetContentType()).Length;
                    length += CRLFBytes.Length;
                    length += CRLFBytes.Length;

                    if (content.ContentReadAction == ContentReadAction.ByteArray)
                    {
                        length += content.GetContentLength();
                    }
                    else
                    {
                        length += content.ReadAsStream().Length;
                    }

                    length += CRLFBytes.Length;
                }

                length += BoundaryEndBytes.Length;

                _contentLength = length;
            }

            return _contentLength;
        }

        public string GetContentType()
        {
            return _contentType;
        }

        public byte[] ReadAsByteArray()
        {
            throw new NotImplementedException();
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IHttpContent> GetEnumerator()
        {
            return _content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}