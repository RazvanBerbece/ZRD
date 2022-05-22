using System.Collections.Generic;
using NUnit.Framework;
using BlockNS;
using TransactionNS;
using System.Text.RegularExpressions;
using WalletNS;
using System.Security.Cryptography;

namespace WalletTestsNS
{

    public class WalletUnitTests
    {

        // Generic values which are Setup for every test
        private Wallet wallet;

        [OneTimeSetUp]
        public void Setup()
        {
            this.wallet = new Wallet(1024);
        }

        [Test]
        public void Wallet_CanConstruct()
        {
            Assert.IsNotEmpty(this.wallet.publicKey);
            Assert.IsNotEmpty(this.wallet.GetPrivateKeyStringBase64());
        }

        [Test]
        public void External_PrivateMemberAccess_BytesArray()
        {
            byte[] privateKey = this.wallet.GetPrivateKeyBytesArray();
            Assert.IsNotEmpty(privateKey);
        }

        [Test]
        public void External_PrivateMemberAccess_Base64()
        {
            string privateKey = this.wallet.GetPrivateKeyStringBase64();
            Assert.IsNotEmpty(privateKey);
        }

        [Test]
        public void Wallet_CanConvertPublicKeyTo_Base64String()
        {
            string publicKey = this.wallet.GetPublicKeyStringBase64();
            Assert.IsNotEmpty(publicKey);
        }

        [Test]
        public void External_PrivateMemberAccess_KeyPairParam()
        {
            Assert.IsNotNull(this.wallet.GetKeyPairParams(), "GetKeyPairParams() should not return null");
            Assert.IsInstanceOf(typeof(RSAParameters), this.wallet.GetKeyPairParams());
        }

    }

}
