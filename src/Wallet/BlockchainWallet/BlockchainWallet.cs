using System;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace WalletNS.BlockchainWalletNS
{
    public class BlockchainWallet
    {

        public byte[] PublicKey { get; set; }
        public string WalletName { get; set; }

        private byte[] PrivateKey { get; set; }
        private RSAParameters KeyPair { get; set; }
        
        public BlockchainWallet() { }
        
        public BlockchainWallet(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.PublicKey = rsa.ExportSubjectPublicKeyInfo();
            this.PrivateKey = rsa.ExportPkcs8PrivateKey();
            this.KeyPair = rsa.ExportParameters(true);
            this.WalletName = "ZRD Network Wallet";
        }
        
        [JsonConstructor]
        public BlockchainWallet(string publicKey, string privateKey, string walletName)
        {
            this.PublicKey = Convert.FromBase64String(publicKey);
            this.PrivateKey = Convert.FromBase64String(privateKey);
            
            // Create RSAParameters from existing public and private keys by importing them into the sp
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportSubjectPublicKeyInfo(this.PublicKey, out _);
            rsa.ImportPkcs8PrivateKey(this.PrivateKey, out _);
            this.KeyPair = rsa.ExportParameters(true);

            this.WalletName = walletName;
        }
        
        public string GetPublicKeyStringBase64()
        {
            return Convert.ToBase64String(this.PublicKey);
        }
        
        private string GetPrivateKeyStringBase64()
        {
            return Convert.ToBase64String(this.PrivateKey);
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