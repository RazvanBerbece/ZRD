using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Peer2PeerNS.FullNodeTcpClientNS;
using Peer2PeerNS.FullNodeTcpServerNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.ClientNS.FullNodeTcpClientTestsNS
{
    [TestFixture]
    public class FullNodeTcpClientTests
    {
        
        private FullNode node;
        private const string PeerHost = "82.26.1.87";
        private FullNodeTcpClient peer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing FullNodeTcpClient --\n");
            this.node = FullNode.ConfigureNode();
            this.peer = new FullNodeTcpClient();
        } 
        
        [TearDown]
        public void TearDown()
        {
            this.peer.Close();
        }

        [Test]
        public void FullNodeTcpClient_CanInit()
        {
            // Start server
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(node);
            server.RunServer(420);
                    
            // Init connection 
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, server.port);

            // Assert
            Assert.That(peer, Is.InstanceOf(typeof(TcpClient)));
        }
        
        [Test]
        public void FullNodeTcpClient_CanConnect()
        {
            // Start server
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(node);
            server.RunServer(420);
            
            // Init connection & Connect
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, server.port);
            NetworkStream stream = peer.Connect();
            
            // Assert stream
            Assert.That(stream, Is.InstanceOf(typeof(NetworkStream)));
            Assert.That(stream.CanRead, Is.True);
            Assert.That(stream.CanWrite, Is.True);
        }
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase("GET BLOCKCHAIN_FOR_INIT")]
        public void FullNodeTcpClient_CanSendDataStringToPeer(string data)
        {
            // Start server
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(node);
            server.RunServer(420);
            
            // Init connection, Connect & send data
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, server.port);
            NetworkStream stream = peer.Connect();
            string received = peer.SendDataStringToPeer(data, stream);
            
            // Assert received data
            Assert.That(string.IsNullOrEmpty(received), Is.False);
        }
        
        [Test]
        public void FullNodeTcpClient_CanCloseConnection()
        {
            // Start server
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(node);
            server.RunServer(420);
            
            // Init connection, Connect and Close
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, server.port);
            peer.Connect();
            peer.Close();
            
            // Assert peer socket status
            Assert.That(peer.peer.Connected, Is.False);
        }
        
    }
}