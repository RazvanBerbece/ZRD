using System;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.CmdClientNS.FullNodeNS
{
    public static class FullNodeServer
    {
        public static void Run(FullNode node, int port)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=     ZRD Blockchain - Full Node Blockchain Sync Server    =\n" +
                "============================================================\n" +
                $"  Opening {node.GetPrivateIpAddressString()}:{port} for new Transactions and Blockchain syncs ...\n" +
                "========================================================================================================================\n");
            node.StartFullServer();
        }
    }
}