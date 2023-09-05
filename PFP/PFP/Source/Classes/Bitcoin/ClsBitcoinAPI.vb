
Option Strict On
Option Explicit On

Imports System.IO
Imports System.Net
Imports System.Reflection.Emit
Imports System.Text
Imports Microsoft.VisualBasic.Devices
Imports PFP.ClsTransaction
'Imports BitcoinNET.ClsBitcoinNET

''' <summary>
''' this class is used for raw communication with the bitcoin-node
''' </summary>
Public Class ClsBitcoinAPI

    Property C_API_URL As String = ""
    'Property API_URL() As String
    '    Get
    '        Return C_API_URL
    '    End Get
    '    Set(value As String)
    '        C_API_URL = value
    '    End Set
    'End Property

    Private C_API_Wallet As String = ""
    Property API_Wallet() As String
        Get
            Return C_API_Wallet
        End Get
        Set(value As String)
            C_API_Wallet = value
        End Set
    End Property

    Private ReadOnly Property Full_API_URL As String
        Get
            Return C_API_URL + "/wallet/" + C_API_Wallet
        End Get
    End Property

    ReadOnly Property C_API_User() As String
    ReadOnly Property C_API_Password() As String

    Public Enum BTC_API_CALLS

        'Blockchain RPCs
        getbestblockhash = 0
        getblock = 1
        getblockchaininfo = 2
        getblockcount = 3
        getblockfilter = 4
        getblockhash = 5
        getblockheader = 6
        getblockstats = 7
        getchaintips = 8
        getchaintxstats = 9
        getdifficulty = 10
        getmempoolancestors = 11
        getmempooldescendants = 12
        getmempoolentry = 13
        getmempoolinfo = 14
        getrawmempool = 15
        gettxout = 16
        gettxoutproof = 17
        gettxoutsetinfo = 18
        preciousblock = 19
        pruneblockchain = 20
        savemempool = 21
        scantxoutset = 22
        verifychain = 23
        verifytxoutproof = 24

        'Control RPCs
        getmemoryinfo = 25
        getrpcinfo = 26
        help = 27
        logging = 28
        Stop_ = 29
        uptime = 30

        'Generating RPCs
        generateblock = 31
        generatetoaddress = 32
        generatetodescriptor = 33

        'Mining RPCs
        getblocktemplate = 34
        getmininginfo = 35
        getnetworkhashps = 36
        prioritisetransaction = 37
        submitblock = 38
        submitheader = 39

        'Network RPCs
        addnode = 40
        clearbanned = 41
        disconnectnode = 42
        getaddednodeinfo = 43
        getconnectioncount = 44
        getnettotals = 45
        getnetworkinfo = 46
        getnodeaddresses = 47
        getpeerinfo = 48
        listbanned = 49
        ping = 50
        setban = 51
        setnetworkactive = 52

        'Rawtransactions RPCs
        analyzepsbt = 53
        combinepsbt = 54
        combinerawtransaction = 55
        converttopsbt = 56
        createpsbt = 57
        createrawtransaction = 58
        decodepsbt = 59
        decoderawtransaction = 60
        decodescript = 61
        finalizepsbt = 62
        fundrawtransaction = 63
        getrawtransaction = 64
        joinpsbts = 65
        sendrawtransaction = 66
        signrawtransactionwithkey = 67
        testmempoolaccept = 68
        utxoupdatepsbt = 69

        'Util RPCs
        createmultisig = 70
        deriveaddresses = 71
        estimatesmartfee = 72
        getdescriptorinfo = 73
        getindexinfo = 74
        signmessagewithprivkey = 75
        validateaddress = 76
        verifymessage = 77

        'Wallet RPCs
        'Note:   the wallet RPCs are only available If Bitcoin Core was built With wallet support, which Is the Default.
        abandontransaction = 78
        abortrescan = 79
        addmultisigaddress = 80
        backupwallet = 81
        bumpfee = 82
        createwallet = 83
        dumpprivkey = 84
        dumpwallet = 85
        encryptwallet = 86
        getaddressesbylabel = 87
        getaddressinfo = 88
        getbalance = 89
        getbalances = 90
        getnewaddress = 91
        getrawchangeaddress = 92
        getreceivedbyaddress = 93
        getreceivedbylabel = 94
        gettransaction = 95
        getunconfirmedbalance = 96
        getwalletinfo = 97
        importaddress = 98
        importdescriptors = 99
        importmulti = 100
        importprivkey = 101
        importprunedfunds = 102
        importpubkey = 103
        importwallet = 104
        keypoolrefill = 105
        listaddressgroupings = 106
        listlabels = 107
        listlockunspent = 108
        listreceivedbyaddress = 109
        listreceivedbylabel = 110
        listsinceblock = 111
        listtransactions = 112
        listunspent = 113
        listwalletdir = 114
        listwallets = 115
        loadwallet = 116
        lockunspent = 117
        psbtbumpfee = 118
        removeprunedfunds = 119
        rescanblockchain = 120
        send = 121
        sendmany = 122
        sendtoaddress = 123
        sethdseed = 124
        setlabel = 125
        settxfee = 126
        setwalletflag = 127
        signmessage = 128
        signrawtransactionwithwallet = 129
        unloadwallet = 130
        upgradewallet = 131
        walletcreatefundedpsbt = 132
        walletlock = 133
        walletpassphrase = 134
        walletpassphrasechange = 135
        walletprocesspsbt = 136

        'getaddressesbyaccount = 11
        'listaccounts = 13

        listdescriptors = 137
        scanblocks = 138

    End Enum

    Sub New(ByVal API_URL As String, ByVal API_Wallet As String, ByVal API_User As String, ByVal API_Password As String)

        C_API_URL = API_URL
        C_API_Wallet = API_Wallet

        C_API_User = API_User
        C_API_Password = API_Password

    End Sub

    Function BuildRequestString(ByVal APICall As BTC_API_CALLS, ByVal Params As String) As String

        Dim RequestString As String = "error"

        Select Case APICall

            'Blockchain RPCs
            Case BTC_API_CALLS.getbestblockhash
            Case BTC_API_CALLS.getblock
            Case BTC_API_CALLS.getblockchaininfo
            Case BTC_API_CALLS.getblockcount
            Case BTC_API_CALLS.getblockfilter
            Case BTC_API_CALLS.getblockhash
            Case BTC_API_CALLS.getblockheader
            Case BTC_API_CALLS.getblockstats
            Case BTC_API_CALLS.getchaintips
            Case BTC_API_CALLS.getchaintxstats
            Case BTC_API_CALLS.getdifficulty
            Case BTC_API_CALLS.getmempoolancestors
            Case BTC_API_CALLS.getmempooldescendants
            Case BTC_API_CALLS.getmempoolentry
            Case BTC_API_CALLS.getmempoolinfo
            Case BTC_API_CALLS.getrawmempool
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""getrawmempool"", ""params"":[]}"
            Case BTC_API_CALLS.gettxout
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""gettxout"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.gettxoutproof
            Case BTC_API_CALLS.gettxoutsetinfo
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""gettxoutsetinfo"",""params"":[]}"
            Case BTC_API_CALLS.preciousblock
            Case BTC_API_CALLS.pruneblockchain
            Case BTC_API_CALLS.savemempool
            Case BTC_API_CALLS.scanblocks
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""scanblocks"",""params"":[" + Params + "]}"

            Case BTC_API_CALLS.scantxoutset
                'TODO: scantxoutset
                'arguments =  { 'action' => 'start', 'scanobjects' => [ 'addr(1A1ywbX9rARomY2HWu4W1Xcsx79Dn3N5dG)' ]  }
                './bitcoin-cli scantxoutset start "[\"addr(1A1ywbX9rARomY2HWu4W1Xcsx79Dn3N5dG)\"]"
                ''{"jsonrpc": "1.0", "id": "curltest", "method": "scantxoutset", "params": ["start", ["raw(76a91411b366edfc0a8b66feebae5c2e25a7b6a5d1cf3188ac)#fm24fxxy"]]}' 

                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""scantxoutset"",""params"":[" + Params + "]}"

            Case BTC_API_CALLS.verifychain
            Case BTC_API_CALLS.verifytxoutproof

                'Control RPCs
            Case BTC_API_CALLS.getmemoryinfo
            Case BTC_API_CALLS.getrpcinfo
            Case BTC_API_CALLS.help
            Case BTC_API_CALLS.logging
            Case BTC_API_CALLS.Stop_
            Case BTC_API_CALLS.uptime

                'Generating RPCs
            Case BTC_API_CALLS.generateblock
            Case BTC_API_CALLS.generatetoaddress
            Case BTC_API_CALLS.generatetodescriptor

                'Mining RPCs
            Case BTC_API_CALLS.getblocktemplate
            Case BTC_API_CALLS.getmininginfo
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""getmininginfo"", ""params"":[]}"
            Case BTC_API_CALLS.getnetworkhashps
            Case BTC_API_CALLS.prioritisetransaction
            Case BTC_API_CALLS.submitblock
            Case BTC_API_CALLS.submitheader

                'Network RPCs
            Case BTC_API_CALLS.addnode
            Case BTC_API_CALLS.clearbanned
            Case BTC_API_CALLS.disconnectnode
            Case BTC_API_CALLS.getaddednodeinfo
            Case BTC_API_CALLS.getconnectioncount
            Case BTC_API_CALLS.getnettotals
            Case BTC_API_CALLS.getnetworkinfo
            Case BTC_API_CALLS.getnodeaddresses
            Case BTC_API_CALLS.getpeerinfo
            Case BTC_API_CALLS.listbanned
            Case BTC_API_CALLS.ping
            Case BTC_API_CALLS.setban
            Case BTC_API_CALLS.setnetworkactive

                'Rawtransactions RPCs
            Case BTC_API_CALLS.analyzepsbt
            Case BTC_API_CALLS.combinepsbt
            Case BTC_API_CALLS.combinerawtransaction
            Case BTC_API_CALLS.converttopsbt
            Case BTC_API_CALLS.createpsbt
            Case BTC_API_CALLS.createrawtransaction
            Case BTC_API_CALLS.decodepsbt
            Case BTC_API_CALLS.decoderawtransaction
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""decoderawtransaction"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.decodescript
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""decodescript"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.finalizepsbt
            Case BTC_API_CALLS.fundrawtransaction
            Case BTC_API_CALLS.getrawtransaction
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""getrawtransaction"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.joinpsbts
            Case BTC_API_CALLS.sendrawtransaction
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""sendrawtransaction"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.signrawtransactionwithkey
            Case BTC_API_CALLS.testmempoolaccept
            Case BTC_API_CALLS.utxoupdatepsbt

                'Util RPCs
            Case BTC_API_CALLS.createmultisig
            Case BTC_API_CALLS.deriveaddresses
            Case BTC_API_CALLS.estimatesmartfee
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""estimatesmartfee"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.getdescriptorinfo
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""getdescriptorinfo"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.getindexinfo
            Case BTC_API_CALLS.signmessagewithprivkey
            Case BTC_API_CALLS.validateaddress
            Case BTC_API_CALLS.verifymessage
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""verifymessage"",""params"":[" + Params + "]}"
                'Wallet RPCs
                'Note:   the wallet RPCs are only available If Bitcoin Core was built With wallet support, which Is the Default.
            Case BTC_API_CALLS.abandontransaction
            Case BTC_API_CALLS.abortrescan
                RequestString = "{""jsonrpc"":""1.0"", ""id"":""1"",""method"":""abortrescan"",""params"": []}"
            Case BTC_API_CALLS.addmultisigaddress
            Case BTC_API_CALLS.backupwallet
            Case BTC_API_CALLS.bumpfee
            Case BTC_API_CALLS.createwallet
                'TODO: create wallet
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""createwallet"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.dumpprivkey
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""dumpprivkey"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.dumpwallet
            Case BTC_API_CALLS.encryptwallet
            Case BTC_API_CALLS.getaddressesbylabel
            Case BTC_API_CALLS.getaddressinfo
            Case BTC_API_CALLS.getbalance
                RequestString = "{""jsonrpc"": ""1.0"", ""id"":1, ""method"": ""getbalance"", ""params"": [ ""*"", 0, true, false]}"
            Case BTC_API_CALLS.getbalances
                RequestString = "{""jsonrpc"": ""1.0"", ""id"":1, ""method"": ""getbalances"", ""params"": []}"
            Case BTC_API_CALLS.getnewaddress
            Case BTC_API_CALLS.getrawchangeaddress
            Case BTC_API_CALLS.getreceivedbyaddress
                RequestString = "{""jsonrpc"": ""1.0"", ""id"":1, ""method"": ""getreceivedbyaddress"", ""params"": [" + Params + "]}"
            Case BTC_API_CALLS.getreceivedbylabel
            Case BTC_API_CALLS.gettransaction
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""gettransaction"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.getunconfirmedbalance
            Case BTC_API_CALLS.getwalletinfo
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""getwalletinfo"", ""params"":[]}"
            Case BTC_API_CALLS.importaddress
                'TODO: import address after create wallet
                '["myaddress", "testing", false]
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""importaddress"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.importdescriptors
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""importdescriptors"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.listdescriptors
                RequestString = "{""jsonrpc"": ""1.0"", ""id"":1, ""method"": ""listdescriptors"", ""params"": []}"
            Case BTC_API_CALLS.importmulti
            Case BTC_API_CALLS.importprivkey
            Case BTC_API_CALLS.importprunedfunds
            Case BTC_API_CALLS.importpubkey
            Case BTC_API_CALLS.importwallet
            Case BTC_API_CALLS.keypoolrefill
            Case BTC_API_CALLS.listaddressgroupings
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""listaddressgroupings"",""params"":[]}"
            Case BTC_API_CALLS.listlabels
            Case BTC_API_CALLS.listlockunspent
            Case BTC_API_CALLS.listreceivedbyaddress
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""listreceivedbyaddress"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.listreceivedbylabel
            Case BTC_API_CALLS.listsinceblock
            Case BTC_API_CALLS.listtransactions
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""listtransactions"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.listunspent

                If Params.Trim = "" Then
                    RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""listunspent"",""params"":[" + Params + "]}"
                Else
                    RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""listunspent"",""params"": [1, 9999999, [" + Params + "]]}"
                End If

            Case BTC_API_CALLS.listwalletdir
            Case BTC_API_CALLS.listwallets
            Case BTC_API_CALLS.loadwallet
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""loadwallet"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.lockunspent
            Case BTC_API_CALLS.psbtbumpfee
            Case BTC_API_CALLS.removeprunedfunds
            Case BTC_API_CALLS.rescanblockchain
            Case BTC_API_CALLS.send
            Case BTC_API_CALLS.sendmany
            Case BTC_API_CALLS.sendtoaddress
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""sendtoaddress"", ""params"":[" + Params + "]}"
            Case BTC_API_CALLS.sethdseed
            Case BTC_API_CALLS.setlabel
            Case BTC_API_CALLS.settxfee
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""settxfee"", ""params"":[" + Params + "]}"
            Case BTC_API_CALLS.setwalletflag
            Case BTC_API_CALLS.signmessage
            Case BTC_API_CALLS.signrawtransactionwithwallet
            Case BTC_API_CALLS.unloadwallet
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1,""method"":""unloadwallet"",""params"":[" + Params + "]}"
            Case BTC_API_CALLS.upgradewallet
            Case BTC_API_CALLS.walletcreatefundedpsbt
            Case BTC_API_CALLS.walletlock
            Case BTC_API_CALLS.walletpassphrase
                RequestString = "{""jsonrpc"":""1.0"", ""id"":1, ""method"":""walletpassphrase"", ""params"":[" + Params + "]}"
            Case BTC_API_CALLS.walletpassphrasechange
            Case BTC_API_CALLS.walletprocesspsbt
            Case Else
                RequestString = "error"
        End Select

        Return RequestString
    End Function


#Region "Blockchain RPCs"

    ''' <summary>
    ''' Gets the unconfirmed transaction output
    ''' </summary>
    ''' <param name="TX">The Transaction to check</param>
    ''' <param name="VOut">The output index of the given Transaction</param>
    ''' <param name="UnconformedToo">The Boolean to check in Unconfirmed ones</param>
    ''' <returns>vout,confirmations,value and hex of the lockingscript</returns>
    Public Function GetTXOut(ByVal TX As String, ByVal VOut As Integer, Optional ByVal UnconformedToo As Boolean = True) As String
        Dim TXOUT As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.gettxout, """" + TX + """, " + VOut.ToString + ", " + UnconformedToo.ToString.ToLower + "")))

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(TXOUT, ClsJSONAndXMLConverter.E_ParseType.JSON)

        Dim Confirmations As Integer = Converter.GetFirstInteger("confirmations")
        Dim Value As Double = Converter.GetFirstDouble("value")
        Dim HEX As String = Converter.FirstValue("hex").ToString()

        If Confirmations <> -1 And Value <> 0.0 And (HEX.Trim() <> "" And HEX.Trim() <> "False") Then
            Return "<vout>" + VOut.ToString() + "</vout><confirmations>" + Confirmations.ToString() + "</confirmations><value>" + String.Format("{0:#0.00000000}", Value) + "</value><hex>" + HEX + "</hex>"
        Else
            Return ""
        End If

        '{
        '	"result":
        '	{
        '		"bestblock":"000000000000000f45c8efa2e3eb383125ce3154ab9030582c96d686de204442",
        '		"confirmations":0,
        '		"value":0.00008100,
        '		"scriptPubKey":
        '		{
        '			"asm":"OP_DUP OP_HASH160 5176b296b00f400de0ea44dae8691d7715a29620 OP_EQUALVERIFY OP_CHECKSIG", 
        '			"hex":"76a9145176b296b00f400de0ea44dae8691d7715a2962088ac", 
        '			"address":"mnwhDr5MPnqPSnaux2Gro6t6NZoXjcSLUp", 
        '			"type":"pubkeyhash"
        '		},
        '		"coinbase":false
        '	},
        '	"error":null, "id": 1
        '}


    End Function

    ''' <summary>
    ''' This method needs to be Threaded
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <param name="Command">start, status, abort</param>
    ''' <returns></returns>
    Public Function ScanTXOUTSet(ByVal Address As String, Optional ByVal Command As String = "start") As String

        Dim TXOUT As String = ""
        '{"result":{"success":true,"txouts":29260757,"height":2476376,"bestblock":"000000000000001aa242fa8fea76be8bb11884241422c96d2a007c75238b0a08","unspents":[],"total_amount":0.00000000},"error":null, "id": 1}
        'Command = "abort"
        If Address.Trim() = "" Then
            TXOUT = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.scantxoutset, "")), 600000)
        Else
            TXOUT = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.scantxoutset, """" + Command + """, [{""desc"":""addr(" + Address + ")"", ""range"":1000}]")), 600000)
        End If

        Return TXOUT

    End Function

    ''' <summary>
    ''' This method needs to be Threaded
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <param name="Command">start, status, abort</param>
    ''' <returns></returns>
    Public Function ScanBlocks(ByVal Address As String, Optional ByVal Command As String = "start") As String

        '{"result"{"from_height":0,"to_height":2476375,"relevant_blocks":["0000000038ec9983077b7480f4ecc9abb58c33103d4d8418a09a64cc6cf7d402","0000000003d78d7af7ce345511ccb801c22231317fc26327a092e3327b33cf81","000000000000310a70b8fe7dc47227bb0e021c287f27e201c015d591f5c71f03"]},"error":null, "id": 1}

        Dim Response As String = ""
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.scanblocks, """" + Command + """, [""addr(" + Address + ")""]")), 600000)
        Return Response

    End Function

