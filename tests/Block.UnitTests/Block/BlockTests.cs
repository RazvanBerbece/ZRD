using System;
using BlockchainNS;
using BlockNS;
using NUnit.Framework;
using TransactionNS;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using WalletNS;

namespace BlockTestsNS
{

    public class BlockUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> randomUnsignedTransactions;
        private List<Transaction> randomSignedTransactions;
        private List<Transaction> emptyList;
        private Block genericUnvalidatedBlock;
        private Block genericValidatedBlock;
        private Block genericBlockToJsonSerialize;
        private Blockchain chain;
        
        private Wallet networkWallet; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet

        [SetUp]
        public void Setup()
        {
            
            // Setup wallets
            networkWallet = new Wallet(1024);
            walletA = new Wallet(1024);

            randomUnsignedTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 10, false);
            emptyList = new List<Transaction> { };

            // Setup a generic unvalidated block
            genericUnvalidatedBlock = new Block(
                this.randomUnsignedTransactions,
                "previousHash",
                1
            );
            genericUnvalidatedBlock.CalculateHash();
            
            // Setup generic block to JSON serialize
            // Force keys, ids and timestamps to be the same across multiple test runs
            // so that the hashes remain the same
            List<Transaction> transactionsToSerialize = new List<Transaction>() { };
            transactionsToSerialize.Add(
                new Transaction(
                    "publicKey123",
                    "publicKey456",
                    1000,
                    id: "id123"
                    )
                );
            this.genericBlockToJsonSerialize = new Block(
                transactionsToSerialize,
                "previousHash",
                99
                );
            genericBlockToJsonSerialize.Timestamp = DateTime.Parse("2022-06-01T17:49:36.823434+01:00").ToLocalTime();
            
            // Setup testing Blockchain
            chain = Blockchain.CreateBlockchain(
                firstMint: new Transaction(
                    this.networkWallet.GetPublicKeyStringBase64(),
                    this.walletA.GetPublicKeyStringBase64(),
                    1000000
                ),
                blockchainWallet: this.networkWallet,
                difficulty: 2,
                blockTime: 5,
                reward: 420
            );
            
            // Sign transactions for validations
            randomSignedTransactions = new List<Transaction> { };
            randomSignedTransactions.Add(new Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 2000));
            randomSignedTransactions.Add(new Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 1500));
            randomSignedTransactions.Add(new Transaction(walletA.GetPublicKeyStringBase64(), networkWallet.GetPublicKeyStringBase64(), 20));
            foreach (Transaction transaction in randomSignedTransactions)
            {
                transaction.SignTransaction(walletA);
            }
            
            // Setup a generic validated block
            genericValidatedBlock = new Block(
                this.randomSignedTransactions,
                "previousHash",
                2
            );
            genericValidatedBlock.CalculateHash();

        }

        [Test]
        public void Block_CanCalculateHash()
        {
            // Calculate hash with random transaction list and index=0
            Block blockWithTransactions = new Block(this.randomUnsignedTransactions, "publicKey123", 0);
            blockWithTransactions.Hash = blockWithTransactions.CalculateHash();
            Assert.IsNotEmpty(blockWithTransactions.Hash);

            // Calculate hash with empty transaction list and index=0
            try
            {
                Block blockEmptyTransactions = new Block(this.emptyList, "publicKey123", 0);
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

            string expectedOutput = File.ReadAllText("../../../tests/Block.UnitTests/Block/ExpectedJsonString.txt");
            
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
