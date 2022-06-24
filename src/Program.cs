using Peer2Peer.NodesNS.MinerNodeNS;
using Peer2PeerNS.CmdClientNS.CmdUIGateway;
using Peer2PeerNS.CmdClientNS.FullNodeNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;
using Peer2PeerNS.NodesNS.LightweightNodeNS;

namespace ZRD
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             
             THIS SHOULD BE RUN WHEN THE BLOCKCHAIN IS DEPLOYED IN LIVE THE FIRST TIME
             
            // Create network wallet
            Wallet networkWallet = new Wallet(publicKey, privateKey, "ZRD Network Wallet");
            
            // Create first users wallets & set up
            Wallet antonioWallet = new Wallet(publicKey, privateKey, "Antonio's Wallet");
            Wallet annaWallet = new Wallet(publicKey, privateKey, "Anna's Wallet");
            Wallet marcoWallet = new Wallet(publicKey, privateKey, "Marco's Wallet");
            Wallet petruWallet = new Wallet(publicKey, privateKey, "Petru's Wallet");
            
            // Create Blockchain instance with initial coin offerings
            List<Transaction> initialCoinOfferings = new List<Transaction>()
            {
                new Transaction(networkWallet.GetPublicKeyStringBase64(), antonioWallet.GetPublicKeyStringBase64(),
                    antonioWallet.InitialOfferings),
                new Transaction(networkWallet.GetPublicKeyStringBase64(), annaWallet.GetPublicKeyStringBase64(),
                    annaWallet.InitialOfferings),
                new Transaction(networkWallet.GetPublicKeyStringBase64(), marcoWallet.GetPublicKeyStringBase64(),
                    marcoWallet.InitialOfferings),
                new Transaction(networkWallet.GetPublicKeyStringBase64(), petruWallet.GetPublicKeyStringBase64(),
                    petruWallet.InitialOfferings), 
            };
            Blockchain blockchain = Blockchain.CreateBlockchain(
                initialCoinOfferings: initialCoinOfferings,
                blockchainWallet: networkWallet,
                difficulty: 3,
                blockTime: 10,
                reward: 420
            );
            Blockchain.SaveJsonStateToFile(blockchain.ToJsonString(), @"local/Blockchain/ZRD.json");
            */

            // Create initial nodes
            LightweightNode lightweightNode = LightweightNode.ConfigureNode();
            FullNode fullNode = FullNode.ConfigureNode();
            MinerNode minerNode = MinerNode.ConfigureNode();

            // CmdUIGateway - Entry Point to Terminal Blockchain Clients
            CmdUIGateway.Run(lightweightNode, fullNode, minerNode);
            
        }
    }
}
