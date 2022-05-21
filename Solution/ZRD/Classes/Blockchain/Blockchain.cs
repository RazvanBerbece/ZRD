/**
 * Class that defines the properties and methods of a generic blockchain
 */

using System;
using BlockNS;
using System.Collections.Generic;

namespace ZRD.Classes.Blockchain
{
    public class Blockchain
    {

        public Block genesisBlock;
        public LinkedList<Block> chain;
        public int difficulty;

        public Blockchain(Block genesisBlock, LinkedList<Block> chain, int difficulty)
        {
            this.genesisBlock = genesisBlock;
            this.chain = chain;
            this.difficulty = difficulty;
        }

        public static Blockchain CreateBlockchain(int difficulty)
        {
            Block genesisBlock = new Block(null, null);
            LinkedList<Block> genesisChain = new LinkedList<Block> { };
            genesisChain.AddFirst(genesisBlock);
            return new Blockchain(genesisBlock, genesisChain, difficulty);
        }
    }
}
