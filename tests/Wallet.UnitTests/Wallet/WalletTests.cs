﻿using BlockchainNS;
using NUnit.Framework;
using WalletNS;
using System.Security.Cryptography;
using TransactionNS;

namespace WalletTestsNS
{

    public class WalletUnitTests
    {

        // Generic values which are Setup for every test
        private Wallet NETWORK_WALLET; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet
        private Wallet walletB; // secondary wallet

        [OneTimeSetUp]
        public void Setup()
        {
            this.NETWORK_WALLET = new Wallet(1024);
            this.walletA = new Wallet(1024);
            this.walletB = new Wallet(1024);
        }

        [Test]
        public void Wallet_CanConstruct()
        {
            Assert.IsNotEmpty(this.walletA.publicKey);
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
                        this.NETWORK_WALLET.GetPublicKeyStringBase64(),
                        this.walletA.GetPublicKeyStringBase64(),
                        1000000
                        ),
                    difficulty: 2,
                    blockTime: 5,
                    reward: 420
                );

            // Send currency
            this.walletA.SendCurrency(amount, this.walletB.GetPublicKeyStringBase64(), blockchain);

            // Get untransferred currency (balance)
            int secondaryWalletBalance = blockchain.GetBalance(this.walletB.GetPublicKeyStringBase64());

            // Guard - WalletB received currency
            Assert.AreEqual(secondaryWalletBalance, amount);

        }

    }

}
