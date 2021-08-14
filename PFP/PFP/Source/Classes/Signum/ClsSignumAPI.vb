Imports System.IO
Imports System.Net
Imports System.Text

Public Class ClsSignumAPI

#Region "SC Structure"
    'AT: 11566074467129414744

    'CreateOrder: 716726961670769723

    'AcceptOrder: 4714436802908501638
    'InjectResponder: 9213622959462902524

    'FinishOrder: 3125596792462301675

    'InjectForwardKey: 198257638303395401
    'SetForwardKeyOK: 6162377613759412635
    'FinishOrder2: 6223530244326486469
#End Region

    Public Const _ReferenceTX As String = "11566074467129414744"
    Public Const _DeployFeeNQT As ULong = 154350000UL
    Public Const _GasFeeNQT As ULong = 29400000UL

    Public ReadOnly Property ReferenceCreateOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(716726961670769723L), 0)
    Public ReadOnly Property ReferenceAcceptOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(4714436802908501638L), 0)
    Public ReadOnly Property ReferenceInjectResponder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(9213622959462902524L), 0)
    Public ReadOnly Property ReferenceFinishOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(3125596792462301675L), 0)
    Public ReadOnly Property ReferenceInjectForwardKey As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(198257638303395401L), 0)
    Public ReadOnly Property ReferenceSetForwardKeyOK As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(6162377613759412635L), 0)
    Public ReadOnly Property ReferenceFinishOrder2 As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(6223530244326486469L), 0)

    Private ReadOnly Property C_ReferenceCreationBytes As String
    ReadOnly Property C_ReferenceMachineCode As String


    Property C_Node As String = "http://nivbox.co.uk:6876/burst"

    Property C_PassPhrase As String = ""
    Property C_AccountID As String
    Property C_Address As String


    Property C_UTXList As List(Of List(Of String)) = New List(Of List(Of String))

    Public Property DEXATList As List(Of String) = New List(Of String)


    Sub New(Optional ByVal Node As String = "", Optional ByVal PassPhrase As String = "", Optional ByVal Account As String = "", Optional ByVal ReferenceTX As String = _ReferenceTX)

        If Not Node.Trim = "" Then
            C_Node = Node
        End If

        If Not PassPhrase.Trim = "" Then
            C_PassPhrase = PassPhrase
        End If

        If Not Account.Trim = "" Then
            C_AccountID = Account
        End If


        Dim ReferenceTXDetails As List(Of String) = GetTransaction(ReferenceTX)
        C_ReferenceCreationBytes = BetweenFromList(ReferenceTXDetails, "<creationBytes>", "</creationBytes>")

        Dim ReferenceATDetails = GetATDetails(ReferenceTX)
        C_ReferenceMachineCode = BetweenFromList(ReferenceATDetails, "<machineCode>", "</machineCode>")

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

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in BroadcastTransaction(): " + Response
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(RespList, "transaction")

        Dim Returner As String = ""
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
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in SignumRequest(" + C_Node + "): " + ex.Message
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

            Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
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

            Dim PubKey As String = JSON.RecursiveListSearch(RespList, "publicKey").ToString

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

    Public Function GetAccountFromPassPhrase() As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()
        Dim Response As String = SignumRequest("requestType=getAccountId&publicKey=" + ByteAry2HEX(MasterkeyList(0)).Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response)
            End If

            Return New List(Of String)
        End If


        Dim Account As String = JSON.RecursiveListSearch(RespList, "account").ToString
        Dim AccountRS As String = JSON.RecursiveListSearch(RespList, "accountRS").ToString
        Dim Balance As List(Of String) = GetBalance(Account, AccountRS)

        Return Balance

    End Function

    Public Function GetAccountPublicKeyFromAccountID_RS(ByVal AccountID_RS As String) As String

        Dim Response As String = SignumRequest("requestType=getAccountPublicKey&account=" + AccountID_RS.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): ->" + vbCrLf + Response
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): " + Response
        End If


        Dim PublicKey As Object = JSON.RecursiveListSearch(RespList, "publicKey").ToString

        Dim Returner As String = ""
        If PublicKey.GetType.Name = GetType(String).Name Then
            Returner = CStr(PublicKey)
        End If

        Return Returner

    End Function

    Public Function RSConvert(ByVal Input As String) As List(Of String)

        Try

            If Input.Contains("-") Then

                Input = Input.Substring(Input.IndexOf("-") + 1) 'remove (T)S- Tag

                Dim AccountID As ULong = ClsReedSolomon.Decode(Input)
                Dim x As List(Of String) = New List(Of String)({"<account>" + AccountID.ToString + "</account>", "<accountRS>" + Input + "</accountRS>"})
                Return x
            Else
                Dim AccountRS As String = ClsReedSolomon.Encode(CULng(Input))
                Dim x As List(Of String) = New List(Of String)({"<account>" + Input + "</account>", "<accountRS>TS-" + AccountRS + "</accountRS>"}) 'TODO: Change TS- Tag
                Return x
            End If

        Catch ex As Exception
            Return New List(Of String)({"<account>" + Input + "</account>", "<accountRS>" + Input + "</accountRS>"})
        End Try

    End Function


    Public Function GetBalance(Optional ByVal AccountID As String = "", Optional ByVal Address1 As String = "") As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        If AccountID.Trim = "" Then
            AccountID = C_AccountID
        End If

        If Address1.Trim = "" Then
            Address1 = C_Address
        End If

        Dim CoinBal As List(Of String) = New List(Of String)({"<coin>SIGNA</coin>", "<account>" + AccountID + "</account>", "<address>" + Address1 + "</address>", "<balance>0</balance>", "<available>0</available>", "<pending>0</pending>"})

        Dim Response As String = SignumRequest("requestType=getAccount&account=" + AccountID.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): -> " + Response
            'Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(): -> " + Response)
            Return CoinBal
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): " + Response
            'Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(): " + Response)
            Return CoinBal
        End If


        Dim BalancePlanckStr As String = JSON.RecursiveListSearch(RespList, "balanceNQT").ToString
        Dim Balance As Double = 0.0

        Try
            Balance = CDbl(BalancePlanckStr.Insert(BalancePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim AvailablePlanckStr As String = JSON.RecursiveListSearch(RespList, "unconfirmedBalanceNQT").ToString
        Dim Available As Double = 0.0

        Try
            Available = CDbl(AvailablePlanckStr.Insert(AvailablePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim Pending As Double = Available - Balance

        Dim Address As String = JSON.RecursiveListSearch(RespList, "accountRS").ToString


        '(Coin, Account, Address, Balance, Available, Pending)
        CoinBal(0) = "<coin>SIGNA</coin>"
        CoinBal(1) = "<account>" + AccountID.Trim + "</account>"
        CoinBal(2) = "<address>" + Address.Trim + "</address>"
        CoinBal(3) = "<balance>" + Balance.ToString + "</balance>"
        CoinBal(4) = "<available>" + Available.ToString + "</available>"
        CoinBal(5) = "<pending>" + Pending.ToString + "</pending>"

        Return CoinBal

    End Function

    Public Function GetSlotFee() As Double

        Dim TXList As List(Of List(Of String)) = GetUnconfirmedTransactions()

        Dim SlotFee As Double = 0.00735

        If TXList.Count = 0 Then
            Return SlotFee
        Else
            SlotFee *= (TXList.Count + 1)
        End If

        Return SlotFee

    End Function


    Public Function GetUnconfirmedTransactions() As List(Of List(Of String))

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getUnconfirmedTransactions")

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetUnconfirmedTransactions(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): -> " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response)
            End If

            Return New List(Of List(Of String))
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unconfirmedTransactions")

        Dim EntryList As List(Of Object) = New List(Of Object)

        If UTX.GetType.Name = GetType(String).Name Then
            Return New List(Of List(Of String))
        ElseIf UTX.GetType.Name = GetType(Boolean).Name Then
            Return New List(Of List(Of String))
        Else

            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each Entry In UTX

                If Entry(0) = "type" Then
                    If TempOBJList.Count > 0 Then
                        EntryList.Add(TempOBJList)
                    End If

                    TempOBJList = New List(Of Object)
                    TempOBJList.Add(Entry)
                Else
                    TempOBJList.Add(Entry)
                End If

            Next

            EntryList.Add(TempOBJList)
        End If


        Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))


        For Each entry1 In EntryList

            Dim TempList As List(Of String) = New List(Of String)

            For Each entry In entry1

                Select Case entry(0)
                    Case "type"

                    Case "subtype"

                    Case "timestamp"
                        TempList.Add("<timestamp>" + entry(1) + "</timestamp>")

                    Case "deadline"

                    Case "senderPublicKey"

                    Case "amountNQT"
                        TempList.Add("<amountNQT>" + entry(1) + "</amountNQT>")

                    Case "feeNQT"
                        TempList.Add("<feeNQT>" + entry(1).ToString + "</feeNQT>")

                    Case "signature"

                    Case "signatureHash"

                'Case "balanceNQT"
                '    UTXDetailList.Add("<balanceNQT>" + Entry(1) + "</balanceNQT>")

                    Case "fullHash"

                    Case "transaction"
                        TempList.Add("<transaction>" + entry(1) + "</transaction>")

                    Case "attachment"

                        'Dim Attachments As List(Of Object) = TryCast(entry(1), List(Of Object))

                        'Dim AttStr As String = "<attachment>"

                        'If Not IsNothing(Attachments) Then

                        '    For Each Attachment In Attachments

                        '        Dim AttList As List(Of String) = TryCast(Attachment, List(Of String))
                        '        'TODO: Recursive2XML

                        '        If Not IsNothing(AttList) Then

                        '            If AttList.Count > 1 Then
                        '                AttStr += "<" + AttList(0) + ">" + AttList(1) + "</" + AttList(0) + ">"
                        '            End If

                        '        End If

                        '    Next

                        'End If

                        'AttStr += "</attachment>"


                        Dim TMsg As String = "<attachment>"
                        Dim Message As String = JSON.RecursiveListSearch(entry(1), "message").ToString

                        If Message.Trim <> "False" Then
                            Dim IsText As String = JSON.RecursiveListSearch(entry(1), "messageIsText").ToString
                            TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                        End If

                        Dim EncMessage = JSON.RecursiveListSearch(entry(1), "encryptedMessage")

                        If EncMessage.GetType.Name = GetType(Boolean).Name Then

                        Else

                            Dim Data As String = JSON.RecursiveListSearch(EncMessage, "data")
                            Dim Nonce As String = JSON.RecursiveListSearch(EncMessage, "nonce")
                            Dim IsText As String = JSON.RecursiveListSearch(entry(1), "isText").ToString

                            If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
                                TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                            End If

                        End If

                        TMsg += "</attachment>"

                        TempList.Add(TMsg)

                    Case "sender"
                        TempList.Add("<sender>" + entry(1) + "</sender>")

                    Case "senderRS"
                        TempList.Add("<senderRS>" + entry(1) + "</senderRS>")

                    Case "recipient"
                        TempList.Add("<recipient>" + entry(1) + "</recipient>")

                    Case "recipientRS"
                        TempList.Add("<recipientRS>" + entry(1) + "</recipientRS>")

                    Case "height"
                        TempList.Add("<height>" + entry(1) + "</height>")

                    Case "version"

                    Case "ecBlockId"

                    Case "ecBlockHeight"

                    Case "block"
                        TempList.Add("<block>" + entry(1) + "</block>")

                    Case "confirmations"
                        TempList.Add("<confirmations>" + entry(1) + "</confirmations>")

                    Case "blockTimestamp"

                    Case "requestProcessingTime"

                End Select
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

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): " + Response)
            End If
            Return 0
        End If

        Dim BlockHeightStr As Object = JSON.RecursiveListSearch(RespList, "height")

        Try
            BlockHeightInt = CInt(BlockHeightStr)
        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): -> " + ex.Message)
            End If

            Return 0
        End Try

        Return BlockHeightInt

    End Function

    Public Function GetTransaction(ByVal TXID As String) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getTransaction&transaction=" + TXID)

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(): " + Response)
            End If

            Return New List(Of String)
        End If



        Dim TXDetailList As List(Of String) = New List(Of String)

        For Each Entry In RespList

            Select Case Entry(0)
                Case "type"

                Case "subtype"

                Case "timestamp"
                    TXDetailList.Add("<timestamp>" + Entry(1) + "</timestamp>")

                Case "deadline"

                Case "senderPublicKey"

                Case "amountNQT"
                    TXDetailList.Add("<feeNQT>" + Entry(1) + "</feeNQT>")

                Case "feeNQT"
                    TXDetailList.Add("<feeNQT>" + Entry(1).ToString + "</feeNQT>")

                Case "signature"

                Case "signatureHash"

                Case "balanceNQT"
                    TXDetailList.Add("<balanceNQT>" + Entry(1) + "</balanceNQT>")

                Case "fullHash"

                Case "transaction"
                    TXDetailList.Add("<transaction>" + Entry(1) + "</transaction>")

                Case "attachment"

                    Dim Attachments As List(Of Object) = TryCast(Entry(1), List(Of Object))

                    Dim AttStr As String = "<attachment>"

                    If Not IsNothing(Attachments) Then

                        For Each Attachment In Attachments
                            Dim AttList As List(Of String) = New List(Of String)

                            If Attachment.GetType.Name = GetType(List(Of )).Name Then
                                For Each x In Attachment

                                    If x.GetType.Name = GetType(String).Name Then
                                        AttList.Add(x)
                                    End If
                                Next

                            End If

                            If Not IsNothing(AttList) Then

                                If AttList.Count > 1 Then
                                    AttStr += "<" + AttList(0) + ">" + AttList(1) + "</" + AttList(0) + ">"
                                End If

                            End If

                        Next

                    End If

                    AttStr += "</attachment>"

                    TXDetailList.Add(AttStr)

                Case "sender"
                    TXDetailList.Add("<sender>" + Entry(1) + "</sender>")

                Case "senderRS"
                    TXDetailList.Add("<senderRS>" + Entry(1) + "</senderRS>")

                Case "height"
                    TXDetailList.Add("<height>" + Entry(1) + "</height>")

                Case "version"

                Case "ecBlockId"

                Case "ecBlockHeight"

                Case "block"
                    TXDetailList.Add("<block>" + Entry(1) + "</block>")

                Case "confirmations"
                    TXDetailList.Add("<confirmations>" + Entry(1) + "</confirmations>")

                Case "blockTimestamp"

                Case "requestProcessingTime"

            End Select

        Next

        Return TXDetailList

    End Function

    Public Function GetAccountTransactions(ByVal AccountID As String, Optional ByVal FromTimestamp As String = "", Optional UseBuffer As Boolean = False) As List(Of List(Of String))

        'TODO: Debug GetAccountTransactions() wich indicates injected Responders wrong


        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Request As String = "requestType=getAccountTransactions&account=" + AccountID

        If Not FromTimestamp.Trim = "" Then
            Request += "&timestamp=" + FromTimestamp
        End If

        Dim Response As String = SignumRequest(Request)

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountTransactions(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): -> " + Response)
            End If

            Return New List(Of List(Of String))
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountTransactions(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): " + Response)
            End If

            Return New List(Of List(Of String))
        End If


        Dim EntryList As List(Of Object) = New List(Of Object)

        Dim TX As Object = JSON.RecursiveListSearch(RespList, "transactions")

        If TX.GetType.Name = GetType(String).Name Then
            Return New List(Of List(Of String))
        ElseIf TX.GetType.Name = GetType(Boolean).Name Then
            Return New List(Of List(Of String))
        Else

            Dim TempOBJList As List(Of Object) = New List(Of Object)

            For Each Entry In TX

                If Entry(0) = "type" Then
                    If TempOBJList.Count > 0 Then
                        EntryList.Add(TempOBJList)
                    End If

                    TempOBJList = New List(Of Object)
                    TempOBJList.Add(Entry)
                Else
                    TempOBJList.Add(Entry)
                End If

            Next

            EntryList.Add(TempOBJList)

            Dim ReturnList As List(Of List(Of String)) = New List(Of List(Of String))

            Dim XML As String = ""
            For Each entry1 In EntryList

                XML = JSON.JSONListToXMLRecursive(entry1)

                Dim TempList As List(Of String) = New List(Of String)

                For Each entry In entry1

                    Select Case True
                        Case entry(0) = "type"

                            'If TempList.Count > 0 Then
                            '    ReturnList.Add(TempList)
                            'End If

                            'TempList = New List(Of String)
                            TempList.Add("")

                        Case entry(0) = "timestamp"
                            TempList.Add("<timestamp>" + entry(1) + "</timestamp>")

                        Case entry(0) = "recipient"

                            'Dim T_TX = GetATDetails(entry(1))
                            'Dim IsAT As String = BetweenFromList(T_TX, "<machineCode>", "</machineCode>")

                            'If Not IsAT.Trim = "" Then
                            '    TempList(0) = "<type>BLSTX</type>"
                            'Else

                            'End If

                            TempList.Add("<recipient>" + entry(1) + "</recipient>")

                        Case entry(0) = "recipientRS"
                            TempList.Add("<recipientRS>" + entry(1) + "</recipientRS>")

                        Case entry(0) = "amountNQT"
                            'TempList(0) += "<amountNQT>" + entry(1) + "</amountNQT>"
                            TempList.Add("<amountNQT>" + entry(1) + "</amountNQT>")

                        Case entry(0) = "feeNQT"
                            TempList.Add("<feeNQT>" + entry(1) + "</feeNQT>")

                        Case entry(0) = "transaction"
                            TempList.Add("<transaction>" + entry(1) + "</transaction>")

                        Case entry(0) = "attachment"

                            Dim TMsg As String = "<attachment>"
                            Dim Message As String = JSON.RecursiveListSearch(entry(1), "message").ToString

                            If Message.Trim <> "False" Then
                                Dim IsText As String = JSON.RecursiveListSearch(entry(1), "messageIsText").ToString
                                TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                            End If

                            Dim EncMessage = JSON.RecursiveListSearch(entry(1), "encryptedMessage")

                            If EncMessage.GetType.Name = GetType(Boolean).Name Then

                            Else

                                Dim Data As String = JSON.RecursiveListSearch(EncMessage, "data")
                                Dim Nonce As String = JSON.RecursiveListSearch(EncMessage, "nonce")
                                Dim IsText As String = JSON.RecursiveListSearch(entry(1), "isText").ToString

                                If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
                                    TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                                End If

                            End If

                            TMsg += "</attachment>"

                            TempList.Add(TMsg)

                        Case entry(0) = "sender"

                            'Dim T_TX = GetATDetails(entry(1))
                            'Dim IsAT As String = BetweenFromList(T_TX, "<machineCode>", "</machineCode>")
                            TempList(0) = TempList(0)
                            If IsAT(entry(1), UseBuffer) Then ' Not IsAT.Trim = "" Then
                                TempList(0) = "<type>BLSTX</type>"
                            Else

                            End If

                            TempList.Add("<sender>" + entry(1) + "</sender>")

                        Case entry(0) = "senderRS"
                            TempList.Add("<senderRS>" + entry(1) + "</senderRS>")

                        Case entry(0) = "confirmations"
                            TempList.Add("<confirmations>" + entry(1) + "</confirmations>")

                    End Select
                Next

                ReturnList.Add(TempList)

            Next


            For Each TXEntry As List(Of String) In ReturnList

                If TXEntry.Count = 0 Then
                    Return New List(Of List(Of String))
                End If

                Dim T_SenderAccount As ULong = CULng(BetweenFromList(TXEntry, "<sender>", "</sender>"))

                If TXEntry(0).Trim = "<type>BLSTX</type>" Then
                    ' ist ATTX
                    Dim MSgIdx As Integer = CInt(BetweenFromList(TXEntry, "<message>", "</message>", True))
                    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")

                    If Not T_Message.Trim = "" Then
                        Try

                            Dim TestMsg As String = HEXStr2String(T_Message)

                            If TestMsg.Trim = "accepted" Then

                            ElseIf TestMsg.Trim = "finished" Then

                            Else

                                Dim ULongList = DataStr2ULngList(T_Message)
                                TestMsg = "<method>" + ULongList(0).ToString + "</method><colBuyAmount>" + ULongList(1).ToString + "</colBuyAmount><xAmount>" + ULongList(2).ToString + "</xAmount><xItem>" + ULng2String(ULongList(3)) + "</xItem>"

                            End If


                            TXEntry(MSgIdx) = "<message>" + TestMsg + "</message>"
                        Catch ex As Exception
                            TXEntry(MSgIdx) = "<message></message>"
                        End Try

                    End If
                Else
                    ' ist keine ATTX

                    Dim T_AmountNQT As ULong = CULng(BetweenFromList(TXEntry, "<amountNQT>", "</amountNQT>"))
                    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")
                    Dim MSgIdx As Integer = CInt(BetweenFromList(TXEntry, "<message>", "</message>", True))

                    Dim MesULng As List(Of ULong) = New List(Of ULong)

                    If MSgIdx <> -1 Then
                        MesULng = DataStr2ULngList(T_Message)
                    End If

                    Dim CNT As Integer = MesULng.Count


                    If MesULng.Count = 0 Then
                        If MSgIdx = -1 Then

                        Else
                            TXEntry(MSgIdx) = "<attachment></attachment>"
                        End If

                    Else

                        Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method>"

                        Select Case MesULng(0)
                            Case ReferenceCreateOrder

                                Select Case MesULng.Count
                                    Case 1

                                    Case 2
                                        AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
                                    Case 3
                                        AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
                                        AttMsg += "<xAmount>" + MesULng(2).ToString + "</xAmount>"
                                    Case 4

                                        Dim MSGStr As String = ULng2String(MesULng(3))

                                        AttMsg += "<colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount>"
                                        AttMsg += "<xAmount>" + MesULng(2).ToString + "</xAmount>"
                                        AttMsg += "<xItem>" + MSGStr.Trim + "</xItem>"

                                End Select

                                'AttMsg += "</attachment>"

                                Dim T_Sum As Double = CDbl(T_AmountNQT) - CDbl(MesULng(1))

                                If T_Sum < 0 Then
                                    TXEntry(0) = "<type>BuyOrder</type>"
                                Else
                                    TXEntry(0) = "<type>SellOrder</type>"
                                End If


                            Case ReferenceAcceptOrder
                                TXEntry(0) = "<type>ResponseOrder</type>"


                            Case ReferenceInjectResponder
                                TXEntry(0) = "<type>ResponseOrder</type>"

                                Select Case MesULng.Count
                                    Case 1

                                    Case 2
                                        AttMsg += "<injectedResponser>" + MesULng(1).ToString + "</injectedResponser>"
                                    Case 3

                                    Case 4

                                    Case Else

                                End Select



                            Case ReferenceFinishOrder
                                TXEntry(0) = "<type>ResponseOrder</type>"
                            Case Else
                                TXEntry(0) = "<type>ResponseOrder</type>"

                        End Select

                        AttMsg += "</attachment>"

                        TXEntry(MSgIdx) = AttMsg

                        '    ElseIf MesULng.Count >= 3 Then

                        '    'The Message contains Paymentinfos

                        '    Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method><colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount><xAmount>" + MesULng(2).ToString + "</xAmount></attachment>"

                        '    If MesULng.Count = 4 Then
                        '        Dim MSGStr As String = ULng2String(MesULng(3))
                        '        AttMsg = "<attachment><method>" + MesULng(0).ToString + "</method><colBuyAmount>" + MesULng(1).ToString + "</colBuyAmount><xAmount>" + MesULng(2).ToString + "</xAmount><xItem>" + MSGStr.Trim + "</xItem></attachment>"
                        '    End If

                        '    TXEntry(MSgIdx) = AttMsg



                        'ElseIf MesULng.Count = 2 Then

                        '    'The message contains inject infos



                        'Else
                        '    Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method></attachment>"

                        '    TXEntry(MSgIdx) = AttMsg

                        '    TXEntry(0) = "<type>ResponseOrder</type>"

                    End If

                End If




                'If T_SenderAccount = AccountID Then
                '    TXEntry(0) = "<type>BLSTX</type>"
                '    Dim MSgIdx As Integer = CInt(BetweenFromList(TXEntry, "<message>", "</message>", True))
                '    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")

                '    If Not T_Message.Trim = "" Then
                '        Try
                '            TXEntry(MSgIdx) = "<message>" + HEXStr2String(T_Message) + "</message>"
                '        Catch ex As Exception
                '            TXEntry(MSgIdx) = "<message></message>"
                '        End Try

                '    End If

                'Else

                '    'TempList.Add("<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount></attachment>")

                '    Dim T_AmountNQT As ULong = CULng(BetweenFromList(TXEntry, "<amountNQT>", "</amountNQT>"))
                '    Dim T_Message As String = BetweenFromList(TXEntry, "<message>", "</message>")
                '    Dim MSgIdx As Integer = CInt(BetweenFromList(TXEntry, "<message>", "</message>", True))

                '    Dim MesULng As List(Of ULong) = New List(Of ULong)

                '    If MSgIdx <> -1 Then
                '        MesULng = DataStr2ULngList(T_Message)
                '    End If

                '    If MesULng.Count = 0 Then
                '        If MSgIdx = -1 Then

                '        Else
                '            TXEntry(MSgIdx) = "<attachment></attachment>"
                '        End If

                '    ElseIf MesULng.Count >= 3 Then
                '        Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount></attachment>"

                '        If MesULng.Count = 4 Then
                '            Dim MSGStr As String = ULng2String(MesULng(3))
                '            AttMsg = "<attachment><method>" + MesULng(0).ToString + "</method><resCol>" + MesULng(1).ToString + "</resCol><amount>" + MesULng(2).ToString + "</amount><xitem>" + MSGStr.Trim + "</xitem></attachment>"
                '        End If

                '        TXEntry(MSgIdx) = AttMsg

                '        Dim T_Sum As Double = CDbl(T_AmountNQT) - CDbl(MesULng(1))

                '        If T_Sum < 0 Then
                '            TXEntry(0) = "<type>BuyOrder</type>"
                '        Else
                '            TXEntry(0) = "<type>SellOrder</type>"
                '        End If

                '    Else
                '        Dim AttMsg As String = "<attachment><method>" + MesULng(0).ToString + "</method></attachment>"

                '        TXEntry(MSgIdx) = AttMsg

                '        TXEntry(0) = "<type>ResponseOrder</type>"

                '    End If

                'End If

            Next

            ReturnList = SortTimeStamp(ReturnList)

            Return ReturnList

        End If

        Return New List(Of List(Of String))

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


        For Each RespEntry In RespList

            If RespEntry(0).GetType.Name = GetType(String).Name Then
                If RespEntry(0).trim = "atIds" Then

                    Dim RetList As List(Of String) = New List(Of String)

                    Try
                        RetList = RespEntry(1)(0)
                    Catch ex As Exception

                    End Try

                    Return RetList

                End If

            End If

        Next

        Return New List(Of String)

    End Function
    'Public Function GetATBasics(ByVal ATID As String) As List(Of String)

    '    Dim ATDetails As List(Of String) = GetATDetails(ATID)

    '    Dim MachineCode As String = BetweenFromList(ATDetails, "<machineCode>", "</machineCode>")

    '    If C_ReferenceMachineCode.Trim = MachineCode.Trim Then
    '        ATDetails.Add("<refMachineCode>True</refMachineCode>")
    '        Return ATDetails
    '    Else
    '        ATDetails.Add("<refMachineCode>False</refMachineCode>")
    '        Return ATDetails
    '    End If

    '    Return New List(Of String)

    'End Function

    Public Function GetATDetails(ByVal ATId As String) As List(Of String)

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Dim Response As String = SignumRequest("requestType=getATDetails&at=" + ATId)

        If Response.Contains(Application.ProductName + "-error") Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATDetails(" + ATId + "): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails(" + ATId + "): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATDetails(" + ATId + "): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails(" + ATId + "): " + Response)
            End If

            Return New List(Of String)
        End If



        Dim ATDetailList As List(Of String) = New List(Of String)

        For Each Entry In RespList

            Select Case Entry(0)
                Case "creator"
                    ATDetailList.Add("<creator>" + Entry(1) + "</creator>")
                Case "creatorRS"
                    ATDetailList.Add("<creatorRS>" + Entry(1) + "</creatorRS>")
                Case "at"
                    ATDetailList.Add("<at>" + Entry(1) + "</at>")

                Case "atRS"
                    ATDetailList.Add("<atRS>" + Entry(1) + "</atRS>")

                Case "atVersion"

                Case "name"
                    ATDetailList.Add("<name>" + Entry(1) + "</name>")

                Case "description"
                    ATDetailList.Add("<description>" + Entry(1).ToString + "</description>")

                Case "machineCode"
                    ATDetailList.Add("<machineCode>" + Entry(1) + "</machineCode>")

                    If Not IsNothing(C_ReferenceMachineCode) Then
                        If C_ReferenceMachineCode.Trim = Entry(1).Trim Then
                            ATDetailList.Add("<referenceMachineCode>True</referenceMachineCode>")
                        Else
                            ATDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
                        End If
                    Else
                        ATDetailList.Add("<referenceMachineCode>False</referenceMachineCode>")
                    End If

                Case "machineData"
                    ATDetailList.Add("<machineData>" + Entry(1) + "</machineData>")

                Case "balanceNQT"
                    ATDetailList.Add("<balanceNQT>" + Entry(1) + "</balanceNQT>")

                Case "prevBalanceNQT"

                Case "nextBlock"

                Case "frozen"
                    ATDetailList.Add("<frozen>" + Entry(1) + "</frozen>")

                Case "running"
                    ATDetailList.Add("<running>" + Entry(1) + "</running>")

                Case "stopped"
                    ATDetailList.Add("<stopped>" + Entry(1) + "</stopped>")

                Case "finished"
                    ATDetailList.Add("<finished>" + Entry(1) + "</finished>")

                Case "dead"
                    ATDetailList.Add("<dead>" + Entry(1) + "</dead>")

                Case "minActivation"""

                Case "creationBlock"""

                Case "requestProcessingTime"

            End Select

        Next

        Return ATDetailList

    End Function

