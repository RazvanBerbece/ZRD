using System;
using WalletNS;

namespace ZRD.Peer2Peer.CmdClient
{
    public static class CreateWallet
    {
        public static void Run()
        {
            Console.WriteLine(
                "\nZRD Wallet Creation\n" +
                "--------------------------------------------------------------------------------\n" +
                "Choose one of the following options to continue :\n" +
                "\t1. Generate Key Pair\n" +
                "\t0. Exit\n");
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("--------------------------------------------------------------------------------\n");
            switch (option)
            {
                case "1":
                    Wallet userWallet = new Wallet(1024);
                    Console.WriteLine(
                        "Your secure wallet has been created with the generated RSA 1024-bit keypair.\n" +
                        "The keys below should be securely stored.\n\n" +
                        $"Wallet Public Key : {userWallet.GetPublicKeyStringBase64()}\n" +
                        $"Wallet Private Key : {userWallet.GetPrivateKeyStringBase64()}");
                    // Save wallet details locally
                    WalletGateway.Run();
                    break;
                case "0":
                    Environment.Exit(1);
                    break;
            }
        }
    }
}