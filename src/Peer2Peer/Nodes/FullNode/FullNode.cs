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
                new DiscoveryManager().LoadPeerDetails("local/Peers/Peers.json");
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

        /// <summary>
        /// Iteratively attempts connections to peers and sends them the blockchain and the peerList
        /// Runs in a loop that never exits
        /// </summary>
        public void Broadcast()
        {
            // Broadcast every 10 seconds
            DateTime lastBroadcast = System.DateTime.Now;
            while (true)
            {
                
                if (lastBroadcast.AddSeconds(10) > System.DateTime.Now)
                    continue;
                
                DiscoveryManager peerDiscovery = new DiscoveryManager();
                List<PeerDetails> possiblePeers = peerDiscovery.LoadPeerDetails("local/Peers/Peers.json");
                foreach (PeerDetails peerItem in possiblePeers)
                {

                    if (
                        peerItem.Port == this.port &&
                        peerItem.ExtIp.Equals(Statics.GetExternalPublicIpAddress().ToString()) &&
                        peerItem.PeerType.Equals("FULL")
                    )
                    {
                        // This machine; do not attempt broadcast and continue
                        continue;
                    }
                    
                    // Create connection to peer EXT NAT IP on their open port
                    FullNodeTcpClient peer = new FullNodeTcpClient();
                    peer.Init(peerItem.ExtIp, peerItem.Port);
                    NetworkStream peerStream = peer.Connect();
                    Console.WriteLine($"Connected to {peerItem.ExtIp}:{peerItem.Port}");
                
                    // Send data - Blockchain data
                    // string blockchainBroadcastReceivedData = peer.SendDataStringToPeer(this.Blockchain.ToJsonString(), peerStream);
                    // Handle response - Blockchain data
                    // TODO ?
                
                    // Send data - Peer list data 
                    string peerListJsonString = JsonSerializer.Serialize(
                        possiblePeers,
                        options: new JsonSerializerOptions()
                        {
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                        }
                    );
                    string peerListBroadcastReceivedData = peer.SendDataStringToPeer(peerListJsonString, peerStream);
                    Console.WriteLine($"Received : {peerListBroadcastReceivedData}");
                    // Close connection
                    peer.Close();
                    Console.WriteLine($"Broadcasted peer list data to {peerItem.ExtIp}:{peerItem.Port}");
                    // Handle response - Peer list data
                    try
                    {
                        // Check whether current node received a peer list to merge into local list
                        List<PeerDetails> upstreamPeerDetailsList = JsonSerializer.Deserialize<List<PeerDetails>>(
                            peerListBroadcastReceivedData,
                            options: new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true,
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                            });
                        // Merge / append / write new peer list to local Peers.json
                        DiscoveryManager discoveryManager = new DiscoveryManager();
                        List<PeerDetails> mergedList = DiscoveryManager.MergePeerLists(upstreamPeerDetailsList,
                            discoveryManager.LoadPeerDetails("local/Peers/Peers.json"));
                        discoveryManager.WritePeerListToFile(mergedList, "local/Peers/Peers.json");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Peer List Broadcast Error: {e}");
                    }
                    
                }
                lastBroadcast = System.DateTime.Now;
            }
        }
        
        /// <summary>
        /// Searches for a suitable peer to download a full ZRD state copy from
        /// Then establishes a TCP connection to the peer and asks for state data
        /// The state data is then deserialized to a Blockchain instance, validated and then saved locally
        /// The node Blockchain member is updated with the data from upstream 
        /// </summary>
        /// <exception cref="CryptographicException"></exception>
        /// <exception cref="JsonException"></exception>
        public void DownloadBlockchainFromPeer()
        {
            // Discover a MINER or FULL peer to ask for a copy of the ZRD Blockchain
            DiscoveryManager peerDiscovery = new DiscoveryManager();
            List<PeerDetails> possiblePeers = peerDiscovery.LoadPeerDetails("local/Peers/Peers.json");
            PeerDetails suitablePeer = peerDiscovery.FindSuitablePeerInList("FULL MINER", possiblePeers, false);
            
            // Use found peer details to connect to it and ask for blockchain copy
            // by sending "GET BLOCKCHAIN_FOR_INIT" operation
            FullNodeTcpClient peerClient = new FullNodeTcpClient();
            peerClient.Init(suitablePeer.ExtIp, suitablePeer.Port);
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
        
        /// <summary>
        /// Starts the listener on PRIVATE_IP:PORT to accept incoming TCP connections
        /// </summary>
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
            if (newPort is < 1 or > 65535)
            {
                throw new ArgumentOutOfRangeException("Port number should be between 1 and 65535");
            }
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