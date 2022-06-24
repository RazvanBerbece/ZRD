using BlockchainNS;
using NUnit.Framework;
using WalletNS;
using System.Security.Cryptography;
using TransactionNS;
using System;
using System.Collections.Generic;
using System.IO;
using WalletNS.BlockchainWalletNS;

namespace WalletTestsNS
{

    public class WalletUnitTests
    {

        // Generic values which are Setup for every test
        private BlockchainWallet networkWallet; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet
        private Wallet walletB; // secondary wallet

        [OneTimeSetUp]
        public void Setup()
        {
            TestContext.Progress.WriteLine("-- Testing Wallet --\n");
            this.networkWallet = new BlockchainWallet(1024);
            this.walletA = new Wallet(1024);
            this.walletB = new Wallet(1024);
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(@"TEST_WALLET.json"))
            {
                File.Delete(@"TEST_WALLET.json");
            }
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
            const int firstAmount = int.MaxValue;
            List<Transaction> initialCoinOfferings = new List<Transaction>()
            {
                new Transaction(networkWallet.GetPublicKeyStringBase64(), walletA.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            // Setup blockchain
            Blockchain blockchain = Blockchain.CreateBlockchain(
                    initialCoinOfferings: initialCoinOfferings,
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
        public void Wallet_GetsJsonString_Correctly()
        {
            Assert.That(string.IsNullOrEmpty(this.walletA.GetJsonString()), Is.Not.True);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(@"TEST_WALLET.json")]
        public void Wallet_SavesToJsonFile_Correctly(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                try
                {
                    this.walletA.SaveToJsonFile(filepath, this.walletA.GetJsonString());
                    Assert.Fail("Wallet.SaveToJsonFile should throw for empty or null parameter value");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            // Check that file exists and that there is content in file "TEST_WALLET.json"
            this.walletA.SaveToJsonFile(filepath, this.walletA.GetJsonString());
            string output = File.ReadAllText(filepath);
            Assert.That(output, Is.Not.Empty);
        }

        [Test]
        public void Static_Wallet_CanDeserializeWalletFromJsonFile()
        {
            this.walletA.SaveToJsonFile(@"TEST_WALLET.json", this.walletA.GetJsonString());
            Wallet importedWallet = Wallet.DeserializeWalletFromJsonFile(@"TEST_WALLET.json");
            
            Assert.That(this.walletA.GetJsonString().Equals(importedWallet.GetJsonString()), Is.True);
        }

    }

}
