using System.Net;
using BlockchainNS;

namespace Peer2PeerNS.NodesNS.Abstract
{
    public interface INode
    {
        public void SetBlockchain(Blockchain upstreamChain);
        public void SetPrivateIpAddress(IPAddress newPrivateIpAddress);
        public void SetPublicNatIpAddress(IPAddress newPublicNatIpAddress);
        public string GetPrivateIpAddressString();
        public string GetPublicNatIpAddressString();
    }
}