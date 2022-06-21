using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        private LightweightNode() { }
        
        /// <summary>
        /// Configures a lightweight node on the user machine
        /// Config consists of :
        ///     - setting node IP address with EXT Public IP
        ///     - syncing local blockchain data with data from upstream
        ///     - 
        /// </summary>
        /// <returns></returns>
        public static LightweightNode ConfigureLightweightNode()
        {
            LightweightNode node = new LightweightNode();
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
        public void SyncBlockchainFromUpstream()
        {
            try
            {
                TcpListener listener = new TcpListener(420);
                TcpClient externalPeer = default(TcpClient);
                listener.Start();
                Console.WriteLine("Lightweight Node listener waiting for new Blockchain from upstream... Press ^C to Stop...");  
                externalPeer = listener.AcceptTcpClient();
                NetworkStream stream = externalPeer.GetStream();
                while (true)
                {
                    while (!stream.DataAvailable);
                    Byte[] bytes = new Byte[externalPeer.Available];
                    stream.Read(bytes, 0, bytes.Length);
                    // Translate bytes of request to string
                    String data = Encoding.UTF8.GetString(bytes);
                    // TODO: Add Blockchain deserialization from received bytes
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An Exception Occurred while Listening : {e}");  
            }
        }

        public void SendBlockchainUpstream(IPAddress upstreamNodeIpAddress)
        {
            throw new NotImplementedException();
        }
        
        public void SetWallet(Wallet userWallet)
        {
            this.Wallet = userWallet;
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