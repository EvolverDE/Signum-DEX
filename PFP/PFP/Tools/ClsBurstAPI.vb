Imports System.IO
Imports System.Net
Imports System.Text


Public Class ClsBurstAPI

    'AT: 2556199170550828612
    'createOrder: 716726961670769723
    'acceptOrder: 4714436802908501638
    'finishOrder: 3125596792462301675
    'injectResponder: 6956773156128522497

    Public Const _ReferenceTX As String = "5527904978073323480" ' "2758976048878098469" ' "4449312026944639919" '15288802811961789572" '16765061203337282908 '15223937934525417316 '15288802811961789572
    Public ReadOnly Property ReferenceCreateOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(716726961670769723), 0)
    Public ReadOnly Property ReferenceAcceptOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(4714436802908501638), 0)
    Public ReadOnly Property ReferenceFinishOrder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(3125596792462301675), 0)
    Public ReadOnly Property ReferenceInjectResponder As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(6956773156128522497), 0)

    Private ReadOnly Property C_ReferenceCreationBytes As String
    ReadOnly Property C_ReferenceMachineCode As String


    Property C_Node As String = "http://nivbox.co.uk:6876/burst"

    Property C_PassPhrase As String = ""
    Property C_AccountID As String
    Property C_Address As String


    Property C_UTXList As List(Of List(Of String)) = New List(Of List(Of String))


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


        Dim ReferenceTXDetails = GetTransaction(ReferenceTX)
        C_ReferenceCreationBytes = BetweenFromList(ReferenceTXDetails, "<creationBytes>", "</creationBytes>")

        Dim ReferenceATDetails = GetATDetails(ReferenceTX)
        C_ReferenceMachineCode = BetweenFromList(ReferenceATDetails, "<machineCode>", "</machineCode>")

    End Sub


#Region "Blockchain Communication"


#Region "Basic API"

    Function BroadcastTransaction(ByVal TXBytesHexStr As String) As String

        Dim Response As String = BurstRequest("requestType=broadcastTransaction&transactionBytes=" + TXBytesHexStr)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in BroadcastTransaction(): ->" + vbCrLf + Response
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in BroadcastTransaction(): " + Response
        End If


        Dim UTX As Object = RecursiveSearch(RespList, "transaction")

        Dim Returner As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        ElseIf UTX.GetType.Name = GetType(Boolean).Name Then

        ElseIf UTX.GetType.Name = GetType(List(Of )).Name Then

        End If

        Return Returner

    End Function

    Function BurstRequest(ByVal postData As String) As String

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
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in BurstRequest(" + C_Node + "): " + ex.Message
            Return Application.ProductName + "-error in BurstRequest(" + C_Node + "): " + ex.Message
        End Try

    End Function

#End Region


