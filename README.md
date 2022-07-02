# ZRD
Local blockchain network built in C# and dotnet.
Supports the ZR420 cryptocurrency and user wallets for transactions (send, receive).

Uses .net6.0 and nUnit for testing.

# Progress
[**IN PROGRESS**] Models (Block, Transaction, Blockchain, Merkel Tree)

[**IN PROGRESS**] Storage Considerations & Solution

[**IN PROGRESS**] Nodes (Full Node, Lightweight Node, Miner Node)

[**COMPLETED**] Genesis Block

[**COMPLETED**] Minting (**NOTE: this is not applicable as ZRD will use the PoW consensus mechanism. minting uses Proof of Stake**)

[**COMPLETED**] Mining

[**COMPLETED**] Process Transactions (send, receive, rewards, etc.)

# CI/CD
## CI
Continuous integration is implemented using GitHub Actions and runs ```dotnet build``` and ```dotnet test``` in a dotnet environment.
This runs the test harness on all Pull Request events.

To see the CI workflow file, visit [continuous-integration.yml](.github/workflows/continuous-integration.yml)

## CD
Releases are automatically created on push events to the ```main``` branch.

Pipeline work in progress.

To see the CD workflow file, visit [continuous-deployment.yml](.github/workflows/continuous-deployment.yml)

# Resources
1. https://www.freecodecamp.org/news/build-a-blockchain-in-golang-from-scratch/
2. https://levelup.gitconnected.com/learn-blockchain-by-building-it-f2f8ccc54892
3. https://enlear.academy/merkle-tree-the-root-of-bitcoin-5a9062394fbf
4. https://medium.com/nerd-for-tech/smart-contract-with-golang-d208c92848a9
5. https://javascript.plainenglish.io/lets-create-a-cryptocurrency-for-fun-using-javascript-42894b50e44c
6. https://mycoralhealth.medium.com/code-your-own-blockchain-in-less-than-200-lines-of-go-e296282bcffc
7. https://dev.to/freakcdev297/build-a-p2p-network-and-release-your-cryptocurrency-clf
8. https://www.c-sharpcorner.com/article/blockchain-basics-building-a-blockchain-in-net-core/
9. https://www.c-sharpcorner.com/article/blockchain-basic-node/
10. https://www.c-sharpcorner.com/article/building-a-blockchain-in-net-core-p2p-network/
11. https://blog.bitstamp.net/post/what-are-blockchain-nodes/
