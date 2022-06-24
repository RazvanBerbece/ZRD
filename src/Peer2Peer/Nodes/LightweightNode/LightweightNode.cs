using System;
using System.Net;
using System.Net.Sockets;
using BlockchainNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.FullNodeTcpClientNS;
using Peer2PeerNS.NodesNS.Abstract;
using StaticsNS;
using TransactionNS;
using WalletNS;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS
{
    public class LightweightNode: INode
    {
        
        // Core
        public Blockchain Blockchain;
        public Wallet Wallet;
        
        // Networking
        private IPAddress privateIpAddress;
        private IPAddress publicNatIpAddress;

        private LightweightNode() { }
        
        /// <summary>
        /// Configures a lightweight node on the user machine
        /// Config consists of :
        ///     - setting node IP address with EXT Public IP
        ///     - syncing local blockchain data with data from upstream
        ///     - 
        /// </summary>
        /// <returns></returns>
        public static LightweightNode ConfigureNode()
        {
            LightweightNode node = new LightweightNode();
            node.SetPublicNatIpAddress(Statics.GetExternalPublicIpAddress());
            node.SetPrivateIpAddress(Statics.GetLocalIpAddress());
            return node;
        }
        
        public void SendBlockchainToPeer(PeerDetails peerDetails)
        {
            throw new NotImplementedException();
        }

        public void SetBlockchain(Blockchain upstreamChain)
        {
            this.Blockchain = upstreamChain;
        }
        
        public void SetWallet(Wallet userWallet)
        {
            this.Wallet = userWallet;
        }
        
        public void SendTransactionToPeer(Transaction transaction, PeerDetails peerDetails)
        {
            if (this.Wallet == null)
            {
                throw new Exception("Lightweight node cannot send transaction before a wallet is configured");
            }
            // Init TcpClient
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(peerDetails.ExtIp, peerDetails.Port);
            // Connect to peer
            NetworkStream stream = peer.Connect();
            string peerResponse = peer.SendDataStringToPeer(transaction.ToJsonString(), stream);
            // Handle response from peer
            // TODO
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

    }
}