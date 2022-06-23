using System;
using System.Net.Sockets;
using System.Threading;
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
        private FullNodeTcpServer server;
        private const string PeerHost = "82.26.1.87";
        private FullNodeTcpClient peer;

        [SetUp]
        public void SetUp()
        {
            this.server = new FullNodeTcpServer();
            this.server.SetFullNode(node);
            this.server.RunServer(420);
        } 

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
            this.server.Kill();
        }

        [Test]
        public void FullNodeTcpClient_CanInit()
        {
            FullNodeTcpClient peer = new FullNodeTcpClient();
            Console.WriteLine("GOT HERE 1\n");
            peer.Init(PeerHost, this.server.port);
            Console.WriteLine("GOT HERE 2\n");
            Assert.That(peer, Is.InstanceOf(typeof(TcpClient)));
        }
        
        [Test]
        public void FullNodeTcpClient_CanConnect()
        {
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, this.server.port);
            NetworkStream stream = peer.Connect();
            Assert.That(stream, Is.InstanceOf(typeof(NetworkStream)));
            Assert.That(stream.CanRead, Is.True);
            Assert.That(stream.CanWrite, Is.True);
        }
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase("GET BLOCKCHAIN_FOR_INIT")]
        public void FullNodeTcpClient_CanSendDataStringToPeer(string data)
        {
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, this.server.port);
            NetworkStream stream = peer.Connect();
            string received = peer.SendDataStringToPeer(data, stream);
            Assert.That(string.IsNullOrEmpty(received), Is.False);
        }
        
        [Test]
        public void FullNodeTcpClient_CanCloseConnection()
        {
            FullNodeTcpClient peer = new FullNodeTcpClient();
            peer.Init(PeerHost, this.server.port);
            peer.Connect();
            peer.Close();
            Assert.That(peer.peer.Connected, Is.False);
        }
        
    }
}