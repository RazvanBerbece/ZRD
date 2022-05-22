using System;
using BlockNS;
using System.Collections.Generic;
using TransactionNS;

namespace BlockchainNS
{
    /// <summary>
    /// Class that defines the properties and methods of a generic blockchain.
    /// Supports the genesis block, a configurable difficulty, TTM (time-to-mine), adding blocks, etc.
    /// </summary>
    public class Blockchain
    {

        public Block genesisBlock;
        public LinkedList<Block> chain;
        public int difficulty;

        public int reward;

        public List<Transaction> unconfirmedTransactions; // pool of transactions to be confirmed

        public int blockTime;

        /// <summary>
        /// Constructor for a <c>Blockchain</c> object.
        /// </summary>
        /// <param name="genesisBlock">Starting block.</param>
        /// <param name="chain">Initial chain. Usually, it only contains the Genesis block.</param>
        /// <param name="difficulty">Amount of effort required to solve the computational problem.</param>
        /// <param name="blockTime">Estimated time (in seconds) it takes for a new block to be added to the chain after mining.</param>
        /// <param name="reward">Reward amount offered to miner that solves the computational problem.</param>
        public Blockchain(Block genesisBlock, LinkedList<Block> chain, int difficulty, int blockTime, int reward)
        {
            this.genesisBlock = genesisBlock;
            this.chain = chain;
            this.difficulty = difficulty;
            this.blockTime = blockTime;
            this.reward = reward;
        }

        public void AddBlock(Block block)
        {
            block.Mine(difficulty: this.difficulty);
            this.chain.AddAfter(this.chain.Last, block);

            // Adjust the difficulty if the new block takes more time than the block time
            if ((DateTime.Now - block.timestamp).Seconds > this.blockTime)
            {
                this.difficulty -= 1;
                Console.WriteLine($"Adjusted difficulty to {this.difficulty}\n");
            }
            else
            {
                this.difficulty += 1;
                Console.WriteLine($"Adjusted difficulty to {this.difficulty}\n");
            }
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

                previousBlock = block;
            }

            return true;
        }

        public void AddTransaction(Transaction transaction)
        {
            foreach (Transaction unconfirmedTransaction in this.unconfirmedTransactions)
            {
                if (unconfirmedTransaction.hash == transaction.hash) // duplicate unconfirmed transaction on chain
                {
                    return;
                }
            }
            this.unconfirmedTransactions.Add(transaction);
        }

        /**
         * Get current balance (untransferred currency) for a given publicKey (user identifier on the blockchain)
         */
        public int GetBalance(string publicKey)
        {
            int balance = 0;

            foreach (Block block in this.chain)
            {
                foreach (Transaction transaction in block.data)
                {
                    if (transaction.Sender == publicKey)
                    {
                        balance -= transaction.Amount;
                    }
                    else if (transaction.Receiver == publicKey)
                    {
                        balance += transaction.Amount;
                    }
                    
                }
            }

            return balance;
        }

        public static Blockchain CreateBlockchain(Transaction firstMint, int difficulty, int blockTime, int reward)
        {
            // Init Genesis block
            List<Transaction> genesisList = new List<Transaction> { };
            genesisList.Add(firstMint);
            Block genesisBlock = new Block(genesisList, "", 0);

            // Init returned chain & add Genesis block to it
            LinkedList<Block> genesisChain = new LinkedList<Block> { };
            genesisChain.AddFirst(genesisBlock);

            return new Blockchain(genesisBlock, genesisChain, difficulty, blockTime, reward);
        }

    }
}
