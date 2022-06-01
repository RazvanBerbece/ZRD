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

using BlockchainNS;
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

        public string id { get; set; }
        public string hash { get; set; }

        public string signature { get; set; }

        public Transaction(string senderPublicKey, string receiverPublicKey, int amount, string id = null)
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
          
            // Generate random version 4 UUID for transaction if id arg is null
            this.id = id ?? Guid.NewGuid().ToString();

            // Calculate hash value of transaction
            string concatenatedData = id + Sender + Receiver + Amount.ToString();
            this.hash = Statics.CreateHashSHA256(concatenatedData);
        }

        /// <summary>
        /// Signs the transaction against a wallet using SHA256 signature.
        /// Verifies that that the publicKey of the wallet matches the transaction sender's publicKey.
        /// </summary>
        /// <param name="wallet">Wallet used to sign the transaction.</param>
        public void SignTransaction(Wallet wallet)
        {
            if (wallet == null)
            {
                throw new ArgumentNullException("Cannot sign transaction with null Wallet object");
            }

            if (wallet.GetPublicKeyStringBase64() == this.Sender)
            {
                
                byte[] originalData = Convert.FromBase64String(this.hash);

                // Create instance of RSACryptoServiceProvider using the
                // key from RSAParameters
                try
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(wallet.GetKeyPairParams());

                    byte[] signature = rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    this.signature = Convert.ToBase64String(signature);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured in SignTransaction(): {e.Message}\n");
                    return;
                }
            }
        }

        public bool IsValid(Blockchain chain)
        {
            // Because Transactions can't have wrong field values (empty strings, negative values, 0s) by design,
            // here we only have to test that:
            //      - the hash matches (between re-calculated hash using current field values and instance hash)
            //      - the signature is verified with the sender public key
            //      - sender's balance is higher than amount sent
            //

            if (this.Amount > chain.GetBalance(this.Sender))
            {
                return false;
            }

            // Calculate hash value of current transaction
            string concatenatedData = id + this.Sender + this.Receiver + this.Amount.ToString();
            string calculatedHash = Statics.CreateHashSHA256(concatenatedData);
            if (this.hash != calculatedHash)
            {
                return false;
            }

            // Get bytes arrays for hash and signature
            byte[] bytesHash = Convert.FromBase64String(this.hash);
            byte[] signatureHash = Convert.FromBase64String(this.signature);
            if (!Statics.SignatureIsValid(bytesHash, signatureHash, this.Sender))
            {
                return false;
            }

            return true;
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
                
                // Get sender & receiver public keys as base64
                string senderPublicKey = Convert.ToBase64String(rsaSender.ExportSubjectPublicKeyInfo());
                string receiverPublicKey = Convert.ToBase64String(rsaReceiver.ExportSubjectPublicKeyInfo());

                transactions.Add(new Transaction(
                    senderPublicKey,
                    receiverPublicKey,
                    randomTransactionAmountEngine.Next(1, 15000))
                );
            }

            return transactions; 
        }

    }

}
