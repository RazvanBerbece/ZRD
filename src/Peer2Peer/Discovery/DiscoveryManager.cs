using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Peer2PeerNS.DiscoveryNS.PeerDetailsNS;
using StaticsNS;
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
            // Guard - extNatIp, port and nodeType are in the correct format
            if (port <= 0 || port >= 65535)
            {
                throw new ArgumentOutOfRangeException("Port number should be between 1 and 65535");
            }
            // Create Regex pattern matcher
            Regex expression = new Regex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$");
            var results = expression.Matches(extNatIp);
            if (results.Count <= 0)
            {
                throw new ArgumentException("extNatIp should be a valid IPv4 address");
            }
            if (!nodeType.Equals("MINER") && !nodeType.Equals("FULL"))
            {
                throw new ArgumentException("Node type should be either FULL or MINER");
            }
            
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
                    peerDetails.Port == details.Port &&
                    peerDetails.ExtIp == details.ExtIp &&
                    peerDetails.PeerType.Equals(details.PeerType)
                )
                {
                    // Found duplicate peer entry, refuse it and return without storing
                    Console.WriteLine("Peers.json already has your current full node configuration");
                    throw new DuplicatePeerDetailInListException();
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
        /// <param name="filepath">Filepath where peer list is stored at</param>
        /// <returns>List of PeerDetails read from filepath</returns>
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
        /// <param name="isLightweightNode">Whether the caller is a wallet - this helps with connecting to server peers on same EXT IP</param>
        /// <returns>PeerDetails suitable object to connect to</returns>
        public PeerDetails FindSuitablePeerInList(string requiredPeerTypes, List<PeerDetails> possiblePeers, bool isLightweightNode)
        {
            string[] types = requiredPeerTypes.Split(' ');
            
            PeerDetails suitablePeer = new PeerDetails();
            foreach (PeerDetails peer in possiblePeers)
            {   
                // If the current peer in list has a suitable type
                // and if the current peer in list is NOT a node running on the same Ext Public IP
                // Then continue
                if (
                    (types.Contains(peer.PeerType) && 
                    !peer.ExtIp.Equals(Statics.GetExternalPublicIpAddress().ToString())) ||
                    isLightweightNode
                    )
                {
                    // Check that a connection can be made to peer
                    // TODO
                    // Found suitable peer, update variable and break loop
                    suitablePeer = peer;
                    break;
                }
            }
            if (string.IsNullOrEmpty(suitablePeer.ExtIp))
            {
                throw new PeerNotFoundInListException();
            }
            return suitablePeer;
        }   
        
        /// <summary>
        /// Removes a PeerDetails object from the Peers.json filepath passed as parameter
        /// Could be used in a scenario where :
        ///     - Full Node spins up and listens for transactions & chains
        ///     - After spinning up, the node PeerDetails are saved in Peers.json
        ///     - When the Full Node goes down, the PeerDetails entry is removed from Peers.json
        /// This scenario can be applied to miner nodes as well.
        /// </summary>
        /// <param name="hostname">IP in PeerDetails object</param>
        /// <param name="port">Port in PeerDetails object</param>
        /// <param name="filepath">Filepath to Peers.json file which contains the PeerDetails objects</param>
        /// <returns>true if peer was removed from list, false otherwise</returns>
        public bool RemovePeerFromPeerListFile(string hostname, int port, string filepath)
        {
            // Load peer list
            List<PeerDetails> peerList = LoadPeerDetails(filepath);
            
            // Check whether peerList has the target hostname and port
            foreach (PeerDetails peer in peerList)
            {
                if (peer.Port == port && peer.ExtIp.Equals(hostname))
                {
                    // found match, delete it from runtime list
                    peerList.Remove(peer);
                    
                    // Write new list back to peer list file
                    WritePeerListToFile(peerList, filepath);

                    return true;
                }
            }

            return false; // no element removed
        }
        
        /// <summary>
        /// Writes a JSON string representation of the passed list of PeerDetails to a file
        /// located at filepath
        /// </summary>
        /// <param name="peerList">List of PeerDetails objects to be written to filepath</param>
        /// <param name="filepath">Storage location of peerList</param>
        public void WritePeerListToFile(List<PeerDetails> peerList, string filepath)
        {
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
        /// Merges 2 lists of PeerDetails into one.
        /// Handles duplicates and non-conforming PeerDetails objects
        /// </summary>
        /// <param name="list1">List 1 to merge</param>
        /// <param name="list2">List 2 to merge</param>
        /// <returns>New list instance of PeerDetails with objects from both lists</returns>
        public static List<PeerDetails> MergePeerLists(List<PeerDetails> list1, List<PeerDetails> list2)
        {
            List<PeerDetails> unionList = new List<PeerDetails>();
            unionList.AddRange(list1);
            unionList.AddRange(list2);
            List<PeerDetails> mergedList = unionList 
                .GroupBy(d => new {d.Port,d.ExtIp})
                .Select(g => g.First())
                .ToList();
            return mergedList;
        }

    }

    public class PeerNotFoundInListException : Exception { }

    /// <summary>
    /// Exception thrown when there is a duplicate PeerDetail struct in a List of PeerDetail
    /// </summary>
    public class DuplicatePeerDetailInListException : Exception { }
    
}