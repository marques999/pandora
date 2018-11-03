using System.Collections.Generic;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal interface IUrlProcessor
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        int GetOrder();

        /// <summary>
        /// </summary>
        /// <param name="inputUrls"></param>
        /// <param name="outputUrls"></param>
        void Process(IList<string> inputUrls, IList<string> outputUrls);
    }
}