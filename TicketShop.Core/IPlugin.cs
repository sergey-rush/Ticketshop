using System.Collections.Generic;

namespace TicketShop.Core
{
    public interface IPlugin
    {
        /// <summary>
        /// Requests for printer name the plugin intended
        /// </summary>
        /// <returns>Printer name the plugin designed</returns>
        string GetName();

        /// <summary>
        /// Sets Ip address for communication with printer
        /// </summary>
        /// <param name="ip">Ip address</param>
        void SetIp(string ip);

        /// <summary>
        /// Checks if printer is ready to print
        /// </summary>
        /// <returns></returns>
        bool IsReady();

        /// <summary>
        /// Prints document
        /// </summary>
        /// <param name="bytes">Input document bytes</param>
        /// <returns>Returns result of printing request</returns>
        bool Print(byte[] bytes);
        /// <summary>
        /// Retrieves printer information
        /// </summary>
        /// <param name="clause">Input query</param>
        /// <returns>Dictionary string/object> result</returns>
        Dictionary<string, object> RunQuery(string clause);

        /// <summary>
        /// Applies configuration settings to the printer 
        /// </summary>
        void Setup();

        /// <summary>
        /// Calibrates printer in order configuration settings take effect
        /// </summary>
        void Calibrate();
    }
}