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
using WalletNS;
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

        public string signature;

        public Transaction(string senderPublicKey, string receiverPublicKey, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Transaction amount cannot be negative or zero");
            }

            if (senderPublicKey.Length == 0 || receiverPublicKey.Length == 0)
            {
                throw new ArgumentException("Sender/Receiver RSA keys cannot be empty");
            }

            this.Sender = senderPublicKey;
            this.Receiver = receiverPublicKey;
            this.Amount = amount;

            this.signature = null; // unsigned at the moment of instantiation
          
            // Generate random version 4 UUID for transaction
            this.id = Guid.NewGuid().ToString();

            // Calculate hash value of transaction
            string concatenatedData = this.Sender + this.Receiver + this.Amount.ToString() + id;
            this.hash = Statics.CreateHashSHA256(concatenatedData);
        }

        /// <summary>
        /// Signs the transaction against a wallet using SHA256 signature.
        /// Verifies that that the publicKey of the wallet matches the transaction sender's publicKey.
        /// </summary>
        /// <param name="wallet">Wallet used to sign the transaction</param>
        public void SignTransaction(Wallet wallet)
        {
            if (wallet == null)
            {
                throw new ArgumentNullException("Cannot sign transaction with null Wallet object");
            }

            if (wallet.GetPublicKeyStringBase64() == this.Sender)
            {
                // Create a UnicodeEncoder to convert between byte array and string.
                ASCIIEncoding ByteConverter = new ASCIIEncoding();

                byte[] originalData = ByteConverter.GetBytes(this.hash);

                // Create instance of RSACryptoServiceProvider using the
                // key from RSAParameters
                try
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(wallet.GetKeyPairParams());

                    byte[] signature = rsa.SignData(originalData, SHA256.Create());
                    this.signature = Convert.ToBase64String(signature);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured in SignTransaction(): {e.Message}\n");
                    return;
                }
            }
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public static List<Transaction> GenerateRandomTransactions(int numberOfTransactions)
        {

            // Throw ArgumentOutOfRangeException if asked to generate a list of 0 or less Transactions
            if (numberOfTransactions <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfTransactions cannot be negative or zero");
            }

            Random randomTransactionAmountEngine = new Random();
            List<Transaction> transactions = new List<Transaction> { };
            for (int i = 0; i < numberOfTransactions; i++)
            {
                // Generate key pairs to simulate party identifiers
                RSACryptoServiceProvider rsaSender = new RSACryptoServiceProvider(1024);
                RSACryptoServiceProvider rsaReceiver = new RSACryptoServiceProvider(1024);

                transactions.Add(new Transaction(
                    rsaSender.ToXmlString(false),
                    rsaReceiver.ToXmlString(false),
                    randomTransactionAmountEngine.Next(1, int.MaxValue))
                );
            }

            return transactions; 
        }
    }

}
