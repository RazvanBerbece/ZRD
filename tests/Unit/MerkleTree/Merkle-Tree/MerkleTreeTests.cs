﻿using System;
using System.Collections.Generic;
using MerkleTreeNS.MerkleNodeNS;
using NUnit.Framework;
using TransactionNS;

namespace ZRD.tests.Unit.MerkleTree.Merkle_Tree
{
    public class MerkleTreeTests
    {
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing MerkleTree --\n");
        }
        
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
                List<TransactionNS.Transaction> emptyTransactionsList = new List<TransactionNS.Transaction>() { };
                
                // Use Static MerkleTree function to generate tree
                MerkleTreeNS.MerkleTree tree = MerkleTreeNS.MerkleTree.CreateMerkleTree(emptyTransactionsList);
                
                Assert.That(tree, Is.Null);
            }
            else
            {
                List<TransactionNS.Transaction> transactions = TransactionNS.Transaction.GenerateRandomTransactions(numberOfTransactions, false);
                
                // Use Static MerkleTree function to generate tree
                MerkleTreeNS.MerkleTree tree = MerkleTreeNS.MerkleTree.CreateMerkleTree(transactions);

                // Check tree status by checking instance type & other data (size, root instance type, etc.)
                Assert.That(tree, Is.InstanceOf(typeof(MerkleTreeNS.MerkleTree)));
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
            MerkleNode rootNode = MerkleTreeNS.MerkleTree.OrganiseTreeFromMerkleNodeList(nodes);

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
