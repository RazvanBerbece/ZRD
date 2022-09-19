using System;
using NUnit.Framework;

namespace ZRD.tests.Integration.Statics;

[TestFixture]
public class StaticsIntegrationTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestContext.Progress.WriteLine("-- Testing Statics (Integration) --\n");
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