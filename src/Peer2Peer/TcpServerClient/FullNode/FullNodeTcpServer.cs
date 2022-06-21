using System.Net;
using System.Net.Sockets;
using BlockchainNS;
using Peer2PeerNS.ServerNS;
using ZRD.Peer2Peer.TcpServerClient.Abstract;

namespace ZRD.Peer2Peer.Server
{
    public class FullNodeTcpServer: ITcpServer
    {

        public int port;
        private TcpListener listener;

        public void Init(int portToOpen)
        {
            this.port = portToOpen;
            this.listener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.port);
            this.listener.Start();
        }

        public TcpClient AcceptConnections()
        {
            TcpClient externalPeer = this.listener.AcceptTcpClient();
            return externalPeer;
        }

        public NetworkStream GetNetworkStreamFromPeer(TcpClient peer)
        {
            throw new System.NotImplementedException();
        }

        public string GetDataFromNetworkStreamBytes(NetworkStream stream)
        {
            throw new System.NotImplementedException();
        }

        public Blockchain DeserializeDataStringToBlockchain(string dataString)
        {
            throw new System.NotImplementedException();
        }

        public void Kill()
        {
            throw new System.NotImplementedException();
        }

        /**
         * UTILS
         */
        public TcpListener GetListenerSocket()
        {
            return this.listener;
        }
    }
}