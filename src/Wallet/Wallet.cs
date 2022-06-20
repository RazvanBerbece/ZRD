using System;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using BlockchainNS;
using Newtonsoft.Json;
using TransactionNS;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        
        // Other Metadata
        public string WalletName { get; set; }

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
            this.WalletName = "ZRD Wallet";
        }
        
        [JsonConstructor]
        public Wallet(string publicKey, string privateKey, string walletName = "ZRD Wallet")
        {
            this.PublicKey = Convert.FromBase64String(publicKey);
            this.PrivateKey = Convert.FromBase64String(privateKey);
            
            // Create RSAParameters from existing public and private keys by importing them into the sp
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPublicKey(this.PublicKey, out _);
            rsa.ImportPkcs8PrivateKey(this.PrivateKey, out _);
            this.KeyPair = rsa.ExportParameters(true);

            this.WalletName = walletName;
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
            this.WalletName = name;
        }

        public string GetWalletName()
        {
            return this.WalletName;
        }

        public string GetJsonString()
        {
            return JsonSerializer.Serialize(
                this,
                options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                }
            );
        }

        public void SaveToJsonFile(string filepath, string jsonString)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentException("Target file to save wallet in should not be empty or null");
            }
            System.IO.File.WriteAllText(filepath, jsonString);
        }

        public static Wallet DeserializeWalletFromJsonFile(string filepath)
        {
            string walletJsonString = System.IO.File.ReadAllText(filepath);
            Wallet loadedWallet = JsonSerializer.Deserialize<Wallet>(
                walletJsonString,
                options: new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                });
            return loadedWallet;
        }

    }
}
