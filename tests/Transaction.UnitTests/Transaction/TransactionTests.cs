using NUnit.Framework;
using TransactionNS;
using System.Collections.Generic;
using System;
using WalletNS;
using System.ComponentModel;

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

                    Console.WriteLine($"GenerateRandomTransactions({numberOfTransactions}) finished after {this.watch.ElapsedMilliseconds}ms\n");

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

        [TestCase(1024)]
        [TestCase(2048)]
        [TestCase(4096)]
        public void Transaction_CanBeSigned(int signatureSize)
        {
            // Setup test wallet
            Wallet wallet = new Wallet(signatureSize);

            // Setup transaction to be signed
            Transaction transaction = new Transaction(wallet.GetPublicKeyStringBase64(), "receiverPublicKey", 9999);

            // Sign transaction
            transaction.SignTransaction(wallet);

            // Verify that transaction was signed
            Assert.NotNull(transaction.signature);
        }

        [TestCase("senderPublicKey", "receiverPublicKey", 2000, true)]
        // [TestCase("", "", , false)] <- TODO: BUILD THIS TEST CASE TO FORCE IsValid() TO RETURN FALSE
        [TestCase("senderPublicKey", "", 2000, false)]
        [TestCase("", "receiverPublicKey", 2000, false)]
        [TestCase("", "receiverPublicKey", -2000, false)]
        [TestCase("senderPublicKey", "receiverPublicKey", 0, false)]
        [TestCase("", "", -1, false)]
        public void Transaction_CanValidate(string senderKey, string receiverKey, int amount, bool expectedValidation)
        {
            try
            {
                // Transaction to be asserted by case
                Transaction transaction = new Transaction(senderKey, receiverKey, amount);

                switch (expectedValidation)
                {
                    case true:
                        Assert.IsTrue(transaction.IsValid());
                        break;
                    case false:
                        Assert.IsFalse(transaction.IsValid());
                        break;
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                WarningException warning = new WarningException(
                    $"Transaction constructor threw {e.GetType()} to instantiate properly: {e.Message}\n" +
                    $"This is expected considering the TestCase params and it should pass as it is the correct behaviour when constructing a Transaction.\n"
                    );
                Console.Write(warning.ToString());
                Assert.Pass();
            }
            catch (ArgumentException e)
            {
                WarningException warning = new WarningException(
                    $"Transaction constructor threw {e.GetType()} to instantiate properly: {e.Message}\n" +
                    $"This is expected considering the TestCase params and it should pass as it is the correct behaviour when constructing a Transaction.\n"
                    );
                Console.Write(warning.ToString());
                Assert.Pass();
            }
            catch (NotImplementedException e)
            {
                WarningException warning = new WarningException(
                    $"Transaction code threw {e.GetType()}: {e.Message}\n" +
                    $"This is expected considering the TestCase params and it should pass as it is the correct behaviour when constructing a Transaction.\n"
                    );
                Console.Write(warning.ToString());
                Assert.Fail();
            }

        }

    }

}
