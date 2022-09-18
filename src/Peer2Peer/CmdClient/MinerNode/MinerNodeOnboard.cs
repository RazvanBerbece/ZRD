using System;
using BlockchainNS;
using Peer2PeerNS.CmdClientNS.InitialMinerBlockchainDownloadNS;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS;

namespace Peer2PeerNS.CmdClientNS.MinerNodeOnboardNS
{
    public static class MinerNodeOnboard
    {
        public static void Run(MinerNode node, int port)
        {
            
            // Standard port range for Miner Nodes : 430-440
            try
            {
                node.SetPort(port);
            }
            catch (ArgumentOutOfRangeException) { }

            // Set Node blockchain instance to existing state in local/Blockchain/ZRD.json
            string intro = "You will first need to download a full copy of the blockchain from a peer node";
            bool loadedFromLocal = false;
            Blockchain blockchainFromStateFile = Blockchain.FileJsonStringToBlockchainInstance("local/Blockchain/ZRD.json");
            if (blockchainFromStateFile != null)
            {
                blockchainFromStateFile.SetFilepathToState("local/Blockchain/ZRD.json");
                node.SetBlockchain(blockchainFromStateFile);
                intro = "Successfully loaded ZRD state from local/Blockchain/ZRD.json";
                loadedFromLocal = true;
                try
                {
                    node.StoreMinerNodeDetailsInPeersList();
                }
                catch (DuplicatePeerDetailInListException)
                {
                    Console.WriteLine("Current node networking config is already in Peers.json file");
                }
            }
            else
            {
                Console.WriteLine("Failed to load ZRD state from local/Blockchain/ZRD.json. File could not be found.\n");
            }

            if (loadedFromLocal)
            {
                Console.WriteLine(
                    "============================================================\n" +
                    "=              ZRD Blockchain - Miner Node Setup           =\n" +
                    "============================================================\n" +
                    $"  {intro}\n" +
                    "   You can set up the Miner node to receive new transactions for the mempool" +
                    " or broadcast the new Blockchain state outbound to peers.\n" +
                    "==================================================================================\n" +
                    "   Choose one of the following options to continue :\n" +
                    "       2. Set up Node Broadcasting Client\n" +
                    "       3. Set up Transaction mempool mining & Blockchain sync server\n" +
                    "       0. Exit\n");   
            }
            else
            {
                Console.WriteLine(
                    "============================================================\n" +
                    "=              ZRD Blockchain - Miner Node Setup           =\n" +
                    "============================================================\n" +
                    $"  {intro}\n" +
                    "   You need a full copy of the ZRD Blockchain in order to run a Miner node." +
                    "   You can download a full ZRD copy through the menu below.\n" +
                    "==================================================================================\n" +
                    "   Choose one of the following options to continue :\n" +
                    "       1. Download ZRD Chain Copy\n" +
                    "       0. Exit\n");
            }
            Console.Write("Option: ");
            var option = Console.ReadLine();
            Console.Write("==================================================================================\n");

            bool loadedFromPeer = false;
            while (true)
            {
                switch (option)
                {
                    case "1":
                        try
                        {
                            InitialMinerBlockchainDownload.Run(node);
                            intro = "Successfully loaded ZRD state from peer";
                            loadedFromPeer = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Could not download initial ZRD state from peer: {e}");
                        }
                        break;
                    case "2":
                        // Broadcast.Run(node);
                        break;
                    case "3":
                        Console.WriteLine(
                            "=              ZRD Blockchain - Miner Node Server          =\n" +
                            "============================================================\n" +
                            $"   Listening on port {port} for incoming Blockchain connections...\n" +
                            "==================================================================================\n" +
                            "   Press CTRL+C to stop the server.\n" +
                            "==================================================================================\n"); 
                        node.GetBlockchainsFromPeers();
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
                        "============================================================\n" +
                        "=              ZRD Blockchain - Miner Node Setup           =\n" +
                        "============================================================\n" +
                        $"  {intro}\n" +
                        "   You can set up the Miner node to receive new transactions for the mempool" +
                        " or broadcast the new Blockchain state outbound to peers.\n" +
                        "==================================================================================\n" +
                        "   Choose one of the following options to continue :\n" +
                        "       2. Set up Node Broadcasting Client\n" +
                        "       3. Set up Transaction mempool mining & Blockchain sync server\n" +
                        "       0. Exit\n");    
                }
                else
                {
                    Console.WriteLine(
                    "============================================================\n" +
                    "=              ZRD Blockchain - Miner Node Setup           =\n" +
                    "============================================================\n" +
                    $"  {intro}\n" +
                    "   You need a full copy of the ZRD Blockchain in order to run a Miner node." +
                    "   You can download a full ZRD copy through the menu below.\n" +
                    "==================================================================================\n" +
                    "   Choose one of the following options to continue :\n" +
                    "       1. Download ZRD Chain Copy\n" +
                    "       0. Exit\n");
                }
                Console.Write("Option: ");
                option = Console.ReadLine();
                Console.Write("==================================================================================\n");
            }
        }
    }
}