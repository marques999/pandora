using System.Net;

namespace XameteoTest.API
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class ApixuByCity : Apixu
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="apixuQuery"></param>
        public ApixuByCity(string apixuQuery) : base(WebUtility.UrlEncode(apixuQuery))
        {
        }
    }
}