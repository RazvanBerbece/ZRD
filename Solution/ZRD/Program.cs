using BlockNS;
using System;
using System.Collections.Generic;
using TransactionNS;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running ZRD blockchain.\n");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            List<Transaction> testBlockTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 5);

            // Create block with generated transactions
            Block testBlock = new Block(testBlockTransactions, "PreviousHash");
            testBlock.hash = testBlock.CalculateHash();

            // Standard Output
            testBlock.Mine(difficulty: 5);
            Console.WriteLine($"Calculated hash {testBlock.hash} with PoW={testBlock.proofOfWork}\n");
        }
    }
}
