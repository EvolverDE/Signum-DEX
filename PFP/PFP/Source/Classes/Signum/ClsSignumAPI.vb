
Option Strict On
Option Explicit On

Imports System.IO
Imports System.Net
Imports System.Text

Public Class ClsSignumAPI

#Region "SC Structure"
    'AT: 16326550633216940674

    'CreateOrder: 716726961670769723
    'AcceptOrder: 4714436802908501638
    'InjectResponder: 9213622959462902524

    'FinishOrder: 3125596792462301675

    'InjectChainSwapHash: 2770910189976301362
    'FinishOrderWithChainSwapKey: -3992805468895771487

#End Region

    Public Const _ReferenceTX As ULong = 16326550633216940674UL
    Public Const _ReferenceTXFullHash As String = "821a53944c8f93e297cfc05a877681fa6e7c7fc25dc29909cc8b0f71f67c4950"
    Public Const _DeployFeeNQT As ULong = 139650000UL
    Public Const _GasFeeNQT As ULong = 29400000UL
    Public Const _AddressPreFix As String = "TS-"
    Public Const _DefaultNode As String = "http://tordek.ddns.net:6876/burst"
    Public Const _Nodes As String = _DefaultNode + ";" + "https://europe3.testnet.signum.network/burst" + ";" + "http://lmsi.club:6876/burst" + ";" + "https://octalsburstnode.ddns.net:6876/burst"

    Public ReadOnly Property ReferenceCreateOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(716726961670769723L), 0)
    Public ReadOnly Property ReferenceAcceptOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(4714436802908501638L), 0)
    Public ReadOnly Property ReferenceInjectResponder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(9213622959462902524L), 0)
    Public ReadOnly Property ReferenceFinishOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(3125596792462301675L), 0)
    Public ReadOnly Property ReferenceInjectChainSwapHash As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(2770910189976301362L), 0)
    Public ReadOnly Property ReferenceFinishOrderWithChainSwapKey As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(-3992805468895771487L), 0)

    Private ReadOnly Property C_ReferenceCreationBytes As String
    ReadOnly Property C_ReferenceMachineCode As String


    Property C_Node As String = ""

    'Public Property C_PromptPIN As Boolean = True
    'Property C_PassPhrase As String = ""
    Property C_AccountID As ULong
    Property C_Address As String


    Property C_UTXList As List(Of List(Of String)) = New List(Of List(Of String))

    Public Property DEXATList As List(Of String) = New List(Of String)


    Sub New(Optional ByVal Node As String = "", Optional ByVal Account As ULong = 0UL, Optional ByVal ReferenceTX As ULong = _ReferenceTX)

        If Not Node.Trim = "" Then
            C_Node = Node
        Else
            C_Node = GetINISetting(E_Setting.DefaultNode, _DefaultNode)
        End If

        'If Not PassPhrase.Trim = "" Then
        '    C_PassPhrase = PassPhrase
        'End If

        'C_PromptPIN = PromptPIN

        If Not Account = 0UL Then
            C_AccountID = Account
        End If


        'PFPForm.MultiInvoker(PFPForm.E_MainFormControls.LabDebug, "Visible", True)
        'PFPForm.MultiInvoker(PFPForm.E_MainFormControls.LabDebug, "Text", "New()")

        Dim ReferenceTXDetails As List(Of String) = GetTransaction(ReferenceTX)
        C_ReferenceCreationBytes = GetStringBetweenFromList(ReferenceTXDetails, "<creationBytes>", "</creationBytes>")

        Dim ReferenceATDetails = GetATDetails(ReferenceTX)
        C_ReferenceMachineCode = GetStringBetweenFromList(ReferenceATDetails, "<machineCode>", "</machineCode>")

    End Sub


#Region "Blockchain Communication"


#Region "Basic API"

    Function BroadcastTransaction(ByVal TXBytesHexStr As String) As String

        Dim Response As String = SignumRequest("requestType=broadcastTransaction&transactionBytes=" + TXBytesHexStr)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in BroadcastTransaction(): ->" + vbCrLf + Response
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in BroadcastTransaction(): " + Response
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "transaction")

        Dim Returner As String = Application.ProductName + "-error in BroadcastTransaction(): -> UTX failure"
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        ElseIf UTX.GetType.Name = GetType(Boolean).Name Then

        ElseIf UTX.GetType.Name = GetType(List(Of )).Name Then

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

    Public Function IsAT(ByVal AccountID As String, Optional ByVal UseBuffer As Boolean = False) As Boolean

        If Not UseBuffer Then

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)

            Dim Response As String = SignumRequest("requestType=getAccount&account=" + AccountID)

            If Response.Contains(Application.ProductName + "-error") Then
                'PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsAT(): -> " + Response
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Out.ErrorLog2File(Application.ProductName + "-error in IsAT(): -> " + Response)
                End If

                Return False
            End If

            Dim JSON As ClsJSON = New ClsJSON

            Dim RespList As Object = JSON.JSONRecursive(Response)

            Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
            If Error0.GetType.Name = GetType(Boolean).Name Then
                'TX OK
            ElseIf Error0.GetType.Name = GetType(String).Name Then
                'TX not OK
                'PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsAT(): " + Response
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Out.ErrorLog2File(Application.ProductName + "-error in IsAT(): " + Response)
                End If

                Return False
            End If

            Dim PubKey As String = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "publicKey").ToString

            If PubKey = "0000000000000000000000000000000000000000000000000000000000000000" Then
                Return True
            Else
                Return False
            End If

        Else

            For Each DEXAT As String In DEXATList
                If DEXAT = AccountID Then
                    Return True
                End If
            Next

            Return False

        End If

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

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): " + Response
        End If


        Dim PublicKey As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "publicKey").ToString

        Dim Returner As String = ""
        If PublicKey.GetType.Name = GetType(String).Name Then
            Returner = CStr(PublicKey)
        End If

        Return Returner

    End Function

    ''' <summary>
    ''' Converts the given AccountID to Address
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

#Region "Deprecated"
        'Dim Response As String = SignumRequest("requestType=getAccount&account=" + AccountID.ToString)

        'If Response.Contains(Application.ProductName + "-error") Then
        '    ' PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): -> " + Response

        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountRS2=" + Address + "): -> " + Response)
        '    End If

        '    Return CoinBal
        'End If

        'Dim JSON As ClsJSON = New ClsJSON

        'Dim RespList As Object = JSON.JSONRecursive(Response)


        'Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        'If Error0.GetType.Name = GetType(Boolean).Name Then
        '    'TX OK
        'ElseIf Error0.GetType.Name = GetType(String).Name Then
        '    'TX not OK
        '    'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): " + Response

        '    'If GetINISetting(E_Setting.InfoOut, False) Then
        '    '    Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountRS3=" + Address + "): " + Response)
        '    'End If

        '    Return CoinBal
        'End If


        'Dim BalancePlanckStr As String = JSON.RecursiveListSearch(RespList, "balanceNQT").ToString
        'Dim Balance As Double = 0.0

        'Try
        '    Balance = Double.Parse(BalancePlanckStr.Insert(BalancePlanckStr.Length - 8, ","))
        'Catch ex As Exception

        'End Try

        'Dim AvailablePlanckStr As String = JSON.RecursiveListSearch(RespList, "unconfirmedBalanceNQT").ToString
        'Dim Available As Double = 0.0

        'Try
        '    Available = Double.Parse(AvailablePlanckStr.Insert(AvailablePlanckStr.Length - 8, ","))
        'Catch ex As Exception

        'End Try

        'Dim Pending As Double = Available - Balance

        ''(Coin, Account, Address, Balance, Available, Pending)
        'CoinBal(0) = "<coin>SIGNA</coin>"
        'CoinBal(1) = "<account>" + AccountID.ToString + "</account>"
        'CoinBal(2) = "<address>" + Address.Trim + "</address>"
        'CoinBal(3) = "<balance>" + Balance.ToString + "</balance>"
        'CoinBal(4) = "<available>" + Available.ToString + "</available>"
        'CoinBal(5) = "<pending>" + Pending.ToString + "</pending>"

        'Return CoinBal
