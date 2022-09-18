using System;
using System.ComponentModel;
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
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("local/Wallet/NetworkWallet/Params/RSAConfig.xml")]
        private string FilepathToRsaXml { get; set; }
        
        public BlockchainWallet() { }
        
        public BlockchainWallet(int keySize, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
        {
            var rsa = new RSACryptoServiceProvider
            {
                KeySize = keySize,
                PersistKeyInCsp = false
            };
            PublicKey = rsa.ExportSubjectPublicKeyInfo();
            PrivateKey = rsa.ExportPkcs8PrivateKey();
            FilepathToRsaXml = filepathToRsaXml;
            KeyPair = rsa.ExportParameters(true);
            WalletName = "ZRD Network Wallet";
            
            System.IO.Directory.CreateDirectory("local/Wallet/NetworkWallet/Params");
            Wallet.SaveRsaConfigToLocal(FilepathToRsaXml, rsa);
        }
        
        [JsonConstructor]
        public BlockchainWallet(string publicKey, string walletName, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
        {
            PublicKey = Convert.FromBase64String(publicKey);
            FilepathToRsaXml = filepathToRsaXml;
            
            // Create RSAParameters from RSA config in RSAConfig.xml
            var rsa = new RSACryptoServiceProvider();
            try
            {
                var rsaConfigString = System.IO.File.ReadAllText(FilepathToRsaXml);
                rsa.FromXmlString(rsaConfigString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"BlockchainWallet setup failed with error: {e}");
            }
            KeyPair = rsa.ExportParameters(true);
            PrivateKey = rsa.ExportPkcs8PrivateKey();

            WalletName = walletName;
        }
        
        public BlockchainWallet(string publicKey, string privateKey, string walletName, string filepathToRsaXml = "local/Wallet/NetworkWallet/Params/RSAConfig.xml")
        {
            PublicKey = Convert.FromBase64String(publicKey);
            PrivateKey = Convert.FromBase64String(privateKey);
            FilepathToRsaXml = filepathToRsaXml;
            
            // Create RSAParameters from RSA config in RSAConfig.xml
            var rsa = new RSACryptoServiceProvider();
            try
            {
                var rsaConfigString = System.IO.File.ReadAllText(FilepathToRsaXml);
                rsa.FromXmlString(rsaConfigString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while loading wallet params: {e}");
            }
            KeyPair = rsa.ExportParameters(true);

            WalletName = walletName;
        }
        
        public string GetPublicKeyStringBase64()
        {
            return Convert.ToBase64String(PublicKey);
        }
        
        public string GetPrivateKeyStringBase64()
        {
            return Convert.ToBase64String(PrivateKey);
        }
        
        public byte[] GetPrivateKeyBytesArray()
        {
            return PrivateKey;
        }
        
        public RSAParameters GetKeyPairParams()
        {
            return KeyPair;
        }

        public void Reconfigure(string newFilepathToRsaXml)
        {
            FilepathToRsaXml = newFilepathToRsaXml;
            
            // Create RSAParameters from RSA config in RSAConfig.xml
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                var rsaConfigString = System.IO.File.ReadAllText(FilepathToRsaXml);
                rsa.FromXmlString(rsaConfigString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while loading wallet params: {e}");
            }
            KeyPair = rsa.ExportParameters(true);
        }

        public Wallet GetCommonWallet()
        {
            var commonWallet = new Wallet(
                GetPublicKeyStringBase64(),
                GetPrivateKeyStringBase64(),
                WalletName,
                FilepathToRsaXml);
            return commonWallet;
        }

    }
}