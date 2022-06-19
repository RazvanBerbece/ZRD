using Peer2PeerNS.NodesNS.LightweightNodeNS;
using NUnit.Framework;

namespace Peer2PeerNS.NodesNS.LightweightNodeNS
{
    [TestFixture]
    public class LightweightNodeTests
    {
        
        private LightweightNode node;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // node = LightweightNode.ConfigureLightweightNode();
        }

        [Test]
        public void LightweightNode_CanConstruct()
        {
            Assert.Pass();
        }
        
        [Test]
        public void LightweightNode_CanSyncBlockchainFromUpstream()
        {
            Assert.Pass();
        }

        [Test]
        public void LightweightNode_CanSendBlockchainUpstream()
        {
            Assert.Pass();
        }
        
        [Test]
        public void LightweightNode_CanCreateWallet()
        {
            Assert.Pass();
        }

    }
}