#End Region

#Region "Control RPCs"

#End Region

#Region "Generating RPCs"


#End Region

#Region "Mining RPCs"

    'Public Function SendToAddress(ByVal Address As String, ByVal Amount As Double, Optional ByVal Fee As Double = 0.00000001) As String

    '    Dim k = RequestFromBitcoinWallet(API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getmininginfo, "")))

    '    'Fee set
    '    Dim ResponseString As String = RequestFromBitcoinWallet(API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.settxfee, Math.Round(Fee, 8).ToString.Replace(",", "."))))

    '    If Not ResponseString.Trim = "" Then
    '        'set passphrase for 10 seconds
    '        ResponseString = RequestFromBitcoinWallet(API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.walletpassphrase, """" + Wallet_PassPhrase + """, 10")))

    '        If Not ResponseString.Trim = "" Then
    '            'send amount to recipient
    '            ResponseString = RequestFromBitcoinWallet(API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.sendtoaddress, """" + Address + """, " + Math.Round(Amount, 8).ToString.Replace(",", ".") + ","""",""""")))

    '        End If
    '    End If

    '    Return ResponseString
    'End Function


    'Public Function GetTXOutSetInfo() As String
    '    Dim TXOUT As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.gettxoutsetinfo, "")))
    '    Return TXOUT
    'End Function

    'Public Function ListTransactions(ByVal Account As String, Optional ByVal Count As Integer = 10, Optional ByVal From As Integer = 0) As String
    '    Dim TXs As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.listtransactions, """" + Account + """, " + Count.ToString + ", " + From.ToString + "")))
    '    Return TXs
    'End Function

    'Public Function GetRawMempool() As String
    '    Dim Mempool As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getrawmempool, "")))
    '    Return Mempool
    'End Function




    'Public Function GetRawTransaction(ByVal TX As String, Optional ByVal Path As String = "result/vout", Optional ByVal SenderRIPE160 As String = "") As List(Of String)
    '    Dim RawTxInfo As String = GetRawTransaction(TX)

    '    If RawTxInfo.Contains("result") Then
    '        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(RawTxInfo, ClsJSONAndXMLConverter.E_ParseType.JSON)

    '    Else

    '    End If

    '    Return New List(Of String)

    'End Function

    'Public Function VerifyMessage(ByVal Address As String, ByVal Signature As String, ByVal Message As String) As String
    '    Dim VerifyResponse As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.verifymessage, """" + Address + """, """ + Signature + """, """ + Message + """")))
    '    Return VerifyResponse
    'End Function

    Public Function GetMiningInfo() As String
        Dim MiningInfo As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getmininginfo, "")), 3000)
        Return MiningInfo
    End Function

