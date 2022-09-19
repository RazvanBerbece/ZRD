using System;
using System.Collections.Generic;
using System.IO;
using BlockNS;
using NUnit.Framework;
using WalletNS;
using WalletNS.BlockchainWalletNS;

namespace ZRD.tests.Integration.Blockchain.Blockchain
{
    [TestFixture]
    public class BlockchainTests
    {
        
        private BlockchainWallet networkWallet;
        private BlockchainNS.Blockchain chain;
        
        private Wallet testWallet;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing Blockchain --\n");
        }
        
        [SetUp]
        public void Setup()
        {
            networkWallet = new BlockchainWallet(1024, "TEST_NETWORK_WALLET_PARAMS.xml");
            testWallet = new Wallet(1024, "TEST_WALLET_PARAMS.xml");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if(File.Exists(@"TEST_ZRD.json"))
                {
                    File.Delete(@"TEST_ZRD.json");
                }
                if(File.Exists(@"TEST_NETWORK_WALLET_PARAMS.xml"))
                {
                    File.Delete(@"TEST_NETWORK_WALLET_PARAMS.xml");
                }
                if(File.Exists(@"TEST_WALLET_PARAMS.xml"))
                {
                    File.Delete(@"TEST_WALLET_PARAMS.xml");
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [TestCase(-1, 10, 1000, false)]
        [TestCase(1, -10, 1000, false)]
        [TestCase(1, 10, -1000, false)]
        [TestCase(1, 10, 1000, true)]
        public void Static_CanCreateBlockchain_Correctly(int difficulty, int blockTime, int reward, bool expectedOutputResult)
        {
            // Create Blockchain instance with initial coin offerings
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    1000000),
            };
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: difficulty,
                blockTime: blockTime,
                reward: reward,
                filepathToState: "TEST_ZRD.json"
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
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );

            if (!testWithNullBlock)
            {
                List<TransactionNS.Transaction> randomTransactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
                Block randomBlock = new Block(
                    randomTransactions,
                    "previousHash",
                    1
                );
                randomBlock.Hash = randomBlock.CalculateHash();
                this.chain.AddBlock(randomBlock);
            
                Assert.That(this.chain.Chain.Count, Is.EqualTo(2)); // chain should have 2 blocks: genesis and randomBlock
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
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
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
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );

            List<TransactionNS.Transaction> randomTransactions;

            switch (blockchainStatus)
            {
                case 1:
                    // Test case #1, Testing on non-compromised blockchain of size 1
                    Assert.That(this.chain.IsValid(), Is.True);
                    break;
                case 2:
                    // Test case #2, Testing on compromised blockchain of size 1
                    this.chain.Chain.First.Value.Transactions[0].Amount -= 100; // mutate first coin release transaction
                    Assert.That(this.chain.IsValid(), Is.False);
                    break;
                case 3:
                    // Test case #3, Testing on non-compromised blockchain of size >1
                    // Add block 1 with random transactions
                    randomTransactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
                    Block randomBlockUncompromised1 = new Block(
                        randomTransactions,
                        this.chain.Chain.Last.Value.Hash,
                        this.chain.Chain.Last.Value.Index + 1
                    );
                    randomBlockUncompromised1.Hash = randomBlockUncompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockUncompromised1);
                    
                    // Add block 2 with random transactions
                    randomTransactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
                    Block randomBlockUncompromised2 = new Block(
                        randomTransactions,
                        this.chain.Chain.Last.Value.Hash,
                        this.chain.Chain.Last.Value.Index + 1
                    );
                    randomBlockUncompromised2.Hash = randomBlockUncompromised2.CalculateHash();
                    this.chain.AddBlock(randomBlockUncompromised2);
                    
