using System;
using System.Net;
using System.Net.Sockets;
using BlockchainNS;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.FullNodeTcpClientNS;
using Peer2PeerNS.NodesNS.Abstract;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.DataOutTypeNS;
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
        ///     - setting node public IP address with EXT Public IP
        ///     - syncing local blockchain data with data from upstream TODO: Consider this
        ///     - setting node private IP address with local IP
        /// </summary>
        /// <returns>pre-configured instance of LightweightNode</returns>
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
        
        /// <summary>
        /// Sends the passed transaction parameter as byte[] to a suitable peer
        /// Also handles possible responses from peer
        /// </summary>
        /// <param name="transaction"></param>
        /// <exception cref="Exception"></exception>
        public void SendTransactionToPeer(Transaction transaction)
        {
            if (this.Wallet == null)
            {
                throw new Exception("Lightweight node cannot send transaction before a wallet is configured");
            }
            // Find suitable FULL node peer to send new transaction to
            DiscoveryManager discoveryManager = new DiscoveryManager();
            PeerDetails suitablePeerDetails = discoveryManager.FindSuitablePeerInList("FULL",
                discoveryManager.LoadPeerDetails("local/Peers/Peers.json"), true);
            // Init TcpClient
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(suitablePeerDetails.ExtIp, suitablePeerDetails.Port);
            // Connect to peer
            NetworkStream stream = peer.Connect();
            var peerResponse = peer.SendDataStringToPeer(transaction.ToJsonString(), stream, DataOutType.Transaction);
            // Handle response from peer
            if (peerResponse.Equals("Transaction successfully added to peer mempool"))
            {
                // send success
                Console.WriteLine(peerResponse);
            }
            else
            {
                // send failure
                Console.WriteLine($"Transaction not added to mempool: {peerResponse}");
            }
        }

        public int GetWalletBalanceFromPeer()
        {
            if (this.Wallet == null)
            {
                throw new Exception("Lightweight node cannot send transaction before a wallet is configured");
            }
            // Find suitable FULL node peer to get wallet balance from
            // Will most of the time try the first ever FULL node in the peer list 
            DiscoveryManager discoveryManager = new DiscoveryManager();
            try
            {
                PeerDetails suitablePeerDetails = discoveryManager.FindSuitablePeerInList("FULL",
                    discoveryManager.LoadPeerDetails("local/Peers/Peers.json"), true);
                // Init TcpClient
                FullNodeTcpClient peer = new FullNodeTcpClient();
                peer.Init(suitablePeerDetails.ExtIp, suitablePeerDetails.Port);
                // Connect to peer
                NetworkStream stream = peer.Connect();
                dynamic peerResponse = peer.SendDataStringToPeer($"GET BALANCE {this.Wallet.GetPublicKeyStringBase64()}", stream, DataOutType.WalletBalanceRequest);
                peer.Close();
                // Handle response from peer
                if (peerResponse == null)
                {
                    return -1;
                }
                return peerResponse;
            }
            catch (PeerNotFoundInListException)
            {
                Console.WriteLine("A suitable FULL node peer was not found to get wallet balance from");
                return -1;
            }
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