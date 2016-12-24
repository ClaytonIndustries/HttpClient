using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class FormUrlEncodedContent : IHttpContent
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _nameValueCollection;

        private byte[] _serialisedContent;

        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            _nameValueCollection = nameValueCollection;
        }

        public int GetContentLength()
        {
            return ReadAsByteArray().Length;
        }

        public string GetContentType()
        {
            return "application/x-www-form-urlencoded";
        }

        public byte[] ReadAsByteArray()
        {
            if (_serialisedContent == null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (KeyValuePair<string, string> nameValue in _nameValueCollection)
                {
                    UrlEncoded(stringBuilder, nameValue.Key, nameValue.Value);
                }

                _serialisedContent = Encoding.ASCII.GetBytes(stringBuilder.ToString());
            }

            return _serialisedContent;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }

        private void UrlEncoded(StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
                sb.Append("&");
            sb.Append(Uri.EscapeUriString(name));
            sb.Append("=");
            sb.Append(Uri.EscapeUriString(value));
        }
    }
}