using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NetworkDiscoveryGUI
{
    [DataContract]
    class MacAddress
    {
        [DataMember]
        public result result { get; set; }
    }
    public class result
    {
        public string company { get; set; }
        public string mac_prefix { get; set; }
        public string address { get; set; }
        public string start_hex { get; set; }
        public string end_hex { get; set; }
        public string country { get; set; }
        public string type { get; set; }
    }
}
