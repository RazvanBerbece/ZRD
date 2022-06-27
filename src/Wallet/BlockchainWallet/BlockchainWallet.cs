using System;
using System.Security.Cryptography;
using Newtonsoft.Json;
using WalletNS.Abstract;

namespace WalletNS.BlockchainWalletNS
{
    public class BlockchainWallet: IWallet
    {

        public byte[] PublicKey { get; set; }
        public string WalletName { get; set; }

        private byte[] PrivateKey { get; set; }
        private RSAParameters KeyPair { get; set; }
        
        private string filepathToRsaXml;
        
        public BlockchainWallet() { }
        
        public BlockchainWallet(int keySize, string filepathToRsaXml = "local/Wallet/Params/RSAConfig.xml")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.PublicKey = rsa.ExportSubjectPublicKeyInfo();
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();
            this.filepathToRsaXml = filepathToRsaXml;
            this.KeyPair = rsa.ExportParameters(true);
            this.WalletName = "ZRD Network Wallet";
        }
        
        [JsonConstructor]
        public BlockchainWallet(string publicKey, string privateKey, string walletName, string filepathToRsaXml = "local/Wallet/Params/RSAConfig.xml")
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
        
        public string GetPublicKeyStringBase64()
        {
            return Convert.ToBase64String(this.PublicKey);
        }
        
        public string GetPrivateKeyStringBase64()
        {
            return Convert.ToBase64String(this.PrivateKey);
        }
        
        public byte[] GetPrivateKeyBytesArray()
        {
            return this.PrivateKey;
        }
        
        public RSAParameters GetKeyPairParams()
        {
            return this.KeyPair;
        }

        public Wallet GetCommonWallet()
        {
            Wallet commonWallet = new Wallet(
                this.GetPublicKeyStringBase64(),
                this.GetPrivateKeyStringBase64(),
                this.WalletName
                    );
            return commonWallet;
        }

    }
}