#Region "Get"

    Public Function IsAT(ByVal AccountID As String) As Boolean

        Dim Out As out = New out(Application.StartupPath)



        Dim Response As String = BurstRequest("requestType=getAccount&account=" + AccountID)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsAT(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in IsAT(): -> " + Response)
            Return False
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in IsAT(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in IsAT(): " + Response)
            Return False
        End If

        Dim PubKey As String = RecursiveSearch(RespList, "publicKey").ToString

        If PubKey = "0000000000000000000000000000000000000000000000000000000000000000" Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetAccountFromPassPhrase() As List(Of String)

        Dim Out As out = New out(Application.StartupPath)

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()
        Dim Response As String = BurstRequest("requestType=getAccountId&publicKey=" + ByteAry2HEX(MasterkeyList(0)).Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): -> " + Response)
            Return New List(Of String)
        End If

        Dim RespList As Object = JSONRecursive(Response)


        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountFromPassPhrase(): " + Response)
            Return New List(Of String)
        End If


        Dim Account As String = RecursiveSearch(RespList, "account").ToString
        Dim AccountRS As String = RecursiveSearch(RespList, "accountRS").ToString
        Dim Balance As List(Of String) = GetBalance(Account, AccountRS)

        Return Balance

    End Function

    Public Function GetAccountPublicKeyFromAccountID_RS(ByVal AccountID_RS As String) As String

        Dim Response As String = BurstRequest("requestType=getAccountPublicKey&account=" + AccountID_RS.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): ->" + vbCrLf + Response
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in GetAccountPublicKeyFromAccountID_RS(): " + Response
        End If


        Dim PublicKey As Object = RecursiveSearch(RespList, "publicKey").ToString

        Dim Returner As String = ""
        If PublicKey.GetType.Name = GetType(String).Name Then
            Returner = CStr(PublicKey)
        End If

        Return Returner

    End Function

    Public Function RSConvert(ByVal Input As String) As List(Of String)

        Dim Out As out = New out(Application.StartupPath)

        Dim Response As String = BurstRequest("requestType=rsConvert&account=" + Input.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in RSConvert(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in RSConvert(): -> " + Response)
            Return New List(Of String)
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in RSConvert(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in RSConvert(): " + Response)
            Return New List(Of String)
        End If

        Dim Account As String = RecursiveSearch(RespList, "account").ToString
        Dim AccountRS As String = RecursiveSearch(RespList, "accountRS").ToString

        Return New List(Of String)({"<account>" + Account + "</account>", "<accountRS>" + AccountRS + "</accountRS>"})

    End Function


    Public Function GetBalance(Optional ByVal AccountID As String = "", Optional ByVal Address1 As String = "") As List(Of String)

        Dim Out As out = New out(Application.StartupPath)

        If AccountID.Trim = "" Then
            AccountID = C_AccountID
        End If

        If Address1.Trim = "" Then
            Address1 = C_Address
        End If

        Dim CoinBal As List(Of String) = New List(Of String)({"<coin>BURST</coin>", "<account>" + AccountID + "</account>", "<address>" + Address1 + "</address>", "<balance>0</balance>", "<available>0</available>", "<pending>0</pending>"})

        Dim Response As String = BurstRequest("requestType=getAccount&account=" + AccountID.Trim)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(): -> " + Response)
            Return CoinBal
        End If


        Dim RespList As Object = JSONRecursive(Response)


        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetBalance(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetBalance(): " + Response)
            Return CoinBal
        End If


        Dim BalancePlanckStr As String = RecursiveSearch(RespList, "balanceNQT").ToString
        Dim Balance As Double = 0.0

        Try
            Balance = CDbl(BalancePlanckStr.Insert(BalancePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim AvailablePlanckStr As String = RecursiveSearch(RespList, "unconfirmedBalanceNQT").ToString
        Dim Available As Double = 0.0

        Try
            Available = CDbl(AvailablePlanckStr.Insert(AvailablePlanckStr.Length - 8, ","))
        Catch ex As Exception

        End Try

        Dim Pending As Double = Available - Balance

        Dim Address As String = RecursiveSearch(RespList, "accountRS").ToString


        '(Coin, Account, Address, Balance, Available, Pending)
        CoinBal(0) = "<coin>BURST</coin>"
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

        Dim Out As out = New out(Application.StartupPath)

        Dim Response As String = BurstRequest("requestType=getUnconfirmedTransactions")

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetUnconfirmedTransactions(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): -> " + Response)
            Return New List(Of List(Of String))
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetUnconfirmedTransactions(): " + Response)
            Return New List(Of List(Of String))
        End If


        Dim UTX As Object = RecursiveSearch(RespList, "unconfirmedTransactions")

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
                        Dim Message As String = RecursiveSearch(entry(1), "message").ToString

                        If Message.Trim <> "False" Then
                            Dim IsText As String = RecursiveSearch(entry(1), "messageIsText").ToString
                            TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                        End If

                        Dim EncMessage = RecursiveSearch(entry(1), "encryptedMessage")

                        If EncMessage.GetType.Name = GetType(Boolean).Name Then

                        Else

                            Dim Data As String = RecursiveSearch(EncMessage, "data")
                            Dim Nonce As String = RecursiveSearch(EncMessage, "nonce")
                            Dim IsText As String = RecursiveSearch(entry(1), "isText").ToString

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

        Dim Out As out = New out(Application.StartupPath)

        Dim BlockHeightInt As Integer = 0

        Dim Response As String = BurstRequest("requestType=getMiningInfo")

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetCurrentBlock(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): -> " + Response)
            Return 0
        End If


        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetCurrentBlock(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): " + Response)
            Return 0
        End If

        Dim BlockHeightStr As Object = RecursiveSearch(RespList, "height")

        Try
            BlockHeightInt = CInt(BlockHeightStr)
        Catch ex As Exception
            Out.ErrorLog2File(Application.ProductName + "-error in GetCurrentBlock(): -> " + ex.Message)
            Return 0
        End Try

        Return BlockHeightInt

    End Function

    Public Function GetTransaction(ByVal TXID As String) As List(Of String)

        Dim Out As out = New out(Application.StartupPath)

        Dim Response As String = BurstRequest("requestType=getTransaction&transaction=" + TXID)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(): -> " + Response)
            Return New List(Of String)
        End If

        Dim RespList As Object = JSONRecursive(Response)



        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetTransaction(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetTransaction(): " + Response)
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

                            Dim AttList As List(Of String) = TryCast(Attachment, List(Of String))

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

    Public Function GetAccountTransactions(ByVal AccountID As String, Optional ByVal FromTimestamp As String = "") As List(Of List(Of String))

        Dim Out As out = New out(Application.StartupPath)

        Dim Request As String = "requestType=getAccountTransactions&account=" + AccountID

        If Not FromTimestamp.Trim = "" Then
            Request += "&timestamp=" + FromTimestamp
        End If

        Dim Response As String = BurstRequest(Request)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountTransactions(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): -> " + Response)
            Return New List(Of List(Of String))
        End If

        Dim RespList As Object = JSONRecursive(Response)


        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetAccountTransactions(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetAccountTransactions(): " + Response)
            Return New List(Of List(Of String))
        End If



        Dim EntryList As List(Of Object) = New List(Of Object)

        Dim TX As Object = RecursiveSearch(RespList, "transactions")

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



            For Each entry1 In EntryList

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
                            Dim Message As String = RecursiveSearch(entry(1), "message").ToString

                            If Message.Trim <> "False" Then
                                Dim IsText As String = RecursiveSearch(entry(1), "messageIsText").ToString
                                TMsg += "<message>" + Message + "</message><isText>" + IsText + "</isText>"
                            End If

                            Dim EncMessage = RecursiveSearch(entry(1), "encryptedMessage")

                            If EncMessage.GetType.Name = GetType(Boolean).Name Then

                            Else

                                Dim Data As String = RecursiveSearch(EncMessage, "data")
                                Dim Nonce As String = RecursiveSearch(EncMessage, "nonce")
                                Dim IsText As String = RecursiveSearch(entry(1), "isText").ToString

                                If Not Data.Trim = "False" And Not Nonce.Trim = "False" Then
                                    TMsg += "<data>" + Data + "</data><nonce>" + Nonce + "</nonce><isText>" + IsText + "</isText>"
                                End If

                            End If

                            TMsg += "</attachment>"

                            TempList.Add(TMsg)

                        Case entry(0) = "sender"

                            'Dim T_TX = GetATDetails(entry(1))
                            'Dim IsAT As String = BetweenFromList(T_TX, "<machineCode>", "</machineCode>")

                            If IsAT(entry(1)) Then ' Not IsAT.Trim = "" Then
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

        Dim Out As out = New out(Application.StartupPath)

        Dim Response As String = BurstRequest("requestType=getATIds")

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATIds(): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetATIds(): -> " + Response)
            Return New List(Of String)
        End If

        Dim RespList As List(Of Object) = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATIds(): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetATIds(): " + Response)
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

        Dim Out As out = New out(Application.StartupPath)

        Dim Response As String = BurstRequest("requestType=getATDetails&at=" + ATId)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATDetails(" + ATId + "): -> " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails(" + ATId + "): -> " + Response)
            Return New List(Of String)
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in GetATDetails(" + ATId + "): " + Response
            Out.ErrorLog2File(Application.ProductName + "-error in GetATDetails(" + ATId + "): " + Response)
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

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()

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

        Dim Response As String = BurstRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMoney(): ->" + vbCrLf + Response
        End If

        Dim RespList As Object = JSONRecursive(Response)


        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SendMoney(): " + Response
        End If


        Dim UTX As Object = RecursiveSearch(RespList, "unsignedTransactionBytes")

        Dim Returner As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        End If


        If Not Returner.Trim = "" Then
            Dim STX As BurstNET.S_Signature = Burst.SignHelper(UTX)
            Returner = BroadcastTransaction(STX.SignedTransaction)
        End If

        Return Returner

    End Function
    Public Function SendMessage(ByVal RecipientID As String, ByVal Message As String, Optional ByVal MessageIsText As Boolean = True, Optional ByVal Encrypt As Boolean = False, Optional ByVal Fee As Double = 0.0) As String

        If C_PassPhrase.Trim = "" Then '.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMessage(): no PassPhrase"
        End If

        If Fee = 0.0 Then
            Fee = GetSlotFee()
        End If
        Dim FeeNQT As String = Dbl2Planck(Fee).ToString

        Dim RecipientPublicKey As String = GetAccountPublicKeyFromAccountID_RS(RecipientID)

        If RecipientPublicKey.Trim = "" Then
            Encrypt = False
        End If

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()

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

            Dim EncryptedMessage_Nonce As String() = Burst.EncryptMessage(Message, RecipientPublicKey, AgreementKey)

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
        postDataRL += " &recipientPublicKey=" + RecipientPublicKey
        'End If

        Dim Response As String = BurstRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in SendMessage(): ->" + vbCrLf + Response
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in SendMessage(): " + Response
        End If

        Dim UTX As Object = RecursiveSearch(RespList, "unsignedTransactionBytes")


        Dim Returner As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            Returner = CStr(UTX)
        End If

        If Not Returner.Trim = "" Then
            Dim STX As BurstNET.S_Signature = Burst.SignHelper(UTX)
            Returner = BroadcastTransaction(STX.SignedTransaction)
        End If

        Return Returner

    End Function

    Public Function ReadMessage(ByVal TX As String) As String

        If C_PassPhrase.Trim = "" Then
            Return Application.ProductName + "-error in ReadMessage(): no PassPhrase"
        End If

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))

        Dim postDataRL As String = "requestType=getTransaction&transaction=" + TX
        Dim Response As String = BurstRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + Response
        End If

        Response = Response.Replace("\", "")

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in ReadMessage(): " + Response
        End If


        Dim EncryptedMsg As Object = RecursiveSearch(RespList, "encryptedMessage")

        Dim SenderPublicKey As String = RecursiveSearch(RespList, "senderPublicKey")
        Dim RecipientPublicKey As String = RecursiveSearch(RespList, "recipient")
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

            Dim Data As String = RecursiveSearch(EncryptedMsg, "data")
            Dim Nonce As String = RecursiveSearch(EncryptedMsg, "nonce")

            Dim DecryptedMsg As String = Burst.DecryptMessage(Data, Nonce, PublicKey, AgreementKey)

            If DecryptedMsg.Contains(Application.ProductName + "-error") Then
                Return Application.ProductName + "-error in ReadMessage(): -> " + vbCrLf + DecryptedMsg
            End If

            RespList = JSONRecursive(DecryptedMsg)

            Dim AT As String = RecursiveSearch(RespList, "at")
            Dim FTX As String = RecursiveSearch(RespList, "tx")
            Dim PPEM As String = RecursiveSearch(RespList, "ppem")
            Dim PPOrder As String = RecursiveSearch(RespList, "ppodr")
            Dim Info As String = ConvertList2String(RecursiveSearch(RespList, "info"))

            If AT.Trim = "False" Or FTX.Trim = "False" Then

            Else
                ReturnStr = "<at>" + AT + "</at><tx>" + FTX + "</tx>"
                If PPEM.Trim <> "False" Then
                    ReturnStr += "<ppem>" + PPEM + "</ppem>"
                End If
                If PPOrder.Trim <> "False" Then
                    ReturnStr += "<ppodr>" + PPOrder + "</ppodr>"
                End If
                If Info.Trim <> "False" Then
                    ReturnStr += "<info>" + Info + "</info>"
                End If

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

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()

        Dim PrivateKey As String = ByteAry2HEX(MasterkeyList(2))
        Dim PublicKey As String = GetAccountPublicKeyFromAccountID_RS(AccountRS)

        Dim DecryptedMsg As String = Burst.DecryptMessage(data, nonce, PublicKey, PrivateKey)

        If DecryptedMsg.Contains(Application.ProductName + "-error") Then
            Return Application.ProductName + "-error in DecryptFrom(): -> " + vbCrLf + DecryptedMsg
        End If

        Dim DecryptedMsgList As Object = JSONRecursive(DecryptedMsg)

        Dim Error0 As Object = RecursiveSearch(DecryptedMsgList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            Return Application.ProductName + "-error in DecryptFrom(): " + DecryptedMsgList
        End If

        Dim ReturnStr As String = ""

        Dim AT As String = RecursiveSearch(DecryptedMsgList, "at")
        Dim FTX As String = RecursiveSearch(DecryptedMsgList, "tx")
        Dim PPEM As String = RecursiveSearch(DecryptedMsgList, "ppem")
        Dim PPOrder As String = RecursiveSearch(DecryptedMsgList, "ppodr")
        Dim Info As String = RecursiveSearch(DecryptedMsgList, "info")

        If AT.Trim = "False" Or FTX.Trim = "False" Then

        Else
            ReturnStr = "<at>" + AT + "</at><tx>" + FTX + "</tx>"
            If PPEM.Trim <> "False" Then
                ReturnStr += "<ppem>" + PPEM + "</ppem>"
            End If
            If PPOrder.Trim <> "False" Then
                ReturnStr += "<ppodr>" + PPOrder + "</ppodr>"
            End If
            If Info.Trim <> "False" Then
                ReturnStr += "<info>" + Info + "</info>"
            End If

        End If

        Return ReturnStr

    End Function

#End Region 'Send

#Region "Send Advance"

    Public Function CreateAT() As List(Of String)

        Dim out As out = New out(Application.StartupPath)

        If C_PassPhrase.Trim = "" Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): no PassPhrase"

            out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): no PassPhrase")
            Return New List(Of String)
        End If

        Dim Burst As BurstNET = New BurstNET(C_PassPhrase)
        Dim MasterkeyList As List(Of Byte()) = Burst.GenerateMasterKeys()

        Dim PublicKey As String = ByteAry2HEX(MasterkeyList(0))
        Dim SignKey As String = ByteAry2HEX(MasterkeyList(1))
        Dim AgreementKey As String = ByteAry2HEX(MasterkeyList(2))


        Dim postDataRL As String = "requestType=createATProgram"
        postDataRL += "&name=BLS"
        postDataRL += "&description=BLS2WLS"
        postDataRL += "&creationBytes=" + C_ReferenceCreationBytes
        'postDataRL += "&code=" 
        'postDataRL += "&data="
        'postDataRL += "&dpages="
        'postDataRL += "&cspages="
        'postDataRL += "&uspages="
        postDataRL += "&minActivationAmountNQT=100000000"
        'postDataRL += "&secretPhrase=" + C_PassPhrase
        postDataRL += "&publicKey=" + PublicKey
        postDataRL += "&feeNQT=200000000" ' + Dbl2Planck(GetSlotFee()).ToString
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


        Dim Response As String = BurstRequest(postDataRL)

        If Response.Contains(Application.ProductName + "-error") Then
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): -> " + Response

            out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): -> " + Response)

            Return New List(Of String)
        End If

        Dim RespList As Object = JSONRecursive(Response)

        Dim Error0 As Object = RecursiveSearch(RespList, "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK
        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            PFPForm.StatusLabel.Text = Application.ProductName + "-error in CreateAT(): " + Response

            out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): " + Response)

            Return New List(Of String)
        End If


        Dim UTX As Object = RecursiveSearch(RespList, "unsignedTransactionBytes")

        Dim TX As String = ""
        If UTX.GetType.Name = GetType(String).Name Then
            TX = CStr(UTX)
        End If


        If Not TX.Trim = "" Then
            Dim STX As BurstNET.S_Signature = Burst.SignHelper(UTX)
            TX = BroadcastTransaction(STX.SignedTransaction)
        End If

        Dim TXDetailList As List(Of String) = New List(Of String)

        For Each Entry In RespList

            Select Case Entry(0)
                Case "transaction"

                Case "fullHash"

                Case "transactionBytes"

                Case "transactionJSON"

                    Dim Type As String = RecursiveSearch(Entry(1), "type")
                    Dim SubType As String = RecursiveSearch(Entry(1), "subtype")
                    Dim Timestamp As String = RecursiveSearch(Entry(1), "timestamp")
                    'Dim Deadline As String = RecursiveSearch(Entry(1), "deadline")
                    'Dim senderPublicKey As String = RecursiveSearch(Entry(1), "senderPublicKey")
                    Dim AmountNQT As String = RecursiveSearch(Entry(1), "amountNQT")
                    Dim FeeNQT As String = RecursiveSearch(Entry(1), "feeNQT")
                    'Dim Signature As String = RecursiveSearch(Entry(1), "signature")
                    'Dim SignatureHash As String = RecursiveSearch(Entry(1), "signatureHash")
                    'Dim FullHash As String = RecursiveSearch(Entry(1), "fullHash")
                    Dim Transaction As String = TX ' RecursiveSearch(Entry(1), "transaction")
                    'Dim Attachments = RecursiveSearch(Entry(1), "attachment")

                    Dim Attachments As List(Of Object) = TryCast(RecursiveSearch(Entry(1), "attachment"), List(Of Object))
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

        Dim Response As String = SendMoney(ATId, Collateral, Fee, MsgStr.Trim, False)

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
        Dim Response As String = SendMoney(ATId, WantToSellAmount, Fee, MsgStr.Trim, False)

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
        Dim Response As String = SendMoney(ATId, Collateral, Fee, MsgStr.Trim, False)

        If Response.Contains(Application.ProductName + "-error") Then
            Response = Application.ProductName + "-error in SendMessage2BLSAT(): -> " + vbCrLf + Response
        End If

        Return Response

    End Function

#End Region

#End Region 'Blockchain Communication


#Region "Convert tools"

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


    Public Function Dbl2Planck(ByVal Burst As Double) As ULong

        Dim Planck As ULong = Burst * 100000000
        Return Planck

    End Function
    Public Function Planck2Dbl(ByVal Planck As ULong) As Double

        Dim Burst As Double = Planck / 100000000
        Return Burst

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


    Function BetweenFromList(ByVal inputList As List(Of String), Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetListIndex As Boolean = False) As String

        For i As Integer = 0 To inputList.Count - 1
            Dim Entry As String = inputList(i)
            If Entry.Contains(startchar) Then

                If GetListIndex Then
                    Return i.ToString
                Else
                    Return Between(Entry, startchar, endchar, GetType(String))
                End If

            End If

        Next

        If GetListIndex Then
            Return "-1"
        Else
            Return ""
        End If

    End Function
    Function Between(ByVal input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")", Optional ByVal GetTyp As Object = Nothing) As Object

        If input.Trim <> "" Then
            If input.Contains(startchar) And input.Contains(endchar) Then

                input = input.Substring(input.IndexOf(startchar) + startchar.Length)
                input = input.Remove(input.IndexOf(endchar))

                If IsNothing(GetTyp) Then
                    Return input
                Else
                    Select Case GetTyp.Name
                        Case GetType(Integer).Name
                            Return CInt(input)
                        Case GetType(Double).Name
                            Return CDbl(input.Replace(".", ","))
                        Case GetType(Date).Name
                            Return CDate(input)
                        Case GetType(String).Name
                            Return input
                    End Select
                End If

            End If
        End If

        If IsNothing(GetTyp) Then
            Return 0.0
        Else
            Select Case GetTyp.Name
                Case GetType(Double).Name
                    Return 0.0
                Case GetType(String).Name
                    Return ""
                Case Else
                    Return 0
            End Select
        End If

    End Function
    Function Between2List(ByVal Input As String, Optional ByVal startchar As String = "(", Optional ByVal endchar As String = ")") As List(Of String) ', Optional ByVal LastIdx As Boolean = False) As List(Of String)

        Dim Output As String = ""
        Dim Rest As String = ""
        Dim Temp1 As String = ""
        Dim Temp2 As String = ""

        Try

            If Input.Trim <> "" Then
                If Input.Contains(startchar) And Input.Contains(endchar) Then


                    Dim StartIdx As Integer = -1
                    Dim EndIdx As Integer = -1
                    Dim CntIdx As Integer = 0


                    Dim StartList As List(Of Integer) = New List(Of Integer)
                    Dim EndList As List(Of Integer) = New List(Of Integer)


                    Dim FirstCheck As String = Input.Substring(0, 2)
                    If FirstCheck = startchar & startchar Then
                        StartList.Add(0)
                        StartList.Add(1)

                        StartIdx = 3
                    End If

                    Dim LastCheck As String = Input.Substring(Input.Length - 2)
                    If LastCheck = endchar & endchar Then
                        EndList.Add(Input.Length - 2)
                        EndList.Add(Input.Length - 1)

                        EndIdx = 3
                    End If


                    FirstCheck = Input.Substring(0, 1)
                    If FirstCheck = startchar Then
                        StartList.Add(0)
                        StartIdx = 2
                    End If

                    'LastCheck = Input.Substring(Input.Length - 1)
                    'If LastCheck = endchar Then
                    '    EndList.Add(Input.Length - 1)
                    '    EndIdx = 2
                    'End If


                    While Not StartIdx = 0 Or Not EndIdx = 0

                        If Not StartIdx = 0 Then

                            If StartIdx = -1 Then StartIdx = 1

                            StartIdx = InStr(StartIdx, Input, startchar)

                            If StartIdx > 0 Then
                                If Not StartIdx = 0 Then
                                    StartList.Add(StartIdx - 1)
                                    StartIdx += 1
                                End If
                            End If
                        End If


                        If Not EndIdx = 0 Then

                            If EndIdx = -1 Then EndIdx = 1

                            EndIdx = InStr(EndIdx, Input, endchar)

                            If EndIdx > 0 Then
                                If Not EndIdx = 0 Then
                                    EndList.Add(EndIdx - 1)
                                    EndIdx += 1
                                End If
                            End If
                        End If

                    End While

                    Dim FinalList As List(Of Integer) = New List(Of Integer)

                    FinalList.AddRange(StartList.ToArray)
                    FinalList.AddRange(EndList.ToArray)

                    FinalList.Sort()

                    Dim SplitIdx As Integer = -1

                    For Each Idx As Integer In FinalList

                        Dim Ch As String = Input.Substring(Idx, 1)

                        If Ch = startchar Then
                            CntIdx += 1
                        ElseIf Ch = endchar Then
                            CntIdx -= 1
                        End If

                        If CntIdx = 0 Then
                            SplitIdx = Idx
                            Exit For
                        End If

                    Next

                    Temp1 = Input.Remove(SplitIdx)
                    Temp1 = Temp1.Remove(FinalList(0), 1)



                    Try
                        Temp2 = Input.Replace(Temp1, "")
                    Catch ex As Exception

                    End Try

                    Output = Temp1
                    Rest = Temp2

                    'Dim Temp As String = ""
                    'Dim Ruecksetz As Boolean = False
                    'Dim Aufnahme As Boolean = False
                    'Dim StartMax As Integer = 0


                    'For Each ch As String In Input

                    '    If startchar.Contains(ch) Then
                    '        Temp += ch
                    '        Ruecksetz = False
                    '    Else
                    '        If Not Ruecksetz Then
                    '            Ruecksetz = True
                    '            Temp = ""
                    '        End If
                    '    End If

                    '    If Temp = startchar Then
                    '        StartMax += 1
                    '        Temp = ""
                    '        If StartMax = 1 Then
                    '            Continue For
                    '        End If

                    '    End If



                    '    If endchar.Contains(ch) Then
                    '        Temp += ch
                    '        Ruecksetz = False
                    '    Else
                    '        If Not Ruecksetz Then
                    '            Ruecksetz = True
                    '            Temp = ""
                    '        End If
                    '    End If

                    '    If Temp = endchar Then
                    '        StartMax -= 1
                    '        Temp = ""
                    '        If StartMax = 0 Then
                    '            Exit For
                    '        End If

                    '    End If



                    '    If StartMax > 0 Then
                    '        Output += ch
                    '    End If

                    'Next





                    'Output = Input.Substring(Input.IndexOf(startchar) + startchar.Length)
                    Try
                        Rest = Input.Replace(Output, "")
                    Catch ex As Exception

                    End Try


                    'If LastIdx Then
                    '    Output = Output.Remove(Output.LastIndexOf(endchar))
                    'Else
                    '    Output = Output.Remove(Output.IndexOf(endchar))
                    'End If

                    If Output.Trim = "" Then
                        Return New List(Of String)({Output, Rest})
                        'Return New List(Of String)({Input.Replace(startchar, "").Replace(endchar, ""), ""})
                    Else

                        'Rest = Input.Replace(Output, "")


                        Return New List(Of String)({Output, Rest})
                        'Return New List(Of String)({Output, Input.Replace(Output, "")})
                    End If


                End If
            End If

        Catch ex As Exception
            Return New List(Of String)
        End Try

        Return New List(Of String)

    End Function


    Private Property RekursivRest() As List(Of Object) = New List(Of Object)

    Function JSONRecursive(ByVal Input As String) As List(Of Object)

        If Input.Length > 0 Then

            Dim U_List As List(Of Object) = New List(Of Object)

            If Input(0) = """" Then

                If Input.Contains(":") Then

                    Dim Prop As String = Input.Remove(Input.IndexOf(":"))
                    Dim Val As String = Input.Substring(Input.IndexOf(":") + 1)

                    If Input.Length > 0 Then
                        If Val(0) = "{" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RekursivRest.Count = 0 Then

                                For Each RekuRest In RekursivRest
                                    U_List.Add(RekuRest)
                                Next
                                RekursivRest = New List(Of Object)
                            End If

                        ElseIf Val(0) = "[" Then

                            U_List.Add(New List(Of Object)({Prop.Replace("""", ""), JSONRecursive(Val)}))

                            If Not RekursivRest.Count = 0 Then

                                For Each RekuRest In RekursivRest
                                    U_List.Add(RekuRest)
                                Next
                                RekursivRest = New List(Of Object)
                            End If

                        Else

                            Dim Brackedfound As String = ""

                            For Each Cha In Input
                                Brackedfound = Cha
                                If Brackedfound = "{" Or Brackedfound = "[" Then
                                    Exit For
                                End If

                            Next


                            If Brackedfound = "{" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("{"))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("{"))

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of String)(TL.Replace("""", "").Split(":")))
                                    End If
                                Next


                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("{},") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If


                                If Not RekursivRest.Count = 0 Then

                                    For Each RekuRest In RekursivRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RekursivRest = New List(Of Object)
                                End If

                            ElseIf Brackedfound = "[" Then

                                Dim T_Vals As String = Input.Remove(Input.IndexOf("["))
                                Dim T_Rest As String = Input.Substring(Input.IndexOf("["))

                                Dim T_List As List(Of String) = T_Vals.Split(",").ToList
                                Dim Key As String = ""

                                For i As Integer = 0 To T_List.Count - 1
                                    Dim TL As String = T_List(i)
                                    If i = T_List.Count - 1 Then
                                        Key = TL.Split(":")(0).Replace("""", "")
                                    Else
                                        U_List.Add(New List(Of String)(TL.Replace("""", "").Split(":")))
                                    End If
                                Next



                                Dim EmptyEntry As Boolean = False
                                If T_Rest.Contains("[],") Then
                                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                                    U_List.Add(New List(Of Object)({Key, New List(Of Object)}))
                                    EmptyEntry = True
                                End If

                                If EmptyEntry Then
                                    Dim T_Entrys As List(Of Object) = JSONRecursive(T_Rest)

                                    For Each T_Entry In T_Entrys
                                        U_List.Add(T_Entry)
                                    Next

                                Else
                                    U_List.Add(New List(Of Object)({Key, JSONRecursive(T_Rest)}))
                                End If



                                If Not RekursivRest.Count = 0 Then

                                    For Each RekuRest In RekursivRest
                                        U_List.Add(RekuRest)
                                    Next
                                    RekursivRest = New List(Of Object)
                                End If

                            Else
                                Dim T_List As List(Of String) = Input.Split(",").ToList

                                For Each TL As String In T_List
                                    U_List.Add(New List(Of String)(TL.Replace("""", "").Split(":")))
                                Next

                                Return U_List

                            End If

                        End If
                    End If

                Else
                    If Input.Contains(",") Then
                        U_List.Add(Input.Replace("""", "").Split(",").ToList)
                    Else
                        U_List.Add(Input.Replace("""", ""))
                    End If

                End If


            ElseIf Input(0) = "{" Then

                Dim T_Input As List(Of String) = Between2List(Input, "{", "}")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If


                If Input(0) = "," Then
                    Input = Input.Substring(1)
                End If

                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next

                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("{},") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("{},") + 3)

                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RekursivRest.Add(Rest)
                    Next

                End If


            ElseIf Input(0) = "[" Then


                Dim T_Input As List(Of String) = Between2List(Input, "[", "]")

                If T_Input.Count > 0 Then
                    Input = T_Input(0)
                End If

                If Not Input.Trim = "" Then
                    If Input(0) = "," Then
                        Input = Input.Substring(1)
                    End If
                End If


                Dim SubList As List(Of Object) = JSONRecursive(Input)

                For Each SubListItem In SubList
                    U_List.Add(SubListItem)
                Next


                If Not RekursivRest.Count = 0 Then

                    For Each RekuRest In RekursivRest
                        U_List.Add(RekuRest)
                    Next
                    RekursivRest = New List(Of Object)
                End If

                Dim T_Rest As String = T_Input(1)

                If T_Rest.Contains("[],") Then
                    T_Rest = T_Rest.Substring(T_Rest.IndexOf("[],") + 3)
                    Dim RestRek As List(Of Object) = JSONRecursive(T_Rest)

                    For Each Rest In RestRek
                        RekursivRest.Add(Rest)
                    Next

                End If


            End If


            Return U_List

        Else
            Return New List(Of Object)
        End If


        '{
        '	"result":
        '	{
        '		"txid":"5f17e18c0e0363fc3a14dd6c2e74141d6f0e636d0592b48d94d0a6272dcb4ef3",
        '		"hash":"188bf760059888f9304321cfb771586b71ef50e1b4f988fdba8db3b405864c5d",
        '		"version":2,
        '		"size":222,
        '		"vsize":141,
        '		"weight":561,
        '		"locktime":1746616,
        '		"vin":
        '		[
        '			{
        '				"txid":"9f30228774543ac5c5ba07a56e26e6079ae1c9221fa3eccb7798596cbb401711",
        '				"vout":0,
        '				"scriptSig":
        '				{
        '					"asm":"",
        '					"hex":""
        '				},
        '				"txinwitness":
        '				[
        '					"30440220225eef02bf7d4f9b655b61a96e8095f9b5f7e482748dbb8d26d642eee64977f70220566064db332acd0577a8918826c03842ce1e6eb19ee8ba3c2fef9a1b6d89cefe01",
        '					"031abc3d54d4d388dd81f154395566397911ff0e6a6e6317d2579d195416332a3d"
        '				],
        '				"sequence":4294967294
        '			}
        '		],
        '		"vout":
        '		[
        '			{
        '				"value":0.01198846,
        '				"n":0,
        '				"scriptPubKey":
        '				{
        '					"asm":"0 b0f27c0a32d29689acf19940c62fade458e5edb1",
        '					"hex":"0014b0f27c0a32d29689acf19940c62fade458e5edb1",
        '					"reqSigs":1,
        '					"type":"witness_v0_keyhash",
        '					"addresses":
        '					[
        '						"tb1qkre8cz3j62tgnt83n9qvvtadu3vwtmd3fzx5vd"
        '					]
        '				}
        '			},
        '			{
        '				"value":0.00500000,
        '				"n":1,
        '				"scriptPubKey":
        '				{
        '					"asm":"0 0f0f5f15ce92234399a10469a2ca7f890eccc2cc",
        '					"hex":"00140f0f5f15ce92234399a10469a2ca7f890eccc2cc",
        '					"reqSigs":1,
        '					"type":"witness_v0_keyhash",
        '					"addresses":
        '					[
        '						"tb1qpu8479wwjg358xdpq3569jnl3y8veskvwqugjt"
        '					]
        '				}
        '			}
        '		]
        '	},
        '	"error":null,
        '    "id":   1
        '}



    End Function
    Function RecursiveSearch(ByVal List As Object, ByVal Key As String) As Object

        Dim Returner As Object = False

        Try

            For Each Entry In List

                If Entry.GetType.Name = GetType(String).Name Then

                    If Entry.ToLower.trim = Key.ToLower.Trim Then

                        Dim FindList As List(Of Object) = New List(Of Object)

                        For i As Integer = 1 To List.count - 1
                            FindList.Add(List(i))
                        Next

                        If FindList.Count > 1 Then
                            Return FindList
                        Else
                            Return FindList(0)
                        End If

                    End If

                Else
                    Returner = RecursiveSearch(Entry, Key)

                    If Not Returner.GetType.Name = GetType(Boolean).Name Then
                        Return Returner
                    End If

                End If

            Next

        Catch ex As Exception

        End Try

        Return Returner

    End Function

#End Region

End Class