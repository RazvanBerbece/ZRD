using System.Net.Sockets;
using BlockchainNS;
using Org.BouncyCastle.Utilities.Net;

namespace ZRD.Peer2Peer.TcpServerClient.Abstract
{
    public interface ITcpClient
    {
        public void Init(string dest, int port);
        public NetworkStream Connect();
        public dynamic SendDataStringToPeer(string data, NetworkStream stream);
        public void Close();
    }
}