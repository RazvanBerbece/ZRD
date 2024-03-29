﻿using System;
using BlockNS;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using TransactionNS;
using System.Text.Json;
using Newtonsoft.Json;
using WalletNS.BlockchainWalletNS;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlockchainNS
{
    /// <summary>
    /// Class that defines the properties and methods of a generic blockchain.
    /// Supports the genesis block, a configurable difficulty, TTM (time-to-mine), adding blocks, etc.
    /// </summary>
    [JsonObject(ItemRequired = Required.Always)]
    public class Blockchain
    {
        
        // Data Models
        public Block GenesisBlock { get; set; }
        public LinkedList<Block> Chain { get; set; }
        public BlockchainWallet BlockchainWallet { get; set; } // used to issue initial offering and mining rewards
        
        // Configurable Settings
        public int Difficulty { get; set; }
        public int Reward { get; set; }
        public int BlockTime { get; set; }
        private string filepathToState; // ZRD.json blockchain full state will be saved here
        
        // Pool of transactions to be confirmed & mined into a new Block
        public List<Transaction> UnconfirmedTransactions { get; set; }

        public Blockchain() { }
        
        /// <summary>
        /// Constructor for a <c>Blockchain</c> object.
        /// </summary>
        /// <param name="genesisBlock">Starting block.</param>
        /// <param name="chain">Initial chain. Usually, it only contains the Genesis block.</param>
        /// <param name="blockchainWallet">The network wallet that issues new coins and rewards miners</param>
        /// <param name="difficulty">Amount of effort required to solve the computational problem.</param>
        /// <param name="blockTime">Estimated time (in seconds) it takes for a new block to be mined. Used to dynamically change the blockchain difficulty.</param>
        /// <param name="reward">Reward amount offered to miner that solves the computational problem and mines a new block with the unconfirmed transactions.</param>
        [JsonConstructor]
        public Blockchain(Block genesisBlock, LinkedList<Block> chain, BlockchainWallet blockchainWallet, int difficulty, int blockTime, int reward)
        {
            this.GenesisBlock = genesisBlock;
            this.Chain = chain;
            this.Difficulty = difficulty;
            this.BlockTime = blockTime;
            this.Reward = reward;
            this.UnconfirmedTransactions = new List<Transaction> { };
            this.BlockchainWallet = blockchainWallet;
        }
        
        // Constructor used for runtime instantiation 
        public Blockchain(Block genesisBlock, LinkedList<Block> chain, BlockchainWallet blockchainWallet, int difficulty, int blockTime, int reward, string filepathToState)
        {
            this.GenesisBlock = genesisBlock;
            this.Chain = chain;
            this.Difficulty = difficulty;
            this.BlockTime = blockTime;
            this.Reward = reward;
            this.UnconfirmedTransactions = new List<Transaction> { };
            this.BlockchainWallet = blockchainWallet;
            this.filepathToState = filepathToState;
        }
        
        /// <summary>
        /// Mines the passed Block parameter and adds it at the end of the chain
        /// Adjusts difficulty based on the current instance setting of BlockTime and the passed
        /// block mining time
        /// Also saves the new Blockchain instance in filepathToState
        /// </summary>
        /// <param name="block">Block to be mined & added to chain</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddBlock(Block block)
        {

            if (block == null)
            {
                throw new ArgumentNullException("Error in Blockchain.AddBlock(): Block argument cannot be null");
            }

            block.Mine(difficulty: this.Difficulty);
            this.Chain.AddAfter(this.Chain.Last, block);

            // Adjust the difficulty if the new block mining process took more time than the setup block time
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
            
            // Save new state with new unconfirmed transaction
            SaveJsonStateToFile(this.ToJsonString(), filepathToState);
        }

        public void ViewChain()
        {
            foreach (Block block in this.Chain)
            {
                Console.WriteLine(block.ToJsonString());
            }
        }
        
        /// <summary>
        /// Serializes the current Blockchain instance to a JSON string
        /// </summary>
        /// <returns>JSON string which represents the Blockchain instance</returns>
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
        /// <param name="filepath">filepath where to save the state of the current blockchain instance</param>
        public static void SaveJsonStateToFile(string jsonBlockchainString, string filepath)
        {
            System.IO.File.WriteAllText(filepath, jsonBlockchainString);
        }
        
        public static Blockchain JsonStringToBlockchainInstance(string blockchainJsonString)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.MissingMemberHandling = MissingMemberHandling.Error;
                Blockchain chain = JsonConvert.DeserializeObject<Blockchain>(
                    blockchainJsonString,
                    jsonSettings);
                return chain;
            }
            catch (Exception)
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
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Checks that the Blockchain instance is valid by iterating through all the blocks and :
        ///     - checking that the indexes are in the correct order
        ///     - checking that the previous block hash and the current block's prevHash match
        ///     - the hash of the current block matches the recalculated hash of the block
        /// </summary>
        /// <returns>true if current instance is valid, false otherwise</returns>
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
        
        /// <summary>
        /// If the passed transaction parameter is valid, not a duplicate in UnconfirmedTransactions
        /// and all the unconfirmed transactions on the current chain are valid,
        /// it's added to UnconfirmedTransactions. Also saves the new state locally.
        /// </summary>
        /// <param name="transaction">
        /// Transaction to be added to UnconfirmedTransactions under current instance
        /// </param>
        /// <returns>true if transaction was added to UnconfirmedTransactions, false otherwise</returns>
        public bool AddTransaction(Transaction transaction)
        {
            if (!transaction.IsValid(this))
            {
                return false;
            }
            
            foreach (Transaction unconfirmedTransaction in this.UnconfirmedTransactions)
            {
                if (unconfirmedTransaction.Hash == transaction.Hash || !unconfirmedTransaction.IsValid(this)) // transaction is already unconfirmed on chain or not valid
                {
                    return false;
                }
            }
            
            this.UnconfirmedTransactions.Add(transaction);
            
            // Save new state with new unconfirmed transaction
            SaveJsonStateToFile(this.ToJsonString(), filepathToState);

            return true;
        }
        
        /// <summary>
        /// Get current balance (untransferred currency) for a given publicKey (user identifier on the blockchain)
        /// </summary>
        /// <param name="publicKey">Wallet address to check transactions against</param>
        /// <returns>Coin available on given address</returns>
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
            rewardTransaction.SignTransaction(this.BlockchainWallet.GetCommonWallet());
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
            
            // Save new state with new unconfirmed transaction
            SaveJsonStateToFile(this.ToJsonString(), filepathToState);
        }

        public void SetFilepathToState(string filepath)
        {
            this.filepathToState = filepath;
        }

        public static Blockchain CreateBlockchain(List<Transaction> initialCoinOfferings, BlockchainWallet blockchainWallet, int difficulty, int blockTime, int reward, string filepathToState)
        {
            
            // Argument sanitising
            if (initialCoinOfferings == null || blockchainWallet == null)
            {
                return null;
            }
            if (difficulty < 0 || blockTime <= 0 || reward < 0)
            {
                return null;
            }

            // Init Genesis block
            List<Transaction> genesisList = new List<Transaction> { };
            
            // Sign initial coin offerings
            foreach (Transaction transaction in initialCoinOfferings)
            {
                transaction.SignTransaction(blockchainWallet.GetCommonWallet());
                genesisList.Add(transaction);
            }
            Block genesisBlock = new Block(genesisList, "", 0);
            genesisBlock.Mine(difficulty);
            
            // Init returned chain & add Genesis block to it
            LinkedList<Block> genesisChain = new LinkedList<Block> { };
            genesisChain.AddFirst(genesisBlock);

            return new Blockchain(genesisBlock, genesisChain, blockchainWallet, difficulty, blockTime, reward, filepathToState);
        }

    }
}
