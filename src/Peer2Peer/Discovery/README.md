# Peer Discovery
Peer Discovery is the problem of finding peers to connect to in a decentralised environment. 
As the peers are usually users with PCs sat behind firewalls, routers, private networks, etc, a truly decentralized approach to peer discovery is hard to develop.

# Options
1. Discovery Server - centralised, although can be built to work with minimum trust, it defeats the purpose of a truly dentralised platform
2. Distributed Hash Table (DHT) - to research
3. Master Peer List - **IN USE AT THE MOMENT OF WRITING**

## Master Peer List
This strategy involves a list of peer details which is circulated between peers on the ZRD network. A peer is documented with :
- The EXT IP to which public internet machines can connect to
- The open EXT port to connect to
- Type of node on given peer (e.g: FULL, MINER, LIGHTWEIGHT)

It has a genesis (see ```local/Peers/Peers.json```) with the first ever full running node's peer details.

This list is updated locally by the ZRD clients once certain operations are done, which register the user machine as a working, ready-to-peer node in the ZRD network.
This list is then broadcasted to the other peers in the list and merged if longer (or more recently updated).

# Suitable Peers
Suitable peer discovery is done by iterating through the list of peers in ```local/Peers/Peers.json``` and finding a stable, working peer which matches the node type requirements : for instance, a lightweight node will have to connect to a FULL node to send a transaction to. In that case, an active FULL node in the peer list will be suitable.

Peer operations (adding new peer. removing peer, finding peer) are done on the ```Peers.json``` file. Adding and removing peers in the codebase will work on a ```List<PeerDetails>``` in the code and will mutate the file contents at runtime.
