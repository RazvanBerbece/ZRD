using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BlockchainNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using TransactionNS;
using ZRD.Peer2Peer.TcpServerClient.Abstract;

namespace Peer2PeerNS.FullNodeTcpServerNS
{
    public class FullNodeTcpServer: ITcpServer
    {

        public int port;
        private TcpListener listener;

        private FullNode node;
        
        /// <summary>
        /// Open given port on localhost to accept TCP connections
        /// And start listening
        /// </summary>
        /// <param name="portToOpen">Port to open on localhost machine</param>
        public void Init(int portToOpen)
        {
            this.port = portToOpen;
            this.listener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.port);
            this.listener.Start();
        }

        public TcpClient AcceptConnection()
        {
            TcpClient externalPeer = this.listener.AcceptTcpClient();
            return externalPeer;
        }
        
        /// <summary>
        /// For a given peer, read data from network stream,
        /// handle it based on the expected type passed as parameter.
        /// and return a suitable response to peer
        /// </summary>
        /// <param name="peer">Peer that TCP connected to server</param>
        public void HandleDataFromPeer(TcpClient peer)
        {
            // Get incoming data through stream and create buffer for reading into 
            NetworkStream stream = peer.GetStream();
            byte[] buffer = new byte[peer.ReceiveBufferSize];
            
            // Read incoming stream
            int readBytes = stream.Read(buffer, 0, peer.ReceiveBufferSize);
            
            // Convert data from buffer to string
            string receivedData = Encoding.ASCII.GetString(buffer, 0, readBytes);
            Console.WriteLine("Received : " + receivedData);
            
            // Handle data by iteratively deserializing through the possible scenarios :
            //      1. Lightweight node connecting to send new Transaction to be added to mempool + validation
            //      2. Miner node connecting to send new Blockchain state after new block was mined + validation
            //      3. Any other node connecting to request initial Blockchain data for setup TODO
            if (Blockchain.JsonStringToBlockchainInstance(receivedData) is Blockchain upstreamBlockchain)
            {
                // TODO
            }
            else if (Transaction.JsonStringToTransactionInstance(receivedData) is Transaction transaction)
            {
                // TODO
            }
            else
            {
                // Data received could not be parsed into object instance
                // i.e. received data format does not match expected data formats
                // Write back to peer and close connection
                byte[] errBuffer = Encoding.ASCII.GetBytes("Data received by server does not match expected formats");
                stream.Write(errBuffer, 0, errBuffer.Length);
            }
            
        }

        public void Kill()
        {
            this.listener.Stop();
        }

        /**
         * UTILS
         */
        public TcpListener GetListenerSocket()
        {
            return this.listener;
        }
        
        public void SetFullNode(FullNode newFullNode)
        {
            this.node = newFullNode;
        }
        
        public void RunServer(int portToOpen)
        {
            Init(portToOpen);
            while (true)
            {
                TcpClient peer = AcceptConnection();
                HandleDataFromPeer(peer);
                peer.Close();
            }
        }
    }
}