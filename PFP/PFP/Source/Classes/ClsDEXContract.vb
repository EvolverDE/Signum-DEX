Option Strict On
Option Explicit On

Public Class ClsDEXContract

#Region "Properties"

    Private C_Node As String = ""
    Public Property Node() As String
        Get
            Return C_Node
        End Get
        Set(value As String)
            C_Node = value
        End Set
    End Property

    Private ReadOnly C_DEXContract As Boolean = False
    ReadOnly Property IsDEXContract() As Boolean
        Get
            Return C_DEXContract
        End Get
    End Property

    Private C_IsReady As Boolean = False
    ReadOnly Property IsReady() As Boolean
        Get
            Return C_IsReady
        End Get
    End Property


    Private C_ContractTimestamp As ULong = 0UL
    ReadOnly Property ContractTimestamp() As ULong
        Get
            Return C_ContractTimestamp
        End Get
    End Property

    Private ReadOnly C_ID As ULong = 0UL
    ReadOnly Property ID() As ULong
        Get
            Return C_ID
        End Get
    End Property

    Private ReadOnly C_Address As String = ""
    ReadOnly Property Address() As String
        Get
            Return C_Address
        End Get
    End Property

    Private ReadOnly C_Name As String = ""
    ReadOnly Property Name() As String
        Get
            Return C_Name
        End Get
    End Property

    Private ReadOnly C_Description As String = ""
    ReadOnly Property Description() As String
        Get
            Return C_Description
        End Get
    End Property

    Private ReadOnly C_CreatorID As ULong = 0UL
    ReadOnly Property CreatorID() As ULong
        Get
            Return C_CreatorID
        End Get
    End Property

    Private ReadOnly C_CreatorAddress As String = ""
    ReadOnly Property CreatorAddress() As String
        Get
            Return C_CreatorAddress
        End Get
    End Property


    Private C_CurrentCreationTransaction As ULong
    ReadOnly Property CurrentCreationTransaction As ULong
        Get
            Return C_CurrentCreationTransaction
        End Get
    End Property

    Private C_CurrentConfirmations As ULong
    ReadOnly Property CurrentConfirmations As ULong
        Get
            Return C_CurrentConfirmations
        End Get
    End Property

    Property C_CurrentTimestamp As ULong = 0UL
    ReadOnly Property CurrentTimestamp() As ULong
        Get
            Return C_CurrentTimestamp
        End Get
    End Property

    Private C_CurrentInitiatorID As ULong = 0UL
    ReadOnly Property CurrentInitiatorID() As ULong
        Get
            Return C_CurrentInitiatorID
        End Get
    End Property

    Private C_CurrentInitiatorAddress As String = ""
    ReadOnly Property CurrentInitiatorAddress() As String
        Get
            Return C_CurrentInitiatorAddress
        End Get
    End Property

    Private C_CurrentResponderID As ULong = 0UL
    ReadOnly Property CurrentResponderID() As ULong
        Get
            Return C_CurrentResponderID
        End Get
    End Property

    Private C_CurrentResponderAddress As String = ""
    ReadOnly Property CurrentResponserAddress() As String
        Get
            Return C_CurrentResponderAddress
        End Get
    End Property

    Private C_IsSellOrder As Boolean = False
    ReadOnly Property IsSellOrder() As Boolean
        Get
            Return C_IsSellOrder
        End Get
    End Property

    Private C_CurrentSellerID As ULong = 0UL
    ReadOnly Property CurrentSellerID() As ULong
        Get
            Return C_CurrentSellerID
        End Get
    End Property

    Private C_CurrentSellerAddress As String = ""
    ReadOnly Property CurrentSellerAddress() As String
        Get
            Return C_CurrentSellerAddress
        End Get
    End Property

    Private C_CurrentBuyerID As ULong = 0UL
    ReadOnly Property CurrentBuyerID() As ULong
        Get
            Return C_CurrentBuyerID
        End Get
    End Property

    Private C_CurrentBuyerAddress As String = ""
    ReadOnly Property CurrentBuyerAddress() As String
        Get
            Return C_CurrentBuyerAddress
        End Get
    End Property


    Private C_CurrentBalance As Double = 0.0
    ReadOnly Property CurrentBalance() As Double
        Get
            Return C_CurrentBalance
        End Get
    End Property

    Private C_CurrentInitiatorsCollateral As Double = 0.0
    ReadOnly Property CurrentInitiatorsCollateral() As Double
        Get
            Return C_CurrentInitiatorsCollateral
        End Get
    End Property

    Private C_CurrentRespondersCollateral As Double = 0.0
    ReadOnly Property CurrentRespondersCollateral() As Double
        Get
            Return C_CurrentRespondersCollateral
        End Get
    End Property

    Private C_CurrentBuySellAmount As Double = 0.0
    ReadOnly Property CurrentBuySellAmount() As Double
        Get
            Return C_CurrentBuySellAmount
        End Get
    End Property

    Private C_CurrentXAmount As Double = 0.0
    ReadOnly Property CurrentXAmount() As Double
        Get
            Return C_CurrentXAmount
        End Get
    End Property

    Private C_CurrentPrice As Double = 0.0
    ReadOnly Property CurrentPrice As Double
        Get
            Return C_CurrentPrice
        End Get
    End Property

    Private C_CurrentXItem As String = ""
    ReadOnly Property CurrentXItem() As String
        Get
            Return C_CurrentXItem
        End Get
    End Property

    Private C_CurrentChainSwapHash As ULong = 0UL
    ReadOnly Property CurrentChainSwapHash() As ULong
        Get
            Return C_CurrentChainSwapHash
        End Get
    End Property

    Private C_CurrentChat As List(Of S_Chat) = New List(Of S_Chat)
    ReadOnly Property CurrentChat() As List(Of S_Chat)
        Get
            Return C_CurrentChat
        End Get
    End Property

    Private C_ContractOrderHistoryList As List(Of S_Order) = New List(Of S_Order)
    ReadOnly Property ContractOrderHistoryList() As List(Of S_Order)
        Get
            Return C_ContractOrderHistoryList
        End Get
    End Property


    Private C_Status As E_Status = E_Status.NEW_
    ReadOnly Property Status() As E_Status
        Get
            Return C_Status
        End Get
    End Property
    Private ReadOnly C_Message As String = ""
    ReadOnly Property Message() As String
        Get
            Return C_Message
        End Get
    End Property


    Private C_IsFrozen As Boolean = False
    ReadOnly Property IsFrozen() As Boolean
        Get
            Return C_IsFrozen
        End Get
    End Property
    Private C_IsRunning As Boolean = False
    ReadOnly Property IsRunning() As Boolean
        Get
            Return C_IsRunning
        End Get
    End Property
    Private C_IsStopped As Boolean = False
    ReadOnly Property IsStopped() As Boolean
        Get
            Return C_IsStopped
        End Get
    End Property
    Private C_IsFinished As Boolean = False
    ReadOnly Property IsFinished() As Boolean
        Get
            Return C_IsFinished
        End Get
    End Property
    Private C_IsDead As Boolean = False
    ReadOnly Property IsDead() As Boolean
        Get
            Return C_IsDead
        End Get
    End Property


