namespace Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.DataOutTypeNS
{
    public enum DataOutType: ushort
    {
        Transaction = 100,
        WalletBalanceRequest = 200,
        BlockchainInitRequest = 300,
        PeerListRequest = 400,
        PeerListPush = 500,
        BlockchainPush = 600
    }
}