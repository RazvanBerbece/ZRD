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

namespace ZRD.Classes.Transaction
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
            this.hash = Statics.Statics.CreateHashSHA256(concatenatedData);
        }
    }
}
