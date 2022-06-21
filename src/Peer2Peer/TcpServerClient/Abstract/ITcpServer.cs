using System.Net.Sockets;
using BlockchainNS;

namespace ZRD.Peer2Peer.TcpServerClient.Abstract
{
    public interface ITcpServer
    {
        /// <summary>
        /// Return a TcpClient socket which 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public void Init(int port);

        public TcpClient AcceptConnections();
        public NetworkStream GetNetworkStreamFromPeer(TcpClient peer);
        public string GetDataFromNetworkStreamBytes(NetworkStream stream);
        public Blockchain DeserializeDataStringToBlockchain(string dataString);
        public void Kill();
    }
}