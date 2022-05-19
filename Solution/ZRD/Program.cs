using System;
using ZRD.Classes.Transaction;
using System.Collections.Generic;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running ZRD blockchain.\n");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            List<Transaction> testBlockTransactions = new List<Transaction> { };
            testBlockTransactions.Add(new Transaction("SenderPublicKey", "ReceiverPublicKey", 1000));
            testBlockTransactions.Add(new Transaction("SenderPublicKey", "ReceiverPublicKey", 1200));
            testBlockTransactions.Add(new Transaction("SenderPublicKey", "ReceiverPublicKey", 1400));

            // Serialize Transaction to JSON string
            Block testBlock = new Block(testBlockTransactions, "PreviousHash");
            testBlock.SetHash();

            // Standard Output
            Console.WriteLine(String.Format("Built testBlock with hash: {0}\n\n", testBlock.hash));
            Console.WriteLine("Transactions under testBlock: \n");
            foreach (Transaction transaction in testBlockTransactions)
            {
                string transactionId = transaction.id;
                string transactionHash = transaction.hash;
                Console.WriteLine(String.Format("Transaction {0} with hash {1}\n", transactionId, transactionHash));
            }
        }
    }
}
