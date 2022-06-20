# Peer2Peer Support for ZRD
In order for ZRD to be truly decentralised, it needs to handle storage and deployment through peer-2-peer networking.

# Technical Implementation
The p2p network will be implemented through direct TCP connections between nodes.
The TCP connections between nodes will manage :
- Sharing Blockchain state data bi-directional (full blockchains, light blockchains)
- New transaction posting (from lightweight nodes to full nodes)
- Other ? (to consider)

# Resources
1. https://en.wikipedia.org/wiki/Hole_punching_%28networking%29
2. https://www.c-sharpcorner.com/article/building-a-blockchain-in-net-core-p2p-network/