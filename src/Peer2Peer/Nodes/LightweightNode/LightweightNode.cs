using System;
using System.Net;
using BlockchainNS;
using Peer2PeerNS.NodesNS.Abstract;
using StaticsNS;
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
            return node;
        }
        
        public void SendBlockchainToPeer(string peerIpAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void SetBlockchain(Blockchain upstreamChain)
        {
            throw new NotImplementedException();
        }
        
        public void SetWallet(Wallet userWallet)
        {
            this.Wallet = userWallet;
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