#End Region

#Region "Structures"
    Public Structure S_Chat
        Dim SenderAddress As String
        Dim RecipientAddress As String

        Dim Attachment As String
        Dim Timestamp As ULong
    End Structure
    Public Structure S_Order

        Dim CreationTransaction As ULong
        Dim LastTransaction As ULong
        Dim Confirmations As ULong

        Dim WasSellOrder As Boolean

        Dim StartTimestamp As ULong
        Dim EndTimestamp As ULong

        'Dim StartDate As Date
        'Dim EndDate As Date

        Dim SellerRS As String
        Dim SellerID As ULong

        Dim BuyerRS As String
        Dim BuyerID As ULong

        Dim Amount As Double
        Dim Collateral As Double

        Dim XAmount As Double
        Dim XItem As String

        Dim Price As Double

        Dim Status As E_Status
        'Dim Message As String

    End Structure
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

#End Region

#Region "Enumerables"

    Public Enum E_Status
        NEW_ = 0
        FREE = 1

        OPEN = 2
        RESERVED = 3
        CLOSED = 4
        CANCELED = 5
        UTX_PENDING = 6
        TX_PENDING = 7
        ERROR_ = 8

    End Enum

#End Region


    Dim C_StartForm As PFPForm

    ''' <summary>
    ''' Loads the entire DEXContract with its HistoryOrders
    ''' </summary>
    ''' <param name="Node">The API node to get the info</param>
    ''' <param name="ContractID">The Contract ID</param>
    ''' <param name="StartDateTime">The start date from which the history is loaded</param>
    Sub New(ByVal StartForm As PFPForm, ByVal Node As String, ByVal ContractID As ULong, Optional ByVal StartDateTime As Date = #01/01/0001#)

        C_StartForm = StartForm

        If Node.Trim = "" Then
            C_Node = GetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)
        Else
            C_Node = Node
        End If

        C_ID = ContractID

        If Not StartDateTime = #01/01/0001# Then
            C_ContractTimestamp = ClsSignumAPI.TimeToUnix(StartDateTime)
        End If

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_Node)
        Dim ContractList As List(Of String) = SignumAPI.GetATDetails(C_ID)

        If ContractList.Count > 0 Then
            C_Address = GetStringBetweenFromList(ContractList, "<atRS>", "</atRS>")
            C_DEXContract = GetBooleanBetweenFromList(ContractList, "<referenceMachineCode>", "</referenceMachineCode>")

            If C_DEXContract Then

                C_CreatorID = GetULongBetweenFromList(ContractList, "<creator>", "</creator>")
                C_CreatorAddress = GetStringBetweenFromList(ContractList, "<creatorRS>", "</creatorRS>")
                C_Name = GetStringBetweenFromList(ContractList, "<name>", "</name>")
                C_Description = GetStringBetweenFromList(ContractList, "<description>", "</description>")

                Dim CreationTXList As List(Of String) = SignumAPI.GetTransaction(C_ID)

                If StartDateTime = #01/01/0001# Then
                    C_ContractTimestamp = GetULongBetweenFromList(CreationTXList, "<timestamp>", "</timestamp>")
                End If

            End If
        Else
            C_DEXContract = False
        End If

        'LoadUpTransactions(C_ContractTimestamp)
        Refresh()

    End Sub

    Sub New(ByVal StartForm As PFPForm, ByVal Node As String, ByVal ContractID As ULong, ByVal HistoryOrders As List(Of S_Order))

        C_StartForm = StartForm

        If Node.Trim = "" Then
            C_Node = GetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)
        Else
            C_Node = Node
        End If

        C_ID = ContractID

        'If HistoryOrders.Count > 0 Then
        C_ContractOrderHistoryList = New List(Of S_Order)(HistoryOrders.ToArray)
        '    Dim LastOrder As S_Order = HistoryOrders(HistoryOrders.Count - 1)
        '    C_ContractTimestamp = LastOrder.EndTimestamp
        'Else

        'End If

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_Node)
        Dim ContractList As List(Of String) = SignumAPI.GetATDetails(C_ID)

        If ContractList.Count > 0 Then
            C_Address = GetStringBetweenFromList(ContractList, "<atRS>", "</atRS>")
            C_DEXContract = GetBooleanBetweenFromList(ContractList, "<referenceMachineCode>", "</referenceMachineCode>")

            If C_DEXContract Then

                C_CreatorID = GetULongBetweenFromList(ContractList, "<creator>", "</creator>")
                C_CreatorAddress = GetStringBetweenFromList(ContractList, "<creatorRS>", "</creatorRS>")
                C_Name = GetStringBetweenFromList(ContractList, "<name>", "</name>")
                C_Description = GetStringBetweenFromList(ContractList, "<description>", "</description>")

                Dim CreationTXList As List(Of String) = SignumAPI.GetTransaction(C_ID)
                C_ContractTimestamp = GetULongBetweenFromList(CreationTXList, "<timestamp>", "</timestamp>")

            End If
        Else
            C_DEXContract = False
        End If

        'LoadUpTransactions(C_ContractTimestamp)
        Refresh()

    End Sub

    Private Sub LoadUpTransactions(ByVal SetStartTimeStamp As ULong)

        Dim SignumAPI = New ClsSignumAPI(C_Node)

        Dim T_ContractTransactionsRAWPieceList As List(Of String) = SignumAPI.GetAccountTransactionsRAWList(C_ID, SetStartTimeStamp)
        Dim T_ContractTransactionsRAWList As List(Of String) = New List(Of String)(T_ContractTransactionsRAWPieceList.ToArray)

        Dim W500 As Integer = T_ContractTransactionsRAWPieceList.Count
        While W500 >= 500

            C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "LoadHistoryTransactions(" + C_ID.ToString + "): " + W500.ToString)

            T_ContractTransactionsRAWPieceList = SignumAPI.GetAccountTransactionsRAWList(C_ID, SetStartTimeStamp, Convert.ToUInt64(W500))

            Dim T_W500 As Integer = T_ContractTransactionsRAWPieceList.Count

            If T_W500 >= 500 Then
                W500 += 500
            Else
                W500 = 0
            End If

            T_ContractTransactionsRAWList.AddRange(T_ContractTransactionsRAWPieceList.ToArray)

        End While

        Dim C_ThreadList As List(Of Threading.Thread) = New List(Of Threading.Thread)

        C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "ProcessingHistoryTransactions(" + C_ID.ToString + ")")

        For i As Integer = 0 To T_ContractTransactionsRAWList.Count - 1

            Dim T_JSONStr As String = T_ContractTransactionsRAWList(i)

            Dim C_Thread As Threading.Thread = New Threading.Thread(AddressOf ConvertThread)
            C_Thread.Start(T_JSONStr)
            C_ThreadList.Add(C_Thread)

        Next

        Dim SubThreadsFinished As Boolean = False

        While Not SubThreadsFinished
            SubThreadsFinished = True

            For Each T_Thread As Threading.Thread In C_ThreadList

                If T_Thread.IsAlive Then
                    SubThreadsFinished = False
                    Application.DoEvents()
                    Exit For
                End If

            Next

        End While


        Dim T_TXList As List(Of S_TX) = New List(Of S_TX)

        For Each TX As List(Of String) In XMLList
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

        XMLList.Clear()

        If T_TXList.Count > 0 Then

            T_TXList = T_TXList.OrderBy(Function(T_TX As S_TX) T_TX.Timestamp).ToList

            Dim T_LastTX As S_TX = T_TXList(T_TXList.Count - 1)

            If T_LastTX.Recipient = C_ID Then
                If T_LastTX.Confirmations < 2 Then

                    If CurrentInitiatorID <> 0UL And CurrentResponderID <> 0UL Then
                        If T_LastTX.Sender = CurrentInitiatorID Or T_LastTX.Sender = CurrentResponderID Then
                            C_Status = E_Status.TX_PENDING
                        End If
                    Else
                        C_Status = E_Status.TX_PENDING
                    End If

                End If
            End If

            Dim T_ContractOrderHistoryList As List(Of S_Order) = New List(Of S_Order)

            For i As Integer = 0 To T_TXList.Count - 1

                Dim T_TX As S_TX = T_TXList(i)

                C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "ProcessingHistoryTransactions(" + C_ID.ToString + ") (" + i.ToString + "/" + T_TXList.Count.ToString + ")")

                If T_TX.Sender = C_ID Then

                    'LastTX = False

                    Dim ReferenceTXIDs As String = GetStringBetween(T_TX.Attachment, "<message>", "</message>")
                    If Not ReferenceTXIDs.Trim = "" Then

                        Dim ReferenceTXIDList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceTXIDs)

                        If Not ReferenceTXIDList(0) = 0UL And Not ReferenceTXIDList(1) = 0UL And Not ReferenceTXIDList(2) = 0UL Then
                            'referenceTXID

                            Dim T_CreationTX As S_TX = Nothing
                            Dim T_AcceptTX As S_TX = Nothing
                            Dim T_FinishTX As S_TX = Nothing

                            For Each R_TX As S_TX In T_TXList
                                Select Case R_TX.Transaction
                                    Case ReferenceTXIDList(0)
                                        T_CreationTX = R_TX
                                    Case ReferenceTXIDList(1)
                                        T_AcceptTX = R_TX
                                    Case ReferenceTXIDList(2)
                                        T_FinishTX = R_TX
                                End Select

                                If R_TX.Transaction = ReferenceTXIDList(1) And ReferenceTXIDList(1) = ReferenceTXIDList(2) Then
                                    T_AcceptTX = R_TX
                                    T_FinishTX = R_TX
                                End If

                                If Not T_CreationTX.Transaction = 0UL And Not T_AcceptTX.Transaction = 0UL And Not T_FinishTX.Transaction = 0UL Then
                                    Exit For
                                End If

                            Next

                            If Not T_CreationTX.Transaction = 0UL And Not T_AcceptTX.Transaction = 0UL And Not T_FinishTX.Transaction = 0UL Then

                                Dim ReferenceCreationMessage As String = GetStringBetween(T_CreationTX.Attachment, "<message>", "</message>")
                                Dim ReferenceCreationMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceCreationMessage)

                                Dim ReferenceAcceptMessage As String = GetStringBetween(T_AcceptTX.Attachment, "<message>", "</message>")
                                Dim ReferenceAcceptMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceAcceptMessage)

                                Dim ReferenceFinishMessage As String = GetStringBetween(T_FinishTX.Attachment, "<message>", "</message>")
                                Dim ReferenceFinishMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceFinishMessage)

                                If SignumAPI.ReferenceCreateOrder = ReferenceCreationMessageList(0) And (SignumAPI.ReferenceAcceptOrder = ReferenceAcceptMessageList(0) Or SignumAPI.ReferenceInjectResponder = ReferenceAcceptMessageList(0)) And ((SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Or SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0)) Or (ReferenceAcceptMessageList(0) = ReferenceFinishMessageList(0) And T_AcceptTX.Transaction = T_FinishTX.Transaction)) Then

                                    Dim T_Order As S_Order = New S_Order

                                    T_Order.CreationTransaction = T_CreationTX.Transaction
                                    T_Order.LastTransaction = T_FinishTX.Transaction

                                    T_Order.Confirmations = T_CreationTX.Confirmations

                                    T_Order.StartTimestamp = T_CreationTX.Timestamp
                                    T_Order.EndTimestamp = T_TX.Timestamp + 1UL

                                    'T_Order.StartDate = ClsSignumAPI.UnixToTime(T_Order.StartTimestamp.ToString) 'Debug
                                    'T_Order.EndDate = ClsSignumAPI.UnixToTime(T_Order.EndTimestamp.ToString) 'Debug

                                    Dim T_Collateral As ULong = ReferenceCreationMessageList(1)
                                    Dim T_Amount As ULong = T_CreationTX.AmountNQT '- T_Collateral - ClsSignumAPI._GasFeeNQT

                                    If T_Amount > T_Collateral Then

                                        '(0) ULong   Creation Method SellOrder
                                        '(1) ULong   Collateral
                                        '(2) ULong   XAmountNQT
                                        '(3) ULong   XItem USD

                                        T_Order.WasSellOrder = True

                                        T_Order.SellerID = T_CreationTX.Sender
                                        T_Order.SellerRS = T_CreationTX.SenderRS
                                        T_Order.BuyerID = T_AcceptTX.Sender
                                        T_Order.BuyerRS = T_AcceptTX.SenderRS

                                        T_Order.Amount = ClsSignumAPI.Planck2Dbl(T_Amount - T_Collateral - ClsSignumAPI._GasFeeNQT)
                                        T_Order.Collateral = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(1))

                                        T_Order.XAmount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(2))
                                        T_Order.XItem = ClsSignumAPI.ULng2String(ReferenceCreationMessageList(3))

                                        T_Order.Price = T_Order.XAmount / T_Order.Amount

                                        Dim Finisher As ULong = T_FinishTX.Sender

                                        If SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Then
                                            If Finisher = T_Order.SellerID Then
                                                T_Order.Status = E_Status.CLOSED
                                            Else 'If Finisher = o.BuyerID Then
                                                T_Order.Status = E_Status.CANCELED
                                            End If
                                        ElseIf SignumAPI.ReferenceAcceptOrder = ReferenceFinishMessageList(0) Then
                                            'initiator canceled Order
                                            T_Order.Status = E_Status.CANCELED
                                        ElseIf SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0) Then
                                            T_Order.Status = E_Status.CLOSED
                                        End If


                                    Else

                                        '(0) ULong   Creation Method BuyOrder
                                        '(1) ULong   WantToBuyAmount
                                        '(2) ULong   XAmountNQT
                                        '(3) ULong   XItem USD

                                        T_Order.WasSellOrder = False

                                        T_Order.BuyerID = T_CreationTX.Sender
                                        T_Order.BuyerRS = T_CreationTX.SenderRS
                                        T_Order.SellerID = T_AcceptTX.Sender
                                        T_Order.SellerRS = T_AcceptTX.SenderRS

                                        T_Order.Amount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(1))
                                        T_Order.Collateral = ClsSignumAPI.Planck2Dbl(T_Amount - ClsSignumAPI._GasFeeNQT)

                                        T_Order.XAmount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(2))
                                        T_Order.XItem = ClsSignumAPI.ULng2String(ReferenceCreationMessageList(3))

                                        T_Order.Price = T_Order.XAmount / T_Order.Amount

                                        Dim Finisher As ULong = T_FinishTX.Sender

                                        If SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Then
                                            If Finisher = T_Order.SellerID Then
                                                T_Order.Status = E_Status.CLOSED
                                            Else 'If Finisher = o.BuyerID Then
                                                T_Order.Status = E_Status.CANCELED
                                            End If
                                        ElseIf SignumAPI.ReferenceAcceptOrder = ReferenceFinishMessageList(0) Then
                                            'initiator canceled Order
                                            T_Order.Status = E_Status.CANCELED
                                        ElseIf SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0) Then
                                            T_Order.Status = E_Status.CLOSED
                                        End If

                                    End If

                                    T_ContractOrderHistoryList.Add(T_Order)

                                End If

                            End If

                        End If

                    End If

                End If

            Next

            'If SetStartTimeStamp = 0UL Then
            C_ContractOrderHistoryList = T_ContractOrderHistoryList.OrderBy(Function(T_TX As S_Order) T_TX.StartTimestamp).ToList
            'Else
            '    T_ContractOrderHistoryList = T_ContractOrderHistoryList.OrderBy(Function(T_TX As S_Order) T_TX.StartTimestamp).ToList
            '    C_ContractOrderHistoryList.AddRange(T_ContractOrderHistoryList.ToArray)
            'End If

        Else

        End If

        C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "")

    End Sub
    Dim XMLList As List(Of List(Of String)) = New List(Of List(Of String))
    Private Sub ConvertThread(ByVal Input As Object)

        Dim JSONStr As String = Convert.ToString(Input)

        Dim JSON As ClsJSON = New ClsJSON
        Dim RespList As Object = JSON.JSONRecursive(JSONStr)

        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")
        If Error0.GetType.Name = GetType(Boolean).Name Then
            'TX OK

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

            XMLList.Add(TempList)

        ElseIf Error0.GetType.Name = GetType(String).Name Then
            'TX not OK
            'If GetINISetting(E_Setting.InfoOut, False) Then
            '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            '    Out.ErrorLog2File(Application.ProductName + "-error in LoadUpTransactions() -> ConvertThread(): " + JSONStr)
            'End If

        End If

    End Sub

    ''' <summary>
    ''' Refreshing DEXContract
    ''' </summary>
    Sub Refresh(Optional ByVal RefreshHistoryOrders As Boolean = True)

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_Node)
        Dim ContractList As List(Of String) = SignumAPI.GetATDetails(C_ID)

        'C_ID = BetweenFromList(ContractList, "<at>", "</at>",, GetType(ULong))
        'C_Address = BetweenFromList(ContractList, "<atRS>", "</atRS>",, GetType(String))
        'C_DEXContract = BetweenFromList(ContractList, "<referenceMachineCode>", "</referenceMachineCode>",, GetType(Boolean))

        If ContractList.Count > 0 Then
            If C_DEXContract Then

                'C_CreatorID = BetweenFromList(ContractList, "<creator>", "</creator>",, GetType(ULong))
                'C_CreatorAddress = BetweenFromList(ContractList, "<creatorRS>", "</creatorRS>",, GetType(String))
                'C_Name = BetweenFromList(ContractList, "<name>", "</name>",, GetType(String))
                'C_Description = BetweenFromList(ContractList, "<description>", "</description>",, GetType(String))

                'C_StartTimestamp = BetweenFromList(ContractList, "<timestamp>", "</timestamp>",, GetType(ULong))

                C_CurrentBalance = ClsSignumAPI.Planck2Dbl(GetULongBetweenFromList(ContractList, "<balanceNQT>", "</balanceNQT>"))

                Dim MachineData As String = GetStringBetweenFromList(ContractList, "<machineData>", "</machineData>")
                Dim MachineDataULongList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(MachineData)

                '0 = Long CreateOrderTX = 0L
                '1 = Long AcceptOrderTX = 0L

                '2 = Address Initiator = null
                '3 = Address Responder = null

                '4 = Long InitiatorsCollateral = 0L
                '5 = Long RespondersCollateral = 0L
                '6 = Long BuySellAmount = 0L
                '7 = Long ChainSwapHash = 0L

                '8 = Boolean SellOrder = False
                '9 = Boolean FirstTurn = True

                Dim TXList As List(Of String) = New List(Of String)

                If Not MachineDataULongList(0) = 0UL Then
                    TXList = SignumAPI.GetTransaction(MachineDataULongList(0))
                End If

                If TXList.Count > 0 Then

                    Dim Message As String = GetStringBetweenFromList(TXList, "<message>", "</message>")
                    Dim MessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(Message)

                    C_CurrentCreationTransaction = GetULongBetweenFromList(TXList, "<transaction>", "</transaction>")
                    C_CurrentConfirmations = GetULongBetweenFromList(TXList, "<confirmations>", "</confirmations>")

                    C_CurrentTimestamp = GetULongBetweenFromList(TXList, "<timestamp>", "</timestamp>")

                    '(0) ULong   creation method
                    '(1) ULong   collateral
                    '(2) ULong   xamountnqt
                    '(3) ULong   xitem USD

                    If MessageList.Count > 2 Then
                        C_CurrentXAmount = ClsSignumAPI.Planck2Dbl(MessageList(2))
                        C_CurrentXItem = ClsSignumAPI.ULng2String(MessageList(3))
                    End If

                    C_CurrentInitiatorID = MachineDataULongList(2)
                    If Not C_CurrentInitiatorID = 0UL Then
                        C_CurrentInitiatorAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(CurrentInitiatorID)
                    Else
                        C_CurrentInitiatorAddress = ""
                    End If

                    C_CurrentResponderID = MachineDataULongList(3)
                    If Not C_CurrentResponderID = 0UL Then
                        C_CurrentResponderAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(CurrentResponderID)
                    Else
                        C_CurrentResponderAddress = ""
                    End If


                    C_CurrentInitiatorsCollateral = ClsSignumAPI.Planck2Dbl(MachineDataULongList(4))
                    C_CurrentRespondersCollateral = ClsSignumAPI.Planck2Dbl(MachineDataULongList(5))
                    C_CurrentBuySellAmount = ClsSignumAPI.Planck2Dbl(MachineDataULongList(6))
                    C_CurrentChainSwapHash = MachineDataULongList(7)

                    C_CurrentPrice = C_CurrentXAmount / C_CurrentBuySellAmount

                    'If C_CurrentInitiatorsCollateral > C_CurrentBuySellAmount Then
                    '    'BuyOrder
                    '    C_IsSellOrder = False
                    '    C_CurrentSellerID = CurrentResponderID
                    '    C_CurrentBuyerID = CurrentInitiatorID
                    '    C_CurrentInitiatorsCollateral = ClsSignumAPI.Planck2Dbl(MachineDataULongList(6))
                    '    C_CurrentBuySellAmount = ClsSignumAPI.Planck2Dbl(MachineDataULongList(4))
                    'Else
                    '    C_IsSellOrder = True
                    '    C_CurrentSellerID = CurrentInitiatorID
                    '    C_CurrentBuyerID = CurrentResponderID
                    'End If

                    If MachineDataULongList(8) = 0UL Then
                        C_IsSellOrder = False
                        C_CurrentSellerID = CurrentResponderID
                        C_CurrentBuyerID = CurrentInitiatorID
                    Else
                        C_IsSellOrder = True
                        C_CurrentSellerID = CurrentInitiatorID
                        C_CurrentBuyerID = CurrentResponderID
                    End If

                    If Not C_CurrentSellerID = 0UL Then
                        C_CurrentSellerAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(CurrentSellerID)
                    Else
                        C_CurrentSellerAddress = ""
                    End If

                    If Not C_CurrentBuyerID = 0UL Then
                        C_CurrentBuyerAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(CurrentBuyerID)
                    Else
                        C_CurrentBuyerAddress = ""
                    End If


                    If CurrentInitiatorID = 0UL And CurrentResponderID = 0UL Then
                        C_Status = E_Status.FREE ' "FREE"
                    ElseIf Not CurrentInitiatorID = 0UL And CurrentResponderID = 0UL Then
                        C_Status = E_Status.OPEN ' "OPEN"
                    ElseIf CurrentInitiatorID = 0UL And Not CurrentResponderID = 0UL Then
                        C_Status = E_Status.ERROR_ ' "ERROR"
                    ElseIf Not CurrentInitiatorID = 0UL And Not CurrentResponderID = 0UL Then
                        C_Status = E_Status.RESERVED ' "RESERVED"
                    End If

                Else

                    If MachineDataULongList(9) = 0L Then
                        C_Status = E_Status.NEW_ ' "NEW"
                    Else

                        C_Status = E_Status.FREE ' "FREE"

                        C_CurrentXAmount = 0.0
                        C_CurrentXItem = ""

                        C_CurrentInitiatorID = 0UL
                        C_CurrentInitiatorAddress = ""

                        C_CurrentResponderID = 0UL
                        C_CurrentResponderAddress = ""

                        C_CurrentInitiatorsCollateral = 0.0
                        C_CurrentRespondersCollateral = 0.0
                        C_CurrentBuySellAmount = 0.0
                        C_CurrentChainSwapHash = 0UL

                        C_IsSellOrder = False
                        C_CurrentSellerID = 0UL
                        C_CurrentBuyerID = 0UL

                        C_CurrentSellerAddress = ""
                        C_CurrentBuyerAddress = ""

                    End If

                End If

                If CheckForUTX() Then
                    C_Status = E_Status.UTX_PENDING ' "UTX PENDING"
                End If

                C_IsFrozen = GetBooleanBetweenFromList(ContractList, "<frozen>", "</frozen>")
                C_IsRunning = GetBooleanBetweenFromList(ContractList, "<running>", "</running>")
                C_IsStopped = GetBooleanBetweenFromList(ContractList, "<stopped>", "</stopped>")
                C_IsFinished = GetBooleanBetweenFromList(ContractList, "<finished>", "</finished>")
                C_IsDead = GetBooleanBetweenFromList(ContractList, "<dead>", "</dead>")



                'While Not LastTx

                '    If C_ContractOrderHistoryList.Count = 0 Then
                '        If GetHistoryTransactions(C_ContractTimestamp) = 0 Then
                '            LastTx = True
                '            Exit While
                '        End If
                '    Else

                '        If StartIndex = 0UL Then
                If RefreshHistoryOrders Then
                    If C_ContractOrderHistoryList.Count = 0 Then
                        GetHistoryTransactions(C_ContractTimestamp)
                    Else
                        GetHistoryTransactions(C_ContractOrderHistoryList(C_ContractOrderHistoryList.Count - 1).EndTimestamp)
                    End If

                End If

                '        Else
                '            GetHistoryTransactions(C_ContractTimestamp)
                '        End If

                '        'If HistoryAmount = NewHistoryAmount Then
                '        '    LastTX = True
                '        '    Exit While
                '        'End If

                '    End If

                'End While

                ''C_CurrentTimestamp = C_ContractOrderHistoryList(C_ContractOrderHistoryList.Count - 1).EndTimestamp
                'StartIndex = 0UL
                'LastTx = False

                GetCurrentChat()

                C_IsReady = True

            Else
                'No DEX Contract
                C_IsReady = True
            End If

        Else
            'not ready
            C_IsReady = False
        End If

    End Sub

    ''' <summary>
    ''' Loads HistoryTransactions
    ''' </summary>
    ''' <param name="SetStartTimeStamp"></param>
    Private Function GetHistoryTransactions(Optional ByVal SetStartTimeStamp As ULong = 0UL) As Integer

        Dim SignumAPI = New ClsSignumAPI(C_Node)

        If SetStartTimeStamp = 0UL Then
            C_ContractOrderHistoryList.Clear()
        End If

        Dim T_ContractTransactionsPieceList As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(C_ID, SetStartTimeStamp)
        Dim T_ContractTransactionsList As List(Of List(Of String)) = New List(Of List(Of String))
        T_ContractTransactionsList.AddRange(T_ContractTransactionsPieceList.ToArray)

        Dim W500 As Integer = T_ContractTransactionsPieceList.Count
        While W500 >= 500

            C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "LoadHistoryTransactions(" + C_ID.ToString + "): " + W500.ToString)

            T_ContractTransactionsPieceList = SignumAPI.GetAccountTransactions(C_ID, SetStartTimeStamp, Convert.ToUInt64(W500))

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

            If T_LastTX.Recipient = C_ID Then
                If T_LastTX.Confirmations < 2 Then

                    If CurrentInitiatorID <> 0UL And CurrentResponderID <> 0UL Then
                        If T_LastTX.Sender = CurrentInitiatorID Or T_LastTX.Sender = CurrentResponderID Then
                            C_Status = E_Status.TX_PENDING
                        End If
                    Else
                        C_Status = E_Status.TX_PENDING
                    End If

                End If
            End If

            Dim T_ContractOrderHistoryList As List(Of S_Order) = New List(Of S_Order)

            For i As Integer = 0 To T_TXList.Count - 1

                Dim T_TX As S_TX = T_TXList(i)

                C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "ProcessingHistoryTransactions(" + C_ID.ToString + ") (" + i.ToString + "/" + T_TXList.Count.ToString + ")")

                If T_TX.Sender = C_ID Then

                    'LastTX = False

                    Dim ReferenceTXIDs As String = GetStringBetween(T_TX.Attachment, "<message>", "</message>")
                    If Not ReferenceTXIDs.Trim = "" Then

                        Dim ReferenceTXIDList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceTXIDs)

                        If Not ReferenceTXIDList(0) = 0UL And Not ReferenceTXIDList(1) = 0UL And Not ReferenceTXIDList(2) = 0UL Then
                            'referenceTXID

                            Dim T_CreationTX As S_TX = Nothing
                            Dim T_AcceptTX As S_TX = Nothing
                            Dim T_FinishTX As S_TX = Nothing

                            For Each R_TX As S_TX In T_TXList
                                Select Case R_TX.Transaction
                                    Case ReferenceTXIDList(0)
                                        T_CreationTX = R_TX
                                    Case ReferenceTXIDList(1)
                                        T_AcceptTX = R_TX
                                    Case ReferenceTXIDList(2)
                                        T_FinishTX = R_TX
                                End Select

                                If R_TX.Transaction = ReferenceTXIDList(1) And ReferenceTXIDList(1) = ReferenceTXIDList(2) Then
                                    T_AcceptTX = R_TX
                                    T_FinishTX = R_TX
                                End If

                                If Not T_CreationTX.Transaction = 0UL And Not T_AcceptTX.Transaction = 0UL And Not T_FinishTX.Transaction = 0UL Then
                                    Exit For
                                End If

                            Next

                            If Not T_CreationTX.Transaction = 0UL And Not T_AcceptTX.Transaction = 0UL And Not T_FinishTX.Transaction = 0UL Then

                                Dim ReferenceCreationMessage As String = GetStringBetween(T_CreationTX.Attachment, "<message>", "</message>")
                                Dim ReferenceCreationMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceCreationMessage)

                                Dim ReferenceAcceptMessage As String = GetStringBetween(T_AcceptTX.Attachment, "<message>", "</message>")
                                Dim ReferenceAcceptMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceAcceptMessage)

                                Dim ReferenceFinishMessage As String = GetStringBetween(T_FinishTX.Attachment, "<message>", "</message>")
                                Dim ReferenceFinishMessageList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(ReferenceFinishMessage)

                                If SignumAPI.ReferenceCreateOrder = ReferenceCreationMessageList(0) And (SignumAPI.ReferenceAcceptOrder = ReferenceAcceptMessageList(0) Or SignumAPI.ReferenceInjectResponder = ReferenceAcceptMessageList(0)) And ((SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Or SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0)) Or (ReferenceAcceptMessageList(0) = ReferenceFinishMessageList(0) And T_AcceptTX.Transaction = T_FinishTX.Transaction)) Then

                                    Dim T_Order As S_Order = New S_Order

                                    T_Order.CreationTransaction = T_CreationTX.Transaction
                                    T_Order.LastTransaction = T_FinishTX.Transaction

                                    T_Order.Confirmations = T_CreationTX.Confirmations

                                    T_Order.StartTimestamp = T_CreationTX.Timestamp
                                    T_Order.EndTimestamp = T_TX.Timestamp + 1UL

                                    'T_Order.StartDate = ClsSignumAPI.UnixToTime(T_Order.StartTimestamp.ToString) 'Debug
                                    'T_Order.EndDate = ClsSignumAPI.UnixToTime(T_Order.EndTimestamp.ToString) 'Debug

                                    Dim T_Collateral As ULong = ReferenceCreationMessageList(1)
                                    Dim T_Amount As ULong = T_CreationTX.AmountNQT '- T_Collateral - ClsSignumAPI._GasFeeNQT

                                    If T_Amount > T_Collateral Then

                                        '(0) ULong   Creation Method SellOrder
                                        '(1) ULong   Collateral
                                        '(2) ULong   XAmountNQT
                                        '(3) ULong   XItem USD

                                        T_Order.WasSellOrder = True

                                        T_Order.SellerID = T_CreationTX.Sender
                                        T_Order.SellerRS = T_CreationTX.SenderRS
                                        T_Order.BuyerID = T_AcceptTX.Sender
                                        T_Order.BuyerRS = T_AcceptTX.SenderRS

                                        T_Order.Amount = ClsSignumAPI.Planck2Dbl(T_Amount - T_Collateral - ClsSignumAPI._GasFeeNQT)
                                        T_Order.Collateral = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(1))

                                        T_Order.XAmount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(2))
                                        T_Order.XItem = ClsSignumAPI.ULng2String(ReferenceCreationMessageList(3))

                                        T_Order.Price = T_Order.XAmount / T_Order.Amount

                                        Dim Finisher As ULong = T_FinishTX.Sender

                                        If SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Then
                                            If Finisher = T_Order.SellerID Then
                                                T_Order.Status = E_Status.CLOSED
                                            Else 'If Finisher = o.BuyerID Then
                                                T_Order.Status = E_Status.CANCELED
                                            End If
                                        ElseIf SignumAPI.ReferenceAcceptOrder = ReferenceFinishMessageList(0) Then
                                            'initiator canceled Order
                                            T_Order.Status = E_Status.CANCELED
                                        ElseIf SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0) Then
                                            T_Order.Status = E_Status.CLOSED
                                        End If


                                    Else

                                        '(0) ULong   Creation Method BuyOrder
                                        '(1) ULong   WantToBuyAmount
                                        '(2) ULong   XAmountNQT
                                        '(3) ULong   XItem USD

                                        T_Order.WasSellOrder = False

                                        T_Order.BuyerID = T_CreationTX.Sender
                                        T_Order.BuyerRS = T_CreationTX.SenderRS
                                        T_Order.SellerID = T_AcceptTX.Sender
                                        T_Order.SellerRS = T_AcceptTX.SenderRS

                                        T_Order.Amount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(1))
                                        T_Order.Collateral = ClsSignumAPI.Planck2Dbl(T_Amount - ClsSignumAPI._GasFeeNQT)

                                        T_Order.XAmount = ClsSignumAPI.Planck2Dbl(ReferenceCreationMessageList(2))
                                        T_Order.XItem = ClsSignumAPI.ULng2String(ReferenceCreationMessageList(3))

                                        T_Order.Price = T_Order.XAmount / T_Order.Amount

                                        Dim Finisher As ULong = T_FinishTX.Sender

                                        If SignumAPI.ReferenceFinishOrder = ReferenceFinishMessageList(0) Then
                                            If Finisher = T_Order.SellerID Then
                                                T_Order.Status = E_Status.CLOSED
                                            Else 'If Finisher = o.BuyerID Then
                                                T_Order.Status = E_Status.CANCELED
                                            End If
                                        ElseIf SignumAPI.ReferenceAcceptOrder = ReferenceFinishMessageList(0) Then
                                            'initiator canceled Order
                                            T_Order.Status = E_Status.CANCELED
                                        ElseIf SignumAPI.ReferenceFinishOrderWithChainSwapKey = ReferenceFinishMessageList(0) Then
                                            T_Order.Status = E_Status.CLOSED
                                        End If

                                    End If

                                    T_ContractOrderHistoryList.Add(T_Order)

                                End If

                            End If

                        End If

                    End If

                End If

            Next

            If SetStartTimeStamp = 0UL Then
                C_ContractOrderHistoryList = T_ContractOrderHistoryList.OrderBy(Function(T_TX As S_Order) T_TX.StartTimestamp).ToList
            Else
                T_ContractOrderHistoryList = T_ContractOrderHistoryList.OrderBy(Function(T_TX As S_Order) T_TX.StartTimestamp).ToList
                C_ContractOrderHistoryList.AddRange(T_ContractOrderHistoryList.ToArray)
            End If
        Else

        End If

        C_StartForm.MultiInvoker(C_StartForm.SubStatusLabel, "Text", "")

        Return C_ContractOrderHistoryList.Count

    End Function

    Private Sub GetCurrentChat()

        If Not C_CurrentInitiatorID = 0UL And Not C_CurrentResponderID = 0UL Then

            Dim SignumAPI = New ClsSignumAPI(C_Node)

            Dim T_TXIDList As List(Of ULong) = SignumAPI.GetTransactionIds(C_CurrentInitiatorID, C_CurrentResponderID, C_CurrentTimestamp)
            T_TXIDList.AddRange(SignumAPI.GetTransactionIds(C_CurrentResponderID, C_CurrentInitiatorID, C_CurrentTimestamp))

            Dim T_TXRList As List(Of List(Of String)) = New List(Of List(Of String))

            For Each T_TXID As ULong In T_TXIDList
                Dim T_TX As List(Of String) = SignumAPI.GetTransaction(T_TXID)
                T_TXRList.Add(T_TX)
            Next


            Dim T_TXList As List(Of S_TX) = New List(Of S_TX)

            For Each TX As List(Of String) In T_TXRList
                Dim T_TX As S_TX = New S_TX

                T_TX.Transaction = GetULongBetweenFromList(TX, "<transaction>", "</transaction>")
                'T_TX.Type = BetweenFromList(TX, "<type>", "</type>",, GetType(Integer))
                T_TX.Timestamp = GetULongBetweenFromList(TX, "<timestamp>", "</timestamp>")

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

            T_TXList = T_TXList.OrderBy(Function(T_TX As S_TX) T_TX.Timestamp).ToList

            C_CurrentChat.Clear()

            For Each T_TX As S_TX In T_TXList

                Dim T_Chat As S_Chat = New S_Chat
                T_Chat.Timestamp = T_TX.Timestamp
                T_Chat.SenderAddress = T_TX.SenderRS
                T_Chat.RecipientAddress = T_TX.RecipientRS
                T_Chat.Attachment = T_TX.Attachment

                C_CurrentChat.Add(T_Chat)

            Next

        End If

    End Sub

    Private Sub GetChatHistory()

        For Each T_Order As S_Order In C_ContractOrderHistoryList
            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_Node)
        Next

    End Sub

    ''' <summary>
    ''' Checks incoming unconfirmedTransaction for DEXContract 
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckForUTX() As Boolean

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_Node)
        Dim T_UTXList As List(Of List(Of String)) = SignumAPI.GetUnconfirmedTransactions()

        For Each UTX In T_UTXList

            Dim TX_RecipientRS As String = GetStringBetweenFromList(UTX, "<recipientRS>", "</recipientRS>")
            Dim TX_SenderRS As String = GetStringBetweenFromList(UTX, "<senderRS>", "</senderRS>")

            If Not TX_RecipientRS.Trim = "" Then
                Dim TX_Recipient As ULong = GetULongBetweenFromList(UTX, "<recipient>", "</recipient>")

                If TX_Recipient = C_ID Then

                    If Not TX_SenderRS.Trim = "" Then

                        Dim TX_Sender As ULong = GetULongBetweenFromList(UTX, "<sender>", "</sender>")

                        If CurrentInitiatorID <> 0UL And CurrentResponderID <> 0UL Then
                            If TX_Sender = CurrentInitiatorID Or TX_Sender = CurrentResponderID Then
                                C_Status = E_Status.UTX_PENDING ' "UTX PENDING"
                                Return True
                            End If
                        Else
                            C_Status = E_Status.UTX_PENDING ' "UTX PENDING"
                            Return True
                        End If

                    Else
                        C_Status = E_Status.UTX_PENDING ' "UTX PENDING"
                        Return True
                    End If

                End If
            End If

        Next

        Return False

    End Function
    ''' <summary>
    ''' Checks incoming Transaction for DEXContract
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckForTX() As Boolean

        Dim T_LastTimestamp As ULong = C_ContractTimestamp

        If C_ContractOrderHistoryList.Count > 0 Then
            T_LastTimestamp = C_ContractOrderHistoryList(C_ContractOrderHistoryList.Count - 1).EndTimestamp
        End If

        If ContractOrderHistoryList.Count > 0 Then
            T_LastTimestamp = ContractOrderHistoryList(ContractOrderHistoryList.Count - 1).EndTimestamp
        End If

        Dim SignumAPI = New ClsSignumAPI(C_Node)
        Dim T_ContractTransactionsList As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(C_ID, T_LastTimestamp)

        Dim T_TXList As List(Of S_TX) = New List(Of S_TX)

        For Each TX As List(Of String) In T_ContractTransactionsList
            Dim T_TX As S_TX = New S_TX

            T_TX.Transaction = GetULongBetweenFromList(TX, "<transaction>", "</transaction>")
            T_TX.Type = GetIntegerBetweenFromList(TX, "<type>", "</type>")
            T_TX.Timestamp = GetULongBetweenFromList(TX, "<timestamp>", "</timestamp>")

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
            T_TXList = T_TXList.OrderBy(Function(T_TX As S_TX) T_TX.Timestamp).ToList

            Dim T_LastTX As S_TX = T_TXList(T_TXList.Count - 1)

            If T_LastTX.Recipient = C_ID Then
                If T_LastTX.Confirmations < 2 Then

                    If CurrentInitiatorID <> 0UL And CurrentResponderID <> 0UL Then
                        If T_LastTX.Sender = CurrentInitiatorID Or T_LastTX.Sender = CurrentResponderID Then
                            C_Status = E_Status.TX_PENDING ' "TX PENDING"
                            Return True
                        End If
                    Else
                        C_Status = E_Status.TX_PENDING ' "TX PENDING"
                        Return True
                    End If

                End If

            End If

        End If

        Return False

    End Function

End Class