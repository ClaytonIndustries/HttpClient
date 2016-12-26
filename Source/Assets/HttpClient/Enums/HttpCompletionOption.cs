
namespace CI.HttpClient
{
    public enum HttpCompletionOption
    {
        /// <summary>
        /// Read all response content without rasing progress updates
        /// </summary>
        AllResponseContent,
        /// <summary>
        /// Read response content in blocks and raise progress updates
        /// </summary>
        StreamResponseContent
    }
}