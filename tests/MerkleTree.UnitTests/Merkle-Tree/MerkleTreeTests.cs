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
                List<Transaction> transactions = Transaction.GenerateRandomTransactions(numberOfTransactions);
                
                // Use Static MerkleTree function to generate tree
                MerkleTree tree = MerkleTree.CreateMerkleTree(transactions);

                // Check tree status by checking instance type & other data (size, root instance type, etc.)
                Assert.That(tree, Is.InstanceOf(typeof(MerkleTree)));
                Assert.That(tree.root, Is.InstanceOf(typeof(MerkleNode)));
                Assert.That(tree.size, Is.EqualTo(Math.Ceiling(Math.Log2(transactions.Count)) + 1));
            }
        }

    }
}
