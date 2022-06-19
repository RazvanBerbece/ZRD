using BlockchainNS;
using System;
using StaticsNS;
using TransactionNS;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using MerkleTreeNS;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlockNS
{
    /// <summary>
    /// Class which defines a Block in the blockchain
    /// 
    /// Fields :
    /// data            = pending transaction list to be minted 
    /// hash            = block hash value (concatenating all fields apart from hash)
    /// previousHash    = previous Block hash value
    /// timestamp       = timestamp when Block was created
    /// proofOfWork     = we use the Proof of Work (PoW) consensus - the amount of effort (iterations) taken to derive a valid hash for the current Block
    /// </summary>
    public class Block
    {

        public int Index { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public int ProofOfWork { get; set; }
        public DateTime Timestamp { get; set; }

        public MerkleTree Tree { get; set; }

        public Block(List<Transaction> transactions, string previousHash, int index)
        {
            this.Index = index;
            this.Transactions = transactions;
            this.PreviousHash = previousHash;
            this.Hash = "";
            this.ProofOfWork = 0;

            this.Timestamp = new DateTime();
            this.Timestamp = DateTime.Now;

            // Process the Merkle tree & root for the given transactions under current Block
            this.Tree = MerkleTree.CreateMerkleTree(this.Transactions);
            if (this.Tree == null)
            {
                throw new ArgumentException("Transaction data list cannot be empty or null");
            }
            
        }

        /// <summary>
        /// Calculates the hash value of the current Block instance using the index, data, previous hash, PoW, timestamp.
        /// TODO: Add Merkle hash support in hash calculation ?
        /// </summary>
        /// <returns>Hash value of Block instance</returns>
        public string CalculateHash()
        {
            string concatenatedBlockData =
                this.Index.ToString() +
                Statics.TransactionsToJsonString(this.Transactions) +
                this.PreviousHash +
                this.ProofOfWork.ToString() +
                this.Timestamp.ToLongTimeString();

            return Statics.CreateHashSha256(concatenatedBlockData);
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
            MatchCollection hashMatches = hashExpression.Matches(this.Hash);

            // While the hash doesn't match (expression doesn't match), keep generating hashes with incremented PoW values
            while (hashMatches.Count == 0)
            {
                this.ProofOfWork++;
                this.Hash = this.CalculateHash();
                hashMatches = hashExpression.Matches(this.Hash);
            }

            return;
        }

        /// <summary>
        /// Converts a Block instance to a JSON formatted string representation
        /// </summary>
        /// <returns>JSON-Formatted String representation of current Block instance</returns>
        public string ToJsonString()
        {
            string jsonStringBlock = JsonSerializer.Serialize(
                this,
                options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                }
                );
            return jsonStringBlock;
        }

        /// <summary>
        /// Returns true if all transactions in a block are valid
        /// </summary>
        /// <param name="chain">Blockchain to validate transactions against</param>
        /// <returns>Boolean on the overall transaction valid status</returns>
        public bool HasAllValidTransactions(Blockchain chain)
        {
            foreach (Transaction transaction in this.Transactions) 
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
