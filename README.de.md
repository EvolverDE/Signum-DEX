[![English](https://img.shields.io/badge/lang-es-yellow.svg)](/README.md)


# Was ist SnipSwap?
SnipSwap ist eine dezentrale Krypto-Tauschbörse mit PayPal-Integration.

# Was kann SnipSwap?
SnipSwap soll einen einfachen Einstieg in die Krypto-Welt ermöglichen.

# Welche Währungen werden unterstützt?
SnipSwap basiert auf den Smart Contracts der Signum.network Blockchain und unterstützt neben dessen Coin "Signa" alle verfügbaren Fiat-Währungen die PayPal auch unterstützt. Diese sind unter folgendem Link zu finden: https://developer.paypal.com/docs/api/reference/currency-codes/

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
Da SnipSwap auf dem .NET-Framework in der Version 4.8 basiert, ist ein Betriebssystem welches dieses unterstützt ebenfalls Vorraussetzung.

# Wie installiert man SnipSwap?
Die SnipSwap.EXE-Datei wird unter https://github.com/EvolverDE/Signum-DEX/releases/tag/v1 zur Verfügung gestellt, die an einen beliebigen Ort auf einer Festplatte verschoben und dort genutzt werden kann.

# Welche Einstellungen müssen vorgenommen werden?
Die Einstellungen werden mit Standard-Werten neben der SnipSwap.EXE-Datei automatisch in einer Settings.ini-Datei erstellt. Neben dem Programm können auch dort direkt Änderungen vorgenommen werden.

# Was bedeuten die anderen Dateien, die neben der SnipSwap.EXE erstellt werden?
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