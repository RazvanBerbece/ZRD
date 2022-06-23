using System;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.CmdClientNS.FullNodeNS
{
    public static class Broadcast
    {
        public static void Run(FullNode node)
        {
            Console.WriteLine(
                "\nZRD Blockchain Broadcast Client\n" +
                $"Searching for peers to share ZRD version and peer list with ... \n" +
                "--------------------------------------------------------------------------------\n");
            node.Broadcast();
        }
    }
}