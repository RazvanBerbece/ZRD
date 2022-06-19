using System;
using System.Net;
using System.Security.Cryptography;
using BlockchainNS;
using Newtonsoft.Json;
using TransactionNS;

namespace WalletNS
{
    /// <summary>
    /// Class that defines the properties and methods of a wallet on the ZRD blockchain.
    /// </summary>
    public class Wallet
    {
        
        // Core
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        private RSAParameters KeyPair { get; set; }
        
        // Private Metadata
        private string walletName;
        private IPAddress publicIpAddress; // .ToString() will return the string representation of the EXT public IP Address

        /// <summary>
        /// Constructor for a <c>Wallet</c> object.
        /// </summary>
        /// <param name="keySize">Size of key to be created. Minimum 1024.</param>
        public Wallet(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.PublicKey = rsa.ExportSubjectPublicKeyInfo();
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();
            this.KeyPair = rsa.ExportParameters(true);
            this.walletName = "ZRD Wallet";
        }
        
        [JsonConstructor]
        public Wallet(string publicKey, string privateKey)
        {
            this.PublicKey = Convert.FromBase64String(publicKey);
            this.PrivateKey = Convert.FromBase64String(privateKey);
            
            // Create RSAParameters from existing public and private keys by importing them into the sp
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPublicKey(this.PublicKey, out _);
            rsa.ImportPkcs8PrivateKey(this.PrivateKey, out _);
            this.KeyPair = rsa.ExportParameters(true);
        }

        public Wallet() { }

        public string GetPrivateKeyStringBase64()
        {
            return Convert.ToBase64String(this.PrivateKey);
        }

        public byte[] GetPrivateKeyBytesArray()
        {
            return this.PrivateKey;
        }

        public string GetPublicKeyStringBase64()
        {
            return Convert.ToBase64String(this.PublicKey);
        }

        public RSAParameters GetKeyPairParams()
        {
            return this.KeyPair;
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

        public void SetWalletName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Wallet name cannot be null or empty");
            }
            this.walletName = name;
        }

        public string GetWalletName()
        {
            return this.walletName;
        }
        
        public void SetPublicIpAddress()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            this.publicIpAddress = IPAddress.Parse(externalIpString);
        }

        public string GetPublicIpAddressString()
        {
            if (this.publicIpAddress == null || string.IsNullOrEmpty(this.publicIpAddress.ToString()))
            {
                return "";
            }
            return this.publicIpAddress.ToString();
        }

    }
}
