using System;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS;

namespace Peer2PeerNS.CmdClientNS.InitialMinerBlockchainDownloadNS
{
    public static class InitialMinerBlockchainDownload
    {
        public static void Run(MinerNode node)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=       ZRD Blockchain - Blockchain Download from Peer     =\n" +
                "============================================================\n" +
                "   Looking for a peer to download a full ZRD copy of the Blockchain from ...\n" +
                "===============================================================================\n");
            node.DownloadBlockchainFromPeer();
            try
            {
                node.StoreMinerNodeDetailsInPeersList();
            }
            catch (DuplicatePeerDetailInListException)
            {
                Console.WriteLine("Current node networking config is already in Peers.json file");
            }
        }
    }
}