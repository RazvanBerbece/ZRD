using System;
using BlockchainNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.CmdClientNS.FullNodeNS
{
    public static class FullNodeOnboard
    {
        public static void Run(FullNode node, int port)
        {
            // Standard port range for Full Nodes : 420-430
            node.SetPort(port);
            
            // Set Node blockchain instance to existing state in local/Blockchain/ZRD.json
            string intro = "You will first need to download a full copy of the blockchain from a peer node";
            bool loadedFromLocal = false;
            Blockchain blockchainFromStateFile = Blockchain.FileJsonStringToBlockchainInstance("local/Blockchain/ZRD.json");
            if (blockchainFromStateFile != null)
            {
                node.SetBlockchain(blockchainFromStateFile);
                intro = "Successfully loaded ZRD state from local/Blockchain/ZRD.json";
                loadedFromLocal = true;
            }

            if (loadedFromLocal)
            {
                Console.WriteLine(
                    "\nZRD Full Node Setup\n" +
                    $"{intro}\n" +
                    "You can set up the server node to receive new transactions for the mempool" +
                    ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                    "--------------------------------------------------------------------------------\n" +
                    "Choose one of the following options to continue :\n" +
                    "\t2. Set up broadcast client\n" +
                    "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                    "\t0. Exit\n");
            }
            else
            {
                Console.WriteLine(
                    "\nZRD Full Node Setup\n" +
                    $"{intro}\n" +
                    "You can set up the server node to receive new transactions for the mempool" +
                    ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                    "--------------------------------------------------------------------------------\n" +
                    "Choose one of the following options to continue :\n" +
                    "\t1. Download ZRD Chain Copy\n" +
                    "\t2. Set up broadcast client\n" +
                    "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                    "\t0. Exit\n");
            }
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("--------------------------------------------------------------------------------\n");

            bool loadedFromPeer = false;
            while (true)
            {
                switch (option)
                {
                    case "1":
                        try
                        {
                            InitialBlockchainDownload.Run(node);
                            intro = "Successfully loaded ZRD state from peer";
                            loadedFromPeer = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Could not download initial ZRD state from peer: {e}");
                        }
                        break;
                    case "2":
                        Broadcast.Run(node);
                        break;
                    case "3":
                        FullNodeServer.Run(node, port);
                        break;
                    case "0":
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine($"Option {option} not available\n");
                        break;
                }

                if (loadedFromLocal || loadedFromPeer)
                {
                    Console.WriteLine(
                        "\nZRD Full Node Setup\n" +
                        $"{intro}\n" +
                        "You can set up the server node to receive new transactions for the mempool" +
                        ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                        "--------------------------------------------------------------------------------\n" +
                        "Choose one of the following options to continue :\n" +
                        "\t2. Set up broadcast client\n" +
                        "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                        "\t0. Exit\n");   
                }
                else
                {
                    Console.WriteLine(
                        "\nZRD Full Node Setup\n" +
                        $"{intro}\n" +
                        "You can set up the server node to receive new transactions for the mempool" +
                        ", updated ZRD versions from miner nodes or to broadcast network to other nodes.\n" +
                        "--------------------------------------------------------------------------------\n" +
                        "Choose one of the following options to continue :\n" +
                        "\t1. Download ZRD Chain Copy\n" +
                        "\t2. Set up broadcast client\n" +
                        "\t3. Set up Transaction mempool & Blockchain sync server\n" +
                        "\t0. Exit\n");
                }
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("--------------------------------------------------------------------------------\n");
            }
        }
    }
}