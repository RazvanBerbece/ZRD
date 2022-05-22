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
        private System.Diagnostics.Stopwatch watch;

        [OneTimeSetUp]
        public void Setup()
        {
            this.list = new List<Transaction> { };
            this.watch = new System.Diagnostics.Stopwatch();
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
        [TestCase(99)]
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
                    // Generate & time
                    // It seems the function is slow for bigger numberOfTransactions
                    //
                    // Example runs :
                    // numberOfTransactions = 5 => 1026ms = 1.026s
                    // numberOfTransactions = 7 => 1548ms = 1.548s
                    // numberOfTransactions = 99 => 20584ms = 20.584s
                    this.watch.Start();
                    list = Transaction.GenerateRandomTransactions(numberOfTransactions);
                    this.watch.Stop();
                    Console.WriteLine($"\nGenerateRandomTransactions({numberOfTransactions}) finished after {this.watch.ElapsedMilliseconds}ms\n");

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
