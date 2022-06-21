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

        [Test]
        public void LightweightNode_CanConstruct()
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            Assert.That(node, Is.InstanceOf(typeof(LightweightNode)));
            Assert.That(node.GetPublicNatIpAddressString().Equals(Statics.GetExternalPublicIpAddress().ToString()), Is.True);
        }
        
        [Test]
        public void LightweightNode_CanSyncBlockchainFromUpstream()
        {
            Assert.Fail();
        }

        [Test]
        public void LightweightNode_CanSendBlockchainUpstream()
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
        public void LightweightNode_CanSetIpAddress(bool isNullIpAddress)
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
        public void LightweightNode_CanGetIpAddressString()
        {
            LightweightNode node = LightweightNode.ConfigureNode();
            Assert.That(node.GetPublicNatIpAddressString().Equals(Statics.GetExternalPublicIpAddress().ToString()), Is.True);
        }
        
    }
}