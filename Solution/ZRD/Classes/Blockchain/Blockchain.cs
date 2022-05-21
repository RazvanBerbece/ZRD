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

        public void AddBlock(Block block)
        {
            block.Mine(difficulty: this.difficulty);
            this.chain.AddAfter(this.chain.Last, block);
        }

        public void ViewChain()
        {
            LinkedList<Block>.Enumerator enumerator = this.chain.GetEnumerator();
            enumerator.MoveNext();
            while (true)
            {
                Console.WriteLine(enumerator.Current.ToJSONString());
                if (!enumerator.MoveNext())
                {
                    break;
                };
            }
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


            return new Blockchain(genesisChain, difficulty);
        }

    }
}
