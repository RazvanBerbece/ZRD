using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using BlockchainNS;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;
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
            try
            {
                this.listener = new TcpListener(IPAddress.Parse(this.node.GetPrivateIpAddressString()), this.port);
                this.listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

            string[] opTokens = receivedData.Split(" ");
            if (opTokens[0].Equals("GET"))
            {
                // GET operation requested from client peer
                switch (opTokens[1])
                {
                    case "BLOCKCHAIN_FOR_INIT":
                        // Send current blockchain instance for init on client peer side
                        byte[] blockchainStateBuffer = Encoding.ASCII.GetBytes(this.node.Blockchain.ToJsonString());
                        stream.Write(blockchainStateBuffer, 0, blockchainStateBuffer.Length);
                        break;
                    case "PEER_LIST":
                        // Send current peer list state for sync / merge / append on client peer side
                        string peerListString = System.IO.File.ReadAllText("local/Peers/Peers.json");
                        byte[] peerListBuffer = Encoding.ASCII.GetBytes(peerListString);
                        stream.Write(peerListBuffer, 0, peerListBuffer.Length);
                        break;
                    case "BALANCE":
                        string walletAddress = opTokens[2];
                        int walletAmount = this.node.Blockchain.GetBalance(walletAddress);
                        byte[] amountBuffer = BitConverter.GetBytes(walletAmount);
                        stream.Write(amountBuffer, 0, amountBuffer.Length);
                        break;
                }
            }
            else
            {
                // Handle data by iteratively deserializing through the possible scenarios :
                //      1. Lightweight node connecting to send new Transaction to be added to mempool + validation
                //      2. Miner node connecting to send new Blockchain state after new block was mined + validation
                //      3. Any other node connecting to request peer list updates TODO
                if (Blockchain.JsonStringToBlockchainInstance(receivedData) is Blockchain upstreamBlockchain)
                {
                    // TODO
                }
                else if (Transaction.JsonStringToTransactionInstance(receivedData) is Transaction transaction)
                {
                    // Add new transaction to mempool, status will be true if transaction is valid 
                    // and if it was successfully added to mempool
                    bool addStatus = this.node.Blockchain.AddTransaction(transaction);
                    if (addStatus)
                    {
                        // Send successful response + balance (tbc) to client peer
                        string successMessage = "Transaction successfully added to peer mempool";
                        byte[] successMessageBuffer = Encoding.ASCII.GetBytes(successMessage);
                        stream.Write(successMessageBuffer, 0, successMessageBuffer.Length);
                    }
                    else
                    {
                        // Send fail response to client peer
                        string errMessage = "Transaction received by peer is not valid against the ZRD Blockchain";
                        byte[] errMessageBuffer = Encoding.ASCII.GetBytes(errMessage);
                        stream.Write(errMessageBuffer, 0, errMessageBuffer.Length);
                    }
                }
                else
                {
                    bool isIncomingPeerList;
                    try
                    {
                        DiscoveryManager discoveryManager = new DiscoveryManager();
                        // Check for incoming peer list update
                        List<PeerDetails> upstreamPeerDetailsList = JsonSerializer.Deserialize<List<PeerDetails>>(
                            receivedData,
                            options: new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true,
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                            });
                        isIncomingPeerList = true;
                        // Send local peer list to connected peer to merge as well
                        string localPeerDetails = System.IO.File.ReadAllText("local/Peers/Peers.json");
                        byte[] localPeerListBuffer = Encoding.ASCII.GetBytes(localPeerDetails);
                        stream.Write(localPeerListBuffer, 0, localPeerListBuffer.Length);
                        // Merge / append / write new peer list to local Peers.json
                        List<PeerDetails> mergedList = DiscoveryManager.MergePeerLists(upstreamPeerDetailsList,
                            discoveryManager.LoadPeerDetails("local/Peers/Peers.json"));
                        discoveryManager.WritePeerListToFile(mergedList, "local/Peers/Peers.json");
                    
                    }
                    catch (Exception)
                    {
                        isIncomingPeerList = false;
                    }

                    if (!isIncomingPeerList)
                    {
                        // Data received could not be parsed into object instance
                        // i.e. received data format does not match expected data formats
                        // Write back to peer and close connection
                        byte[] errBuffer = Encoding.ASCII.GetBytes("Data received by server does not match expected formats");
                        stream.Write(errBuffer, 0, errBuffer.Length);   
                    }
                }
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

        public static string GetPeerPublicIp(TcpClient peer)
        {
            var peerEndpoint = peer.Client.LocalEndPoint as IPEndPoint;
            string localAddress = peerEndpoint.Address.ToString();
            return localAddress;
        }
        
        public void RunServer(int portToOpen)
        {
            try
            {
                Init(portToOpen);
                while (true)
                {
                    TcpClient peer = AcceptConnection();
                    Console.WriteLine($"Accepted incoming connection from {GetPeerPublicIp(peer)}");
                    HandleDataFromPeer(peer);
                    peer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
    }
}