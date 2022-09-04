using System;
using Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS;
using Peer2PeerNS.CmdClientNS.MinerGatewayNS;
using Peer2PeerNS.CmdClientNS.MinerNodeOnboardNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS;
using WalletNS;

namespace Peer2PeerNS.CmdClientNS.CreateMinerWalletNS
{
    public static class CreateMinerWallet
    {
        public static void Run(MinerNode node, int port)
        {
            Console.WriteLine(
                "============================================================\n" +
                "=            ZRD Blockchain - Miner Wallet Creation        =\n" +
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
                    Wallet userWallet = new Wallet(1024, "local/Wallet/MinerWallet/Params/RSAConfig.xml");
                    Console.WriteLine(
                        "Your secure wallet has been created with the generated RSA 1024-bit keypair.\n" +
                        "The keys below should be securely stored.\n\n" +
                        $"Wallet Public Key (Address) : {userWallet.GetPublicKeyStringBase64()}\n" +
                        $"Wallet Private Key : {userWallet.GetPrivateKeyStringBase64()}");
                    // Set node wallet data
                    node.SetMinerWallet(userWallet);
                    // Save wallet details locally
                    userWallet.SaveToJsonFile("local/Wallet/MinerWallet/Wallet.json", userWallet.GetJsonString());
                    // TODO: Download initial Blockchain data from upstream
                    MinerNodeOnboard.Run(node, port);
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