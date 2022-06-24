using System;
using Peer2PeerNS.NodesNS.LightweightNodeNS;

namespace Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS
{
    public static class WalletGateway
    {
        public static void Run(LightweightNode node)
        {
    
            Console.WriteLine(
                "============================================================\n" +
                "=                  ZRD Blockchain - Wallet                 =\n" +
                "============================================================\n" +
                $"   Address : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                $"   Wallet Name : {node.Wallet.GetWalletName()}\n" +
                // $"   Address : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                "============================================================\n" +
                "   1. Send ZRD420 Coin\n" +
                "   2. Receive ZRD420 Coin\n" +
                "   3. Change Wallet Name\n" +
                "   0. Exit\n" +
                "============================================================\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("============================================================\n");
            while (true)
            {
                switch (option)
                {
                    case "1":
                        break;
                    case "2":
                        Console.WriteLine(
                            "============================================================\n" +
                            "=                  ZRD Blockchain - Wallet                 =\n" +
                            "============================================================\n" +
                            "   Other Wallets on the ZRD Blockchain can send you ZRD420  \n" +
                            "   via the address below.\n" +
                            "   Share your public address below to receive ZRD420.\n" +
                            "============================================================\n" +
                            "   Public Address:\n" +
                            $"  {node.Wallet.GetPublicKeyStringBase64()}\n" +
                            "============================================================\n"
                        );
                        break;
                    case "3":
                        Console.Write("New Wallet Name : ");
                        var name = Console.ReadLine();
                        node.Wallet.SetWalletName(name);
                        node.Wallet.SaveToJsonFile(@"../../../local/Wallet/Wallet.json", node.Wallet.GetJsonString());
                        break;
                    case "0":
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine($"Option {option} not available\n");
                        break;
                }
                Console.WriteLine(
                    "============================================================\n" +
                    "=                  ZRD Blockchain - Wallet                 =\n" +
                    "============================================================\n" +
                    $"   Address : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                    $"   Wallet Name : {node.Wallet.GetWalletName()}\n" +
                    // $"   Address : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                    "============================================================\n" +
                    "   1. Send ZRD420 Coin\n" +
                    "   2. Receive ZRD420 Coin\n" +
                    "   3. Change Wallet Name\n" +
                    "   0. Exit\n" +
                    "============================================================\n");
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("--------------------------------------------------------------------------------\n");
            }
        }
    }
}