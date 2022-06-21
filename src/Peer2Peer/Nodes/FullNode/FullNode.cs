using System;
using System.Net;
using BlockchainNS;
using StaticsNS;
using WalletNS;
using Peer2PeerNS.NodesNS.Abstract;

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

        public void SendBlockchainToPeer(IPAddress peerIpAddress)
        {
            throw new NotImplementedException();
        }
        
        public void SetWallet(Wallet userWallet)
        {
            this.NetworkWallet = userWallet;
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