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
        
        public BlockchainWallet(int keySize, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.PublicKey = rsa.ExportSubjectPublicKeyInfo();
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();
            this.filepathToRsaXml = filepathToRsaXml;
            this.KeyPair = rsa.ExportParameters(true);
            this.WalletName = "ZRD Network Wallet";
            Wallet.SaveRsaConfigToLocal(this.filepathToRsaXml, rsa);
        }
        
        [JsonConstructor]
        public BlockchainWallet(string publicKey, string walletName, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
        {
            this.PublicKey = Convert.FromBase64String(publicKey);
            this.filepathToRsaXml = filepathToRsaXml;
            
            // Create RSAParameters from RSA config in RSAConfig.xml
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                string rsaConfigString = System.IO.File.ReadAllText(this.filepathToRsaXml);
                rsa.FromXmlString(rsaConfigString);
            }
            catch (Exception)
            {
                Console.WriteLine($"BlockchainWallet setup configuring...");
            }
            this.KeyPair = rsa.ExportParameters(true);
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();

            this.WalletName = walletName;
        }
        
        public BlockchainWallet(string publicKey, string privateKey, string walletName, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
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

        public void Reconfigure(string newFilepathToRsaXml)
        {
            this.filepathToRsaXml = newFilepathToRsaXml;
            
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
            this.KeyPair = rsa.ExportParameters(true);
        }

        public Wallet GetCommonWallet()
        {
            Wallet commonWallet = new Wallet(
                this.GetPublicKeyStringBase64(),
                this.GetPrivateKeyStringBase64(),
                this.WalletName,
                this.filepathToRsaXml
                    );
            return commonWallet;
        }

    }
}