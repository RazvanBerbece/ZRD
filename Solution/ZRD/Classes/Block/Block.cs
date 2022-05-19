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
using ZRD.Classes.Statics;

namespace ZRD
{
    public class Block
    {
        public string data;
        public string hash;
        public string previousHash;
        public DateTime timestamp;
        public int proofOfWork;

        public Block(string data, string previousHash)
        {
            this.data = data;
            this.hash = "";
            this.previousHash = previousHash;
            this.timestamp = new DateTime();
            this.proofOfWork = 0;
        }

        /*
         * Calculates the hash value of the Block instance
         */
        public void SetHash()
        {
            string concatenatedBlockData =
                this.data +
                this.previousHash +
                this.timestamp.ToLongTimeString() +
                this.proofOfWork.ToString();

            this.hash = Statics.CreateHashSHA256(concatenatedBlockData);
        }

    }
}
