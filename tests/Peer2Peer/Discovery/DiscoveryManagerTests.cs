using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;

namespace Peer2PeerNS.DiscoveryNS.DiscoveryManagerTestsNS
{
    public class DiscoveryManagerTests
    {

        [TearDown]
        public void TearDown()
        {
            // Remove all TEST_PEERS_*.json files from bin/Debug/net5.0/
            List<string> testPeerFilepaths = new List<string>()
            {
                @"TEST_PEERS.json",
                @"TEST_PEERS_2.json",
                @"TEST_PEERS_3.json",
                @"TEST_PEERS_4.json",
                @"TEST_PEERS_DUPLICATE.json"
            };
            foreach (string filepath in testPeerFilepaths)
            {
                try
                {
                    if(File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                }
                catch (FileNotFoundException) { }
            }
        }

        [TestCase("127.0.0.1", 420, "FULL", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 420, "MINER", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 420, "FULL MINER", "TEST_PEERS.json")]
        [TestCase("", 420, "FULL", "TEST_PEERS.json")]
        [TestCase(null, 420, "FULL", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 0, "FULL", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", -1, "FULL", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 420, "", "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 420, null, "TEST_PEERS.json")]
        [TestCase("127.0.0.1", 420, "FULL", "")]
        [TestCase("127.0.0.1", 420, "FULL", null)]
        [TestCase("abcdefgh", 420, "FULL", "TEST_PEERS.json")]
        [TestCase("1023.266.913.213", 420, "FULL", null)]
        [TestCase("127.0.0.1", 420, "FULL", "TEST_PEERS_DUPLICATE.json")]  // Test case for duplicate entry where we expect an exception to be thrown
        public void DiscoveryManager_CanStoreExtNatIpAndPortToFile(string extNat, int port, string type,
            string filepath)
        {
            if (string.IsNullOrEmpty(extNat) ||
                string.IsNullOrEmpty(type) ||
                string.IsNullOrEmpty(filepath) ||
                port <= 0 ||
                extNat.Equals("abcdefgh") ||
                extNat.Equals("1023.266.913.213") ||
                type.Equals("FULL MINER")
               )
            {
                try
                {
                    DiscoveryManager dmng = new DiscoveryManager();
                    dmng.StoreExtNatIpAndPortToFile(extNat, port, type, filepath);
                    Assert.Fail("Discovery Manager should not store wrong data in file or try to save in empty filepath");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else if (type.Equals("TEST_PEERS_DUPLICATE.json"))
            {
                // Add duplicate and check for exception to be thrown
                try
                {
                    DiscoveryManager dmng = new DiscoveryManager();
                    dmng.StoreExtNatIpAndPortToFile(extNat, port, type, filepath);
                    dmng.StoreExtNatIpAndPortToFile(extNat, port, type, filepath); // <- this should throw exception
                }
                catch (Exception e)
                {
                    Assert.Pass($"Exception caught : {e.Message}");
                }
            }
            else
            {
                DiscoveryManager dmng = new DiscoveryManager();
                dmng.StoreExtNatIpAndPortToFile(extNat, port, type, filepath);
                // Check that file exists and that it has content in it
                string peerDetailsString = System.IO.File.ReadAllText(filepath);
                Assert.That(string.IsNullOrEmpty(peerDetailsString), Is.False);
            }
        }

        [TestCase("MINER")]
        [TestCase("FULL")]
        [TestCase("MINER FULL")]
        [TestCase("FULL MINER")]
        [TestCase("")]
        [TestCase(null)]
        public void DiscoveryManager_CanFindSuitablePeerInList(string type)
        {
            DiscoveryManager discoveryManager = new DiscoveryManager();
            List<PeerDetails> possiblePeers = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 420, "FULL"),
            };
            if (string.IsNullOrEmpty(type))
            {
                try
                {
                    discoveryManager.FindSuitablePeerInList(type, possiblePeers);
                    Assert.Fail("Discovery Manager should not find peers with null or empty types");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                PeerDetails suitablePeer = discoveryManager.FindSuitablePeerInList(type, possiblePeers);
                Assert.That(type.Split(" ").Contains(suitablePeer.PeerType), Is.True);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("TEST_PEERS_2.json")]
        public void DiscoveryManager_CanLoadPeerDetails(string filepath)
        {
            DiscoveryManager discoveryManager = new DiscoveryManager();
            if (string.IsNullOrEmpty(filepath))
            {
                try
                {
                    List<PeerDetails> peerList = discoveryManager.LoadPeerDetails(filepath);
                    Assert.Fail("Discovery Manager should not parse peer details from empty or null filepath");
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                discoveryManager.StoreExtNatIpAndPortToFile("127.0.0.1", 420, "FULL", filepath);
                List<PeerDetails> peerList = discoveryManager.LoadPeerDetails(filepath);
                Assert.That(peerList.Count, Is.EqualTo(1));   
            }
        }

        [TestCase("127.0.0.1", 420, "TEST_PEERS_3.json")]
        [TestCase("", 420, "TEST_PEERS_3.json")]
        [TestCase("127.0.0.1", 0, "TEST_PEERS_3.json")]
        [TestCase(null, 420, "TEST_PEERS_3.json")]
        [TestCase("127.0.0.1", 420, "")]
        [TestCase("127.0.0.1", 420, null)]
        public void DiscoveryManager_CanRemovePeerFromPeerListFile(string hostname, int port, string filepath)
        {
            DiscoveryManager discoveryManager = new DiscoveryManager();
            discoveryManager.StoreExtNatIpAndPortToFile("127.0.0.1", 420, "MINER", "TEST_PEERS_3.json");
            discoveryManager.StoreExtNatIpAndPortToFile("127.0.0.1", 425, "FULL", "TEST_PEERS_3.json");
            
            if (string.IsNullOrEmpty(hostname) ||
                string.IsNullOrEmpty(filepath) ||
                port <= 0)
            {
                try
                {
                    bool deletedPeer = discoveryManager.RemovePeerFromPeerListFile(hostname, port, filepath);
                    Assert.Fail(
                        "Discovery Manager should not attempt to delete peer with missing details " +
                        "or from empty or null filepath"
                    );
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                bool deletedPeer = discoveryManager.RemovePeerFromPeerListFile(hostname, port, filepath);
                // Assert that a peer was deleted from the list by checking the bool value and the list size
                Assert.That(deletedPeer, Is.EqualTo(true));
                List<PeerDetails> peers = discoveryManager.LoadPeerDetails("TEST_PEERS_3.json");
                Assert.That(peers.Count, Is.EqualTo(1));
            }

        }

        [TestCase("TEST_PEERS_4.json")]
        [TestCase("")]
        [TestCase(null)]
        public void DiscoveryManager_CanStorePeerList(string filepath)
        {
            DiscoveryManager discoveryManager = new DiscoveryManager();
            List<PeerDetails> possiblePeers = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 425, "FULL"),
            };

            if (string.IsNullOrEmpty(filepath))
            {
                try
                {
                    discoveryManager.WritePeerListToFile(possiblePeers, filepath);
                    Assert.Fail(
                        "Discovery Manager should not attempt to store peer lists " +
                        "in null or empty filepaths"
                        );
                }
                catch (Exception)
                {
                    Assert.Pass();
                }
            }
            else
            {
                discoveryManager.WritePeerListToFile(possiblePeers, filepath);
                // Check that the file content matches the runtime list content
                List<PeerDetails> peerDetailsList = discoveryManager.LoadPeerDetails(filepath);
                Assert.That(peerDetailsList.Count == possiblePeers.Count, Is.True);
            }
        }

        [Test]
        public void Static_DiscoveryManager_MergesPeerListsCorrectly()
        {
            // ======================== COMMON CASE ========================
            // Construct lists
            List<PeerDetails> peerList1 = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 425, "FULL"),
                new PeerDetails("127.0.0.1", 430, "MINER"),
                new PeerDetails("127.0.0.1", 435, "FULL")
            };
            List<PeerDetails> peerList2 = new List<PeerDetails>
            {
                new PeerDetails("127.0.2.1", 420, "MINER"),
                new PeerDetails("127.0.2.1", 425, "FULL"),
                new PeerDetails("127.0.2.1", 430, "MINER"),
                new PeerDetails("127.0.2.1", 435, "FULL")
            };
            
            // Merge lists
            List<PeerDetails> mergedList = DiscoveryManager.MergePeerLists(peerList1, peerList2);
            
            // Assert on list length, objects, etc.
            Assert.That(mergedList.Count, Is.EqualTo(8));
            
            // ======================== DUPLICATE CASE ========================
            // Construct lists
            List<PeerDetails> peerList3 = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 425, "FULL"),
                new PeerDetails("127.0.0.1", 430, "MINER"),
                new PeerDetails("127.0.0.1", 435, "FULL")
            };
            List<PeerDetails> peerList4 = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 425, "FULL"),
                new PeerDetails("127.0.2.1", 430, "MINER"),
                new PeerDetails("127.0.2.1", 435, "FULL")
            };
            
            // Merge lists
            List<PeerDetails> mergedList2 = DiscoveryManager.MergePeerLists(peerList3, peerList4);
            
            // Assert on list length, objects, etc.
            Assert.That(mergedList2.Count, Is.EqualTo(6));
            
            // ======================== DUPLICATE CASE - TYPE ========================
            // Construct lists
            List<PeerDetails> peerList5 = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "MINER"),
                new PeerDetails("127.0.0.1", 425, "FULL"),
                new PeerDetails("127.0.0.1", 430, "MINER"),
                new PeerDetails("127.0.0.1", 435, "FULL")
            };
            List<PeerDetails> peerList6 = new List<PeerDetails>
            {
                new PeerDetails("127.0.0.1", 420, "FULL"), // <- culprit, in same list, cannot have 2 different nodes with same connection details
                new PeerDetails("127.0.2.1", 425, "FULL"),
                new PeerDetails("127.0.2.1", 430, "MINER"),
                new PeerDetails("127.0.2.1", 435, "FULL")
            };
            
            // Merge lists
            List<PeerDetails> mergedList3 = DiscoveryManager.MergePeerLists(peerList5, peerList6);

            // Assert on list length, objects, etc.
            Assert.That(mergedList3.Count, Is.EqualTo(7));
            
        }
        
    }
}