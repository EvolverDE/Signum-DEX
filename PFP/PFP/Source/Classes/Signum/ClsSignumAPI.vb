Option Strict On
Option Explicit On

Imports System.ComponentModel.Design
Imports System.IO
Imports System.Net
Imports System.Text
Imports PFP.ClsDEXContract

Public Class ClsSignumAPI

#Region "SmartContract Structure"
    'SmartContract: 14302362561079850525

    'ActivateDeactivateDispute: -9199918549131231789

    'CreateOrderWithResponder: -5335884675757206276
    'CreateOrder: 716726961670769723
    'AcceptOrder: 4714436802908501638
    'InjectResponder: 9213622959462902524

    'OpenDispute: 7510787419861318753
    'MediateDispute: 1115156232660555199
    'Appeal: 7341272028202959329
    'CheckCloseDispute: -5140474353491861087

    'FinishOrder: 3125596792462301675

    'InjectChainSwapHash: 2770910189976301362
    'FinishOrderWithChainSwapKey: -3992805468895771487

#End Region

    Public Const _ReferenceTX As ULong = 14302362561079850525UL
    Public Const _ReferenceTXFullHash As String = "1db602def8327cc6650b90d857c079107d4fc542ad7ad60161e83b15929c441a"
    Public Const _DeployFeeNQT As ULong = 240000000UL
    Public Const _GasFeeNQT As ULong = 50000000UL
    Public Const _AddressPreFix As String = "TS-"
    Public Const _DefaultNode As String = "https://testnet.signum.zone/api"
    Public Const _Nodes As String = _DefaultNode '+ ";" + "http://tordek.ddns.net:6876/api" + ";" + "http://lmsi.club:6876/api"

    'Public ReadOnly Property ReferenceCreateOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(716726961670769723L), 0)
    'Public ReadOnly Property ReferenceAcceptOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(4714436802908501638L), 0)
    'Public ReadOnly Property ReferenceInjectResponder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(9213622959462902524L), 0)
    Public ReadOnly Property ReferenceFinishOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(3125596792462301675L), 0)
    'Public ReadOnly Property ReferenceInjectChainSwapHash As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(2770910189976301362L), 0)
    'Public ReadOnly Property ReferenceFinishOrderWithChainSwapKey As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(-3992805468895771487L), 0)

    ReadOnly Property C_ReferenceMachineCode As String
    ReadOnly Property C_ReferenceMachineCodeHash As String
    ReadOnly Property C_ReferenceMachineCodeHashID As ULong

    Private ReadOnly Property C_CreationMachineData As String = "0000000000000000000000000000000000000000000000000100000000000000020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001f00000000000000"
    Private ReadOnly Property C_ReferenceMachineData As String = C_CreationMachineData + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"

    Property C_Node As String = ""

    Property C_AccountID As ULong
    Property C_Address As String


    Property C_UTXList As List(Of List(Of String)) = New List(Of List(Of String))

    Public Property DEXSmartContractList As List(Of String) = New List(Of String)


    Sub New(Optional ByVal Node As String = "", Optional ByVal Account As ULong = 0UL, Optional ByVal ReferenceTX As ULong = _ReferenceTX)

        If Not Node.Trim = "" Then
            C_Node = Node
        Else
            C_Node = GetINISetting(E_Setting.DefaultNode, _DefaultNode)
        End If

        If Not Account = 0UL Then
            C_AccountID = Account
        End If

        Dim ReferenceTXDetails As List(Of String) = GetTransaction(ReferenceTX)

        Dim ReferenceSmartContractDetails = GetSmartContractDetails(ReferenceTX)
        C_ReferenceMachineCode = GetStringBetweenFromList(ReferenceSmartContractDetails, "<machineCode>", "</machineCode>")
        C_ReferenceMachineCodeHashID = GetULongBetweenFromList(ReferenceSmartContractDetails, "<machineCodeHashId>", "</machineCodeHashId>")
        C_ReferenceMachineData = GetStringBetweenFromList(ReferenceSmartContractDetails, "<creationMachineData>", "</creationMachineData>")

    End Sub


#Region "Blockchain Communication"

#Region "Basic API"

    Function BroadcastTransaction(ByVal TXBytesHexStr As String) As String

        Dim Response As String = SignumRequest("requestType=broadcastTransaction&transactionBytes=" + TXBytesHexStr)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in BroadcastTransaction(): ->" + vbCrLf + Response
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            Return Application.ProductName + "-error in BroadcastTransaction(): " + Response
        End If

        Dim UTX As Object = Converter.FirstValue("transaction") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "transaction")

        Dim Returner As String = Application.ProductName + "-error in BroadcastTransaction(): -> UTX failure"
        If UTX.GetType = GetType(String) Then
            Returner = CStr(UTX)
        ElseIf UTX.GetType = GetType(Boolean) Then

        ElseIf UTX.GetType = GetType(List(Of )) Then

        End If

        Return Returner

    End Function

    Function SignumRequest(ByVal postData As String) As String

        Try

            Dim request As WebRequest = WebRequest.Create(C_Node)
            request.Method = "POST"

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length
            request.Timeout = 5000


            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()


            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            reader.Close()
            dataStream.Close()
            response.Close()

            Return responseFromServer

        Catch ex As Exception
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in SignumRequest(" + C_Node + "): " + ex.Message
            Return Application.ProductName + "-error in SignumRequest(" + C_Node + "): " + ex.Message
        End Try

    End Function

#End Region

