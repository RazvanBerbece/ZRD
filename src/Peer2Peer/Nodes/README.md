# Node Intro for the ZRD Blockchain

Every device that wishes to participate in the ZRD network must run a client that follows the ZRD protocol. Each node continuously demands that the other nodes follow the same rules.

Since full nodes store the entire database of ZRD420 transactions, they can check whether a recently added transaction has already taken place in the past. They block any previously spent transactions in order to prevent **double spending**.

**Full nodes are, therefore, essential for maintaining the honesty of the entire network**.

All node types will have some access to TCP connectivity capabilities :
1. TCP Server creation, listener setup and incoming message handling
2. TCP Client creation, upstream connection and incoming & outbound message handling

# Node Categories

1. Full Node
2. Lightweight Node
3. Miner Node

# Node Specifications
## 1 Full Node
Full nodes store unconfirmed transactions in their memory pool, or mempool, and check whether transactions are valid according to ZRD’s consensus rules.
They also spread their blockchain status across the P2P network.

A full node should :
- Have a local copy of the entire blockchain stored
    - Blockchain instance -> genesis block, list, difficulty, unconfirmed transactions etc.
- Connect to peers and share the blockchain status (the saved data from above)
- Receive new transactions from peers and add them to the mempool
- Go through the unconfirmed transactions and validate them

A full node should NOT :
- Have a wallet other than the blockchain wallet configured
- Make transactions (sending, receiving coin)

## 2 Lightweight Node
A lightweight node functioning as a wallet sends a new transaction through to full nodes, which spread the information across the network.

It contains only a partial list of a blockchain data, which usually includes just the block headers (TODO: Figure out what data is needed for a header), instead of its entire transaction history.

A lightweight node should :
- Have a lightweight copy of the blockchain (headers)
- Have a wallet configured
- Be able to make transactions (send, receive coin)
- Get available balance for current wallet

## 3 Miner Node
Mining nodes confirm transactions by including them in blocks (mining unconfirmed transactions).

Miners take transactions from the mempool to confirm them. They do so by including transactions into blocks through the process of mining. Miners are rewarded for their work with mining rewards and transaction fees, while full nodes act as assistants to keep the Bitcoin network decentralized and honest.

A miner node should :
- Have a full copy of the blockchain
- Solve the cryptographic puzzle based on the blockchain difficulty