#End Region 'Get

#Region "Get Advance"





#End Region


#Region "Send"

    Public Function SendMoney(ByVal RecipientID As String, ByVal Amount As Double, Optional ByVal Fee As Double = 0.0, Optional ByVal Message As String = "", Optional ByVal MessageIsText As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String

        If C_PassPhrase.Trim = "" Then
            Return "error in SendMoney(): no PassPhrase"
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        'Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))


        Dim AmountNQT As String = Dbl2Planck(Amount).ToString

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If

        Dim FeeNQT As String = Dbl2Planck(Fee).ToString

        Dim postDataRL As String = "requestType=sendMoney"
        postDataRL += "&recipient=" + RecipientID
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

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMoney(): ->" + vbCrLf + Response
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)


        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SendMoney(): " + Response
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")

        Dim Returner As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        End If


        If Not Returner.Trim = "" Then
            Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX)
            Returner = BroadcastTransaction(STX.SignedTransaction)
        End If

        Return Returner

    End Function
    Public Function SendMessage(ByVal RecipientID As String, ByVal Message As String, Optional ByVal MessageIsText As Boolean = True, Optional ByVal Encrypt As Boolean = False, Optional ByVal Fee As Double = 0.0, Optional ByVal RecipientPublicKey As String = "") As String

        If C_PassPhrase.Trim = "" Then '.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMessage(): no PassPhrase"
        End If

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If
        Dim FeeNQT As String = Dbl2Planck(Fee).ToString

        If RecipientPublicKey = "" Then
            RecipientPublicKey = GetAccountPublicKeyFromAccountID_RS(RecipientID)
        End If

        If RecipientPublicKey.Trim = "" Or RecipientPublicKey.Trim = "0000000000000000000000000000000000000000000000000000000000000000" Then
            Encrypt = False
            RecipientPublicKey = ""
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        'Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))

        Dim postDataRL As String = "requestType=sendMessage"
        postDataRL += "&recipient=" + RecipientID
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey
        postDataRL += "&feeNQT=" + FeeNQT
        postDataRL += "&deadline=60"
        'postDataRL += "&referencedTransactionFullHash="
        'postDataRL += "&broadcast="

        If Encrypt Then
            ' postDataRL += "&messageToEncrypt=" + Message
            postDataRL += "&messageToEncryptIsText=" + MessageIsText.ToString

            Dim EncryptedMessage_Nonce As String() = Signum.EncryptMessage(Message, RecipientPublicKey, AgreementKey)

            postDataRL += "&encryptedMessageData=" + EncryptedMessage_Nonce(0)
            postDataRL += "&encryptedMessageNonce=" + EncryptedMessage_Nonce(1)

        Else
            postDataRL += "&message=" + Message
            postDataRL += "&messageIsText=" + MessageIsText.ToString
        End If


        'postDataRL += "&messageToEncryptToSelf="
        'postDataRL += "&messageToEncryptToSelfIsText="
        'postDataRL += "&encryptToSelfMessageData="
        'postDataRL += "&encryptToSelfMessageNonce="

        'If Not RecipientPublicKey.Trim = "" Then

        If Not RecipientPublicKey.Trim = "" Then
            postDataRL += " &recipientPublicKey=" + RecipientPublicKey
        End If


        'If Not RecipientPublicKey.Contains(Application.ProductName + "-error") And Not RecipientPublicKey = "False" Then
        '    postDataRL += " &recipientPublicKey=" + RecipientPublicKey
        'End If


        'End If

        Dim Response As String = SignumRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMessage(): ->" + vbCrLf + Response
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SendMessage(): " + Response
        End If

        Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")


        Dim Returner As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        End If

        If Not Returner.Trim = "" Then
            Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX)
            Returner = BroadcastTransaction(STX.SignedTransaction)
        End If

        Return Returner

    End Function

    Public Function ReadMessage(ByVal TX As String) As String

        If C_PassPhrase.Trim = "" Then
            Return Application.ProductName + "-error in ReadMessage(): no PassPhrase"
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))

        Dim postDataRL As String = "requestType=getTransaction&transaction=" + TX
        Dim Response As String = SignumRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + Response
        End If

        Response = Response.Replace("\", "")

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in ReadMessage(): " + Response
        End If


        Dim EncryptedMsg As Object = JSON.RecursiveListSearch(RespList, "encryptedMessage")

        Dim SenderPublicKey As String = JSON.RecursiveListSearch(RespList, "senderPublicKey")
        Dim RecipientPublicKey As String = JSON.RecursiveListSearch(RespList, "recipient")
        RecipientPublicKey = GetAccountPublicKeyFromAccountID_RS(RecipientPublicKey)

        If PublicKey = SenderPublicKey Then
            PublicKey = RecipientPublicKey
        ElseIf PublicKey = RecipientPublicKey Then
            PublicKey = SenderPublicKey
        End If

        Dim ReturnStr As String = ""

        If EncryptedMsg.GetType.Name = GetType(String).Name Then
            ReturnStr = EncryptedMsg
        ElseIf EncryptedMsg.GetType.Name = GetType(Boolean).Name Then

        Else

            Dim Data As String = JSON.RecursiveListSearch(EncryptedMsg, "data")
            Dim Nonce As String = JSON.RecursiveListSearch(EncryptedMsg, "nonce")

            Dim DecryptedMsg As String = Signum.DecryptMessage(Data, Nonce, PublicKey, AgreementKey)

            If DecryptedMsg.Contains(Application.ProductName + "-error") Then
                Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + DecryptedMsg
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

            For Each Entry In input
                ReturnStr += Entry.ToString
            Next

            Return ReturnStr

        Else
            Return input.ToString
        End If


    End Function


    Public Function DecryptFrom(ByVal AccountRS As String, ByVal data As String, ByVal nonce As String, Optional ByVal IsText As Boolean = True) As String

        If C_PassPhrase.Trim = "" Then
            Return Application.ProductName + "-error in DecryptFrom(): no PassPhrase"
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PrivateKey As String = ByteAry2HEX(MasterkeyList(2))
        Dim PublicKey As String = GetAccountPublicKeyFromAccountID_RS(AccountRS)

        Dim DecryptedMsg As String = Signum.DecryptMessage(data, nonce, PublicKey, PrivateKey)

        If DecryptedMsg.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in DecryptFrom(): -> " + vbCrLf + DecryptedMsg
        Else

            If Not MessageIsHEXString(DecryptedMsg) Then
                Return DecryptedMsg
            Else
                Return Application.ProductName + "-error in DecryptFrom(): -> " + vbCrLf + DecryptedMsg
            End If

        End If

    End Function

#End Region 'Send

#Region "Send Advance"

    Public Function CreateAT() As List(Of String)

        Dim out As ClsOut = New ClsOut(Application.StartupPath)

        If C_PassPhrase.Trim = "" Then
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): no PassPhrase"
            If GetINISetting(E_Setting.InfoOut, False) Then
                out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): no PassPhrase")
            End If

            Return New List(Of String)
        End If

        Dim Signum As ClsSignumNET = New ClsSignumNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Signum.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))


        Dim postDataRL As String = "requestType=createATProgram"
        postDataRL += "&name=PFPAT"
        postDataRL += "&description=PFPAT"
        postDataRL += "&creationBytes=" + C_ReferenceCreationBytes
        'postDataRL += "&code=" 
        'postDataRL += "&data="
        'postDataRL += "&dpages="
        'postDataRL += "&cspages="
        'postDataRL += "&uspages="
        postDataRL += "&minActivationAmountNQT=100000000"
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey
        postDataRL += "&feeNQT=" + _DeployFeeNQT.ToString
        postDataRL += "&deadline=60"
        'postDataRL += "&referencedTransactionFullHash="
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
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): -> " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): -> " + Response)
            End If

            Return New List(Of String)
        End If

        Dim JSON As ClsJSON = New ClsJSON

        Dim RespList As Object = JSON.JSONRecursive(Response)

        Dim Error0 As Object = JSON.RecursiveListSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): " + Response
            If GetINISetting(E_Setting.InfoOut, False) Then
                out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): " + Response)
            End If

            Return New List(Of String)
        End If


        Dim UTX As Object = JSON.RecursiveListSearch(RespList, "unsignedTransactionBytes")

        Dim TX As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            TX = CStr(UTX)
        End If


        If Not TX.Trim = "" Then
            Dim STX As ClsSignumNET.S_Signature = Signum.SignHelper(UTX)
            TX = BroadcastTransaction(STX.SignedTransaction)
        End If

        Dim TXDetailList As List(Of String) = New List(Of String)

        For Each Entry In RespList

            Select Case Entry(0)
                Case "transaction"

                Case "fullHash"

                Case "transactionBytes"

                Case "transactionJSON"

                    Dim Type As String = JSON.RecursiveListSearch(Entry(1), "type")
                    Dim SubType As String = JSON.RecursiveListSearch(Entry(1), "subtype")
                    Dim Timestamp As String = JSON.RecursiveListSearch(Entry(1), "timestamp")
                    'Dim Deadline As String = RecursiveSearch(Entry(1), "deadline")
                    'Dim senderPublicKey As String = RecursiveSearch(Entry(1), "senderPublicKey")
                    Dim AmountNQT As String = JSON.RecursiveListSearch(Entry(1), "amountNQT")
                    Dim FeeNQT As String = JSON.RecursiveListSearch(Entry(1), "feeNQT")
                    'Dim Signature As String = RecursiveSearch(Entry(1), "signature")
                    'Dim SignatureHash As String = RecursiveSearch(Entry(1), "signatureHash")
                    'Dim FullHash As String = RecursiveSearch(Entry(1), "fullHash")
                    Dim Transaction As String = TX ' RecursiveSearch(Entry(1), "transaction")
                    'Dim Attachments = RecursiveSearch(Entry(1), "attachment")

                    Dim Attachments As List(Of Object) = TryCast(JSON.RecursiveListSearch(Entry(1), "attachment"), List(Of Object))
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
                    TXDetailList.Add("<transaction>" + Transaction + "</transaction>")
                    TXDetailList.Add(AttStr)

                    Exit For
                Case "requestProcessingTime"


            End Select

        Next

        Return TXDetailList

    End Function


    Public Function SetBLSATBuyOrder(ByVal ATId As String, ByVal WantToBuyAmount As Double, ByVal Collateral As Double, ByVal Xitem As String, ByVal XAmount As Double, Optional Fee As Double = 0.0) As String

        Dim AmountNQT As ULong = Dbl2Planck(Collateral)
        Dim XAmountNQT As ULong = Dbl2Planck(XAmount)

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If

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

        Dim ULngList As List(Of ULong) = New List(Of ULong)({CULng(ReferenceCreateOrder), ReserveNQT, XAmountNQT, String2ULng(Xitem.Trim)})
        Dim MsgStr As String = ULngList2DataStr(ULngList)

        Dim Response As String = SendMoney(ATId, Collateral + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        If Response.Contains(Application.ProductName + "-error") Then
            Response = Application.ProductName + "-error in SetBLSATBuyOrder(): -> " + vbCrLf + Response
        End If

        Return Response

    End Function
    Public Function SetBLSATSellOrder(ByVal ATId As String, ByVal WantToSellAmount As Double, ByVal Collateral As Double, ByVal Xitem As String, ByVal XAmount As Double, Optional Fee As Double = 0.0) As String

        'Dim AmountNQT As ULong = Dbl2Planck(WantToSellAmount)
        Dim XAmountNQT As ULong = Dbl2Planck(XAmount)

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If

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

        Dim ULngList As List(Of ULong) = New List(Of ULong)({CULng(ReferenceCreateOrder), CollateralNQT, XAmountNQT, String2ULng(Xitem.Trim)})
        Dim MsgStr As String = ULngList2DataStr(ULngList)
        Dim Response As String = SendMoney(ATId, WantToSellAmount + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        If Response.Contains(Application.ProductName + "-error") Then
            Response = Application.ProductName + "-error in SetBLSATSellOrder(): -> " + vbCrLf + Response
        End If

        Return Response

    End Function

    Public Function SendMessage2BLSAT(ByVal ATId As String, ByVal Collateral As Double, ByVal ULongMsgList As List(Of ULong), Optional ByVal Fee As Double = 0.0) As String

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If

        Dim MsgStr As String = ULngList2DataStr(ULongMsgList)
        Dim Response As String = ""

        Response = SendMoney(ATId, Collateral + Planck2Dbl(_GasFeeNQT), Fee, MsgStr.Trim, False)

        If Response.Contains(Application.ProductName + "-error") Then
            Response = Application.ProductName + "-error in SendMessage2BLSAT(): -> " + vbCrLf + Response
        End If

        Return Response

    End Function

#End Region

#End Region 'Blockchain Communication


#Region "Convert tools"


    Public Function TimeToUnix(ByVal dteDate As Date) As String
        If dteDate.IsDaylightSavingTime = True Then
            dteDate = DateAdd(DateInterval.Hour, -1, dteDate)
        End If
        Return DateDiff(DateInterval.Second, CDate("11.08.2014 04:00:16"), dteDate)
    End Function


    Public Function UnixToTime(ByVal strUnixTime As String) As Date
        UnixToTime = DateAdd(DateInterval.Second, Val(strUnixTime), CDate("11.08.2014 04:00:16"))
        If UnixToTime.IsDaylightSavingTime = True Then
            UnixToTime = DateAdd(DateInterval.Hour, 1, UnixToTime)
        End If

        Return UnixToTime

    End Function


    Public Function ULng2String(ByVal Lng As ULong) As String

        Dim MsgByteAry() As Byte = BitConverter.GetBytes(Lng)
        Dim MsgByteList As List(Of Byte) = New List(Of Byte)(MsgByteAry)

        Dim MsgStr As String = System.Text.Encoding.UTF8.GetString(MsgByteList.ToArray)

        MsgStr = MsgStr.Replace(Convert.ToChar(0), "")

        Return MsgStr

    End Function
    Public Function String2ULng(ByVal input As String) As ULong

        Dim ByteAry As List(Of Byte) = System.Text.Encoding.UTF8.GetBytes(input).ToList

        For i As Integer = ByteAry.Count To 15
            ByteAry.Add(0)
        Next

        Dim MsgLng As ULong = BitConverter.ToInt64(ByteAry.ToArray, 0)

        Return MsgLng

    End Function

    Public Function ULng2HEX(ByVal ULng As ULong) As String

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

    Public Function ByteAry2HEX(ByVal BytAry() As Byte) As String

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

    Public Function String2HEX(ByVal input As String) As String

        Dim inpLng As ULong = String2ULng(input)

        Return ULng2HEX(inpLng)

    End Function
    Public Function HEXStr2String(ByVal input As String) As String

        Dim RetStr As String = ""
        Dim Ungerade As Integer = input.Length Mod 2

        If Ungerade = 1 Then
            input += "0"
        End If

        For j As Integer = 0 To (input.Length / 2) - 1
            Dim HEXStr As String = input.Substring(j * 2, 2)

            Dim HEXByte As Byte = Convert.ToByte(HEXStr, 16)

            RetStr += Chr(HEXByte)
        Next

        Return RetStr.Replace(Convert.ToChar(0), "")

    End Function


    Public Function DataStr2ULngList(ByVal ParaStr As String) As List(Of ULong)

        Dim RetStr As String = ""
        Dim Ungerade As Integer = ParaStr.Length Mod 16

        While Not Ungerade = 0
            ParaStr += "0"
            Ungerade = ParaStr.Length Mod 16
        End While


        Dim RetList As List(Of ULong) = New List(Of ULong)
        Try

            Dim HowMuchParas As Double = ParaStr.Length / 16

            For i As Integer = 0 To HowMuchParas - 1

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
    Public Function ULngList2DataStr(ByVal ULngList As List(Of ULong)) As String

        Dim RetStr As String = ""

        For Each ULn As ULong In ULngList
            RetStr += ULng2HEX(ULn)
        Next

        Return RetStr.ToLower

    End Function

    Public Function GetSHA256_64(ByVal InputKey As String) As List(Of ULong)

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

        Dim Planck As ULong = Signa * 100000000
        Return Planck

    End Function
    Public Shared Function Planck2Dbl(ByVal Planck As ULong) As Double

        Dim Signa As Double = Planck / 100000000
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

            Dim T_Timestamp As ULong = CULng(BetweenFromList(Entry, "<timestamp>", "</timestamp>"))
            Dim T_Transaction As ULong = CULng(BetweenFromList(Entry, "<transaction>", "</transaction>"))

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

                Dim T_Timestamp As ULong = CULng(BetweenFromList(retent, "<timestamp>", "</timestamp>"))
                Dim T_Transaction As ULong = CULng(BetweenFromList(retent, "<transaction>", "</transaction>"))

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