#Region "Get"

    Public Function IsSmartContract(ByVal AccountID As String, Optional ByVal UseBuffer As Boolean = False) As Boolean

        If Not UseBuffer Then

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)

            Dim Response As String = SignumRequest("requestType=getAccount&account=" + AccountID)

            If Response.Contains(Application.ProductName + "-error") Then
                'PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsSmartContract(): -> " + Response
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Out.ErrorLog2File(Application.ProductName + "-error in IsSmartContract(): -> " + Response)
                End If

                Return False
            End If

            Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

            'Dim JSON As ClsJSON = New ClsJSON

            'Dim RespList As Object = JSON.JSONRecursive(Response)

            Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

            If Error0 <> -1 Then
                'TX not OK
                'PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsSmartContract(): " + Response
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Out.ErrorLog2File(Application.ProductName + "-error in IsSmartContract(): " + Response)
                End If

                Return False
            End If

            Dim PubKey As String = Converter.FirstValue("publicKey").ToString() ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "publicKey").ToString

            If PubKey = "0000000000000000000000000000000000000000000000000000000000000000" Then
                Return True
            Else
                Return False
            End If

        Else

            For Each DEXSmartContract As String In DEXSmartContractList
                If DEXSmartContract = AccountID Then
                    Return True
                End If
            Next

            Return False

        End If

    End Function

    Private Structure S_TX
        Dim Transaction As ULong
        Dim Type As Integer
        Dim Timestamp As ULong

        Dim DateTimestamp As Date

        Dim Sender As ULong
        Dim SenderRS As String

        Dim AmountNQT As ULong
        Dim FeeNQT As ULong
        Dim Attachment As String

        Dim Recipient As ULong
        Dim RecipientRS As String

        Dim Confirmations As ULong
    End Structure

    Public Function IsValidDEXContract(ByVal ContractID As ULong, ByVal MachineData As String) As Boolean

        If C_ReferenceMachineData = MachineData Or MachineData.Contains(C_ReferenceMachineData) Then
            Return True
        End If

        Dim T_ContractTransactionsPieceList As List(Of List(Of String)) = GetAccountTransactions(ContractID)
        Dim T_ContractTransactionsList As List(Of List(Of String)) = New List(Of List(Of String))
        T_ContractTransactionsList.AddRange(T_ContractTransactionsPieceList.ToArray)

        Dim W500 As Integer = T_ContractTransactionsPieceList.Count
        While W500 >= 500

            T_ContractTransactionsPieceList = GetAccountTransactions(ContractID, 0, Convert.ToUInt64(W500))

            Dim T_W500 As Integer = T_ContractTransactionsPieceList.Count

            If T_W500 >= 500 Then
                W500 += 500
            Else
                W500 = 0
            End If

            T_ContractTransactionsList.AddRange(T_ContractTransactionsPieceList.ToArray)

        End While


        Dim T_TXList As List(Of S_TX) = New List(Of S_TX)

        For Each TX As List(Of String) In T_ContractTransactionsList
            Dim T_TX As S_TX = New S_TX

            T_TX.Transaction = GetULongBetweenFromList(TX, "<transaction>", "</transaction>")
            T_TX.Type = GetIntegerBetweenFromList(TX, "<type>", "</type>")
            T_TX.Timestamp = GetULongBetweenFromList(TX, "<timestamp>", "</timestamp>")

            T_TX.DateTimestamp = ClsSignumAPI.UnixToTime(T_TX.Timestamp.ToString)

            T_TX.Sender = GetULongBetweenFromList(TX, "<sender>", "</sender>")
            T_TX.SenderRS = GetStringBetweenFromList(TX, "<senderRS>", "</senderRS>")

            T_TX.AmountNQT = GetULongBetweenFromList(TX, "<amountNQT>", "</amountNQT>")
            T_TX.FeeNQT = GetULongBetweenFromList(TX, "<feeNQT>", "</feeNQT>")
            T_TX.Attachment = GetStringBetweenFromList(TX, "<attachment>", "</attachment>")

            T_TX.Recipient = GetULongBetweenFromList(TX, "<recipient>", "</recipient>")
            T_TX.RecipientRS = GetStringBetweenFromList(TX, "<recipientRS>", "</recipientRS>")

            T_TX.Confirmations = GetULongBetweenFromList(TX, "<confirmations>", "</confirmations>")

            T_TXList.Add(T_TX)

        Next

        If T_TXList.Count > 0 Then

            T_TXList = T_TXList.OrderBy(Function(T_TX As S_TX) T_TX.DateTimestamp).ToList

            Dim T_LastTX As S_TX = T_TXList(T_TXList.Count - 1)

            'Dim T_ContractOrderHistoryList As List(Of S_Order) = New List(Of S_Order)

            T_TXList = T_TXList.Where(Function(c As S_TX) c.Sender = ContractID).ToList()

            Dim FirstTX As S_TX = T_TXList.FirstOrDefault(Function(c As S_TX) c.Sender = ContractID)

            For Each ContractTX As S_TX In T_TXList

                Dim ReferenceTXIDs As String = GetStringBetween(ContractTX.Attachment, "<message>", "</message>")
                If Not ReferenceTXIDs.Trim = "" Then
                    Dim ReferenceTXIDList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceTXIDs)
                    If Not ReferenceTXIDList(0) = 0UL And ReferenceTXIDList(1) = 0UL And ReferenceTXIDList(2) = 0UL And ReferenceTXIDList(3) = 0UL Then
                        If T_TXList.Where(Function(s As S_TX) s.Transaction = ReferenceTXIDList(0)).Any() Then
                            Return True
                        End If
                    End If
                End If

            Next

            'For i As Integer = 0 To T_TXList.Count - 1

            '    Dim T_TX As S_TX = T_TXList(i)

            '    If T_TX.Sender = ContractID Then

            '        Dim ReferenceTXIDs As String = GetStringBetween(T_TX.Attachment, "<message>", "</message>")
            '        If Not ReferenceTXIDs.Trim = "" Then

            '            Dim ReferenceTXIDList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceTXIDs)

            '            If Not ReferenceTXIDList(0) = 0UL And ReferenceTXIDList(1) = 0UL And ReferenceTXIDList(2) = 0UL And ReferenceTXIDList(3) = 0UL Then

            '                If T_TXList.Where(Function(s As S_TX) s.Transaction = ReferenceTXIDList(0)).Any() Then

            '                End If

            '                For Each R_TX As S_TX In T_TXList

            '                    If R_TX.Transaction = ReferenceTXIDList(0) Then
            '                        Return True
            '                    End If

            '                Next

            '            End If

            '        End If

            '        Exit For

            '    End If

            'Next

        End If

        Return False

    End Function

    'Public Function GetAccountFromPassPhrase() As List(Of String)

    '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)

    '    Dim Signum As ClsSignumNET = New ClsSignumNET(C_PromptPIN)
    '    Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()
    '    Dim Response As String = SignumRequest("requestType=getAccountId&publicKey=" + ByteAry2HEX(MasterkeyList(0)).Trim)

    '    If Response.Contains(Application.ProductName + "-error") Then
    '        'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response
    '        If GetINISetting(E_Setting.InfoOut, False) Then
    '            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response)
    '        End If

    '        Return New List(Of String)
    '    End If

    '    Dim JSON As ClsJSON = New ClsJSON

    '    Dim RespList As Object = JSON.JSONRecursive(Response)


    '    Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
    '    If Error0.GetType.Name = GetType(Boolean).Name Then
    '        'TX OK
    '    ElseIf Error0.GetType.Name = GetType(String).Name Then
    '        'TX not OK
    '        'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response
    '        If GetINISetting(E_Setting.InfoOut, False) Then
    '            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response)
    '        End If

    '        Return New List(Of String)
    '    End If


    '    Dim Account As String = JSON.RecursiveListSearch(RespList, "account").ToString
    '    Dim AccountRS As String = JSON.RecursiveListSearch(RespList, "accountRS").ToString
    '    Dim Balance As List(Of String) = GetBalance(AccountRS)

    '    Return Balance

    'End Function

    Public Function GetAccountPublicKeyFromAccountID_RS(ByVal AccountID_RS As String) As String

        Dim Response As String = SignumRequest("requestType=getAccountPublicKey&account=" + AccountID_RS.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): ->" + vbCrLf + Response
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): " + Response
        End If

        Dim PublicKey As Object = Converter.FirstValue("publicKey") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "publicKey").ToString

        Dim Returner As String = ""
        If PublicKey.GetType.Name = GetType(String).Name Then
            Returner = CStr(PublicKey)
        End If

        Return Returner

    End Function

    ''' <summary>
    ''' Converts the given AccountID to Address (0= account;1= address)
    ''' </summary>
    ''' <param name="AccountID"></param>
    ''' <returns></returns>
    Public Function RSConvert(ByVal AccountID As ULong) As List(Of String)

        Try

            Dim AccountRS As String = ClsReedSolomon.Encode(AccountID)
            Dim x As List(Of String) = New List(Of String)({"<account>" + AccountID.ToString + "</account>", "<accountRS>" + ClsSignumAPI._AddressPreFix + AccountRS + "</accountRS>"})
            Return x

        Catch ex As Exception
            Return New List(Of String)({"<account>" + AccountID.ToString + "</account>", "<accountRS>" + AccountID.ToString + "</accountRS>"})
        End Try

    End Function
    ''' <summary>
    ''' Converts the given Address to AccountID
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <returns></returns>
    Public Function RSConvert(ByVal Address As String) As List(Of String)

        Try

            If Address.Contains("-") Then

                Address = Address.Substring(Address.IndexOf("-") + 1) 'remove (T)S- Tag

                Dim AccountID As ULong = ClsReedSolomon.Decode(Address)
                Dim x As List(Of String) = New List(Of String)({"<account>" + AccountID.ToString + "</account>", "<accountRS>" + Address + "</accountRS>"})
                Return x
            Else
                Dim AccountRS As String = ClsReedSolomon.Encode(ULong.Parse(Address))
                Dim x As List(Of String) = New List(Of String)({"<account>" + Address + "</account>", "<accountRS>" + ClsSignumAPI._AddressPreFix + AccountRS + "</accountRS>"})
                Return x
            End If

        Catch ex As Exception
            Return New List(Of String)({"<account>" + Address + "</account>", "<accountRS>" + Address + "</accountRS>"})
        End Try

    End Function

    ''' <summary>
    ''' Gets the Balance from the given Address (HTML-Tags= coin, account, address, balance, available, pending)
    ''' </summary>
    ''' <param name="Address"></param>
    ''' <returns></returns>
    Public Function GetBalance(ByVal Address As String) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim AccountID As ULong = 0

        Dim ConvAddress As List(Of String) = ConvertAddress(Address)
        If ConvAddress.Count > 0 Then
            AccountID = Convert.ToUInt64(ConvAddress(0))
        End If

        Dim CoinBal As List(Of String) = New List(Of String)({"<coin>SIGNA</coin>", "<account>" + AccountID.ToString + "</account>", "<address>" + Address + "</address>", "<balance>0</balance>", "<available>0</available>", "<pending>0</pending>"})

        If AccountID = 0 Then

            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(Address=" + Address + "): -> AccountID=0")
            End If

            Return CoinBal
        Else
            Return GetBalance(AccountID)
        End If

    End Function
    ''' <summary>
    ''' Gets the Balance from the given AccountID (HTML-Tags= coin, account, address, balance, available, pending)
    ''' </summary>
    ''' <param name="AccountID"></param>
    ''' <returns></returns>
    Public Function GetBalance(ByVal AccountID As ULong) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Address As String = ClsReedSolomon.Encode(AccountID)

        Dim CoinBal As List(Of String) = New List(Of String)({"<coin>SIGNA</coin>", "<account>" + AccountID.ToString + "</account>", "<address>" + ClsSignumAPI._AddressPreFix + Address + "</address>", "<balance>0</balance>", "<available>0</available>", "<pending>0</pending>"})

        Dim Response As String = SignumRequest("requestType=getAccount&account=" + AccountID.ToString)

        If Response.Contains(Application.ProductName + "-error") Then

            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountID=" + AccountID.ToString + "): -> " + Response)
            End If

            Return CoinBal
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK

            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountID=" + AccountID.ToString + "): " + Response)
            End If

            Return CoinBal
        End If

        Dim BalancePlanckStr As String = Converter.FirstValue("balanceNQT").ToString() ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "balanceNQT").ToString
        Dim Balance As Double = 0.0

        Try
            Balance = Double.Parse(BalancePlanckStr.Insert(BalancePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim AvailablePlanckStr As String = Converter.FirstValue("unconfirmedBalanceNQT").ToString() ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "unconfirmedBalanceNQT").ToString
        Dim Available As Double = 0.0

        Try
            Available = Double.Parse(AvailablePlanckStr.Insert(AvailablePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim Pending As Double = Available - Balance

        '(Coin, Account, Address, Balance, Available, Pending)
        CoinBal(0) = "<coin>SIGNA</coin>"
        CoinBal(1) = "<account>" + AccountID.ToString + "</account>"
        CoinBal(2) = "<address>" + Address.Trim + "</address>"
        CoinBal(3) = "<balance>" + Balance.ToString + "</balance>"
        CoinBal(4) = "<available>" + Available.ToString + "</available>"
        CoinBal(5) = "<pending>" + Pending.ToString + "</pending>"

        Return CoinBal

    End Function

    Enum E_Fee
        Cheap = 0
        Standard = 1
        Priority = 2
    End Enum

    Public Function GetFees() As List(Of Double)

        Dim FeeList As List(Of Double) = New List(Of Double)
        Dim FeesXML As String = GetFeesXML()

        FeeList.Add(ClsSignumAPI.Planck2Dbl(GetULongBetween(FeesXML, "<" + ClsSignumAPI.E_Fee.Cheap.ToString().ToLower() + ">", "</" + ClsSignumAPI.E_Fee.Cheap.ToString().ToLower() + ">")))
        FeeList.Add(ClsSignumAPI.Planck2Dbl(GetULongBetween(FeesXML, "<" + ClsSignumAPI.E_Fee.Standard.ToString().ToLower() + ">", "</" + ClsSignumAPI.E_Fee.Standard.ToString().ToLower() + ">")))
        FeeList.Add(ClsSignumAPI.Planck2Dbl(GetULongBetween(FeesXML, "<" + ClsSignumAPI.E_Fee.Priority.ToString().ToLower() + ">", "</" + ClsSignumAPI.E_Fee.Priority.ToString().ToLower() + ">")))

        Return FeeList

    End Function

    Public Function GetFee(ByVal FeePrio As E_Fee) As Double

        Dim Fees As List(Of Double) = GetFees()

        Select Case FeePrio
            Case E_Fee.Cheap
                Return Fees(0)
            Case E_Fee.Standard
                Return Fees(1)
            Case E_Fee.Priority
                Return Fees(2)
        End Select

        Return Fees(1)

    End Function

    Public Function GetFeesXML() As String

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        Dim Response As String = SignumRequest("requestType=suggestFee")

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTXFee(1): -> " + Response)
            End If

            Return "<cheap>1000000</cheap><standard>2000000</standard><priority>3000000</priority>" '<requestProcessingTime>0</requestProcessingTime>

        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)
        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode")

        If Error0 <> -1 Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTXFee(2): " + Response)
            End If

            Return "<cheap>1000000</cheap><standard>2000000</standard><priority>3000000</priority>" '<requestProcessingTime>0</requestProcessingTime>
        End If

        Return Converter.XMLString

    End Function

    Public Function GetTXFee(Optional ByVal Message As String = "", Optional ByVal FeePriority As E_Fee = E_Fee.Standard) As Double

        If Message = "" Then
            Return GetFee(FeePriority)
        Else
            Return CalculateFee(Message)
        End If

    End Function
    Private Function CalculateFee(ByVal Message As String) As Double

        Dim TXFee As Double = 0.01 * (Math.Floor(Message.Length / 176) + 2)

        If TXFee < 0.01 Then
            TXFee = 0.01
        End If

        Return TXFee

    End Function

    Public Function GetUnconfirmedTransactions() As List(Of List(Of String))

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getUnconfirmedTransactions")

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): -> " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim UTXs As List(Of KeyValuePair(Of String, Object)) = Converter.Search(Of List(Of KeyValuePair(Of String, Object)))("unconfirmedTransactions", False)

#Region "old"
        'Dim EntryList As List(Of Object) = New List(Of Object)

        'If UTX.GetType.Name = GetType(String).Name Then
        '    Return New List(Of List(Of String))
        'ElseIf UTX.GetType.Name = GetType(Boolean).Name Then
        '    Return New List(Of List(Of String))
        'ElseIf UTX.GetType.Name = GetType(List(Of Object)).Name Then

        '    Dim TempOBJList As List(Of Object) = New List(Of Object)

        '    For Each T_Entry In DirectCast(UTX, List(Of Object))

        '        Dim Entry As List(Of Object) = New List(Of Object)

        '        If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '            Entry = DirectCast(T_Entry, List(Of Object))
        '        End If

        '        If Entry.Count > 0 Then
        '            If Entry(0).ToString = "type" Then
        '                If TempOBJList.Count > 0 Then
        '                    EntryList.Add(TempOBJList)
        '                End If

        '                TempOBJList = New List(Of Object) From {
        '                    Entry
        '                }
        '            Else
        '                TempOBJList.Add(Entry)
        '            End If

        '        End If

        '    Next

        '    EntryList.Add(TempOBJList)

        'Else
        '    Return New List(Of List(Of String))
        'End If
#End Region

        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each Entry As KeyValuePair(Of String, Object) In UTXs

            Dim SubConverter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Entry)

            Dim TempList As List(Of String) = New List(Of String)

            TempList.Add(SubConverter.Search("type", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("timestamp", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("recipient", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("recipientRS", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("amountNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("feeNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("transaction", ClsJSONAndXMLConverter.E_ParseType.XML))

            Dim Attachment As List(Of KeyValuePair(Of String, Object)) = SubConverter.Search(Of List(Of KeyValuePair(Of String, Object)))("attachment")
            Dim MessageList As List(Of KeyValuePair(Of String, Object)) = Attachment.Where(Function(c) c.Key = "message").ToList()
            Dim Message As KeyValuePair(Of String, Object) = MessageList.FirstOrDefault()
            Dim AttachmentString As String = "<attachment>"

            If Not IsNothing(Message.Key) Then
                Dim IsText As String = Attachment.Where(Function(c) c.Key = "messageIsText").ToList()(0).Value.ToString()
                AttachmentString += "<message>" + Message.Value.ToString() + "</message><isText>" + IsText + "</isText>"
            End If

            Dim EncMessageList As List(Of KeyValuePair(Of String, Object)) = Attachment.Where(Function(c) c.Key = "encryptedMessage").ToList()
            Dim EncMessage As KeyValuePair(Of String, Object) = EncMessageList.FirstOrDefault()

            If Not IsNothing(EncMessage.Key) Then ' EncMessage.GetType.Name = GetType(Boolean).Name Then

                Dim Data As String = New ClsJSONAndXMLConverter(EncMessage).Search("data", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "data"))
                Dim Nonce As String = New ClsJSONAndXMLConverter(EncMessage).Search("nonce", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "nonce"))
                Dim IsText As String = New ClsJSONAndXMLConverter(EncMessage).Search("isText", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "isText"))

                If Not Data.Trim() = "" And Not Nonce.Trim() = "" Then
                    AttachmentString += Data + Nonce + IsText ' "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                End If

            End If

            AttachmentString += "</attachment>"

            TempList.Add(AttachmentString)

            TempList.Add(SubConverter.Search("sender", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("senderRS", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("confirmations", ClsJSONAndXMLConverter.E_ParseType.XML))

            ReturnList.Add(TempList)

        Next

#Region "deprecaded"

        'For Each T_Entry In EntryList

        '    Dim Entry As List(Of Object) = New List(Of Object)

        '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '        Entry = DirectCast(T_Entry, List(Of Object))
        '    End If

        '    Dim TempList As List(Of String) = New List(Of String)

        '    For Each T_SubEntry In Entry

        '        Dim SubEntry As List(Of Object) = New List(Of Object)
        '        If T_SubEntry.GetType.Name = GetType(List(Of Object)).Name Then
        '            SubEntry = DirectCast(T_SubEntry, List(Of Object))
        '        End If

        '        If SubEntry.Count > 0 Then

        '            Select Case SubEntry(0).ToString
        '                Case "type"

        '                Case "subtype"

        '                Case "timestamp"
        '                    TempList.Add("<timestamp>" + SubEntry(1).ToString + "</timestamp>")

        '                Case "deadline"

        '                Case "senderPublicKey"

        '                Case "amountNQT"
        '                    TempList.Add("<amountNQT>" + SubEntry(1).ToString + "</amountNQT>")

        '                Case "feeNQT"
        '                    TempList.Add("<feeNQT>" + SubEntry(1).ToString + "</feeNQT>")

        '                Case "signature"

        '                Case "signatureHash"

        '        'Case "balanceNQT"
        '        '    UTXDetailList.Add("<balanceNQT>" + Entry(1) + "</balanceNQT>")

        '                Case "fullHash"

        '                Case "transaction"
        '                    TempList.Add("<transaction>" + SubEntry(1).ToString + "</transaction>")

        '                Case "attachment"

        '                    Dim TMsg As String = "<attachment>"


        '                    Dim SubSubEntry As List(Of Object) = New List(Of Object)

        '                    If SubEntry(1).GetType.Name = GetType(List(Of Object)).Name Then
        '                        SubSubEntry = DirectCast(SubEntry(1), List(Of Object))
        '                    End If

        '                    If SubSubEntry.Count > 0 Then

        '                        Dim Message As String = Converter.FirstValue("message").ToString() ' JSON.RecursiveListSearch(SubSubEntry, "message").ToString

        '                        If Message.Trim <> "False" Then
        '                            Dim IsText As String = Converter.FirstValue("messageIsText").ToString() ' JSON.RecursiveListSearch(SubSubEntry, "messageIsText").ToString
        '                            TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
        '                        End If

        '                        Dim EncMessage As Object = Converter.FirstValue("encryptedMessage") ' JSON.RecursiveListSearch(SubSubEntry, "encryptedMessage")

        '                        'If EncMessage.GetType.Name = GetType(Boolean).Name Then

        '                        'Else
        '                        If EncMessage.GetType.Name = GetType(List(Of Object)).Name Then

        '                            Dim EncryptedMessageList As List(Of Object) = New List(Of Object)
        '                            If EncMessage.GetType.Name = GetType(List(Of Object)).Name Then
        '                                EncryptedMessageList = DirectCast(EncMessage, List(Of Object))
        '                            End If

        '                            Dim Data As String = Convert.ToString(Converter.FirstValue("data")) 'JSON.RecursiveListSearch(EncryptedMessageList, "data"))
        '                            Dim Nonce As String = Convert.ToString(Converter.FirstValue("nonce")) 'JSON.RecursiveListSearch(EncryptedMessageList, "nonce"))
        '                            Dim IsText As String = Converter.FirstValue("isText").ToString() ' JSON.RecursiveListSearch(SubSubEntry, "isText").ToString

        '                            If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
        '                                TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
        '                            End If
        '                        Else

        '                        End If

        '                    End If

        '                    TMsg += "</attachment>"

        '                    TempList.Add(TMsg)

        '                Case "sender"
        '                    TempList.Add("<sender>" + SubEntry(1).ToString + "</sender>")

        '                Case "senderRS"
        '                    TempList.Add("<senderRS>" + SubEntry(1).ToString + "</senderRS>")

        '                Case "recipient"
        '                    TempList.Add("<recipient>" + SubEntry(1).ToString + "</recipient>")

        '                Case "recipientRS"
        '                    TempList.Add("<recipientRS>" + SubEntry(1).ToString + "</recipientRS>")

        '                Case "height"
        '                    TempList.Add("<height>" + SubEntry(1).ToString + "</height>")

        '                Case "version"

        '                Case "ecBlockId"

        '                Case "ecBlockHeight"

        '                Case "block"
        '                    TempList.Add("<block>" + SubEntry(1).ToString + "</block>")

        '                Case "confirmations"
        '                    TempList.Add("<confirmations>" + SubEntry(1).ToString + "</confirmations>")

        '                Case "blockTimestamp"

        '                Case "requestProcessingTime"

        '            End Select

        '        End If

        '    Next

        '    ReturnList.Add(TempList)

        'Next

#End Region

        C_UTXList.Clear()
        C_UTXList.AddRange(ReturnList.ToArray)

        Return ReturnList

    End Function

    Public Function GetCurrentBlock() As Integer

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim BlockHeightInt As Integer = 0

        Dim Response As String = SignumRequest("requestType=getMiningInfo")

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): -> " + Response)
            End If
            Return 0
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): " + Response)
            End If
            Return 0
        End If

        'Dim BlockHeightStr As Object = Converter.FirstValue("height") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "height")

        Try
            BlockHeightInt = Convert.ToInt32(Converter.FirstValue("height"))
        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): -> " + ex.Message)
            End If

            Return 0
        End Try

        Return BlockHeightInt

    End Function

    Public Function GetTransaction(ByVal TXID As ULong) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getTransaction&transaction=" + TXID.ToString)

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(TXID=" + TXID.ToString + "): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(TXID=" + TXID.ToString + "): " + Response)
            End If

            Return New List(Of String)
        End If

        Dim RecipientAccountID As String = Converter.Search("recipient", ClsJSONAndXMLConverter.E_ParseType.XML)
        Dim RecipientPublicKey As String = GetAccountPublicKeyFromAccountID_RS(RecipientAccountID)

        If IsErrorOrWarning(RecipientPublicKey, "", False, False) Then
            RecipientPublicKey = ""
        End If

        Dim TXDetailList As List(Of String) = New List(Of String)

        TXDetailList.Add(Converter.Search("timestamp", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("recipient", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("recipientRS", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add("<recipientPublicKey>" + RecipientPublicKey + "</recipientPublicKey>")
        TXDetailList.Add(Converter.Search("amountNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("feeNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("balanceNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("transaction", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("attachment", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("sender", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("senderRS", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("senderPublicKey", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("height", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("block", ClsJSONAndXMLConverter.E_ParseType.XML))
        TXDetailList.Add(Converter.Search("confirmations", ClsJSONAndXMLConverter.E_ParseType.XML))

        TXDetailList = TXDetailList.Where(Function(l) l <> "").ToList()

#Region "deprecaded"
        'For Each T_Entry In DirectCast(RespList, List(Of Object))

        '    Dim Entry As List(Of Object) = New List(Of Object)

        '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '        Entry = DirectCast(T_Entry, List(Of Object))
        '    End If

        '    If Entry.Count > 0 Then

        '        Select Case Entry(0).ToString
        '            Case "type"

        '            Case "subtype"

        '            Case "timestamp"
        '                TXDetailList.Add("<timestamp>" + Entry(1).ToString + "</timestamp>")

        '            Case "deadline"

        '            Case "senderPublicKey"

        '            Case "recipient"
        '                TXDetailList.Add("<recipient>" + Entry(1).ToString + "</recipient>")

        '            Case "recipientRS"
        '                TXDetailList.Add("<recipientRS>" + Entry(1).ToString + "</recipientRS>")

        '            Case "amountNQT"
        '                TXDetailList.Add("<amountNQT>" + Entry(1).ToString + "</amountNQT>")

        '            Case "feeNQT"
        '                TXDetailList.Add("<feeNQT>" + Entry(1).ToString + "</feeNQT>")

        '            Case "signature"

        '            Case "signatureHash"

        '            Case "balanceNQT"
        '                TXDetailList.Add("<balanceNQT>" + Entry(1).ToString + "</balanceNQT>")

        '            Case "fullHash"

        '            Case "transaction"
        '                TXDetailList.Add("<transaction>" + Entry(1).ToString + "</transaction>")

        '            Case "attachment"

        '                Dim Attachments As List(Of Object) = TryCast(Entry(1), List(Of Object))

        '                Dim AttStr As String = "<attachment>"

        '                If Not Attachments Is Nothing Then
        '                    AttStr += JSON.JSONListToXMLRecursive(Attachments)
        '                End If

        '                AttStr += "</attachment>"

        '                TXDetailList.Add(AttStr)

        '            Case "attachmentBytes"

        '            Case "sender"
        '                TXDetailList.Add("<sender>" + Entry(1).ToString + "</sender>")

        '            Case "senderRS"
        '                TXDetailList.Add("<senderRS>" + Entry(1).ToString + "</senderRS>")

        '            Case "height"
        '                TXDetailList.Add("<height>" + Entry(1).ToString + "</height>")

        '            Case "version"

        '            Case "ecBlockId"

        '            Case "ecBlockHeight"

        '            Case "block"
        '                TXDetailList.Add("<block>" + Entry(1).ToString + "</block>")

        '            Case "confirmations"
        '                TXDetailList.Add("<confirmations>" + Entry(1).ToString + "</confirmations>")

        '            Case "blockTimestamp"

        '            Case "requestProcessingTime"

        '        End Select

        '    End If

        'Next
#End Region

        Return TXDetailList

    End Function

    Public Function GetAccountTransactions(ByVal AccountID As ULong, Optional ByVal FromTimestamp As ULong = 0UL, Optional ByVal FirstIndex As ULong = 0UL, Optional ByVal LastIndex As ULong = 0UL) As List(Of List(Of String))

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Request As String = "requestType=getAccountTransactions&account=" + AccountID.ToString

        If Not FromTimestamp = 0UL Then
            Request += "&timestamp=" + FromTimestamp.ToString
        End If

        If Not FirstIndex = 0UL Then
            Request += "&firstIndex=" + FirstIndex.ToString
        End If

        If Not LastIndex = 0UL Then
            Request += "&lastIndex=" + LastIndex.ToString
        End If

        Dim Response As String = SignumRequest(Request)

        'requestType=getAccountTransactions&account=568413546036034892&timestamp=280156462
        'requestType=getAccountTransactions&account=17481325922122010625&timestamp=278086925

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): -> " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON
        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        Dim Transactions As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/transactions")

        If IsNothing(Transactions.Key) Then
            Return ReturnList
        End If

        Dim TransactionList As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object)) ' DirectCast(Transactions.Value, List(Of KeyValuePair(Of String, Object)))

        If Transactions.Value.GetType = GetType(List(Of KeyValuePair(Of String, Object))) Then
            TransactionList = DirectCast(Transactions.Value, List(Of KeyValuePair(Of String, Object)))
        Else
            TransactionList.Add(New KeyValuePair(Of String, Object)("#", Transactions.Value))
        End If


        For Each Transaction As KeyValuePair(Of String, Object) In TransactionList

            Dim SubConverter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Transaction)

            Dim TempList As List(Of String) = New List(Of String)

            TempList.Add(SubConverter.Search("type", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("timestamp", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("recipient", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("recipientRS", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("amountNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("feeNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("transaction", ClsJSONAndXMLConverter.E_ParseType.XML))

            Dim Attachment As List(Of KeyValuePair(Of String, Object)) = SubConverter.Search(Of List(Of KeyValuePair(Of String, Object)))("attachment")
            Dim MessageList As List(Of KeyValuePair(Of String, Object)) = Attachment.Where(Function(c) c.Key = "message").ToList()
            Dim Message As KeyValuePair(Of String, Object) = MessageList.FirstOrDefault()
            Dim AttachmentString As String = "<attachment>"

            If Not IsNothing(Message.Key) Then
                Dim IsText As String = Attachment.Where(Function(c) c.Key = "messageIsText").ToList()(0).Value.ToString()
                AttachmentString += "<message>" + Message.Value.ToString() + "</message><isText>" + IsText + "</isText>"
            End If

            Dim EncMessageList As List(Of KeyValuePair(Of String, Object)) = Attachment.Where(Function(c) c.Key = "encryptedMessage").ToList()
            Dim EncMessage As KeyValuePair(Of String, Object) = EncMessageList.FirstOrDefault()

            If Not IsNothing(EncMessage.Key) Then ' EncMessage.GetType.Name = GetType(Boolean).Name Then

                Dim Data As String = New ClsJSONAndXMLConverter(EncMessage).Search("data", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "data"))
                Dim Nonce As String = New ClsJSONAndXMLConverter(EncMessage).Search("nonce", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "nonce"))
                Dim IsText As String = New ClsJSONAndXMLConverter(EncMessage).Search("isText", ClsJSONAndXMLConverter.E_ParseType.XML) ' Convert.ToString(JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "isText"))

                If Not Data.Trim() = "" And Not Nonce.Trim() = "" Then
                    AttachmentString += Data + Nonce + IsText ' "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                End If

            End If

            AttachmentString += "</attachment>"

            TempList.Add(AttachmentString)

            TempList.Add(SubConverter.Search("sender", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("senderRS", ClsJSONAndXMLConverter.E_ParseType.XML))
            TempList.Add(SubConverter.Search("confirmations", ClsJSONAndXMLConverter.E_ParseType.XML))

            ReturnList.Add(TempList)

        Next

        Return ReturnList

    End Function

    'Public Function GetAccountTransactionsRAWList(ByVal AccountID As ULong, Optional ByVal FromTimestamp As ULong = 0UL, Optional ByVal FirstIndex As ULong = 0UL, Optional ByVal LastIndex As ULong = 0UL) As List(Of String)

    '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)

    '    Dim Request As String = "requestType=getAccountTransactions&account=" + AccountID.ToString

    '    If Not FromTimestamp = 0UL Then
    '        Request += "&timestamp=" + FromTimestamp.ToString
    '    End If

    '    If Not FirstIndex = 0UL Then
    '        Request += "&firstIndex=" + FirstIndex.ToString
    '    End If

    '    If Not LastIndex = 0UL Then
    '        Request += "&lastIndex=" + LastIndex.ToString
    '    End If

    '    Dim Response As String = SignumRequest(Request)

    '    If Response.Contains(Application.ProductName + "-error") Then
    '        If GetINISetting(E_Setting.InfoOut, False) Then
    '            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactionsRAWList(): -> " + Response)
    '        End If

    '        Return New List(Of String)
    '    End If

    '    Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

    '    'Dim JSON As ClsJSON = New ClsJSON

    '    Response = GetStringBetween(Response, "[", "]", True)

    '    If Response.Trim = "" Then
    '        Return New List(Of String)
    '    End If

    '    Dim T_List As List(Of String) = Between2List(Response, "{", "}")
    '    T_List(1) = T_List(1).Replace("{},", "")
    '    Dim JSONStringList As List(Of String) = New List(Of String)
    '    JSONStringList.Add(T_List(0))

    '    While T_List(1).Length > 2
    '        T_List = Between2List(T_List(1), "{", "}")

    '        If T_List.Count > 0 Then
    '            T_List(1) = T_List(1).Replace("{},", "")
    '            JSONStringList.Add(T_List(0))
    '        End If

    '    End While

    '    Return JSONStringList

    'End Function

    Public Function GetTransactionIds(ByVal Sender As ULong, Optional ByVal Recipient As ULong = 0UL, Optional ByVal FromTimestamp As ULong = 0UL) As List(Of ULong)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Request As String = "requestType=getTransactionIds&sender=" + Sender.ToString

        If Not Recipient = 0UL Then
            Request += "&recipient=" + Recipient.ToString
        End If

        If Not FromTimestamp = 0UL Then
            Request += "&timestamp=" + FromTimestamp.ToString
        End If

        Dim Response As String = SignumRequest(Request)

        '{"transactionIds":["7277823693073560564"],"requestProcessingTime":12}

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        Dim TXList As List(Of String) = Converter.AllValues("transactionIds")

        Dim NuList As List(Of ULong) = New List(Of ULong)

        For Each TXId As String In TXList
            NuList.Add(Convert.ToUInt64(TXId))
        Next

        Return NuList

#Region "deprecaded"

        ''Dim ResponseList As List(Of Object) = New List(Of Object)
        ''If RespList.GetType.Name = GetType(List(Of Object)).Name Then
        ''    ResponseList = DirectCast(RespList, List(Of Object))
        ''End If

        'If TxIdsx.Count > 0 Then

        '    For Each T_Entry In TxIdsx

        '        Dim Entry As List(Of Object) = New List(Of Object)

        '        If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '            Entry = DirectCast(T_Entry, List(Of Object))
        '        End If

        '        If Entry.Count > 0 Then

        '            Select Case Entry(0).ToString
        '                Case "transactionIds"

        '                    Dim TXIDs As List(Of Object) = New List(Of Object) ' TryCast(Entry(1), List(Of Object))

        '                    If Entry(1).GetType.Name = GetType(List(Of Object)).Name Then
        '                        TXIDs = DirectCast(Entry(1), List(Of Object))
        '                    End If

        '                    If TXIDs.Count > 0 Then

        '                        If Not TXIDs Is Nothing Then

        '                            For Each TXID In TXIDs

        '                                If TXID.GetType.Name = GetType(String).Name Then
        '                                    NuList.Add(Convert.ToUInt64(TXID))
        '                                ElseIf TXIDs.GetType.Name = GetType(List(Of Object)).Name Then

        '                                    Dim ObjList As List(Of Object) = TryCast(TXID, List(Of Object))

        '                                    If ObjList Is Nothing Then

        '                                        Dim StrList As List(Of String) = TryCast(TXID, List(Of String))

        '                                        If StrList Is Nothing Then

        '                                        Else
        '                                            For Each STXID In StrList

        '                                                If STXID.GetType.Name = GetType(String).Name Then
        '                                                    NuList.Add(Convert.ToUInt64(STXID))
        '                                                ElseIf STXID.GetType.Name = GetType(List(Of Object)).Name Then

        '                                                End If

        '                                            Next
        '                                        End If

        '                                    Else
        '                                        For Each STXID In ObjList

        '                                            If STXID.GetType.Name = GetType(String).Name Then
        '                                                NuList.Add(Convert.ToUInt64(STXID))
        '                                            ElseIf STXID.GetType.Name = GetType(List(Of Object)).Name Then

        '                                            End If

        '                                        Next
        '                                    End If



        '                                End If

        '                            Next

        '                        Else

        '                        End If

        '                    End If

        '                    'Try

        '                    '    For Each x As Object In Entry(1) '(0)
        '                    '        Try
        '                    '            NuList.Add(ULong.Parse(x))
        '                    '        Catch ex As Exception

        '                    '        End Try
        '                    '    Next

        '                    'Catch ex As Exception

        '                    'End Try

        '            End Select

        '        End If

        '    Next

        'End If

        'Return New List(Of ULong)

#End Region

    End Function

    Public Function GetSmartContractIds() As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getATIds&machineCodeHashId=" + C_ReferenceMachineCodeHashID.ToString())

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetSmartContractIds(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetSmartContractIds(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As List(Of Object) = Nothing ' JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(RespList, "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetSmartContractIds(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetSmartContractIds(): " + Response)
            End If

            Return New List(Of String)
        End If

        Return Converter.AllValues("atIds")

#Region "deprecaded"

        'For Each T_Entry In RespList

        '    Dim Entry As List(Of Object) = New List(Of Object)

        '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '        Entry = DirectCast(T_Entry, List(Of Object))
        '    End If

        '    If Entry.Count > 0 Then

        '        If Entry(0).GetType.Name = GetType(String).Name Then
        '            If Entry(0).ToString = "atIds" Then

        '                Dim SubEntry As List(Of Object) = New List(Of Object)

        '                If Entry(1).GetType.Name = GetType(List(Of Object)).Name Then
        '                    SubEntry = DirectCast(Entry(1), List(Of Object))
        '                End If

        '                If SubEntry.Count > 0 Then

        '                    Dim RetList As List(Of String) = New List(Of String)

        '                    If SubEntry(0).GetType.Name = GetType(List(Of String)).Name Then
        '                        RetList = DirectCast(SubEntry(0), List(Of String))
        '                    ElseIf SubEntry(0).GetType.Name = GetType(String).Name Then
        '                        RetList.Add(SubEntry(0).ToString())
        '                    End If

        '                    'Try
        '                    '    RetList = SubEntry(0)
        '                    'Catch ex As Exception

        '                    'End Try

        '                    Return RetList

        '                End If

        '            End If

        '        End If

        '    End If


        'Next
#End Region

        Return New List(Of String)

    End Function

    Public Function GetSmartContractDetails(ByVal SmartContractID As ULong) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getAT&at=" + SmartContractID.ToString)

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetSmartContractDetails(" + SmartContractID.ToString + "): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As Object = Nothing ' JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetSmartContractDetails2(" + SmartContractID.ToString + "): " + Response)
            End If

            Return New List(Of String)
        End If

        Dim SmartContractDetailList As List(Of String) = New List(Of String)


        SmartContractDetailList.Add(Converter.Search("creator", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("creatorRS", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("at", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("atRS", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("name", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("description", ClsJSONAndXMLConverter.E_ParseType.XML))


        SmartContractDetailList.Add(Converter.Search("machineCode", ClsJSONAndXMLConverter.E_ParseType.XML))
        If Not C_ReferenceMachineCode Is Nothing Then
            If C_ReferenceMachineCode.Trim() = GetStringBetween(Converter.Search("machineCode", ClsJSONAndXMLConverter.E_ParseType.XML), "<machineCode>", "</machineCode>") Then
                SmartContractDetailList.Add("<referenceMachineCode>True</referenceMachineCode>")
            Else
                SmartContractDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
            End If
        Else
            SmartContractDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
        End If

        SmartContractDetailList.Add(Converter.Search("machineCodeHashId", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("machineData", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("creationMachineData", ClsJSONAndXMLConverter.E_ParseType.XML))

        If Not C_ReferenceMachineData Is Nothing Then
            If C_ReferenceMachineData.Trim = GetStringBetween(Converter.Search("creationMachineData", ClsJSONAndXMLConverter.E_ParseType.XML), "<creationMachineData>", "</creationMachineData>") Then
                SmartContractDetailList.Add("<referenceMachineData>True</referenceMachineData>")
            Else
                SmartContractDetailList.Add("<referenceMachineData>False</referenceMachineData>")
            End If

        End If

        SmartContractDetailList.Add(Converter.Search("balanceNQT", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("frozen", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("running", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("stopped", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("finished", ClsJSONAndXMLConverter.E_ParseType.XML))
        SmartContractDetailList.Add(Converter.Search("dead", ClsJSONAndXMLConverter.E_ParseType.XML))

        SmartContractDetailList = SmartContractDetailList.Where(Function(l) l <> "").ToList()

#Region "deprecaded"
        'For Each T_Entry In DirectCast(RespList, List(Of Object))

        '    Dim Entry As List(Of Object) = New List(Of Object)

        '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
        '        Entry = DirectCast(T_Entry, List(Of Object))
        '    End If

        '    If Entry.Count > 0 Then

        '        Select Case Entry(0).ToString
        '            Case "creator"
        '                SmartContractDetailList.Add("<creator>" + Entry(1).ToString + "</creator>")
        '            Case "creatorRS"
        '                SmartContractDetailList.Add("<creatorRS>" + Entry(1).ToString + "</creatorRS>")
        '            Case "at"
        '                SmartContractDetailList.Add("<at>" + Entry(1).ToString + "</at>")

        '            Case "atRS"
        '                SmartContractDetailList.Add("<atRS>" + Entry(1).ToString + "</atRS>")

        '            Case "atVersion"

        '            Case "name"
        '                SmartContractDetailList.Add("<name>" + Entry(1).ToString + "</name>")

        '            Case "description"
        '                SmartContractDetailList.Add("<description>" + Entry(1).ToString + "</description>")

        '            Case "machineCode"
        '                SmartContractDetailList.Add("<machineCode>" + Entry(1).ToString + "</machineCode>")

        '                If Not C_ReferenceMachineCode Is Nothing Then
        '                    If C_ReferenceMachineCode.Trim() = Entry(1).ToString().Trim() Then
        '                        SmartContractDetailList.Add("<referenceMachineCode>True</referenceMachineCode>")
        '                    Else
        '                        SmartContractDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
        '                    End If
        '                Else
        '                    SmartContractDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
        '                End If
        '            Case "machineCodeHashId"
        '                SmartContractDetailList.Add("<machineCodeHashId>" + Entry(1).ToString + "</machineCodeHashId>")

        '            Case "machineData"
        '                SmartContractDetailList.Add("<machineData>" + Entry(1).ToString + "</machineData>")

        '            Case "creationMachineData"
        '                SmartContractDetailList.Add("<creationMachineData>" + Entry(1).ToString + "</creationMachineData>")

        '                If Not C_ReferenceMachineData Is Nothing Then
        '                    If C_ReferenceMachineData.Trim = Entry(1).ToString.Trim Then
        '                        SmartContractDetailList.Add("<referenceMachineData>True</referenceMachineData>")
        '                    Else
        '                        SmartContractDetailList.Add("<referenceMachineData>False</referenceMachineData>")
        '                    End If

        '                End If

        '                'SmartContractDetailList.Add("<creationMachineData>" + Entry(1).ToString + "</creationMachineData>")

        '            Case "balanceNQT"
        '                SmartContractDetailList.Add("<balanceNQT>" + Entry(1).ToString + "</balanceNQT>")

        '            Case "prevBalanceNQT"

        '            Case "nextBlock"

        '            Case "frozen"
        '                SmartContractDetailList.Add("<frozen>" + Entry(1).ToString + "</frozen>")

        '            Case "running"
        '                SmartContractDetailList.Add("<running>" + Entry(1).ToString + "</running>")

        '            Case "stopped"
        '                SmartContractDetailList.Add("<stopped>" + Entry(1).ToString + "</stopped>")

        '            Case "finished"
        '                SmartContractDetailList.Add("<finished>" + Entry(1).ToString + "</finished>")

        '            Case "dead"
        '                SmartContractDetailList.Add("<dead>" + Entry(1).ToString + "</dead>")

        '            Case "minActivation"""

        '            Case "creationBlock"""

        '            Case "requestProcessingTime"

        '        End Select

        '    End If

        'Next
#End Region

        Return SmartContractDetailList

    End Function

#End Region 'Get

#Region "Get Advance"



#End Region

#Region "Send"

    Public Function SendMoney(ByVal SenderPublicKey As String, ByVal RecipientID As ULong, ByVal Amount As Double, Optional ByVal Fee As Double = 0.0, Optional ByVal Message As String = "", Optional ByVal MessageIsText As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String

        'If C_PassPhrase.Trim = "" Then
        '    Return "error in SendMoney(): no PassPhrase"
        'End If

        'Dim SignumNET As ClsSignumNET = New ClsSignumNET()
        'Dim MasterkeyList As List(Of Byte()) = SignumNET.GenerateMasterKeys()

        Dim PublicKey As String = SenderPublicKey ' ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        'Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))


        Dim AmountNQT As String = Dbl2Planck(Amount).ToString

        If Fee = 0.0 Then
            Fee = GetTXFee(Message)
        End If

        Dim FeeNQT As String = Dbl2Planck(Fee).ToString

        Dim postDataRL As String = "requestType=sendMoney"
        postDataRL += "&recipient=" + RecipientID.ToString
        postDataRL += "&amountNQT=" + AmountNQT
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey ' <<< debug errormaker
        postDataRL += "&feeNQT=" + FeeNQT
        postDataRL += "&deadline=60"
        'postDataRL += "&referencedTransactionFullHash="
        'postDataRL += "&broadcast="

        If Not Message.Trim = "" Then
            postDataRL += "&message=" + Message
            postDataRL += "&messageIsText=" + MessageIsText.ToString
        End If

        'postDataRL += "&messageToEncrypt="
        'postDataRL += "&messageToEncryptIsText="
        'postDataRL += "&encryptedMessageData="
        'postDataRL += "&encryptedMessageNonce="
        'postDataRL += "&messageToEncryptToSelf="
        'postDataRL += "&messageToEncryptToSelfIsText="
        'postDataRL += "&encryptToSelfMessageData="
        'postDataRL += "&encryptToSelfMessageNonce="


        If Not RecipientPublicKey.Trim = "" Then
            postDataRL += " &recipientPublicKey=" + RecipientPublicKey
        End If

        Dim Response As String = SignumRequest(postDataRL)

        Return Response

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        'Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        'If Error0.GetType.Name = GetType(Boolean).Name Then
        '    'TX OK
        'ElseIf Error0.GetType.Name = GetType(String).Name Then
        '    'TX not OK
        '    Return Application.ProductName + "-error in SendMoney(): " + Response
        'End If


        'Dim UTXBytes As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")

        'Dim Returner As String = ""
        'If UTXBytes.GetType.Name = GetType(String).Name Then
        '    Returner = CStr(UTXBytes)
        'End If


        'If Not Returner.Trim = "" Then
        '    Dim SignumNET As ClsSignumNET = New ClsSignumNET()
        '    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTXBytes, SenderSignKey)
        '    Returner = BroadcastTransaction(STX.SignedTransaction)
        'End If

        'Return Returner

    End Function
    Public Function SendMessage(ByVal SenderPublicKey As String, ByVal SenderAgreementKey As String, ByVal RecipientID As ULong, ByVal Message As String, Optional ByVal MessageIsText As Boolean = True, Optional ByVal Encrypt As Boolean = False, Optional ByVal Fee As Double = 0.0, Optional ByVal RecipientPublicKey As String = "") As String

        If RecipientPublicKey = "" Then
            RecipientPublicKey = GetAccountPublicKeyFromAccountID_RS(RecipientID.ToString())
        End If

        If RecipientPublicKey.Trim() = "" Or RecipientPublicKey.Trim() = "0000000000000000000000000000000000000000000000000000000000000000" Or RecipientPublicKey.Contains(Application.ProductName + "-error") Then
            Encrypt = False
            RecipientPublicKey = ""
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET()
        'Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = SenderPublicKey ' ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        Dim AgreementKey As String = SenderAgreementKey ' ByteAry2HEX(MasterkeyList(2))

        Dim postDataRL As String = "requestType=sendMessage"
        postDataRL += "&recipient=" + RecipientID.ToString()
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey

        postDataRL += "&deadline=60"
        'postDataRL += "&referencedTransactionFullHash="
        'postDataRL += "&broadcast="

        If Encrypt Then
            ' postDataRL += "&messageToEncrypt=" + Message
            postDataRL += "&messageToEncryptIsText=" + MessageIsText.ToString()

            Dim EncryptedMessage_Nonce As String() = Signum.EncryptMessage(Message, RecipientPublicKey, AgreementKey)

            postDataRL += "&encryptedMessageData=" + EncryptedMessage_Nonce(0)
            postDataRL += "&encryptedMessageNonce=" + EncryptedMessage_Nonce(1)

            If Fee = 0.0 Then
                Fee = GetTXFee(EncryptedMessage_Nonce(0) + EncryptedMessage_Nonce(1))
            End If

        Else

            If Fee = 0.0 Then
                Fee = GetTXFee(Message)
            End If

            postDataRL += "&message=" + Message
            postDataRL += "&messageIsText=" + MessageIsText.ToString()
        End If

        Dim FeeNQT As String = Dbl2Planck(Fee).ToString()
        postDataRL += "&feeNQT=" + FeeNQT

        'postDataRL += "&messageToEncryptToSelf="
        'postDataRL += "&messageToEncryptToSelfIsText="
        'postDataRL += "&encryptToSelfMessageData="
        'postDataRL += "&encryptToSelfMessageNonce="

        'If Not RecipientPublicKey.Trim = "" Then

        If Not RecipientPublicKey.Trim() = "" Then
            postDataRL += "&recipientPublicKey=" + RecipientPublicKey
        End If


        'If Not RecipientPublicKey.Contains(Application.ProductName + "-error") And Not RecipientPublicKey = "False" Then
        '    postDataRL += " &recipientPublicKey=" + RecipientPublicKey
        'End If


        'End If

        Dim Response As String = SignumRequest(postDataRL)

        Return Response

        'If Response.Contains(Application.ProductName + "-error") Then
        '    Return Application.ProductName + "-error in SendMessage(): ->" + vbCrLf + Response
        'End If

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As Object = JSON.JSONRecursive(Response)

        'Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        'If Error0.GetType.Name = GetType(Boolean).Name Then
        '    'TX OK
        'ElseIf Error0.GetType.Name = GetType(String).Name Then
        '    'TX not OK
        '    Return Application.ProductName + "-error in SendMessage(): " + Response
        'End If

        'Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")


        'Dim Returner As String = ""
        'If UTX.GetType.Name = GetType(String).Name Then
        '    Returner = CStr(UTX)
        'End If

        'If Not Returner.Trim = "" Then
        '    Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX, SenderSignKey)
        '    Returner = BroadcastTransaction(STX.SignedTransaction)
        'End If

        'Return Returner

    End Function

    Public Function ReadMessage(ByVal TXID As ULong, ByVal AccountID As ULong) As String

        Dim postDataRL As String = "requestType=getTransaction&transaction=" + TXID.ToString()
        Dim Response As String = SignumRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + Response
        End If

        Response = Response.Replace("\", "")

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(Response, ClsJSONAndXMLConverter.E_ParseType.JSON)

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As Object = Nothing ' JSON.JSONRecursive(Response)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

        If Error0 <> -1 Then
            'TX not OK
            Return Application.ProductName + "-error in ReadMessage(): " + Response
        End If

        Dim EncryptedMsg As Object = Converter.FirstValue("encryptedMessage") ' JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "encryptedMessage")
        Dim Data As String = Convert.ToString(Converter.FirstValue("data")) 'JSON.RecursiveListSearch(DirectCast(EncryptedMsg, List(Of Object)), "data"))
        Dim Nonce As String = Convert.ToString(Converter.FirstValue("nonce")) 'JSON.RecursiveListSearch(DirectCast(EncryptedMsg, List(Of Object)), "nonce"))


        Dim SenderID As String = Convert.ToString(Converter.FirstValue("sender")) 'JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "sender"))
        Dim RecipientID As String = Convert.ToString(Converter.FirstValue("recipient")) 'JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "recipient"))

        If AccountID = Convert.ToUInt64(SenderID) Then
            AccountID = Convert.ToUInt64(RecipientID)
        ElseIf AccountID = Convert.ToUInt64(RecipientID) Then
            AccountID = Convert.ToUInt64(SenderID)
        End If

        Dim AccountPublicKey As String = GetAccountPublicKeyFromAccountID_RS(AccountID.ToString())

        If AccountPublicKey.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> no PublicKey for " + AccountID.ToString
        End If

        Dim ReturnStr As String = ""

        If EncryptedMsg.GetType() = GetType(String) Then

            If Nonce = "False" Or Data = "False" Then
                ReturnStr = EncryptedMsg.ToString()
            Else
                Dim SignumAPI As ClsSignumNET = New ClsSignumNET()
                Dim DecryptedMsg As String = SignumAPI.DecryptFrom(AccountPublicKey, Data, Nonce)

                If DecryptedMsg.Contains(Application.ProductName + "-error") Then
                    Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + DecryptedMsg
                ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                    Return Application.ProductName + "-warning in ReadMessage(): -> " + vbCrLf + DecryptedMsg
                End If

                If Not MessageIsHEXString(DecryptedMsg) Then
                    ReturnStr = DecryptedMsg
                End If
            End If

        ElseIf EncryptedMsg.GetType() = GetType(Boolean) Then
            ReturnStr = Convert.ToString(Converter.FirstValue("message")) 'JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "message"))

        End If

        Return ReturnStr

    End Function


    Function ConvertList2String(ByVal input As Object) As String

        If input.GetType().Name = GetType(String).Name Then
            Return input.ToString()
        ElseIf input.GetType().Name = GetType(List(Of )).Name Then

            Dim ReturnStr As String = ""

            For Each Entry In DirectCast(input, List(Of Object))
                ReturnStr += Entry.ToString()
            Next

            Return ReturnStr

        Else
            Return input.ToString()
        End If


    End Function


#End Region 'Send

#Region "Send Advance"

    Public Function CreateSmartContract(ByVal SenderPublicKey As String) As String

        Dim out As ClsOut = New ClsOut(Application.StartupPath)

        Dim PublicKey As String = SenderPublicKey

        Dim postDataRL As String = "requestType=createATProgram"
        postDataRL += "&name=CarbonDEXContract"
        postDataRL += "&description=v12OptimizedContract"
        'postDataRL += "&creationBytes=" + C_ReferenceCreationBytes
        'postDataRL += "&code=" 
        postDataRL += "&data=" + C_CreationMachineData
        'postDataRL += "&dpages=2"
        'postDataRL += "&cspages=1"
        'postDataRL += "&uspages=1"
        postDataRL += "&minActivationAmountNQT=" + _GasFeeNQT.ToString
        postDataRL += "&referencedTransactionFullHash=" + _ReferenceTXFullHash
        postDataRL += "&feeNQT=" + _DeployFeeNQT.ToString
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey
        postDataRL += "&deadline=1440"
        'postDataRL += "&broadcast=true"
        'postDataRL += "&message="
        'postDataRL += "&messageIsText="
        'postDataRL += "&messageToEncrypt="
        'postDataRL += "&messageToEncryptIsText="
        'postDataRL += "&encryptedMessageData="
        'postDataRL += "&encryptedMessageNonce="
        'postDataRL += "&messageToEncryptToSelf="
        'postDataRL += "&messageToEncryptToSelfIsText="
        'postDataRL += "&encryptToSelfMessageData="
        'postDataRL += "&encryptToSelfMessageNonce="
        'postDataRL += "&recipientPublicKey"

        Dim Response As String = SignumRequest(postDataRL)
        '{"errorCode":4,"errorDescription":"Invalid AT creation bytes","requestProcessingTime":37}
        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in CreateSmartContract(): ->" + vbCrLf + Response
        End If

        Return Response

    End Function

#End Region

#End Region 'Blockchain Communication


#Region "Convert tools"

    Public Shared Function ConvertUnsignedTXToList(ByVal UnsignedTX As String) As List(Of String)

        Dim out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(UnsignedTX, ClsJSONAndXMLConverter.E_ParseType.JSON)

        Dim Error0 As Integer = Converter.GetFirstInteger("errorCode")

        If Error0 <> -1 Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                out.ErrorLog2File(Application.ProductName + "-error in ConvertUnsignedTXToList(): " + UnsignedTX)
            End If

            Return New List(Of String)
        End If

        Dim TXDetailList As List(Of String) = New List(Of String)

        TXDetailList.Add(Converter.Search("unsignedTransactionBytes", ClsJSONAndXMLConverter.E_ParseType.XML))

        Dim TransactionJSON As List(Of KeyValuePair(Of String, Object)) = Converter.Search(Of List(Of KeyValuePair(Of String, Object)))("transactionJSON")

        If TransactionJSON.Count > 0 Then

            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "type").ToList()(0)).XMLString)
            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "subtype").ToList()(0)).XMLString)
            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "timestamp").ToList()(0)).XMLString)
            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "amountNQT").ToList()(0)).XMLString)
            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "feeNQT").ToList()(0)).XMLString)
            TXDetailList.Add(New ClsJSONAndXMLConverter(TransactionJSON.Where(Function(c) c.Key = "attachment").ToList()(0)).XMLString)

        End If

        Return TXDetailList

    End Function

    Public Shared Function TimeToUnix(ByVal dteDate As Date) As ULong
        If dteDate.IsDaylightSavingTime = True Then
            dteDate = DateAdd(DateInterval.Hour, -1, dteDate)
        End If
        Return Convert.ToUInt64(DateDiff(DateInterval.Second, CDate("11.08.2014 04:00:16"), dteDate))
    End Function
    Public Shared Function UnixToTime(ByVal strUnixTime As String) As Date
        Dim UnixToTimex As Date = DateAdd(DateInterval.Second, Val(strUnixTime), CDate("11.08.2014 04:00:16"))
        If UnixToTimex.IsDaylightSavingTime = True Then
            UnixToTimex = DateAdd(DateInterval.Hour, 1, UnixToTimex)
        End If

        Return UnixToTimex

    End Function

    Public Shared Function ULng2String(ByVal Lng As ULong) As String

        Dim MsgByteAry() As Byte = BitConverter.GetBytes(Lng)
        Dim MsgByteList As List(Of Byte) = New List(Of Byte)(MsgByteAry)

        MsgByteList.Reverse()

        Dim MsgStr As String = System.Text.Encoding.UTF8.GetString(MsgByteList.ToArray)

        MsgStr = MsgStr.Replace(Convert.ToChar(0), "")

        Return MsgStr

    End Function
    Public Shared Function String2ULng(ByVal input As String, Optional ByVal Reverse As Boolean = True) As ULong

        Dim ByteAry As List(Of Byte) = System.Text.Encoding.UTF8.GetBytes(input).ToList

        If Reverse Then
            ByteAry.Reverse()
        End If

        For i As Integer = ByteAry.Count To 15
            ByteAry.Add(0)
        Next

        Dim MsgLng As ULong = BitConverter.ToUInt64(ByteAry.ToArray, 0)

        Return MsgLng

    End Function

    Public Shared Function ULng2HEX(ByVal ULng As ULong) As String

        Dim RetStr As String = ""

        Dim ParaBytes As List(Of Byte) = BitConverter.GetBytes(ULng).ToList

        For Each ParaByte As Byte In ParaBytes
            Dim T_RetStr As String = Conversion.Hex(ParaByte)

            If T_RetStr.Length < 2 Then
                T_RetStr = "0" + T_RetStr
            End If

            RetStr += T_RetStr

        Next

        Return RetStr.ToLower

    End Function

    Public Shared Function HEX2ULng(ByVal HEX As String) As ULong

        Dim T_ULong As ULong = 0UL

        Dim ByteList As List(Of Byte) = New List(Of Byte)

        For j As Integer = 0 To Convert.ToInt32(HEX.Length / 2) - 1
            Dim HEXStr As String = HEX.Substring(j * 2, 2)

            Dim HEXByte As Byte = Convert.ToByte(HEXStr, 16)
            ByteList.Add(HEXByte)

        Next

        T_ULong = BitConverter.ToUInt64(ByteList.ToArray, 0)

        Return T_ULong

    End Function

    'Public Shared Function ByteAry2HEX(ByVal BytAry() As Byte) As String

    '    Dim RetStr As String = ""

    '    Dim ParaBytes As List(Of Byte) = BytAry.ToList

    '    For Each ParaByte As Byte In ParaBytes
    '        Dim T_RetStr As String = Conversion.Hex(ParaByte)

    '        If T_RetStr.Length < 2 Then
    '            T_RetStr = "0" + T_RetStr
    '        End If

    '        RetStr += T_RetStr

    '    Next

    '    Return RetStr.ToLower

    'End Function

    'Public Shared Function String2HEX(ByVal input As String) As String

    '    Dim inpLng As ULong = String2ULng(input, False)

    '    Return ULng2HEX(inpLng)

    'End Function
    'Public Shared Function HEXStr2String(ByVal input As String) As String

    '    Dim RetStr As String = ""
    '    Dim Ungerade As Integer = input.Length Mod 2

    '    If Ungerade = 1 Then
    '        input += "0"
    '    End If

    '    For j As Integer = 0 To Convert.ToInt32(input.Length / 2) - 1
    '        Dim HEXStr As String = input.Substring(j * 2, 2)

    '        Dim HEXByte As Byte = Convert.ToByte(HEXStr, 16)

    '        RetStr += Chr(HEXByte)
    '    Next

    '    Return RetStr.Replace(Convert.ToChar(0), "")

    'End Function

    Public Shared Function DataStr2ULngList(ByVal ParaStr As String) As List(Of ULong)

        Dim RetStr As String = ""
        Dim Ungerade As Integer = ParaStr.Length Mod 16

        While Not Ungerade = 0
            ParaStr += "0"
            Ungerade = ParaStr.Length Mod 16
        End While


        Dim RetList As List(Of ULong) = New List(Of ULong)
        Try

            Dim HowMuchParas As Double = ParaStr.Length / 16

            For i As Integer = 0 To Convert.ToInt32(HowMuchParas) - 1

                Dim Parameter As String = ParaStr.Substring(i * 16, 16)

                Dim LittleEndianHEXList As List(Of Byte) = New List(Of Byte)

                For j As Integer = 0 To 7
                    Dim HEXStr As String = Parameter.Substring(j * 2, 2)

                    Dim HEXByte As Byte = Convert.ToByte(HEXStr, 16)

                    LittleEndianHEXList.Add(HEXByte)
                Next

                Dim BE As ULong = BitConverter.ToUInt64(LittleEndianHEXList.ToArray, 0)

                RetList.Add(BE)
            Next

            'If RetList.Count < 4 Then
            '    For i As Integer = RetList.Count To 4 - 1
            '        RetList.Add(0UL)
            '    Next
            'End If

            Return RetList

        Catch ex As Exception

            'If RetList.Count < 4 Then
            '    For i As Integer = RetList.Count To 4 - 1
            '        RetList.Add(0UL)
            '    Next
            'End If

            Return RetList
        End Try

    End Function
    Public Shared Function ULngList2DataStr(ByVal ULngList As List(Of ULong)) As String

        Dim RetStr As String = ""

        For Each ULn As ULong In ULngList
            RetStr += ULng2HEX(ULn)
        Next

        Return RetStr.ToLower

    End Function

    '''' <summary>
    '''' Hashing Inputkey and converting them into List(Of ULong)(FirstULongKey, SecondULongKey, ULongHash)
    '''' </summary>
    '''' <param name="InputKey"></param>
    '''' <returns></returns>
    'Public Shared Function GetSHA256_64(ByVal InputKey As String) As List(Of ULong)

    '    Dim InputBytes As List(Of Byte) = New List(Of Byte)

    '    If MessageIsHEXString(InputKey) Then
    '        InputBytes = HEXStringToByteArray(InputKey).ToList
    '    Else
    '        InputBytes = System.Text.Encoding.ASCII.GetBytes(InputKey).ToList
    '    End If


    '    For i As Integer = InputBytes.Count To 24 - 1
    '        InputBytes.Add(0)
    '    Next

    '    Dim FirstULong As ULong = BitConverter.ToUInt64(InputBytes.ToArray, 0)
    '    Dim SecondULong As ULong = BitConverter.ToUInt64(InputBytes.ToArray, 8)
    '    Dim ThirdULong As ULong = BitConverter.ToUInt64(InputBytes.ToArray, 16)

    '    Dim FullHash As Byte() = HEXStringToByteArray(GetSHA64_256(FirstULong, SecondULong, ThirdULong))

    '    Dim ULongHash1 As ULong = BitConverter.ToUInt64(FullHash, 0)
    '    Dim ULongHash2 As ULong = BitConverter.ToUInt64(FullHash, 8)
    '    Dim ULongHash3 As ULong = BitConverter.ToUInt64(FullHash, 16)

    '    Return New List(Of ULong)({FirstULong, SecondULong, ThirdULong, ULongHash1, ULongHash1, ULongHash1})

    'End Function

    'Public Shared Function GetSHA64_256(ByVal FirstULong As ULong, ByVal SecondULong As ULong, ByVal ThirdULong As ULong) As String

    '    Dim ByteList As List(Of Byte) = New List(Of Byte)
    '    ByteList.AddRange(BitConverter.GetBytes(FirstULong))
    '    ByteList.AddRange(BitConverter.GetBytes(SecondULong))
    '    ByteList.AddRange(BitConverter.GetBytes(ThirdULong))
    '    ByteList.AddRange({0, 0, 0, 0, 0, 0, 0, 0})

    '    Dim SHA256 As System.Security.Cryptography.SHA256Managed = New System.Security.Cryptography.SHA256Managed
    '    Dim FullHash As List(Of Byte) = SHA256.ComputeHash(ByteList.ToArray).ToList

    '    Return ByteArrayToHEXString(FullHash.ToArray)

    'End Function

    'Public Shared Function GetSHA256_64_256(ByVal InputKey As String) As String
    '    Dim T_ULongList As List(Of ULong) = GetSHA256_64(InputKey)
    '    Return GetSHA64_256(T_ULongList(0), T_ULongList(1), T_ULongList(2))
    'End Function

    Public Shared Function Dbl2Planck(ByVal Signa As Double) As ULong

        If Double.IsInfinity(Signa) Then
            Signa = 0.0
        End If

        Dim Planck As ULong = Convert.ToUInt64(Signa * 100000000UL)
        Return Planck

    End Function
    Public Shared Function Planck2Dbl(ByVal Planck As ULong) As Double

        Dim Signa As Double = Planck / 100000000UL
        Return Signa

    End Function

