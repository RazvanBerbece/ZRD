using System.Collections.Generic;
using NUnit.Framework;
using BlockNS;
using TransactionNS;

namespace BlockTestsNS
{

    public class BlockUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> randomTransactions;
        private List<Transaction> emptyList;
        private Block genericBlock;

        [OneTimeSetUp]
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

        [Test]
        public void Block_CanBeMined()
        {

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
