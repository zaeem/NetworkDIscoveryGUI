using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetworkDiscoveryGUI
{
    class AssetHandler
    {
        public static HttpClient client;
        public AssetHandler(HttpClient c)
        {
            client = c;
        }
        public AssetHandler()
        { }
        public bool initRequest()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization");
            return true;
        }
        public bool postAssetData(FormUrlEncodedContent postBody, string path)
        {
            var wcfResponse = client.PostAsync(path, postBody);
            wcfResponse.Wait();
            HttpResponseMessage res = wcfResponse.Result;
            return res.IsSuccessStatusCode;
        }
    }
}
