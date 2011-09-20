using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Luna.Common.Network
{
    [XmlType]
    public class Proxy
    {
        public bool HttpInited { get; set; }
        [XmlElement]
        public string IP { get; set; }
        
        [XmlElement]
        public string Port { get; set; }
        [XmlElement]
        public ProxyType ProxyType { get; set; }
        [XmlElement]
        public string Username { get; set; }
        [XmlElement]
        public string Password { get; set; }

        [XmlElement]
        public string Domain { get; set; }
        public bool UsingFlag { get; set; }
    }

    public enum ProxyType
    {
        None,
        Http
        //Socket4,
        //Socket5
    }
}
