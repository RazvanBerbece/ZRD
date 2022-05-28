using System;
using BlockNS;
using BlockchainNS;
using NUnit.Framework;
using System.Collections.Generic;
using TransactionNS;
using WalletNS;

namespace BlockchainTestsNS
{
    [TestFixture]
    public class BlockchainTests
    {
        
        private Wallet networkWallet;
        private Blockchain chain;
        
        private Wallet testWallet;
        
        [SetUp]
        public void Setup()
        {
            networkWallet = new Wallet(1024);
            testWallet = new Wallet(1024);
        }

        [TestCase(-1, 10, 1000, false)]
        [TestCase(1, -10, 1000, false)]
        [TestCase(1, 10, -1000, false)]
        [TestCase(1, 10, 1000, true)]
        public void Static_CanCreateBlockchain(int difficulty, int blockTime, int reward, bool expectedOutputResult)
        {
            this.chain = Blockchain.CreateBlockchain(
                firstMint: new Transaction(
                    this.networkWallet.GetPublicKeyStringBase64(),
                    this.testWallet.GetPublicKeyStringBase64(),
                    1000000
                    ),
                blockchainWallet: this.networkWallet,
                difficulty: difficulty,
                blockTime: blockTime,
                reward: reward
                );

            if (!expectedOutputResult)
            {
                Assert.That(this.chain, Is.Null);
            }
            else
            {
                Assert.That(this.chain, Is.Not.Null);
            }
        }

        [TestCase(true, TestName = "Test case #1, Using Block object instance")]
        [TestCase(false, TestName = "Test case #2, Using null for Block object instance")]
        public void Blockchain_CanAddBlock(bool testWithNullBlock)
        {
            
            // Setup test Blockchain
            this.chain = Blockchain.CreateBlockchain(
                firstMint: new Transaction(
                    this.networkWallet.GetPublicKeyStringBase64(),
                    this.testWallet.GetPublicKeyStringBase64(),
                    1000000
                ),
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10
            );

            if (!testWithNullBlock)
            {
                List<Transaction> randomTransactions = Transaction.GenerateRandomTransactions(10);
                Block randomBlock = new Block(
                    randomTransactions,
                    "previousHash",
                    1
                );
                randomBlock.hash = randomBlock.CalculateHash();
                this.chain.AddBlock(randomBlock);
            
                Assert.That(this.chain.chain.Count, Is.EqualTo(2)); // chain should have 2 blocks: genesis and randomBlock
            }
            else
            {
                try
                {
                    this.chain.AddBlock(null);
                    Assert.Fail("Should not be able to add null Block to Blockchain instance");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("atRuntimePublicKey")]
        [TestCase("nonExistingKey")]
        public void Blockchain_GetsBalance(string publicKey)
        {
            
            // Setup test Blockchain
            int firstAmount = 1000000;
            this.chain = Blockchain.CreateBlockchain(
                firstMint: new Transaction(
                    this.networkWallet.GetPublicKeyStringBase64(),
                    this.testWallet.GetPublicKeyStringBase64(),
                    firstAmount
                ),
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10
            );
            
            if (publicKey == "")
            {
                int balance = this.chain.GetBalance(publicKey);
                Assert.That(balance, Is.EqualTo(-1));
            }
            else if (publicKey == "nonExistingKey")
            {
                int balance = this.chain.GetBalance(publicKey);
                Assert.That(balance, Is.EqualTo(0));
            }
            else
            {
                publicKey = this.testWallet.GetPublicKeyStringBase64();
                int balance = this.chain.GetBalance(publicKey);
                Assert.That(balance, Is.EqualTo(firstAmount));
            }
        }

    }
}