#End Region

#Region "Toolfunctions"

    'Private Structure S_Sorter
    '    Dim Timestamp As ULong
    '    Dim TXID As ULong
    'End Structure

    'Private Function SortTimeStamp(ByVal input As List(Of List(Of String))) As List(Of List(Of String))

    '    Dim TSSort As List(Of S_Sorter) = New List(Of S_Sorter)

    '    For i As Integer = 0 To input.Count - 1

    '        Dim Entry As List(Of String) = input(i)

    '        Dim T_Timestamp As ULong = GetULongBetweenFromList(Entry, "<timestamp>", "</timestamp>")
    '        Dim T_Transaction As ULong = GetULongBetweenFromList(Entry, "<transaction>", "</transaction>")

    '        Dim NuSort As S_Sorter = New S_Sorter
    '        NuSort.Timestamp = T_Timestamp
    '        NuSort.TXID = T_Transaction

    '        TSSort.Add(NuSort)
    '    Next

    '    TSSort = TSSort.OrderBy(Function(s) s.Timestamp).ToList

    '    Dim SReturnList As List(Of List(Of String)) = New List(Of List(Of String))

    '    For Each sort In TSSort

    '        For i As Integer = 0 To input.Count - 1
    '            Dim retent = input(i)

    '            Dim T_Timestamp As ULong = GetULongBetweenFromList(retent, "<timestamp>", "</timestamp>")
    '            Dim T_Transaction As ULong = GetULongBetweenFromList(retent, "<transaction>", "</transaction>")

    '            If T_Timestamp = sort.Timestamp And T_Transaction = sort.TXID Then
    '                SReturnList.Add(retent)
    '                Exit For
    '            End If

    '        Next

    '    Next

    '    Return SReturnList

    'End Function

#End Region

End Class