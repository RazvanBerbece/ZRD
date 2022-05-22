using System;
using System.Security.Cryptography;

namespace WalletNS
{
    /// <summary>
    /// Class that defines the properties and methods of a wallet for the ZRD blockchain.
    /// </summary>
    public class Wallet
    {

        public string publicKey;
        private string privateKey;

        /// <summary>
        /// Constructor for a <c>Wallet</c> object.
        /// </summary>
        /// <param name="keySize">Size of key to be created. Minimum 1024.</param>
        public Wallet(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            this.publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
            this.privateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());
        }

        public string GetPrivateKey()
        {
            return this.privateKey;
        }
    }
}
