[![DE (German)](https://img.shields.io/badge/lang-es-yellow.svg)](/README.de.md)

# What is SnipSwap?
SnipSwap is a decentralized crypto exchange with PayPal integration.

# What can SnipSwap do?
SnipSwap is designed to provide an easy entry into the crypto world.

# What currencies are supported?
SnipSwap is based on the Signum.network blockchain smart contracts and supports all available fiat currencies that PayPal also supports, in addition to its coin "Signa". These can be found at the following link: https://developer.paypal.com/docs/api/reference/currency-codes/

Furthermore, with the first additional blockchain implementation, AtomicSwaps with bitcoin (BTC) is now possible.
To interact with BTC, bitcoin core must be installed and the daemon must be started with additional parameters:

`C:\path\to\bitcoind.exe -conf=C:\path\to\bitcoin.conf`

The following content is defined for the bitcoin.conf:

[TESTNET]
```
testnet=1
datadir=C:\path\to\bitcoin_testnet
txindex=1
blockfilterindex=1

[test]
rpcauth=bitcointest:64df7b3d85540109cc356d46180b7cfe$79455d660e5b9ec940912bf619c1ff959462ff9c663ae79cc4a49c3ed165f72b
#rpcpass=C8_o5A4uu7RHd6Fs8yStABQLSnZ8WWO49tTqp9UD1Bo
onlynet=ipv4
rpcbind=0.0.0.0:18332
rpcallowip=192.168.1.0/24
```

[MAINNET]
```
testnet=0
datadir=C:\path\to\bitcoin_mainnet
txindex=1
blockfilterindex=1

[main]
rpcauth=bitcoin:a96660a39639686cabe0bc8846e199e3$148bf41f57d597e12fa3a8622857b323b09b9588ce4944fa371e5fea0eecaee8
#rpcpass=j5Sjnqcxb8KrKGKO7lHyIGHatVfUBJtefatcTWu49jU
onlynet=ipv4
rpcbind=0.0.0.0:8332
rpcallowip=192.168.1.0/24
```

# What are the prerequisites?
Since SnipSwap is based on the .NET framework in version 4.8, an operating system that supports it is also required.

# How to install SnipSwap?
The SnipSwap.EXE file is provided at https://github.com/EvolverDE/Signum-DEX/releases/tag/v1, which can be moved to any location on a hard disk and used there.

# What settings need to be made?
The settings are automatically created with default values next to the SnipSwap.EXE file in a Settings.ini file. Besides the program, changes can also be made there directly.

# What do the other files mean, which are created beside the SnipSwap.EXE?
Beside the Settings.ini other files like "cache.dat" and possible .LOG files are created. The cache.dat contains information of all smart contracts located on the Signum.network blockchain.

# Here is a small function overview
- Establish secure connections to public Signum-nodes thanks to integrated EC-KCDSA and curve25519 solutions
- Create load balancing on external signum nodes when fetching smart contract data such as transaction history
- Make reusable payment channels (smart contracts) available to everyone
- Provide a TCP API for external applications (via standard port 8130)
- Extend the Signa network with offchain solutions to enhance the trading experience with the integrated TCP based DEXNET (via standard port 8131)
- Now also mono project compatible~ (unfortunately not running anymore)

# Known issues
- Continuous loop in PayPal sandbox when trying to pay PayPal orders via the sandbox
- Every now and then errors occur, which are either thrown in the lower status bar of the program or in an error.log

# DISCLAIMER
THE SOFTWARE IS DELIVERED AS IS. NO LIABILITY IS ASSUMED FOR DAMAGES OF ANY KIND.