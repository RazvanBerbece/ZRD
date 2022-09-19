using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BlockchainNS;
using NUnit.Framework;
using WalletNS.BlockchainWalletNS;

namespace ZRD.tests.Unit.Block.Block
{

    public class BlockUnitTests
    {

        // Generic values which are Setup for every test
        private List<TransactionNS.Transaction> randomUnsignedTransactions;
        private List<TransactionNS.Transaction> randomSignedTransactions;
        private List<TransactionNS.Transaction> emptyList;
        private BlockNS.Block genericUnvalidatedBlock;
        private BlockNS.Block genericValidatedBlock;
        private BlockNS.Block genericBlockToJsonSerialize;
        private Blockchain chain;
        
        private BlockchainWallet networkWallet; // used for rewards, first mint, etc.
        private WalletNS.Wallet walletA; // main wallet

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing Block --\n");
        }

        [SetUp]
        public void Setup()
        {
            
            // Setup wallets
            networkWallet = new BlockchainWallet(1024, "NETWORK_WALLET_PARAMS.xml");
            walletA = new WalletNS.Wallet(1024, "USER_WALLET_PARAMS_1.xml");

            randomUnsignedTransactions = TransactionNS.Transaction.GenerateRandomTransactions(numberOfTransactions: 10, false);
            emptyList = new List<TransactionNS.Transaction> { };

            // Setup a generic unvalidated block
            genericUnvalidatedBlock = new BlockNS.Block(
                this.randomUnsignedTransactions,
                "previousHash",
                1
            );
            genericUnvalidatedBlock.CalculateHash();
            
            // Setup generic block to JSON serialize
            // Force keys, ids and timestamps to be the same across multiple test runs
            // so that the hashes remain the same
            List<TransactionNS.Transaction> transactionsToSerialize = new List<TransactionNS.Transaction>() { };
            transactionsToSerialize.Add(
                new TransactionNS.Transaction(
                    "publicKey123",
                    "publicKey456",
                    1000,
                    id: "id123"
                    )
                );
            this.genericBlockToJsonSerialize = new BlockNS.Block(
                transactionsToSerialize,
                "previousHash",
                99
                );
            // An Issue was observed where running the test Block.ToJsonString() on GitHub Actions would lead to parsing a different timestamp
            // to the one provided due to CultureInfo system settings
            // Adjusting to Universal with no culture info to have matching behaviour on different machines solved the issue
            // We force a timestamp that we can hardcode in ExpectedJsonString.json for testing
            genericBlockToJsonSerialize.Timestamp = DateTime.Parse("2022-06-01T17:49:36.823434+01:00", null, System.Globalization.DateTimeStyles.AdjustToUniversal);
            
            const int firstAmount = 1000000;
            List<TransactionNS.Transaction> initialCoinOfferings = new List<TransactionNS.Transaction>()
            {
                new TransactionNS.Transaction(networkWallet.GetPublicKeyStringBase64(), walletA.GetPublicKeyStringBase64(),
                    firstAmount),
            };
            // Setup test blockchain
            // Setup testing Blockchain
            chain = Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 5,
                reward: 420,
                filepathToState: "TEST_ZRD.json"
            );
            
            // Sign transactions for validations
            randomSignedTransactions = new List<TransactionNS.Transaction> { };
            randomSignedTransactions.Add(new TransactionNS.Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 2000));
            randomSignedTransactions.Add(new TransactionNS.Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 1500));
            randomSignedTransactions.Add(new TransactionNS.Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 20));
            foreach (TransactionNS.Transaction transaction in randomSignedTransactions)
            {
                transaction.SignTransaction(walletA);
            }
            
            // Setup a generic validated block
            genericValidatedBlock = new BlockNS.Block(
                this.randomSignedTransactions,
                "previousHash",
                2
            );
            genericValidatedBlock.CalculateHash();

        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("NETWORK_WALLET_PARAMS.xml"))
            {
                File.Delete("NETWORK_WALLET_PARAMS.xml");
            }
            if (File.Exists("USER_WALLET_PARAMS_1.xml"))
            {
                File.Delete("USER_WALLET_PARAMS_1.xml");
            }
        }

        [Test]
        public void Block_CanCalculateHash()
        {
            // Calculate hash with random transaction list and index=0
            BlockNS.Block blockWithTransactions = new BlockNS.Block(this.randomUnsignedTransactions, "publicKey123", 0);
            blockWithTransactions.Hash = blockWithTransactions.CalculateHash();
            Assert.IsNotEmpty(blockWithTransactions.Hash);

            // Calculate hash with empty transaction list and index=0
            try
            {
                BlockNS.Block blockEmptyTransactions = new BlockNS.Block(this.emptyList, "publicKey123", 0);
                Assert.Fail("It should not be possible to create a Block with a null MerkleTree representation");
            }
            catch (Exception)
            {
                Assert.Pass();
            }
            
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Block_CanBeMined(int difficulty)
        {
            // Mine new block
            this.genericUnvalidatedBlock.Mine(difficulty);

            // Setup expected regex patern
            string regexHashPattern = $"^(0){{{difficulty}}}.*";
            Regex hashExpression = new Regex(regexHashPattern, RegexOptions.Compiled);
            MatchCollection hashMatches = hashExpression.Matches(this.genericUnvalidatedBlock.Hash);

            Assert.Greater(hashMatches.Count, 0);
        }

        [Test]
        public void Block_Converts_ToJsonString()
        {
            string genericBlockJsonSerializerJsonString = this.genericBlockToJsonSerialize.ToJsonString();

            string expectedOutput = File.ReadAllText("../../../tests/Unit/Block/Block/ExpectedJsonString.txt");
            
            Assert.AreEqual(expectedOutput, genericBlockJsonSerializerJsonString);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Block_ValidatesTransactions(bool expectedResult)
        {
            switch (expectedResult)
            {
                case true:
                    Assert.That(this.genericValidatedBlock.HasAllValidTransactions(this.chain), Is.True);
                    break;
                case false:
                    // The transactions in the genericUnvalidatedBlock are all unsigned
                    // so HasAllValidTransactions() should return false
                    Assert.That(this.genericUnvalidatedBlock.HasAllValidTransactions(this.chain), Is.Not.True);
                    break;
            }
        }

    }

}
