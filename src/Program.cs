using System.Collections.Generic;
using BlockchainNS;
using TransactionNS;
using Peer2PeerNS.CmdClientNS.FullNodeNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;
using WalletNS;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Create network wallet
            Wallet networkWallet = new Wallet(keySize: 1024);
            networkWallet.SetWalletName("ZRD Network Wallet");
            
            // Create first users wallets & set up
            Wallet antonioWallet = new Wallet(keySize: 1024);
            Wallet annaWallet = new Wallet(keySize: 1024);
            antonioWallet.SetWalletName("Antonio's Wallet");
            annaWallet.SetWalletName("Anna's Wallet");
            
            // Create Blockchain instance with initial coin offerings
            List<Transaction> initialCoinOfferings = new List<Transaction>()
            {
                new Transaction(networkWallet.GetPublicKeyStringBase64(), antonioWallet.GetPublicKeyStringBase64(),
                    1000000),
                new Transaction(networkWallet.GetPublicKeyStringBase64(), annaWallet.GetPublicKeyStringBase64(),
                    1000000)
            };
            Blockchain blockchain = Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: networkWallet,
                difficulty: 2,
                blockTime: 5,
                reward: 420
            );
            Blockchain.SaveJsonStateToFile(blockchain.ToJsonString(), @"local/Blockchain/ZRD.json");

            // Create example nodes
            FullNode fullNode = FullNode.ConfigureNode();

            // Onboard - Entry Point to Terminal Blockchain Clients
            FullNodeOnboard.Run(fullNode, port: 420);
            
        }
    }
}
