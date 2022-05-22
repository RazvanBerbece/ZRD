using System.Collections.Generic;
using NUnit.Framework;
using BlockNS;
using TransactionNS;
using System.Text.RegularExpressions;

namespace BlockTestsNS
{

    public class BlockUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> randomTransactions;
        private List<Transaction> emptyList;
        private Block genericBlock;

        [SetUp]
        public void Setup()
        {
            this.randomTransactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 10);
            this.emptyList = new List<Transaction> { };

            // Setup a generic block
            this.genericBlock = new Block(
                Transaction.GenerateRandomTransactions(numberOfTransactions: 25),
                "previousHash",
                1
            );
            this.genericBlock.CalculateHash();
            
        }

        [Test]
        public void Block_CanCalculateHash()
        {
            // Calculate hash with random transaction list and index=0
            Block blockWithTransactions = new Block(this.randomTransactions, "publicKey123", 0);
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
            this.genericBlock.Mine(difficulty);

            // Setup expected regex patern
            string regexHashPattern = $"^(0){{{difficulty}}}.*";
            Regex hashExpression = new Regex(regexHashPattern, RegexOptions.Compiled);
            MatchCollection hashMatches = hashExpression.Matches(this.genericBlock.hash);

            Assert.Greater(hashMatches.Count, 0);
        }

        [Test]
        public void Block_Converts_ToJSONString()
        {
            string genericBlockJSONString = this.genericBlock.ToJSONString();

            string expectedOutput =
                $"{{\n\tIndex: {this.genericBlock.index},\n\tTimestamp: {this.genericBlock.timestamp.ToLongTimeString()},\n\tCurrent Hash: \"{this.genericBlock.hash}\",\n\tPrevious Hash: \"{this.genericBlock.previousHash}\",\n\tPoW: {this.genericBlock.proofOfWork}\n}},";

            Assert.AreEqual(genericBlockJSONString, expectedOutput);
        }

    }

}