#End Region

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
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): -> " + Response

            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountID=" + AccountID.ToString + "): -> " + Response)
            End If

            Return CoinBal
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): " + Response

            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(AccountID=" + AccountID.ToString + "): " + Response)
            End If

            Return CoinBal
        End If


        Dim BalancePlanckStr As String = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "balanceNQT").ToString
        Dim Balance As Double = 0.0

        Try
            Balance = Double.Parse(BalancePlanckStr.Insert(BalancePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim AvailablePlanckStr As String = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "unconfirmedBalanceNQT").ToString
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



    Public Function GetTXFee(Optional ByVal Message As String = "") As Double

#Region "deprecated"

        'Dim TXList As List(Of List(Of String)) = GetUnconfirmedTransactions()

        'Dim SlotFee As Double = 0.00735

        'If TXList.Count = 0 Then
        '    Return SlotFee
        'Else
        '    SlotFee *= (TXList.Count + 1)
        'End If
#End Region

        Dim TXFee As Double = 0.00735 * (Math.Floor(Message.Length / 176) + 1) '69

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

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response)
            End If

            Return New List(Of List(Of String))
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "unconfirmedTransactions")

        Dim EntryList As List(Of Object) = New List(Of Object)

        If UTX.GetType.Name = GetType(String).Name Then
            Return New List(Of List(Of String))
        ElseIf UTX.GetType.Name = GetType(Boolean).Name Then
            Return New List(Of List(Of String))
        ElseIf UTX.GetType.Name = GetType(List(Of Object)).Name Then

            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each T_Entry In DirectCast(UTX, List(Of Object))

                Dim Entry As List(Of Object) = New List(Of Object)

                If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                    Entry = DirectCast(T_Entry, List(Of Object))
                End If

                If Entry.Count > 0 Then
                    If Entry(0).ToString = "type" Then
                        If TempOBJList.Count > 0 Then
                            EntryList.Add(TempOBJList)
                        End If

                        TempOBJList = New List(Of Object) From {
                            Entry
                        }
                    Else
                        TempOBJList.Add(Entry)
                    End If

                End If

            Next

            EntryList.Add(TempOBJList)

        Else
            Return New List(Of List(Of String))
        End If


        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each T_Entry In EntryList

            Dim Entry As List(Of Object) = New List(Of Object)

            If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                Entry = DirectCast(T_Entry, List(Of Object))
            End If

            Dim TempList As List(Of String) = New List(Of String)

            For Each T_SubEntry In Entry

                Dim SubEntry As List(Of Object) = New List(Of Object)
                If T_SubEntry.GetType.Name = GetType(List(Of Object)).Name Then
                    SubEntry = DirectCast(T_SubEntry, List(Of Object))
                End If

                If SubEntry.Count > 0 Then

                    Select Case SubEntry(0).ToString
                        Case "type"

                        Case "subtype"

                        Case "timestamp"
                            TempList.Add("<timestamp>" + SubEntry(1).ToString + "</timestamp>")

                        Case "deadline"

                        Case "senderPublicKey"

                        Case "amountNQT"
                            TempList.Add("<amountNQT>" + SubEntry(1).ToString + "</amountNQT>")

                        Case "feeNQT"
                            TempList.Add("<feeNQT>" + SubEntry(1).ToString + "</feeNQT>")

                        Case "signature"

                        Case "signatureHash"

                'Case "balanceNQT"
                '    UTXDetailList.Add("<balanceNQT>" + Entry(1) + "</balanceNQT>")

                        Case "fullHash"

                        Case "transaction"
                            TempList.Add("<transaction>" + SubEntry(1).ToString + "</transaction>")

                        Case "attachment"

                            Dim TMsg As String = "<attachment>"


                            Dim SubSubEntry As List(Of Object) = New List(Of Object)

                            If SubEntry(1).GetType.Name = GetType(List(Of Object)).Name Then
                                SubSubEntry = DirectCast(SubEntry(1), List(Of Object))
                            End If

                            If SubSubEntry.Count > 0 Then

                                Dim Message As String = JSON.RecursiveListSearch(SubSubEntry, "message").ToString

                                If Message.Trim <> "False" Then
                                    Dim IsText As String = JSON.RecursiveListSearch(SubSubEntry, "messageIsText").ToString
                                    TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                                End If

                                Dim EncMessage As Object = JSON.RecursiveListSearch(SubSubEntry, "encryptedMessage")

                                'If EncMessage.GetType.Name = GetType(Boolean).Name Then

                                'Else
                                If EncMessage.GetType.Name = GetType(List(Of Object)).Name Then

                                    Dim EncryptedMessageList As List(Of Object) = New List(Of Object)
                                    If EncMessage.GetType.Name = GetType(List(Of Object)).Name Then
                                        EncryptedMessageList = DirectCast(EncMessage, List(Of Object))
                                    End If

                                    Dim Data As String = Convert.ToString(JSON.RecursiveListSearch(EncryptedMessageList, "data"))
                                    Dim Nonce As String = Convert.ToString(JSON.RecursiveListSearch(EncryptedMessageList, "nonce"))
                                    Dim IsText As String = JSON.RecursiveListSearch(SubSubEntry, "isText").ToString

                                    If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
                                        TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                                    End If
                                Else

                                End If

                            End If

                            TMsg += "</attachment>"

                            TempList.Add(TMsg)

                        Case "sender"
                            TempList.Add("<sender>" + SubEntry(1).ToString + "</sender>")

                        Case "senderRS"
                            TempList.Add("<senderRS>" + SubEntry(1).ToString + "</senderRS>")

                        Case "recipient"
                            TempList.Add("<recipient>" + SubEntry(1).ToString + "</recipient>")

                        Case "recipientRS"
                            TempList.Add("<recipientRS>" + SubEntry(1).ToString + "</recipientRS>")

                        Case "height"
                            TempList.Add("<height>" + SubEntry(1).ToString + "</height>")

                        Case "version"

                        Case "ecBlockId"

                        Case "ecBlockHeight"

                        Case "block"
                            TempList.Add("<block>" + SubEntry(1).ToString + "</block>")

                        Case "confirmations"
                            TempList.Add("<confirmations>" + SubEntry(1).ToString + "</confirmations>")

                        Case "blockTimestamp"

                        Case "requestProcessingTime"

                    End Select

                End If

            Next

            ReturnList.Add(TempList)

        Next

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

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): " + Response)
            End If
            Return 0
        End If

        Dim BlockHeightStr As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "height")

        Try
            BlockHeightInt = Convert.ToInt32(BlockHeightStr)
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

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(TXID=" + TXID.ToString + "): " + Response)
            End If

            Return New List(Of String)
        End If



        Dim TXDetailList As List(Of String) = New List(Of String)

        For Each T_Entry In DirectCast(RespList, List(Of Object))

            Dim Entry As List(Of Object) = New List(Of Object)

            If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                Entry = DirectCast(T_Entry, List(Of Object))
            End If

            If Entry.Count > 0 Then

                Select Case Entry(0).ToString
                    Case "type"

                    Case "subtype"

                    Case "timestamp"
                        TXDetailList.Add("<timestamp>" + Entry(1).ToString + "</timestamp>")

                    Case "deadline"

                    Case "senderPublicKey"

                    Case "recipient"
                        TXDetailList.Add("<recipient>" + Entry(1).ToString + "</recipient>")

                    Case "recipientRS"
                        TXDetailList.Add("<recipientRS>" + Entry(1).ToString + "</recipientRS>")

                    Case "amountNQT"
                        TXDetailList.Add("<amountNQT>" + Entry(1).ToString + "</amountNQT>")

                    Case "feeNQT"
                        TXDetailList.Add("<feeNQT>" + Entry(1).ToString + "</feeNQT>")

                    Case "signature"

                    Case "signatureHash"

                    Case "balanceNQT"
                        TXDetailList.Add("<balanceNQT>" + Entry(1).ToString + "</balanceNQT>")

                    Case "fullHash"

                    Case "transaction"
                        TXDetailList.Add("<transaction>" + Entry(1).ToString + "</transaction>")

                    Case "attachment"

                        Dim Attachments As List(Of Object) = TryCast(Entry(1), List(Of Object))

                        Dim AttStr As String = "<attachment>"

                        If Not IsNothing(Attachments) Then
                            AttStr += JSON.JSONListToXMLRecursive(Attachments)
