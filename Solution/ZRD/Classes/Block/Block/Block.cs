/**
 * Class which defines a Block in the blockchain
 * 
 * Fields :
 *  data            = pending transaction list to be minted 
 *  hash            = block hash value (concatenating all fields apart from hash)
 *  previousHash    = previous Block hash value
 *  timestamp       = timestamp when Block was created
 *  proofOfWork     = we use the Proof of Work (PoW) consensus - the amount of effort taken to derive the current Block hash
 *  
 */

using System;
using StaticsNS;
using TransactionNS;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlockNS
{
    public class Block
    {
        public List<Transaction> data;
        public string hash;
        public string previousHash;
        public int proofOfWork;
        public DateTime timestamp;

        public Block(List<Transaction> data, string previousHash)
        {
            this.data = data;
            this.previousHash = previousHash;
            this.hash = "";
            this.proofOfWork = 0;
            this.timestamp = new DateTime();
        }

        /*
         * Calculates the hash value of the current Block instance
         */
        public void SetHash()
        {
            string concatenatedBlockData =
                Statics.TransactionsToJSONString(this.data) +
                this.previousHash +
                this.proofOfWork.ToString() +
                this.timestamp.ToLongTimeString();

            this.hash = Statics.CreateHashSHA256(concatenatedBlockData);
        }

        /**
         * Mines a new block on the blockchain
         * ie. solves the computational problem of generating a hash that matches the difficulty (difficulty = 3, 3 leading zeroes)
         * 
         * Params:
         * difficulty = the difficulty of mining a new block 
         */
        public void Mine(int difficulty)
        {
            // We will use regular expressions to validate that the resulted hash matches the leading zeros rule
            // 'Work' starts on a block with PoW=0 and then calculates hashes with incrementing PoW values (1, 2, 3...)
            Regex hashExpression = new Regex("^(0){${difficulty}}.*", RegexOptions.Compiled);
            MatchCollection hashMatches = hashExpression.Matches(this.hash);

            // While the hash doesn't match (expression doesn't match), keep generating hashes with incremented PoW values
            while (hashMatches.Count == 0)
            {
                this.proofOfWork++;
                this.SetHash();
            }
        }

    }
}
