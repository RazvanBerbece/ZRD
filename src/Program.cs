using BlockNS;
using System;
using System.Collections.Generic;
using TransactionNS;
using BlockchainNS;
using Peer2PeerNS.CmdClientNS.FullNodeNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using StaticsNS;
using WalletNS;
using ZRD.Peer2Peer.CmdClientNS.LightweightNodeNS;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Create example nodes
            // LightweightNode lightNode = LightweightNode.ConfigureNode();
            FullNode fullNode = FullNode.ConfigureNode();

            // Onboard - Entry Point to Terminal Blockchain Clients
            // Onboard.Run(lightNode);
            FullNodeOnboard.Run(fullNode);

            Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("ON the ZRD Blockchain...");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            // Create network wallet
            Wallet networkWallet = new Wallet(keySize: 1024);
            networkWallet.SetWalletName("ZRD Network Wallet");
            
            // Create first user wallet & set up
            Wallet antonioWallet = new Wallet(keySize: 1024);

            // Console.WriteLine($"Node with EXT Public IP: {node.GetIpAddressString()}\n");
            Console.WriteLine($"Antonio's Wallet with publicKey: {antonioWallet.GetPublicKeyStringBase64()}\n");

            // Create Blockchain instance
            Blockchain blockchain = Blockchain.CreateBlockchain(
                firstMint: new Transaction(networkWallet.GetPublicKeyStringBase64(), antonioWallet.GetPublicKeyStringBase64(), 1000000),
                blockchainWallet: networkWallet,
                difficulty: 2,
                blockTime: 5,
                reward: 420
            );

            // Add new blocks to chain
            List<Transaction> testBlockTransactions;

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5, true);
            Block testBlock1 = new Block(testBlockTransactions, blockchain.Chain.Last.Value.Hash, blockchain.Chain.Last.Value.Index + 1);
            blockchain.AddBlock(testBlock1);
            Console.WriteLine($"-> Added Block with hash {(testBlock1.Hash)} to the Blockchain !\n");

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5, true);
            Block testBlock2 = new Block(testBlockTransactions, blockchain.Chain.Last.Value.Hash, blockchain.Chain.Last.Value.Index + 1);
            blockchain.AddBlock(testBlock2);
            Console.WriteLine($"-> Added Block with hash {(testBlock2.Hash)} to the Blockchain !\n");

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5, true);
            Block testBlock3 = new Block(testBlockTransactions, blockchain.Chain.Last.Value.Hash, blockchain.Chain.Last.Value.Index + 1);
            blockchain.AddBlock(testBlock3);
            Console.WriteLine($"-> Added Block with hash {(testBlock3.Hash)} to the Blockchain !\n");

            // Visualise blockchain
            // blockchain.ViewChain();
            Console.WriteLine($"Blockchain is {(blockchain.IsValid() ? "VALID" : "COMPROMISED")}\n");

            // Mutate Block -> Blockchain is compromised
            // blockchain.chain.Last.Value.data[0].Amount += 150;
            // Console.WriteLine($"\nBlockchain is {(blockchain.IsValid() ? "VALID" : "COMPROMISED")}\n");

            // Look for balance for AntonioPublicKey
            int balance = blockchain.GetBalance(antonioWallet.GetPublicKeyStringBase64());
            Console.WriteLine($"Amount for key {antonioWallet.GetPublicKeyStringBase64()} : {balance}");
            
            // Save Blockchain to JSON
            Blockchain.SaveJsonStateToFile(blockchain.ToJsonString(), @"../../../local/Blockchain/ZRD.json");
            
            // Import Blockchain from JSON file
            // Blockchain importedBlockchain = Blockchain.DeserializeJsonStateToBlockchainInstance(@"../../../local/Blockchain/ZRD.json");
            // Blockchain.SaveJsonStateToFile(importedBlockchain.ToJsonString(), @"../../../local/Blockchain/ZRD_IMPORT.json");
            
            Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("OFF the ZRD Blockchain.");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

        }
    }
}
