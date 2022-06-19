using System;
using WalletNS;

namespace ZRD.Peer2Peer.CmdClient
{
    public static class WalletGateway
    {
        public static void Run()
        {
            Console.WriteLine(
                "\nZRD Wallet Gateway\n" +
                $"Public Key : PLACEHOLDER\n" +
                "--------------------------------------------------------------------------------\n" +
                "Choose one of the following options to continue :\n" +
                "\t1. Send Currency\n" +
                "\t2. Get Balance\n" +
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
                    case "0":
                        Environment.Exit(1);
                        break;
                }
                Console.WriteLine(
                    "\nZRD Wallet Gateway\n" +
                    $"Public Key : PLACEHOLDER\n" +
                    "--------------------------------------------------------------------------------\n" +
                    "Choose one of the following options to continue :\n" +
                    "\t1. Send Currency\n" +
                    "\t2. Get Balance\n" +
                    "\t0. Exit\n");
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("--------------------------------------------------------------------------------\n");
            }
        }
    }
}