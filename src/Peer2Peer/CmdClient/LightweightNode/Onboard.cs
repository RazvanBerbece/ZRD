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
                "\nWelcome to the ZRD Blockchain!\n" +
                "You need to create a Wallet to be able to use the Blockchain capabilities.\n" +
                $"EXT IP Address: {node.GetIpAddressString()}\n" +
                "--------------------------------------------------------------------------------\n" +
                "Choose one of the following options to continue :\n" +
                "\t1. Create Wallet\n" +
                "\t2. Login into Wallet\n" +
                "\t3. Run Full ZRD Node\n" +
                "\t4. Run Miner ZRD Node\n" +
                "\t0. Exit\n");
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
                            Wallet.DeserializeWalletFromJsonFile(@"../../../local/Wallet/Wallet.json");
                        node.SetWallet(loggedOnWallet);
                        WalletGateway.Run(node);
                    }
                    catch (FileNotFoundException)
                    {
                        // local/Wallet/Wallet.json data file not found
                        Console.WriteLine(
                            "\nThe Wallet config file was not found in the local/Wallet/ directory.\n" +
                            "Create a Wallet or recover your previous Wallet.\n" +
                            "--------------------------------------------------------------------------------\n" +
                            "Choose one of the following options to continue :\n" +
                            "\t1. Create Wallet\n" +
                            "\t2. Recover Wallet\n" +
                            "\t0. Exit\n");
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
                case "3":
                    break;
                case "4":
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