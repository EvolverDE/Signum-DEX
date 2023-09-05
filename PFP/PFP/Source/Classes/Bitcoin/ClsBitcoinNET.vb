
Option Strict On
Option Explicit On

Imports Microsoft.VisualBasic.Devices
''' <summary>
''' this class is for data preparation from BTC_API
''' </summary>
Public Class ClsBitcoinNET
    Private Property BTC_API As ClsBitcoinAPI

    'Private Property C_Transaction As ClsTransaction

    Public Structure S_UnspentTransactionOutput
        Dim TransactionID As String
        Dim AmountNQT As ULong
        Dim VoutIDX As Integer
        Dim LockingScript As List(Of ClsScriptEntry)
        Dim Addresses As List(Of String)
        Dim Confirmations As Integer
        Dim Typ As AbsClsOutputs.E_Type

        Sub New(Optional ByVal x As String = "")
            LockingScript = New List(Of ClsScriptEntry)
            Addresses = New List(Of String)
        End Sub
    End Structure

    Public Structure S_ThreadedMethod
        Dim APICALL As ClsBitcoinAPI.BTC_API_CALLS
        Dim CallThread As Threading.Thread
        Dim Result As String
    End Structure

    Private Enum E_BitcoinConfigEntry
        BITCOINAPINODE = 0
        BITCOINWALLET = 1
        BITCOINAPIWALLET = 2
        BITCOINAPIUSER = 3
        BITCOINAPIPASSWORD = 4
        BITCOINDEXADDRESS = 5
        BITCOINMNEMONIC = 6
    End Enum

    Public Enum E_Output
        JSON = 0
        XML = 1
    End Enum

    Private Function GetBitcoinConfig(ByVal ConfigEntry As E_BitcoinConfigEntry, Optional ByVal DefaultValue As String = "") As String
        Return INIGetValue(BitcoinConfigFile, BitcoinConfigFileSection, ConfigEntry.ToString, DefaultValue)
    End Function

    'Private Sub SetBitcoinConfig(ByVal ConfigEntry As E_BitcoinConfigEntry, ByVal Value As String)
    '    INISetValue(BitcoinConfigFile, BitcoinConfigFileSection, ConfigEntry.ToString, Value)
    'End Sub

    Sub New()

        Dim API_URL As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINAPINODE, "http://127.0.0.1:18332")
        Dim API_Wallet As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINWALLET, "DEXWALLET")

        Dim API_User As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINAPIUSER, "bitcoin")
        Dim API_Password As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINAPIPASSWORD, "bitcoin")

        BTC_API = New ClsBitcoinAPI(API_URL, API_Wallet, API_User, API_Password)

    End Sub


#Region "Wallet handling"
    Public Function CreateNewWallet(ByVal WalletName As String) As Boolean

        If WalletName.Trim = "" Then
            Return False
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Dim Result As String = New ClsJSONAndXMLConverter(BTC_API.CreateWallet(WalletName), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        '<name>DEXWALLET</name><warning></warning>

        If IsErrorOrWarning(Result, Application.ProductName + "-error in CreateNewWallet(): -> ", True) Then
            Return False
        End If

        Return True

    End Function

    Public Function LoadWallet(ByVal WalletName As String) As Boolean

        If WalletName.Trim = "" Then
            Return False
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Dim Result As String = LoadWalletRaw(WalletName)
        '<name>DEXWALLET</name><warning></warning>
        '<error><code>-35</code><0>message</0><1>Wallet file verification failed. Refusing to load database. Data file 'C</1><2>\\Coinz\\bitcoin_testnet\\testnet3\\wallets\\DEXWALLET\\wallet.dat' is already loaded.</2></error>

        If IsErrorOrWarning(Result, Application.ProductName + "-error in LoadWallet(): -> ", True) Then
            Return False
        End If

        Return True

    End Function

    Public Function LoadWalletRaw(ByVal WalletName As String) As String

        If WalletName.Trim = "" Then
            Return ""
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Return New ClsJSONAndXMLConverter(BTC_API.LoadWallet(WalletName), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        '<name>DEXWALLET</name><warning></warning>
        '<error><code>-35</code><0>message</0><1>Wallet file verification failed. Refusing to load database. Data file 'C</1><2>\\Coinz\\bitcoin_testnet\\testnet3\\wallets\\DEXWALLET\\wallet.dat' is already loaded.</2></error>

    End Function

    Public Function UnloadWallet(ByVal WalletName As String) As Boolean

        If WalletName.Trim = "" Then
            Return False
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Dim Result As String = New ClsJSONAndXMLConverter(BTC_API.UnloadWallet(WalletName), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        '<name>DEXWALLET</name><warning></warning>

        If IsErrorOrWarning(Result, Application.ProductName + "-error in UnloadWallet(): -> ", True) Then
            Return False
        End If

        Return True

    End Function


#Region "deprecaded"

    'Public Function CreateNewAddress(ByVal Address As String) As Boolean

    '    Dim Result As String = ""

    '    Dim LambdaThread As New Threading.Thread(
    '        Sub()
    '            Result = BTC_API.ImportAddress(Address, "", False)
    '        End Sub
    '    )
    '    LambdaThread.Start()

    '    While LambdaThread.IsAlive
    '        Application.DoEvents()
    '    End While

    '    '{"result":null, "error": null, "id": 1}
    '    Result = New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

    '    If IsErrorOrWarning(Result, Application.ProductName + "-error in CreateNewAddress(): -> ", True) Then
    '        Return False
    '    End If

    '    Return True

    'End Function

    'Public Function ImportAddress(ByVal Address As String) As Boolean

    '    Dim Result As String = BTC_API.ImportAddress(Address, "", True)

    '    'Dim LambdaThread As New Threading.Thread(
    '    '    Sub()
    '    '        Result = BTC_API.ImportAddress(Address, "", True)
    '    '    End Sub
    '    ')
    '    'LambdaThread.Start()

    '    'While LambdaThread.IsAlive
    '    '    Application.DoEvents()
    '    'End While

    '    '{"result":null, "error": null, "id": 1}
    '    '{"result":null,"error":{"code":-4,"message":"Wallet is currently rescanning. Abort existing rescan or wait."},"id":1}
    '    Result = New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

    '    If IsErrorOrWarning(Result, Application.ProductName + "-error in ImportAddress(): -> ", True) Then
    '        Return False
    '    End If

    '    Return True

    'End Function

    'Public Function ImportAddress(ByVal Address As String, ByVal Output As E_Output) As String

    '    Dim Result As String = BTC_API.ImportAddress(Address, "", True)

    '    Return Result
    '    'return New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

    'End Function

#End Region

    Public Function ImportDescriptor(ByVal Address As String, Optional ByVal FromTimestamp As String = """now""") As String

        Dim Result As String = ""
        Dim found As Boolean = False
        For i As Integer = 0 To BitcoinNodeRequestList.Count - 1
            Dim T_Thread As S_ThreadedMethod = BitcoinNodeRequestList(i)
            If T_Thread.APICALL = ClsBitcoinAPI.BTC_API_CALLS.importdescriptors Then
                found = True

                If T_Thread.CallThread.ThreadState = Threading.ThreadState.Running Then
                    '{"result":{"walletname":"TESTWALLET","walletversion":169900,"format":"sqlite","balance":0.00012628,"unconfirmed_balance":0.00000000,"immature_balance":0.00000000,"txcount":3,"keypoolsize":0,"keypoolsize_hd_internal":0,"paytxfee":0.00000000,"private_keys_enabled":false,"avoid_reuse":false,"scanning":{"duration":0,"progress":0},"descriptors":true,"external_signer":false},"error":null,"id":1}
                    Result = New ClsJSONAndXMLConverter(BTC_API.GetWalletInfo(), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
                    Result = GetStringBetween(Result, "<scanning>", "</scanning>")
                    Return GetStringBetween(Result, "<progress>", "</progress>")
                Else
                    '{"result":[{"success":true}],"error":null,"id":1}
                    Result = New ClsJSONAndXMLConverter(T_Thread.Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
                    Result = GetStringBetween(Result, "<success>", "</success>")
                    BitcoinNodeRequestList.RemoveAt(i)
                    Return Result
                End If

                Exit For
            End If

        Next

        If Not found Then

            Dim CallThread As S_ThreadedMethod = New S_ThreadedMethod
            CallThread.APICALL = ClsBitcoinAPI.BTC_API_CALLS.importdescriptors
            CallThread.CallThread = New Threading.Thread(AddressOf SubImportDescriptor)
            BitcoinNodeRequestList.Add(CallThread)
            BitcoinNodeRequestList(BitcoinNodeRequestList.Count - 1).CallThread.Start({Address, FromTimestamp, (BitcoinNodeRequestList.Count - 1).ToString()})

        End If

        '{"result":{"walletname":"TESTWALLET","walletversion":169900,"format":"sqlite","balance":0.00012628,"unconfirmed_balance":0.00000000,"immature_balance":0.00000000,"txcount":3,"keypoolsize":0,"keypoolsize_hd_internal":0,"paytxfee":0.00000000,"private_keys_enabled":false,"avoid_reuse":false,"scanning":false,"descriptors":true,"external_signer":false},"error":null,"id":1}
        Result = New ClsJSONAndXMLConverter(BTC_API.GetWalletInfo(), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        Result = GetStringBetween(Result, "<scanning>", "</scanning>")
        Return GetStringBetween(Result, "<progress>", "</progress>")

    End Function

    Private Sub SubImportDescriptor(ByVal Paras As Object)

        Dim ParaList As List(Of String) = New List(Of String)(DirectCast(Paras, String()))
        Dim ThreadIdx As Integer = CInt(ParaList(ParaList.Count - 1))
        Dim SubThread As S_ThreadedMethod = BitcoinNodeRequestList(ThreadIdx)
        SubThread.Result = BTC_API.ImportDescriptors(ParaList(0), ParaList(1))
        BitcoinNodeRequestList(ThreadIdx) = SubThread

    End Sub

    Public Function GetBalance() As String

        Dim Result As String = BTC_API.GetBalance()
        Return Result

    End Function

    Public Function GetBalances() As String

        Dim Result As String = BTC_API.GetBalances()
        Return Result

    End Function

    Public Function GetDescriptorInfo(ByVal Descriptor As String) As String

        Dim Result As String = BTC_API.GetDescriptorInfo(Descriptor)
        Return Result

    End Function

    Public Function GetRawTransaction(ByVal TransactionID As String) As String

        Dim ResultJSON As String = BTC_API.GetRawTransaction(TransactionID)
        Return ResultJSON

    End Function

    Public Function GetTransaction(ByVal TransactionID As String) As String

        Dim ResultJSON As String = BTC_API.GetTransaction(TransactionID)
        Return ResultJSON

    End Function

    Public Function ListUnspentRaw(ByVal Address As String) As String

        Dim ResultJSON As String = BTC_API.ListUnspent(Address)
        Return ResultJSON

    End Function

    Public Function GetReceivedByAddress(ByVal Address As String) As String

        Dim ResultJSON As String = BTC_API.GetReceivedByAddress(Address)
        Return ResultJSON

    End Function

    Public Function ScanBlocks(ByVal Address As String) As String

        Dim ResultJSON As String = BTC_API.ScanBlocks(Address)
        Return ResultJSON

    End Function

#End Region

#Region "Get"

    Public Function GetUnspent(Optional ByVal Address As String = "") As List(Of S_UnspentTransactionOutput)

        Dim Result As String = BTC_API.ListUnspent(Address)

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON)

        Dim XML_Vouts As List(Of KeyValuePair(Of String, Object)) = Converter.Search(Of List(Of KeyValuePair(Of String, Object)))("result")

        If XML_Vouts.Count > 0 Then
            If XML_Vouts(0).Value.GetType = GetType(List(Of KeyValuePair(Of String, Object))) Then
                Dim KeyVals As List(Of KeyValuePair(Of String, Object)) = DirectCast(XML_Vouts(0).Value, List(Of KeyValuePair(Of String, Object)))
                XML_Vouts = KeyVals
            Else
                Dim KeyVals As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))
                KeyVals.Add(New KeyValuePair(Of String, Object)("result", XML_Vouts))
                XML_Vouts = KeyVals
            End If
        End If

        Dim T_PrevTXList As List(Of S_UnspentTransactionOutput) = New List(Of S_UnspentTransactionOutput)

        For Each Entry As KeyValuePair(Of String, Object) In XML_Vouts

            Converter = New ClsJSONAndXMLConverter(Entry)

            Dim T_PrevTX As S_UnspentTransactionOutput = New S_UnspentTransactionOutput("")

            T_PrevTX.TransactionID = Converter.FirstValue("txid").ToString()
            T_PrevTX.VoutIDX = Converter.GetFirstInteger("vout")
            T_PrevTX.Addresses.Add(Converter.FirstValue("address").ToString())
            T_PrevTX.LockingScript = ClsTransaction.ConvertLockingScriptStrToList(Converter.FirstValue("scriptPubKey").ToString())

            Dim T_RIPE160List As List(Of String) = GetRIPE160FromScript(T_PrevTX.LockingScript)

            If T_RIPE160List.Count > 0 Then
                For Each Ripe160 As String In T_RIPE160List
                    Dim T_Address As String = RIPE160ToAddress(Ripe160, BitcoinAddressPrefix)

                    If Not T_PrevTX.Addresses.Contains(T_Address) Then
                        T_PrevTX.Addresses.Add(T_Address)
                    End If

                Next
            End If
            T_PrevTX.Typ = ClsTransaction.GetScriptType(T_PrevTX.LockingScript)

            T_PrevTX.AmountNQT = ClsSignumAPI.Dbl2Planck(Converter.GetFirstDouble("amount"))
            T_PrevTX.Confirmations = Converter.GetFirstInteger("confirmations")

            T_PrevTXList.Add(T_PrevTX)

        Next

        Return T_PrevTXList

    End Function

    Public Function GetMiningInfo() As String
        Dim MiningInfo As String = BTC_API.GetMiningInfo()

        If Not IsErrorOrWarning(MiningInfo, "", False, False) Then
            Return New ClsJSONAndXMLConverter(MiningInfo, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        Else
            Return MiningInfo
        End If

    End Function

    Public Function GetWalletInfo() As String
        Dim WalletInfo As String = BTC_API.GetWalletInfo()

        If Not IsErrorOrWarning(WalletInfo, "", False, False) Then
            Return New ClsJSONAndXMLConverter(WalletInfo, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        Else
            Return WalletInfo
        End If

    End Function

    Public Function GetFee(Optional ByVal Blocks As Integer = 1) As String

        Dim Fee As String = BTC_API.GetFee(Blocks)

        If Not IsErrorOrWarning(Fee, "", False, False) Then
            Return New ClsJSONAndXMLConverter(Fee, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        Else
            Return Fee
        End If

    End Function

    Public Function AbortReScan() As String

        Dim AbortReScanResult As String = BTC_API.AbortRescan()

        If Not IsErrorOrWarning(AbortReScanResult, Application.ProductName + "-error in AbortReScan(): -> ", True) Then
            Return New ClsJSONAndXMLConverter(AbortReScanResult, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        Else
            Return AbortReScanResult
        End If

    End Function

    'Public Function GetTransaction(ByVal TX As String) As String

    '    If TX.Trim = "" Or Not MessageIsHEXString(TX) Then
    '        Return ""
    '    End If

    '    Dim Transaction As String = BTC_API.GetTransaction(TX)

    '    If Not IsErrorOrWarning(Transaction, "", False, False) Then
    '        Return New ClsJSONAndXMLConverter(Transaction, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
    '    Else
    '        Return Transaction
    '    End If

    'End Function

    Public Function GetTXOut(ByVal TX As String, ByVal VOut As Integer) As String

        If TX.Trim = "" Or Not MessageIsHEXString(TX) Or VOut < 0 Then
            Return ""
        End If

        Return BTC_API.GetTXOut(TX, VOut)

    End Function

    Public Function ScanTXOUTSet(Optional ByVal Address As String = "") As String

        Dim found As Boolean = False
        For i As Integer = 0 To BitcoinNodeRequestList.Count - 1
            Dim T_Thread As S_ThreadedMethod = BitcoinNodeRequestList(i)
            If T_Thread.APICALL = ClsBitcoinAPI.BTC_API_CALLS.scantxoutset Then
                found = True

                If T_Thread.CallThread.ThreadState = Threading.ThreadState.Running Then
                    Return BTC_API.ScanTXOUTSet(Address, "status")
                Else
                    Dim T_Result As String = T_Thread.Result
                    BitcoinNodeRequestList.RemoveAt(i)
                    Return T_Result
                End If

                Exit For
            End If

        Next

        If Not found Then

            Dim CallThread As S_ThreadedMethod = New S_ThreadedMethod
            CallThread.APICALL = ClsBitcoinAPI.BTC_API_CALLS.scantxoutset
            CallThread.CallThread = New Threading.Thread(AddressOf SubScanTXOUTSet)
            BitcoinNodeRequestList.Add(CallThread)
            BitcoinNodeRequestList(BitcoinNodeRequestList.Count - 1).CallThread.Start({Address, "start", (BitcoinNodeRequestList.Count - 1).ToString()})

        End If

        Return BTC_API.ScanTXOUTSet(Address, "status")

    End Function

    Private Sub SubScanTXOUTSet(ByVal Paras As Object)

        Dim ParaList As List(Of String) = New List(Of String)(DirectCast(Paras, String()))
        Dim ThreadIdx As Integer = CInt(ParaList(ParaList.Count - 1))
        Dim SubThread As S_ThreadedMethod = BitcoinNodeRequestList(ThreadIdx)
        SubThread.Result = BTC_API.ScanTXOUTSet(ParaList(0), ParaList(1))
        BitcoinNodeRequestList(ThreadIdx) = SubThread

    End Sub

    Public Function GetTXDetails(ByVal TXID As String) As List(Of String)

        If TXID.Trim = "" Or Not MessageIsHEXString(TXID) Then
            Return New List(Of String)
        End If

        Dim ResultJSON As String = BTC_API.GetRawTransaction(TXID)

        If Not IsErrorOrWarning(ResultJSON, "", False, False) Then
            Dim RAWTransactionKeyValueList As List(Of KeyValuePair(Of String, Object)) = New ClsJSONAndXMLConverter(ResultJSON, ClsJSONAndXMLConverter.E_ParseType.JSON).ListOfKeyValues

            If RAWTransactionKeyValueList.Any(Function(C) C.Key = "error" And C.Value.ToString() <> "null") Then
                'error
                Return New List(Of String)
            Else
                Dim Result As KeyValuePair(Of String, Object) = RAWTransactionKeyValueList.FirstOrDefault(Function(C) C.Key = "result")
                ResultJSON = BTC_API.DecodeRawTransaction(Result.Value.ToString())

                Dim TransactionKeyValueList As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(ResultJSON, ClsJSONAndXMLConverter.E_ParseType.JSON)
                Dim Vout As KeyValuePair(Of String, Object) = TransactionKeyValueList.GetFromPath("result/vout")
                Dim Subs As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Vout)

                Dim cnter As Integer = 0
                Dim XMLEntry As String = Subs.Search(cnter.ToString(), ClsJSONAndXMLConverter.E_ParseType.XML)
                Dim XMLList As List(Of String) = New List(Of String)

                While Not XMLEntry.Trim() = ""
                    XMLList.Add(XMLEntry)

                    cnter += 1
                    XMLEntry = Subs.Search(cnter.ToString(), ClsJSONAndXMLConverter.E_ParseType.XML)
                End While

                Return XMLList

            End If

        Else
            Return New List(Of String)
        End If

    End Function


    Public Function GetFilteredTransactions(ByVal Address As String, ByVal Amount As Double) As List(Of S_UnspentTransactionOutput)

        Dim AmountNQT As ULong = Dbl2Satoshi(Amount * 1.1)

        Dim FilteredTXOs As List(Of S_UnspentTransactionOutput) = New List(Of S_UnspentTransactionOutput)

        Dim UTOs As List(Of S_UnspentTransactionOutput) = GetUnspent(Address)

        If UTOs.Count > 0 Then
            UTOs = UTOs.OrderBy(Function(x) x.AmountNQT).ToList
        End If

        For Each UTO As S_UnspentTransactionOutput In UTOs

            If UTO.Addresses.Contains(Address) Or Address.Trim = "" Then
                If AmountNQT > 0 Then
                    FilteredTXOs.Add(UTO)

                    If UTO.AmountNQT > AmountNQT Then
                        AmountNQT = 0UL
                    Else
                        AmountNQT -= UTO.AmountNQT
                    End If

                Else
                    Exit For
                End If
            End If

        Next

        If AmountNQT > 0 Then
            Return New List(Of S_UnspentTransactionOutput)
        Else
            Return FilteredTXOs
        End If

    End Function

    'Function GetRIPE160FromScript(ByVal Script As String) As List(Of String)

    '    Dim T_RIPE160List As List(Of String) = New List(Of String)

    '    Dim RIPE160 As String = GetXFromScript(Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient)

    '    If Not RIPE160.Trim = "" Then
    '        T_RIPE160List.Add(RIPE160)
    '    End If

    '    RIPE160 = GetXFromScript(Script, ClsScriptEntry.E_OP_Code.RIPE160Sender)

    '    If Not RIPE160.Trim = "" Then
    '        T_RIPE160List.Add(RIPE160)
    '    End If

    '    Return T_RIPE160List

    'End Function

    Function GetRIPE160FromScript(ByVal ScriptList As List(Of ClsScriptEntry)) As List(Of String)

        Dim T_RIPE160List As List(Of String) = New List(Of String)

        Dim RIPE160 As String = GetXFromScript(ScriptList, ClsScriptEntry.E_OP_Code.RIPE160Recipient)

        If Not RIPE160.Trim = "" Then
            T_RIPE160List.Add(RIPE160)
        End If

        RIPE160 = GetXFromScript(ScriptList, ClsScriptEntry.E_OP_Code.RIPE160Sender)

        If Not RIPE160.Trim = "" Then
            T_RIPE160List.Add(RIPE160)
        End If

        Return T_RIPE160List

    End Function

    Public Shared Function GetXFromScript(ByVal Script As String, Optional ByVal OP_Code As ClsScriptEntry.E_OP_Code = ClsScriptEntry.E_OP_Code.OP_HASH160) As String
        Dim T_ScriptList As List(Of ClsScriptEntry) = ClsTransaction.ConvertLockingScriptStrToList(Script)
        Return GetXFromScript(T_ScriptList, OP_Code)
    End Function

    Public Shared Function GetXFromScript(ByVal ScriptList As List(Of ClsScriptEntry), Optional ByVal OP_Code As ClsScriptEntry.E_OP_Code = ClsScriptEntry.E_OP_Code.OP_HASH160) As String

        For i As Integer = 0 To ScriptList.Count - 1
            Dim OP_SE As ClsScriptEntry = ScriptList(i)

            If OP_Code = ClsScriptEntry.E_OP_Code.LockTime Then

                Dim KeyCode As Integer = OP_SE.Key

                Select Case KeyCode
                    Case 82 To 96 ' ClsScriptEntry.E_OP_Code.OP_2 to ClsScriptEntry.E_OP_Code.OP_16, ClsScriptEntry.E_OP_Code.LockTime
                        Return IntToHex(KeyCode - 80, 1, False)
                    Case -8
                        Return OP_SE.ValueHex
                End Select

            Else
                If OP_SE.Key = OP_Code Then
                    Return OP_SE.ValueHex
                End If
            End If

        Next

        Return ""

    End Function

#End Region 'Get


#Region "Send"

    Public Function SendRawTX(ByVal RawTX As String) As String

        If RawTX.Trim = "" Or Not MessageIsHEXString(RawTX) Then
            Return ""
        End If

        Return New ClsJSONAndXMLConverter(BTC_API.SendRawTransaction(RawTX), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

    End Function

#End Region 'Send


#Region "Convert/Encode/Decode"

    'Public Function DecodeRawTX(ByVal RawTX As String) As String
    '    Return BTC_API.DecodeRawTransaction(RawTX)
    'End Function
    'Public Function DecodeScript(ByVal Script As String) As String
    '    Return BTC_API.DecodeScript(Script)
    'End Function

    'Private Function ConvertJSONToXML(ByVal Input As String, Optional ByVal SearchString As String = "result") As String

    '    Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Input, ClsJSONAndXMLConverter.E_ParseType.JSON)

    '    'Dim JSON As ClsJSON = New ClsJSON
    '    Dim JSONList As Object = Converter.FirstValue("input").ToString() ' JSON.JSONRecursive(Input)

    '    'Dim Obj As Object = JSON.RecursiveListSearch(JSONList, SearchString)

    '    'If Obj.GetType.Name = GetType(String).Name Then
    '    '    Dim Obj2 As Object = JSON.RecursiveListSearch(JSONList, "error")

    '    '    If Obj2.GetType.Name = GetType(String).Name Then
    '    '        'error is null
    '    '        Input = Obj.ToString
    '    '    Else

    '    '        Dim Obj2List As List(Of Object) = DirectCast(Obj2, List(Of Object))

    '    '        Input = JSON.JSONListToXMLRecursive(Obj2List)

    '    '        If Input.Contains("<message>") Then
    '    '            Input = "<error>" + GetStringBetween(Input, "<message>", "</message>") + "</error>"
    '    '        Else
    '    '            Input = "<error>" + Input + "</error>"
    '    '        End If

    '    '    End If

    '    'ElseIf Obj.GetType.Name = GetType(Boolean).Name Then
    '    '    Return "<error>" + Input + "</error>"
    '    'Else
    '    '    Dim ObjList As List(Of Object) = DirectCast(Obj, List(Of Object))
    '    '    Input = JSON.JSONListToXMLRecursive(ObjList)
    '    'End If

    '    Return Input

    'End Function

#End Region 'Convert/Encode/Decode

    'Public Function GetScriptTypesFromTransaction(ByVal TX As String) As List(Of E_ScriptType)

    '    Dim ScriptTypes As List(Of E_ScriptType) = New List(Of E_ScriptType)
    '    Dim Scripts As List(Of S_UnspentTransactionOutput) = GetScriptsFromUTXO(TX)

    '    For Each Script As S_UnspentTransactionOutput In Scripts
    '        ScriptTypes.Add(GetScriptType(Script.LockingScript))
    '    Next

    '    Return ScriptTypes

    'End Function
    'Public Function GetScriptTypesFromTransaction(ByVal TX As String, ByVal RIPE160 As String) As List(Of E_ScriptType)

    '    Dim ScriptTypes As List(Of E_ScriptType) = New List(Of E_ScriptType)
    '    Dim Scripts As List(Of S_UnspentTransactionOutput) = GetScriptsFromUTXO(TX)

    '    For Each Script As S_UnspentTransactionOutput In Scripts
    '        For Each ScriptEntry As Object In Script.LockingScript

    '            If ScriptEntry.ToString = RIPE160 Then
    '                ScriptTypes.Add(GetScriptType(Script.LockingScript))
    '                Exit For
    '            End If

    '        Next
    '    Next

    '    Return ScriptTypes

    'End Function

End Class