#End Region

#Region "Network RPCs"

#End Region

#Region "Rawtransactions RPCs"
    Public Function GetRawTransaction(ByVal TX As String) As String
        Dim RawTxInfo As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getrawtransaction, """" + TX + """")))
        Return RawTxInfo

        '{
        '	"result":"0200000000010124d3ed953e57bd7785b54accd4f5c9729004b4c9c546a48b398239a6e4b1cb560000000000feffffff0210270000000000001976a914aa08a90b8b1d81da80deaf11564ab5b71bb7bf6288ac594e050000000000160014444c726b593faad2cbea9adc438b8834aaf483800247304402200c34d9c68ab9e9feae28e23efe37648e1ac0055ef55facede0ade92b507e1d29022021edc740861524d1c09190dcc4cdfe677b6e7e4503acf462e48d37cb41c4f9e301210376493706530b47ed1951460716cd49f9112188ddd72dc3fed016a1b3f6aa963258d52300",
        '	"error":null,
        '    "id": 1
        '}

    End Function

    Public Function SendRawTransaction(ByVal RawTX As String) As String
        Dim SendRawTXResponse As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.sendrawtransaction, """" + RawTX + """")))
        Return SendRawTXResponse
    End Function

    Public Function DecodeRawTransaction(ByVal RawTX As String) As String
        Dim DecRawTx As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.decoderawtransaction, """" + RawTX + """")))
        Return DecRawTx
    End Function

    'Public Function DecodeScript(ByVal Script As String) As String
    '    Dim DecScript As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.decodescript, """" + Script + """")))
    '    Return DecScript
    'End Function

