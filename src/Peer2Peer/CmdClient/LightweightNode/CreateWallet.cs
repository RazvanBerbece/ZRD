using System;
using Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using WalletNS;

namespace Peer2PeerNS.CmdClientNS.LightweightNodeNS
{
    public static class CreateWallet
    {
        public static void Run(LightweightNode node)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=              ZRD Blockchain - Wallet Creation            =\n" +
                "============================================================\n" +
                "   Choose one of the options below to continue.\n" +
                "============================================================\n" +
                "   1. Generate Key Pair\n" +
                "   0. Exit\n" +
                "============================================================\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("============================================================\n");
            switch (option)
            {
                case "1":
                    Wallet userWallet = new Wallet(1024);
                    Console.WriteLine(
                        "Your secure wallet has been created with the generated RSA 1024-bit keypair.\n" +
                        "The keys below should be securely stored.\n\n" +
                        $"Wallet Public Key (Address) : {userWallet.GetPublicKeyStringBase64()}\n" +
                        $"Wallet Private Key : {userWallet.GetPrivateKeyStringBase64()}");
                    // Set node wallet data
                    node.SetWallet(userWallet);
                    // Save wallet details locally
                    userWallet.SaveToJsonFile(@"../../../local/Wallet/Wallet.json", userWallet.GetJsonString());
                    // TODO: Download initial Blockchain data from upstream
                    WalletGateway.Run(node);
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