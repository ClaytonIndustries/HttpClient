using System.IO;

namespace CI.HttpClient
{
    public interface IHttpContent
    {
        int GetContentLength();
        string GetContentType();
        byte[] ReadAsByteArray();
        Stream ReadAsStream();
    }
}