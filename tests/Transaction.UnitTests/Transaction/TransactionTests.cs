using NUnit.Framework;
using TransactionNS;
using System.Collections.Generic;
using System;

namespace TransactionTestsNS
{

    public class TransactionUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> list;

        [SetUp]
        public void Setup()
        {
            this.list = new List<Transaction> { };
        }

        [TearDown]
        public void Teardown()
        {
            this.list.Clear();
        }

        [TestCase("senderKey", "receiverKey", int.MinValue)]
        [TestCase("senderKey", "receiverKey", 0)]
        [TestCase("senderKey", "receiverKey", int.MaxValue)]
        [TestCase("", "receiverKey", int.MaxValue)]
        [TestCase("senderKey", "", int.MaxValue)]
        public void Transaction_ConstructsCorrectly(string senderKey, string receiverKey, int amount)
        {

            // Guard - ArgumentException for empty keys
            if (senderKey.Length == 0 || receiverKey.Length == 0)
            {
                try
                {
                    Transaction transaction = new Transaction(senderKey, receiverKey, amount);
                    Assert.Fail("Transaction constructor should throw ArgumentException for empty keys");
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(ArgumentException))
                    {
                        Assert.Pass();
                    }
                }
            }

            // Guard - ArgumentOutOfRangeException for non-valid transaction amounts
            if (amount <= 0)
            {
                try
                {
                    Transaction transaction = new Transaction(senderKey, receiverKey, amount);
                    Assert.Fail("Transaction constructor should throw ArgumentOutOfRangeException for equal or smaller than 0 transaction amounts");
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(ArgumentOutOfRangeException))
                    {
                        Assert.Pass();
                    }
                }
            }

        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(999)]
        public void Static_Can_GenerateRandomTransactions(int numberOfTransactions)
        {

            // Guard - List is null for 0 transactions
            if (numberOfTransactions <= 0)
            {
                try
                {
                    // Generate 
                    list = Transaction.GenerateRandomTransactions(numberOfTransactions);
                    Assert.Fail("GenerateRandomTransactions() should throw ArgumentOutOfRangeException if asked to generate a list of 0 or less Transactions");
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(ArgumentOutOfRangeException))
                    {
                        Assert.Pass();
                    }
                }
            }
            else
            {
                try
                {
                    // Generate 
                    list = Transaction.GenerateRandomTransactions(numberOfTransactions);

                    // Guard - List length
                    Assert.That(list.Count == numberOfTransactions);

                    // Guard - List items have populated data
                    foreach (Transaction transaction in list)
                    {
                        Assert.IsNotEmpty(transaction.id);
                        Assert.IsNotEmpty(transaction.hash);
                        Assert.IsNotEmpty(transaction.Sender);
                        Assert.IsNotEmpty(transaction.Receiver);
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail($"No exception should be thrown for numberOfTransactions={numberOfTransactions}\n{e.Message}");
                }
            }

        }

    }

}
