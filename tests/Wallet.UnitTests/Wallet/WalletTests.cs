using BlockchainNS;
using NUnit.Framework;
using WalletNS;
using System.Security.Cryptography;
using TransactionNS;
using System;

namespace WalletTestsNS
{

    public class WalletUnitTests
    {

        // Generic values which are Setup for every test
        private Wallet networkWallet; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet
        private Wallet walletB; // secondary wallet

        [OneTimeSetUp]
        public void Setup()
        {
            this.networkWallet = new Wallet(1024);
            this.walletA = new Wallet(1024);
            this.walletB = new Wallet(1024);
        }

        [Test]
        public void Wallet_CanConstruct()
        {
            Assert.IsNotEmpty(this.walletA.PublicKey);
            Assert.IsNotEmpty(this.walletA.GetPrivateKeyStringBase64());
        }

        [Test]
        public void External_PrivateMemberAccess_BytesArray()
        {
            byte[] privateKey = this.walletA.GetPrivateKeyBytesArray();
            Assert.IsNotEmpty(privateKey);
        }

        [Test]
        public void External_PrivateMemberAccess_Base64()
        {
            string privateKey = this.walletA.GetPrivateKeyStringBase64();
            Assert.IsNotEmpty(privateKey);
        }

        [Test]
        public void Wallet_CanConvertPublicKeyTo_Base64String()
        {
            string publicKey = this.walletA.GetPublicKeyStringBase64();
            Assert.IsNotEmpty(publicKey);
        }

        [Test]
        public void External_PrivateMemberAccess_KeyPairParam()
        {
            Assert.IsNotNull(this.walletA.GetKeyPairParams(), "GetKeyPairParams() should not return null");
            Assert.IsInstanceOf(typeof(RSAParameters), this.walletA.GetKeyPairParams());
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1250)]
        [TestCase(int.MaxValue)]
        public void Wallet_CanSendCurrency(int amount)
        {
            // Setup blockchain
            Blockchain blockchain = Blockchain.CreateBlockchain(
                    firstMint: new Transaction(
                        this.networkWallet.GetPublicKeyStringBase64(),
                        this.walletA.GetPublicKeyStringBase64(),
                        int.MaxValue
                        ),
                    blockchainWallet: this.networkWallet,
                    difficulty: 2,
                    blockTime: 5,
                    reward: 420
                );

            if (amount <= 0)
            {
                try
                {
                    // Send currency
                    this.walletA.SendCurrency(amount, this.walletB.GetPublicKeyStringBase64(), blockchain);
                    Assert.Fail("Wallet should not be able to send negative or 0 amounts of currency");
                }
                catch (Exception )
                {
                    Assert.Pass();
                }
            }

            // Guard - Wallet cannot send currency with empty string for receiver key
            try
            {
                this.walletA.SendCurrency(amount, "", blockchain);
                Assert.Fail("Wallet cannot send currency with empty string for receiver key");
            }
            catch (Exception) {}

            // Send currency
            this.walletA.SendCurrency(amount, this.walletB.GetPublicKeyStringBase64(), blockchain);
            blockchain.MineUnconfirmedTransactions(walletA.GetPublicKeyStringBase64());

            // Get untransferred currency (balance)
            int secondaryWalletBalance = blockchain.GetBalance(this.walletB.GetPublicKeyStringBase64());

            // Guard - WalletB received currency
            Assert.AreEqual(amount, secondaryWalletBalance);

        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("validName")]
        public void Wallet_CanSet_WalletName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                try
                {
                    this.walletA.SetWalletName(name);
                    Assert.Fail("Wallet should not set name with null or empty parameter");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            
            this.walletA.SetWalletName(name);
            Assert.That(this.walletA.GetWalletName().Equals("validName"), Is.True);
        }

        [Test]
        public void Wallet_CanGet_WalletName()
        {
            this.walletA.SetWalletName("validName");
            Assert.That(this.walletA.GetWalletName().Equals("validName"), Is.True);
        }
        
        [Test]
        public void Wallet_CanSet_PublicIpAddress()
        {
            this.walletA.SetPublicIpAddress();
            Assert.That(string.IsNullOrEmpty(this.walletA.GetPublicIpAddressString()), Is.Not.True);
        }
        
        [Test]
        public void Wallet_CanGet_PublicIpAddress()
        {
            Assert.That(this.walletA.GetPublicIpAddressString().Equals(""), Is.True);
            this.walletA.SetPublicIpAddress();
            Assert.That(this.walletA.GetPublicIpAddressString().Equals(""), Is.Not.True);
        }

    }

}