#Region "deprecated"
                            'For Each Attachment In Attachments
                            '    Dim AttList As List(Of String) = New List(Of String)

                            '    If Attachment.GetType.Name = GetType(List(Of )).Name Then
                            '        For Each x In Attachment

                            '            If x.GetType.Name = GetType(String).Name Then
                            '                AttList.Add(x)
                            '            ElseIf x.GetType.Name = GetType(List(Of )).Name Then
                            '                For Each y In x

                            '                Next
                            '            End If
                            '        Next

                            '    End If

                            '    If Not IsNothing(AttList) Then

                            '        If AttList.Count > 1 Then
                            '            AttStr += "<" + AttList(0) + ">" + AttList(1) + "</" + AttList(0) + ">"
                            '        End If

                            '    End If

                            'Next
#End Region
                        End If

                        AttStr += "</attachment>"

                        TXDetailList.Add(AttStr)

                    Case "attachmentBytes"

                    Case "sender"
                        TXDetailList.Add("<sender>" + Entry(1).ToString + "</sender>")

                    Case "senderRS"
                        TXDetailList.Add("<senderRS>" + Entry(1).ToString + "</senderRS>")

                    Case "height"
                        TXDetailList.Add("<height>" + Entry(1).ToString + "</height>")

                    Case "version"

                    Case "ecBlockId"

                    Case "ecBlockHeight"

                    Case "block"
                        TXDetailList.Add("<block>" + Entry(1).ToString + "</block>")

                    Case "confirmations"
                        TXDetailList.Add("<confirmations>" + Entry(1).ToString + "</confirmations>")

                    Case "blockTimestamp"

                    Case "requestProcessingTime"

                End Select

            End If

        Next

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

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): -> " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim JSON As ClsJSON = New ClsJSON
        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        Response = GetStringBetween(Response, "[", "]", True)

        If Response.Trim = "" Then
            Return ReturnList
        End If

        Dim T_List As List(Of String) = Between2List(Response, "{", "}")
        T_List(1) = T_List(1).Replace("{},", "")
        Dim JSONStringList As List(Of String) = New List(Of String)
        JSONStringList.Add(T_List(0))

        While T_List(1).Length > 2
            T_List = Between2List(T_List(1), "{", "}")

            If T_List.Count > 0 Then
                T_List(1) = T_List(1).Replace("{},", "")
                JSONStringList.Add(T_List(0))
            End If

        End While

        For i As Integer = 0 To JSONStringList.Count - 1

            Dim ResponseTX As String = JSONStringList(i)


            Dim RespList As Object = JSON.JSONRecursive(ResponseTX)

            Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
            If Error0.GetType.Name = GetType(Boolean).Name Then
                'TX OK
            ElseIf Error0.GetType.Name = GetType(String).Name Then
                'TX not OK
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): " + Response)
                End If

                Return New List(Of List(Of String))
            End If

            Dim EntryList As List(Of Object) = New List(Of Object)

            'Dim TX As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "transactions")

            'If TX.GetType.Name = GetType(String).Name Then
            '    Return New List(Of List(Of String))
            'ElseIf TX.GetType.Name = GetType(Boolean).Name Then
            '    Return New List(Of List(Of String))
            'Else

            'Dim TempOBJList As List(Of Object) = New List(Of Object)

            'For Each T_Entry In DirectCast(RespList, List(Of Object))

            '    Dim Entry As List(Of Object) = New List(Of Object)

            '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
            '        Entry = DirectCast(T_Entry, List(Of Object))
            '    End If

            '    If Entry.Count > 0 Then

            '        If Entry(0).ToString = "type" Then
            '            If TempOBJList.Count > 0 Then
            '                EntryList.Add(TempOBJList)
            '            End If

            '            TempOBJList = New List(Of Object) From {
            '                            Entry
            '                        }
            '        Else
            '            TempOBJList.Add(Entry)
            '        End If

            '    End If

            'Next

            'EntryList.Add(TempOBJList)

            'Dim XML As String = ""
            'For Each T_Entry In DirectCast(RespList, List(Of Object))

            '    Dim Entry As List(Of Object) = New List(Of Object)

            '    If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
            '        Entry = DirectCast(T_Entry, List(Of Object))
            '    End If

            'XML = JSON.JSONListToXMLRecursive(Entry)

            Dim TempList As List(Of String) = New List(Of String)

            For Each T_SubEntry In DirectCast(RespList, List(Of Object))

                Dim SubEntry As List(Of Object) = New List(Of Object)

                If T_SubEntry.GetType.Name = GetType(List(Of Object)).Name Then
                    SubEntry = DirectCast(T_SubEntry, List(Of Object))
                End If

                If SubEntry.Count > 0 Then

                    Select Case True
                        Case SubEntry(0).ToString = "type"
                            TempList.Add("<type>" + SubEntry(1).ToString + "</type>")
                        Case SubEntry(0).ToString = "timestamp"
                            TempList.Add("<timestamp>" + SubEntry(1).ToString + "</timestamp>")
                        Case SubEntry(0).ToString = "recipient"
                            TempList.Add("<recipient>" + SubEntry(1).ToString + "</recipient>")
                        Case SubEntry(0).ToString = "recipientRS"
                            TempList.Add("<recipientRS>" + SubEntry(1).ToString + "</recipientRS>")
                        Case SubEntry(0).ToString = "amountNQT"
                            TempList.Add("<amountNQT>" + SubEntry(1).ToString + "</amountNQT>")
                        Case SubEntry(0).ToString = "feeNQT"
                            TempList.Add("<feeNQT>" + SubEntry(1).ToString + "</feeNQT>")
                        Case SubEntry(0).ToString = "transaction"
                            TempList.Add("<transaction>" + SubEntry(1).ToString + "</transaction>")
                        Case SubEntry(0).ToString = "attachment"

                            Dim TMsg As String = "<attachment>"
                            Dim Message As String = JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "message").ToString

                            If Message.Trim <> "False" Then
                                Dim IsText As String = JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "messageIsText").ToString
                                TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                            End If

                            Dim EncMessage As Object = JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "encryptedMessage")

                            If EncMessage.GetType.Name = GetType(Boolean).Name Then

                            ElseIf EncMessage.GetType.Name = GetType(List(Of Object)).Name Then

                                Dim Data As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "data"))
                                Dim Nonce As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncMessage, List(Of Object)), "nonce"))
                                Dim IsText As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(SubEntry(1), List(Of Object)), "isText"))

                                If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
                                    TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                                End If

                            End If

                            TMsg += "</attachment>"
                            TempList.Add(TMsg)

                        Case SubEntry(0).ToString = "sender"
                            TempList.Add("<sender>" + SubEntry(1).ToString + "</sender>")
                        Case SubEntry(0).ToString = "senderRS"
                            TempList.Add("<senderRS>" + SubEntry(1).ToString + "</senderRS>")
                        Case SubEntry(0).ToString = "confirmations"
                            TempList.Add("<confirmations>" + SubEntry(1).ToString + "</confirmations>")
                    End Select

                End If

            Next

            ReturnList.Add(TempList)

            ' Next


