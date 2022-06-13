using System;
using MerkleTreeNS.MerkleNodeNS;
using TransactionNS;
using System.Collections.Generic;
using StaticsNS;

namespace MerkleTreeNS
{
    public class MerkleTree
    {

        public double size { get; set; }
        public MerkleNode root { get; set; }

        public MerkleTree(MerkleNode root, double size)
        {
            this.size = size;
            this.root = root;
        }
        
        /// <summary>
        /// This function takes in a list of transactions and generates a MerkleTree based on the transaction list size
        /// and indexed elements.
        /// It creates a list of simple MerkleNodes with no parents, which are then organised in OrganiseTreeFromMerkleNodeList()
        /// </summary>
        /// <param name="transactions">List of transactions to be used in building list of MerkleNodes</param>
        /// <returns>MerkleTree instance</returns>
        public static MerkleTree CreateMerkleTree(List<Transaction> transactions)
        {

            if (transactions.Count <= 0)
            {
                return null;
            }
            
            double size = Math.Ceiling(Math.Log2(transactions.Count)) + 1;

            // Create list of MerkleNodes from list of transactions
            List<MerkleNode> nodes = new List<MerkleNode> { };
            foreach (Transaction transaction in transactions)
            {
                nodes.Add(new MerkleNode(Statics.CreateHashSHA256FromTransaction(transaction), null, null));
            }

            MerkleNode root = OrganiseTreeFromMerkleNodeList(nodes);

            return new MerkleTree(root, size);
        }

        /// <summary>
        /// This function takes in a list of MerkleNodes
        /// and organises it in a tree (list data structure for internal representation)
        /// by pushing MerkleNodes with parents and calculated value to a new list and returning it.
        /// </summary>
        /// <param name="transactionNodes">List of tree nodes organised so far. Starts initialised with MerkleNodes with no parents.</param>
        /// <returns>MerkleNode which can be used as root of the MerkleTree.</returns>
        public static MerkleNode OrganiseTreeFromMerkleNodeList(List<MerkleNode> transactionNodes)
        {

            if (transactionNodes.Count <= 0) return null;
            if (transactionNodes.Count == 1) return transactionNodes[0];

            List<MerkleNode> nodeList = new List<MerkleNode> { };
            int transactionNodesSize = transactionNodes.Count;

            for (int i = 0; i < transactionNodesSize; i += 2)
            {
                if (i + 1 >= transactionNodesSize) // Last node left, return it to use as MerkleTree root
                {
                    nodeList.Add(transactionNodes[i]);
                    break;
                }

                // Group (concatenate) node values for MerkleTree node
                MerkleNode nextItem = transactionNodes[i + 1];
                string groupHash = transactionNodes[i].value + nextItem.value;

                // Create MerkleNode with parents and add to tree/node list
                MerkleNode newNode = new MerkleNode(
                    Statics.CreateHashSHA256(groupHash),
                    transactionNodes[i],
                    nextItem
                    );
                nodeList.Add(newNode);
            }

            return OrganiseTreeFromMerkleNodeList(nodeList);
        }

    }
}
