using BlockNS;
using System;
using System.Collections.Generic;
using TransactionNS;
using BlockchainNS;
using System.Linq;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running ZRD blockchain.\n");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            // Create Blockchain instance
            Blockchain blockchain = Blockchain.CreateBlockchain(
                difficulty: 2,
                blockTime: 5
            );

            // Add new blocks to chain
            List<Transaction> testBlockTransactions = new List<Transaction> { };

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5);
            Block testBlock1 = new Block(testBlockTransactions, blockchain.chain.Last.Value.hash, blockchain.chain.Last.Value.index + 1);
            blockchain.AddBlock(testBlock1);

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5);
            Block testBlock2 = new Block(testBlockTransactions, blockchain.chain.Last.Value.hash, blockchain.chain.Last.Value.index + 1);
            blockchain.AddBlock(testBlock2);

            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5);
            Block testBlock3 = new Block(testBlockTransactions, blockchain.chain.Last.Value.hash, blockchain.chain.Last.Value.index + 1);
            blockchain.AddBlock(testBlock3);

            // Visualise blockchain
            blockchain.ViewChain();
            Console.WriteLine($"\nBlockchain is {(blockchain.IsValid() ? "VALID" : "COMPROMISED")}\n");

            // Mutate Block -> Blockchain is compromised
            // blockchain.chain.Last.Value.data[0].Amount += 150;
            // Console.WriteLine($"\nBlockchain is {(blockchain.IsValid() ? "VALID" : "COMPROMISED")}\n");

        }
    }
}