#Region "deprecated"

            'For Each TXEntry As List(Of String) In ReturnList

            '    If TXEntry.Count = 0 Then
            '        Return New List(Of List(Of String))
            '    End If

            '    Dim T_SenderAccount As ULong = BetweenFromList(TXEntry, "<sender>", "</sender>",, GetType(ULong))

            '    If TXEntry(0).Trim = "<type>BLSTX</type>" Then
            '        ' ist ATTX
            '        Dim MSgIdx As Integer = BetweenFromList(TXEntry, "<message>", "</message>", True, GetType(Integer))
            '        Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>",, GetType(String))

            '        If Not T_Message.Trim = "" Then
            '            Try

            '                Dim TestMsg As String = HEXStr2String(T_Message)

            '                If TestMsg.Trim = "accepted" Then

            '                ElseIf TestMsg.Trim = "finished" Then

            '                Else

            '                    Dim ULongList = DataStr2ULngList(T_Message)
            '                    TestMsg = "<method>" + ULongList(0).ToString + "</method><colBuyAmount>" + ULongList(1).ToString + "</colBuyAmount><xAmount>" + ULongList(2).ToString + "</xAmount><xItem>" + ULng2String(ULongList(3)) + "</xItem>"

            '                End If


            '                TXEntry(MSgIdx) = "<message>" + TestMsg + "</message>"
            '            Catch ex As Exception
            '                TXEntry(MSgIdx) = "<message></message>"
            '            End Try

            '        End If
            '    Else
            '        ' ist keine ATTX

            '        Dim T_AmountNQT As ULong = BetweenFromList(TXEntry, "<amountNQT>", "</amountNQT>",, GetType(ULong))
            '        Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>",, GetType(String))
            '        Dim MSgIdx As Integer = BetweenFromList(TXEntry, "<message>", "</message>", True, GetType(Integer))

            '        Dim MesULng As List(Of ULong) = New List(Of ULong)

            '        If MSgIdx <> -1 Then
            '            MesULng = DataStr2ULngList(T_Message)
            '        End If

            '        Dim CNT As Integer = MesULng.Count


            '        If MesULng.Count = 0 Then
            '            If MSgIdx = -1 Then

            '            Else
            '                TXEntry(MSgIdx) = "<attachment></attachment>"
            '            End If

            '        Else

            '            Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method>"

            '            Select Case MesULng(0)
            '                Case ReferenceCreateOrder

            '                    Select Case MesULng.Count
            '                        Case 1

            '                        Case 2
            '                            AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
            '                        Case 3
            '                            AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
            '                            AttMsg += "<xAmount>" + MesULng(2).ToString + "</xAmount>"
            '                        Case 4

            '                            Dim MSGStr As String = ULng2String(MesULng(3))

            '                            AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
            '                            AttMsg += "<xAmount>" + MesULng(2).ToString + "</xAmount>"
            '                            AttMsg += "<xItem>" + MSGStr.Trim + "</xItem>"

            '                    End Select

            '                    Dim T_Sum As Double = Double.Parse(T_AmountNQT) - Double.Parse(MesULng(1))

            '                    If T_Sum < 0 Then
            '                        TXEntry(0) = "<type>BuyOrder</type>"
            '                    Else
            '                        TXEntry(0) = "<type>SellOrder</type>"
            '                    End If


            '                Case ReferenceAcceptOrder
            '                    TXEntry(0) = "<type>ResponseOrder</type>"
            '                Case ReferenceInjectResponder
            '                    TXEntry(0) = "<type>ResponseOrder</type>"

            '                    Select Case MesULng.Count
            '                        Case 1

            '                        Case 2
            '                            AttMsg += "<injectedResponser>" + MesULng(1).ToString + "</injectedResponser>"
            '                        Case 3

            '                        Case 4

            '                        Case Else

            '                    End Select

            '                Case ReferenceFinishOrder
            '                    TXEntry(0) = "<type>ResponseOrder</type>"
            '                Case Else
            '                    TXEntry(0) = "<type>ResponseOrder</type>"

            '            End Select

            '            AttMsg += "</attachment>"
            '            TXEntry(MSgIdx) = AttMsg

            '        End If

            '    End If



            '    'If T_SenderAccount = AccountID Then
            '    '    TXEntry(0) = "<type>BLSTX</type>"
            '    '    Dim MSgIdx As Integer = Integer.Parse(BetweenFromList(TXEntry, "<message>", "</message>", True))
            '    '    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")

            '    '    If Not T_Message.Trim = "" Then
            '    '        Try
            '    '            TXEntry(MSgIdx) = "<message>" + HEXStr2String(T_Message) + "</message>"
            '    '        Catch ex As Exception
            '    '            TXEntry(MSgIdx) = "<message></message>"
            '    '        End Try

            '    '    End If

            '    'Else

            '    '    'TempList.Add("<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount></attachment>")

            '    '    Dim T_AmountNQT As ULong = ULong.Parse(BetweenFromList(TXEntry, "<amountNQT>", "</amountNQT>"))
            '    '    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")
            '    '    Dim MSgIdx As Integer = Integer.Parse(BetweenFromList(TXEntry, "<message>", "</message>", True))

            '    '    Dim MesULng As List(Of ULong) = New List(Of ULong)

            '    '    If MSgIdx <> -1 Then
            '    '        MesULng = DataStr2ULngList(T_Message)
            '    '    End If

            '    '    If MesULng.Count = 0 Then
            '    '        If MSgIdx = -1 Then

            '    '        Else
            '    '            TXEntry(MSgIdx) = "<attachment></attachment>"
            '    '        End If

            '    '    ElseIf MesULng.Count >= 3 Then
            '    '        Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount></attachment>"

            '    '        If MesULng.Count = 4 Then
            '    '            Dim MSGStr As String = ULng2String(MesULng(3))
            '    '            AttMsg = "<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount><xitem>" + MSGStr.Trim + "</xitem></attachment>"
            '    '        End If

            '    '        TXEntry(MSgIdx) = AttMsg

            '    '        Dim T_Sum As Double = Double.Parse(T_AmountNQT) - Double.Parse(MesULng(1))

            '    '        If T_Sum < 0 Then
            '    '            TXEntry(0) = "<type>BuyOrder</type>"
            '    '        Else
            '    '            TXEntry(0) = "<type>SellOrder</type>"
            '    '        End If

            '    '    Else
            '    '        Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method></attachment>"

            '    '        TXEntry(MSgIdx) = AttMsg

            '    '        TXEntry(0) = "<type>ResponseOrder</type>"

            '    '    End If

            '    'End If

            'Next

            'ReturnList = SortTimeStamp(ReturnList)

