using BlockchainNS;
using System;
using StaticsNS;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using WalletNS;

namespace TransactionNS
{
    public class Transaction
    {

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int Amount { get; set; }

        public string Id { get; set; }
        public string Hash { get; set; }

        public string Signature { get; set; }

        public Transaction(string sender, string receiver, int amount, string id = null, string hash = null)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Transaction amount cannot be negative or zero");
            }

            if (sender.Length == 0 || receiver.Length == 0)
            {
                throw new ArgumentException("Sender/Receiver RSA keys cannot be empty");
            }

            this.Sender = sender;
            this.Receiver = receiver;
            this.Amount = amount;

            this.Signature = null; // unsigned at the moment of instantiation
          
            // Generate random version 4 UUID for transaction if id arg is null
            this.Id = id ?? Guid.NewGuid().ToString();

            // Calculate hash value of transaction
            string concatenatedData = this.Id + this.Sender + this.Receiver + this.Amount.ToString();
            this.Hash = hash ?? Statics.CreateHashSha256(concatenatedData);
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
                
                byte[] originalData = Convert.FromBase64String(this.Hash);

                // Create instance of RSACryptoServiceProvider using the
                // key from RSAParameters
                try
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(wallet.GetKeyPairParams());

                    byte[] signature = rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    this.Signature = Convert.ToBase64String(signature);
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

            if (this.Amount > chain.GetBalance(this.Sender))
            {
                return false;
            }

            // Calculate hash value of current transaction
            string concatenatedData = this.Id + this.Sender + this.Receiver + this.Amount.ToString();
            string calculatedHash = Statics.CreateHashSha256(concatenatedData);
            if (this.Hash != calculatedHash)
            {
                return false;
            }

            // Get bytes arrays for hash and signature
            byte[] bytesHash = Convert.FromBase64String(this.Hash);
            byte[] signatureHash = Convert.FromBase64String(this.Signature);
            if (!Statics.SignatureIsValid(bytesHash, signatureHash, this.Sender))
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Generates a list of Transactions of size |numberOfTransactions|.
        /// It generates random wallet addresses for senders and receivers and random amounts.
        /// If the signed argument is set to true, the generated transactions will also be signed against the sender's wallet.
        /// </summary>
        /// <param name="numberOfTransactions">The number of transactions to generate and add to the returned list</param>
        /// <param name="signed">Toggle whether the generated transactions are signed</param>
        /// <returns>List of size numberOfTransactions of random transactions</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<Transaction> GenerateRandomTransactions(int numberOfTransactions, bool signed)
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
                // Generate wallets to simulate party identifiers
                Wallet senderWallet = new Wallet(1024);
                Wallet receiverWallet = new Wallet(1024);

                Transaction newTransaction = new Transaction(
                    senderWallet.GetPublicKeyStringBase64(),
                    receiverWallet.GetPublicKeyStringBase64(),
                    randomTransactionAmountEngine.Next(1, 15000));
                if (signed)
                {
                    newTransaction.SignTransaction(senderWallet);
                }
                transactions.Add(newTransaction);
            }

            return transactions; 
        }
        
        public string ToJsonString()
        {   
            // TODO: This can be improved using JsonSerializer.SerializeToUtf8Bytes and then saving bytes to file
            string jsonStringBlock = JsonSerializer.Serialize(
                this,
                options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                }
            );
            return jsonStringBlock;
        }
        
        public static Transaction JsonStringToTransactionInstance(string transactionJsonString)
        {
            try
            {
                Transaction transaction = JsonSerializer.Deserialize<Transaction>(
                    transactionJsonString,
                    options: new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    });
                return transaction;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}
