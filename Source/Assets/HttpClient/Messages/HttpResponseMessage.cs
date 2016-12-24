using System;
using System.Net;

namespace CI.HttpClient
{
    public class HttpResponseMessage<T>
    {
        public HttpWebRequest OriginalRequest
        {
            get; set;
        }

        public HttpWebResponse OriginalResponse
        {
            get; set;
        }

        public T Data
        {
            get; set;
        }

        public long ContentLength
        {
            get; set;
        }

        public long TotalContentRead
        {
            get; set;
        }

        public long ContentReadThisRound
        {
            get; set;
        }

        public int PercentageComplete
        {
            get
            {
                if (ContentLength <= 0)
                {
                    return 0;
                }
                else
                {
                    return (int)(((double)TotalContentRead / ContentLength) * 100);
                }
            }
        }

        public HttpStatusCode StatusCode
        {
            get; set;
        }

        public string ReasonPhrase
        {
            get; set;
        }

        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }

        public Exception Exception
        {
            get; set;
        }
    }
}