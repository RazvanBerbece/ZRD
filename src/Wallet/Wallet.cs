using System;
using System.Security.Cryptography;

namespace WalletNS
{
    /// <summary>
    /// Class that defines the properties and methods of a wallet for the ZRD blockchain.
    /// </summary>
    public class Wallet
    {

        public byte[] publicKey;
        private byte[] privateKey;

        private RSAParameters keyPair;

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
    }
}
