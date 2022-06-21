using System;
using BlockNS;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using TransactionNS;
using WalletNS;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlockchainNS
{
    /// <summary>
    /// Class that defines the properties and methods of a generic blockchain.
    /// Supports the genesis block, a configurable difficulty, TTM (time-to-mine), adding blocks, etc.
    /// </summary>
    public class Blockchain
    {

        public Block GenesisBlock { get; set; }
        public LinkedList<Block> Chain { get; set; }
        public int Difficulty { get; set; }

        public int Reward { get; set; }

        public List<Transaction> UnconfirmedTransactions { get; set; } // pool of transactions to be confirmed & mined into a new Block

        public int BlockTime { get; set; }

        public Wallet BlockchainWallet { get; set; }

        /// <summary>
        /// Constructor for a <c>Blockchain</c> object.
        /// </summary>
        /// <param name="genesisBlock">Starting block.</param>
        /// <param name="chain">Initial chain. Usually, it only contains the Genesis block.</param>
        /// <param name="blockchainWallet">The network wallet that issues new coins.</param>
        /// <param name="difficulty">Amount of effort required to solve the computational problem.</param>
        /// <param name="blockTime">Estimated time (in seconds) it takes for a new block to be mined. Used to dynamically change the blockchain difficulty.</param>
        /// <param name="reward">Reward amount offered to miner that solves the computational problem and mines a new block with the unconfirmed transactions.</param>
        public Blockchain(Block genesisBlock, LinkedList<Block> chain, Wallet blockchainWallet, int difficulty, int blockTime, int reward)
        {
            this.GenesisBlock = genesisBlock;
            this.Chain = chain;
            this.Difficulty = difficulty;
            this.BlockTime = blockTime;
            this.Reward = reward;
            this.UnconfirmedTransactions = new List<Transaction> { };
            this.BlockchainWallet = blockchainWallet;
        }

        public void AddBlock(Block block)
        {

            if (block == null)
            {
                throw new ArgumentNullException("Error in Blockchain.AddBlock(): Block argument cannot be null");
            }

            block.Mine(difficulty: this.Difficulty);
            this.Chain.AddAfter(this.Chain.Last, block);

            // Adjust the difficulty if the new block takes more time than the block time
            if ((DateTime.Now - block.Timestamp).Seconds > this.BlockTime)
            {
                this.Difficulty -= 1;
                // Console.WriteLine($"Adjusted difficulty to {this.difficulty}\n");
            }
            else
            {
                this.Difficulty += 1;
                // Console.WriteLine($"Adjusted difficulty to {this.difficulty}\n");
            }
        }

        public void ViewChain()
        {
            foreach (Block block in this.Chain)
            {
                Console.WriteLine(block.ToJsonString());
            }
        }

        public string ToJsonString()
        {   
            // TODO: This can be improved using JsonSerializer.SerializeToUtf8Bytes and then saving bytes to file
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
        /// Saves the JSON representation of the blockchain instance to a file passed as a parameter
        /// </summary>
        /// <param name="jsonBlockchainString">JSON string of Blockchain instance</param>
        public static void SaveJsonStateToFile(string jsonBlockchainString, string filepath)
        {
            System.IO.File.WriteAllText(filepath, jsonBlockchainString);
        }
        
        public static Blockchain JsonStringToBlockchainInstance(string blockchainJsonString)
        {
            try
            {
                Blockchain chain = JsonSerializer.Deserialize<Blockchain>(
                    blockchainJsonString,
                    options: new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    });
                return chain;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads the JSON contents from file at argument location
        /// and deserializes it into a Blockchain object instance
        /// </summary>
        /// <param name="blockchainJsonFilePath">Filepath to JSON file containing Blockchain data (blocks, transactions, etc.)</param>
        public static Blockchain FileJsonStringToBlockchainInstance(string blockchainJsonFilePath)
        {
            try
            {
                string blockchainJsonString = System.IO.File.ReadAllText(blockchainJsonFilePath);
                Blockchain chain = JsonSerializer.Deserialize<Blockchain>(
                    blockchainJsonString,
                    options: new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    });
                return chain;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool IsValid()
        {
            Block previousBlock = this.Chain.First.Value;

            foreach (Block block in this.Chain)
            {
                if (block.Index == 0) // if Genesis block
                {
                    // Still validate for Genesis block
                    // Guard - Genesis block hash matches
                    if (this.GenesisBlock.Hash != block.CalculateHash())
                    {
                        return false;
                    }
                    
                    // Guard - Previous hash is ""
                    if (block.PreviousHash != "")
                    {
                        return false;
                    }
                    
                    previousBlock = this.GenesisBlock;
                    continue;
                }

                // Guard - Blocks are in right order
                if (previousBlock.Index + 1 != block.Index)
                {
                    return false;
                }

                // Guard - Hash of the previous block is equal to current Block.previousHash
                if (previousBlock.Hash != block.PreviousHash)
                {
                    return false;        
                }

                // Guard - Changes made to current Block hash (ie: changes in transactions)
                if (block.Hash != block.CalculateHash())
                {
                    return false;
                }

                previousBlock = block;
            }

            return true;
        }

        public void AddTransaction(Transaction transaction)
        {
            if (!transaction.IsValid(this))
            {
                return;
            }
            
            foreach (Transaction unconfirmedTransaction in this.UnconfirmedTransactions)
            {
                if (unconfirmedTransaction.Hash == transaction.Hash || !unconfirmedTransaction.IsValid(this)) // transaction is already unconfirmed on chain or not valid
                {
                    return;
                }
            }
            this.UnconfirmedTransactions.Add(transaction);
        }

        /**
         * Get current balance (untransferred currency) for a given publicKey (user identifier on the blockchain)
         */
        public int GetBalance(string publicKey)
        {

            if (string.IsNullOrEmpty(publicKey))
            {
                return -1;
            }
            
            int balance = 0;

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
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

        /// <summary>
        /// This kickstarts the mining process - creating & mining a block with valid unconfirmed transactions and adding it to the chain.
        /// Also rewards the miner with the set blockchain reward for mining new blocks.
        /// </summary>
        /// <param name="minerPublicKey">Address to send successful block mine reward to.</param>
        public void MineUnconfirmedTransactions(string minerPublicKey) 
        {

            Transaction rewardTransaction = new Transaction(
                this.BlockchainWallet.GetPublicKeyStringBase64(),
                minerPublicKey,
                this.Reward
                );
            rewardTransaction.SignTransaction(this.BlockchainWallet);
            this.AddTransaction(rewardTransaction);

            List<Transaction> transactionsCopy = new List<Transaction> { };
            transactionsCopy.AddRange(this.UnconfirmedTransactions);

            this.AddBlock(
                new Block(
                    transactionsCopy,
                    Chain.Last.Value.Hash,
                    Chain.Last.Value.Index + 1
                    )
                );

            this.UnconfirmedTransactions.Clear();
        }

        public static Blockchain CreateBlockchain(Transaction firstMint, Wallet blockchainWallet, int difficulty, int blockTime, int reward)
        {
            
            // Argument sanitising
            if (firstMint == null || blockchainWallet == null)
            {
                return null;
            }
            if (difficulty < 0 || blockTime <= 0 || reward < 0)
            {
                return null;
            }

            // Init Genesis block
            List<Transaction> genesisList = new List<Transaction> { };
            firstMint.SignTransaction(blockchainWallet); // sign first transaction
            genesisList.Add(firstMint);
            Block genesisBlock = new Block(genesisList, "", 0);
            genesisBlock.Mine(difficulty);
            
            // Init returned chain & add Genesis block to it
            LinkedList<Block> genesisChain = new LinkedList<Block> { };
            genesisChain.AddFirst(genesisBlock);

            return new Blockchain(genesisBlock, genesisChain, blockchainWallet, difficulty, blockTime, reward);
        }

    }
}
