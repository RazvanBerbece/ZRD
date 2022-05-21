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
            int difficulty = 1;
            Blockchain blockchain = Blockchain.CreateBlockchain(difficulty);

            // Add new block to chain
            List<Transaction> testBlockTransactions = new List<Transaction> { };
            testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5);
            Block testBlock = new Block(testBlockTransactions, blockchain.chain.Last.Value.hash, blockchain.chain.Last.Value.index + 1);
            blockchain.AddBlock(testBlock);

            // Visualise blockchain
            blockchain.ViewChain();
            Console.WriteLine($"Blockchain is {(blockchain.IsValid() ? "VALID" : "COMPROMISED")}\n");

        }
    }
}
