using System;
using NUnit.Framework;

namespace MerkleTreeNS.MerkleNodeNS
{
    public class MerkleNodeTests
    {

        // Generic values which are Setup for every test

        [Test]
        public void MerkleNode_CanConstruct()
        {
            // Hash value used for all cases
            string value = "NodeHashValueOfParentsHash";

            // Node values for different cases
            MerkleNode NodeNullParents = new MerkleNode(value, null, null);
            MerkleNode NodeNullLeft = new MerkleNode(value, null, NodeNullParents);
            MerkleNode NodeNullRight = new MerkleNode(value, NodeNullParents, null);
            MerkleNode NodeFull = new MerkleNode(value, NodeNullParents, NodeNullParents);
            MerkleNode NodeNoValue;

            // NodeNullParents
            Assert.IsNotEmpty(NodeNullParents.value, "NodeNullParents should have a non-empty value");
            Assert.IsNull(NodeNullParents.left, "NodeNullParents should have the left parent null");
            Assert.IsNull(NodeNullParents.right, "NodeNullParents should have the right parent null");

            // NodeNullLeft
            Assert.IsNotEmpty(NodeNullParents.value, "NodeNullLeft should have a non-empty value");
            Assert.IsNull(NodeNullLeft.left, "NodeNullLeft should have the left parent null");
            Assert.IsInstanceOf(typeof(MerkleNode), NodeNullLeft.right, "NodeNullLeft should have the right parent a MerkleNode");

            // NodeNullRight
            Assert.IsNotEmpty(NodeNullRight.value, "NodeNullRight should have a non-empty value");
            Assert.IsNull(NodeNullRight.right, "NodeNullRight should have the right parent null");
            Assert.IsInstanceOf(typeof(MerkleNode), NodeNullRight.left, "NodeNullRight should have the left parent a MerkleNode");

            // NodeFull
            Assert.IsNotEmpty(NodeFull.value, "NodeFull should have a non-empty value");
            Assert.IsInstanceOf(typeof(MerkleNode), NodeFull.left, "NodeFull should have the left parent a MerkleNode");
            Assert.IsInstanceOf(typeof(MerkleNode), NodeFull.right, "NodeFull should have the right parent a MerkleNode");

            // NodeNoValue
            try
            {
                NodeNoValue = new MerkleNode("", NodeNullParents, NodeNullParents);
                Assert.Fail("MerkleNode constructor with empty string value should throw ArgumentException");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

        }

    }
}
