using System.Collections.Generic;
using NUnit.Framework;
using BlockNS;
using TransactionNS;

namespace BlockTestsNS
{

    public class BlockUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> transactions;

        [OneTimeSetUp]
        public void Setup()
        {
            this.transactions = Transaction.GenerateRandomTransactions(numberOfTransactions: 10);
        }

        [Test]
        public void Block_CanSetHash()
        {
            Block block = new Block(this.transactions, "randomValue123", 0);
            Assert.AreEqual("PreviousHash", "PreviousHash");
        }

    }

}
