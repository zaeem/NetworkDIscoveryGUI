using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDiscoveryGUI
{
    [System.Runtime.Serialization.DataContract]
    public class InfoToSend
    {
        [System.Runtime.Serialization.DataMember]
        public string displayName { get; set; } 
        [System.Runtime.Serialization.DataMember]
        public string macAddress { get; set; } 
        [System.Runtime.Serialization.DataMember]
        public string ipAddress { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string os { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string manf { get; set; }
    }
}
