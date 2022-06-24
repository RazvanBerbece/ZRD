using System;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using NUnit.Framework;
using StaticsNS;
using WalletNS;

namespace Peer2PeerNS.NodesNS.LightweightNodeTestsNS
{
    [TestFixture]
    public class LightweightNodeTests
    {
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing LightweightNode --\n");
        }

        [Test]
        public void LightweightNode_CanConstruct()
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            Assert.That(node, Is.InstanceOf(typeof(LightweightNode)));
            Assert.That(node.GetPublicNatIpAddressString().Equals(Statics.GetExternalPublicIpAddress().ToString()), Is.True);
        }
        
        [Test]
        public void LightweightNode_CanSendTransactionToPeer()
        {
            Assert.Fail();
        }

        [Test]
        public void LightweightNode_CanSendBlockchainToPeer()
        {
            Assert.Fail();
        }
        
        [TestCase(true, TestName = "Test case #1, Testing by passing null Wallet to setter")]
        [TestCase(false, TestName = "Test case #2, Testing by passing correct Wallet to setter")]
        public void LightweightNode_CanSetWallet(bool isNullWallet)
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            if (isNullWallet)
            {
                try
                {
                    node.SetWallet(null);
                    Assert.Fail("LightweightNode should not set wallet to null");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                Wallet wallet = new Wallet(1024);
                node.SetWallet(wallet);
                Assert.That(node.Wallet != default, Is.True);
            }
        }

        [TestCase(true, TestName = "Test case #1, Testing by passing null IPAddress to setter")]
        [TestCase(false, TestName = "Test case #2, Testing by passing correct IPAddress to setter")]
        public void LightweightNode_CanSetPublicIpAddress(bool isNullIpAddress)
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            if (isNullIpAddress)
            {
                try
                {
                    node.SetPublicNatIpAddress(null);
                    Assert.Fail("LightweightNode should not set IPAddress to null");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                node.SetPublicNatIpAddress(Statics.GetExternalPublicIpAddress());
                Assert.That(string.IsNullOrEmpty(node.GetPublicNatIpAddressString()), Is.Not.True);
            }
        }
        
        [Test]
        public void LightweightNode_CanGetPublicIpAddressString()
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            Assert.That(node.GetPublicNatIpAddressString().Equals(Statics.GetExternalPublicIpAddress().ToString()), Is.True);
        }
        
        [TestCase(true, TestName = "Test case #1, Testing by passing null IPAddress to setter")]
        [TestCase(false, TestName = "Test case #2, Testing by passing correct IPAddress to setter")]
        public void LightweightNode_CanSetPrivateIpAddress(bool isNullIpAddress)
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            if (isNullIpAddress)
            {
                try
                {
                    node.SetPrivateIpAddress(null);
                    Assert.Fail("LightweightNode should not set IPAddress to null");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                node.SetPrivateIpAddress(Statics.GetLocalIpAddress());
                Assert.That(string.IsNullOrEmpty(node.GetPrivateIpAddressString()), Is.Not.True);
            }
        }
        
        [Test]
        public void LightweightNode_CanGetPrivateIpAddressString()
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            Assert.That(node.GetPrivateIpAddressString().Equals(Statics.GetLocalIpAddress().ToString()), Is.True);
        }
        
    }
}