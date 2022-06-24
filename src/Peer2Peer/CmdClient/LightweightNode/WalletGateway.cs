using System;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using TransactionNS;

namespace Peer2PeerNS.CmdClientNS.LightweightNodeNS.WalletGatewayNS
{
    public static class WalletGateway
    {
        public static void Run(LightweightNode node)
        {
    
            Console.WriteLine(
                "============================================================\n" +
                "=                 ZRD Blockchain - Wallet                  =\n" +
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
                        Console.WriteLine(
                            "============================================================\n" +
                            "=                 ZRD Blockchain - Send Coin               =\n" +
                            "============================================================\n" +
                            "   Send ZRD420 coin to another Wallet by inputting their  \n" +
                            "   Wallet address and how much coin to send below.\n" +
                            "============================================================\n"
                        );
                        string remoteWalletAddress;
                        int transactionAmount;
                        Console.Write(" Wallet Address: ");
                        remoteWalletAddress = Console.ReadLine();
                        Console.Write(" Amount (ZRD420): ");
                        while (true)
                        {
                            try
                            {
                                transactionAmount = Int32.Parse(Console.ReadLine());
                                break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"    Transaction amount cannot be negative or greater than {int.MaxValue}\n");
                            }
                        }
                        Console.Write("============================================================\n");
                        Console.Write($"Sending coin to {remoteWalletAddress} ...\n");
                        
                        // Setup Transaction to send to full node
                        Transaction transaction = new Transaction(
                            node.Wallet.GetPublicKeyStringBase64(),
                            remoteWalletAddress,
                            transactionAmount
                            );
                        // Send transaction to peer full node - to be added in mempool
                        node.SendTransactionToPeer(transaction);
                        break;
                    case "2":
                        Console.WriteLine(
                            "============================================================\n" +
                            "=                  ZRD Blockchain - Receive Coin           =\n" +
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
                Console.Write("============================================================\n");
            }
        }
    }
}