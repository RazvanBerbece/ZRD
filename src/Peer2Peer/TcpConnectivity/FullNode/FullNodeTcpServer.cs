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
using Peer2PeerNS.TcpConnectivity.PeerCommLogStructNS;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.TcpDirectionEnumNS;
using StaticsNS;
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
                IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
                this.listener = new TcpListener(ipAddress, this.port);
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
            string receivedData = Encoding.Default.GetString(buffer, 0, readBytes);
            // Log TCP IN details
            PeerCommLogStruct.LogPeerCommunication(peer, receivedData, DateTime.Now, TcpDirectionEnum.In);
            // Console.WriteLine("Received : " + receivedData);

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
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, this.node.Blockchain.ToJsonString(), DateTime.Now, TcpDirectionEnum.Out);
                        break;
                    case "PEER_LIST":
                        // Send current peer list state for sync / merge / append on client peer side
                        string peerListString = System.IO.File.ReadAllText("local/Peers/Peers.json");
                        byte[] peerListBuffer = Encoding.ASCII.GetBytes(peerListString);
                        stream.Write(peerListBuffer, 0, peerListBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, peerListString, DateTime.Now, TcpDirectionEnum.Out);
                        break;
                    case "BALANCE":
                        string walletAddress = opTokens[2];
                        int walletAmount = this.node.Blockchain.GetBalance(walletAddress);
                        byte[] amountBuffer = BitConverter.GetBytes(walletAmount);
                        stream.Write(amountBuffer, 0, amountBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, walletAmount.ToString(), DateTime.Now, TcpDirectionEnum.Out);
                        break;
                }
            }
            else
            {
                // Handle data by iteratively deserializing through the possible scenarios :
                //      1. Lightweight node connecting to send new Transaction to be added to mempool + validation
                //      2. Miner node connecting to send new Block after new block was mined + validation
                //      3. Full node connecting to send Blockchain nas part of broadcasting
                //      4. Any other node connecting to request peer list updates TODO
                if (Blockchain.JsonStringToBlockchainInstance(receivedData) is { } remoteBlockchain)
                {
                    Console.WriteLine("-- DESERIALIZING BLOCKCHAIN --");
                    bool shouldUpdatePeer = ResolveBlockchainMerge(this.node.Blockchain, remoteBlockchain);
                    if (shouldUpdatePeer)
                    {
                        // Write new blockchain to stream
                        byte[] blockchainStateBuffer = Encoding.ASCII.GetBytes(this.node.Blockchain.ToJsonString());
                        stream.Write(blockchainStateBuffer, 0, blockchainStateBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, this.node.Blockchain.ToJsonString(), DateTime.Now, TcpDirectionEnum.Out);   
                    }
                }
                else if (Transaction.JsonStringToTransactionInstance(receivedData) is { } transaction)
                {
                    Console.WriteLine("-- DESERIALIZING TRANSACTION --");
                    // Add new transaction to mempool, status will be true if transaction is valid 
                    // and if it was successfully added to mempool
                    bool addStatus = this.node.Blockchain.AddTransaction(transaction);
                    if (addStatus)
                    {
                        // Send successful response + balance (tbc) to client peer
                        string successMessage = "Transaction successfully added to peer mempool";
                        byte[] successMessageBuffer = Encoding.ASCII.GetBytes(successMessage);
                        stream.Write(successMessageBuffer, 0, successMessageBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, successMessage, DateTime.Now, TcpDirectionEnum.Out);
                    }
                    else
                    {
                        // Send fail response to client peer
                        string errMessage = "Transaction received by peer is not valid against the ZRD Blockchain";
                        byte[] errMessageBuffer = Encoding.ASCII.GetBytes(errMessage);
                        stream.Write(errMessageBuffer, 0, errMessageBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, errMessage, DateTime.Now, TcpDirectionEnum.Out);
                    }
                }
                else
                {
                    bool isIncomingPeerList;
                    try
                    {
                        Console.WriteLine("-- DESERIALIZING INCOMING PEER LIST --");
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
                        // Merge / append / write new peer list to local Peers.json
                        List<PeerDetails> mergedList = DiscoveryManager.MergePeerLists(upstreamPeerDetailsList,
                            discoveryManager.LoadPeerDetails("local/Peers/Peers.json"));
                        discoveryManager.WritePeerListToFile(mergedList, "local/Peers/Peers.json");
                        // Send local peer list to connected peer to merge as well
                        string localPeerDetails = System.IO.File.ReadAllText("local/Peers/Peers.json");
                        byte[] localPeerListBuffer = Encoding.ASCII.GetBytes(localPeerDetails);
                        stream.Write(localPeerListBuffer, 0, localPeerListBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, localPeerDetails, DateTime.Now, TcpDirectionEnum.Out);
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
                        string errMessage = "Data received by server does not match expected formats";
                        byte[] errBuffer = Encoding.ASCII.GetBytes(errMessage);
                        stream.Write(errBuffer, 0, errBuffer.Length);
                        
                        PeerCommLogStruct.LogPeerCommunication(peer, errMessage, DateTime.Now, TcpDirectionEnum.Out);
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
        
        /// <summary>
        /// Resolves the blockchain sync discussion between two peers by :
        ///     - Accepting the longer one, if valid
        ///     - Merging the local mempool with the upstream one's valid & unique transactions
        /// Mutates the node Blockchain if remote Blockchain is preferred
        /// </summary>
        /// <param name="localBlockchain"></param>
        /// <param name="remoteBlockchain"></param>
        /// <returns>1 if peer needs to be updated with new Blockchain, 0 otherwise</returns>
        private bool ResolveBlockchainMerge(Blockchain localBlockchain, Blockchain remoteBlockchain)
        {
            if (remoteBlockchain.Chain.Count <= localBlockchain.Chain.Count) return false;
            // Check that upstream Blockchain is valid
            if (remoteBlockchain.IsValid())
            {
                // Resolve unvalidated transactions & Use remote Blockchain for local
                List<Transaction> mergedTransactions = new List<Transaction>() { };
                // Add local mempool to final list of unvalidated transactions
                bool localMempoolIsInitiallyEmpty = localBlockchain.UnconfirmedTransactions.Count == 0;
                mergedTransactions.AddRange(localBlockchain.UnconfirmedTransactions);
                // Add valid transactions from remote mempool into local one
                foreach (Transaction transaction in remoteBlockchain.UnconfirmedTransactions)
                {
                    // If upstream mempool transaction is valid and NOT a duplicate in local
                    if (transaction.IsValid(localBlockchain) && !mergedTransactions.Contains(transaction))
                    {
                        mergedTransactions.Add(transaction);
                    }
                }
                // Sync local
                this.node.SetBlockchain(remoteBlockchain);
                this.node.Blockchain.UnconfirmedTransactions = mergedTransactions;

                if (localMempoolIsInitiallyEmpty)
                {
                    return false;
                }
                
                return true;
            }

            return false;
        }

        public void RunServer(int portToOpen)
        {
            try
            {
                Init(portToOpen);
                while (true)
                {
                    TcpClient peer = AcceptConnection();
                    Console.WriteLine($"Accepted incoming connection from {Statics.GetPeerPublicIp(peer)}");
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