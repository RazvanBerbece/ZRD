using System;
using Peer2PeerNS.NodesNS.LightweightNodeNS;

namespace Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS
{
    public static class WalletGateway
    {
        public static void Run(LightweightNode node)
        {
            Console.WriteLine(
                "\nZRD Wallet Gateway\n" +
                "--------------------------------------------------------------------------------\n" +
                $"Public Key : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                $"Wallet Name : {node.Wallet.GetWalletName()}\n" +
                "--------------------------------------------------------------------------------\n" +
                "Choose one of the following options to continue :\n" +
                "\t1. Send Currency\n" +
                "\t2. Get Balance\n" +
                "\t3. Change Wallet Name\n" +
                "\t0. Exit\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("--------------------------------------------------------------------------------\n");
            while (true)
            {
                switch (option)
                {
                    case "1":
                        break;
                    case "2":
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
                    "\nZRD Wallet Gateway\n" +
                    "--------------------------------------------------------------------------------\n" +
                    $"Public Key : {node.Wallet.GetPublicKeyStringBase64()}\n" +
                    $"Wallet Name : {node.Wallet.GetWalletName()}\n" +
                    "--------------------------------------------------------------------------------\n" +
                    "Choose one of the following options to continue :\n" +
                    "\t1. Send Currency\n" +
                    "\t2. Get Balance\n" +
                    "\t3. Change Wallet Name\n" +
                    "\t0. Exit\n");
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("--------------------------------------------------------------------------------\n");
            }
        }
    }
}