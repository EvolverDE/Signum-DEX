Scroll down for English

# Was ist PFP?
PFP (Perls for Pigs) ist der codename für eine dezentrale Krypto-Tauschbörse mit PayPal-Integration.
# Was kann PFP?
PFP soll einen einfachen Einstieg in die Krypto-Welt ermöglichen.
# Welche Währungen werden unterstützt?
PFP basiert auf den Smart Contracts der Signum.network Blockchain und unterstützt neben dessen Coin "Signa" alle verfügbaren Fiat-Währungen die PayPal auch unterstützt. Diese sind unter folgendem Link zu finden: https://developer.paypal.com/docs/api/reference/currency-codes/

Des weiteren wird nun mit der ersten zusätzlichen Blockchain-implementierung AtomicSwaps mit bitcoin (BTC) möglich
Für die Interaktion mit BTC muss bitcoin core installiert sein und der Daemon mit zusätzlichen parametern gestartet werden:

`C:\coins\bitcoind.exe -testnet -datadir=C:\coindata\bitcoin_testnet -rpcuser=bitcoin -rpcpassword=bitcoin -txindex`
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

`C:\coins\bitcoind.exe -testnet -datadir=C:\coindata\bitcoin_testnet -rpcuser=bitcoin -rpcpassword=bitcoin -txindex`
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

#DISCLAIMER
THE SOFTWARE IS DELIVERED AS IS. NO LIABILITY IS ASSUMED FOR DAMAGES OF ANY KIND.