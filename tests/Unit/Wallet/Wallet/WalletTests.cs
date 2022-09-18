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
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing Wallet --\n");
        }
        
        [SetUp]
        public void Setup()
        {
            this.networkWallet = new BlockchainWallet(1024, "TEST_NETWORK_WALLET.xml");
            this.walletA = new Wallet(1024, "TEST_USER_WALLET_1.xml");
            this.walletB = new Wallet(1024, "TEST_USER_WALLET_2.xml");
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(@"TEST_ZRD.json"))
            {
                File.Delete(@"TEST_ZRD.json");
            }
            if(File.Exists(@"TEST_WALLET.json"))
            {
                File.Delete(@"TEST_WALLET.json");
            }
            if(File.Exists(@"TEST_NETWORK_WALLET.xml"))
            {
                File.Delete(@"TEST_NETWORK_WALLET.xml");
            }
            if(File.Exists(@"TEST_USER_WALLET_1.xml"))
            {
                File.Delete(@"TEST_USER_WALLET_1.xml");
            }
            if(File.Exists(@"TEST_USER_WALLET_2.xml"))
            {
                File.Delete(@"TEST_USER_WALLET_2.xml");
            }
        }

        [Test]
        public void Wallet_CanConstruct_WithKeySize()
        {
            Assert.IsNotEmpty(this.walletA.PublicKey);
            Assert.IsNotEmpty(this.walletA.GetPrivateKeyStringBase64());
            Assert.IsNotEmpty(System.IO.File.ReadAllText("TEST_USER_WALLET_1.xml"));
        }
        
        [Test]
        public void Wallet_CanConstruct_WithBase64String_AndXml()
        {
            // Test constructor from deserialization
            Wallet testWallet = Wallet.DeserializeWalletFromJsonFile("../../../tests/Unit/Wallet/Wallet/Wallet.json", "../../../tests/Unit/Wallet/Wallet/Params/RSAConfig.xml");
            Assert.IsNotEmpty(testWallet.PublicKey);
            Assert.IsNotEmpty(testWallet.GetPrivateKeyStringBase64());
            
            // Test constructor with hardcoded params
            Wallet testWallet2 = new Wallet(
                "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC3EMJeuHfbgDrG0qxWPumhScTLE5oOFQOmV9TJjgncLSyk1CoE/nCUGfVv9RLP7IHCIBM3g19iMu5qFU5016O9/C2qv2kpNetbHQDmC2Fg+XeY2oTqE13SD1VgF9LxLJrnH75WKv9i+GTi6toAFm1bcoP+l7MMwIjSh9Sb06kapQIDAQAB",
                "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALcQwl64d9uAOsbSrFY+6aFJxMsTmg4VA6ZX1MmOCdwtLKTUKgT+cJQZ9W/1Es/sgcIgEzeDX2Iy7moVTnTXo738Laq/aSk161sdAOYLYWD5d5jahOoTXdIPVWAX0vEsmucfvlYq/2L4ZOLq2gAWbVtyg/6XswzAiNKH1JvTqRqlAgMBAAECgYBvu4rmxTBiiKFXOL525W8zQhMa35vnfGv92x3E5yyddfUJpXUAF0wfGLj03F/fCDsqgOk5uLU++lcJ6Hc6WWNWSIcQop9j5rS/ggWUfU9llAzadbb1irTpM3gmX0n/2xE8xcAg9cgy9TX462AdrtZ7Jo6NSu8QDXfJEkEvIxRoAQJBAOyTtErLWwqiRD5yhFWmf25qwJVUwntq3/+o3bBIspWynB1RyqwOyioYL0WvHr8vFGVwx9QjAZK9MeLzAy1CjQ0CQQDGGF46JVdYl+XGxE4H1HZpI+TzkPP5KX/vGh3nBpBFsnua0KYvNsmQmQBvwl/Pvc3UFBxi658REsDwLpeZRU35AkAi5H4Y8flRjjFGjJlcEJyG6pPQ8plknpS/HmbkEzTTw24nHOMpkVzb7Ik8W+HLDOSTOZkffrJCtEjhUjpLuJ8ZAkEArmQ/d9LtzVmT+GNTCoOZZsApy979WYmWTglg78SQeDtDo6wx0PjbhAeeIcUtkfZXYG//+XnSxDYNUqTB4zXnCQJBAMG+ID2BiA9zNKDOanIeSF0wOTLreppS32TghnzuH/cQeuqen+Mpa0J+AHl3Wv8ceGnCikIvugUUyi64MY3erec=",
                "ZRD Wallet",
                "../../../tests/Wallet.UnitTests/Wallet/Params/RSAConfig.xml"
                );
            Assert.IsNotEmpty(testWallet2.PublicKey);
            Assert.IsNotEmpty(testWallet2.GetPrivateKeyStringBase64());
            Assert.IsNotNull(testWallet2.GetKeyPairParams());
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
                    reward: 420,
                    filepathToState: "TEST_ZRD.json"
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
            Wallet importedWallet = Wallet.DeserializeWalletFromJsonFile(@"TEST_WALLET.json", "TEST_USER_WALLET_1.xml");
            
            Assert.That(this.walletA.GetJsonString().Equals(importedWallet.GetJsonString()), Is.True);
        }

    }

}
