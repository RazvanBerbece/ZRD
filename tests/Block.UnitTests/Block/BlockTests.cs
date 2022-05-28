using BlockchainNS;
using BlockNS;
using NUnit.Framework;
using TransactionNS;
using System.Collections.Generic;
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
        private Blockchain chain;
        
        private Wallet networkWallet; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet

        [SetUp]
        public void Setup()
        {
            
            // Setup wallets
            networkWallet = new Wallet(1024);
            walletA = new Wallet(1024);

            randomUnsignedTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 10);
            emptyList = new List<Transaction> { };

            // Setup a generic unvalidated block
            genericUnvalidatedBlock = new Block(
                this.randomUnsignedTransactions,
                "previousHash",
                1
            );
            genericUnvalidatedBlock.CalculateHash();
            
            // Setup testing Blockchain
            // Setup blockchain
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
            blockWithTransactions.hash = blockWithTransactions.CalculateHash();
            Assert.IsNotEmpty(blockWithTransactions.hash);

            // Calculate hash with empty transaction list and index=0
            Block blockEmptyTransactions = new Block(this.emptyList, "publicKey123", 0);
            blockEmptyTransactions.hash = blockEmptyTransactions.CalculateHash();
            Assert.IsNotEmpty(blockEmptyTransactions.hash);
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
            MatchCollection hashMatches = hashExpression.Matches(this.genericUnvalidatedBlock.hash);

            Assert.Greater(hashMatches.Count, 0);
        }

        [Test]
        public void Block_Converts_ToJSONString()
        {
            string genericUnvalidatedBlockJSONString = this.genericUnvalidatedBlock.ToJSONString();

            string expectedOutput =
                $"{{\n\tIndex: {this.genericUnvalidatedBlock.index},\n\tTimestamp: {this.genericUnvalidatedBlock.timestamp.ToLongTimeString()},\n\tCurrent Hash: \"{this.genericUnvalidatedBlock.hash}\",\n\tPrevious Hash: \"{this.genericUnvalidatedBlock.previousHash}\",\n\tPoW: {this.genericUnvalidatedBlock.proofOfWork}\n}},";

            Assert.AreEqual(genericUnvalidatedBlockJSONString, expectedOutput);
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
