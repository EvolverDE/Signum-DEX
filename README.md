Scroll down for English version

# Was ist PFP?
PFP (Perls for Pigs) ist der codename für eine dezentrale Krypto-Tauschbörse mit PayPal-Integration.
# Was kann PFP?
PFP soll einen einfachen Einstieg in die Krypto-Welt ermöglichen.
# Welche Währungen werden unterstützt?
PFP basiert auf den Smart Contracts der Signum.network Blockchain und unterstützt neben dessen Coin "Signa" alle verfügbaren Fiat-Währungen die PayPal auch unterstützt. Diese sind unter folgendem Link zu finden: https://developer.paypal.com/docs/api/reference/currency-codes/

Des weiteren wird nun mit der ersten zusätzlichen Blockchain-implementierung AtomicSwaps mit bitcoin (BTC) möglich.
Für die Interaktion mit BTC muss bitcoin core installiert sein und der Daemon mit zusätzlichen parametern gestartet werden:

`C:\pfad\zu\bitcoind.exe -conf=C:\pfad\zu\bitcoin.conf`

Für die bitcoin.conf wird folgender Inhalt festgelegt:

[TESTNET]
```
testnet=1
datadir=C:\pfad\zu\bitcoin_testnet
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
datadir=C:\pfad\zu\bitcoin_mainnet
txindex=1
blockfilterindex=1

[main]
rpcauth=bitcoin:a96660a39639686cabe0bc8846e199e3$148bf41f57d597e12fa3a8622857b323b09b9588ce4944fa371e5fea0eecaee8
#rpcpass=j5Sjnqcxb8KrKGKO7lHyIGHatVfUBJtefatcTWu49jU
onlynet=ipv4
rpcbind=0.0.0.0:8332
rpcallowip=192.168.1.0/24
```

# Welche Vorraussetzungen braucht man?
Da PFP auf dem .NET-Framework in der Version 4.8 basiert, ist ein Betriebssystem welches dieses unterstützt ebenfalls Vorraussetzung.
# Wie installiert man PFP?
In der Entwicklungsphase wird eine PFP.EXE-Datei unter https://github.com/EvolverDE/Signum/blob/master/PFP/PFP/bin/Debug/PFP.exe zur Verfügung gestellt, die an einen beliebigen Ort auf einer Festplatte verschoben und dort genutzt werden kann.
# Welche Einstellungen müssen vorgenommen werden?
Die Einstellungen werden mit Standard-Werten neben der PFP.EXE-Datei automatisch in einer Settings.ini-Datei erstellt. Neben dem Programm können auch dort direkt Änderungen vorgenommen werden.
# Was bedeuten die anderen Dateien, die neben der PFP.EXE erstellt werden?
Neben der Settings.ini werden noch weitere Dateien wie z.b. "cache.dat" sowie mögliche .LOG-Dateien erstellt. Die cache.dat enthält informationen aller auf der Signum.network blockchain befindlichen Smart Contracts.

# Hier ist eine kleine Funktionsübersicht
- sichere Verbindung zu öffentlichen Signum-nodes aufbauen dank integrierter EC-KCDSA und curve25519 Lösungen
- Lastenausgleich auf externe Signum-Nodes beim Einholen der Smart Contract-Daten wie z.b. der Transaktionshistorie schaffen
- wiederverwendbare payment-channels (smart contracts) für jedermann zur Verfügung stellen
- eine TCP API für externe Applikationen (über Standardport 8130) zur verfügung stellen
- mit dem integriertem auf TCP basiertem DEXNET (über Standardport 8131) das Signa-Netzwerk um offchain-Lösungen erweitern um das Handels-Erlebnis zu verbessern
- ~nun auch mono projekt kompatibel~ (leider nicht mehr lauffähig)

# Bekannte Probleme
- dauerschleife in der PayPal-Sandbox wenn man PayPal-Orders über die Sandbox bezahlen will
- hin und wieder kommen Fehler vor, die entweder in der unteren Statusleiste des Programms oder in einer Error.log geworfen werden

# HAFTUNGSAUSSCHLUSS
DIE SOFTWARE WIRD SO GELIEFERT, WIE SIE IST. ES WIRD KEINE HAFTUNG FÜR SCHÄDEN JEGLICHER ART ÜBERNOMMEN.

----------------------------------

# What is PFP?
PFP (Perls for Pigs) is the codename for a decentralized crypto exchange with PayPal integration.
# What can PFP do?
PFP is designed to provide an easy entry into the crypto world.
# What currencies are supported?
PFP is based on the Signum.network blockchain smart contracts and supports all available fiat currencies that PayPal also supports, in addition to its coin "Signa". These can be found at the following link: https://developer.paypal.com/docs/api/reference/currency-codes/

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
Since PFP is based on the .NET framework in version 4.8, an operating system that supports it is also required.
# How to install PFP?
In the development phase, a PFP.EXE file is provided at https://github.com/EvolverDE/Signum/blob/master/PFP/PFP/bin/Debug/PFP.exe, which can be moved to any location on a hard disk and used there.
# What settings need to be made?
The settings are automatically created with default values next to the PFP.EXE file in a Settings.ini file. Besides the program, changes can also be made there directly.
# What do the other files mean, which are created beside the PFP.EXE?
Beside the Settings.ini other files like "cache.dat" and possible .LOG files are created. The cache.dat contains information of all smart contracts located on the Signum.network blockchain.

# Here is a small function overview
- establish secure connections to public Signum-nodes thanks to integrated EC-KCDSA and curve25519 solutions
- create load balancing on external signum nodes when fetching smart contract data such as transaction history
- make reusable payment channels (smart contracts) available to everyone
- provide a TCP API for external applications (via standard port 8130)
- extend the Signa network with offchain solutions to enhance the trading experience with the integrated TCP based DEXNET (via standard port 8131)
- ~now also mono project compatible~ (unfortunately not running anymore)

# Known issues
- continuous loop in PayPal sandbox when trying to pay PayPal orders via the sandbox
- every now and then errors occur, which are either thrown in the lower status bar of the program or in an error.log

# DISCLAIMER
THE SOFTWARE IS DELIVERED AS IS. NO LIABILITY IS ASSUMED FOR DAMAGES OF ANY KIND.