#End Region


            'End If

        Next

        Return ReturnList


    End Function

    Public Function GetAccountTransactionsRAWList(ByVal AccountID As ULong, Optional ByVal FromTimestamp As ULong = 0UL, Optional ByVal FirstIndex As ULong = 0UL, Optional ByVal LastIndex As ULong = 0UL) As List(Of String)

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

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactionsRAWList(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Response = GetStringBetween(Response, "[", "]", True)

        If Response.Trim = "" Then
            Return New List(Of String)
        End If

        Dim T_List As List(Of String) = Between2List(Response, "{", "}")
        T_List(1) = T_List(1).Replace("{},", "")
        Dim JSONStringList As List(Of String) = New List(Of String)
        JSONStringList.Add(T_List(0))

        While T_List(1).Length > 2
            T_List = Between2List(T_List(1), "{", "}")

            If T_List.Count > 0 Then
                T_List(1) = T_List(1).Replace("{},", "")
                JSONStringList.Add(T_List(0))
            End If

        End While

        Return JSONStringList

    End Function

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

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim NuList As List(Of ULong) = New List(Of ULong)

        Dim ResponseList As List(Of Object) = New List(Of Object)
        If RespList.GetType.Name = GetType(List(Of Object)).Name Then
            ResponseList = DirectCast(RespList, List(Of Object))
        End If

        If ResponseList.Count > 0 Then

            For Each T_Entry In ResponseList

                Dim Entry As List(Of Object) = New List(Of Object)

                If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                    Entry = DirectCast(T_Entry, List(Of Object))
                End If

                If Entry.Count > 0 Then

                    Select Case Entry(0).ToString
                        Case "transactionIds"

                            Dim TXIDs As List(Of Object) = New List(Of Object) ' TryCast(Entry(1), List(Of Object))

                            If Entry(1).GetType.Name = GetType(List(Of Object)).Name Then
                                TXIDs = DirectCast(Entry(1), List(Of Object))
                            End If

                            If TXIDs.Count > 0 Then

                                If Not IsNothing(TXIDs) Then

                                    For Each TXID In TXIDs

                                        If TXID.GetType.Name = GetType(String).Name Then
                                            NuList.Add(Convert.ToUInt64(TXID))
                                        ElseIf TXIDs.GetType.Name = GetType(List(Of Object)).Name Then

                                            For Each STXID In DirectCast(TXID, List(Of Object))

                                                If STXID.GetType.Name = GetType(String).Name Then
                                                    NuList.Add(Convert.ToUInt64(STXID))
                                                ElseIf STXID.GetType.Name = GetType(List(Of Object)).Name Then

                                                End If

                                            Next

                                        End If

                                    Next

                                Else

                                End If

                            End If

                            'Try

                            '    For Each x As Object In Entry(1) '(0)
                            '        Try
                            '            NuList.Add(ULong.Parse(x))
                            '        Catch ex As Exception

                            '        End Try
                            '    Next

                            'Catch ex As Exception

                            'End Try

                    End Select

                End If

            Next

        End If

        Return NuList

    End Function


    Public Function GetATIds() As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getATIds")

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATIds(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATIds(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As List(Of Object) = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATIds(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATIds(): " + Response)
            End If

            Return New List(Of String)
        End If


        For Each T_Entry In RespList

            Dim Entry As List(Of Object) = New List(Of Object)

            If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                Entry = DirectCast(T_Entry, List(Of Object))
            End If

            If Entry.Count > 0 Then

                If Entry(0).GetType.Name = GetType(String).Name Then
                    If Entry(0).ToString = "atIds" Then

                        Dim SubEntry As List(Of Object) = New List(Of Object)

                        If Entry(1).GetType.Name = GetType(List(Of Object)).Name Then
                            SubEntry = DirectCast(Entry(1), List(Of Object))
                        End If

                        If SubEntry.Count > 0 Then

                            Dim RetList As List(Of String) = New List(Of String)

                            If SubEntry(0).GetType.Name = GetType(List(Of String)).Name Then
                                RetList = DirectCast(SubEntry(0), List(Of String))
                            End If

                            'Try
                            '    RetList = SubEntry(0)
                            'Catch ex As Exception

                            'End Try

                            Return RetList

                        End If

                    End If

                End If

            End If


        Next

        Return New List(Of String)

    End Function

    Public Function GetATDetails(ByVal ATID As ULong) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getATDetails&at=" + ATID.ToString)

        If Response.Contains(Application.ProductName + "-error") Then
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails1(" + ATID.ToString + "): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails2(" + ATID.ToString + "): " + Response)
            End If

            Return New List(Of String)
        End If

        Dim ATDetailList As List(Of String) = New List(Of String)

        For Each T_Entry In DirectCast(RespList, List(Of Object))

            Dim Entry As List(Of Object) = New List(Of Object)

            If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                Entry = DirectCast(T_Entry, List(Of Object))
            End If

            If Entry.Count > 0 Then

                Select Case Entry(0).ToString
                    Case "creator"
                        ATDetailList.Add("<creator>" + Entry(1).ToString + "</creator>")
                    Case "creatorRS"
                        ATDetailList.Add("<creatorRS>" + Entry(1).ToString + "</creatorRS>")
                    Case "at"
                        ATDetailList.Add("<at>" + Entry(1).ToString + "</at>")

                    Case "atRS"
                        ATDetailList.Add("<atRS>" + Entry(1).ToString + "</atRS>")

                    Case "atVersion"

                    Case "name"
                        ATDetailList.Add("<name>" + Entry(1).ToString + "</name>")

                    Case "description"
                        ATDetailList.Add("<description>" + Entry(1).ToString + "</description>")

                    Case "machineCode"
                        ATDetailList.Add("<machineCode>" + Entry(1).ToString + "</machineCode>")

                        If Not IsNothing(C_ReferenceMachineCode) Then
                            If C_ReferenceMachineCode.Trim = Entry(1).ToString.Trim Then
                                ATDetailList.Add("<referenceMachineCode>True</referenceMachineCode>")
                            Else
                                ATDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
                            End If
                        Else
                            ATDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
                        End If

                    Case "machineData"
                        ATDetailList.Add("<machineData>" + Entry(1).ToString + "</machineData>")

                    Case "balanceNQT"
                        ATDetailList.Add("<balanceNQT>" + Entry(1).ToString + "</balanceNQT>")

                    Case "prevBalanceNQT"

                    Case "nextBlock"

                    Case "frozen"
                        ATDetailList.Add("<frozen>" + Entry(1).ToString + "</frozen>")

                    Case "running"
                        ATDetailList.Add("<running>" + Entry(1).ToString + "</running>")

                    Case "stopped"
                        ATDetailList.Add("<stopped>" + Entry(1).ToString + "</stopped>")

                    Case "finished"
                        ATDetailList.Add("<finished>" + Entry(1).ToString + "</finished>")

                    Case "dead"
                        ATDetailList.Add("<dead>" + Entry(1).ToString + "</dead>")

                    Case "minActivation"""

                    Case "creationBlock"""

                    Case "requestProcessingTime"

                End Select

            End If

        Next

        Return ATDetailList

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
            RecipientPublicKey = GetAccountPublicKeyFromAccountID_RS(RecipientID.ToString)
        End If

        If RecipientPublicKey.Trim = "" Or RecipientPublicKey.Trim = "0000000000000000000000000000000000000000000000000000000000000000" Or RecipientPublicKey.Contains(Application.ProductName + "-error") Then
            Encrypt = False
            RecipientPublicKey = ""
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET()
        'Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = SenderPublicKey ' ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        Dim AgreementKey As String = SenderAgreementKey ' ByteAry2HEX(MasterkeyList(2))

        Dim postDataRL As String = "requestType=sendMessage"
        postDataRL += "&recipient=" + RecipientID.ToString
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey

        postDataRL += "&deadline=60"
        'postDataRL += "&referencedTransactionFullHash="
        'postDataRL += "&broadcast="

        If Encrypt Then
            ' postDataRL += "&messageToEncrypt=" + Message
            postDataRL += "&messageToEncryptIsText=" + MessageIsText.ToString

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
            postDataRL += "&messageIsText=" + MessageIsText.ToString
        End If

        Dim FeeNQT As String = Dbl2Planck(Fee).ToString
        postDataRL += "&feeNQT=" + FeeNQT

        'postDataRL += "&messageToEncryptToSelf="
        'postDataRL += "&messageToEncryptToSelfIsText="
        'postDataRL += "&encryptToSelfMessageData="
        'postDataRL += "&encryptToSelfMessageNonce="

        'If Not RecipientPublicKey.Trim = "" Then

        If Not RecipientPublicKey.Trim = "" Then
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

        Dim postDataRL As String = "requestType=getTransaction&transaction=" + TXID.ToString
        Dim Response As String = SignumRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + Response
        End If

        Response = Response.Replace("\", "")

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in ReadMessage(): " + Response
        End If


        Dim EncryptedMsg As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "encryptedMessage")

        Dim SenderID As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "sender"))
        Dim RecipientID As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "recipient"))

        If AccountID = Convert.ToUInt64(SenderID) Then
            AccountID = Convert.ToUInt64(RecipientID)
        ElseIf AccountID = Convert.ToUInt64(RecipientID) Then
            AccountID = Convert.ToUInt64(SenderID)
        End If

        Dim AccountPublicKey As String = GetAccountPublicKeyFromAccountID_RS(AccountID.ToString)

        If AccountPublicKey.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> no PublicKey for " + AccountID.ToString
        End If

        Dim ReturnStr As String = ""

        If EncryptedMsg.GetType.Name = GetType(String).Name Then
            ReturnStr = EncryptedMsg.ToString
        ElseIf EncryptedMsg.GetType.Name = GetType(Boolean).Name Then
            ReturnStr = Convert.ToString(JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "message"))
        Else

            Dim Data As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncryptedMsg, List(Of Object)), "data"))
            Dim Nonce As String = Convert.ToString(JSON.RecursiveListSearch(DirectCast(EncryptedMsg, List(Of Object)), "nonce"))

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

        Return ReturnStr

    End Function


    Function ConvertList2String(ByVal input As Object) As String

        If input.GetType.Name = GetType(String).Name Then
            Return input.ToString
        ElseIf input.GetType.Name = GetType(List(Of )).Name Then

            Dim ReturnStr As String = ""

            For Each Entry In DirectCast(input, List(Of Object))
                ReturnStr += Entry.ToString
            Next

            Return ReturnStr

        Else
            Return input.ToString
        End If


    End Function


