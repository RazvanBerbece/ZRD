using System;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.CmdClientNS.FullNodeNS
{
    public static class InitialBlockchainDownload
    {
        public static void Run(FullNode node)
        {
            Console.WriteLine(
                "\nZRD Blockchain Download from Peer\n" +
                "Looking for a peer to download a full ZRD copy of the Blockchain from ...\n" +
                "--------------------------------------------------------------------------------\n");
            node.DownloadBlockchainFromPeer();
            try
            {
                node.StoreFullNodeDetailsInPeersList();
            }
            catch (DuplicatePeerDetailInListException)
            {
                Console.WriteLine("Current node networking config is already in Peers.json file");
            }
        }
    }
}