#End Region

#Region "Util RPCs"
    Public Function GetFee(Optional Blocks As Integer = 1) As String
        Dim Fee As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.estimatesmartfee, Blocks.ToString())), 1000)
        Return Fee
    End Function

    Public Function GetDescriptorInfo(ByVal Descriptor As String) As String

        Dim DescriptorInfo As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getdescriptorinfo, """addr(" + Descriptor + ")""")), 1000)
        Return DescriptorInfo

    End Function

#End Region

#Region "Wallet RPCs"

    Private Function LoadWallet() As String
        Dim T_LoadWallet As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.loadwallet, """" + API_Wallet + """")))
        Return T_LoadWallet
    End Function

    Public Function LoadWallet(ByVal WalletName As String) As String

        Dim Response As String = "{""result"":{""name"":""" + WalletName.Trim + """,""warning"":""""},""Error"":null,""id"":1}"
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.loadwallet, """" + WalletName + """")))
        API_Wallet = WalletName
        Return Response

    End Function

    Public Function UnloadWallet(ByVal WalletName As String) As String

        Dim Response As String = "{""result"":{""name"":""" + WalletName.Trim + """,""warning"":""""},""Error"":null,""id"":1}"
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.unloadwallet, """" + WalletName + """")))
        API_Wallet = WalletName
        Return Response

    End Function

    Public Function CreateWallet(ByVal Name As String) As String ' , Optional ByVal Description As String = "", Optional ByVal Rescan As Boolean = True) As String
        Dim CreateWalletResponse As String = "{""result"":{""name"":""" + Name.Trim + """,""warning"":""""},""error"":null,""id"":1}"
        '                                                                                                                                           , disable_private_keys=false, blank=true, "passphrase"=null, avoid_reuse=false, descriptors=false, load_on_startup=null")))
        CreateWalletResponse = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.createwallet, """" + Name + """, true, true, null, false, true, null")))

        API_Wallet = Name

        Return CreateWalletResponse

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim XMLList As List(Of String) = JSON.GetFromJSON(CreateWalletResponse, "result/")

        'For Each XML As String In XMLList

        '    If XML.Contains("warning") Then

        '        Dim Warn As String = GetStringBetween(XML, "<warning>", "</warning>").Trim

        '        If Warn <> "" Then
        '            Return Warn
        '        End If

        '    End If

        'Next

        'Return "ok"
    End Function

    Public Function GetWalletInfo() As String
        LoadWallet()
        Dim WalletInfo As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getwalletinfo, "")), 1000)
        Return WalletInfo
    End Function