#End Region 'Send

#Region "Send Advance"

    Public Function CreateAT(ByVal SenderPublicKey As String) As String

        Dim out As ClsOut = New ClsOut(Application.StartupPath)

        'If C_PassPhrase.Trim = "" Then
        '    'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): no PassPhrase"
        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): no PassPhrase")
        '    End If

        '    Return New List(Of String)
        'End If

        'Dim Signum As ClsSignumNET = New ClsSignumNET(C_PromptPIN)
        'Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = SenderPublicKey ' ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        'Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))


        Dim postDataRL As String = "requestType=createATProgram"
        postDataRL += "&name=CarbonPFPDEX"
        postDataRL += "&description=PFPAT"
        'postDataRL += "&creationBytes=" + C_ReferenceCreationBytes
        'postDataRL += "&code=" 
        'postDataRL += "&data="
        'postDataRL += "&dpages="
        'postDataRL += "&cspages="
        'postDataRL += "&uspages="
        postDataRL += "&minActivationAmountNQT=100000000"
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey
        postDataRL += "&feeNQT=" + _DeployFeeNQT.ToString
        postDataRL += "&deadline=1440"
        postDataRL += "&referencedTransactionFullHash=" + _ReferenceTXFullHash
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

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in CreateAT(): ->" + vbCrLf + Response
        End If

        Return Response


        'If Response.Contains(Application.ProductName + "-error") Then
        '    'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): -> " + Response
        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): -> " + Response)
        '    End If

        '    Return New List(Of String)
        'End If

        'Dim JSON As ClsJSON = New ClsJSON
        'Dim RespList As Object = JSON.JSONRecursive(Response)

        'Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        'If Error0.GetType.Name = GetType(Boolean).Name Then
        '    'TX OK
        'ElseIf Error0.GetType.Name = GetType(String).Name Then
        '    'TX not OK
        '    'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): " + Response
        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): " + Response)
        '    End If

        '    Return New List(Of String)
        'End If


        ''Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")

        ''Dim TX As String = ""
        ''If UTX.GetType.Name = GetType(String).Name Then
        ''    TX = CStr(UTX)
        ''End If


        ''If Not TX.Trim = "" Then
        ''    Dim Signum As ClsSignumNET = New ClsSignumNET()
        ''    Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX, SenderSignKey)
        ''    TX = BroadcastTransaction(STX.SignedTransaction)
        ''End If

        'Dim TXDetailList As List(Of String) = New List(Of String)

        'For Each Entry In RespList

        '    Select Case Entry(0)
        '        Case "broadcasted"

        '        Case "unsignedTransactionBytes"

        '            TXDetailList.Add("<unsignedTransactionBytes>" + Entry(1) + "</unsignedTransactionBytes>")

        '        Case "transactionJSON"

        '            Dim Type As String = JSON.RecursiveListSearch(Entry(1), "type")
        '            Dim SubType As String = JSON.RecursiveListSearch(Entry(1), "subtype")
        '            Dim Timestamp As String = JSON.RecursiveListSearch(Entry(1), "timestamp")
        '            'Dim Deadline As String = RecursiveSearch(Entry(1), "deadline")
        '            'Dim senderPublicKey As String = RecursiveSearch(Entry(1), "senderPublicKey")
        '            Dim AmountNQT As String = JSON.RecursiveListSearch(Entry(1), "amountNQT")
        '            Dim FeeNQT As String = JSON.RecursiveListSearch(Entry(1), "feeNQT")
        '            'Dim Signature As String = RecursiveSearch(Entry(1), "signature")
        '            'Dim SignatureHash As String = RecursiveSearch(Entry(1), "signatureHash")
        '            'Dim FullHash As String = RecursiveSearch(Entry(1), "fullHash")
        '            'Dim Transaction As String = TX ' RecursiveSearch(Entry(1), "transaction")
        '            'Dim Attachments = RecursiveSearch(Entry(1), "attachment")

        '            Dim Attachments As List(Of Object) = TryCast(JSON.RecursiveListSearch(Entry(1), "attachment"), List(Of Object))
        '            Dim AttStr As String = "<attachment>"
        '            If Not IsNothing(Attachments) Then
        '                For Each Attachment In Attachments
        '                    Dim AttList As List(Of String) = TryCast(Attachment, List(Of String))
        '                    If Not IsNothing(AttList) Then
        '                        If AttList.Count > 1 Then
        '                            AttStr += "<" + AttList(0) + ">" + AttList(1) + "</" + AttList(0) + ">"
        '                        End If
        '                    End If
        '                Next
        '            End If

        '            AttStr += "</attachment>"

        '            'Dim SenderID As String = JSON.RecursiveListSearch(Entry(1), "sender")
        '            'Dim SenderRS As String = JSON.RecursiveListSearch(Entry(1), "senderRS")
        '            'Dim Height As String = JSON.RecursiveListSearch(Entry(1), "height")
        '            'Dim Version As String = JSON.RecursiveListSearch(Entry(1), "version")
        '            'Dim ECBlockID As String = JSON.RecursiveListSearch(Entry(1), "ecBlockId")
        '            'Dim ECBlockHeight As String = JSON.RecursiveListSearch(Entry(1), "ecBlockHeight")


        '            TXDetailList.Add("<type>" + Type + "</type>")
        '            TXDetailList.Add("<subtype>" + SubType + "</subtype>")
        '            TXDetailList.Add("<timestamp>" + Timestamp + "</timestamp>")
        '            'TXDetailList.Add("<deadline>" + Deadline + "</deadline>")
        '            'TXDetailList.Add("<senderPublicKey>" + senderPublicKey + "</senderPublicKey>")
        '            TXDetailList.Add("<amountNQT>" + AmountNQT + "</amountNQT>")
        '            TXDetailList.Add("<feeNQT>" + FeeNQT + "</feeNQT>")
        '            'TXDetailList.Add("<signature>" + Signature + "</signature>")
        '            'TXDetailList.Add("<signatureHash>" + SignatureHash + "</signatureHash>")
        '            'TXDetailList.Add("<fullHash>" + FullHash + "</fullHash>")
        '            'TXDetailList.Add("<transaction>" + Transaction + "</transaction>")
        '            TXDetailList.Add(AttStr)

        '            Exit For

        '        Case "requestProcessingTime"


        '    End Select

        'Next

        'Return TXDetailList

    End Function


    Public Function SetBLSATBuyOrder(ByVal SenderPublicKey As String, ByVal ATID As ULong, ByVal WantToBuyAmount As Double, ByVal Collateral As Double, ByVal Xitem As String, ByVal XAmount As Double, Optional Fee As Double = 0.0) As String

        Dim AmountNQT As ULong = Dbl2Planck(Collateral)
        Dim XAmountNQT As ULong = Dbl2Planck(XAmount)

        'Dim FeeNQT As ULong = Dbl2Planck(Fee)
        Dim ReserveNQT As ULong = Dbl2Planck(WantToBuyAmount)
        'Dim ATDetails = GetATDetails(ATId)

        'Dim Name As String = BetweenFromList(ATDetails, "<name>", "</name>")
        'Dim ATRS As String = BetweenFromList(ATDetails, "<atRS>", "</atRS>")
        'Dim Description As String = BetweenFromList(ATDetails, "<desciption>", "</desciption>")
        'Dim MachineCode As String = BetweenFromList(ATDetails, "<machineCode>", "</machineCode>")
        'Dim MachineData As String = BetweenFromList(ATDetails, "<machineData>", "</machineData>")
        'Dim ParaList As List(Of ULong) = DataStr2ULngList(MachineData)

        '2469808197076732305 CreateOrder method address: 224685783a1b4991 HEX ; 2469808197076732305 DEC     00c012 HEX ; 49170 DEC    00000547 HEX ; 1351 DEC
        '4714436802908501638 AcceptOrder method address: 416d0b4b4963b686 HEX ; 4714436802908501638 DEC     008c12 HEX ; 35858 DEC    000003d0 HEX ; 976  DEC
        '3125596792462301675 FinishOrder method address: 2b6059b8fdd0d9eb HEX ; 3125596792462301675 DEC     006612 HEX ; 26130 DEC    000000cf HEX ; 207  DEC

        Dim ULngList As List(Of ULong) = New List(Of ULong)({Convert.ToUInt64(ReferenceCreateOrder), ReserveNQT, XAmountNQT, String2ULng(Xitem.Trim)})
        Dim MsgStr As String = ULngList2DataStr(ULngList)

        'If Fee = 0.0 Then
        '    Fee = GetTXFee(MsgStr)
        'End If

        Dim Response As String = SendMoney(SenderPublicKey, ATID, Collateral + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        Dim JSON As ClsJSON = New ClsJSON
        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SetBLSATBuyOrder(): ->" + vbCrLf + Response
        End If

        Return Response

    End Function
    Public Function SetBLSATSellOrder(ByVal SenderPublicKey As String, ByVal ATID As ULong, ByVal WantToSellAmount As Double, ByVal Collateral As Double, ByVal Xitem As String, ByVal XAmount As Double, Optional Fee As Double = 0.0) As String

        'Dim AmountNQT As ULong = Dbl2Planck(WantToSellAmount)
        Dim XAmountNQT As ULong = Dbl2Planck(XAmount)

        'Dim FeeNQT As ULong = Dbl2Planck(Fee)
        Dim CollateralNQT As ULong = Dbl2Planck(Collateral)
        'Dim ATDetails = GetATDetails(ATId)

        'Dim Name As String = BetweenFromList(ATDetails, "<name>", "</name>")
        'Dim ATRS As String = BetweenFromList(ATDetails, "<atRS>", "</atRS>")
        'Dim Description As String = BetweenFromList(ATDetails, "<desciption>", "</desciption>")
        'Dim MachineCode As String = BetweenFromList(ATDetails, "<machineCode>", "</machineCode>")
        'Dim MachineData As String = BetweenFromList(ATDetails, "<machineData>", "</machineData>")
        'Dim ParaList As List(Of ULong) = DataStr2ULngList(MachineData)

        '2469808197076732305 CreateOrder method address: 224685783a1b4991 HEX ; 2469808197076732305 DEC     00c012 HEX ; 49170 DEC    00000547 HEX ; 1351 DEC
        '4714436802908501638 AcceptOrder method address: 416d0b4b4963b686 HEX ; 4714436802908501638 DEC     008c12 HEX ; 35858 DEC    000003d0 HEX ; 976  DEC
        '3125596792462301675 FinishOrder method address: 2b6059b8fdd0d9eb HEX ; 3125596792462301675 DEC     006612 HEX ; 26130 DEC    000000cf HEX ; 207  DEC

        Dim ULngList As List(Of ULong) = New List(Of ULong)({Convert.ToUInt64(ReferenceCreateOrder), CollateralNQT, XAmountNQT, String2ULng(Xitem.Trim)})
        Dim MsgStr As String = ULngList2DataStr(ULngList)

        'If Fee = 0.0 Then
        '    Fee = GetTXFee(MsgStr)
        'End If

        Dim Response As String = SendMoney(SenderPublicKey, ATID, WantToSellAmount + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        Dim JSON As ClsJSON = New ClsJSON
        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SetBLSATSellOrder(): ->" + vbCrLf + Response
        End If

        Return Response

    End Function

    Public Function SendMessage2BLSAT(ByVal SenderPublicKeyHEX As String, ByVal ATID As ULong, ByVal Collateral As Double, ByVal ULongMsgList As List(Of ULong), Optional ByVal Fee As Double = 0.0) As String
        Dim MsgStr As String = ULngList2DataStr(ULongMsgList)

        'If Fee = 0.0 Then
        '    Fee = GetTXFee(MsgStr)
        'End If

        Dim Response As String = SendMoney(SenderPublicKeyHEX, ATID, Collateral + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        Dim JSON As ClsJSON = New ClsJSON
        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SetBLSATSellOrder(): ->" + vbCrLf + Response
        End If

        Return Response

    End Function

    'Public Function SendMessage2BLSATManual(ByVal SenderPublicKey As String, ByVal ATID As ULong, ByVal Collateral As Double, ByVal ULongMsgList As List(Of ULong), Optional ByVal Fee As Double = 0.0) As String

    '    If Fee = 0.0 Then
    '        Fee = GetSlotFee()
    '    End If

    '    Dim MsgStr As String = ULngList2DataStr(ULongMsgList)
    '    Dim Response As String = Application.ProductName + "-error in SendMessage2BLSAT(): -> no Keys"

    '    'Dim MAsterkeys As List(Of String) = GetPassPhrase()

    '    'If MAsterkeys.Count > 0 Then
    '    Response = SendMoney(SenderPublicKey, ATID, Collateral + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

    '    'Dim UTXList As List(Of String) = ConvertUnsignedTXToList(Response)
    '    'Dim UTX As String = BetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>",, GetType(String))
    '    'Dim SignumNET As ClsSignumNET = New ClsSignumNET
    '    'Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MAsterkeys(1))
    '    'Dim TX As String = BroadcastTransaction(STX.SignedTransaction)

    '    'UTXList.Add("<transaction>" + TX + "</transaction>")

    '    'Response = TX

    '    'End If

    '    'If Response.Contains(Application.ProductName + "-error") Then
    '    '    Response = Application.ProductName + "-error in SendMessage2BLSAT(): -> " + vbCrLf + Response
    '    'End If

    '    Return Response

    'End Function

#End Region

#End Region 'Blockchain Communication


#Region "Convert tools"

    Public Shared Function ConvertUnsignedTXToList(ByVal UnsignedTX As String) As List(Of String)

        Dim out As ClsOut = New ClsOut(Application.StartupPath)

        Dim JSON As ClsJSON = New ClsJSON
        Dim RespList As Object = JSON.JSONRecursive(UnsignedTX)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                out.ErrorLog2File(Application.ProductName + "-error in ConvertUnsignedTXToList(): " + UnsignedTX)
            End If

            Return New List(Of String)
        End If


        'Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")

        'Dim TX As String = ""
        'If UTX.GetType.Name = GetType(String).Name Then
        '    TX = CStr(UTX)
        'End If


        'If Not TX.Trim = "" Then
        '    Dim Signum As ClsSignumNET = New ClsSignumNET()
        '    Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX, SenderSignKey)
        '    TX = BroadcastTransaction(STX.SignedTransaction)
        'End If

        Dim TXDetailList As List(Of String) = New List(Of String)

        For Each T_Entry In DirectCast(RespList, List(Of Object))

            Dim Entry As List(Of Object) = New List(Of Object)

            If T_Entry.GetType.Name = GetType(List(Of Object)).Name Then
                Entry = DirectCast(T_Entry, List(Of Object))
            End If


            Select Case Entry(0).ToString
                Case "broadcasted"

                Case "unsignedTransactionBytes"

                    TXDetailList.Add("<unsignedTransactionBytes>" + Entry(1).ToString + "</unsignedTransactionBytes>")

                Case "transactionJSON"

                    Dim SubEntry As List(Of Object) = New List(Of Object)

                    If Entry(1).GetType.Name = GetType(List(Of Object)).Name Then
                        SubEntry = DirectCast(Entry(1), List(Of Object))
                    End If

                    Dim Type As String = Convert.ToString(JSON.RecursiveListSearch(SubEntry, "type"))
                    Dim SubType As String = Convert.ToString(JSON.RecursiveListSearch(SubEntry, "subtype"))
                    Dim Timestamp As String = Convert.ToString(JSON.RecursiveListSearch(SubEntry, "timestamp"))
                    'Dim Deadline As String = RecursiveSearch(Entry(1), "deadline")
                    'Dim senderPublicKey As String = RecursiveSearch(Entry(1), "senderPublicKey")
                    Dim AmountNQT As String = Convert.ToString(JSON.RecursiveListSearch(SubEntry, "amountNQT"))
                    Dim FeeNQT As String = Convert.ToString(JSON.RecursiveListSearch(SubEntry, "feeNQT"))
                    'Dim Signature As String = RecursiveSearch(Entry(1), "signature")
                    'Dim SignatureHash As String = RecursiveSearch(Entry(1), "signatureHash")
                    'Dim FullHash As String = RecursiveSearch(Entry(1), "fullHash")
                    'Dim Transaction As String = TX ' RecursiveSearch(Entry(1), "transaction")
                    'Dim Attachments = RecursiveSearch(Entry(1), "attachment")

                    Dim Attachments As List(Of Object) = TryCast(JSON.RecursiveListSearch(SubEntry, "attachment"), List(Of Object))
                    Dim AttStr As String = "<attachment>"
                    If Not IsNothing(Attachments) Then
                        For Each Attachment In Attachments
                            Dim AttList As List(Of String) = TryCast(Attachment, List(Of String))
                            If Not IsNothing(AttList) Then
                                If AttList.Count > 1 Then
                                    AttStr += "<" + AttList(0) + ">" + AttList(1) + "</" + AttList(0) + ">"
                                End If
                            End If
                        Next
                    End If

                    AttStr += "</attachment>"

                    'Dim SenderID As String = JSON.RecursiveListSearch(Entry(1), "sender")
                    'Dim SenderRS As String = JSON.RecursiveListSearch(Entry(1), "senderRS")
                    'Dim Height As String = JSON.RecursiveListSearch(Entry(1), "height")
                    'Dim Version As String = JSON.RecursiveListSearch(Entry(1), "version")
                    'Dim ECBlockID As String = JSON.RecursiveListSearch(Entry(1), "ecBlockId")
                    'Dim ECBlockHeight As String = JSON.RecursiveListSearch(Entry(1), "ecBlockHeight")


                    TXDetailList.Add("<type>" + Type + "</type>")
                    TXDetailList.Add("<subtype>" + SubType + "</subtype>")
                    TXDetailList.Add("<timestamp>" + Timestamp + "</timestamp>")
                    'TXDetailList.Add("<deadline>" + Deadline + "</deadline>")
                    'TXDetailList.Add("<senderPublicKey>" + senderPublicKey + "</senderPublicKey>")
                    TXDetailList.Add("<amountNQT>" + AmountNQT + "</amountNQT>")
                    TXDetailList.Add("<feeNQT>" + FeeNQT + "</feeNQT>")
                    'TXDetailList.Add("<signature>" + Signature + "</signature>")
                    'TXDetailList.Add("<signatureHash>" + SignatureHash + "</signatureHash>")
                    'TXDetailList.Add("<fullHash>" + FullHash + "</fullHash>")
                    'TXDetailList.Add("<transaction>" + Transaction + "</transaction>")
                    TXDetailList.Add(AttStr)

                    Exit For

                Case "requestProcessingTime"


            End Select

        Next

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

        Dim MsgStr As String = System.Text.Encoding.UTF8.GetString(MsgByteList.ToArray)

        MsgStr = MsgStr.Replace(Convert.ToChar(0), "")

        Return MsgStr

    End Function
    Public Shared Function String2ULng(ByVal input As String) As ULong

        Dim ByteAry As List(Of Byte) = System.Text.Encoding.UTF8.GetBytes(input).ToList

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

    Public Shared Function ByteAry2HEX(ByVal BytAry() As Byte) As String

        Dim RetStr As String = ""

        Dim ParaBytes As List(Of Byte) = BytAry.ToList

        For Each ParaByte As Byte In ParaBytes
            Dim T_RetStr As String = Conversion.Hex(ParaByte)

            If T_RetStr.Length < 2 Then
                T_RetStr = "0" + T_RetStr
            End If

            RetStr += T_RetStr

        Next

        Return RetStr.ToLower

    End Function

    Public Shared Function String2HEX(ByVal input As String) As String

        Dim inpLng As ULong = String2ULng(input)

        Return ULng2HEX(inpLng)

    End Function
    Public Shared Function HEXStr2String(ByVal input As String) As String

        Dim RetStr As String = ""
        Dim Ungerade As Integer = input.Length Mod 2

        If Ungerade = 1 Then
            input += "0"
        End If

        For j As Integer = 0 To Convert.ToInt32(input.Length / 2) - 1
            Dim HEXStr As String = input.Substring(j * 2, 2)

            Dim HEXByte As Byte = Convert.ToByte(HEXStr, 16)

            RetStr += Chr(HEXByte)
        Next

        Return RetStr.Replace(Convert.ToChar(0), "")

    End Function


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

            Return RetList

        Catch ex As Exception
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

    Public Shared Function GetSHA256_64(ByVal InputKey As String) As List(Of ULong)

        Dim InputBytes As List(Of Byte) = System.Text.Encoding.ASCII.GetBytes(InputKey).ToList

        For i As Integer = InputBytes.Count To 16
            InputBytes.Add(0)
        Next

        Dim FirstULong As ULong = BitConverter.ToUInt64(InputBytes.ToArray, 0)
        Dim SecondULong As ULong = BitConverter.ToUInt64(InputBytes.ToArray, 8)

        Dim FirstULongBytes As Byte() = BitConverter.GetBytes(FirstULong)
        Dim SecondULongBytes As Byte() = BitConverter.GetBytes(SecondULong)

        Dim ByteList As List(Of Byte) = New List(Of Byte)
        ByteList.AddRange(FirstULongBytes)
        ByteList.AddRange(SecondULongBytes)
        ByteList.AddRange({0, 0, 0, 0, 0, 0, 0, 0})
        ByteList.AddRange({0, 0, 0, 0, 0, 0, 0, 0})

        Dim SHA256 As System.Security.Cryptography.SHA256Managed = New System.Security.Cryptography.SHA256Managed
        Dim Hash As List(Of Byte) = SHA256.ComputeHash(ByteList.ToArray).ToList

        Dim HashULong As ULong = BitConverter.ToUInt64(Hash.ToArray, 0)

        Return New List(Of ULong)({FirstULong, SecondULong, HashULong})

    End Function

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

    Private Structure S_Sorter
        Dim Timestamp As ULong
        Dim TXID As ULong
    End Structure

    Private Function SortTimeStamp(ByVal input As List(Of List(Of String))) As List(Of List(Of String))

        Dim TSSort As List(Of S_Sorter) = New List(Of S_Sorter)

        For i As Integer = 0 To input.Count - 1

            Dim Entry As List(Of String) = input(i)

            Dim T_Timestamp As ULong = GetULongBetweenFromList(Entry, "<timestamp>", "</timestamp>")
            Dim T_Transaction As ULong = GetULongBetweenFromList(Entry, "<transaction>", "</transaction>")

            Dim NuSort As S_Sorter = New S_Sorter
            NuSort.Timestamp = T_Timestamp
            NuSort.TXID = T_Transaction

            TSSort.Add(NuSort)
        Next

        TSSort = TSSort.OrderBy(Function(s) s.Timestamp).ToList

        Dim SReturnList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each sort In TSSort

            For i As Integer = 0 To input.Count - 1
                Dim retent = input(i)

                Dim T_Timestamp As ULong = GetULongBetweenFromList(retent, "<timestamp>", "</timestamp>")
                Dim T_Transaction As ULong = GetULongBetweenFromList(retent, "<transaction>", "</transaction>")

                If T_Timestamp = sort.Timestamp And T_Transaction = sort.TXID Then
                    SReturnList.Add(retent)
                    Exit For
                End If

            Next

        Next

        Return SReturnList

    End Function

#End Region

End Class