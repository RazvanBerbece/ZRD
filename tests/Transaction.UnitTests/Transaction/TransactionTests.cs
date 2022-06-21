using BlockchainNS;
using NUnit.Framework;
using TransactionNS;
using System.Collections.Generic;
using System;
using WalletNS;
using System.ComponentModel;
using System.IO;

namespace TransactionTestsNS
{

    public class TransactionUnitTests
    {

        // Generic values which are Setup for every test
        private List<Transaction> list;
        private System.Diagnostics.Stopwatch watch;

        private Wallet networkWallet; // used for rewards, first mint, etc.
        private Wallet walletA; // main wallet
        private Wallet walletB; // secondary wallet

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            this.list = new List<Transaction> { };
            this.watch = new System.Diagnostics.Stopwatch();
            this.networkWallet = new Wallet(1024);
            this.walletA = new Wallet(1024);
            this.walletB = new Wallet(1024);
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
                    Transaction _ = new Transaction(senderKey, receiverKey, amount);
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
                    Transaction _ = new Transaction(senderKey, receiverKey, amount);
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
                    list = Transaction.GenerateRandomTransactions(numberOfTransactions, false);
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
                    list = Transaction.GenerateRandomTransactions(numberOfTransactions, false);
                    this.watch.Stop();

                    Console.WriteLine($"GenerateRandomTransactions({numberOfTransactions}) finished after {this.watch.ElapsedMilliseconds}ms\n");

                    // Guard - List length
                    Assert.That(list.Count == numberOfTransactions);

                    // Guard - List items have populated data
                    foreach (Transaction transaction in list)
                    {
                        Assert.IsNotEmpty(transaction.Id);
                        Assert.IsNotEmpty(transaction.Hash);
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
            Assert.NotNull(transaction.Signature);
        }

        [TestCase("senderPublicKey", "receiverPublicKey", 2000, true)]
        [TestCase("placeholder", "placeholder", 2000, false)]
        [TestCase("senderPublicKey", "", 2000, false)]
        [TestCase("", "receiverPublicKey", 2000, false)]
        [TestCase("", "receiverPublicKey", -2000, false)]
        [TestCase("senderPublicKey", "receiverPublicKey", 0, false)]
        [TestCase("", "", -1, false)]
        public void Transaction_CanValidate(string senderKey, string receiverKey, int amount, bool expectedValidation)
        {
            try
            {
                // Setup blockchain
                Blockchain blockchain = Blockchain.CreateBlockchain(
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

                // Transaction instatiation to be caught  
                Transaction _ = new Transaction(senderKey, receiverKey, amount);

                // Actual transaction to validate with correct format public keys
                Transaction transactionToValidate = new Transaction(this.walletA.GetPublicKeyStringBase64(), this.walletB.GetPublicKeyStringBase64(), amount);
                transactionToValidate.SignTransaction(this.walletA);
                
                switch (expectedValidation)
                {
                    case true:
                        Assert.IsTrue(transactionToValidate.IsValid(blockchain));
                        break;
                    case false:
                        // At this point in code the transaction will be meddled with and the test case will assert that the validation fails
                        transactionToValidate.Amount += 1000;
                        Assert.IsFalse(transactionToValidate.IsValid(blockchain));
                        break;
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                WarningException warning = new WarningException(
                    $"Transaction constructor threw {e.GetType()} and failed to instantiate properly: {e.Message}\n" +
                    $"This is expected considering the TestCase params and it should pass as it is the correct behaviour when constructing a Transaction.\n"
                    );
                Console.Write(warning.ToString());
                Assert.Pass();
            }
            catch (ArgumentException e)
            {
                WarningException warning = new WarningException(
                    $"Transaction constructor threw {e.GetType()} and failed to instantiate properly: {e.Message}\n" +
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

        [Test]
        public void Transaction_CanSerializeToJsonString()
        {
            Transaction transaction = new Transaction(this.walletA.GetPublicKeyStringBase64(), "receiverPublicKey", 9999);
            transaction.SignTransaction(this.walletA);
            Assert.That(string.IsNullOrEmpty(transaction.ToJsonString()), Is.False);
        }

        [Test]
        public void Transaction_CanDeserializeFromJsonString()
        {
            // Generate expected transaction to compare output with
            Transaction expectedTransaction = new Transaction(this.walletA.GetPublicKeyStringBase64(), "receiverPublicKey", 9999);
            expectedTransaction.Id = "2ce4d267-0709-4372-8415-971663529079";
            expectedTransaction.SignTransaction(this.walletA);
            
            // Get JSON transaction string from file and deserialize
            string jsonString = File.ReadAllText("../../../tests/Transaction.UnitTests/Transaction/ExpectedJsonString.txt");
            Transaction actualTransaction = Transaction.JsonStringToTransactionInstance(jsonString);
            
            // Assert that fields match
            // Note: Signature won't match due to it using the current timestamp
            // TODO: Maybe dynamic ExpectedJsonString.txt file ? 
            Assert.That(actualTransaction, Is.InstanceOf(typeof(Transaction)));
            Assert.That(actualTransaction.Id == expectedTransaction.Id, Is.True);
            Assert.That(actualTransaction.Amount == expectedTransaction.Amount, Is.True);
        }

    }

}