                    Assert.That(this.chain.IsValid(), Is.True);
                    break;
                case 4:
                    // Test case #4, Testing on compromised blockchain of size >1
                    Random rnd = new Random();
                    // Add block 1 with random transactions
                    randomTransactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
                    Block randomBlockCUncompromised1 = new Block(
                        randomTransactions,
                        this.chain.Chain.Last.Value.Hash,
                        this.chain.Chain.Last.Value.Index + 1
                    );
                    randomBlockCUncompromised1.Hash = randomBlockCUncompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockCUncompromised1);
                    
                    // Add block 2 with random transactions AND mutate transaction post-hashing
                    randomTransactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
                    Block randomBlockCompromised1 = new Block(
                        randomTransactions,
                        this.chain.Chain.Last.Value.Hash,
                        this.chain.Chain.Last.Value.Index + 1
                    );
                    randomBlockCompromised1.Hash = randomBlockCompromised1.CalculateHash();
                    this.chain.AddBlock(randomBlockCompromised1);
                    
                    // Mutate
                    this.chain.Chain.Last.Value.Transactions[rnd.Next(1, 10)].Amount += -100;
                    
                    Assert.That(this.chain.IsValid(), Is.False);
                    break;
            }
        }

        [TestCase("nullTransaction", 
            TestName = "Test case #1, Testing by passing null reference to AddTransaction")]
        [TestCase("uncompromisedTransaction", 
            TestName = "Test case #2, Testing by passing uncompromised Transaction to AddTransaction")]
        [TestCase("compromisedTransaction", 
            TestName = "Test case #3, Testing by passing compromised Transaction to AddTransaction")]
        public void Blockchain_CanAddTransaction_Correctly(string transactionStatus)
        {
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );
            
            switch (transactionStatus)
            {
                case "nullTransaction":
                    try
                    {
                        this.chain.AddTransaction(null);
                        Assert.Fail("Blockchain should not add null Transaction references to unconfirmedTransactions");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                case "uncompromisedTransaction":
                    
                    TransactionNS.Transaction transaction = new TransactionNS.Transaction(
                        this.testWallet.GetPublicKeyStringBase64(),
                        this.networkWallet.GetPublicKeyStringBase64(),
                        2000
                    );
                    transaction.SignTransaction(this.testWallet);
                    
                    this.chain.AddTransaction(transaction);
                    
                    Assert.That(this.chain.UnconfirmedTransactions.Count , Is.EqualTo(1));
                    break;
                case "compromisedTransaction":
                    
                    TransactionNS.Transaction compromisedTransaction = new TransactionNS.Transaction(
                        this.testWallet.GetPublicKeyStringBase64(),
                        this.networkWallet.GetPublicKeyStringBase64(),
                        2000
                    );
                    compromisedTransaction.Amount += -100;
                    compromisedTransaction.SignTransaction(this.testWallet);
                    
                    this.chain.AddTransaction(compromisedTransaction);
                    
                    Assert.That(this.chain.UnconfirmedTransactions.Count , Is.EqualTo(0));
                    break;
            }
        }

        [TestCase("", TestName = "Test case #1, Testing by passing empty string to MineUnconfirmedTransactions")]
        [TestCase(null, TestName = "Test case #2, Testing by passing null reference to MineUnconfirmedTransactions")]
        [TestCase("atRuntimeMinerPublicKey", TestName = "Test case #3, Testing by passing public key to MineUnconfirmedTransactions")]
        public void Blockchain_MinesUnconfirmedTransactions_Correctly(string minerPublicKey)
        {
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );
            
            // Add unconfirmed transactions to Blockchain
            List<TransactionNS.Transaction> transactions = TransactionNS.Transaction.GenerateRandomTransactions(10, false);
            int transactionsToAdd = 10;
            for (int i = 0; i < transactionsToAdd + 1; i++)
            {
                transactions.Add(new TransactionNS.Transaction(
                        this.testWallet.GetPublicKeyStringBase64(),
                        this.networkWallet.GetPublicKeyStringBase64(),
                        100 * i + 1
                    )
                );
                transactions[i].SignTransaction(this.testWallet);
                this.chain.AddTransaction(transactions[i]);
            }

            switch (minerPublicKey)
            {
                case "":
                    try
                    {
                        this.chain.MineUnconfirmedTransactions(minerPublicKey);
                        Assert.Fail("Blockchain should not mine transactions with empty string public key miner");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                case null:
                    try
                    {
                        this.chain.MineUnconfirmedTransactions(null);
                        Assert.Fail("Blockchain should not mine transactions with null public key miner");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                case "atRuntimeMinerPublicKey":
                    minerPublicKey = this.testWallet.GetPublicKeyStringBase64(); // get wallet key at runtime
                    this.chain.MineUnconfirmedTransactions(minerPublicKey);
                    
                    Assert.That(this.chain.UnconfirmedTransactions.Count, Is.EqualTo(0));
                    Assert.That(this.chain.Chain.Count, Is.EqualTo(2)); // 2 block; 1 Genesis 1 for transactions
                    Assert.That(this.chain.IsValid(), Is.True);
                        
                    break;
            }
        }

        [Test]
        public void Blockchain_SavesJsonCorrectly_ToFile()
        {
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );
            
            BlockchainNS.Blockchain.SaveJsonStateToFile(this.chain.ToJsonString(), @"TEST_ZRD.json");
            
            // Check that file exists and that there is content in file "ZRD.json"
            string output = File.ReadAllText(@"TEST_ZRD.json");
            
            Assert.That(output, Is.Not.Empty);
        }

        [Test]
        public void Static_Blockchain_CanDeserializeFileJson_Correctly()
        {
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), testWallet.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            this.chain = BlockchainNS.Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 10,
                reward: 10,
                filepathToState: "TEST_ZRD.json"
            );
            
            BlockchainNS.Blockchain.SaveJsonStateToFile(this.chain.ToJsonString(), @"TEST_ZRD.json");
            
            // Check that the function returns null when json cannot be deserialized
            // by passing wrong filepath to deserializer
            Assert.That(BlockchainNS.Blockchain.FileDataToBlockchainInstance(@"TEST_ZRDx.json"), Is.Null);
            
            // Load a valid blockchain state in JSON format from 'ZRD.json'
            BlockchainNS.Blockchain deserializedChain = BlockchainNS.Blockchain.FileDataToBlockchainInstance(@"TEST_ZRD.json");

            // Assert that string data was successfully deserialized into the Blockchain instance
            Assert.That(deserializedChain, Is.InstanceOf(typeof(BlockchainNS.Blockchain)));
            Assert.That(deserializedChain.IsValid(), Is.True);
            
            // Compare blockchain data by comparing objects (uses overriden operator)
            Assert.That(this.chain.ToJsonString().Equals(deserializedChain.ToJsonString()), Is.True);

        }

    }
}