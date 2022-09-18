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

        public TcpClient AcceptConnection();
        public void HandleDataFromPeer(TcpClient peer);
        public void Kill();
    }
}