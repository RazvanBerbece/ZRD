using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using BlockchainNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.FullNodeTcpClientNS;
using StaticsNS;
using WalletNS;
using Peer2PeerNS.NodesNS.Abstract;
using DiscoveryManager = Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS.DiscoveryManager;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS
{
    public class FullNode: INode
    {
        
        // Core
        public Blockchain Blockchain;
        public Wallet NetworkWallet;
        
        // Networking
        private IPAddress privateIpAddress;
        private IPAddress publicNatIpAddress;
        private int port;

        private FullNode() { }
        
        /// <summary>
        /// Configures a lightweight node on the user machine
        /// Config consists of :
        ///     - setting node IP address with EXT Public IP
        ///     - syncing local blockchain data with data from upstream
        ///     - 
        /// </summary>
        /// <returns></returns>
        public static FullNode ConfigureNode()
        {
            FullNode node = new FullNode();
            node.SetPublicNatIpAddress(Statics.GetExternalPublicIpAddress());
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

        public void DownloadBlockchainFromPeer()
        {
            // Discover a MINER or FULL peer to ask for a copy of the ZRD Blockchain
            DiscoveryManager peerDiscovery = new DiscoveryManager();
            List<PeerDetails> possiblePeers = peerDiscovery.LoadPeerDetails(@"../../../local/Blockchain/ZRD.json");
            PeerDetails suitablePeer = peerDiscovery.FindSuitablePeerInList("FULL MINER", possiblePeers);
            
            // Use found peer details to connect to it and ask for blockchain copy
            // by sending "DOWNLOAD_BLOCKCHAIN_FOR_INIT" operation
            FullNodeTcpClient peerClient = new FullNodeTcpClient();
            peerClient.Init(suitablePeer.GetExtIp(), suitablePeer.GetPort());
            string response = peerClient.SendDataStringToPeer("DOWNLOAD_BLOCKCHAIN_FOR_INIT", peerClient.Connect());
            
            // Handle response: Deserialize received JSON Blockchain to actual instance
            Blockchain upstreamBlockchain = Blockchain.JsonStringToBlockchainInstance(response);
            if (upstreamBlockchain is not null)
            {
                if (upstreamBlockchain.IsValid())
                {
                    SetBlockchain(upstreamBlockchain);
                    SetWallet(upstreamBlockchain.BlockchainWallet);
                    Blockchain.SaveJsonStateToFile(this.Blockchain.ToJsonString(), @"../../../local/Blockchain/ZRD.json");
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
        
        public void SetWallet(Wallet newWallet)
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
                "../../../local/Peers/Peers.json");
        }

    }
}