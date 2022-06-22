using System;
using Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using WalletNS;

namespace Peer2PeerNS.CmdClientNS.FullNodeNS
{
    public static class FullNodeOnboard
    {
        public static void Run(FullNode node)
        {
            Console.WriteLine(
                "\nZRD Full Node Setup\n" +
                "You will first need to download a full copy of the blockchain from a peer node\n" +
                "Then, you can set up the server node to receive new transactions for the mempool " +
                ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                "--------------------------------------------------------------------------------\n" +
                "Choose one of the following options to continue :\n" +
                "\t1. Download ZRD Chain Copy\n" +
                "\t2. Set up broadcast client\n" +
                "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                "\t0. Exit\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("--------------------------------------------------------------------------------\n");
            while (true)
            {
                switch (option)
                {
                    case "1":
                        InitialBlockchainDownload.Run(node);
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "0":
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine($"Option {option} not available\n");
                        break;
                }
                Console.WriteLine(
                    "\nZRD Full Node Setup\n" +
                    "You will first need to download a full copy of the blockchain from a peer node\n" +
                    "Then, you can set up the server node to receive new transactions for the mempool " +
                    ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                    "--------------------------------------------------------------------------------\n" +
                    "Choose one of the following options to continue :\n" +
                    "\t1. Download ZRD Chain Copy\n" +
                    "\t2. Set up broadcast client\n" +
                    "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                    "\t0. Exit\n");
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("--------------------------------------------------------------------------------\n");
            }
        }
    }
}