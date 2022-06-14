using MerkleTreeNS.MerkleNodeNS;
using MerkleTreeNS.MerkleTreeNS;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TransactionNS;

namespace MerkleTreeNS.MerkleTreeNS
{
    public class MerkleTreeTests
    {
        
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(99)]
        public void Static_MerkleTree_CanCreateMerkleTree(int numberOfTransactions)
        {
            // Generate transactions to organise in linked list manner -- for tree representation
            if (numberOfTransactions <= 0)
            {
                List<Transaction> emptyTransactionsList = new List<Transaction>() { };
                
                // Use Static MerkleTree function to generate tree
                MerkleTree tree = MerkleTree.CreateMerkleTree(emptyTransactionsList);
                
                Assert.That(tree, Is.Null);
            }
            else
            {
                List<Transaction> transactions = Transaction.GenerateRandomTransactions(numberOfTransactions, false);
                
                // Use Static MerkleTree function to generate tree
                MerkleTree tree = MerkleTree.CreateMerkleTree(transactions);

                // Check tree status by checking instance type & other data (size, root instance type, etc.)
                Assert.That(tree, Is.InstanceOf(typeof(MerkleTree)));
                Assert.That(tree.Root, Is.InstanceOf(typeof(MerkleNode)));
                Assert.That(tree.Size, Is.EqualTo(Math.Ceiling(Math.Log2(transactions.Count)) + 1));
            }
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(37)]
        public void Static_MerkleTree_CanOrganiseTreeFromMerkleNodeList(int numberOfNodes)
        {
            // Create list of simple MerkleNodes to organise
            List<MerkleNode> nodes = new List<MerkleNode>() { };
            for (int i = 0; i < numberOfNodes; ++i)
            {
                nodes.Add(
                    new MerkleNode(
                        "0xHashValue123-" + i.ToString(),
                        null,
                        null
                    )
                );
            }
            
            // Organise MerkleNodes -- process hashes based on parent hashes
            MerkleNode rootNode = MerkleTree.OrganiseTreeFromMerkleNodeList(nodes);

            // Assert resulted root node & the tree
            switch (numberOfNodes)
            {
                case -1:
                    Assert.That(rootNode, Is.Null);
                    break;
                case 0:
                    Assert.That(rootNode, Is.Null);
                    break;
                case 1:
                    // Whole MerkleTree consists of 1 root MerkleNode
                    Assert.That(rootNode, Is.InstanceOf(typeof(MerkleNode)));
                    Assert.That(rootNode.Left, Is.Null);
                    Assert.That(rootNode.Right, Is.Null);
                    break;
                default:
                    // Assert that parents are MerkleNodes
                    Assert.That(rootNode, Is.InstanceOf(typeof(MerkleNode)));
                    Assert.That(rootNode.Left, Is.InstanceOf(typeof(MerkleNode)));
                    Assert.That(rootNode.Right, Is.InstanceOf(typeof(MerkleNode)));
                    break;
            }

        }

    }
}
