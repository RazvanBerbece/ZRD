/**
 * Class which defines a Transaction in the blockchain
 * 
 * Fields :
 *  id          = transaction identifier
 *  sender      = public key of sender
 *  receiver    = public key of receiver
 *  amount      = transaction amount
 *  hash        = hash value of Transaction
 *  
 */

using System;
using StaticsNS;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TransactionNS
{
    public class Transaction
    {

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int Amount { get; set; }

        public string id = "";
        public string hash = "";

        public Transaction(string senderPublicKey, string receiverPublicKey, int amount)
        {
            this.Sender = senderPublicKey;
            this.Receiver = receiverPublicKey;
            this.Amount = amount;

            // Generate random version 4 UUID for transaction
            this.id = Guid.NewGuid().ToString();

            // Calculate hash value of transaction
            string concatenatedData = this.Sender + this.Receiver + this.Amount.ToString() + id;
            this.hash = Statics.CreateHashSHA256(concatenatedData);
        }

        public static List<Transaction> GenerateRandomTransactions(int numberOfTransactions)
        {
            Random randomTransactionAmountEngine = new Random();
            List<Transaction> transactions = new List<Transaction> { };
            for (int i = 0; i < numberOfTransactions; ++i)
            {

                RSACryptoServiceProvider rsaSender = new RSACryptoServiceProvider(1024);
                RSACryptoServiceProvider rsaReceiver = new RSACryptoServiceProvider(1024);

                transactions.Add(new Transaction(
                    rsaSender.ToXmlString(false),
                    rsaReceiver.ToXmlString(false),
                    randomTransactionAmountEngine.Next(1, 999999))
                );
            }
            return transactions; 
        }
    }

}
