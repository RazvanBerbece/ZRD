using System;
using ZRD.Classes.Transaction;
using ZRD.Classes.Statics;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running ZRD blockchain.\n");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            Transaction[] transactions = new Transaction[100];
            transactions[0] = new Transaction("SenderPublicKey", "ReceiverPublicKey", 1000);
            transactions[1] = new Transaction("SenderPublicKey", "ReceiverPublicKey", 1200);
            transactions[2] = new Transaction("SenderPublicKey", "ReceiverPublicKey", 1400);

            // Serialize Transaction to JSON string
            Block testBlock = new Block(Statics.TransactionsToJSONString(transactions), "PreviousHash");
            testBlock.SetHash();

            Console.WriteLine(String.Format("Built testBlock with hash: {0}", testBlock.hash));
        }
    }
}
