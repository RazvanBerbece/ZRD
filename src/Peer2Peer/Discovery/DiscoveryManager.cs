using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Peer2PeerNS.DiscoveryNS.DiscoveryManagerNS
{
    /// <summary>
    /// Class that handles logging of current node IPs, ports, node types and discovery of peers
    /// </summary>
    public class DiscoveryManager
    {

        public DiscoveryManager() { }

        /// <summary>
        /// Stores the current machine's external NAT IP address and the open port in the Peers.json file
        /// This is used for peer discovery
        /// </summary>
        /// <param name="extNatIp">Public internet IP address</param>
        /// <param name="port">Port that peers can connect to (NOTE: THIS IS USUALLY PORT FORWARDED)</param>
        /// <param name="nodeType">Type of current node: lightweight, full, miner, etc.</param>
        /// <param name="filepath">Filepath to store potential peer list at</param>
        public void StoreExtNatIpAndPortToFile(string extNatIp, int port, string nodeType, string filepath)
        {
            // Create struct with passed details
            PeerDetails details = new PeerDetails(extNatIp, port, nodeType);
            // Deserialize data from Peers json file & add new record in
            List<PeerDetails> peerList = LoadPeerDetails(filepath);
            if (peerList == null)
            {
                // No peers were found in the list, so this is the first peer to be added
                List<PeerDetails> initList = new List<PeerDetails> { };
                initList.Add(details);
                string peerDetails = JsonSerializer.Serialize(
                    initList,
                    options: new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    }
                );
                // Write to file
                System.IO.File.WriteAllText(filepath, peerDetails);
                return;
            }
            // Sanity check for duplicate entries in list, duplicates are not allowed
            foreach (PeerDetails peerDetails in peerList)
            {
                if (
                    peerDetails.GetPort() == details.GetPort() &&
                    peerDetails.GetExtIp() == details.GetExtIp() &&
                    peerDetails.GetPort() == details.GetPort()
                )
                {
                    // Found duplicate peer entry, refuse it and return without storing
                    Console.WriteLine("Peers.json already has your current full node configuration");
                    return;
                }
            }
            peerList.Add(details);
            // Serialize updated (appended) list
            string jsonPeerList = JsonSerializer.Serialize(
                peerList,
                options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                }
            );
            // Write to file
            System.IO.File.WriteAllText(filepath, jsonPeerList);
        }

        /// <summary>
        /// Loads and returns a list of all PeerDetails in Peers.json
        /// </summary>
        /// <returns></returns>
        public List<PeerDetails> LoadPeerDetails(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentException("Filepath cannot be null or empty");
            }
            try
            {
                string peerDetailsString = System.IO.File.ReadAllText(filepath);
                List<PeerDetails> peerList = JsonSerializer.Deserialize<List<PeerDetails>>(
                    peerDetailsString,
                    options: new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                    });
                return peerList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Finds the first suitable peer in the list of possible peers.
        /// This means looking for a peer that matches the current node op:
        ///     1. To send new transaction to (light -> full)
        ///     2. To download blockchain copy from (full -> full, full -> miner)
        ///     3. To send new blockchain to (miner -> full, full -> full, full -> light)
        /// The method scans that the given IP and port are reachable
        /// TODO: If not, maybe delete from list ?
        /// </summary>
        /// <param name="requiredPeerTypes">String representation of peer type(s) (e.g.: LIGHT, FULL, MINER, FULL MINER)</param>
        /// <param name="possiblePeers">List of PeerDetails structs</param>
        /// <returns></returns>
        public PeerDetails FindSuitablePeerInList(string requiredPeerTypes, List<PeerDetails> possiblePeers)
        {
            string[] types = requiredPeerTypes.Split(' ');
            
            PeerDetails suitablePeer = new PeerDetails();
            foreach (PeerDetails peer in possiblePeers)
            {
                if (types.Contains(peer.PeerType))
                {
                    // Check that a connection can be made to peer
                    // TODO
                    // Found suitable peer, update variable and break loop
                    suitablePeer = peer;
                    break;
                }
            }

            return suitablePeer;
        }

        public void RemovePeerFromPeerList(string hostname, int port)
        {
            throw new NotImplementedException();
        }

    }
}