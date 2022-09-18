using BlockchainNS;
using DiscoveryManager = Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS.DiscoveryManager;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.FullNodeTcpClientNS;
using Peer2PeerNS.NodesNS.Abstract;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.DataOutTypeNS;
using StaticsNS;
using WalletNS;
using WalletNS.BlockchainWalletNS;
using ZRD.Peer2Peer.TcpConnectivity.MinerNode;

namespace Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS
{
    public class MinerNode: INode
    {
        
        // Core
        public Blockchain Blockchain;
        public BlockchainWallet NetworkWallet;
        public Wallet MinerWallet;
        
        // Networking
        private IPAddress _privateIpAddress;
        private IPAddress _publicNatIpAddress;
        private int _port;

        private MinerNode() { }
        
        public static MinerNode ConfigureNode(string filepathToPeerList = "local/Peers/peers.json")
        {
            MinerNode node = new MinerNode();
            node.SetPrivateIpAddress(Statics.GetLocalIpAddress());
            node.SetPublicNatIpAddress(Statics.GetExternalPublicIpAddress());
            try
            {
                new DiscoveryManager().LoadPeerDetails(filepathToPeerList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not load Peers.json file: {e}");
            }
            return node;
        }
        
        public void SetPrivateIpAddress(IPAddress newPrivateIpAddress)
        {
            _privateIpAddress = newPrivateIpAddress;
        }
        
        public void SetPublicNatIpAddress(IPAddress newPublicNatIpAddress)
        {
            _publicNatIpAddress = newPublicNatIpAddress;
        }
        
        public string GetPrivateIpAddressString()
        {
            if (_privateIpAddress == null || string.IsNullOrEmpty(_privateIpAddress.ToString()))
            {
                return "";
            }
            return _privateIpAddress.ToString();
        }
        
        public string GetPublicNatIpAddressString()
        {
            if (_publicNatIpAddress == null || string.IsNullOrEmpty(_publicNatIpAddress.ToString()))
            {
                return "";
            }
            return _publicNatIpAddress.ToString();
        }

        public void SetPort(int newPort)
        {
            if (newPort is < 1 or > 65535)
            {
                throw new ArgumentOutOfRangeException("Port number should be between 1 and 65535");
            }
            _port = newPort;
        }
        
        public void SetBlockchain(Blockchain chain)
        {
            Blockchain = chain;
        }
        
        private void SetBlockchainWallet(BlockchainWallet newWallet)
        {
            NetworkWallet = newWallet;
        }
        
        public void SetMinerWallet(Wallet newWallet)
        {
            MinerWallet = newWallet;
        }
        
        /// <summary>
        /// Adds current node connection details (ext NAT IP, open port, node type)
        /// to the list of potential peers
        /// </summary>
        public void StoreMinerNodeDetailsInPeersList()
        {
            DiscoveryManager peerDiscovery = new DiscoveryManager();
            peerDiscovery.StoreExtNatIpAndPortToFile(
                _publicNatIpAddress.ToString(), 
                _port, 
                "MINER", 
                "local/Peers/Peers.json"
            );
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
            string response = peerClient.SendDataStringToPeer("GET BLOCKCHAIN_FOR_INIT", peerClient.Connect(), DataOutType.BlockchainInitRequest);
            
            // Handle response: Deserialize received JSON Blockchain to actual instance
            Blockchain upstreamBlockchain = Blockchain.JsonStringToBlockchainInstance(response);
            if (upstreamBlockchain is not null)
            {
                if (upstreamBlockchain.IsValid())
                {
                    
                    // Update Blockchain instance with upstream state
                    SetBlockchain(upstreamBlockchain);

                    // Update Blockchain Wallet with a newly configured instance
                    // from the config file and the upstream state
                    BlockchainWallet blockchainWallet = new BlockchainWallet(
                        Blockchain.BlockchainWallet.GetPublicKeyStringBase64(),
                        Blockchain.BlockchainWallet.WalletName
                    );
                    Blockchain.BlockchainWallet = blockchainWallet;
                    SetBlockchainWallet(blockchainWallet);

                    Blockchain.SaveJsonStateToFile(Blockchain.ToJsonString(), "local/Blockchain/ZRD.json");
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

        public void GetBlockchainsFromPeers()
        {
            try
            {
                var server = new MinerNodeTcpServer().OpenListenerOnPort(_port, this);
                while (true)
                {
                    var prevMempoolCount = this.Blockchain.UnconfirmedTransactions.Count;
                    server
                        .AcceptIncomingConnections()
                        .HandleIncomingData()
                        .CloseConnection();
                    
                    // Check if there are any new unvalidated transactions to mine
                    var currentMempoolCount = this.Blockchain.UnconfirmedTransactions.Count;
                    if (currentMempoolCount > prevMempoolCount)
                    {
                        // Mine transactions
                        Blockchain.MineUnconfirmedTransactions(MinerWallet.GetPublicKeyStringBase64());
                    }
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