using System;
using System.Collections.Generic;

namespace CI.HttpClient
{
    /// <summary>
    /// Represents a HTTP request message
    /// </summary>
    public class HttpRequestMessage
    {
        /// <summary>
        /// The content of the HTTP message
        /// </summary>
        public IHttpContent Content { get; set; }

        /// <summary>
        /// The HTTP method used by the HTTP message
        /// </summary>
        public HttpAction Method { get; set; }

        /// <summary>
        /// Headers to be included with the HTTP message. If the HttpClient also has headers then they will be merged together,
        /// if there are duplicates then these take precedence
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The Uri used by the HTTP message
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        ///  Initialises a new instance of the CI.HttpClient.HttpRequestMessage class
        /// </summary>
        public HttpRequestMessage()
        {
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initialises a new instance of the CI.HttpClient.HttpRequestMessage class with a HTTP method and request Uri
        /// </summary>
        /// <param name="method">The HTTP method</param>
        /// <param name="uri">The request Uri</param>
        public HttpRequestMessage(HttpAction method, Uri uri)
            : this()
        {
            Method = method;
            Uri = uri;
        }
    }
}