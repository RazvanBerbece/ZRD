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
            testBlock.SetHash();

            // Standard Output
            Console.WriteLine(String.Format("Built testBlock with hash: {0}\n", testBlock.hash));
            Console.WriteLine("Transactions under testBlock: \n");
            foreach (Transaction transaction in testBlockTransactions)
            {
                Console.WriteLine(String.Format("From: {0}\nTo: {1}\n", transaction.Sender, transaction.Receiver));
                Console.WriteLine(String.Format("Transaction {0} with hash {1}\n", transaction.id, transaction.hash));
            }
        }
    }
}
