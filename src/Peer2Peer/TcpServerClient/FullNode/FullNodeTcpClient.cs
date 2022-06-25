using System.Net.Sockets;
using System.Text;
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

        public string SendDataStringToPeer(string data, NetworkStream stream)
        {
            // Convert string to byte array
            byte[] bytesToSend = Encoding.ASCII.GetBytes(data);
            // Write byte array to stream for peer to read
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            // Handle response from peer
            byte[] bytesToRead = new byte[this.peer.ReceiveBufferSize];
            int bytesRead = stream.Read(bytesToRead, 0, this.peer.ReceiveBufferSize);
            // Convert response byte array to string for deserialization
            string receivedData = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            return receivedData;
        }

        public void Close()
        {
            this.peer.GetStream().Close();
            this.peer.Close();
        }
    }
}