#Region "deprecaded"

    'Public Function ImportAddress(ByVal Address As String, Optional ByVal Label As String = "", Optional ByVal Rescan As Boolean = False) As String
    '    '["myaddress", "testing", false]
    '    Dim ImportAddressResponse As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.importaddress, """" + Address + """, """ + Label + """, " + Rescan.ToString.ToLower)), 60000)
    '    Return ImportAddressResponse
    'End Function

#End Region

    Public Function GetTransaction(ByVal TX As String) As String
        LoadWallet()
        Dim GetTX As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.gettransaction, """" + TX + """")))
        Return GetTX
    End Function

    Public Function AbortRescan() As String
        Dim Abort As String = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.abortrescan, "")), 1000)
        Return Abort
    End Function

    Public Function ListUnspent(Optional ByVal Address As String = "") As String
        LoadWallet()
        If Address.Trim <> "" Then
            Address = """" + Address + """"
        End If

        'Dim Unspends As String =
        Return RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.listunspent, Address)))
        'Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Unspends, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim Result As List(Of KeyValuePair(Of String, Object)) = Converter.Search(Of List(Of KeyValuePair(Of String, Object)))("result")

        'If Result.Count > 0 Then
        '    If Result(0).Value.GetType = GetType(List(Of KeyValuePair(Of String, Object))) Then
        '        Dim KeyVals As List(Of KeyValuePair(Of String, Object)) = DirectCast(Result(0).Value, List(Of KeyValuePair(Of String, Object)))
        '        Return KeyVals
        '    Else
        '        Dim KeyVals As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))
        '        KeyVals.Add(New KeyValuePair(Of String, Object)("result", Result))
        '        Return KeyVals
        '    End If
        'End If

        'Return New List(Of KeyValuePair(Of String, Object))

    End Function

    ''' <summary>
    ''' This Method needs to be Threaded
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <returns></returns>
    Public Function ImportDescriptors(ByVal Address As String, Optional ByVal FromTimestamp As String = """now""") As String

        If Address.Trim() = "" Then
            Return ""
        End If

        LoadWallet()

        ''{"jsonrpc": "1.0", "id": "curltest", "method": "scantxoutset", "params": ["start", ["raw(76a91411b366edfc0a8b66feebae5c2e25a7b6a5d1cf3188ac)#fm24fxxy"]]}' 
        'addr(mkmZxiEcEd8ZqjQWVZuC6so5dFMKEFpN2j)#02wpgw69
        'Dim test1 = DescSum_Create("addr(1A1ywbX9rARomY2HWu4W1Xcsx79Dn3N5dG)") 'should m6gdl6sh
        'Dim test2 = DescSum_Create("addr(mkmZxiEcEd8ZqjQWVZuC6so5dFMKEFpN2j)") 'should 02wpgw69
        'Dim test3 = DescSum_Create("raw(deadbeef)") 'should 89f8spxm

        Dim Requests As String = ""
        Requests += "[{"
        Requests += """desc"": ""addr(" + Address + ")#" + DescSum_Create("addr(" + Address + ")") + """, "
        Requests += """timestamp"": " + FromTimestamp + ", "
        Requests += """internal"": false, "
        Requests += """label"": """ + Address + """"
        Requests += "}]"

        Return RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.importdescriptors, Requests)), 600000)

    End Function

    Public Function GetBalance() As String
        LoadWallet()
        Dim Response As String = ""
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getbalance, "")))
        Return Response
    End Function

    Public Function GetBalances() As String
        LoadWallet()
        Dim Response As String = ""
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getbalances, "")))
        Return Response
    End Function

    Public Function GetReceivedByAddress(ByVal Address As String) As String
        LoadWallet()
        Dim Response As String = ""
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.getbalances, """" + Address + """, 1")))
        Return Response

    End Function

    Public Function ListDescriptors(ByVal Descriptor As String) As String
        LoadWallet()
        Dim Response As String = ""
        Response = RequestFromBitcoinNode(Full_API_URL, ReqStrToByte(BuildRequestString(BTC_API_CALLS.listdescriptors, "")))
        Return Response

    End Function

