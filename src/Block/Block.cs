/**
 * Class which defines a Block in the blockchain
 * 
 * Fields :
 *  data            = pending transaction list to be minted 
 *  hash            = block hash value (concatenating all fields apart from hash)
 *  previousHash    = previous Block hash value
 *  timestamp       = timestamp when Block was created
 *  proofOfWork     = we use the Proof of Work (PoW) consensus - the amount of effort (iterations) taken to derive a valid hash for the current Block
 *  
 */

using BlockchainNS;
using System;
using StaticsNS;
using TransactionNS;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MerkleTreeNS;
using MerkleTreeNS.MerkleNodeNS;

namespace BlockNS
{
    public class Block
    {

        public int index;
        public List<Transaction> data;
        public string hash;
        public string previousHash;
        public int proofOfWork;
        public DateTime timestamp;

        public MerkleTree tree;
        public MerkleNode root;

        public Block(List<Transaction> data, string previousHash, int index)
        {
            this.index = index;
            this.data = data;
            this.previousHash = previousHash;
            this.hash = "";
            this.proofOfWork = 0;

            this.timestamp = new DateTime();
            this.timestamp = DateTime.Now;

            // Process the Merkle tree & root for the given transactions under current Block
            this.tree = MerkleTree.CreateMerkleTree(this.data);
            this.root = this.tree.root;
        }

        /// <summary>
        /// Calculates the hash value of the current Block instance using the index, data, previous hash, PoW, timestamp.
        /// TODO: Add Merkle hash support in hash calculation ?
        /// </summary>
        /// <returns>Hash value of Block instance</returns>
        public string CalculateHash()
        {
            string concatenatedBlockData =
                this.index.ToString() +
                Statics.TransactionsToJSONString(this.data) +
                this.previousHash +
                this.proofOfWork.ToString() +
                this.timestamp.ToLongTimeString();

            return Statics.CreateHashSHA256(concatenatedBlockData);
        }
        
        /// <summary>
        /// Mines a new block on the blockchain.
        /// ie. solves the computational problem of generating a hash that matches the difficulty (difficulty = 3, 3 leading zeroes)
        /// </summary>
        /// <param name="difficulty">Mining difficulty (number of zeroes needed for a valid hash)</param>>
        public void Mine(int difficulty)
        {
            // We will use regular expressions to validate that the resulted hash matches the leading zeros rule
            // 'Work' starts on a block with PoW=0 and then calculates hashes with incrementing PoW values (1, 2, 3...)
            string regexHashPattern = $"^(0){{{difficulty}}}.*";
            Regex hashExpression = new Regex(regexHashPattern, RegexOptions.Compiled);
            MatchCollection hashMatches = hashExpression.Matches(this.hash);

            // While the hash doesn't match (expression doesn't match), keep generating hashes with incremented PoW values
            while (hashMatches.Count == 0)
            {
                this.proofOfWork++;
                this.hash = this.CalculateHash();
                hashMatches = hashExpression.Matches(this.hash);
            }

            return;
        }

        /// <summary>
        /// Converts a Block instance to a JSON formatted string representation
        /// </summary>
        /// <returns>JSON-Formatted String representation of current Block instance</returns>
        public string ToJSONString()
        {
            string output =
                $"{{\n\tIndex: {this.index},\n\tTimestamp: {this.timestamp.ToLongTimeString()},\n\tCurrent Hash: \"{this.hash}\",\n\tPrevious Hash: \"{this.previousHash}\",\n\tPoW: {this.proofOfWork}\n}},";

            return output;
        }

        /// <summary>
        /// Returns true if all transactions in a block are valid
        /// </summary>
        /// <param name="chain">Blockchain to validate transactions against</param>
        /// <returns>Boolean on the overall transaction valid status</returns>
        public bool HasAllValidTransactions(Blockchain chain)
        {
            foreach (Transaction transaction in this.data) 
            {
                if (!transaction.IsValid(chain))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
