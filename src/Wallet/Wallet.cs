using System;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using BlockchainNS;
using Newtonsoft.Json;
using TransactionNS;
using WalletNS.Abstract;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WalletNS
{
    /// <summary>
    /// Class that defines the properties and methods of a wallet on the ZRD blockchain.
    /// </summary>
    public class Wallet: IWallet
    {
        
        // Core
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        private RSAParameters KeyPair { get; set; }
        
        // Other Metadata
        public string WalletName { get; set; }

        private string filepathToRsaXml;

        /// <summary>
        /// Constructor for a <c>Wallet</c> object.
        /// </summary>
        /// <param name="keySize">Size of key to be created. Minimum 1024.</param>
        /// <param name="filepathToRsaXml">Filepath to save XML string of keypair at</param>
        public Wallet(int keySize, string filepathToRsaXml = "local/Wallet/Params/RSAConfig.xml")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.PublicKey = rsa.ExportSubjectPublicKeyInfo();
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();
            this.filepathToRsaXml = filepathToRsaXml;
            this.KeyPair = rsa.ExportParameters(true);
            this.WalletName = "ZRD Wallet";
            
            System.IO.Directory.CreateDirectory("local/Wallet/Params");
            SaveRsaConfigToLocal(this.filepathToRsaXml, rsa);
        }
        
        [JsonConstructor]
        public Wallet(string publicKey, string privateKey, string walletName = "ZRD Wallet", string filepathToRsaXml = "local/Wallet/Params/RSAConfig.xml")
        {
            this.PublicKey = Convert.FromBase64String(publicKey);
            this.PrivateKey = Convert.FromBase64String(privateKey);
            this.filepathToRsaXml = filepathToRsaXml;
            
            // Create RSAParameters from RSA config in RSAConfig.xml
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                string rsaConfigString = System.IO.File.ReadAllText(this.filepathToRsaXml);
                rsa.FromXmlString(rsaConfigString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while loading wallet params: {e}");
            }
            // rsa.ImportSubjectPublicKeyInfo(this.PublicKey, out _);
            // rsa.ImportPkcs8PrivateKey(this.PrivateKey, out _);
            
            this.KeyPair = rsa.ExportParameters(true);

            this.WalletName = walletName;
        }

        public Wallet() { }

        public static void SaveRsaConfigToLocal(string filepath, RSACryptoServiceProvider rsa)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentException("Target file to save RSA config in should not be empty or null");
            }
            System.IO.File.WriteAllText(filepath, rsa.ToXmlString(true));
        }

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

        public static Wallet DeserializeWalletFromJsonFile(string filepathToWalletJson, string filePathToXmlString = "local/Wallet/Params/RSAConfig.xml")
        {
            string walletJsonString = System.IO.File.ReadAllText(filepathToWalletJson);
            Wallet loadedWalletNoXml = JsonSerializer.Deserialize<Wallet>(
                walletJsonString,
                options: new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                });
            // Create new object with values from json and load passed xml string
            Wallet loadedWalletWithXml = new Wallet(
                loadedWalletNoXml.GetPublicKeyStringBase64(),
                loadedWalletNoXml.GetPrivateKeyStringBase64(),
                loadedWalletNoXml.GetWalletName(),
                filePathToXmlString);
            return loadedWalletWithXml;
        }

    }
}
