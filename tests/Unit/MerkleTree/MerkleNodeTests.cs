using System;
using MerkleTreeNS.MerkleNodeNS;
using NUnit.Framework;

namespace ZRD.tests.Unit.MerkleTree
{
    public class MerkleNodeTests
    {

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing MerkleNode --\n");
        }

        [Test]
        public void MerkleNode_CanConstruct()
        {
            // Hash value used for all cases
            string value = "NodeHashValueOfParentsHash";

            // Node values for different cases
            MerkleNode nodeNullParents = new MerkleNode(value, null, null);
            MerkleNode nodeNullLeft = new MerkleNode(value, null, nodeNullParents);
            MerkleNode nodeNullRight = new MerkleNode(value, nodeNullParents, null);
            MerkleNode nodeFull = new MerkleNode(value, nodeNullParents, nodeNullParents);
            MerkleNode nodeNoValue;

            // NodeNullParents
            Assert.IsNotEmpty(nodeNullParents.Value, "NodeNullParents should have a non-empty value");
            Assert.IsNull(nodeNullParents.Left, "NodeNullParents should have the left parent null");
            Assert.IsNull(nodeNullParents.Right, "NodeNullParents should have the right parent null");

            // NodeNullLeft
            Assert.IsNotEmpty(nodeNullParents.Value, "NodeNullLeft should have a non-empty value");
            Assert.IsNull(nodeNullLeft.Left, "NodeNullLeft should have the left parent null");
            Assert.IsInstanceOf(typeof(MerkleNode), nodeNullLeft.Right, "NodeNullLeft should have the right parent a MerkleNode");

            // NodeNullRight
            Assert.IsNotEmpty(nodeNullRight.Value, "NodeNullRight should have a non-empty value");
            Assert.IsNull(nodeNullRight.Right, "NodeNullRight should have the right parent null");
            Assert.IsInstanceOf(typeof(MerkleNode), nodeNullRight.Left, "NodeNullRight should have the left parent a MerkleNode");

            // NodeFull
            Assert.IsNotEmpty(nodeFull.Value, "NodeFull should have a non-empty value");
            Assert.IsInstanceOf(typeof(MerkleNode), nodeFull.Left, "NodeFull should have the left parent a MerkleNode");
            Assert.IsInstanceOf(typeof(MerkleNode), nodeFull.Right, "NodeFull should have the right parent a MerkleNode");

            // NodeNoValue
            try
            {
                nodeNoValue = new MerkleNode("", nodeNullParents, nodeNullParents);
                Assert.Fail("MerkleNode constructor with empty string value should throw ArgumentException");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

        }

    }
}
