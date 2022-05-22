using System;
using MerkleTreeNS.MerkleNodeNS;
using TransactionNS;
using System.Collections.Generic;
using StaticsNS;

namespace MerkleTreeNS
{
    public class MerkleTree
    {

        public int size;
        public MerkleTree root;

        public MerkleTree(MerkleTree root, int size)
        {
            this.size = size;
            this.root = root;
        }

        static public MerkleTree CreateMerkleTree(List<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function takes in a list of MerkleNodes
        /// and organises it in a tree (list data structure for internal representation)
        /// by pushing MerkleNodes with parents and calculated value to a new list and returning it
        /// </summary>
        /// <param name="transactionNodes">List of tree nodes organised so far. Starts initialised with MerkleNodes with no parents.</param>
        /// <returns>MerkleNode which can be used as root of the MerkleTree</returns>
        static public MerkleNode MakeMerkleTreeFromTransactionList(List<MerkleNode> transactionNodes)
        {

            if (transactionNodes.Count == 0) return null;
            if (transactionNodes.Count == 1) return transactionNodes[1];

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

            return MerkleTree.MakeMerkleTreeFromTransactionList(nodeList);
        }
    }
}
