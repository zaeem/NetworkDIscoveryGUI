using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NetworkDiscoveryGUI
{
    class MacAPI
    {
        #region fields
        private const string baseUrl = "https://macvendors.co/api/{0}";

        #endregion

        #region methods
        public static MacAddress GetMAC(string findmanu)
        {
            var url = string.Format(baseUrl, findmanu);
            var syncClient = new WebClient();
            var content = syncClient.DownloadString(url);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MacAddress));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            {
                var mac = (MacAddress)serializer.ReadObject(ms);
                return mac;
            }
        }

        #endregion
    }
}
