using System;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.DataOutTypeNS;
using ZRD.Peer2Peer.TcpServerClient.Abstract;

namespace Peer2PeerNS.FullNodeTcpClientNS
{
    public class FullNodeTcpClient: ITcpClient
    {
        
        public TcpClient peer;
        
        public void Init(string dest, int port)
        {
            this.peer = new TcpClient(dest, port);
        }

        public NetworkStream Connect()
        {
            NetworkStream stream = this.peer.GetStream();
            return stream;
        }

        public dynamic SendDataStringToPeer(string data, NetworkStream stream, DataOutType type)
        {
            // Convert string to byte array
            byte[] bytesToSend = Encoding.ASCII.GetBytes(data);
            // Write byte array to stream for peer to read
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            // Handle response from peer
            byte[] bytesToRead = new byte[this.peer.ReceiveBufferSize];
            int bytesRead = stream.Read(bytesToRead, 0, this.peer.ReceiveBufferSize);
            // Convert response byte array to string or int for further deserialization
            dynamic receivedData;
            try
            {
                switch (type)
                {
                    case DataOutType.Transaction:
                        receivedData = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                        return receivedData;
                    case DataOutType.WalletBalanceRequest:
                        receivedData = BitConverter.ToInt32(bytesToRead);
                        return receivedData;
                    case DataOutType.PeerListPush:
                        receivedData = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                        return receivedData;
                    case DataOutType.BlockchainInitRequest:
                        receivedData = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                        return receivedData;
                    case DataOutType.BlockchainPush:
                        receivedData = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                        return receivedData;
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception caught in SendDataStringToPeer: {e}");
                return null;
            }
        }

        public void Close()
        {
            this.peer.GetStream().Close();
            this.peer.Close();
        }
    }
}