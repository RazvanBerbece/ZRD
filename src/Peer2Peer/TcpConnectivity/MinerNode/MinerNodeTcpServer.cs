using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using BlockchainNS;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using Peer2PeerNS.TcpConnectivity.PeerCommLogStructNS;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.TcpDirectionEnumNS;
using TransactionNS;

namespace ZRD.Peer2Peer.TcpConnectivity.MinerNode;

public class MinerNodeTcpServer
{
    private int _port;
    private TcpListener _listener;

    private Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS.MinerNode _node;

    private TcpClient _externalPeer;

    public MinerNodeTcpServer OpenListenerOnPort(int portToOpen, Peer2PeerNS.NodesNS.MinerNodeNS.MinerNodeNS.MinerNode node)
    {
        _node = node;
        _port = portToOpen;
        
        try
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            _listener = new TcpListener(ipAddress, _port);
            _listener.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return this;
    }

    public MinerNodeTcpServer AcceptIncomingConnections()
    {
        _externalPeer = _listener.AcceptTcpClient();
        return this;
    }

    public MinerNodeTcpServer HandleIncomingData()
    {
        // Get incoming data through stream and create buffer for reading into 
        NetworkStream stream = _externalPeer.GetStream();
        byte[] buffer = new byte[_externalPeer.ReceiveBufferSize];
            
        // Read incoming stream
        int readBytes = stream.Read(buffer, 0, _externalPeer.ReceiveBufferSize);
        // Convert data from buffer to string
        string receivedData = Encoding.Default.GetString(buffer, 0, readBytes);
        // Log TCP IN details
        PeerCommLogStruct.LogPeerCommunication(_externalPeer, receivedData, DateTime.Now, TcpDirectionEnum.In);
        // Console.WriteLine("Received : " + receivedData);

        string[] opTokens = receivedData.Split(" ");
        if (opTokens[0].Equals("GET"))
        {
            // GET operation requested from client peer
            switch (opTokens[1])
            {
                case "BLOCKCHAIN_FOR_INIT":
                    // Send current blockchain instance for init on client peer side
                    byte[] blockchainStateBuffer = Encoding.ASCII.GetBytes(this._node.Blockchain.ToJsonString());
                    stream.Write(blockchainStateBuffer, 0, blockchainStateBuffer.Length);
                        
                    PeerCommLogStruct.LogPeerCommunication(_externalPeer, this._node.Blockchain.ToJsonString(), DateTime.Now, TcpDirectionEnum.Out);
                    break;
                case "PEER_LIST":
                    // Send current peer list state for sync / merge / append on client peer side
                    string peerListString = System.IO.File.ReadAllText("local/Peers/Peers.json");
                    byte[] peerListBuffer = Encoding.ASCII.GetBytes(peerListString);
                    stream.Write(peerListBuffer, 0, peerListBuffer.Length);
                        
                    PeerCommLogStruct.LogPeerCommunication(_externalPeer, peerListString, DateTime.Now, TcpDirectionEnum.Out);
                    break;
            }
        }
        else
        {
            // Handle data by iteratively deserializing through the possible scenarios :
            //      1. Full node connecting to send Blockchain as part of broadcasting
            //      2. Full node connecting to send peer list updates
            if (Blockchain.JsonStringToBlockchainInstance(receivedData) is { } remoteBlockchain)
            {
                Console.WriteLine("-- DESERIALIZING BLOCKCHAIN --");
                var shouldUpdatePeer = ResolveBlockchainMerge(this._node.Blockchain, remoteBlockchain);
                if (shouldUpdatePeer)
                {
                    // Write new blockchain to stream
                    var blockchainStateBuffer = Encoding.ASCII.GetBytes(this._node.Blockchain.ToJsonString());
                    stream.Write(blockchainStateBuffer, 0, blockchainStateBuffer.Length);

                    PeerCommLogStruct.LogPeerCommunication(_externalPeer, this._node.Blockchain.ToJsonString(), DateTime.Now, TcpDirectionEnum.Out);
                }
            }
            else
            {
                bool isIncomingPeerList;
                try
                {
                    Console.WriteLine("-- DESERIALIZING INCOMING PEER LIST --");
                    var discoveryManager = new DiscoveryManager();
                    // Check for incoming peer list update
                    var upstreamPeerDetailsList = JsonSerializer.Deserialize<List<PeerDetails>>(
                        receivedData,
                        options: new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true,
                            Encoder = JavaScriptEncoder
                                .UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                        });
                    isIncomingPeerList = true;
                    // Merge / append / write new peer list to local Peers.json
                    var mergedList = DiscoveryManager.MergePeerLists(upstreamPeerDetailsList,
                        discoveryManager.LoadPeerDetails("local/Peers/Peers.json"));
                    discoveryManager.WritePeerListToFile(mergedList, "local/Peers/Peers.json");
                    // Send local peer list to connected peer to merge as well
                    var localPeerDetails = System.IO.File.ReadAllText("local/Peers/Peers.json");
                    var localPeerListBuffer = Encoding.ASCII.GetBytes(localPeerDetails);
                    stream.Write(localPeerListBuffer, 0, localPeerListBuffer.Length);

                    PeerCommLogStruct.LogPeerCommunication(_externalPeer, localPeerDetails, DateTime.Now, TcpDirectionEnum.Out);
                }
                catch (Exception)
                {
                    isIncomingPeerList = false;
                }

                if (!isIncomingPeerList)
                {
                    // Data received could not be parsed into object instance
                    // i.e. received data format does not match expected data formats
                    // Write back to peer and close connection
                    var errMessage = "Data received by server does not match expected formats";
                    var errBuffer = Encoding.ASCII.GetBytes(errMessage);
                    stream.Write(errBuffer, 0, errBuffer.Length);

                    PeerCommLogStruct.LogPeerCommunication(_externalPeer, errMessage, DateTime.Now, TcpDirectionEnum.Out);
                }
            }
        }
        