#End Region

#Region "ToolFunctions"

    Function ReqStrToByte(ByVal RequestString As String) As Byte()
        Dim ByteArray As Byte() = Encoding.UTF8.GetBytes(RequestString)
        Return ByteArray
    End Function

    Function RequestFromBitcoinNode(ByVal URL As String, ByVal ByteArray As Byte(), Optional ByVal TimeOut As Integer = 60000) As String

        Try

            Dim Request As WebRequest
            Request = WebRequest.Create(URL)

            If C_API_User.Trim = "" Or C_API_Password.Trim = "" Then
                Return ""
            End If

            Request.Credentials = New NetworkCredential(C_API_User, C_API_Password)
            Request.Method = "POST"
            Request.ContentType = "application/json-rpc;"
            Request.Timeout = TimeOut

            Request.ContentLength = ByteArray.Length
            Dim RequestStream As Stream = Request.GetRequestStream()

            RequestStream.Write(ByteArray, 0, ByteArray.Length)
            RequestStream.Close()

            Dim WebResponse As WebResponse = Request.GetResponse()

            Dim ResponseStream As Stream = WebResponse.GetResponseStream
            Dim ResponseReader As New StreamReader(ResponseStream)
            Dim Responsemsg As String = ResponseReader.ReadToEnd()

            Return Responsemsg

        Catch ex As WebException

            If Not IsNothing(ex.Response) Then
                Dim x = New StreamReader(ex.Response.GetResponseStream())
                Dim xstr As String = x.ReadToEnd()
                Return xstr
            End If

            Dim out As ClsOut = New ClsOut(Application.StartupPath)
            out.ErrorLog2File(Application.ProductName + "-error in RequestFromBitcoinNode(" + URL + ", " + ByteArrayToHEXString(ByteArray) + ", " + TimeOut.ToString() + ") -> ConvertThread(WebException): " + ex.Message)

            Return Application.ProductName + "-error in RequestFromBitcoinNode(" + URL + ", " + ByteArrayToHEXString(ByteArray) + ", " + TimeOut.ToString() + ") -> ConvertThread(WebException): " + ex.Message

        Catch ex As Exception

            Return Application.ProductName + "-error in RequestFromBitcoinNode(" + URL + ", " + ByteArrayToHEXString(ByteArray) + ", " + TimeOut.ToString() + ") -> ConvertThread(Exception): " + ex.Message

        End Try

    End Function

    Function DescSum_Create(ByVal s As String) As String

        Dim CHECKSUM_CHARSET As String = "qpzry9x8gf2tvdw0s3jn54khce6mua7l"

        Dim Symbols As List(Of Integer) = DescSum_Expand(s)
        Symbols.AddRange({0, 0, 0, 0, 0, 0, 0, 0})
        Dim Checksum As ULong = DescSum_PolyMod(Symbols) Xor 1UL

        Dim Result As String = ""

        For i As Integer = 0 To 7
            Dim Reverse As Integer = 7 - i
            Dim Multi As Integer = 5 * Reverse
            Dim Checksumed As ULong = Checksum >> Multi
            Dim Filtered As ULong = Checksumed And 31UL
            Result += CHECKSUM_CHARSET(CInt(Filtered))
        Next

        Return Result

    End Function

    Function DescSum_Expand(ByVal s As String) As List(Of Integer)

        Dim INPUT_CHARSET As String = "0123456789()[],'/*abcdefgh@:$%{}IJKLMNOPQRSTUVWXYZ&+-.;<=>?!^_|~ijklmnopqrstuvwxyzABCDEFGH`#""\ "

        Dim Groups As List(Of Integer) = New List(Of Integer)
        Dim Symbols As List(Of Integer) = New List(Of Integer)

        For Each c As Char In s

            If Not INPUT_CHARSET.Contains(c) Then
                Return Nothing
            End If

            Dim v As Integer = INPUT_CHARSET.IndexOf(c)

            Symbols.Add(v And 31)
            Groups.Add(v >> 5)

            If Groups.Count = 3 Then
                Symbols.Add((Groups(0) * 9) + (Groups(1) * 3) + Groups(2))
                Groups.Clear()
            End If

        Next
        If Groups.Count = 1 Then
            Symbols.Add(Groups(0))
        ElseIf Groups.Count = 2 Then
            Symbols.Add((Groups(0) * 3) + Groups(1))
        End If

        Return Symbols

    End Function

    Private Function DescSum_PolyMod(ByVal symbols As List(Of Integer)) As ULong

        Dim GENERATOR As List(Of ULong) = New List(Of ULong)({&HF5DEE51989, &HA9FDCA3312, &H1BAB10E32D, &H3706B1677A, &H644D626FFD})

        Dim chk As ULong = 1UL
        For Each value As Integer In symbols
            Dim top As ULong = chk >> 35
            chk = (chk And &H7FFFFFFFFUL) << 5UL Xor CULng(value)
            For i As Integer = 0 To 4
                If ((top >> i) And 1UL) <> 0 Then
                    chk = chk Xor GENERATOR(i)
                End If
            Next
        Next
        Return chk
    End Function

#End Region

End Class