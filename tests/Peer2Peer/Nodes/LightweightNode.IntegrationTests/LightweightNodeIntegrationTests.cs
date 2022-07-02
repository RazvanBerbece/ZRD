using System.IO;
using TransactionNS;
using NUnit.Framework;
using WalletNS;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS.IntegrationTestsNS
{
    public class LightweightNodeIntegrationTests
    {

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing LightweightNode Scenarios --\n");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("REMOTE_WALLET_PARAMS.xml"))
            {
                File.Delete("REMOTE_WALLET_PARAMS.xml");
            }
        }

        [Test]
        public void LightweightNode_CanLogin_AndAttemptTransaction()
        {
            
            // Setup node
            LightweightNode node = LightweightNode.ConfigureNode();
            
            // Setup ext wallet & other transaction data
            Wallet remoteWallet = new Wallet(1024, "REMOTE_WALLET_PARAMS.xml");
            int transactionAmount = 1337;
            
            // Setup wallet - login
            Wallet testWallet = Wallet.DeserializeWalletFromJsonFile(
                "../../../tests/Peer2Peer/Nodes/LightweightNode.IntegrationTests/Wallet.json", 
                "../../../tests/Peer2Peer/Nodes/LightweightNode.IntegrationTests/Params/RSAConfig.xml"
                );
            node.SetWallet(testWallet);

            // Setup Transaction to send to full node
            Transaction transaction = new Transaction(
                node.Wallet.GetPublicKeyStringBase64(),
                remoteWallet.GetPublicKeyStringBase64(),
                transactionAmount
            );
            transaction.SignTransaction(node.Wallet);
            
            Assert.That(transaction.Signature, Is.Not.Empty);

        }
        
    }
}