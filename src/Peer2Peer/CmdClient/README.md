# Command Line Client
The initial ZRD application will be based off a command line client.
This client involves setting up one of the documented node types (full, miner, lightweight) and their respective configs to work.

By progressing through the command line options, the node configs will be mutated until the node is fully configured and functional for its specific purpose (mine, broadcast or transact). 

# Node Setups & Workflows
## Lightweight Node
- Sets up Wallet via cmdline
- Stores Wallet keypair and other metadata locally
  - At this point, a user can login into existing locally-stored wallet
    - TODO: Implement master password for wallet access 
- Changes Wallet name & other metadata
  - TODO: Implement wallet creation timestamp
- Sends new transactions to full nodes (via direct TCP to full node peer)
  - TODO: Peer networking (IPs & ports) considerations: initial master list that is broadcasted, central discovery server, distributed discovery server, peer discovery mesh ?
- Loads Blockchain state from local file (if existing)
- Listens for new Blockchain states from full nodes 
  - Same considerations as for Peer networking

## Full Node
In progress.

## Miner Node
In progress.