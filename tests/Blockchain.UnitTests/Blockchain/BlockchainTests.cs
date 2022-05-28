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

            int balance;
            switch (publicKey)
            {
                case "":
                    balance = this.chain.GetBalance(publicKey);
                    Assert.That(balance, Is.EqualTo(-1));
                    break;
                case "nonExistingKey":
                    balance = this.chain.GetBalance(publicKey);
                    Assert.That(balance, Is.EqualTo(0));
                    break;
                case "atRuntimePublicKey":
                    publicKey = this.testWallet.GetPublicKeyStringBase64();
                    balance = this.chain.GetBalance(publicKey);
                    Assert.That(balance, Is.EqualTo(firstAmount));
                    break;
            }

        }

        [TestCase(1, TestName = "Test case #1, Testing on non-compromised blockchain of size 1")]
        [TestCase(2, TestName = "Test case #2, Testing on compromised blockchain of size 1")]
        [TestCase(3, TestName = "Test case #3, Testing on non-compromised blockchain of size >1")]
        [TestCase(4, TestName = "Test case #4, Testing on compromised blockchain of size >1")]
        public void Blockchain_CanValidate(int blockchainStatus)
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

            List<Transaction> randomTransactions;

            switch (blockchainStatus)
            {
                case 1:
                    // Test case #1, Testing on non-compromised blockchain of size 1
                    Assert.That(this.chain.IsValid(), Is.True);
                    break;
                case 2:
                    // Test case #2, Testing on compromised blockchain of size 1
                    this.chain.chain.First.Value.data[0].Amount -= 100; // mutate first coin release transaction
                    Assert.That(this.chain.IsValid(), Is.False);
                    break;
                case 3:
                    // Test case #3, Testing on non-compromised blockchain of size >1
                    // Add block 1 with random transactions
                    randomTransactions = Transaction.GenerateRandomTransactions(10);
                    Block randomBlockUncompromised1 = new Block(
                        randomTransactions,
                        this.chain.chain.Last.Value.hash,
                        this.chain.chain.Last.Value.index + 1
                    );
                    randomBlockUncompromised1.hash = randomBlockUncompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockUncompromised1);
                    
                    // Add block 2 with random transactions
                    randomTransactions = Transaction.GenerateRandomTransactions(10);
                    Block randomBlockUncompromised2 = new Block(
                        randomTransactions,
                        this.chain.chain.Last.Value.hash,
                        this.chain.chain.Last.Value.index + 1
                    );
                    randomBlockUncompromised2.hash = randomBlockUncompromised2.CalculateHash();
                    this.chain.AddBlock(randomBlockUncompromised2);
                    
                    Assert.That(this.chain.IsValid(), Is.True);
                    break;
                case 4:
                    // Test case #4, Testing on compromised blockchain of size >1
                    Random rnd = new Random();
                    // Add block 1 with random transactions
                    randomTransactions = Transaction.GenerateRandomTransactions(10);
                    Block randomBlockCUncompromised1 = new Block(
                        randomTransactions,
                        this.chain.chain.Last.Value.hash,
                        this.chain.chain.Last.Value.index + 1
                    );
                    randomBlockCUncompromised1.hash = randomBlockCUncompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockCUncompromised1);
                    
                    // Add block 2 with random transactions AND mutate transaction post-hashing
                    randomTransactions = Transaction.GenerateRandomTransactions(10);
                    Block randomBlockCompromised1 = new Block(
                        randomTransactions,
                        this.chain.chain.Last.Value.hash,
                        this.chain.chain.Last.Value.index + 1
                    );
                    randomBlockCompromised1.hash = randomBlockCompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockCompromised1);
                    
                    // Mutate
                    this.chain.chain.Last.Value.data[rnd.Next(1, 11)].Amount += -100;
                    
                    Assert.That(this.chain.IsValid(), Is.False);
                    break;
            }

        }

    }
}