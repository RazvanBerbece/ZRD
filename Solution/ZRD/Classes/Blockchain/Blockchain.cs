/**
 * Class that defines the properties and methods of a generic blockchain
 * Supports the genesis block, a configurable difficulty, TTM (time-to-mine), adding blocks, etc.
 */

using System;
using BlockNS;
using System.Collections.Generic;
using TransactionNS;

namespace BlockchainNS
{
    public class Blockchain
    {

        public Block genesisBlock;
        public LinkedList<Block> chain;
        public int difficulty;

        public Blockchain(LinkedList<Block> chain, int difficulty)
        {
            this.chain = chain;
            this.difficulty = difficulty;
        }

        public Blockchain(Block genesisBlock, LinkedList<Block> chain, int difficulty)
        {
            this.genesisBlock = genesisBlock;
            this.chain = chain;
            this.difficulty = difficulty;
        }

        public void AddBlock(Block block)
        {
            block.Mine(difficulty: this.difficulty);
            this.chain.AddAfter(this.chain.Last, block);
        }

        public void ViewChain()
        {
            foreach (Block block in this.chain)
            {
                Console.WriteLine(block.ToJSONString());
            }
        }

        public bool IsValid()
        {
            Block previousBlock = new Block(null, null, -1);
            foreach (Block block in this.chain)
            {
                if (block.index == 0) // if Genesis block
                {
                    previousBlock = this.genesisBlock;
                    continue;
                }

                // Guard - Blocks are in right order
                if (previousBlock.index + 1 != block.index)
                {
                    return false;
                }

                // Guard - Hash of the previous block is equal to current Block.previousHash
                if (previousBlock.hash != block.previousHash)
                {
                    return false;        
                }

                // Guard - Changes made to current Block hash (ie: changes in transactions)
                if (block.hash != block.CalculateHash())
                {
                    return false;
                }
            }

            return true;
        }

        public static Blockchain CreateBlockchain(int difficulty)
        {
            // Init Genesis block
            List<Transaction> emptyList = new List<Transaction> { };
            Block genesisBlock = new Block(emptyList, "", 0);
            genesisBlock.Mine(difficulty);

            // Init returned chain & add Genesis block to it
            LinkedList<Block> genesisChain = new LinkedList<Block> { };
            genesisChain.AddFirst(genesisBlock);

            return new Blockchain(genesisBlock, genesisChain, difficulty);
        }

    }
}
