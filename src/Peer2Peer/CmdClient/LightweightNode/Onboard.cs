using Peer2PeerNS.NodesNS.LightweightNodeNS;
using System;
using System.IO;
using Peer2PeerNS.CmdClientNS.LightweightNodeNS;
using Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS;
using WalletNS;

namespace ZRD.Peer2Peer.CmdClientNS.LightweightNodeNS
{
    public static class Onboard
    {
        public static void Run(LightweightNode node)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=                  ZRD Blockchain - Onboard                =\n" +
                "============================================================\n" +
                "   Welcome to the ZRD Blockchain Wallet Onboarding!\n" +
                "   You need to create a Wallet or login into one\n" +
                "   to be able to use the Blockchain capabilities.\n" +
                $"  EXT IP Address: {node.GetPublicNatIpAddressString()}\n" +
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
                    CreateWallet.Run(node);
                    break;
                case "2":
                    // TODO: Access wallet using master password ?
                    try
                    {
                        // Create Wallet object with existing pair and name from file
                        Wallet loggedOnWallet =
                            Wallet.DeserializeWalletFromJsonFile("local/Wallet/Wallet.json");
                        node.SetWallet(loggedOnWallet);
                        WalletGateway.Run(node);
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
                                CreateWallet.Run(node);
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