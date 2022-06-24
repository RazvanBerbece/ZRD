using System;

namespace Peer2PeerNS.DiscoveryNS.PeerDetailsNS
{
    public struct PeerDetails
    {
        public string ExtIp { get; set; } // remote address
        public int Port  { get; set; } // port open on remote machine
        public string PeerType { get; set; } // type of peer running on details above: light, full, miner

        public PeerDetails(string extIp, int port, string type)
        {
            if (string.IsNullOrEmpty(extIp) ||
                port is < 1 or >= 65535 ||
                (!type.Equals("FULL") && !type.Equals("MINER"))
               )
            {
                throw new ArgumentException($"PeerDetails cannot be constructed with the provided details : {extIp}-{port}-{type}");
            }
            this.ExtIp = extIp;
            this.Port = port;
            this.PeerType = type;
        }

    }
}