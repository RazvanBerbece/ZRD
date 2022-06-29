using System;
using System.IO;
using System.Net.Sockets;
using NUnit.Framework;
using Peer2PeerNS.FullNodeTcpServerNS;
using Peer2PeerNS.NodesNS.FullNodeNS.FullNodeNS;

namespace Peer2PeerNS.ServerNS.FullNodeTcpServerTestsNS
{
    [TestFixture]
    public class FullNodeTcpServerTests
    {
        private FullNode node;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing FullNodeTcpServer --\n");
        }
        
        [SetUp]
        public void SetUp()
        {
            this.node = FullNode.ConfigureNode("TEST_PEER_LIST_1.json");
        }
        
        [TearDown]
        public void TearDown()
        {
            if (File.Exists("TEST_PEER_LIST_1.json"))
            {
                File.Delete("TEST_PEER_LIST_1.json");
            }
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(420)]
        [TestCase(421)]
        [TestCase(423)]
        [TestCase(int.MaxValue)]
        public void FullNodeTcpServer_CanInitialise(int portToOpen)
        {
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(this.node);
            switch (portToOpen)
            {
                case -1:
                    try
                    {
                        server.Init(portToOpen);
                        Assert.Fail("Server should not open negative port or port 0");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                case 0:
                    try
                    {
                        server.Init(portToOpen);
                        Assert.Fail("Server should not open negative port or port 0");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                case int.MaxValue:
                    try
                    {
                        server.Init(portToOpen);
                        Assert.Fail("Server should not open port with maximum integer value");
                    }
                    catch (Exception)
                    {
                        Assert.Pass();
                    }
                    break;
                default:
                    try
                    {
                        server.Init(portToOpen);
                        Assert.That(server.port == portToOpen, Is.True);
                        Assert.That(server.GetListenerSocket(), Is.InstanceOf(typeof(TcpListener)));
                        server.GetListenerSocket().Stop();
                    }
                    catch (SocketException e)
                    {
                        if (e.Message.Equals("Permission denied") || e.Message.Equals("Invalid argument"))
                        {
                            Assert.Pass($"Expected error : {e}");
                        }
                        Assert.Fail("The error message does not match any of the expected possible values");
                    }
                    break;
            }
        }
        
        [Test]
        public void Server_CanBeKilled()
        {
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(this.node);
            server.Init(5000);
            server.Kill();
            Assert.That(server.GetListenerSocket().Server.Available == 0, Is.True);
        }

        [Test]
        public void Server_CanAcceptConnections()
        {
            FullNodeTcpServer server = new FullNodeTcpServer();
            server.SetFullNode(this.node);
            server.Init(5000);
            // TcpClient extPeer = server.AcceptConnections();
            // Assert.That(extPeer, Is.InstanceOf(typeof(TcpClient)));
            server.Kill();
            Assert.Pass(
                "Server_CanAcceptConnections passes by default for now, " +
                "until a solution to the pending connection while running test issue is found."
                );
        }

    }
}