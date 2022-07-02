using System;
using Peer2Peer.NodesNS.MinerNodeNS;
using Peer2PeerNS.CmdClientNS.FullNodeNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using ZRD.Peer2Peer.CmdClientNS.LightweightNodeNS;

namespace Peer2PeerNS.CmdClientNS.CmdUIGateway
{
    public static class CmdUIGateway
    {
        public static void Run(LightweightNode lightweightNode, FullNode fullNode, MinerNode minerNode)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=                       ZRD Blockchain                     =\n" +
                "============================================================\n" +
                "   Welcome to the ZRD Blockchain.\n" +
                "   Open a ZRD client by picking an option below.\n" +
                "============================================================\n" +
                "   1. Lightweight Node (*Wallet App*)\n" +
                "   2. Full Node Client (*Network Intensive*)\n" +
                "   3. Miner Node Client (*CPU Intensive*)\n" +
                "   0. Exit\n" +
                "============================================================\n"
            );
            Console.Write("   Option: ");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Onboard.Run(lightweightNode);
                    break;
                case "2":
                    while (true)
                    {
                        Console.Write("   Open full node instance on port: ");
                        option = Console.ReadLine();
                        try
                        {
                            int port = Int32.Parse(option);
                            FullNodeOnboard.Run(fullNode, port);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e is FormatException or ArgumentOutOfRangeException
                                ? "   Given port is not valid. The port has to be a number between 1 and 65535.\n"
                                : $"Error occured while setting up FullNode: {e}");
                            if (e is not FormatException or ArgumentOutOfRangeException)
                            {
                                int port = Int32.Parse(option);
                                FullNodeOnboard.Run(fullNode, port);
                            }
                        }
                    }
                    break;
                case "3":
                    // TODO: Miner node UI
                    break;
                case "0":
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine($"Option {option} not available\n");
                    break;
            }
        }
    }
}