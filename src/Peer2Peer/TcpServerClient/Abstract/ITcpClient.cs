using System.Net.Sockets;
using BlockchainNS;
using Org.BouncyCastle.Utilities.Net;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.DataOutTypeNS;

namespace ZRD.Peer2Peer.TcpServerClient.Abstract
{
    public interface ITcpClient
    {
        public void Init(string dest, int port);
        public NetworkStream Connect();
        public dynamic SendDataStringToPeer(string data, NetworkStream stream, DataOutType type);
        public void Close();
    }
}