        return this;
    }

    public void CloseConnection()
    {
        _externalPeer.Close();
    }
    
    /// <summary>
    /// Resolves the blockchain sync discussion between two peers by :
    ///     - Accepting the longer one, if valid
    ///     - Merging the local mempool with the upstream one's valid & unique transactions
    /// Mutates the node Blockchain if remote Blockchain is preferred
    /// </summary>
    /// <param name="localBlockchain"></param>
    /// <param name="remoteBlockchain"></param>
    /// <returns>true if peer needs to be updated with new Blockchain, false otherwise</returns>
    private bool ResolveBlockchainMerge(Blockchain localBlockchain, Blockchain remoteBlockchain)
    {
        if (remoteBlockchain.Chain.Count < localBlockchain.Chain.Count) return false;
        // Check that upstream Blockchain is valid
        if (!remoteBlockchain.IsValid()) return false;
        // Resolve unvalidated transactions & Use remote Blockchain for local
        var mergedTransactions = new List<Transaction>() { };
        // Add local mempool to final list of unvalidated transactions
        var localMempoolIsEmpty = localBlockchain.UnconfirmedTransactions.Count == 0;
        if (!localMempoolIsEmpty)
        {
            mergedTransactions.AddRange(localBlockchain.UnconfirmedTransactions);   
        }

        // Add valid transactions from remote mempool into local one
        foreach (var transaction in remoteBlockchain.UnconfirmedTransactions)
        {
            // If upstream mempool transaction is valid and NOT a duplicate in local
            if (transaction.IsValid(localBlockchain) && !mergedTransactions.Contains(transaction))
            {
                mergedTransactions.Add(transaction);
            }
        }
            
        // Sync local Blockchain & mempool
        this._node.SetBlockchain(remoteBlockchain);
        this._node.Blockchain.UnconfirmedTransactions = mergedTransactions;

        if (localMempoolIsEmpty)
        {
            return false;
        }
                
        return true;

    }
}