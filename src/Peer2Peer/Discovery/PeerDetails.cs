namespace Peer2PeerNS.DiscoveryNS.PeerDetailsNS
{
    public struct PeerDetails
    {
        public string ExtIp { get; set; } // remote address
        public int Port  { get; set; } // port open on remote machine
        public string PeerType { get; set; } // type of peer running on details above: light, full, miner

        public PeerDetails(string extIp, int port, string type)
        {
            this.ExtIp = extIp;
            this.Port = port;
            this.PeerType = type;
        }

        public string GetExtIp()
        {
            return this.ExtIp;
        }
        public int GetPort()
        {
            return this.Port;
        }
        public string GetPeerType()
        {
            return this.PeerType;
        }
        
    }
}