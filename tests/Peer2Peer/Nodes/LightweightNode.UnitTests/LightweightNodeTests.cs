using System;
using Peer2PeerNS.NodesNS.LightweightNodeNS;
using NUnit.Framework;
using System.Net;
using StaticsNS;
using WalletNS;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS
{
    [TestFixture]
    public class LightweightNodeTests
    {
        
        private LightweightNode node;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            node = LightweightNode.ConfigureLightweightNode();
        }

        [Test]
        public void LightweightNode_CanConstruct()
        {
            Assert.Fail();
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
            if (isNullWallet)
            {
                try
                {
                    this.node.SetWallet(null);
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
                this.node.SetWallet(wallet);
                Assert.That(this.node.Wallet != default, Is.True);
            }
        }

        [TestCase(true, TestName = "Test case #1, Testing by passing null IPAddress to setter")]
        [TestCase(false, TestName = "Test case #2, Testing by passing correct IPAddress to setter")]
        public void LightweightNode_CanSetIpAddress(bool isNullIpAddress)
        {
            if (isNullIpAddress)
            {
                try
                {
                    this.node.SetIpAddress(null);
                    Assert.Fail("LightweightNode should not set IPAddress to null");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                this.node.SetIpAddress(Statics.GetExternalPublicIpAddress());
                Assert.That(string.IsNullOrEmpty(node.GetIpAddressString()), Is.Not.True);
            }
        }
        
        [Test]
        public void LightweightNode_CanGetIpAddressString()
        {
            Assert.Fail();
        }
        
    }
}