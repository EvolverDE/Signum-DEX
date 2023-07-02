
Option Strict On
Option Explicit On

Imports Microsoft.VisualBasic.Devices
''' <summary>
''' this class is for data preparation from BTC_API
''' </summary>
Public Class ClsBitcoinNET
    Private Property BTC_API As ClsBitcoinAPI

    Private Property C_Transaction As ClsTransaction

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

    Enum E_BitcoinConfigEntry
        BITCOINAPINODE = 0
        BITCOINAPIWALLET = 1
        BITCOINAPIUSER = 2
        BITCOINAPIPASSWORD = 3
        BITCOINDEXADDRESS = 4
        BITCOINMNEMONIC = 5

    End Enum

    Private Function GetBitcoinConfig(ByVal ConfigEntry As E_BitcoinConfigEntry, Optional ByVal DefaultValue As String = "") As String
        Return INIGetValue(BitcoinConfigFile, BitcoinConfigFileSection, ConfigEntry.ToString, DefaultValue)
    End Function

    Private Sub SetBitcoinConfig(ByVal ConfigEntry As E_BitcoinConfigEntry, ByVal Value As String)
        INISetValue(BitcoinConfigFile, BitcoinConfigFileSection, ConfigEntry.ToString, Value)
    End Sub

    Sub New()

        Dim API_URL As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINAPINODE, "http://127.0.0.1:18332")
        Dim API_Wallet As String = GetBitcoinConfig(E_BitcoinConfigEntry.BITCOINAPIWALLET, "DEXWALLET")

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

        If IsErrorOrWarning(Result) Then
            Return False
        End If

        Return True

    End Function

    Public Function LoadWallet(ByVal WalletName As String) As Boolean

        If WalletName.Trim = "" Then
            Return False
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Dim Result As String = New ClsJSONAndXMLConverter(BTC_API.LoadWallet(WalletName), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        '<name>DEXWALLET</name><warning></warning>
        '<error><code>-35</code><0>message</0><1>Wallet file verification failed. Refusing to load database. Data file 'C</1><2>\\Coinz\\bitcoin_testnet\\testnet3\\wallets\\DEXWALLET\\wallet.dat' is already loaded.</2></error>

        If IsErrorOrWarning(Result) Then
            Return False
        End If

        Return True

    End Function

    Public Function UnloadWallet(ByVal WalletName As String) As Boolean

        If WalletName.Trim = "" Then
            Return False
        End If

        '{"result":{"name":"DEXWALLET","warning":""},"Error":null,"id":1}
        Dim Result As String = New ClsJSONAndXMLConverter(BTC_API.UnloadWallet(WalletName), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
        '<name>DEXWALLET</name><warning></warning>

        If IsErrorOrWarning(Result) Then
            Return False
        End If

        Return True

    End Function


    Public Function CreateNewAddress(ByVal Address As String) As Boolean

        Dim Result As String = ""

        Dim LambdaThread As New Threading.Thread(
            Sub()
                Result = BTC_API.ImportAddress(Address, "", False)
            End Sub
        )
        LambdaThread.Start()

        While LambdaThread.IsAlive
            Application.DoEvents()
        End While

        '{"result":null, "error": null, "id": 1}
        Result = New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

        If IsErrorOrWarning(Result) Then
            Return False
        End If

        Return True

    End Function

    Public Function ImportAddress(ByVal Address As String) As Boolean

        Dim Result As String = BTC_API.ImportAddress(Address, "", True)

        'Dim LambdaThread As New Threading.Thread(
        '    Sub()
        '        Result = BTC_API.ImportAddress(Address, "", True)
        '    End Sub
        ')
        'LambdaThread.Start()

        'While LambdaThread.IsAlive
        '    Application.DoEvents()
        'End While

        '{"result":null, "error": null, "id": 1}
        '{"result":null,"error":{"code":-4,"message":"Wallet is currently rescanning. Abort existing rescan or wait."},"id":1}
        Result = New ClsJSONAndXMLConverter(Result, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

        If IsErrorOrWarning(Result) Then
            Return False
        End If

        Return True

    End Function


#End Region

#Region "Get"

    Public Function GetUnspent(Optional ByVal Address As String = "") As List(Of S_UnspentTransactionOutput)

        Dim XML_Vouts As List(Of KeyValuePair(Of String, Object)) = BTC_API.ListUnspent(Address)

        Dim T_PrevTXList As List(Of S_UnspentTransactionOutput) = New List(Of S_UnspentTransactionOutput)

        For Each Entry As KeyValuePair(Of String, Object) In XML_Vouts

            Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Entry)

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

#Region "deprecaded"
        'Dim MaxIDX As Integer = -1

        'For Each Entry As KeyValuePair(Of String, Object) In XML_Vouts


        '    If Entry.Key = "vout" Then

        '        Dim IDX As Integer = GetIntegerBetween(Entry, "<", ">")

        '        If IDX > MaxIDX Then
        '            MaxIDX = IDX
        '        End If

        '    End If



        'Next

        'If MaxIDX <> -1 Then

        '    'For i As Integer = 0 To MaxIDX

        '    '    Dim T_PrevTX As S_UnspentTransactionOutput = New S_UnspentTransactionOutput("")

        '    '    Dim found As Boolean = False
        '    '    For Each T_XML As String In XML_Vouts

        '    '        Dim T_IDX As Integer = GetIntegerBetween(T_XML, "<", ">")

        '    '        If T_IDX = i Then

        '    '            Dim T_SubXML As String = GetStringBetween(T_XML, "<" + i.ToString + ">", "</" + i.ToString + ">")
        '    '            Dim T_Tag As String = GetStringBetween(T_SubXML, "<", ">")
        '    '            Dim T_Val As String = GetStringBetween(T_SubXML, "<" + T_Tag + ">", "</" + T_Tag + ">")

        '    '            Select Case T_Tag
        '    '                Case "txid"
        '    '                    found = True
        '    '                    T_PrevTX.TransactionID = T_Val
        '    '                Case "vout"
        '    '                    found = True
        '    '                    T_PrevTX.VoutIDX = Convert.ToInt32(T_Val)
        '    '                Case "address"
        '    '                    T_PrevTX.Addresses.Add(T_Val)
        '    '                Case "label"
        '    '                Case "scriptPubKey"
        '    '                    found = True
        '    '                    T_PrevTX.LockingScript = ClsTransaction.ConvertLockingScriptStrToList(T_Val)

        '    '                    Dim T_RIPE160List As List(Of String) = GetRIPE160FromScript(T_PrevTX.LockingScript)

        '    '                    If T_RIPE160List.Count > 0 Then
        '    '                        For Each Ripe160 As String In T_RIPE160List
        '    '                            Dim T_Address As String = RIPE160ToAddress(Ripe160, BitcoinAddressPrefix)

        '    '                            If Not T_PrevTX.Addresses.Contains(T_Address) Then
        '    '                                T_PrevTX.Addresses.Add(T_Address)
        '    '                            End If

        '    '                        Next
        '    '                    End If

        '    '                    T_PrevTX.Typ = ClsTransaction.GetScriptType(T_PrevTX.LockingScript)
        '    '                Case "amount"
        '    '                    found = True
        '    '                    T_PrevTX.AmountNQT = Convert.ToUInt64(T_Val.Replace(".", "").Replace(",", ""))
        '    '                Case "confirmations"
        '    '                    found = True
        '    '                    T_PrevTX.Confirmations = Convert.ToInt32(T_Val.Replace(".", "").Replace(",", ""))
        '    '                Case "spendable"
        '    '                Case "solvable"
        '    '                Case "safe"
        '    '                Case Else

        '    '            End Select

        '    '        End If

        '    '    Next

        '    '    If found Then
        '    '        T_PrevTXList.Add(T_PrevTX)
        '    '    End If

        '    'Next

        'Else

        '    Dim T_PrevTX As S_UnspentTransactionOutput = New S_UnspentTransactionOutput("")

        '    'For Each T_XML As String In XML_Vouts

        '    '    Dim T_IDX As Integer = GetIntegerBetween(T_XML, "<", ">")

        '    '    Dim T_SubXML As String = GetStringBetween(T_XML, "<-1>", "</-1>")
        '    '    Dim T_Tag As String = GetStringBetween(T_SubXML, "<", ">")
        '    '    Dim T_Val As String = GetStringBetween(T_SubXML, "<" + T_Tag + ">", "</" + T_Tag + ">")


        '    '    Select Case T_Tag
        '    '        Case "txid"
        '    '            T_PrevTX.TransactionID = T_Val
        '    '        Case "vout"
        '    '            T_PrevTX.VoutIDX = Convert.ToInt32(T_Val)
        '    '        Case "address"
        '    '            T_PrevTX.Addresses.Add(T_Val)
        '    '        Case "label"
        '    '        Case "scriptPubKey"
        '    '            T_PrevTX.LockingScript = ClsTransaction.ConvertLockingScriptStrToList(T_Val)

        '    '            Dim T_RIPE160List As List(Of String) = GetRIPE160FromScript(T_PrevTX.LockingScript)

        '    '            If T_RIPE160List.Count > 0 Then
        '    '                For Each Ripe160 As String In T_RIPE160List
        '    '                    Dim T_Address As String = RIPE160ToAddress(Ripe160, BitcoinAddressPrefix)

        '    '                    If Not T_PrevTX.Addresses.Contains(T_Address) Then
        '    '                        T_PrevTX.Addresses.Add(T_Address)
        '    '                    End If

        '    '                Next
        '    '            End If

        '    '            T_PrevTX.Typ = ClsTransaction.GetScriptType(T_PrevTX.LockingScript)
        '    '        Case "amount"
        '    '            T_PrevTX.AmountNQT = Convert.ToUInt64(T_Val.Replace(".", "").Replace(",", ""))
        '    '        Case "confirmations"
        '    '            T_PrevTX.Confirmations = Convert.ToInt32(T_Val.Replace(".", "").Replace(",", ""))
        '    '        Case "spendable"
        '    '        Case "solvable"
        '    '        Case "safe"
        '    '        Case Else

        '    '    End Select

        '    'Next

        '    T_PrevTXList.Add(T_PrevTX)

        'End If
#End Region

        Return T_PrevTXList

    End Function

    Public Function GetMiningInfo() As String
        Return New ClsJSONAndXMLConverter(BTC_API.GetMiningInfo(), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
    End Function

    Public Function GetWalletInfo() As String
        Return New ClsJSONAndXMLConverter(BTC_API.GetWalletInfo(), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
    End Function

    Public Function GetFee(Optional ByVal Blocks As Integer = 1) As String
        Return New ClsJSONAndXMLConverter(BTC_API.GetFee(Blocks), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
    End Function

    Public Function AbortReScan() As String
        Return New ClsJSONAndXMLConverter(BTC_API.AbortRescan(), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString
    End Function

    'Public Function GetRawTransaction(ByVal RawTX As String) As String

    '    If RawTX.Trim = "" Or Not MessageIsHEXString(RawTX) Then
    '        Return ""
    '    End If

    '    Return String.Concat(BTC_API.GetRawTransaction(RawTX, "result"))

    'End Function

    Public Function GetTransaction(ByVal TX As String) As String

        If TX.Trim = "" Or Not MessageIsHEXString(TX) Then
            Return ""
        End If

        Return New ClsJSONAndXMLConverter(BTC_API.GetTransaction(TX), ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

    End Function

    Public Function GetTXOut(ByVal TX As String, ByVal VOut As Integer) As String

        If TX.Trim = "" Or Not MessageIsHEXString(TX) Or VOut < 0 Then
            Return ""
        End If

        Return BTC_API.GetTXOut(TX, VOut)

    End Function

    Public Function GetTXDetails(ByVal TXID As String) As List(Of String)

        If TXID.Trim = "" Or Not MessageIsHEXString(TXID) Then
            Return New List(Of String)
        End If

        Dim ResultJSON As String = BTC_API.GetRawTransaction(TXID)
        Dim ResultXMLValue As String = New ClsJSONAndXMLConverter(ResultJSON, ClsJSONAndXMLConverter.E_ParseType.JSON).XMLString

        If Not ResultXMLValue.Contains("<error>") Then
            ResultJSON = BTC_API.DecodeRawTransaction(ResultXMLValue)

            Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(ResultJSON, ClsJSONAndXMLConverter.E_ParseType.JSON)

            'Dim JSON As ClsJSON = New ClsJSON()
            Dim VOut As String = Converter.FirstValue("vout").ToString() ' JSON.GetFromJSON(ResultJSON, "result/vout")
            Return New List(Of String)
        Else
            Return New List(Of String)({ResultXMLValue})
        End If

    End Function


    Public Function GetFilteredTransactions(ByVal Address As String, ByVal Amount As Double) As List(Of S_UnspentTransactionOutput)

        Dim AmountNQT As ULong = Dbl2Satoshi(Amount * 1.1)

        Dim FilteredTXOs As List(Of S_UnspentTransactionOutput) = New List(Of S_UnspentTransactionOutput)

        Dim UTOs As List(Of S_UnspentTransactionOutput) = GetUnspent()

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

    Function GetRIPE160FromScript(ByVal Script As String) As List(Of String)

        Dim T_RIPE160List As List(Of String) = New List(Of String)

        Dim RIPE160 As String = GetXFromScript(Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient)

        If Not RIPE160.Trim = "" Then
            T_RIPE160List.Add(RIPE160)
        End If

        RIPE160 = GetXFromScript(Script, ClsScriptEntry.E_OP_Code.RIPE160Sender)

        If Not RIPE160.Trim = "" Then
            T_RIPE160List.Add(RIPE160)
        End If

        Return T_RIPE160List

    End Function

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

    Public Function DecodeRawTX(ByVal RawTX As String) As String
        Return BTC_API.DecodeRawTransaction(RawTX)
    End Function
    Public Function DecodeScript(ByVal Script As String) As String
        Return BTC_API.DecodeScript(Script)
    End Function

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