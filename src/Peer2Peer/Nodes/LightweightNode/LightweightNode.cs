using System;
using System.Net;
using BlockchainNS;
using StaticsNS;
using WalletNS;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS
{
    public class LightweightNode
    {
        
        // Core
        public Blockchain Blockchain;
        public Wallet Wallet;
        
        // Networking
        private IPAddress ipAddress;

        public LightweightNode(Blockchain chain, Wallet wallet)
        {
            this.Blockchain = chain;
            this.Wallet = wallet;
        }
        
        /// <summary>
        /// Configures a lightweight node on the user machine
        /// Config consists of :
        ///     - setting node IP address with EXT Public IP
        ///     - syncing local blockchain data with data from upstream
        ///     - 
        /// </summary>
        /// <returns></returns>
        public static LightweightNode ConfigureLightweightNode(Blockchain chain, Wallet wallet)
        {
            LightweightNode node = new LightweightNode(chain, wallet);
            node.SetIpAddress(Statics.GetExternalPublicIpAddress());
            return node;
        }
        
        /// <summary>
        /// Accepts inbound connections from FullNodes and sync blockchain with the one received
        /// Verifies whether the new chain is correct, longer than the current node chain
        ///
        /// Sends current node chain to FullNode if current chain longer than the full node one
        /// </summary>
        /// <param name="newChain">Chain received from upstream</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SyncBlockchainFromUpstream(Blockchain newChain)
        {
            throw new NotImplementedException();
        }

        public void SendBlockchainUpstream(IPAddress upstreamNodeIpAddress)
        {
            throw new NotImplementedException();
        }
        
        public void CreateWallet()
        {
            throw new NotImplementedException();
        }

        public void SetIpAddress(IPAddress newIpAddress)
        {
            this.ipAddress = newIpAddress;
        }
        
        public string GetIpAddressString()
        {
            if (this.ipAddress == null || string.IsNullOrEmpty(this.ipAddress.ToString()))
            {
                return "";
            }
            return this.ipAddress.ToString();
        }

    }
}