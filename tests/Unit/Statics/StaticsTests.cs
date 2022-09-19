using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ZRD.tests.Unit.Statics;

[TestFixture]
public class StaticsTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestContext.Progress.WriteLine("-- Testing Statics --\n");
    }

    [TestCase("")]
    [TestCase("dataToEncode")]
    public void Can_CreateSHA256HashString_FromStringData(string data)
    {
        Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.CreateHashSha256(null));

        var outputHash = StaticsNS.Statics.CreateHashSha256(data);
        Assert.That(outputHash, Is.Not.Null);
        Assert.That(outputHash, Has.Length.GreaterThan(0));
    }

    [Test]
    public void Can_CreateSHA256HashString_FromTransaction()
    {
        var transaction = new TransactionNS.Transaction("id456", "id123", 1000);
        
        Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.CreateHashSha256FromTransaction(null));

        var outputHash = StaticsNS.Statics.CreateHashSha256FromTransaction(transaction);
        Assert.That(outputHash, Is.Not.Null);
        Assert.That(outputHash, Has.Length.GreaterThan(0));
    }

    [TestCase("google.com", 5000)]
    [TestCase("", 5000)]
    [TestCase(null, 5000)]
    [TestCase("google.com", 0)]
    [TestCase("google.com", -1)]
    public void Can_PingHost(string hostname, int timeoutInMs)
    {
        if (timeoutInMs <= 0)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StaticsNS.Statics.CanPingHost(hostname, timeoutInMs));
        }
        else
        {
            switch (hostname)
            {
                case null:
                    Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.CanPingHost(null, timeoutInMs));
                    break;
                case "":
                    Assert.Throws<ArgumentException>(() => StaticsNS.Statics.CanPingHost("", timeoutInMs));
                    break;
                default:
                    
                    // This is required as Github Actions machines run on Microsoft VMs
                    // where ICMP is disabled, so this test will fail upstream
                    // Thus, do not run this assertion if on VM
                    if (StaticsNS.Statics.RunsOnVm()) Assert.Pass("Test running on Microsoft VM. Automatically pass.");
                    
                    var canPingGoogle = StaticsNS.Statics.CanPingHost(hostname, timeoutInMs);
                    Assert.That(canPingGoogle, Is.True);
                    break;
            }   
        }
    }

    [Test]
    public void Can_Serialize_TransactionsToJsonString()
    {
        // Create transactions
        var transaction1 = new TransactionNS.Transaction("id456", "id123", 1000);
        var transaction2 = new TransactionNS.Transaction("id456", "id123", 1200);
        var transaction3 = new TransactionNS.Transaction("id456", "id123", 1400);
        var transaction4 = new TransactionNS.Transaction("id456", "id123", 1600);
        
        // Create lists used for assertions
        var transactions = new List<TransactionNS.Transaction> { transaction1, transaction2, transaction3, transaction4 };

        Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.TransactionsToJsonString(null));

        var actualStringOutput = StaticsNS.Statics.TransactionsToJsonString(transactions);
        Assert.That(actualStringOutput, Is.Not.Null);
        Assert.That(actualStringOutput, Is.InstanceOf<string>());
        Assert.That(actualStringOutput, Has.Length.GreaterThan(0));
    }

    [Test]
    public void Can_GetPublicIpFromPeerClient()
    {
        TcpClient peer = new TcpClient("google.com", 443);

        Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.GetPeerPublicIp(null));

        var actualIpOutput = StaticsNS.Statics.GetPeerPublicIp(peer);
        Assert.That(actualIpOutput, Is.Not.Null);
        Assert.That(actualIpOutput, Is.InstanceOf<string>());
       
        // Check that output is in IPv4 form
        var expression = new Regex(
            @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$", 
            RegexOptions.Compiled
        );
        var results = expression.Matches(actualIpOutput);
        if (results.Count != 0)
        {
            Assert.Pass();
        }
        else Assert.Fail();
    }

    [Test]
    public void Can_GetLocalIpAddress()
    {
        var actualIpOutput = StaticsNS.Statics.GetLocalIpAddress();
        
        Assert.That(actualIpOutput, Is.Not.Null);
        Assert.That(actualIpOutput, Is.InstanceOf<IPAddress>());
        // Check that output is in IPv4 form
        var expression = new Regex(
            @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$", 
            RegexOptions.Compiled
        );
        var results = expression.Matches(actualIpOutput.ToString());
        if (results.Count != 0)
        {
            Assert.Pass();
        }
        else Assert.Fail();
    }

    [Test]
    public void Can_CheckThatSignature_IsValid()
    {
        var walletSender = new WalletNS.Wallet(1024);
        var walletReceiver = new WalletNS.Wallet(1024);
        var signableTransaction = new TransactionNS.Transaction(
            walletSender.GetPublicKeyStringBase64(), 
            walletReceiver.GetPublicKeyStringBase64(), 
            1000);
        signableTransaction.SignTransaction(walletSender);
        
        // Get required data from transaction for signature check
        var bytesHash = Convert.FromBase64String(signableTransaction.Hash);
        var signatureHash = Convert.FromBase64String(signableTransaction.Signature);
        var transactionSignatureIsValid =
            StaticsNS.Statics.SignatureIsValid(
                bytesHash, signatureHash, walletSender.GetPublicKeyStringBase64());
        Assert.That(transactionSignatureIsValid, Is.True);
        
        // Assert on a transaction which does not have a valid signature
        // reuses above transaction by mutating it
        // Uses a little bit of a hack to slightly modify the hash :
        //  1. Noted the hash of the transaction built above
        //  2. Changed one digit in the hash string representation -> hash does not match actual data
        //  3. Signature should not verify now
        const string hackHash = "8a225c6695401271b141619967d933d1bae1808a8909d2a31ad1b7d0d6daad88";
        signableTransaction.Hash = hackHash;
        var bytesHashNotValid = Convert.FromBase64String(signableTransaction.Hash);
        var signatureHashNotValid = Convert.FromBase64String(signableTransaction.Signature);
        var actualInvalidTransactionSignatureCheck =
            StaticsNS.Statics.SignatureIsValid(
                bytesHashNotValid, signatureHashNotValid, walletSender.GetPublicKeyStringBase64());
        Assert.That(actualInvalidTransactionSignatureCheck, Is.False);
    }
    
}