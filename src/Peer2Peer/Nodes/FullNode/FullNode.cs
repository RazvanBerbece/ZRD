using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using BlockchainNS;
using Peer2PeerNS.NodesNS.Abstract;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.FullNodeTcpClientNS;
using Peer2PeerNS.FullNodeTcpServerNS;
using StaticsNS;
using WalletNS;
using WalletNS.BlockchainWalletNS;
using DiscoveryManager = Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS.DiscoveryManager;

namespace Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS
{
    public class FullNode: INode
    {
        
        // Core
        public Blockchain Blockchain;
        public BlockchainWallet NetworkWallet;
        
        // Networking
        private IPAddress privateIpAddress;
        private IPAddress publicNatIpAddress;
        private int port;
        
        // Peer Discovery
        private List<PeerDetails> possiblePeers;

        private FullNode() { }
        
        /// <summary>
        /// Configures a lightweight node on the user machine
        /// Config consists of :
        ///     - setting node public IP address with EXT Public IP
        ///     - setting node private IP address with private local IP (e.g. : 192.168.x.x)
        ///     - loading up peer list from Peers.json
        /// </summary>
        /// <returns></returns>
        public static FullNode ConfigureNode()
        {
            FullNode node = new FullNode();
            node.SetPrivateIpAddress(Statics.GetLocalIpAddress());
            node.SetPublicNatIpAddress(Statics.GetExternalPublicIpAddress());
            try
            {
                node.possiblePeers = new DiscoveryManager().LoadPeerDetails("local/Peers/Peers.json");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not load Peers.json file: {e}");
            }
            return node;
        }
        
        public void SetBlockchain(Blockchain upstreamBlockchain)
        {
            this.Blockchain = upstreamBlockchain;
        }

        public void SendBlockchainToPeer(string peerIpAddress, int port)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Iteratively attempts connections to peers and sends them the blockchain and the peerList
        /// TODO: Consider merging peerLists ??
        /// </summary>
        public void Broadcast()
        {
            foreach (PeerDetails peerItem in this.possiblePeers)
            {
                // Create connection to peer EXT NAT IP on their open port
                FullNodeTcpClient peer = new FullNodeTcpClient();
                peer.Init(peerItem.ExtIp, peerItem.Port);
                NetworkStream peerStream = peer.Connect();
                
                // Send data - Blockchain data
                string blockchainBroadcastReceivedData = peer.SendDataStringToPeer(this.Blockchain.ToJsonString(), peerStream);
                // Handle response - Blockchain data
                // TODO ?
                
                // Send data - Peer list data 
                string peerListJsonString = JsonSerializer.Serialize(
                    this.possiblePeers.ToString(),
                    options: new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    }
                );
                string peerListBroadcastReceivedData = peer.SendDataStringToPeer(peerListJsonString, peerStream);
                // Handle response - Peer list data
                // TODO ?
                
            }
        }

        public void DownloadBlockchainFromPeer()
        {
            // Discover a MINER or FULL peer to ask for a copy of the ZRD Blockchain
            DiscoveryManager peerDiscovery = new DiscoveryManager();
            List<PeerDetails> possiblePeers = peerDiscovery.LoadPeerDetails("local/Peers/Peers.json");
            PeerDetails suitablePeer = peerDiscovery.FindSuitablePeerInList("FULL MINER", possiblePeers);
            
            // Use found peer details to connect to it and ask for blockchain copy
            // by sending "GET BLOCKCHAIN_FOR_INIT" operation
            FullNodeTcpClient peerClient = new FullNodeTcpClient();
            peerClient.Init(suitablePeer.GetExtIp(), suitablePeer.GetPort());
            string response = peerClient.SendDataStringToPeer("GET BLOCKCHAIN_FOR_INIT", peerClient.Connect());
            
            // Handle response: Deserialize received JSON Blockchain to actual instance
            Blockchain upstreamBlockchain = Blockchain.JsonStringToBlockchainInstance(response);
            if (upstreamBlockchain is not null)
            {
                if (upstreamBlockchain.IsValid())
                {
                    SetBlockchain(upstreamBlockchain);
                    SetWallet(upstreamBlockchain.BlockchainWallet);
                    Blockchain.SaveJsonStateToFile(this.Blockchain.ToJsonString(), "local/Blockchain/ZRD.json");
                }
                else
                {
                    throw new CryptographicException("Blockchain instance from upstream is compromised");
                }
            }
            else
            {
                throw new JsonException("Blockchain JSON string from peer could not be deserialized");
            }
        }
        
        public void StartFullServer()
        {
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(this);
            server.RunServer(this.port);
        }
        
        public void SetWallet(BlockchainWallet newWallet)
        {
            this.NetworkWallet = newWallet;
        }

        public void SetPrivateIpAddress(IPAddress newPrivateIpAddress)
        {
            this.privateIpAddress = newPrivateIpAddress;
        }
        
        public void SetPublicNatIpAddress(IPAddress newPublicNatIpAddress)
        {
            this.publicNatIpAddress = newPublicNatIpAddress;
        }
        
        public string GetPrivateIpAddressString()
        {
            if (this.privateIpAddress == null || string.IsNullOrEmpty(this.privateIpAddress.ToString()))
            {
                return "";
            }
            return this.privateIpAddress.ToString();
        }
        
        public string GetPublicNatIpAddressString()
        {
            if (this.publicNatIpAddress == null || string.IsNullOrEmpty(this.publicNatIpAddress.ToString()))
            {
                return "";
            }
            return this.publicNatIpAddress.ToString();
        }

        public void SetPort(int newPort)
        {
            this.port = newPort;
        }

        /// <summary>
        /// Adds current node connection details (ext NAT IP, open port, node type)
        /// to the list of potential peers
        /// </summary>
        public void StoreFullNodeDetailsInPeersList()
        {
            DiscoveryManager peerDiscovery = new DiscoveryManager();
            peerDiscovery.StoreExtNatIpAndPortToFile(
                this.publicNatIpAddress.ToString(), 
                this.port, 
                "FULL", 
                "local/Peers/Peers.json"
                );
        }

    }
}