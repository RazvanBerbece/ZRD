using System;
using System.IO;
using Peer2PeerNS.CmdClientNS.CreateMinerWalletNS;
using Peer2PeerNS.CmdClientNS.MinerNodeOnboardNS;
using Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS;
using WalletNS;

namespace Peer2PeerNS.CmdClientNS.MinerWalletOnboardNS
{
    public class MinerWalletOnboard
    {
        public static void Run(MinerNode node, int port)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=            ZRD Blockchain - Miner Wallet Onboard         =\n" +
                "============================================================\n" +
                "   You need to configure a Miner Wallet or login into one \n" +
                "   to receive miner rewards and transaction fees.\n" +
                "   Create a Wallet or connect to your Wallet.\n" +
                "============================================================\n" +
                "   1. Create Wallet\n" +
                "   2. Login into Wallet\n" +
                "   0. Exit\n" +
                "============================================================\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    CreateMinerWallet.Run(node, port);
                    break;
                case "2":
                    // TODO: Access wallet using master password ?
                    try
                    {
                        // Create Wallet object with existing pair and name from file
                        Wallet loggedOnWallet =
                            Wallet.DeserializeWalletFromJsonFile("local/Wallet/Wallet.json");
                        node.SetMinerWallet(loggedOnWallet);
                        MinerNodeOnboard.Run(node, port);
                    }
                    catch (FileNotFoundException)
                    {
                        // local/Wallet/Wallet.json data file not found
                        Console.WriteLine(
                            "============================================================\n" +
                            "=                  ZRD Blockchain - Onboard                =\n" +
                            "============================================================\n" +
                            "   The Wallet config file was not found \n" +
                            "   in the local/Wallet/ directory.\n" +
                            "   Create a Wallet or recover your previous Wallet.\n" +
                            "============================================================\n" +
                            "   1. Create Wallet\n" +
                            "   2. Recover Wallet\n" +
                            "   0. Exit\n" +
                            "============================================================\n");
                            Console.Write("Option: ");
                        option = Console.ReadLine();
                        switch (option)
                        {
                            case "1":
                                CreateMinerWallet.Run(node, port);
                                break;
                            case "2":
                                // TODO: Recover Wallet workflow
                                Console.WriteLine("Recover Wallet workflow not available at the moment.\n");
                                break;
                            case "0":
                                Environment.Exit(1);
                                break;
                            default:
                                Console.WriteLine($"Option {option} not available\n");
                                break;
                        }
                    }
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