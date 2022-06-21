using System.Net.Sockets;
using BlockchainNS;
using Org.BouncyCastle.Utilities.Net;

namespace ZRD.Peer2Peer.TcpServerClient.Abstract
{
    public interface ITcpClient
    {
        public void Init(IPAddress dest, int port);
        public NetworkStream Connect();
        public string GetDataFromNetworkStream(NetworkStream stream);
        public Blockchain DeserializeDataStringToBlockchain(string dataString);
        public void Close();
    }
}