using System;
using System.Security.Cryptography;
using BlockchainNS;
using TransactionNS;

namespace WalletNS
{
    /// <summary>
    /// Class that defines the properties and methods of a wallet on the ZRD blockchain.
    /// </summary>
    public class Wallet
    {

        public byte[] publicKey { get; set; }
        private byte[] privateKey { get; set; }

        private RSAParameters keyPair { get; set; }

        /// <summary>
        /// Constructor for a <c>Wallet</c> object.
        /// </summary>
        /// <param name="keySize">Size of key to be created. Minimum 1024.</param>
        public Wallet(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.publicKey = rsa.ExportSubjectPublicKeyInfo();
            this.privateKey = rsa.ExportPkcs8PrivateKey();
            this.keyPair = rsa.ExportParameters(true);
        }

        public string GetPrivateKeyStringBase64()
        {
            return Convert.ToBase64String(this.privateKey);
        }

        public byte[] GetPrivateKeyBytesArray()
        {
            return this.privateKey;
        }

        public string GetPublicKeyStringBase64()
        {
            return Convert.ToBase64String(this.publicKey);
        }

        public RSAParameters GetKeyPairParams()
        {
            return this.keyPair;
        }

        public void SendCurrency(int amount, string receiverPublicKey, Blockchain blockchain)
        {

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Wallet cannot send negative or 0 currency amounts");
            }

            if (receiverPublicKey == "")
            {
                throw new ArgumentException("receiverPublicKey cannot be the empty string");
            }

            Transaction transaction = new Transaction(this.GetPublicKeyStringBase64(), receiverPublicKey, amount);
            transaction.SignTransaction(this);
            blockchain.AddTransaction(transaction);
        }

    }
}
