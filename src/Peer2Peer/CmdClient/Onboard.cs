using System;

namespace ZRD.Peer2Peer.CmdClient
{
    public static class Onboard
    {
        public static void Run()
        {
            Console.WriteLine(
                "\nWelcome to the ZRD Blockchain!\n" +
                "You need to create a Wallet to be able to use the Blockchain capabilities.\n" +
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
                    CreateWallet.Run();
                    break;
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "0":
                    Environment.Exit(1);
                    break;
            }
        }
    }
}