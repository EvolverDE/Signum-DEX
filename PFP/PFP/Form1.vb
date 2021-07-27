

Public Class PFPForm

    Property NodeList() As List(Of String) = New List(Of String)

    Property PrimaryNode() As String = ""

    Property NuBlock As Integer = 1
    Property Block() As Integer = 0
    Property Fee() As Double = 0.0
    Property UTXList() As List(Of List(Of String))
    Property RefreshTime() As Integer = 600

    Property CurrentMarket As String = ""
    Property MarketIsCrypto() As Boolean = False
    Property Decimals() As Integer = 8

    Property AccountID() As String

    Property Boottime As Integer = 0

    Property DEXATList As List(Of String) = New List(Of String)

    Property CoBxChartSelectedItem As String = ""
    Property CoBxTickSelectedItem As String = ""

    Property InfoOut As Boolean = GetINISetting(E_Setting.InfoOut, True)


    Function GetPaymentInfoFromOrderSettings(ByVal TX As String, Optional ByVal Quantity As Double = 0.0, Optional ByVal XAmount As Double = 0.0, Optional ByVal Market As String = "") As String

        Dim PaymentInfo As String = ""

        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(TX)

        If T_OSList.Count > 0 Then
            Dim T_OS As ClsOrderSettings = T_OSList(0)
            T_OS.SetPayType()

            If T_OS.AutoSendInfotext Then

                Select Case T_OS.PayType
                    Case ClsOrderSettings.E_PayType.Bankaccount
                        PaymentInfo = "Bankdetails=" + T_OS.Infotext.Replace(",", ";")
                    Case ClsOrderSettings.E_PayType.PayPal_E_Mail
                        PaymentInfo = "PayPal-E-Mail=" + T_OS.Infotext.Replace(",", ";")
                    Case ClsOrderSettings.E_PayType.PayPal_Order

                        Dim APIOK As String = CheckPayPalAPI()

                        If APIOK = "True" Then
                            Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal
                            PPAPI_Autoinfo.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                            PPAPI_Autoinfo.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                            Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", Quantity, XAmount, Market)
                            Dim PPOrderID As String = BetweenFromList(PPOrderIDList, "<id>", "</id>")
                            PaymentInfo = "PayPal-Order=" + PPOrderID
                        End If

                    Case ClsOrderSettings.E_PayType.Self_Pickup
                        PaymentInfo = "PaymentInfotext=" + GetINISetting(E_Setting.PaymentInfoText, "").Replace(",", ";")
                    Case ClsOrderSettings.E_PayType.Other
                        PaymentInfo = "PaymentInfotext=" + GetINISetting(E_Setting.PaymentInfoText, "").Replace(",", ";")
                End Select

            End If

        End If

        Return PaymentInfo

    End Function
    Function CheckPayPalOrder(ByVal ATID As String, ByVal PayPalOrder As String) As String

        Dim Status As String = ""

        If Not PayPalOrder = "0" And Not PayPalOrder = "" Then

            Dim APIOK As String = CheckPayPalAPI()
            If APIOK = "True" Then

                'PayPal Approving check
                Dim PPAPI As ClsPayPal = New ClsPayPal
                PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                Dim OrderDetails As List(Of String) = PPAPI.GetOrderDetails(PayPalOrder)
                Dim PayPalStatus As String = BetweenFromList(OrderDetails, "<status>", "</status>")

                If PayPalStatus = "APPROVED" Then
                    PPAPI = New ClsPayPal
                    PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                    PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                    OrderDetails = PPAPI.CaptureOrder(PayPalOrder)
                    PayPalStatus = BetweenFromList(OrderDetails, "<status>", "</status>")

                    If PayPalStatus = "COMPLETED" Then
                        Dim BCR1 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                        Dim TXStr As String = BCR1.SendMessage2BLSAT(ATID, 1.0, New List(Of ULong)({BCR1.ReferenceFinishOrder}))

                        If TXStr.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TXStr)
                        Else
                            Status = "COMPLETED"
                        End If

                    Else
                        Status = PayPalStatus
                    End If

                ElseIf PayPalStatus = "COMPLETED" Then

                    'Dim BoolUTX As Boolean = CheckForUTX(, Order.ATRS)
                    'Dim BoolTX As Boolean = CheckForTX(Order.ATRS, Order.Seller, Order.FirstTimestamp, "order accepted", True)

                    'If BoolUTX = False And BoolTX = False Then
                    '    AlreadySend = "COMPLETED"
                    'Else
                    Status = "COMPLETED"
                    'End If

                ElseIf PayPalStatus = "CREATED" Then

                    Status = "PayPal Order created"

                Else
                    'TODO: auto Recreate PayPal Order
                    Status = "No Payment received"
                End If

            End If

        End If

        Return Status

    End Function
    Function CheckPayPalTransaction(ByVal Order As S_Order) As String

        Dim Status As String = ""

        Dim APIOK As String = CheckPayPalAPI()
        If APIOK = "True" Then

            Dim BCR1 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
            Dim CheckAttachment As String = BCR1.ULngList2DataStr(New List(Of ULong)({BCR1.ReferenceFinishOrder}))
            Dim UTXCheck As Boolean = CheckForUTX(Order.Seller, Order.ATRS, CheckAttachment)
            Dim TXCheck As Boolean = CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, CheckAttachment)

            If Not UTXCheck And Not TXCheck Then

                If Not GetAutosignalTXFromINI(Order.FirstTransaction) Then 'Check for autosignal-TX in Settings.ini and skip if founded

                    'PayPal Approving check
                    Dim PPAPI_GetPayPalTX_to_Autosignal_AT As ClsPayPal = New ClsPayPal
                    PPAPI_GetPayPalTX_to_Autosignal_AT.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                    PPAPI_GetPayPalTX_to_Autosignal_AT.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                    Dim TXDetails As List(Of List(Of String)) = PPAPI_GetPayPalTX_to_Autosignal_AT.GetTransactionList(ColWords.GenerateColloquialWords(Order.FirstTransaction, True, "-", 5))

                    If TXDetails.Count > 0 Then

                        Dim PayPalStatus As String = BetweenFromList(TXDetails(0), "<transaction_status>", "</transaction_status>")
                        Dim Amount As String = BetweenFromList(TXDetails(0), "<transaction_amount>", "</transaction_amount>")

                        If CDbl(Amount) >= Order.XAmount And PayPalStatus.ToLower.Trim = "s" Then
                            'complete
                            Dim TXStr As String = BCR1.SendMessage2BLSAT(Order.AT, 1.0, New List(Of ULong)({BCR1.ReferenceFinishOrder}))
                            Status = "COMPLETED"

                            If SetAutosignalTX2INI(Order.FirstTransaction) Then 'Set autosignal-TX in Settings.ini
                                'ok
                            End If

                        End If

                    End If

                End If

            Else
                Status = "PENDING"
            End If

        End If

        Return Status

    End Function


    Structure S_AT
        Dim AT As String
        Dim ATRS As String
        Dim IsBLS_AT As Boolean
    End Structure
    Property CSVATList() As List(Of S_AT) = New List(Of S_AT)


    Public Structure S_PFPAT
        Dim AT As String
        Dim ATRS As String
        Dim Creator As String
        Dim CreatorRS As String
        Dim Name As String
        Dim Description As String
        Dim Sellorder As Boolean
        Dim Initiator As String
        Dim Responser As String
        Dim InitiatorsCollateral As Double
        Dim ResponsersCollateral As Double
        Dim BuySellAmount As Double
        Dim XAmount As Double
        Dim XItem As String
        Dim Balance As String
        Dim AT_TXList As List(Of S_PFPAT_TX)
        Dim AT_OrderList As List(Of S_Order)
        Dim Status As String
        Dim Frozen As Boolean
        Dim Running As Boolean
        Dim Stopped As Boolean
        Dim Finished As Boolean
        Dim Dead As Boolean
    End Structure
    Property PFPList() As List(Of S_PFPAT) = New List(Of S_PFPAT)

    Public Structure S_PFPAT_TX

        Sub New(ByVal null)
            Type = ""
            Timestamp = ""
            Recipient = ""
            RecipientRS = ""
            AmountNQT = 0.0
            FeeNQT = 0.0
            Transaction = ""
            Attachment = ""
            Sender = ""
            SenderRS = ""
            Confirmations = ""

        End Sub

        Dim Type As String
        Dim Timestamp As String
        Dim Recipient As String
        Dim RecipientRS As String
        Dim AmountNQT As String
        Dim FeeNQT As String
        Dim Transaction As String
        Dim Attachment As String
        Dim Sender As String
        Dim SenderRS As String
        Dim Confirmations As String
    End Structure

    Public Structure S_Order
        Dim AT As String
        Dim ATRS As String
        Dim Type As String
        Dim Seller As String
        Dim SellerID As String
        Dim Buyer As String
        Dim BuyerID As String
        Dim XItem As String
        Dim XAmount As Double
        Dim Quantity As Double
        Dim Price As Double
        Dim Collateral As Double
        Dim Status As String
        Dim Attachment As String
        Dim FirstTransaction As String
        Dim FirstTimestamp As String
        Dim FirstTX As S_PFPAT_TX
        Dim LastTX As S_PFPAT_TX
    End Structure
    Property OrderList() As List(Of S_Order) = New List(Of S_Order)


    Structure S_APIRequest
        Dim RequestThread As Threading.Thread
        Dim Command As String
        Dim Node As String
        Dim Status As String
        Dim Result As Object
    End Structure

    Property APIRequestList As List(Of S_APIRequest) = New List(Of S_APIRequest)

    Property TCPAPITradeHistoryList As List(Of List(Of String)) = New List(Of List(Of String))

    Dim SplitPanel As SplitContainer = New SplitContainer
    Dim PanelForSplitPanel As Panel = New Panel

    Dim TLS As SplitContainer = New SplitContainer
    Dim TTTL As TradeTrackerTimeLine = New TradeTrackerTimeLine


#Region "GUI Control"

#Region "General"

    Property LoadRunning As Boolean = False
    Property Shutdown As Boolean = False
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If InfoOut Then
            Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
            IOut.Info2File(Application.ProductName + "-info from Form1_FormClosing(): -> app close")
        End If

        'If ChBxTCPAPI.Checked Then
        Dim Wait As Boolean = TCPAPI.StopAPIServer()
        If Not IsNothing(DEXNET) Then
            DEXNET.StopServer()
        End If


        If LoadRunning Then
            ClsMsgs.MBox("Shutting down, please wait...", "Shutdown", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.OneOnly),, ClsMsgs.Status.Information, 3, ClsMsgs.Timer_Type.AutoOK)
            e.Cancel = True
            Shutdown = True
        Else
            Dim TestMultiExit As S_APIRequest = New S_APIRequest
            TestMultiExit.Command = "Exit()"
            APIRequestList.Add(TestMultiExit)
        End If
        'End If

    End Sub

    Property T_PassPhrase() As String = ""
    Property T_SignkeyHEX As String = ""
    Property T_AgreementKeyHEX As String = ""
    Property T_PublicKeyHEX As String = ""


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


#Region "TradeTracker"

        SplitPanel.Dock = DockStyle.Fill
        SplitPanel.Orientation = Orientation.Horizontal

        SplitPanel.SplitterDistance = 70

        SplitPanel.FixedPanel = FixedPanel.Panel1

        PanelForSplitPanel.AutoScroll = True
        PanelForSplitPanel.Dock = DockStyle.Fill

        SplitPanel.BorderStyle = BorderStyle.FixedSingle

        TLS.Dock = DockStyle.Fill
        TLS.IsSplitterFixed = True
        TLS.BorderStyle = BorderStyle.FixedSingle
        'TLS.SplitterDistance = 32
        TLS.Size = New Size(0, 70)
        'WTTL.Dock = DockStyle.Fill
        TTTL.Height = TLS.Height
        TTTL.TradeTrackTimerEnable = False

        AddHandler TTTL.TimerTick, AddressOf TradeTrackerTimeLine1_TimerTick

        Dim LabChart As Label = New Label
        LabChart.Text = "Chart (Days): "
        LabChart.Font = New Font(LabChart.Font, FontStyle.Bold)
        LabChart.Location = New Point(0, 10)

        Dim CoBxChart As ComboBox = New ComboBox
        CoBxChart.Location = New Point(LabChart.Width, 10)
        CoBxChart.Font = New Font(CoBxChart.Font, FontStyle.Bold)
        CoBxChart.Size = New Size(50, CoBxChart.Size.Height)
        CoBxChart.DropDownStyle = ComboBoxStyle.DropDownList
        CoBxChart.Name = "CoBxChart"
        CoBxChart.Items.AddRange({1, 3, 7, 15, 30})
        CoBxChart.SelectedItem = CoBxChart.Items(1)

        CoBxChartSelectedItem = CoBxChart.Items(1)
        AddHandler CoBxChart.DropDownClosed, AddressOf CoBx_Chart_Handler


        Dim LabTick As Label = New Label
        LabTick.Text = "Tick (Min): "
        LabTick.Font = New Font(LabTick.Font, FontStyle.Bold)
        LabTick.Location = New Point(0, CoBxChart.Height + 10)

        Dim CoBxTick As ComboBox = New ComboBox
        CoBxTick.Location = New Point(LabTick.Width, CoBxChart.Height + 10)
        CoBxTick.Font = New Font(CoBxTick.Font, FontStyle.Bold)
        CoBxTick.Size = New Size(50, CoBxTick.Size.Height)
        CoBxTick.DropDownStyle = ComboBoxStyle.DropDownList
        CoBxTick.Name = "CoBxTick"
        CoBxTick.Items.AddRange({1, 5, 15, 30, 60, 360, 720})
        CoBxTick.SelectedItem = CoBxTick.Items(2)

        CoBxTickSelectedItem = CoBxTick.Items(2)
        AddHandler CoBxTick.DropDownClosed, AddressOf CoBx_Tick_Handler

        TLS.Panel1.Controls.AddRange({LabChart, LabTick, CoBxChart, CoBxTick})
        TLS.Panel2.Controls.Add(TTTL)

        Dim WTS As TradeTrackerSlot = New TradeTrackerSlot
        WTS.Location = New Point(0, 0)
        WTS.Dock = DockStyle.Fill
        WTS.LabExch.Text = "booting..."
        WTS.LabPair.Text = "booting..."


        SplitPanel.Panel1.Controls.Add(TLS)
        PanelForSplitPanel.Controls.Add(WTS)
        SplitPanel.Panel2.Controls.Add(PanelForSplitPanel)

        SplitContainer2.Panel1.Controls.Add(SplitPanel)
#End Region

        T_PassPhrase = GetINISetting(E_Setting.PassPhrase, "")

        If T_PassPhrase.Trim = "" Then

            BlockTimer.Enabled = False
            Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()

            If T_PassPhrase.Trim = "" Then
                ClsMsgs.MBox("No PassPhrase set or unknown Address, program will close.", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)

                Application.Exit()
                Exit Sub
            Else
                BlockTimer.Enabled = True
            End If

            TBSNOPassPhrase.Text = T_PassPhrase
        Else
            TBSNOPassPhrase.Text = T_PassPhrase
        End If

        Dim MasterKeyHEXList As List(Of String) = GetMasterKeys(T_PassPhrase)

        T_PublicKeyHEX = MasterKeyHEXList(0)
        T_SignkeyHEX = MasterKeyHEXList(1)
        T_AgreementKeyHEX = MasterKeyHEXList(2)

        CurrentMarket = GetINISetting(E_Setting.LastMarketViewed, "EUR")
        CoBxMarket.SelectedItem = CurrentMarket


        Dim Nodes As String = GetINISetting(E_Setting.Nodes, "http://nivbox.co.uk:6876/burst;https://testnet.burstcoin.network:6876/burst") ' INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "Nodes", "http://nivbox.co.uk:6876/burst;https://testnet.burstcoin.network:6876/burst;https://octalsburstnode.ddns.net:6876/burst;https://testnetwallet.burstcoin.ro/burst")

        NodeList.Clear()
        If Nodes.Contains(";") Then
            NodeList.AddRange(Nodes.Split(";"))
        Else
            NodeList.Add(Nodes)
        End If


        'InitiateDEXNET()

        PrimaryNode = GetINISetting(E_Setting.DefaultNode, "http://nivbox.co.uk:6876/burst")

        'TODO: Reset Refreshtime
        RefreshTime = 100 ' CInt(GetINISetting(E_Setting.RefreshMinutes, "1")) * 600

        If CBool(GetINISetting(E_Setting.TCPAPIEnable, "False")) Then
            TCPAPI.StartAPIServer()
        End If

        If TBSNOPassPhrase.Text.Trim = "" Then
            BlockTimer.Enabled = False
            Exit Sub
        End If

        Dim T_DEXATList As List(Of String()) = GetDEXATsFromCSV()
        DEXATList.Clear()

        For Each T_DEX In T_DEXATList
            If T_DEX(2) = "True" Then
                DEXATList.Add(T_DEX(0))
            End If
        Next


        Dim GetThr As Threading.Thread = New Threading.Thread(AddressOf GetThread)
        GetThr.Start()

        'SplitContainer2.Panel1.Visible = False

        ResetLVColumns()

        ForceReload = True

        'BlockTimer_Tick(True, Nothing)

        Dim Currency_Day_TickList As List(Of List(Of String)) = GetTCPAPICurrencyDaysTicks()

        Dim GetCandlesInfo As String = ""
        For Each CDT As List(Of String) In Currency_Day_TickList
            GetCandlesInfo += "{""pair"":""" + CDT(0) + """, ""days"":""" + CDT(1) + """, ""tickmin"":""" + CDT(2) + """},"
        Next

        GetCandlesInfo = GetCandlesInfo.Remove(GetCandlesInfo.Length - 1)
        GetCandlesInfo += ""

        Dim DEXAPIInfo As String = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""GetInfo"","
        DEXAPIInfo += """requests"":{"
        DEXAPIInfo += """GetInfo"":""shows this info"","
        DEXAPIInfo += """GetCandles"":[{""queryExample"":""/API/v1.0/GetCandles?pair=USD_SIGNA&days=3&tickmin=15""},"
        DEXAPIInfo += GetCandlesInfo
        DEXAPIInfo += "],"
        DEXAPIInfo += """GetOpenOrders"":""shows the list of open orders"""
        DEXAPIInfo += "}}"


        Dim DEXAPIGetInfoResponse As ClsTCPAPI.API_Response = New ClsTCPAPI.API_Response
        DEXAPIGetInfoResponse.API_Interface = "API"
        DEXAPIGetInfoResponse.API_Version = "v1.0"
        DEXAPIGetInfoResponse.API_Command = "GetInfo"
        DEXAPIGetInfoResponse.API_Response = DEXAPIInfo
        DEXAPIGetInfoResponse.API_Parameters = New List(Of String)({""})

        TCPAPI.ResponseMSGList.Add(DEXAPIGetInfoResponse)

    End Sub

    Sub CoBx_Chart_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxChart As ComboBox = DirectCast(sender, ComboBox)
        CoBxChartSelectedItem = CoBxChart.SelectedItem

        ForceReload = True
        'BlockTimer_Tick(True, Nothing)

    End Sub

    Sub CoBx_Tick_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxTick As ComboBox = DirectCast(sender, ComboBox)
        CoBxTickSelectedItem = CoBxTick.SelectedItem

        ForceReload = True

        'BlockTimer_Tick(True, Nothing)

    End Sub



    Private Sub TradeTrackerTimeLine1_TimerTick(sender As Object)

        For Each TTSlot As TradeTrackerSlot In PanelForSplitPanel.Controls

            Dim TempSC As SplitContainer = CType(SplitPanel.Panel1.Controls(0), SplitContainer)
            Dim TempTimeLine As TradeTrackerTimeLine = CType(TempSC.Panel2.Controls(0), TradeTrackerTimeLine)
            Try

                TLS.SplitterDistance = TTSlot.SplitterDistance
            Catch ex As Exception

            End Try
            TTSlot.Dock = DockStyle.Fill
            TTTL.Dock = DockStyle.Fill

            TTSlot.Chart_EMA_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.Chart_EMA_EndDate = TempTimeLine.SkalaEndDate

            TTSlot.MACD_RSI_TR_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.MACD_RSI_TR_EndDate = TempTimeLine.SkalaEndDate

        Next

    End Sub

    Private Sub BlockTimer_Tick(sender As Object, e As EventArgs) Handles BlockTimer.Tick

        Boottime += 1

        'StatusBar.Visible = True
        'StatusBar.Maximum = RefreshTime
        'StatusBar.Value = RefreshTime - Boottime

        Application.DoEvents()

        SetDEXNETRelevantMsgsToLVs()

        Dim Wait As Boolean = False

        If Boottime >= RefreshTime Or ForceReload Then
            Boottime = 0

            BlockTimer.Enabled = False

            Dim T_NuBlockThread As Threading.Thread = New Threading.Thread(AddressOf GetNuBlock)
            T_NuBlockThread.Start(PrimaryNode)

            While T_NuBlockThread.IsAlive
                Application.DoEvents()
            End While

            If NuBlock > Block Or ForceReload Then
                Block = NuBlock
                ForceReload = False

                LoadRunning = True

                Application.DoEvents()

                If Not IsNothing(DEXNET) Then

                    Dim Peers As List(Of ClsDEXNET.S_Peer) = DEXNET.GetPeers()

                    'TODO: first start skip
                    If Peers.Count = 0 Then
                        DEXNET.StopServer()
                        InitiateDEXNET()
                    End If

                Else
                    InitiateDEXNET()
                End If

                Wait = Loading()
                Wait = SetInLVs()

                'TODO: reset refreshtime
                RefreshTime = 100 ' CInt(GetINISetting(E_Setting.RefreshMinutes, "1")) * 600

                Dim CoBxChartVal As Integer = 1
                Dim CoBxTickVal As Integer = 1

                For Each CTRL In TLS.Panel1.Controls

                    If CTRL.GetType.Name = GetType(ComboBox).Name Then

                        If CTRL.Name = "CoBxChart" Then
                            Dim CoBxChart As ComboBox = DirectCast(CTRL, ComboBox)
                            CoBxChartVal = CInt(CoBxChart.SelectedItem)
                        End If

                        If CTRL.Name = "CoBxTick" Then
                            Dim CoBxTick As ComboBox = DirectCast(CTRL, ComboBox)
                            CoBxTickVal = CInt(CoBxTick.SelectedItem)
                        End If

                    End If

                Next


                Dim ViewThread As Threading.Thread = New Threading.Thread(AddressOf LoadHistory)
                ViewThread.Start(New List(Of Object)({CoBxChartVal, CoBxTickVal, CurrentMarket}))


                TTTL.TradeTrackTimer.Enabled = True
                BlockTimer.Enabled = True

                LoadRunning = False

                If Shutdown Then
                    Me.Close()
                End If
            Else
                BlockTimer.Enabled = True
            End If

        End If

    End Sub


    Property ForceReload As Boolean = False
    Private Sub CoBxMarket_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxMarket.SelectedIndexChanged, CoBxMarket.DropDownClosed

        CurrentMarket = CoBxMarket.SelectedItem
        ResetLVColumns()

        ForceReload = True
    End Sub
    Private Sub TBSNOPassPhrase_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBSNOPassPhrase.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Select Case keys
                'Case 48 To 57, 44, 8
                    ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                BtCheckAddress.PerformClick()
                e.Handled = True
            Case Else
                ' alle anderen Eingaben unterdrücken
                'e.Handled = True
        End Select

    End Sub
    Private Sub BtCheckAddress_Click(sender As Object, e As EventArgs) Handles BtCheckAddress.Click

        'Dim BCR As ClsSignumAPI = New ClsSignumAPI With {.C_PassPhrase = TBSNOPassPhrase.Text}

        'Dim x As List(Of String) = BCR.GetAccountFromPassPhrase()

        'TBSNOAddress.Text = BCR.BetweenFromList(x, "<address>", "</address>")
        'TBSNOBalance.Text = BCR.BetweenFromList(x, "<available>", "</available>")
        'AccountID = BCR.BetweenFromList(x, "<account>", "</account>")

        CoBxMarket_SelectedIndexChanged(Nothing, Nothing)

    End Sub

#End Region

#Region "Marketdetails - Controls"

    Private Sub BtCreateNewAT_Click(sender As Object, e As EventArgs) Handles BtCreateNewAT.Click

        Dim BSR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)

        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new Payment channel" + vbCrLf + "(AT=Automated Transaction)?", "Create AT", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

            Dim NuList As List(Of String) = BSR.CreateAT()

            If NuList.Count = 0 Then
                ClsMsgs.MBox("Error creating new AT", "Error",,, ClsMsgs.Status.Erro)
                Exit Sub
            End If

            ' Dim NuATList As List(Of S_AT) = New List(Of S_AT)
            Dim NuAT As S_AT = New S_AT
            NuAT.AT = BetweenFromList(NuList, "<transaction>", "</transaction>")

            Dim AccRS As List(Of String) = BSR.RSConvert(NuAT.AT)
            NuAT.ATRS = BetweenFromList(AccRS, "<accountRS>", "</accountRS>")
            NuAT.IsBLS_AT = True

            'NuATList.Add(NuAT)
            'SaveATsToCSV(NuATList)

            ClsMsgs.MBox("New AT Created" + vbCrLf + vbCrLf + "TX: " + NuAT.AT, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

        End If

    End Sub
    Private Sub TBarCollateralPercent_Scroll(sender As Object, e As EventArgs) Handles TBarCollateralPercent.Scroll

        If TBarCollateralPercent.Value = 0 Then
            NUDSNOCollateral.Minimum = 0
            NUDSNOCollateral.Maximum = 0
            NUDSNOAmount.Maximum = 100
            NUDSNOCollateral.Value = 0.0
            LabColPercentage.Text = "0 %"

        Else

            NUDSNOAmount.Maximum = Decimal.MaxValue '79228162514264337593543950335

            Dim T_Amount As Decimal = NUDSNOAmount.Value
            Dim T_Percentage As Decimal = 28 + (TBarCollateralPercent.Value * 2)

            LabColPercentage.Text = T_Percentage.ToString + " %"

            If T_Amount > 0 Then
                NUDSNOCollateral.Minimum = (T_Amount / 100) * 30
                NUDSNOCollateral.Maximum = (T_Amount / 100) * 50
                NUDSNOCollateral.Value = (T_Amount / 100) * T_Percentage
            Else
                NUDSNOCollateral.Minimum = 0
                NUDSNOCollateral.Maximum = 1
                NUDSNOCollateral.Value = 0
            End If

        End If

    End Sub
    Private Sub NUDSNOCollateral_ValueChanged(sender As Object, e As EventArgs) Handles NUDSNOCollateral.ValueChanged

        Dim T_Amount As Decimal = NUDSNOAmount.Value
        Dim T_Collateral As Decimal = NUDSNOCollateral.Value

        Dim T_Percentage As Decimal = 0

        If T_Amount > 0 And T_Collateral > 0 Then
            T_Percentage = T_Collateral / T_Amount * 100
            'ElseIf T_Amount > 0 And TBarCollateralPercent.Value > 0 Then
            '    T_Percentage = 28 + (TBarCollateralPercent.Value * 2)
        End If

        T_Percentage = Math.Round(T_Percentage, 2)

        LabColPercentage.Text = T_Percentage.ToString + " %"
        'TBarCollateralPercent.Value = T_Percentage / 10

    End Sub
    Private Sub NUDSNOAmount_ValueChanged(sender As Object, e As EventArgs) Handles NUDSNOAmount.ValueChanged


        TBarCollateralPercent_Scroll(Nothing, Nothing)



        'Dim T_Amount As Decimal = NUDSNOAmount.Value
        'Dim T_Collateral As Decimal = NUDSNOCollateral.Value

        'Dim T_Percentage As Decimal = 0

        'If T_Amount > 0 And T_Collateral > 0 Then
        '    T_Percentage = T_Collateral / T_Amount * 100
        'ElseIf T_Amount > 0 And TBarCollateralPercent.Value > 0 Then
        '    T_Percentage = 28 + (TBarCollateralPercent.Value * 2)
        'End If

        'T_Percentage = Math.Round(T_Percentage, 2)

        'LabColPercentage.Text = T_Percentage.ToString + " %"
        'TBarCollateralPercent.Value = T_Percentage / 10

    End Sub


    Private Sub RBSNOSell_CheckedChanged(sender As Object, e As EventArgs) Handles RBSNOSell.CheckedChanged, RBSNOBuy.CheckedChanged

        If RBSNOSell.Checked Then
            LabWTX.Text = "WantToSell:"
        ElseIf RBSNOBuy.Checked Then
            LabWTX.Text = "WantToBuy:"
        End If

    End Sub
    Private Sub BtSNOSetOrder_Click(sender As Object, e As EventArgs) Handles BtSNOSetOrder.Click

        BtSNOSetOrder.Text = "Wait..."
        BtSNOSetOrder.Enabled = False

        Dim BSR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,, AccountID)

        Try

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
            Dim x As List(Of String) = BCR.GetAccountFromPassPhrase()

            TBSNOAddress.Text = BetweenFromList(x, "<address>", "</address>")
            TBSNOBalance.Text = BetweenFromList(x, "<available>", "</available>")
            AccountID = BetweenFromList(x, "<account>", "</account>")


            Dim MinAmount As Double = CDbl(NUDSNOAmount.Text) '100,00000000
            Dim XItemMinAmount As Double = CDbl(NUDSNOItemAmount.Value) '1,00000000


            If MinAmount > 0.0 And XItemMinAmount > 0.0 Then

                Dim MinXAmount As Double = 1.0

                For i As Integer = 1 To Decimals
                    MinXAmount *= 0.1
                Next

                MinXAmount = Math.Round(MinXAmount, 8)

                'If MarketIsCrypto Then
                If XItemMinAmount / MinAmount < MinXAmount Then
                    ClsMsgs.MBox("Minimum for one SIGNA must be greater than " + Dbl2LVStr(MinXAmount, Decimals) + " " + CurrentMarket + "!", "Value to low",,, ClsMsgs.Status.Erro)
                    BtSNOSetOrder.Text = "Set Order"
                    BtSNOSetOrder.Enabled = True
                    Exit Sub
                End If

            Else
                ClsMsgs.MBox("Minimum values must be greater than 0.0!", "Value to low",,, ClsMsgs.Status.Erro)
                BtSNOSetOrder.Text = "Set Order"
                BtSNOSetOrder.Enabled = True
                Exit Sub
            End If


            Dim AccAmount As Double = BetweenFromList(BSR.GetBalance(TBSNOAddress.Text), "<available>", "</available>")

            If LVOpenChannels.Items.Count > 0 Then

                Dim BLS As S_PFPAT = Nothing

                Dim FoundOne As Boolean = False

                For Each LVi As ListViewItem In LVOpenChannels.Items

                    Dim Status As String = GetLVColNameFromSubItem(LVOpenChannels, "Status", LVi)

                    If Status = "Reserved for you" Then
                        FoundOne = True
                        BLS = DirectCast(LVi.Tag, S_PFPAT)
                        Exit For
                    End If

                Next


                If Not FoundOne Then

                    For Each LVi As ListViewItem In LVOpenChannels.Items

                        If Not LVi.BackColor = Color.Crimson Then
                            BLS = DirectCast(LVi.Tag, S_PFPAT)

                            If CheckForUTX(, BLS.ATRS) Or CheckATforTX(BLS.ATRS) Then
                                BLS = Nothing ' New S_BLSAT
                            Else
                                Exit For
                            End If

                        End If

                    Next
                End If



                If IsNothing(BLS.ATRS) Then
                    ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                    BtSNOSetOrder.Text = "Set Order"
                    BtSNOSetOrder.Enabled = True
                    Exit Sub
                End If


                Dim Recipient As String = BLS.AT
                Dim Amount As Double = CDbl(NUDSNOAmount.Value)
                Dim Fee As Double = CDbl(NUDSNOTXFee.Value)
                Dim Collateral As Double = CDbl(NUDSNOCollateral.Value)
                Dim Item As String = CurrentMarket


                Dim ItemAmount As Double = CDbl(NUDSNOItemAmount.Value)
                Dim PassPhrase As String = TBSNOPassPhrase.Text

                BSR.C_PassPhrase = PassPhrase

                If RBSNOSell.Checked Then

                    If AccAmount > Amount + Fee + Collateral Then
                        'enough balance

                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new SellOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, Decimals) + " " + Item, "Create SellOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                            Dim TX As String = BSR.SetBLSATSellOrder(Recipient, Amount + Collateral + 1.0, Collateral, Item, ItemAmount, Fee)
                            If TX.Contains(Application.ProductName + "-error") Then
                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                out.ErrorLog2File(TX)
                            Else
                                ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                            End If

                        End If

                    Else
                        'not enough balance
                        ClsMsgs.MBox("not enough balance", "Error",,, ClsMsgs.Status.Erro)
                    End If

                ElseIf RBSNOBuy.Checked Then

                    If AccAmount > Collateral + Fee Then
                        'enough balance

                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new BuyOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, Decimals) + " " + Item, "Create BuyOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                            Dim TX As String = BSR.SetBLSATBuyOrder(Recipient, Amount, Collateral + 1.0, Item, ItemAmount, Fee)

                            If TX.Contains(Application.ProductName + "-error") Then
                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                out.ErrorLog2File(TX)
                            Else
                                ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                            End If


                        End If

                    Else
                        'not enough balance
                        ClsMsgs.MBox("not enough balance", "Error",,, ClsMsgs.Status.Erro)
                    End If

                End If

            Else
                ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                BtSNOSetOrder.Text = "Set Order"
                BtSNOSetOrder.Enabled = True
                Exit Sub

            End If

        Catch ex As Exception
            ClsMsgs.MBox(ex.Message, "Error",,, ClsMsgs.Status.Erro)
        End Try

        BtSNOSetOrder.Text = "Set Order"
        BtSNOSetOrder.Enabled = True

    End Sub

    Private Sub BtSNOSetCurFee_Click(sender As Object, e As EventArgs) Handles BtSNOSetCurFee.Click
        NUDSNOTXFee.Value = CDec(Fee)
    End Sub


    Private Sub LVSellorders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVSellorders.MouseUp
        BtBuy.Text = "Buy"
        LVSellorders.ContextMenuStrip = Nothing

        If LVSellorders.SelectedItems.Count > 0 Then

            Dim Order As S_PFPAT = DirectCast(LVSellorders.SelectedItems(0).Tag, S_PFPAT)

            If Order.Initiator = TBSNOAddress.Text Then
                BtBuy.Text = "cancel"
            Else

                Dim SAPI As ClsSignumAPI = New ClsSignumAPI

                Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerPubKey.Text = "copy seller public key"
                LVCMItemSellerPubKey.Tag = SAPI.GetAccountPublicKeyFromAccountID_RS(Order.Initiator)

                AddHandler LVCMItemSellerPubKey.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerPubKey)

                Dim LVCMItemSellerID As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerID.Text = "copy sellerID"
                LVCMItemSellerID.Tag = GetAccountIDFromRS(Order.Initiator)

                AddHandler LVCMItemSellerID.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerID)

                Dim LVCMItemSellerRS As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerRS.Text = "copy sellerRS"
                LVCMItemSellerRS.Tag = Order.Initiator

                AddHandler LVCMItemSellerRS.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerRS)

                LVSellorders.ContextMenuStrip = LVContextMenu

                BtBuy.Text = "Buy"
            End If

        End If
    End Sub
    Private Sub LVBuyorders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVBuyorders.MouseUp
        BtSell.Text = "Sell"
        LVBuyorders.ContextMenuStrip = Nothing

        If LVBuyorders.SelectedItems.Count > 0 Then

            Dim Order As S_PFPAT = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_PFPAT)

            If Order.Initiator = TBSNOAddress.Text Then
                BtSell.Text = "cancel"
            Else

                Dim SAPI As ClsSignumAPI = New ClsSignumAPI

                Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerPubKey.Text = "copy buyer public key"
                LVCMItemSellerPubKey.Tag = SAPI.GetAccountPublicKeyFromAccountID_RS(Order.Initiator)

                AddHandler LVCMItemSellerPubKey.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerPubKey)

                Dim LVCMItemSellerID As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerID.Text = "copy buyerID"
                LVCMItemSellerID.Tag = GetAccountIDFromRS(Order.Initiator)

                AddHandler LVCMItemSellerID.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerID)

                Dim LVCMItemSellerRS As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerRS.Text = "copy buyerRS"
                LVCMItemSellerRS.Tag = Order.Initiator

                AddHandler LVCMItemSellerRS.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerRS)

                LVBuyorders.ContextMenuStrip = LVContextMenu

                BtSell.Text = "Sell"
            End If

        End If
    End Sub


    Private Sub BtBuy_Click(sender As Object, e As EventArgs) Handles BtBuy.Click

        Dim OldTxt As String = BtBuy.Text

        BtBuy.Text = "Wait..."
        BtBuy.Enabled = False

        If LVSellorders.SelectedItems.Count > 0 Then

            Dim BLS As S_PFPAT = DirectCast(LVSellorders.SelectedItems(0).Tag, S_PFPAT)

            If CheckForUTX("", BLS.ATRS) Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtBuy.Text = OldTxt
                BtBuy.Enabled = True
                Exit Sub
            End If

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
            Dim BLSAT_TX As S_PFPAT_TX = GetLastTXWithValues(BLS.AT_TXList, CurrentMarket)
            Dim Amount As Double = BCR.Planck2Dbl(CULng(Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))


            If Not BLS.Initiator = TBSNOAddress.Text Then
                Dim BCR2 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

                'AccountID = BCR2.BetweenFromList(x, "<account>", "</account>")
                'TBSNOAddress.Text = BCR2.BetweenFromList(x, "<address>", "</address>")

                Dim Available As Double = 0.0
                Dim AvaStr As String = BetweenFromList(x, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Amount + 1.0 Then

                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to Buy " + Dbl2LVStr(BLS.BuySellAmount) + " SIGNA for " + Dbl2LVStr(BCR.Planck2Dbl(BLS.XAmount), Decimals) + " " + BLS.XItem + "?" + vbCrLf + vbCrLf + "collateral: " + Dbl2LVStr(BLS.InitiatorsCollateral) + " SIGNA" + vbCrLf + "handling fees: " + Dbl2LVStr(1.0) + " SIGNA", "Buy Order?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                        Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, Amount + 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))


                        If TX.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TX)
                        Else
                            ClsMsgs.MBox("SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                        End If


                    End If

                Else
                    'not enough balance
                    Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance for the AT acception." + vbCrLf + "do you like to ask the seller for accepting this offer offchain?", "not enough balance", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Question)

                    If Result = ClsMsgs.CustomDialogResult.Yes Then
                        'TODO: Check RecipientPublicKey
                        DEXNET.BroadcastMessage("<ATID>" + BLS.AT + "</ATID><Ask>WTB</Ask>", T_SignkeyHEX, T_AgreementKeyHEX, T_PublicKeyHEX, BCR.GetAccountPublicKeyFromAccountID_RS(BLS.Initiator))
                    End If

                End If
            Else

                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the SellOrder?", "Cancel SellOrder?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                    Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                    If TX.Contains(Application.ProductName + "-error") Then
                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                        out.ErrorLog2File(TX)
                    Else
                        ClsMsgs.MBox("SellOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                    End If

                End If

            End If



        End If

        BtBuy.Text = OldTxt
        BtBuy.Enabled = True

    End Sub
    Private Sub BtSell_Click(sender As Object, e As EventArgs) Handles BtSell.Click

        Dim OldTxt As String = BtSell.Text

        BtSell.Text = "Wait..."
        BtSell.Enabled = False

        If LVBuyorders.SelectedItems.Count > 0 Then

            Dim BLS As S_PFPAT = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_PFPAT)

            If CheckForUTX(, BLS.ATRS) Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtSell.Text = OldTxt
                BtSell.Enabled = True
                Exit Sub
            End If

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
            Dim BLSAT_TX As S_PFPAT_TX = GetLastTXWithValues(BLS.AT_TXList, CurrentMarket)
            Dim Amount As Double = BCR.Planck2Dbl(CULng(Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))
            Dim XItem As String = Between(BLSAT_TX.Attachment, "<xItem>", "</xItem>", GetType(String))
            Dim XAmount As Double = BCR.Planck2Dbl(CULng(Between(BLSAT_TX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
            Dim Sum As Double = Amount + BCR.Planck2Dbl(CULng(BLSAT_TX.AmountNQT))

            If Not BLS.Initiator = TBSNOAddress.Text Then

                Dim BCR2 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

                Dim Available As Double = 0.0
                Dim AvaStr As String = BetweenFromList(x, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Amount + 1.0 Then

                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to Sell " + Dbl2LVStr(BLS.BuySellAmount) + " SIGNA for " + Dbl2LVStr(BCR.Planck2Dbl(BLS.XAmount), Decimals) + " " + BLS.XItem + "?" + vbCrLf + vbCrLf + "collateral: " + Dbl2LVStr(BLS.InitiatorsCollateral) + " SIGNA" + vbCrLf + "handling fees: " + Dbl2LVStr(1.0) + " SIGNA", "Sell Order?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, Sum, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                        If TX.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TX)

                        Else

                            Dim PayInfo As String = GetPaymentInfoFromOrderSettings(BLSAT_TX.Transaction, Amount, XAmount, CurrentMarket)

                            If Not PayInfo.Trim = "" Then

                                If PayInfo.Contains("PayPal-E-Mail=") Then
                                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(BLSAT_TX.Transaction, True, "-", 5)

                                    PayInfo += " Reference/Note=" + ColWordsString
                                End If

                                Dim T_MsgStr As String = "AT=" + BLS.ATRS + " TX=" + BLSAT_TX.Transaction + " " + Dbl2LVStr(BCR.Planck2Dbl(BLS.XAmount), Decimals) + " " + BLS.XItem + " " + PayInfo
                                Dim TXr As String = SendBillingInfos(BLSAT_TX.Sender, T_MsgStr)

                                If TXr.Contains(Application.ProductName + "-error") Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(TXr)
                                Else

                                End If

                            End If

                            ClsMsgs.MBox("BuyOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                        End If

                    End If

                Else
                    'not enough balance
                    ClsMsgs.MBox("not enough balance", "Error",,, ClsMsgs.Status.Erro)
                End If

            Else

                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the BuyOrder?", "Cancel BuyOrder?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                    Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                    If TX.Contains(Application.ProductName + "-error") Then
                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                        out.ErrorLog2File(TX)
                    Else
                        ClsMsgs.MBox("BuyOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                    End If

                End If

            End If

        End If

        BtSell.Text = OldTxt
        BtSell.Enabled = True

    End Sub

#End Region

#Region "MyOrders - Controls"
    Private Sub LVMyOpenOrders_MouseDown(sender As Object, e As MouseEventArgs) Handles LVMyOpenOrders.MouseDown

        LVMyOpenOrders.ContextMenuStrip = Nothing
        Label15.Visible = False
        TBManuMsg.Visible = False
        BtSendMsg.Visible = False
        ChBxEncMsg.Visible = False
        BtExecuteOrder.Visible = False

    End Sub
    Private Sub LVMyOpenOrders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVMyOpenOrders.MouseUp
        LVMyOpenOrders.ContextMenuStrip = Nothing

        If LVMyOpenOrders.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)

            Dim Seller As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVi)
            Dim Buyer As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVi)
            Dim Status As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Status", LVi)

            Dim Order As S_Order = LVi.Tag


            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem.Text = "copy status"
            LVCMItem.Tag = Status

            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            If Status.Contains("http") Then
                'only shows on buyer side

                If Buyer = "Me" Then

                    BtExecuteOrder.Text = "cancel AT"
                    BtExecuteOrder.Visible = True
                    BtPayOrder.Visible = True

                    Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem1.Text = "cancel AT"
                    AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                    LVContextMenu.Items.Add(LVCMItem1)

                    Dim LVCMItem2 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem2.Text = "pay Order"
                    AddHandler LVCMItem2.Click, AddressOf BtPayOrder_Click
                    LVContextMenu.Items.Add(LVCMItem2)

                ElseIf Seller = "Me" Then

                    BtExecuteOrder.Text = "execute AT"
                    BtExecuteOrder.Visible = True

                    Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem1.Text = "execute AT"
                    AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                    LVContextMenu.Items.Add(LVCMItem1)

                End If

            ElseIf Status = "No Payment received" Then
                'only shows on seller side

                If Buyer = "Me" Then
                    BtExecuteOrder.Text = "cancel AT"

                    If Buyer.Trim = "" Or Seller.Trim = "" Then
                        BtExecuteOrder.Visible = False
                    Else
                        BtExecuteOrder.Visible = True
                        BtReCreatePayPalOrder.Visible = False
                        BtPayOrder.Visible = False

                        Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                        LVCMItem1.Text = "cancel AT"
                        AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                        LVContextMenu.Items.Add(LVCMItem1)

                    End If

                ElseIf Seller = "Me" Then

                    BtExecuteOrder.Text = "execute anyway"

                    If Buyer.Trim = "" Or Seller.Trim = "" Then
                        BtExecuteOrder.Visible = False
                    Else
                        BtExecuteOrder.Visible = True
                        BtReCreatePayPalOrder.Visible = True
                        BtPayOrder.Visible = False

                        Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                        LVCMItem1.Text = "execute anyway"
                        AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                        LVContextMenu.Items.Add(LVCMItem1)

                    End If

                End If


            Else
                'shows on both sides

                BtPayOrder.Visible = False
                Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem

                If Buyer = "Me" Then
                    BtExecuteOrder.Text = "cancel AT"
                    LVCMItem1.Text = "cancel AT"
                ElseIf Seller = "Me" Then
                    BtExecuteOrder.Text = "execute anyway"
                    LVCMItem1.Text = "execute anyway"
                End If

                If Buyer.Trim = "" Or Seller.Trim = "" Then
                    BtExecuteOrder.Text = "cancel AT"
                    LVCMItem1.Text = "cancel AT"
                    BtExecuteOrder.Visible = True

                Else
                    BtExecuteOrder.Visible = True

                    'If Not ChBxAutoSendPaymentInfo.Checked Then

                    'If Order.Seller = TBSNOAddress.Text Then
                    Label15.Visible = True
                    TBManuMsg.Visible = True
                    BtSendMsg.Visible = True
                    ChBxEncMsg.Visible = True
                    'End If

                    'Else

                    '    Label15.Visible = False
                    '    TBManuMsg.Visible = False
                    '    BtSendMsg.Visible = False
                    '    ChBxEncMsg.Visible = False

                    'End If

                End If

                AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                LVContextMenu.Items.Add(LVCMItem1)

            End If


            LVMyOpenOrders.ContextMenuStrip = LVContextMenu



            'AlreadySend = "COMPLETED"
            'AlreadySend = "PayPal Order created"
            'AlreadySend = "No Payment received"


        End If

    End Sub


    Private Sub LVMyClosedOrders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVMyClosedOrders.MouseUp

        LVMyClosedOrders.ContextMenuStrip = Nothing

        If LVMyClosedOrders.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMyClosedOrders.SelectedItems(0)

            Dim FirstTransaction As String = GetLVColNameFromSubItem(LVMyClosedOrders, "First Transaction", LVi)
            Dim LastTransaction As String = GetLVColNameFromSubItem(LVMyClosedOrders, "Last Transaction", LVi)
            Dim Seller As String = GetLVColNameFromSubItem(LVMyClosedOrders, "Seller", LVi)
            Dim Buyer As String = GetLVColNameFromSubItem(LVMyClosedOrders, "Buyer", LVi)


            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip


            Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem1.Text = "copy first transaction"
            LVCMItem1.Tag = FirstTransaction

            AddHandler LVCMItem1.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem1)


            Dim LVCMItem2 As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem2.Text = "copy last transaction"
            LVCMItem2.Tag = LastTransaction

            AddHandler LVCMItem2.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem2)


            LVMyClosedOrders.ContextMenuStrip = LVContextMenu

        End If


    End Sub


    Private Sub Copy2CB(sender As Object, e As EventArgs)

        Try
            If Not IsNothing(sender.tag) Then
                Clipboard.SetText(sender.tag.ToString)

                'Dim Out As out = New out(Application.StartupPath)

                'For Each LVI As ListViewItem In LVMyClosedOrders.Items
                '    Out.ToFile(LVI.SubItems(0).Text + vbCrLf)
                '    Out.ToFile(LVI.SubItems(1).Text + vbCrLf)
                'Next

            Else

            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub BtExecuteOrder_Click(sender As Object, e As EventArgs) Handles BtExecuteOrder.Click

        Dim OldTxt As String = BtExecuteOrder.Text

        BtExecuteOrder.Text = "Wait..."
        BtExecuteOrder.Enabled = False

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim TX As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            If CheckForUTX(, TX.ATRS) Or CheckATforTX(TX.ATRS) Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtExecuteOrder.Text = OldTxt
                BtExecuteOrder.Enabled = True
                Exit Sub
            End If


            If TX.Type = "SellOrder" Then

                If TX.Buyer.Trim = "" Then
                    'cancel AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the AT?", "cancel AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                        If TXStr.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TXStr)
                        Else
                            ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                        End If

                    End If

                Else
                    'execute AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to execute the AT?", "execute AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceFinishOrder}))

                        If TXStr.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TXStr)
                        Else
                            ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                        End If

                    End If

                End If

            Else 'BuyOrder

                If TX.Seller.Trim = "" Then
                    'cancel AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the AT?", "cancel AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                        If TXStr.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TXStr)
                        Else
                            ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                        End If

                    End If

                Else
                    'execute AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to execute the AT?", "execute AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceFinishOrder}))

                        If TXStr.Contains(Application.ProductName + "-error") Then
                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                            out.ErrorLog2File(TXStr)
                        Else
                            ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                        End If

                    End If

                End If

            End If

        End If

        BtExecuteOrder.Text = OldTxt
        BtExecuteOrder.Enabled = True

    End Sub
    Private Sub BtPayOrder_Click(sender As Object, e As EventArgs) Handles BtPayOrder.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)

            Dim Status As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Status", LVi)

            Process.Start(Status)

        End If

    End Sub
    Private Sub BtReCreatePayPalOrder_Click(sender As Object, e As EventArgs) Handles BtReCreatePayPalOrder.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim Order As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            If CheckForUTX(, Order.ATRS) Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                Exit Sub
            End If

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)


            Dim Amount As Double = BCR.Planck2Dbl(CULng(Between(Order.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))
            Dim XItem As String = Between(Order.Attachment, "<xItem>", "</xItem>", GetType(String))
            Dim XAmount As Double = BCR.Planck2Dbl(CULng(Between(Order.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
            Dim Sum As Double = Amount + BCR.Planck2Dbl(CULng(Order.FirstTX.AmountNQT))

            Dim PayInfo As String = GetPaymentInfoFromOrderSettings(Order.FirstTransaction, Amount, XAmount, CurrentMarket)

            If Not PayInfo.Trim = "" Then

                If PayInfo.Contains("PayPal-E-Mail=") Then
                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(Order.FirstTransaction, True, "-", 5)

                    PayInfo += " Reference/Note=" + ColWordsString
                End If

                Dim T_MsgStr As String = "AT=" + Order.ATRS + " TX=" + Order.FirstTransaction + " " + Dbl2LVStr(Order.XAmount, Decimals) + " " + Order.XItem + " " + PayInfo
                Dim TXr As String = SendBillingInfos(Order.FirstTX.Sender, T_MsgStr)

                If TXr.Contains(Application.ProductName + "-error") Then
                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                    out.ErrorLog2File(TXr)
                Else
                    ClsMsgs.MBox("New PayPal Order sended as encrypted Message", "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

            End If

        End If

    End Sub
    Private Sub BtSendMsg_Click(sender As Object, e As EventArgs) Handles BtSendMsg.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim Order As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            Dim T_Infotext As String = "Infotext=" + TBManuMsg.Text.Replace(",", ";").Replace(":", ";").Replace("""", ";")
            Dim T_MsgStr As String = "AT=" + Order.ATRS + " TX=" + Order.FirstTransaction + " " + Dbl2LVStr(Order.XAmount, Decimals) + " " + Order.XItem + " " + T_Infotext

            Dim Recipient As String = Order.Buyer

            If Order.Buyer = TBSNOAddress.Text Then
                Recipient = Order.Seller
            End If

            Dim TXr As String = SendBillingInfos(Recipient, T_MsgStr, ChBxEncMsg.Checked)

            If TXr.Contains(Application.ProductName + "-error") Then
                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                out.ErrorLog2File(TXr)
            Else
                If ChBxEncMsg.Checked Then
                    ClsMsgs.MBox("encrypted Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                Else
                    ClsMsgs.MBox("public Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

                TBManuMsg.Text = ""

            End If

        End If

    End Sub

#End Region

#Region "Settings - Controls"
    Dim OldPaymentinfoText As String = Nothing

    'Private Sub ChBxAutoSendPaymentInfo_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxAutoSendPaymentInfo.CheckedChanged

    '    If ChBxAutoSendPaymentInfo.Checked Then
    '        TBPaymentInfo.Enabled = True
    '        'ChBxUsePayPalSettings.Enabled = True
    '    Else

    '        TBPaymentInfo.Enabled = False
    '        'ChBxUsePayPalSettings.Enabled = False
    '    End If

    'End Sub

    'Private Sub ChBxUsePayPalSettings_CheckedChanged(sender As Object, e As EventArgs)

    '    If ChBxUsePayPalSettings.Checked Then

    '        TBPaymentInfo.Enabled = False
    '        OldPaymentinfoText = TBPaymentInfo.Text
    '    Else

    '        TBPaymentInfo.Enabled = True
    '    End If

    '    RBPayPalEMail_CheckedChanged(Nothing, Nothing)

    'End Sub

    Private Sub TVSettings_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TVSettings.AfterSelect


        If SCSettings.Panel2.Controls.Count > 0 Then
            Dim T_FRM As Form = SCSettings.Panel2.Controls(0)
            T_FRM.Close()
        End If

        Dim TNS As TreeNode = TVSettings.SelectedNode

        Select Case TNS.Text
            Case "Default Settings"

                Dim FrmGenSet As New FrmGeneralSettings(Me)
                FrmGenSet.Dock = DockStyle.Fill
                FrmGenSet.TopMost = True
                FrmGenSet.TopLevel = False

                SCSettings.Panel2.Controls.Add(FrmGenSet)
                FrmGenSet.Show()

            Case "MyOrders Settings"

                Dim FrmMyOrderSettings As New FrmMyOrderSettings(Me)
                FrmMyOrderSettings.Dock = DockStyle.Fill
                FrmMyOrderSettings.TopMost = True
                FrmMyOrderSettings.TopLevel = False

                SCSettings.Panel2.Controls.Add(FrmMyOrderSettings)
                FrmMyOrderSettings.Show()

            Case "Develope"

                Dim FrmDev As New FrmDevelope(Me)
                FrmDev.Dock = DockStyle.Fill
                FrmDev.TopMost = True
                FrmDev.TopLevel = False

                SCSettings.Panel2.Controls.Add(FrmDev)
                FrmDev.Show()

        End Select


    End Sub
    Function GetSubForm(ByVal Template As String) As Form

        If SCSettings.Panel2.Controls.Count > 0 Then
            Dim T_Frm As Form = SCSettings.Panel2.Controls(0)

            If T_Frm.Name = Template Then
                Return T_Frm
            End If

        End If

        Return Nothing

    End Function

#End Region

#End Region

#Region "Methods/Functions"

    Function GetMarketCurrencyIsCrypto(ByVal Currency As String) As Boolean

        Select Case Currency

            Case "AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"
                'Non CryptoCurrencys Supported by PayPal
                Return False

        End Select

        Return True

    End Function

    Function GetCurrencyDecimals(ByVal Currency As String) As Integer

        Select Case Currency

            Case "AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "INR", "ILS", "MYR", "MXN", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"
                'Non CryptoCurrencys Supported by PayPal
                Return 2

            Case "HUF", "JPY", "TWD"
                'These Currencys do not support decimals
                Return 0

            Case Else
                Return 8

        End Select

        Return 8

    End Function

    Function GetAndCheckATs() As Boolean

        Try

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim ATList As List(Of String) = BCR.GetATIds()
            Dim CSVATList As List(Of String()) = GetATsFromCSV()


            Dim Nu_ATList As List(Of String) = New List(Of String)

            For Each AT As String In ATList

                Dim Found As Boolean = False
                For Each CSVAT As String() In CSVATList

                    If AT = CSVAT(0) Then
                        Found = True
                        Exit For
                    End If
                Next

                If Not Found Then
                    Nu_ATList.Add(AT)
                End If

            Next


            For Each CSVAT As String() In CSVATList

                If CSVAT(2).Trim = "True" Then
                    Nu_ATList.Add(CSVAT(0))
                End If

            Next


            APIRequestList.Clear()

            For Each AT As String In Nu_ATList
                Dim TestMulti As S_APIRequest = New S_APIRequest

                TestMulti.Command = "GetDetails(" + AT.Trim + ")"
                TestMulti.Status = "Wait..."

                APIRequestList.Add(TestMulti)
            Next

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in GetAndCheckATs(): -> " + ex.Message)
        End Try

        Return True

    End Function

    Function ResetLVColumns() As Boolean

        LVSellorders.Columns.Clear()
        LVSellorders.Columns.Add("Price (" + CurrentMarket + ")")
        LVSellorders.Columns.Add("Amount (" + CurrentMarket + ")")
        LVSellorders.Columns.Add("Total (SIGNA)")
        LVSellorders.Columns.Add("Collateral (SIGNA)")
        LVSellorders.Columns.Add("Method")
        LVSellorders.Columns.Add("Autoinfo")
        LVSellorders.Columns.Add("Autofinish")
        LVSellorders.Columns.Add("Seller")
        LVSellorders.Columns.Add("AT")


        LVBuyorders.Columns.Clear()
        With LVBuyorders.Columns.Add("Price (" + CurrentMarket + ")")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Amount (" + CurrentMarket + ")")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Total (SIGNA)")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Collateral (SIGNA)")
            .TextAlign = HorizontalAlignment.Right
        End With
        LVBuyorders.Columns.Add("Method")
        LVBuyorders.Columns.Add("Autoinfo")
        LVBuyorders.Columns.Add("Autofinish")
        LVBuyorders.Columns.Add("Buyer")
        LVBuyorders.Columns.Add("AT")


        LVMyOpenOrders.Columns.Clear()
        LVMyOpenOrders.Columns.Add("Confirmations")
        LVMyOpenOrders.Columns.Add("AT")
        LVMyOpenOrders.Columns.Add("Type")
        LVMyOpenOrders.Columns.Add("Method")
        LVMyOpenOrders.Columns.Add("Autoinfo")
        LVMyOpenOrders.Columns.Add("Autofinish")
        LVMyOpenOrders.Columns.Add("Seller")
        LVMyOpenOrders.Columns.Add("Buyer")
        LVMyOpenOrders.Columns.Add("XItem")
        LVMyOpenOrders.Columns.Add("XAmount")
        LVMyOpenOrders.Columns.Add("Quantity")
        LVMyOpenOrders.Columns.Add("Price")
        LVMyOpenOrders.Columns.Add("Status")


        LVMyClosedOrders.Columns.Clear()
        LVMyClosedOrders.Columns.Add("First Transaction")
        LVMyClosedOrders.Columns.Add("Last Transaction")
        LVMyClosedOrders.Columns.Add("Confirmations")
        LVMyClosedOrders.Columns.Add("AT")
        LVMyClosedOrders.Columns.Add("Type")
        ' LVMyClosedOrders.Columns.Add("Method")
        LVMyClosedOrders.Columns.Add("Seller")
        LVMyClosedOrders.Columns.Add("Buyer")
        LVMyClosedOrders.Columns.Add("XItem")
        LVMyClosedOrders.Columns.Add("XAmount")
        LVMyClosedOrders.Columns.Add("Quantity")
        LVMyClosedOrders.Columns.Add("Price")
        LVMyClosedOrders.Columns.Add("Status")
        'LVMyClosedOrders.Columns.Add("Conditions")

        Return True

    End Function

    Function SetInLVs()

        Try

            Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

#Region "set LVs"

            OpenChannelLVIList.Clear()
            BuyOrderLVIList.Clear()
            SellOrderLVIList.Clear()
            MyOpenOrderLVIList.Clear()
            MyClosedOrderLVIList.Clear()

            Dim Wait As Boolean = ProcessingATs()

            BtBuy.Text = "Buy"
            BtSell.Text = "Sell"

            BtPayOrder.Visible = False
            BtReCreatePayPalOrder.Visible = False
            BtExecuteOrder.Visible = False

            Label15.Visible = False 'Sendmessage MyOpenOrders
            TBManuMsg.Visible = False
            BtSendMsg.Visible = False
            ChBxEncMsg.Visible = False


            LVOpenChannels.Visible = False
            LVSellorders.Visible = False
            LVBuyorders.Visible = False
            LVMyOpenOrders.Visible = False
            LVMyClosedOrders.Visible = False



            LVOpenChannels.Items.Clear()
            For Each OpenChannel As ListViewItem In OpenChannelLVIList
                MultiInvoker(LVOpenChannels, "Items", {"Add", OpenChannel})
            Next

            LVBuyorders.Items.Clear()
            For Each BuyOrder As ListViewItem In BuyOrderLVIList
                MultiInvoker(LVBuyorders, "Items", {"Add", BuyOrder})
            Next

            LVSellorders.Items.Clear()
            For Each OpeSellOrdernChannel As ListViewItem In SellOrderLVIList
                MultiInvoker(LVSellorders, "Items", {"Add", OpeSellOrdernChannel})
            Next

            LVMyOpenOrders.Items.Clear()
            For Each MyOpenOrder As ListViewItem In MyOpenOrderLVIList
                MultiInvoker(LVMyOpenOrders, "Items", {"Add", MyOpenOrder})
            Next

            LVMyClosedOrders.Items.Clear()
            For Each MyClosedOrder As ListViewItem In MyClosedOrderLVIList
                MultiInvoker(LVMyClosedOrders, "Items", {"Add", MyClosedOrder})
            Next

            For Each BroadcastMsg As String In BroadcastMsgs
                DEXNET.BroadcastMessage(BroadcastMsg, GetSignKeyHEX(TBSNOPassPhrase.Text.Trim),, GetPubKeyHEX(TBSNOPassPhrase.Text.Trim))
            Next

            BroadcastMsgs.Clear()


            LVMyOpenOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0)
            LVMyClosedOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 2)

            LVSellorders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0)
            LVSellorders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

            LVBuyorders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Descending, 0)
            LVBuyorders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

            LVOpenChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

            LVMyOpenOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
            LVMyClosedOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

#End Region
            'setLVs

            Dim T_BlockFeeThread As Threading.Thread = New Threading.Thread(AddressOf BlockFeeThread)

            StatusBlockLabel.Text = "loading Blockheight..."
            StatusFeeLabel.Text = "loading Current Slotfee..."
            StatusStrip1.Refresh()

            T_BlockFeeThread.Start(PrimaryNode)

            'BLSAT_TX_List.Clear()
            'BLSAT_TX_List.AddRange(T_BLSAT_TX_List.ToArray)
            'BLSAT_TX_List = BLSAT_TX_List.OrderBy(Function(BLSAT_TX As S_BLSAT_TX) BLSAT_TX.Timestamp).ToList

            StatusBar.Value = 0
            StatusBar.Maximum = 100
            StatusBar.Visible = False
            StatusLabel.Text = ""

            LVOpenChannels.Visible = True
            LVSellorders.Visible = True
            LVBuyorders.Visible = True
            LVMyOpenOrders.Visible = True
            LVMyClosedOrders.Visible = True

            'SplitContainer2.Panel1.Visible = True

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in SetInLVs(): -> " + ex.Message)
        End Try

        Return True

    End Function


    Function ProcessingATs() As Boolean

        Try

            OrderList.Clear()
            MTSAT2LVList.Clear()

            OrderSettingsBuffer.Clear()
            OrderSettingsBuffer = GetOrderSettings()

            For Each PFPAT As S_PFPAT In PFPList

                OrderList.AddRange(PFPAT.AT_OrderList)

                Dim T_SetLVThreadStruc As S_MultiThreadSetAT2LV = New S_MultiThreadSetAT2LV

                T_SetLVThreadStruc.SubThread = New Threading.Thread(AddressOf MultiThreadSetAT2LV)

                Dim Index As Integer = MTSAT2LVList.Count
                T_SetLVThreadStruc.AT = PFPAT
                T_SetLVThreadStruc.Market = CurrentMarket

                Application.DoEvents()

                MTSAT2LVList.Add(T_SetLVThreadStruc)

                T_SetLVThreadStruc.SubThread.Start({Index})

            Next


            Dim exiter As Boolean = False
            While Not exiter
                exiter = True

                Application.DoEvents()

                For Each S_TH As S_MultiThreadSetAT2LV In MTSAT2LVList

                    Application.DoEvents()

                    If S_TH.SubThread.IsAlive Then

                        MultiInvoker(StatusLabel, "Text", "Processing " + S_TH.AT.ATRS)
                        Application.DoEvents()

                        exiter = False
                        Exit For
                    End If

                Next

            End While

            SaveOrderSettingsToCSV(OrderSettingsBuffer)

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in ProcessingATs(): -> " + ex.Message)
        End Try

        Return True

    End Function


    Structure S_MultiThreadSetAT2LV
        Dim SubThread As Threading.Thread
        Dim AT As S_PFPAT
        'Dim ATListIndex As Integer
        Dim Market As String
    End Structure

    Property MTSAT2LVList As List(Of S_MultiThreadSetAT2LV) = New List(Of S_MultiThreadSetAT2LV)

    Property OpenChannelLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property BuyOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property SellOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property MyOpenOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property MyClosedOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)

    Property BroadcastMsgs As List(Of String) = New List(Of String)

    Sub MultiThreadSetAT2LV(ByVal Input As Object)

        Try

            Dim Index As Integer = DirectCast(Input(0), Integer)
            Dim S_MTSetAT2LV As S_MultiThreadSetAT2LV = MTSAT2LVList(Index)

            Dim Market As String = S_MTSetAT2LV.Market
            Dim PFPAT As S_PFPAT = S_MTSetAT2LV.AT


            Dim SigAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim T_LVI As ListViewItem = New ListViewItem

            If PFPAT.AT_TXList.Count = 0 Then

                If PFPAT.CreatorRS = TBSNOAddress.Text Then

                    T_LVI.Text = PFPAT.ATRS 'SmartContract
                    T_LVI.SubItems.Add("Reserved for you") 'Status
                    T_LVI.Tag = PFPAT

                    OpenChannelLVIList.Add(T_LVI)

                End If

            Else 'TXList > 0

                Dim ATChannelOpen As Boolean = True

                For Each Order As S_Order In PFPAT.AT_OrderList

#Region "Set To LVs"

#Region "Confirmations"
                    Dim Confirms As String = "0"

                    If Not IsNothing(Order.LastTX.Confirmations) Then
                        Confirms = Order.LastTX.Confirmations.ToString
                    Else
                        Confirms = Order.FirstTX.Confirmations.ToString
                    End If
#End Region

#Region "XItem/Payment Method"

                    Dim XItem As String = Order.XItem
                    Dim PayMet As String = "Unknown"

                    'If XItem.Contains("-") Then
                    '    PayMet = XItem.Split("-")(1)
                    '    XItem = XItem.Split("-")(0)
                    'End If

#End Region

                    If Order.Status = "OPEN" Then

                        ATChannelOpen = False

                        Try

                            If Order.XItem.Contains(Market) Then

#Region "Market - GUI"

                                Dim Autosendinfotext As String = "False"
                                Dim AutocompleteAT As String = "False"

                                If Not CheckForUTX(, Order.ATRS) And Not CheckATforTX(PFPAT.ATRS) Then

                                    'TODO: Debug Double OPEN And RESERVED Orders by Injected Recipients

                                    If Order.Type = "SellOrder" Then

                                        T_LVI = New ListViewItem

                                        T_LVI.Text = Dbl2LVStr(Order.Price, Decimals) 'price
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral

                                        Dim T_SellerRS As String = Order.Seller
                                        If T_SellerRS = TBSNOAddress.Text Then

                                            Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(Order.FirstTransaction)

                                            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(Order.AT, Order.FirstTransaction, Order.Type, Order.Status)

                                            If T_OSList.Count = 0 Then
                                                T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                                T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                                OrderSettingsBuffer.Add(T_OS)
                                            Else
                                                T_OS = T_OSList(0)
                                                T_OS.SetPayType()
                                                PayMet = T_OS.PaytypeString
                                                Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                                AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                            End If

                                            T_SellerRS = "Me"
                                            T_LVI.BackColor = Color.Magenta
                                        Else

#Region "SellOrder-Info from DEXNET"
                                            Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                            If Not IsNothing(DEXNET) Then
                                                RelMsgs = DEXNET.GetRelevantMsgs()
                                            End If


                                            Dim RelKeyFounded As Boolean = False

                                            For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                                If RelMsg.RelevantKey = "<ATID>" + PFPAT.AT.Trim + "</ATID>" Then
                                                    RelKeyFounded = True

                                                    If Not RelMsg.RelevantMessage.Trim = "" Then

                                                        Dim PublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))

                                                        If Not T_SellerRS = TBSNOAddress.Text Then

                                                            If Order.SellerID.Trim = GetAccountID(PublicKey).ToString Then

                                                                Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                                                                PayMet = PayMethod

                                                                Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                                                                Autosendinfotext = T_Autosendinfotext

                                                                Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                                                                AutocompleteAT = T_AutocompleteAT

                                                            End If

                                                        End If


                                                    End If

                                                End If

                                            Next

                                            If Not RelKeyFounded Then
                                                If Not IsNothing(DEXNET) Then
                                                    DEXNET.AddRelevantKey("<ATID>" + PFPAT.AT.Trim + "</ATID>")
                                                End If
                                            End If


#End Region
                                        End If

                                        T_LVI.SubItems.Add(PayMet) 'payment method
                                        T_LVI.SubItems.Add(Autosendinfotext) 'autosend infotext
                                        T_LVI.SubItems.Add(AutocompleteAT) 'autocomplete at
                                        T_LVI.SubItems.Add(T_SellerRS) 'seller
                                        T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                        T_LVI.Tag = PFPAT

                                        SellOrderLVIList.Add(T_LVI)

                                    Else 'BuyOrder

                                        T_LVI = New ListViewItem

                                        T_LVI.Text = Dbl2LVStr(Order.Price, Decimals) 'price
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                        T_LVI.SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral

                                        Dim T_BuyerRS As String = Order.Buyer
                                        If T_BuyerRS = TBSNOAddress.Text Then

                                            Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(Order.FirstTransaction)
                                            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(Order.AT, Order.FirstTransaction, Order.Type, Order.Status)

                                            If T_OSList.Count = 0 Then
                                                T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                                T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                                OrderSettingsBuffer.Add(T_OS)
                                            Else
                                                T_OS = T_OSList(0)
                                                T_OS.SetPayType()
                                                PayMet = T_OS.PaytypeString
                                                Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                                AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                            End If

                                            T_BuyerRS = "Me"
                                            T_LVI.BackColor = Color.Magenta
                                        Else

#Region "BuyOrder-Info from DEXNET"
                                            Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                            If Not IsNothing(DEXNET) Then
                                                RelMsgs = DEXNET.GetRelevantMsgs()
                                            End If

                                            Dim RelKeyFounded As Boolean = False
                                            For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                                If RelMsg.RelevantKey = "<ATID>" + PFPAT.AT.Trim + "</ATID>" Then
                                                    RelKeyFounded = True

                                                    If Not RelMsg.RelevantMessage.Trim = "" Then

                                                        Dim PublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))

                                                        If Not T_BuyerRS = TBSNOAddress.Text Then

                                                            If Order.BuyerID.Trim = GetAccountID(PublicKey).ToString Then
                                                                Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                                                                PayMet = PayMethod

                                                                Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                                                                Autosendinfotext = T_Autosendinfotext

                                                                Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                                                                AutocompleteAT = T_AutocompleteAT

                                                            End If

                                                        End If

                                                    End If

                                                End If

                                            Next

                                            If Not RelKeyFounded Then
                                                If Not IsNothing(DEXNET) Then
                                                    DEXNET.AddRelevantKey("<ATID>" + PFPAT.AT.Trim + "</ATID>")
                                                End If
                                            End If
#End Region
                                        End If


                                        T_LVI.SubItems.Add(PayMet) 'payment method
                                        T_LVI.SubItems.Add(Autosendinfotext) 'autosend infotext
                                        T_LVI.SubItems.Add(AutocompleteAT) 'autocomplete at
                                        T_LVI.SubItems.Add(T_BuyerRS) 'buyer
                                        T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                        T_LVI.Tag = PFPAT

                                        BuyOrderLVIList.Add(T_LVI)

                                    End If

                                End If
#End Region

#Region "MyOPENOrders - GUI"

                                If TBSNOAddress.Text = Order.Seller Then

                                    'Broadcast info over DEXNET
                                    BroadcastMsgs.Add("<ATID>" + PFPAT.AT + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                    T_LVI = New ListViewItem
                                    T_LVI.Text = Confirms 'confirms
                                    T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                    T_LVI.SubItems.Add(Order.Type) 'type
                                    T_LVI.SubItems.Add(PayMet) 'method
                                    T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                    T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                    T_LVI.SubItems.Add("Me") 'seller
                                    T_LVI.SubItems.Add(Order.Buyer) 'buyer
                                    T_LVI.SubItems.Add(XItem) 'xitem
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                    T_LVI.SubItems.Add("OPEN") 'status
                                    T_LVI.Tag = Order

                                    MyOpenOrderLVIList.Add(T_LVI)

                                ElseIf TBSNOAddress.Text = Order.Buyer Then

                                    'Broadcast info over DEXNET
                                    BroadcastMsgs.Add("<ATID>" + PFPAT.AT + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                    T_LVI = New ListViewItem
                                    T_LVI.Text = Confirms 'confirms
                                    T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                    T_LVI.SubItems.Add(Order.Type) 'type
                                    T_LVI.SubItems.Add(PayMet) 'method
                                    T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                    T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                    T_LVI.SubItems.Add(Order.Seller) 'seller
                                    T_LVI.SubItems.Add("Me") 'buyer
                                    T_LVI.SubItems.Add(XItem) 'xitem
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                    T_LVI.SubItems.Add("OPEN") 'status
                                    T_LVI.Tag = Order

                                    MyOpenOrderLVIList.Add(T_LVI)

                                End If 'myaddress
#End Region

                            End If 'market(USD)

                        Catch ex As Exception
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(OPEN): -> " + ex.Message)
                        End Try

                    ElseIf Order.Status = "RESERVED" Then

                        ATChannelOpen = False

                        Try

#Region "MyRESERVEDOrders - GUI"

                            If TBSNOAddress.Text = Order.Seller Then

                                'Save/ Update() my RESERVED SellOrder to cache2.dat (MyOrders Settings)
                                Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(Order.FirstTransaction)
                                Dim T_OS As ClsOrderSettings = New ClsOrderSettings(Order.AT, Order.FirstTransaction, Order.Type, Order.Status)

                                Dim Autosendinfotext As String = "False"
                                Dim AutocompleteAT As String = "False"

                                If Order.Type = "SellOrder" Then
                                    If T_OSList.Count = 0 Then
                                        T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                        T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                    Else
                                        T_OS = T_OSList(0)
                                        T_OS.SetPayType()
                                        T_OS.Status = Order.Status
                                        PayMet = T_OS.PaytypeString
                                        Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                        AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                    End If

                                    OrderSettingsBuffer.Add(T_OS)
                                    'Broadcast info over DEXNET
                                    BroadcastMsgs.Add("<ATID>" + PFPAT.AT + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                Else

#Region "SellOrder-Info from DEXNET"

                                    Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                    If Not IsNothing(DEXNET) Then
                                        RelMsgs = DEXNET.GetRelevantMsgs()
                                    End If

                                    Dim RelKeyFounded As Boolean = False

                                    For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                        If RelMsg.RelevantKey = "<ATID>" + PFPAT.AT.Trim + "</ATID>" Then

                                            RelKeyFounded = True

                                            If Not RelMsg.RelevantMessage.Trim = "" Then

                                                Dim PublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))

                                                If Order.BuyerID.Trim = GetAccountID(PublicKey).ToString Then

                                                    Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                                                    PayMet = PayMethod

                                                    Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                                                    Autosendinfotext = T_Autosendinfotext

                                                    Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                                                    AutocompleteAT = T_AutocompleteAT

                                                End If


                                            End If

                                        End If

                                    Next

                                    If Not RelKeyFounded Then
                                        If Not IsNothing(DEXNET) Then
                                            DEXNET.AddRelevantKey("<ATID>" + PFPAT.AT.Trim + "</ATID>")
                                        End If
                                    End If

#End Region
                                End If



                                Dim AlreadySend As String = CheckBillingInfosAlreadySend(Order)

#Region "Automentation"
                                If AlreadySend.Contains(Application.ProductName + "-error") Then
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(AlreadySend)

                                ElseIf AlreadySend.Trim = "" Then

                                    Dim SearchAttachmentHEX As String = SigAPI.ULngList2DataStr(New List(Of ULong)({SigAPI.ReferenceFinishOrder}))
                                    If CheckForUTX(Order.Seller, Order.ATRS, SearchAttachmentHEX) Then
                                        AlreadySend = "PENDING"
                                    ElseIf CheckForUTX(Order.Buyer, Order.ATRS) Then

                                    Else

                                        AlreadySend = "RESERVED"

                                        'StatusLabel.Text = "Checking Transactions between " + Order.Seller + " and " + Order.ATRS
                                        'Application.DoEvents()

                                        'If CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, BCR.ReferenceFinishOrder.ToString) Then
                                        '    AlreadySend = "PENDING"
                                        'Else
                                        '    AlreadySend = "RESERVED, Wait for Payment-Info"
                                        'End If

                                    End If

                                    If T_OS.AutoSendInfotext Then 'autosend info to Buyer

#Region "AutoSend PaymentInfotext"

                                        If Not GetAutoinfoTXFromINI(Order.FirstTransaction) Then 'Check for autosend-info-TX in Settings.ini and skip if founded

                                            Dim PayInfo As String = GetPaymentInfoFromOrderSettings(Order.FirstTransaction, Order.Quantity, Order.XAmount, CurrentMarket)

                                            If PayInfo.Contains("PayPal-E-Mail=") Then
                                                Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                Dim ColWordsString As String = ColWords.GenerateColloquialWords(Order.FirstTransaction, True, "-", 5)

                                                PayInfo += " Reference/Note=" + ColWordsString
                                            End If

                                            Dim T_MsgStr As String = "AT=" + Order.ATRS + " TX=" + Order.FirstTransaction + " " + Dbl2LVStr(Order.XAmount, Decimals) + " " + Order.XItem + " " + PayInfo
                                            Dim TXr As String = SendBillingInfos(Order.BuyerID, T_MsgStr)

                                            If TXr.Contains(Application.ProductName + "-error") Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.ErrorLog2File(TXr)
                                            Else
                                                If SetAutoinfoTX2INI(Order.FirstTransaction) Then 'Set autosend-info-TX in Settings.ini
                                                    'ok
                                                End If
                                            End If

                                        End If

#End Region

                                    End If

                                Else 'BillingInfo Already send, check for aproving

                                    Dim PayInfoList As List(Of String) = New List(Of String)

                                    If AlreadySend.Contains(" ") Then
                                        PayInfoList.AddRange(AlreadySend.Split(" "))
                                    End If

                                    If PayInfoList.Count > 2 Then
                                        Dim NuPayInfo As String = ""
                                        For i As Integer = 2 To PayInfoList.Count - 1
                                            Dim T_PayInfo As String = PayInfoList(i)
                                            If Not T_PayInfo.Trim = "" Then
                                                NuPayInfo += T_PayInfo + " "
                                            End If
                                        Next
                                        AlreadySend = NuPayInfo.Trim
                                    End If


                                    If DelAutoinfoTXFromINI(Order.FirstTransaction) Then 'Delete autosend-info-TX from Settings.ini
                                        'ok
                                    End If

                                    If T_OS.AutoCompleteAT Then 'autosignal AT

#Region "AutoComplete AT with PayPalInfo"

                                        If T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_E_Mail Then

                                            Dim PayPalStatus As String = CheckPayPalTransaction(Order)

                                            If Not PayPalStatus.Trim = "" Then
                                                AlreadySend = PayPalStatus
                                            End If

                                        ElseIf T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_Order Then

                                            If AlreadySend.Contains("PayPal-Order=") Then
                                                Dim PayPalOrder As String = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim
                                                If PayPalOrder.Contains(" ") Then
                                                    PayPalOrder = PayPalOrder.Remove(PayPalOrder.IndexOf(" "))
                                                End If

                                                Dim PayPalStatus As String = CheckPayPalOrder(Order.AT, PayPalOrder)

                                                If Not PayPalStatus.Trim = "" Then
                                                    AlreadySend = PayPalStatus
                                                End If

                                            End If

                                        End If

#End Region

                                    End If

                                End If

#End Region

                                T_LVI = New ListViewItem

                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                T_LVI.SubItems.Add(Order.Type) 'type
                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                T_LVI.SubItems.Add("Me") 'seller
                                T_LVI.SubItems.Add(Order.Buyer) 'buyer
                                T_LVI.SubItems.Add(XItem) 'xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                T_LVI.SubItems.Add(AlreadySend) 'status
                                T_LVI.Tag = Order

                                MyOpenOrderLVIList.Add(T_LVI)

                            ElseIf TBSNOAddress.Text = Order.Buyer Then

                                'Save/ Update() my RESERVED SellOrder to cache2.dat (MyOrders Settings)
                                Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(Order.FirstTransaction)
                                Dim T_OS As ClsOrderSettings = New ClsOrderSettings(Order.AT, Order.FirstTransaction, Order.Type, Order.Status)

                                Dim Autosendinfotext As String = "False"
                                Dim AutocompleteAT As String = "False"

                                If Order.Type = "BuyOrder" Then
                                    If T_OSList.Count = 0 Then
                                        T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                        T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                    Else
                                        T_OS = T_OSList(0)
                                        T_OS.Status = Order.Status
                                        PayMet = T_OS.PaytypeString
                                        Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                        AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                    End If

                                    OrderSettingsBuffer.Add(T_OS)
                                    'Broadcast info over DEXNET
                                    BroadcastMsgs.Add("<ATID>" + PFPAT.AT + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                Else

#Region "BuyOrder-Info from DEXNET"

                                    Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                    If Not IsNothing(DEXNET) Then
                                        RelMsgs = DEXNET.GetRelevantMsgs()
                                    End If

                                    Dim RelKeyFounded As Boolean = False

                                    For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                        If RelMsg.RelevantKey = "<ATID>" + PFPAT.AT.Trim + "</ATID>" Then

                                            RelKeyFounded = True

                                            If Not RelMsg.RelevantMessage.Trim = "" Then

                                                Dim PublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))

                                                If Order.SellerID.Trim = GetAccountID(PublicKey).ToString Then
                                                    Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                                                    PayMet = PayMethod

                                                    Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                                                    Autosendinfotext = T_Autosendinfotext

                                                    Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                                                    AutocompleteAT = T_AutocompleteAT

                                                End If

                                            End If

                                        End If

                                    Next

                                    If Not RelKeyFounded Then
                                        If Not IsNothing(DEXNET) Then
                                            DEXNET.AddRelevantKey("<ATID>" + PFPAT.AT.Trim + "</ATID>")
                                        End If
                                    End If
#End Region

                                End If


                                Dim AlreadySend As String = CheckBillingInfosAlreadySend(Order)

                                If AlreadySend.Contains(Application.ProductName + "-error") Then

                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(AlreadySend)

                                ElseIf AlreadySend = "" Then

                                    Dim SearchAttachmentHEX As String = SigAPI.ULngList2DataStr(New List(Of ULong)({SigAPI.ReferenceFinishOrder}))
                                    If CheckForUTX(Order.Seller, Order.ATRS, SearchAttachmentHEX) Then
                                        AlreadySend = "PENDING"
                                    ElseIf CheckForUTX(Order.Buyer, Order.ATRS) Then

                                    Else

                                        AlreadySend = "RESERVED"

                                        'StatusLabel.Text = "Checking Transactions between " + Order.Seller + " and " + Order.ATRS
                                        'Application.DoEvents()

                                        'If CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, BCR.ReferenceFinishOrder.ToString) Then
                                        '    AlreadySend = "PENDING"
                                        'Else
                                        '    AlreadySend = "RESERVED, Wait for Payment-Info"
                                        'End If

                                    End If

                                ElseIf AlreadySend.Trim <> "" Then

                                    Dim PayPalOrder As String = ""
                                    If AlreadySend.Contains("PayPal-Order=") Then
                                        PayPalOrder = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim

                                        If PayPalOrder.Trim <> "" Then
                                            Dim PPAPI As ClsPayPal = New ClsPayPal()

                                            AlreadySend = PPAPI.URL + "/checkoutnow?token=" + PayPalOrder '"https://www.sandbox.paypal.com/checkoutnow?token=" 
                                            Process.Start(AlreadySend)
                                        End If

                                    Else

                                        Dim PayInfoList As List(Of String) = New List(Of String)

                                        If AlreadySend.Contains(" ") Then
                                            PayInfoList.AddRange(AlreadySend.Split(" "))
                                        End If

                                        If PayInfoList.Count > 2 Then
                                            Dim NuPayInfo As String = ""
                                            For i As Integer = 2 To PayInfoList.Count - 1
                                                Dim T_PayInfo As String = PayInfoList(i)
                                                If Not T_PayInfo.Trim = "" Then
                                                    NuPayInfo += T_PayInfo + " "
                                                End If
                                            Next
                                            AlreadySend = NuPayInfo.Trim
                                        End If

                                    End If

                                End If

                                Dim BCR1 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                                Dim CheckAttachment As String = BCR1.ULngList2DataStr(New List(Of ULong)({SigAPI.ReferenceFinishOrder}))
                                Dim UTXCheck As Boolean = CheckForUTX(Order.Seller, Order.ATRS, CheckAttachment)
                                Dim TXCheck As Boolean = CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, CheckAttachment)

                                If Not UTXCheck And Not TXCheck Then

                                Else
                                    AlreadySend = "PENDING"
                                End If

                                T_LVI = New ListViewItem

                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                T_LVI.SubItems.Add(Order.Type) 'type
                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                T_LVI.SubItems.Add(Order.Seller) 'seller
                                T_LVI.SubItems.Add("Me") 'buyer
                                T_LVI.SubItems.Add(XItem) 'xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                T_LVI.SubItems.Add(AlreadySend) 'status
                                T_LVI.Tag = Order

                                MyOpenOrderLVIList.Add(T_LVI)

                            End If 'myaddress

#End Region

                        Catch ex As Exception
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED): -> " + ex.Message)
                        End Try

                    Else 'CLOSED or CANCELED

#Region "MyClosedOrders - GUI"

                        Try

                            If DelAutosignalTXFromINI(Order.FirstTransaction) Then 'Delete autosignal-TX from Settings.ini
                                'ok
                            End If

                        Catch ex As Exception
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(Delete): -> " + ex.Message)
                        End Try


                        Dim TOSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(Order.FirstTransaction)

                        Try

                            If TOSList.Count > 0 Then
                                For i As Integer = 0 To OrderSettingsBuffer.Count - 1
                                    Dim T_TOS As ClsOrderSettings = OrderSettingsBuffer(i)

                                    If T_TOS.TX = Order.FirstTransaction Then
                                        OrderSettingsBuffer.RemoveAt(i)
                                        Exit For
                                    End If
                                Next

                            End If

                        Catch ex As Exception
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(DeleteOrdersettingbuffer): -> " + ex.Message)
                        End Try

                        If Order.XItem.Contains(Market) Then

                            If TBSNOAddress.Text = Order.Seller Then

                                Try

                                    T_LVI = New ListViewItem

                                    T_LVI.Text = Order.FirstTransaction 'first transaction
                                    If Not IsNothing(Order.LastTX.Transaction) Then
                                        T_LVI.SubItems.Add(Order.LastTX.Transaction) 'last transaction
                                    Else
                                        T_LVI.SubItems.Add("")
                                    End If

                                    T_LVI.SubItems.Add(Confirms) 'confirms
                                    T_LVI.SubItems.Add(Order.ATRS) 'at
                                    T_LVI.SubItems.Add(Order.Type) 'type
                                    T_LVI.SubItems.Add("Me") 'seller
                                    T_LVI.SubItems.Add(Order.Buyer) 'buyer
                                    T_LVI.SubItems.Add(XItem) 'xitem
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                    T_LVI.SubItems.Add(Order.Status) 'status
                                    'T_LVI.SubItems.Add(Order.Attachment.ToString) 'conditions
                                    T_LVI.Tag = Order

                                    MyClosedOrderLVIList.Add(T_LVI)

                                Catch ex As Exception
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(SetClosedSellOrder): -> " + ex.Message)
                                End Try

                            ElseIf TBSNOAddress.Text = Order.Buyer Then

                                Try

                                    T_LVI = New ListViewItem

                                    T_LVI.Text = Order.FirstTransaction 'first transaction

                                    If Not IsNothing(Order.LastTX.Transaction) Then
                                        T_LVI.SubItems.Add(Order.LastTX.Transaction) 'last transaction
                                    Else
                                        T_LVI.SubItems.Add("")
                                    End If

                                    T_LVI.SubItems.Add(Confirms) 'confirms
                                    T_LVI.SubItems.Add(Order.ATRS) 'at
                                    T_LVI.SubItems.Add(Order.Type) 'type
                                    T_LVI.SubItems.Add(Order.Seller) 'seller
                                    T_LVI.SubItems.Add("Me") 'buyer
                                    T_LVI.SubItems.Add(XItem) 'xitem
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'xamount
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'quantity
                                    T_LVI.SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                    T_LVI.SubItems.Add(Order.Status) 'status
                                    'T_LVI.SubItems.Add(Order.Attachment.ToString) 'conditions
                                    T_LVI.Tag = Order

                                    MyClosedOrderLVIList.Add(T_LVI)

                                Catch ex As Exception
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(SetClosedBuyOrder): -> " + ex.Message)
                                End Try

                            End If 'myaddress

                        End If 'market

#End Region

                    End If

#End Region

                Next


                If ATChannelOpen Then

                    If PFPAT.Frozen.ToString.ToLower = "true" And PFPAT.Finished.ToString.ToLower = "true" And PFPAT.Dead.ToString.ToLower <> "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = PFPAT.ATRS 'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.Tag = PFPAT

                        OpenChannelLVIList.Add(T_LVI)

                    ElseIf PFPAT.Dead.ToString.ToLower = "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = PFPAT.ATRS 'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.BackColor = Color.Crimson
                        T_LVI.ForeColor = Color.White
                        T_LVI.Tag = PFPAT

                        OpenChannelLVIList.Add(T_LVI)

                    End If

                End If


            End If 'BLS.AT_TXList.Count

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(): -> " + ex.Message)
        End Try

    End Sub

    Function Loading() As Boolean

        Try

            Dim BCR2 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)

            Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

            TBSNOAddress.Text = BetweenFromList(x, "<address>", "</address>")
            TBSNOBalance.Text = BetweenFromList(x, "<available>", "</available>")
            AccountID = BetweenFromList(x, "<account>", "</account>")

            LabXItem.Text = CurrentMarket
            LabXitemAmount.Text = CurrentMarket + " Amount: "

            MarketIsCrypto = GetMarketCurrencyIsCrypto(CurrentMarket)
            Decimals = GetCurrencyDecimals(CurrentMarket)

            NUDSNOItemAmount.DecimalPlaces = Decimals

            PFPList.Clear()
            CSVATList.Clear()

            Dim Wait As Boolean = GetAndCheckATs()
            Wait = MultithreadMonitor()


            Dim T_DEXATList As List(Of String()) = GetDEXATsFromCSV()

            For Each APIRequest As S_APIRequest In APIRequestList

                If Not IsNothing(APIRequest.Result) Then
                    If APIRequest.Result.GetType = GetType(String) Then

                        Dim AT_ATRS_BLSAT As List(Of String) = New List(Of String)(APIRequest.Result.ToString.Split(","))

                        Dim SAT As S_AT = New S_AT With {.AT = AT_ATRS_BLSAT(0), .ATRS = AT_ATRS_BLSAT(1), .IsBLS_AT = False}
                        CSVATList.Add(SAT)

                    Else

                        Dim BLSAT As S_PFPAT = DirectCast(APIRequest.Result, S_PFPAT)

                        PFPList.Add(BLSAT)
                        Dim SAT As S_AT = New S_AT With {.AT = BLSAT.AT, .ATRS = BLSAT.ATRS, .IsBLS_AT = True}
                        CSVATList.Add(SAT)

                    End If
                End If

            Next

            SaveATsToCSV(CSVATList)

            If T_DEXATList.Count = 0 Then
                T_DEXATList = GetDEXATsFromCSV()
            End If

            If T_DEXATList.Count <> 0 Then
                DEXATList.Clear()
            End If

            If DEXATList.Count = 0 Then
                For Each T_DEX In T_DEXATList
                    If T_DEX(2) = "True" Then
                        DEXATList.Add(T_DEX(0))
                    End If
                Next
            End If

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in Loading(): -> " + ex.Message)
        End Try

        Return True

    End Function
    Sub LoadHistory(ByVal input As Object)

        Try

            Dim CoBxChartVal As Integer = DirectCast(input(0), Integer)
            Dim CoBxTickVal As Integer = DirectCast(input(1), Integer)


            Dim Xitem As String = DirectCast(input(2), String)

            Dim T_OrderList As List(Of S_Order) = New List(Of S_Order)
            Dim T_OpenOrderList As List(Of S_Order) = New List(Of S_Order)

            For Each HisOrder As S_Order In OrderList

                Dim T_Order As S_Order = New S_Order
                T_Order = HisOrder

                If HisOrder.Status = "CLOSED" Then
                    T_OrderList.Add(T_Order)
                ElseIf HisOrder.Status = "OPEN" Then
                    T_OpenOrderList.Add(T_Order)
                End If

            Next

            If CBool(GetINISetting(E_Setting.TCPAPIEnable, "")) Then ' ChBxTCPAPI.Checked Then
                LoadTCPAPIOpenOrders(T_OpenOrderList)
                LoadTCPAPIHistorys(T_OrderList)
            End If



            Dim Chart As List(Of S_Candle) = GetCandles(Xitem, T_OrderList, CoBxChartVal, CoBxTickVal)

            Dim Minval As Double = Double.MaxValue
            Dim Maxval As Double = 0.0

            Dim TradeHistoryJSON As String = "{""response"":""GetCandles"",""PAIR"":""" + Xitem.Trim + "/SIGNA"",""DAYS"":""" + CoBxChartVal.ToString + """,""TICKMIN"":""" + CoBxTickVal.ToString + """,""DATA"":{"

            For Each HisCandle As S_Candle In Chart

                If HisCandle.CloseValue < Minval Then
                    Minval = HisCandle.CloseValue
                End If

                If HisCandle.CloseValue > Maxval Then
                    Maxval = HisCandle.CloseValue
                End If


                If HisCandle.LowValue < Minval Then
                    Minval = HisCandle.LowValue
                End If

                If HisCandle.HighValue > Maxval Then
                    Maxval = HisCandle.HighValue
                End If

            Next

            If Not Shutdown Then
                '0=MinVal; 1=MaxVal; 2=LastVal; 3=List(date, close)
                PanelForSplitPanel.Controls(0).Tag = New List(Of Object)({Xitem, Minval, Maxval, Chart(Chart.Count - 1).CloseValue, Chart})

                TradeTrackCalcs(PanelForSplitPanel.Controls(0))
            End If

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in LoadHistory(): -> " + ex.Message)
        End Try

    End Sub

    Function TradeTrackCalcs(ByVal input As Object) As Boolean

        Try

            Dim TTSlot As TradeTrackerSlot = DirectCast(input, TradeTrackerSlot)

            Dim Upper_Big_Graph As Graph = New Graph
            Dim Lower_Small_Graph As Graph = New Graph


            '0=XItem; 1=MinVal; 2=MaxVal; 3=LastVal; 4=List(date, close)
            Dim MetaList As List(Of Object) = DirectCast(TTSlot.Tag, List(Of Object))

            Dim XItem As String = MetaList(0)
            Dim MinVals As Double = CDbl(MetaList(1))
            Dim MaxVals As Double = CDbl(MetaList(2))
            Dim LastVal As Double = CDbl(MetaList(3))

            MultiInvoker(TTSlot.LabExch, "Text", "")
            MultiInvoker(TTSlot.LabPair, "Text", XItem)

            Dim Chart As List(Of S_Candle) = DirectCast(MetaList(4), List(Of S_Candle))

            Dim Extra As List(Of Graph.S_Extra) = New List(Of Graph.S_Extra)
            Dim XList As List(Of Object) = New List(Of Object)

            For Each Candle As S_Candle In Chart
                XList.Add(New List(Of Object)({Candle.CloseDat.ToString, Candle.CloseValue, Candle.HighValue, Candle.LowValue}))
            Next


            Upper_Big_Graph.GraphValuesList = New List(Of Graph.S_Graph)
            Lower_Small_Graph.GraphValuesList = New List(Of Graph.S_Graph)

            Dim CandleGraph As Graph.S_Graph = New Graph.S_Graph
            CandleGraph.PieceGraphList = DateIntToCandle(XList)

            If TTSlot.ChBxCandles.Checked = True Then

                CandleGraph.MinValue = MinVals
                CandleGraph.MaxValue = MaxVals
                CandleGraph.Extra = Extra
                CandleGraph.GraphArt = Graph.E_GraphArt.Candle

                Upper_Big_Graph.GraphValuesList.Add(CandleGraph)
            Else

            End If


            Dim LineGraph As Graph.S_Graph = New Graph.S_Graph
            If TTSlot.ChBxLine.Checked = True Then

                LineGraph.MinValue = MinVals
                LineGraph.MaxValue = MaxVals
                LineGraph.Extra = Extra
                LineGraph.GraphArt = Graph.E_GraphArt.Line
                LineGraph.PieceGraphList = CandleGraph.PieceGraphList

                Upper_Big_Graph.GraphValuesList.Add(LineGraph)
            Else

            End If


            Dim EMAFastList = EMAx(XList, (CInt(TTSlot.TBMACDEMA1.Text) * 1440) / 5) '1440 min * textbox (12) / 5min candle
            If TTSlot.ChBxEMA1.Checked = True Then

                Dim EMAFastGraph As Graph.S_Graph = New Graph.S_Graph
                EMAFastGraph.GraphArt = Graph.E_GraphArt.EMAFast
                EMAFastGraph.MinValue = MinVals
                EMAFastGraph.MaxValue = MaxVals

                EMAFastGraph.PieceGraphList = DateIntToCandle(EMAFastList)

                Upper_Big_Graph.GraphValuesList.Add(EMAFastGraph)
            Else

            End If


            Dim EMASlowList = EMAx(XList, (CInt(TTSlot.TBMACDEMA2.Text) * 1440) / 5) '1440 min * textbox (26) / 5min candle
            If TTSlot.ChBxEMA2.Checked = True Then

                Dim EMASlowGraph As Graph.S_Graph = New Graph.S_Graph
                EMASlowGraph.MinValue = MinVals
                EMASlowGraph.MaxValue = MaxVals
                EMASlowGraph.GraphArt = Graph.E_GraphArt.EMASlow
                EMASlowGraph.PieceGraphList = DateIntToCandle(EMASlowList)

                Upper_Big_Graph.GraphValuesList.Add(EMASlowGraph)
            Else

            End If

            Dim MACDL = MACDx(EMAFastList, EMASlowList)
            Dim MinMACD As Double = Double.MaxValue
            Dim MaxMACD As Double = 0.0

            For Each MAC In MACDL
                Dim MACVal As Double = MAC(1)

                If MACVal > MaxMACD Then
                    MaxMACD = MACVal
                End If

                If MACVal < MinMACD Then
                    MinMACD = MACVal
                End If

            Next

            If TTSlot.ChBxMACD.Checked = True Then

                Dim MACDGraph As Graph.S_Graph = New Graph.S_Graph
                MACDGraph.GraphArt = Graph.E_GraphArt.MACD
                MACDGraph.MinValue = MinMACD
                MACDGraph.MaxValue = MaxMACD
                MACDGraph.PieceGraphList = DateIntToCandle(MACDL)

                Lower_Small_Graph.GraphValuesList.Add(MACDGraph)

            Else

            End If



            Dim SignalList = EMAx(MACDL, (CInt(TTSlot.TBMACDSig.Text) * 1440) / 5)
            If TTSlot.ChBxMACDSig.Checked = True Then

                Dim SignalGraph As Graph.S_Graph = New Graph.S_Graph
                SignalGraph.MinValue = MinMACD
                SignalGraph.MaxValue = MaxMACD
                SignalGraph.GraphArt = Graph.E_GraphArt.Signal
                SignalGraph.PieceGraphList = DateIntToCandle(SignalList)

                Lower_Small_Graph.GraphValuesList.Add(SignalGraph)

            Else

            End If



            Dim MSigL = MACDx(MACDL, SignalList)

            Dim MinMSig As Double = Double.MaxValue
            Dim MaxMSig As Double = 0.0

            Dim PosHG As Integer = 0
            Dim NegHG As Integer = 0

            For Each MSigi In MSigL
                Dim ZMACVal As Double = MSigi(1)

                If ZMACVal > MaxMSig Then
                    MaxMSig = ZMACVal
                End If

                If ZMACVal < MinMSig Then
                    MinMSig = ZMACVal
                End If

                If ZMACVal < 0 Then
                    NegHG += 1
                ElseIf ZMACVal > 0 Then
                    PosHG += 1
                End If

            Next

            Dim HGTrend As String = ""

            If MaxMSig > MinMSig * -1 Then
                HGTrend = "↑"
            ElseIf MaxMSig < MinMSig * -1 Then
                HGTrend = "↓"
            Else
                HGTrend = "→"
            End If



            If TTSlot.ChBxMACDHis.Checked = True Then

                Dim MACDHistogramm As Graph.S_Graph = New Graph.S_Graph
                MACDHistogramm.GraphArt = Graph.E_GraphArt.Sticks
                MACDHistogramm.MinValue = MinMSig
                MACDHistogramm.MaxValue = MaxMSig
                MACDHistogramm.PieceGraphList = DateIntToCandle(MSigL)

                Lower_Small_Graph.GraphValuesList.Insert(0, MACDHistogramm)

            End If


            If TTSlot.ChBxMACDHis.Checked = True Or TTSlot.ChBxMACDSig.Checked = True Or TTSlot.ChBxMACD.Checked = True Then
                MultiInvoker(TTSlot.GroupBox2, "Visible", True)
            Else
                MultiInvoker(TTSlot.GroupBox2, "Visible", False)
            End If


            Dim RSIL = RSIx(CandleGraph.PieceGraphList, CInt(TTSlot.TBRSICandles.Text) * 10)

            Dim MinRSI As Double = Double.MaxValue
            Dim MaxRSI As Double = 0.0

            For Each RSIi In RSIL
                Dim RSIVal As Double = RSIi(1)

                If RSIVal > MaxRSI Then
                    MaxRSI = RSIVal
                End If

                If RSIVal < MinRSI Then
                    MinRSI = RSIVal
                End If

            Next

            If TTSlot.ChBxRSI.Checked = True Then

                MultiInvoker(TTSlot.GroupBox3, "Visible", True)

                Dim RSIGraph As Graph.S_Graph = New Graph.S_Graph
                RSIGraph.GraphArt = Graph.E_GraphArt.RSI
                RSIGraph.MinValue = MinRSI
                RSIGraph.MaxValue = MaxRSI
                RSIGraph.PieceGraphList = DateIntToCandle(RSIL)

                Lower_Small_Graph.GraphValuesList.Add(RSIGraph)

            Else

                MultiInvoker(TTSlot.GroupBox3, "Visible", False)

            End If


            MultiInvoker(TTSlot.LabMAX, "Text", String.Format("{0:#0.00000000}", MaxVals))
            MultiInvoker(TTSlot.LabLast, "Text", String.Format("{0:#0.00000000}", LastVal))

            If HGTrend = "↑" Then
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Green)

            ElseIf HGTrend = "↓" Then
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Red)
            Else
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Black)
            End If

            MultiInvoker(TTSlot.LabMIN, "Text", String.Format("{0:#0.00000000}", MinVals))

            TTSlot.Chart_EMA_Graph = Upper_Big_Graph
            TTSlot.MACD_RSI_TR_GraphList = Lower_Small_Graph


        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in TradeTrackCalcs(): -> " + ex.Message)
        End Try

        Return True

    End Function


#Region "TCP API"

    Property TCPAPI As ClsTCPAPI = New ClsTCPAPI(GetINISetting(E_Setting.TCPAPIServerPort, 8130), GetINISetting(E_Setting.TCPAPIShowStatus, False))

    Sub LoadTCPAPIOpenOrders(ByVal OrderList As List(Of S_Order))

        Try

            Dim OpenOrdersJSON As String = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""GetOpenOrders"",""data"":["

            For Each T_Order As S_Order In OrderList

                Dim AT As String = T_Order.ATRS
                Dim Type As String = T_Order.Type
                Dim Seller As String = T_Order.Seller
                Dim Buyer As String = T_Order.Buyer

                Dim Collateral As String = T_Order.Collateral.ToString.Replace(",", ".")
                Dim SignaAmount As String = T_Order.Quantity.ToString.Replace(",", ".")
                Dim Price As String = T_Order.Price.ToString.Replace(",", ".")
                Dim XItem As String = T_Order.XItem + "_SIGNA"
                Dim XAmount As String = T_Order.XAmount.ToString.Replace(",", ".")

                OpenOrdersJSON += "{""at"":""" + AT + ""","
                OpenOrdersJSON += """type"":""" + Type + ""","
                OpenOrdersJSON += """seller"":""" + Seller + ""","
                OpenOrdersJSON += """buyer"":""" + Buyer + ""","
                OpenOrdersJSON += """signaCollateral"":""" + Collateral + ""","
                OpenOrdersJSON += """signa"":""" + SignaAmount + ""","
                OpenOrdersJSON += """xItem"":""" + XItem + ""","
                OpenOrdersJSON += """xAmount"":""" + XAmount + ""","
                OpenOrdersJSON += """price"":""" + Price + """},"

            Next

            OpenOrdersJSON = OpenOrdersJSON.Remove(OpenOrdersJSON.Length - 1)
            OpenOrdersJSON += "]}"

            Dim Parameters As List(Of String) = New List(Of String) '({"type=" + Type, "pair=" + XItem})
            RefreshTCPAPIResponse("GetOpenOrders", Parameters, OpenOrdersJSON)

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIOpenOrders(): -> " + ex.Message)
        End Try

    End Sub
    Sub LoadTCPAPIHistorys(ByVal OrderList As List(Of S_Order))

        Try

            '"AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"
            'CoBxChart.Items.AddRange({1, 3, 7, 15, 30})
            'CoBxTick.Items.AddRange({1, 5, 15, 30, 60, 360, 720})

            Dim Currency_Day_TickList As List(Of List(Of String)) = GetTCPAPICurrencyDaysTicks()

            Try

                For Each CDT As List(Of String) In Currency_Day_TickList

                    Dim Curr As String = CDT(0)
                    Dim Day As String = CDT(1)
                    Dim Tick As String = CDT(2)

                    Dim ViewThread As Threading.Thread = New Threading.Thread(AddressOf LoadTCPAPIHistory)
                    ViewThread.Start(New List(Of Object)({Curr, OrderList, Day.ToString, Tick.ToString}))
                    Threading.Thread.Sleep(10)
                Next

            Catch ex As Exception
                TCPAPI.StopAPIServer()
                MultiInvoker(StatusLabel, "Text", "LoadTCPAPIHistorys(): " + ex.Message + " TCPAPIServer stopped")
            End Try


            'LabMSGs.Text = "MSGs: " + TCPAPI.StatusMSG.Count.ToString

            Dim T_FrmDevelope As FrmDevelope = GetSubForm("FrmDevelope")
            If Not IsNothing(T_FrmDevelope) Then

                If TCPAPI.StatusMSG.Count > 0 Then

                    For i As Integer = 0 To TCPAPI.StatusMSG.Count - 1
                        Dim MSG As String = TCPAPI.StatusMSG(i)

                        MultiInvoker(T_FrmDevelope.LiBoTCPAPIStatus, "Items", {"Insert", 0, MSG})
                    Next

                    TCPAPI.StatusMSG.Clear()

                End If
            End If

            'LabClients.Text = "Clients: " + TCPAPI.ConnectionList.Count.ToString

        Catch ex As Exception

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIHistorys(): -> " + ex.Message)
        End Try

    End Sub
    Function GetTCPAPICurrencyDaysTicks(Optional ByVal OCurrencys As List(Of String) = Nothing) As List(Of List(Of String))

        Try

            Dim Currency_Day_Tick As List(Of List(Of String)) = New List(Of List(Of String))

            Dim Currencys As List(Of String) = New List(Of String)

            If IsNothing(OCurrencys) Then
                Currencys.Add(CurrentMarket)
            Else
                Currencys.AddRange(OCurrencys.ToArray)
            End If


            Dim Days As List(Of Integer) = New List(Of Integer)({1})
            Dim TickMins As List(Of Integer) = New List(Of Integer)({5, 15, 30, 60, 360, 720})

            For Each Cur As String In Currencys

                Dim FullCur As String = Cur + "_SIGNA"

                For Each Day As Integer In Days

                    For Each Tick As Integer In TickMins
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day, Tick})
                        Currency_Day_Tick.Add(T_t)
                    Next

                Next

            Next

            Days = New List(Of Integer)({3})
            TickMins = New List(Of Integer)({15, 30, 60, 360, 720})


            For Each Cur As String In Currencys

                Dim FullCur As String = Cur + "_SIGNA"

                For Each Day As Integer In Days

                    For Each Tick As Integer In TickMins
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day, Tick})
                        Currency_Day_Tick.Add(T_t)
                    Next

                Next

            Next


            Days = New List(Of Integer)({7})
            TickMins = New List(Of Integer)({30, 60, 360, 720})


            For Each Cur As String In Currencys

                Dim FullCur As String = Cur + "_SIGNA"

                For Each Day As Integer In Days

                    For Each Tick As Integer In TickMins
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day, Tick})
                        Currency_Day_Tick.Add(T_t)
                    Next

                Next

            Next


            Days = New List(Of Integer)({15, 30})
            TickMins = New List(Of Integer)({60, 360, 720})

            For Each Cur As String In Currencys

                Dim FullCur As String = Cur + "_SIGNA"

                For Each Day As Integer In Days

                    For Each Tick As Integer In TickMins
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day, Tick})
                        Currency_Day_Tick.Add(T_t)
                    Next

                Next

            Next

            Return Currency_Day_Tick

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in GetTCPAPICurrencyDaysTicks(): -> " + ex.Message)

            Return New List(Of List(Of String))
        End Try

    End Function
    Sub LoadTCPAPIHistory(ByVal input As Object)

        Try

            Dim XItem As String = DirectCast(input(0), String)
            Dim OrderList As List(Of S_Order) = DirectCast(input(1), List(Of S_Order))
            Dim Days As String = DirectCast(input(2), String)
            Dim Tick As String = DirectCast(input(3), String)


            Dim Chart As List(Of S_Candle) = GetCandles(XItem.Remove(XItem.IndexOf("_")), OrderList, CInt(Days), CInt(Tick))

            Dim TradeHistoryJSON As String = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""GetCandles"",""pair"":""" + XItem.Trim + """,""days"":""" + Days.Trim + """,""tickmin"":""" + Tick.Trim + """,""data"":["

            For Each HisCandle As S_Candle In Chart

                TradeHistoryJSON += "{""openDate"":""" + HisCandle.OpenDat.ToShortDateString + " " + HisCandle.OpenDat.ToShortTimeString + ""","
                TradeHistoryJSON += """open"":""" + HisCandle.OpenValue.ToString.Replace(",", ".") + ""","
                TradeHistoryJSON += """low"":""" + HisCandle.LowValue.ToString.Replace(",", ".") + ""","
                TradeHistoryJSON += """high"":""" + HisCandle.HighValue.ToString.Replace(",", ".") + ""","
                TradeHistoryJSON += """close"":""" + HisCandle.CloseValue.ToString.Replace(",", ".") + ""","
                TradeHistoryJSON += """closeDate"":""" + HisCandle.CloseDat.ToShortDateString + " " + HisCandle.CloseDat.ToShortTimeString + ""","
                TradeHistoryJSON += """volume"":""" + HisCandle.Volume.ToString.Replace(",", ".") + """},"

            Next

            TradeHistoryJSON = TradeHistoryJSON.Remove(TradeHistoryJSON.Length - 1)
            TradeHistoryJSON += "]}"




            Dim Parameters As List(Of String) = New List(Of String)({"pair=" + XItem, "days=" + Days, "tickmin=" + Tick})
            RefreshTCPAPIResponse("GetCandles", Parameters, TradeHistoryJSON)

            'Dim FoundCommand As Boolean = False
            'For i As Integer = 0 To TCPAPI.ResponseMSGList.Count - 1
            '    Dim T_Response = TCPAPI.ResponseMSGList(i)

            '    If T_Response.API_Command = "GetCandles" Then

            '        If T_Response.API_Parameters.Count >= 3 Then

            '            If T_Response.API_Parameters(0) = "pair=" + XItem And T_Response.API_Parameters(1) = "days=" + Days And T_Response.API_Parameters(2) = "tickmin=" + Tick Then
            '                T_Response.API_Response = TradeHistoryJSON
            '                FoundCommand = True
            '                TCPAPI.ResponseMSGList(i) = T_Response
            '                Exit For
            '            End If

            '        End If

            '    End If

            'Next


            'If Not FoundCommand Then

            '    Dim Response As TCPAPI.API_Response = New TCPAPI.API_Response
            '    Response.API_Interface = "API"
            '    Response.API_Version = "v1.0"
            '    Response.API_Command = "GetCandles"
            '    Response.API_Response = TradeHistoryJSON
            '    Response.API_Parameters = New List(Of String)({"pair=" + XItem, "days=" + Days, "tickmin=" + Tick})

            '    TCPAPI.ResponseMSGList.Add(Response)

            'End If

        Catch ex As Exception
            MultiInvoker(StatusLabel, "Text", "LoadTCPAPIHistory(): " + ex.Message)
        End Try

    End Sub
    Sub RefreshTCPAPIResponse(ByVal Command As String, ByVal Parameters As List(Of String), ByVal ResponseMSG As String)

        Try

            Dim FoundCommand As Boolean = False
            For i As Integer = 0 To TCPAPI.ResponseMSGList.Count - 1
                Dim T_Response = TCPAPI.ResponseMSGList(i)

                If T_Response.API_Command = Command Then

                    If T_Response.API_Parameters.Count = Parameters.Count Then

                        If CompareLists(T_Response.API_Parameters, Parameters) Then
                            T_Response.API_Response = ResponseMSG
                            FoundCommand = True
                            TCPAPI.ResponseMSGList(i) = T_Response

                            Exit For

                        End If

                    End If

                End If

            Next


            If Not FoundCommand Then

                Dim Response As ClsTCPAPI.API_Response = New ClsTCPAPI.API_Response
                Response.API_Interface = "API"
                Response.API_Version = "v1.0"
                Response.API_Command = Command
                Response.API_Response = ResponseMSG
                Response.API_Parameters = New List(Of String)(Parameters.ToArray)

                TCPAPI.ResponseMSGList.Add(Response)

            End If

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in RefreshTCPAPIResponse(): -> " + ex.Message)
        End Try


    End Sub
    Function CompareLists(ByVal List1 As List(Of String), List2 As List(Of String)) As Boolean

        Try

            Dim Returner As Boolean = True

            If List1.Count <> List2.Count Then
                Return False
            End If

            If List1.Count = 0 Then
                Return True
            End If

            Dim Result As List(Of Boolean) = New List(Of Boolean)

            For i As Integer = 0 To List1.Count - 1

                Dim List1Entry As String = List1(i)

                Dim Founded As Boolean = False
                For j As Integer = 0 To List2.Count - 1

                    Dim List2Entry As String = List2(j)

                    If List1Entry = List2Entry Then
                        Result.Add(True)
                        Founded = True
                        Exit For
                    End If

                Next

                If Not Founded Then
                    Result.Add(False)
                End If

            Next

            For Each Res As Boolean In Result
                If Res = False Then
                    Return False
                End If
            Next


        Catch ex As Exception

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in CompareLists(): -> " + ex.Message)
        End Try

        Return True

    End Function

#End Region


#Region "DEXNET"

    Property DEXNET As ClsDEXNET

    Public Function InitiateDEXNET() As Boolean

        Try

            If GetINISetting(E_Setting.DEXNETEnable, True) Then

                If IsNothing(DEXNET) Then
                    DEXNET = New ClsDEXNET(GetINISetting(E_Setting.DEXNETServerPort, 8131), GetINISetting(E_Setting.DEXNETShowStatus, False))
                    DEXNET.DEXNET_AgreeKeyHEX = T_AgreementKeyHEX
                Else
                    If DEXNET.DEXNETClose = True Then
                        DEXNET = New ClsDEXNET(GetINISetting(E_Setting.DEXNETServerPort, 8131), GetINISetting(E_Setting.DEXNETShowStatus, False))
                        DEXNET.DEXNET_AgreeKeyHEX = T_AgreementKeyHEX
                    End If
                End If


                Dim DEXNETNodesString As String = GetINISetting(E_Setting.DEXNETNodes, "burstcoin.online:8131")
                Dim DEXNETMyHost As String = GetINISetting(E_Setting.DEXNETMyHost, "")


                Dim DEXNETNodes As List(Of String) = New List(Of String)
                If DEXNETNodesString.Contains(";") Then
                    DEXNETNodes.AddRange(DEXNETNodesString.Split(";"))
                Else
                    DEXNETNodes.Add(DEXNETNodesString)
                End If

                For Each DNNode As String In DEXNETNodes

                    If Not DEXNETMyHost = "" Then
                        If Not DEXNET.CheckHostIP_OK(DNNode, DEXNETMyHost) Then
                            Continue For
                        End If
                    End If

                    Dim HostIP As String = ""
                    Dim RemotePort As Integer = 0
                    If DNNode.Contains(":") Then
                        HostIP = DNNode.Remove(DNNode.IndexOf(":"))
                        RemotePort = CInt(DNNode.Substring(DNNode.IndexOf(":") + 1))
                    End If

                    DEXNET.Connect(HostIP, RemotePort, DEXNETMyHost)

                Next

            End If


        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in InitiateDEXNET(): -> " + ex.Message)
        End Try

        Return True

    End Function

#End Region


#Region "AT Interactions"
    Function GetLastTXWithValues(ByVal BLSTX As List(Of S_PFPAT_TX), Optional ByVal Xitem As String = "", Optional ByVal AttSearch As String = "<xItem>") As S_PFPAT_TX

        Dim BLSAT_TX As S_PFPAT_TX = New S_PFPAT_TX

        Dim NuBLSTX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)
        NuBLSTX.AddRange(BLSTX.ToArray)
        NuBLSTX.Reverse()

        Dim Found As Boolean = False
        Dim ReferenceSenderID As String = ""
        Dim ReferenceSenderRS As String = ""


        For i As Integer = 0 To NuBLSTX.Count - 1
            Dim TTX As S_PFPAT_TX = NuBLSTX(i)
            BLSAT_TX = TTX


            If Not IsNothing(BLSAT_TX.Attachment) Then
                If Not BLSAT_TX.Attachment.Contains(Xitem) And Not Xitem.Trim = "" Then
                    Found = False
                End If

                If Not ReferenceSenderRS = BLSAT_TX.SenderRS Then

                    If Not BLSAT_TX.Attachment.Contains("<injectedResponser>" + ReferenceSenderID + "</injectedResponser>") Then
                        Found = False
                    Else

                        Found = True

                        BLSAT_TX.Sender = ReferenceSenderID
                        BLSAT_TX.SenderRS = ReferenceSenderRS

                    End If

                Else
                    Found = True
                End If

            End If

            If Found Then
                Exit For
            End If

            If TTX.Type = "BLSTX" Then

                If TTX.Attachment.Contains(AttSearch) Then
                    Found = True
                    ReferenceSenderRS = TTX.RecipientRS
                    ReferenceSenderID = TTX.Recipient

                End If

            End If

        Next

        If Found Then
            Return BLSAT_TX
        Else
            Return New S_PFPAT_TX("")
        End If


    End Function

    Function ConvertTXs2Orders(ByVal BLSAT As S_PFPAT) As List(Of S_Order)

        Dim NuOrderList As List(Of S_Order) = New List(Of S_Order)
        Dim TXSplitList As List(Of List(Of S_PFPAT_TX)) = GetDeals(BLSAT.AT_TXList, "")

        For Each TXOrder As List(Of S_PFPAT_TX) In TXSplitList

            Dim Order As S_Order = ConvertTXs2Order(BLSAT, TXOrder)

            If Not IsNothing(Order) Then
                NuOrderList.Add(Order)
            End If

        Next

        If NuOrderList.Count > 1 Then
            For i As Integer = 1 To NuOrderList.Count - 1
                Dim NuOrder As S_Order = NuOrderList(i)

                If NuOrder.Status <> "CLOSED" And NuOrder.Status <> "CANCELED" Then
                    NuOrder.Status = "CANCELED"
                    NuOrderList(i) = NuOrder
                End If

            Next
        End If

        Return NuOrderList

    End Function
    Function GetDeals(ByVal BLSTX As List(Of S_PFPAT_TX), ByVal MyAddress As String, Optional ByVal Xitem As String = "") As List(Of List(Of S_PFPAT_TX))

        Dim RetList As List(Of List(Of S_PFPAT_TX)) = New List(Of List(Of S_PFPAT_TX))

        Dim BLSAT_TX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

        Dim NuBLSTX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)
        NuBLSTX.AddRange(BLSTX.ToArray)
        NuBLSTX.Reverse()

        Dim FoundDeal As Boolean = False
        Dim FoundMyDeal As Boolean = False

        Dim Initiator As String = ""
        Dim CheckAttachment As String = ""

        If MyAddress.Trim = "" Then
            FoundMyDeal = True
        End If

        For i As Integer = 0 To NuBLSTX.Count - 1
            Dim TTX As S_PFPAT_TX = NuBLSTX(i)
            BLSAT_TX.Add(TTX)

            If TTX.SenderRS = MyAddress Or TTX.RecipientRS = MyAddress Then
                FoundMyDeal = True
            End If

            If FoundDeal And TTX.SenderRS = Initiator And TTX.Attachment = CheckAttachment Then
                If (TTX.Attachment.Contains(Xitem) Or Xitem.Trim = "") And FoundMyDeal Then
                    BLSAT_TX.Reverse()
                    RetList.Add(BLSAT_TX)
                    FoundDeal = False

                    If Not MyAddress.Trim = "" Then
                        FoundMyDeal = False
                    End If

                    BLSAT_TX = New List(Of S_PFPAT_TX)
                    Continue For
                Else
                    FoundDeal = False
                    BLSAT_TX = New List(Of S_PFPAT_TX)
                End If
            End If

            If TTX.Type = "BLSTX" Then

                If TTX.Attachment.Contains("<xItem>") Then
                    FoundDeal = True
                    Initiator = TTX.RecipientRS
                    CheckAttachment = TTX.Attachment
                End If

            End If

        Next

        Return RetList

    End Function
    Function ConvertTXs2Order(ByVal BLSAT As S_PFPAT, ByVal TXList As List(Of S_PFPAT_TX)) As S_Order

        'TODO: Fix OPEN and RESERVED TXs

        Dim NuOrder As S_Order = New S_Order
        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        Dim FirstTX As S_PFPAT_TX = New S_PFPAT_TX

        If TXList.Count > 0 Then
            FirstTX = GetLastTXWithValues(TXList, "",)
            If FirstTX.Attachment.Trim = "" Then
                Return Nothing
            End If
        End If

        Dim FirstAmount As Double = BCR.Planck2Dbl(CULng(FirstTX.AmountNQT))

        Dim Xitem As String = Between(FirstTX.Attachment, "<xItem>", "</xItem>", GetType(String))
        Dim Xamount As Double = BCR.Planck2Dbl(CULng(Between(FirstTX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
        Dim Collateral As Double = BCR.Planck2Dbl(CULng(Between(FirstTX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))

        Dim SellOrder As Boolean = False
        If FirstAmount > Collateral Then
            SellOrder = True
        End If


        Dim ResponserRS As String = GetLastTXWithValues(TXList, "", "accepted").SenderRS
        Dim ResponserID As String = GetLastTXWithValues(TXList, "", "accepted").Sender
        Dim Status As String = GetLastTXWithValues(TXList, "", "finished").Transaction


        If Status.Trim = "" Then
            If ResponserRS.Trim = "" Then
                Status = "OPEN"
            Else
                Status = "RESERVED"
            End If

        Else

            Dim LastSender As String = GetLastTXWithValues(TXList, "", "finished").SenderRS

            If FirstTX.Type = "SellOrder" Then
                If FirstTX.SenderRS = LastSender And Not ResponserRS = "" Then
                    'Order CLOSED
                    Status = "CLOSED"
                Else
                    'Order CANCELED
                    Status = "CANCELED"
                End If
            Else
                If FirstTX.SenderRS = LastSender Then
                    'Order CANCELED
                    Status = "CANCELED"
                Else
                    'Order CLOSED
                    Status = "CLOSED"
                End If

            End If

        End If



        Dim LastTX As S_PFPAT_TX = New S_PFPAT_TX

        If Status = "CANCELED" And ResponserRS.Trim = "" Then

            For Each ConfirmedTX As S_PFPAT_TX In TXList

                If ConfirmedTX.Attachment.Contains("finished") Then
                    LastTX = ConfirmedTX
                    Exit For
                End If

            Next

        Else

            For Each ConfirmedTX As S_PFPAT_TX In TXList

                If ConfirmedTX.Type = "BLSTX" And BCR.Planck2Dbl(CULng(ConfirmedTX.AmountNQT)) > 0.0 Then

                    If Status = "CLOSED" Then

                        If SellOrder Then
                            'responser = buyer
                            If ConfirmedTX.RecipientRS = ResponserRS Then
                                LastTX = ConfirmedTX
                                Exit For
                            End If
                        Else
                            'initiator = buyer
                            If ConfirmedTX.RecipientRS = FirstTX.SenderRS Then
                                LastTX = ConfirmedTX
                                Exit For
                            End If
                        End If

                    ElseIf Status = "CANCELED" Then

                        If SellOrder Then
                            'responser = buyer
                            If ConfirmedTX.RecipientRS = ResponserRS Then
                                LastTX = ConfirmedTX
                                Exit For
                            End If
                        Else
                            'initiator = buyer
                            If ConfirmedTX.RecipientRS = FirstTX.SenderRS Then
                                LastTX = ConfirmedTX
                                Exit For
                            End If
                        End If

                    End If

                End If

            Next

        End If



        If FirstTX.Type = "SellOrder" Then

            Dim WTSAmount As Double = BCR.Planck2Dbl(CULng(FirstTX.AmountNQT)) - Collateral - 1

            NuOrder = New S_Order With {
                    .AT = BLSAT.AT,
                    .ATRS = BLSAT.ATRS,
                    .Type = FirstTX.Type,
                    .Seller = FirstTX.SenderRS,
                    .SellerID = FirstTX.Sender,
                    .Buyer = ResponserRS,
                    .BuyerID = ResponserID,
                    .XItem = Xitem,
                    .XAmount = Xamount,
                    .Quantity = WTSAmount,
                    .Price = Xamount / WTSAmount,
                    .Collateral = Collateral,
                    .Status = Status,
                    .Attachment = FirstTX.Attachment.ToString,
                    .FirstTransaction = FirstTX.Transaction,
                    .FirstTimestamp = FirstTX.Timestamp,
                    .FirstTX = FirstTX,
                    .LastTX = LastTX
                }


        ElseIf FirstTX.Type = "BuyOrder" Then

            NuOrder = New S_Order With {
                    .AT = BLSAT.AT,
                    .ATRS = BLSAT.ATRS,
                    .Type = FirstTX.Type,
                    .Seller = ResponserRS,
                    .SellerID = ResponserID,
                    .Buyer = FirstTX.SenderRS,
                    .BuyerID = FirstTX.Sender,
                    .XItem = Xitem,
                    .XAmount = Xamount,
                    .Quantity = Collateral,
                    .Price = Xamount / Collateral,
                    .Collateral = BCR.Planck2Dbl(CULng(FirstTX.AmountNQT)),
                    .Status = Status,
                    .Attachment = FirstTX.Attachment.ToString,
                    .FirstTransaction = FirstTX.Transaction,
                    .FirstTimestamp = FirstTX.Timestamp,
                    .FirstTX = FirstTX,
                    .LastTX = LastTX
                }

        End If

        Return NuOrder

    End Function

    Function GetAccountTXList(ByVal ATID As String, Optional ByVal Node As String = "", Optional FromTimestamp As String = "", Optional ByVal UseBuffer As Boolean = True) As List(Of S_PFPAT_TX)

        If Node = "" Then
            Node = PrimaryNode
        End If

        Dim BCR As ClsSignumAPI = New ClsSignumAPI(Node)
        BCR.DEXATList = DEXATList


        Dim ATTXs As List(Of List(Of String)) = BCR.GetAccountTransactions(ATID, FromTimestamp, UseBuffer)
        Dim BLSTXs As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

        For Each ATTX As List(Of String) In ATTXs

            Dim TX_Type As String = BetweenFromList(ATTX, "<type>", "</type>")
            Dim TX_Timestamp As String = BetweenFromList(ATTX, "<timestamp>", "</timestamp>")
            Dim TX_Recipient As String = BetweenFromList(ATTX, "<recipient>", "</recipient>")
            Dim TX_RecipientRS As String = BetweenFromList(ATTX, "<recipientRS>", "</recipientRS>")
            Dim TX_AmountNQT As String = BetweenFromList(ATTX, "<amountNQT>", "</amountNQT>")
            Dim TX_FeeNQT As String = BetweenFromList(ATTX, "<feeNQT>", "</feeNQT>")
            Dim TX_Transaction As String = BetweenFromList(ATTX, "<transaction>", "</transaction>")

            'StatusLabel.Text = "checking AT TX: " + TX_Transaction

            MultiInvoker(StatusLabel, "Text", "checking AT TX: " + TX_Transaction)


            Dim TX_Attachment As String = BetweenFromList(ATTX, "<attachment>", "</attachment>")
            If TX_Attachment.Trim = "" Then
                TX_Attachment = BetweenFromList(ATTX, "<message>", "</message>")
            End If


            Dim TX_Sender As String = BetweenFromList(ATTX, "<sender>", "</sender>")
            Dim TX_SenderRS As String = BetweenFromList(ATTX, "<senderRS>", "</senderRS>")
            Dim TX_Confirmations As String = BetweenFromList(ATTX, "<confirmations>", "</confirmations>")


            Dim BLSTX As S_PFPAT_TX = New S_PFPAT_TX With {
                    .Type = TX_Type,
                    .Timestamp = TX_Timestamp,
                    .Recipient = TX_Recipient,
                    .RecipientRS = TX_RecipientRS,
                    .AmountNQT = TX_AmountNQT,
                    .FeeNQT = TX_FeeNQT,
                    .Transaction = TX_Transaction,
                    .Attachment = TX_Attachment,
                    .Sender = TX_Sender,
                    .SenderRS = TX_SenderRS,
                    .Confirmations = TX_Confirmations
                }
            BLSTXs.Add(BLSTX)

        Next

        BLSTXs = BLSTXs.OrderBy(Function(BLSAT_TX As S_PFPAT_TX) BLSAT_TX.Timestamp).ToList

        Return BLSTXs

    End Function

    Function GetCurrentBLSATStatus(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As String

        Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

            If BLSATTX.Type = "BLSTX" Then

                If BLSATTX.Attachment.Contains("<colBuyAmount>") Then

                    Return "OPEN"

                ElseIf BLSATTX.Attachment.Contains("accepted") Then

                    Return "RESERVED"

                ElseIf BLSATTX.Attachment.Contains("finished") Then

                    Return "CLOSED"

                End If

            End If

        Next

        Return "CLOSED"

    End Function
    Function GetCurrentBLSATXItem(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As String

        Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

            If BLSATTX.Type = "BLSTX" Then

                If BLSATTX.Attachment.Contains("<xItem>") Then

                    Return Between(BLSATTX.Attachment, "<xItem>", "</xItem>", GetType(String))

                End If

            End If

        Next

        Return ""

    End Function
    Function GetCurrentBLSATXAmount(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As Double

        Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

            If BLSATTX.Type = "BLSTX" Then

                If BLSATTX.Attachment.Contains("<xAmount>") Then

                    Return CDbl(Between(BLSATTX.Attachment, "<xAmount>", "</xAmount>", GetType(String)))

                End If

            End If

        Next

        Return 0.0

    End Function

    Function SendBillingInfos(ByVal Recipient As String, ByVal Message As String, Optional ByVal Encrypt As Boolean = True)
        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
        Dim TX As String = BCR.SendMessage(Recipient, Message,, Encrypt)
        Return TX
    End Function

#End Region


#Region "Candles"
    Structure S_Candle
        Dim OpenDat As Date
        Dim CloseDat As Date

        Dim OpenValue As Double
        Dim CloseValue As Double

        Dim HighValue As Double
        Dim LowValue As Double

        Dim Volume As Double
    End Structure
    Structure S_Trade

        Dim OpenDat As Date
        Dim CloseDat As Date

        Dim Price As Double
        Dim Quantity As Double

    End Structure
    Function GetCandles(ByVal XItem As String, ByVal Orders As List(Of S_Order), ByVal ChartRangeDays As Integer, Optional ByVal ResolutionMin As Integer = 1) As List(Of S_Candle)

        Orders = Orders.OrderBy(Function(s) CDbl(s.LastTX.Timestamp)).ToList

        Dim GridNow As Date = GetGriddedTime(Now, ResolutionMin)

        Dim RangeStart As Date = GridNow.AddDays(-ChartRangeDays)

        Dim CandleList As List(Of S_Candle) = New List(Of S_Candle)

        Dim OldCandle As S_Candle = New S_Candle

        While Not RangeStart.ToLongDateString + " " + RangeStart.ToShortTimeString = GridNow.ToLongDateString + " " + GridNow.ToShortTimeString

            Dim CurrentDay As Date = RangeStart.AddMinutes(ResolutionMin)

            Dim T_TickTrades As List(Of S_Trade) = New List(Of S_Trade)

            Dim FoundSome As Boolean = False
            For Each Order As S_Order In Orders

                If Order.Status = "OPEN" Or Order.Status = "CANCELED" Or Not Order.XItem.Contains(XItem) Then
                    Continue For
                End If

                Dim TempDay As Date = ConvertLongMSToDate(Order.LastTX.Timestamp)

                If TempDay > RangeStart And TempDay < CurrentDay Then

                    FoundSome = True
                    Dim STrade As S_Trade = New S_Trade

                    STrade.OpenDat = CDate(RangeStart.ToShortDateString + " " + RangeStart.ToShortTimeString)
                    STrade.CloseDat = CDate(CurrentDay.ToShortDateString + " " + CurrentDay.ToShortTimeString)

                    STrade.Price = Order.Price
                    STrade.Quantity = Order.Quantity

                    T_TickTrades.Add(STrade)

                End If

            Next

            If Not FoundSome Then
                T_TickTrades.Add(New S_Trade With {.OpenDat = CDate(RangeStart.ToShortDateString + " " + RangeStart.ToShortTimeString), .CloseDat = CDate(CurrentDay.ToShortDateString + " " + CurrentDay.ToShortTimeString), .Price = -1.0, .Quantity = 0.0})
            End If


            Dim Candle As S_Candle = New S_Candle

            Candle.OpenDat = T_TickTrades(0).OpenDat
            Candle.CloseDat = T_TickTrades(T_TickTrades.Count - 1).CloseDat

            Candle.OpenValue = OldCandle.CloseValue
            Candle.CloseValue = T_TickTrades(T_TickTrades.Count - 1).Price

            If Candle.CloseValue = -1.0 Then Candle.CloseValue = Candle.OpenValue

            If CandleList.Count = 0 Then
                Candle.OpenValue = T_TickTrades(0).Price
            End If


            Dim High As Double = 0.0
            Dim Low As Double = Double.MaxValue

            For Each Trade As S_Trade In T_TickTrades
                Candle.Volume += Trade.Quantity

                If Trade.Price < Low Then
                    Low = Trade.Price
                End If

                If Trade.Price > High Then
                    High = Trade.Price
                End If

            Next

            If Low = -1.0 Then
                Low = 0.0
            End If

            Candle.HighValue = High
            Candle.LowValue = Low

            OldCandle = Candle

            CandleList.Add(Candle)

            RangeStart = CurrentDay

        End While


        Return CandleList

    End Function


    Function GetGriddedTime(ByVal Time As Date, ByVal TickMin As Integer) As Date

        Select Case TickMin
            Case 5

                Dim Minute As Integer = Time.Minute

                While Minute Mod 5 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")


            Case 10
                Dim Minute As Integer = Time.Minute

                While Minute Mod 10 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case 15

                Dim Minute As Integer = Time.Minute

                While Minute Mod 15 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case 30

                Dim Minute As Integer = Time.Minute

                While Minute Mod 30 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case 60

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":00:00")

            Case 180

                Dim Hour As Integer = Time.Hour

                While Hour Mod 3 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case 360

                Dim Hour As Integer = Time.Hour

                While Hour Mod 6 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case 720

                Dim Hour As Integer = Time.Hour

                While Hour Mod 12 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case 1440

                Return CDate(Time.ToShortDateString + " 00:00:00")

        End Select

        Return Time

    End Function





#End Region
#Region "Checking UTX/TX"
    Function CheckForUTX(Optional ByVal SenderRS As String = "", Optional ByVal RecipientRS As String = "", Optional ByVal SearchAttachmentHEX As String = "") As Boolean

        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()

        For Each UTX In T_UTXList

            Dim TX_SenderRS As String = BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim TX_RecipientRS As String = BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If (TX_SenderRS = SenderRS Or SenderRS = "") And (RecipientRS = TX_RecipientRS Or RecipientRS = "") And Not (SenderRS = "" And RecipientRS = "") Then

                Dim T_Msg As String = BetweenFromList(UTX, "<message>", "</message>")
                If T_Msg.ToLower.Trim = SearchAttachmentHEX.ToLower.Trim Or SearchAttachmentHEX = "" Then
                    Return True
                End If


                'Dim EncMessage As String = BCR.BetweenFromList(UTX, "<data>", "</data>")
                'If Not EncMessage.Trim = "" Then

                '    Dim Transaction As String = BCR.BetweenFromList(UTX, "<transaction>", "</transaction>")
                '    Dim DecryptedMsg As String = BCR.ReadMessage(Transaction)

                '    DecryptedMsg = DecryptedMsg

                'End If


            End If


            'For Each UTXKey As String In UTX

            '    Select Case True
            '        Case UTXKey.Contains("<timestamp>")

            '        Case UTXKey.Contains("<amountNQT>")

            '        Case UTXKey.Contains("<feeNQT>")

            '        Case UTXKey.Contains("<transaction>")

            '        Case UTXKey.Contains("<attachment>")

            '        Case UTXKey.Contains("<sender>")

            '        Case UTXKey.Contains("<senderRS>")
            '            'Dim SenderRS As String = BCR.Between(UTXKey, "<senderRS>", "</senderRS>")

            '            'If SenderRS = AddressRS Then
            '            '    Return True
            '            'End If

            '        Case UTXKey.Contains("<recipient>")

            '        Case UTXKey.Contains("<recipientRS>")
            '            'Dim RecipientRS As String = BCR.Between(UTXKey, "<recipientRS>", "</recipientRS>")

            '            'If RecipientRS = AddressRS Then
            '            '    Return True
            '            'End If

            '        Case UTXKey.Contains("<height>")

            '    End Select

            'Next

        Next

        Return False

    End Function

    Function CheckForTX(ByVal SenderRS As String, ByVal RecipientRS As String, ByVal FromTimestamp As String, ByVal SearchAttachment As String) As Boolean

        Dim BCR As ClsSignumAPI = New ClsSignumAPI
        BCR.DEXATList = DEXATList
        Dim AccountAddressList As List(Of String) = New List(Of String)

        'If Not SenderRS.Trim = "" Then
        AccountAddressList = BCR.RSConvert(SenderRS)
        'Else
        '    AccountAddressList = BCR.RSConvert(RecipientRS)
        'End If


        Dim T_Account As String = Between(AccountAddressList(0), "<account>", "</account>")

        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(T_Account, FromTimestamp, True)

        Dim AnswerTX As List(Of String) = New List(Of String)

        For Each T_TX In TXList

            Dim TX_SenderRS As String = BetweenFromList(T_TX, "<senderRS>", "</senderRS>")
            Dim TX_RecipientRS As String = BetweenFromList(T_TX, "<recipientRS>", "</recipientRS>")


            If TX_SenderRS = SenderRS And RecipientRS = TX_RecipientRS And Not (SenderRS = "" And RecipientRS = "") Then

                Dim T_Msg As String = BetweenFromList(T_TX, "<message>", "</message>")

                If T_Msg.Trim = "" Then
                    T_Msg = BetweenFromList(T_TX, "<method>", "</method>")
                End If

                'If SearchAttachment = "" Then
                '    Return True
                'End If

                If T_Msg.ToLower.Trim = SearchAttachment.ToLower.Trim Then
                    Return True
                End If

            End If

        Next

        Return False

    End Function
    Function CheckForTX(ByVal SenderRS As String, ByVal RecipientRS As String, ByVal FromTimestamp As String, ByVal SearchAttachment As ULong) As Boolean

        Dim BCR1 As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
        Dim CheckAttachment As String = BCR1.ULngList2DataStr(New List(Of ULong)({SearchAttachment}))

        Return CheckForTX(SenderRS, RecipientRS, FromTimestamp, CheckAttachment)

    End Function

    Function CheckATforTX(ByVal ATID As String) As Boolean

        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        Dim T_Timestamp As String = BCR.TimeToUnix(Now.AddDays(-1)).ToString

        Dim ATTXList As List(Of S_PFPAT_TX) = GetAccountTXList(ATID,, T_Timestamp)

        ATTXList.Reverse()

        If ATTXList.Count > 0 Then
            If ATTXList(0).Type = "ResponseOrder" Then
                Return True
            End If
        Else

        End If

        Return False

    End Function

    Function CheckBillingInfosAlreadySend(ByVal Order As S_Order) As String
        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
        BCR.DEXATList = DEXATList

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()
        T_UTXList.Reverse()

        For Each UTX As List(Of String) In T_UTXList

            Dim TX As String = BetweenFromList(UTX, "<transaction>", "</transaction>")
            Dim Sender As String = BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Seller And Recipient = Order.Buyer Then

                Dim Data As String = BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else

                    Dim VSAcc As String = Order.Buyer
                    If TBSNOAddress.Text.Trim = VSAcc Then
                        VSAcc = Order.Seller
                    End If

                    Dim DecryptedMsg As String = BCR.DecryptFrom(VSAcc, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                            Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                            Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))

                            If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                                Return DecryptedMsg
                            End If

                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(1): -> " + DecryptedMsg
                        End If

                    Else
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(1): -> " + DecryptedMsg
                    End If

                End If

            ElseIf Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim Data As String = BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else

                    Dim VSAcc As String = Order.Seller
                    If TBSNOAddress.Text.Trim = VSAcc Then
                        VSAcc = Order.Buyer
                    End If

                    Dim DecryptedMsg As String = BCR.DecryptFrom(VSAcc, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                            Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                            Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))

                            If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                                Return DecryptedMsg
                            End If

                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(2): -> " + DecryptedMsg
                        End If
                    Else
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(2): -> " + DecryptedMsg
                    End If

                End If

            End If

        Next


        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(Order.BuyerID, Order.FirstTimestamp, True)
        TXList.Reverse()

        For Each SearchTX As List(Of String) In TXList

            Dim TX As String = BetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim Sender As String = BetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Seller And Recipient = Order.Buyer Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then
                        Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                        Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))

                        If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                    End If
                Else
                    Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                End If

            ElseIf Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                        Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                        Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))

                        If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                    End If

                Else
                    Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                End If

            End If

        Next

        Return ""

    End Function

    Function CheckPaymentAlreadySend(ByVal Order As S_Order) As String

        Dim BCR As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
        BCR.DEXATList = DEXATList

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()
        T_UTXList.Reverse()

        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(Order.SellerID, Order.FirstTimestamp, True)
        TXList.Reverse()

        For Each UTX As List(Of String) In T_UTXList

            Dim TX As String = BetweenFromList(UTX, "<transaction>", "</transaction>")
            Dim Sender As String = BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim Data As String = BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else
                    Dim DecryptedMsg As String = BCR.DecryptFrom(Order.Seller, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                            Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                            Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))


                            If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                                Return DecryptedMsg
                            End If

                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in CheckPaymentAlreadySend(): -> " + DecryptedMsg
                        End If

                        Return DecryptedMsg
                    Else
                        Return Application.ProductName + "-error in CheckPaymentAlreadySend(): -> " + DecryptedMsg
                    End If

                End If

            End If

        Next



        For Each SearchTX As List(Of String) In TXList

            Dim TX As String = BetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim Sender As String = BetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                    Dim DCAT As String = Between(DecryptedMsg, "AT=", " TX=", GetType(String))
                    Dim DCTransaction As String = Between(DecryptedMsg, "TX=", " ", GetType(String))

                    If DCAT = Order.ATRS And DCTransaction = Order.FirstTransaction Then
                        Return DecryptedMsg
                    End If
                Else
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(DecryptedMsg)
                End If

            End If

        Next

        Return ""


    End Function


    Function SetAutoinfoTX2INI(ByVal TX As String) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")

        If AutoinfoTXStr.Trim = "" Then
            AutoinfoTXStr = TX + ";"
        ElseIf AutoinfoTXStr.Contains(";") Then
            AutoinfoTXStr += TX + ";"
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return True

    End Function
    Function GetAutoinfoTXFromINI(ByVal TX As String) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")
        Dim AutoinfoTXList As List(Of String) = New List(Of String)

        If AutoinfoTXStr.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(AutoinfoTXStr.Split(";"))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    AutoinfoTXList.Add(TempTX)
                End If
            Next
        Else
            If Not AutoinfoTXStr.Trim = "" Then
                AutoinfoTXList.Add(AutoinfoTXStr)
            End If
        End If


        For Each AutoinfoTX As String In AutoinfoTXList
            If TX = AutoinfoTX Then
                Return True
            End If
        Next

        Return False

    End Function
    Function DelAutoinfoTXFromINI(ByVal TX As String) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")
        Dim AutoinfoTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutoinfoTXStr.Contains(TX + ";") Then
            Returner = True
            AutoinfoTXStr = AutoinfoTXStr.Replace(TX + ";", "")
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return Returner

    End Function


    Function SetAutosignalTX2INI(ByVal TX As String) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")

        If AutosignalTXStr.Trim = "" Then
            AutosignalTXStr = TX + ";"
        ElseIf AutosignalTXStr.Contains(";") Then
            AutosignalTXStr += TX + ";"
        End If

        SetINISetting(E_Setting.AutoSignalTransactions, AutosignalTXStr.Trim)

        Return True

    End Function
    Function GetAutosignalTXFromINI(ByVal TX As String) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Dim AutosignalTXList As List(Of String) = New List(Of String)

        If AutosignalTXStr.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(AutosignalTXStr.Split(";"))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    AutosignalTXList.Add(TempTX)
                End If
            Next
        Else
            If Not AutosignalTXStr.Trim = "" Then
                AutosignalTXList.Add(AutosignalTXStr)
            End If
        End If


        For Each AutosignalTX As String In AutosignalTXList
            If TX = AutosignalTX Then
                Return True
            End If
        Next

        Return False

    End Function
    Function DelAutosignalTXFromINI(ByVal TX As String) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Dim AutosignalTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutosignalTXStr.Contains(TX + ";") Then
            Returner = True
            AutosignalTXStr = AutosignalTXStr.Replace(TX + ";", "")
        End If

        SetINISetting(E_Setting.AutoSignalTransactions, AutosignalTXStr.Trim)

        Return Returner

    End Function

#End Region
#Region "Tools"
    Function Dbl2LVStr(ByVal Dbl As Double, Optional ByVal Decs As Integer = 8) As String

        Dim DecStr As String = ""
        For i As Integer = 1 To Decs
            DecStr += "0"
        Next

        Return String.Format("{0:#0." + DecStr + "}", Dbl)
    End Function
    Function ConvertLongMSToDate(ByVal LongS As Long) As Date
        Dim StartDate As Date = New DateTime(2014, 8, 11, 4, 0, 0)
        StartDate = StartDate.AddSeconds(LongS)
        Return StartDate
    End Function

#End Region

#End Region

#Region "Multithreadings"

    Structure S_RelevantMsg
        Dim Setted As Boolean
        Dim RelevantMessage As String
    End Structure

    Dim RelevantMsgsBuffer As List(Of S_RelevantMsg) = New List(Of S_RelevantMsg)

    Sub SetDEXNETRelevantMsgsToLVs()


        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

        If Not IsNothing(DEXNET) Then
            RelMsgs = DEXNET.GetRelevantMsgs
        End If

#Region "Clean RelevantMsgBuffer"
        Dim WExit As Boolean = False
        While Not WExit
            WExit = True

            Dim Cnt As Integer = RelevantMsgsBuffer.Count
            For i As Integer = 0 To Cnt - 1

                If Cnt <> RelevantMsgsBuffer.Count Then
                    Exit For
                End If

                Dim x As S_RelevantMsg = RelevantMsgsBuffer(i)

                Dim Founded As Boolean = False

                For ii As Integer = 0 To RelMsgs.Count - 1
                    Dim y As ClsDEXNET.S_RelevantMessage = RelMsgs(ii)

                    If y.RelevantMessage = x.RelevantMessage Then
                        Founded = True
                        Exit For
                    End If

                Next

                If Not Founded Then
                    WExit = False
                    RelevantMsgsBuffer.RemoveAt(i)
                End If

            Next

        End While

#End Region

#Region "Fill RelevantMsgBuffer"
        For i As Integer = 0 To RelMsgs.Count - 1
            Dim RelMsg As ClsDEXNET.S_RelevantMessage = RelMsgs(i)

            If RelMsg.RelevantMessage = "" Then
                Continue For
            End If

            Dim Founded As Boolean = False
            For Each x As S_RelevantMsg In RelevantMsgsBuffer
                If x.RelevantMessage = RelMsg.RelevantMessage Then
                    Founded = True
                    Exit For
                End If
            Next

            If Not Founded Then
                Dim T_RelevantMsgBuf As S_RelevantMsg = New S_RelevantMsg
                T_RelevantMsgBuf.Setted = False
                T_RelevantMsgBuf.RelevantMessage = RelMsg.RelevantMessage
                RelevantMsgsBuffer.Add(T_RelevantMsgBuf)
            End If

        Next
#End Region


        For ii As Integer = 0 To RelevantMsgsBuffer.Count - 1
            Dim RelMsg As S_RelevantMsg = RelevantMsgsBuffer(ii)

            If RelMsg.Setted Then
                Continue For
            End If

            If RelMsg.RelevantMessage.Contains("<ATID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<PayType>") And RelMsg.RelevantMessage.Contains("<Autosendinfotext>") And RelMsg.RelevantMessage.Contains("<AutocompleteAT>") Then

#Region "Refresh LVs"

                Dim RM_ATID As String = Between(RelMsg.RelevantMessage, "<ATID>", "</ATID>", GetType(String))
                Dim SigAPI As ClsSignumAPI = New ClsSignumAPI()
                Dim ATIDRSList As List(Of String) = SigAPI.RSConvert(CULng(RM_ATID))
                Dim RM_ATRS As String = BetweenFromList(ATIDRSList, "<accountRS>", "</accountRS>")

                Dim RM_AccountPublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))
                Dim RM_AccountPublicKeyList As List(Of String) = SigAPI.RSConvert(GetAccountID(RM_AccountPublicKey).ToString)
                Dim RM_AccountRS As String = BetweenFromList(RM_AccountPublicKeyList, "<accountRS>", "</accountRS>")

                Dim ForContinue As Boolean = False
                For Each LVI As ListViewItem In LVSellorders.Items
                    Dim LVI_ATRS As String = GetLVColNameFromSubItem(LVSellorders, "AT", LVI)
                    Dim LVI_SellerRS As String = GetLVColNameFromSubItem(LVSellorders, "Seller", LVI)

                    If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_SellerRS Then

                        Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                        SetLVColName2SubItem(LVSellorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                        SetLVColName2SubItem(LVSellorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                        SetLVColName2SubItem(LVSellorders, LVI, "Autofinish", T_AutocompleteAT)

                        RelMsg.Setted = True
                        RelevantMsgsBuffer(ii) = RelMsg

                        ForContinue = True
                        Exit For
                    End If

                Next

                If ForContinue Then
                    Continue For
                End If

                For Each LVI As ListViewItem In LVBuyorders.Items
                    Dim LVI_ATRS As String = GetLVColNameFromSubItem(LVBuyorders, "AT", LVI)
                    Dim LVI_BuyerRS As String = GetLVColNameFromSubItem(LVBuyorders, "Buyer", LVI)

                    If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_BuyerRS Then

                        Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                        SetLVColName2SubItem(LVBuyorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                        SetLVColName2SubItem(LVBuyorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                        SetLVColName2SubItem(LVBuyorders, LVI, "Autofinish", T_AutocompleteAT)

                        RelMsg.Setted = True
                        RelevantMsgsBuffer(ii) = RelMsg

                        ForContinue = True
                        Exit For

                    End If

                Next

                If ForContinue Then
                    Continue For
                End If

                For Each LVI As ListViewItem In LVMyOpenOrders.Items

                    Dim LVI_ATRS As String = GetLVColNameFromSubItem(LVMyOpenOrders, "AT", LVI)
                    Dim LVI_Type As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Type", LVI)
                    Dim LVI_SellerRS As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVI)
                    Dim LVI_BuyerRS As String = GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVI)


                    If LVI_Type = "SellOrder" Then

                        If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_SellerRS Then

                            Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteAT)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    Else

                        If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_BuyerRS Then

                            Dim PayMethod As String = Between(RelMsg.RelevantMessage, "<PayType>", "</PayType>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = Between(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteAT As String = Between(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>", GetType(String))
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteAT)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    End If



                Next

#End Region

            ElseIf RelMsg.RelevantMessage.Contains("<ATID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<Ask>") Then
                Dim RM_ATID As String = Between(RelMsg.RelevantMessage, "<ATID>", "</ATID>", GetType(String))
                Dim SigAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, TBSNOPassPhrase.Text)
                Dim RM_AccountPublicKey As String = Between(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>", GetType(String))
                Dim RM_AccountPublicKeyList As List(Of String) = SigAPI.RSConvert(GetAccountID(RM_AccountPublicKey).ToString)
                Dim RM_AccountID As String = BetweenFromList(RM_AccountPublicKeyList, "<account>", "</account>")
                Dim RM_AccIDULong As ULong = 0UL

                Try
                    RM_AccIDULong = CULng(RM_AccountID)
                Catch ex As Exception
                    'TODO: Error Out SetDEXNETRelevantMsgsToLVs(RM_AccointID)
                End Try

                'TODO:########################################
                'TODO: Processing UnexpectetMsgs from DEXNET

                Dim RM_Ask As String = Between(RelMsg.RelevantMessage, "<Ask>", "</Ask>", GetType(String))

                If RM_Ask = "WTB" And Not RM_AccIDULong = 0 And Not RM_AccountPublicKey = "" Then

                    For Each LVI As ListViewItem In LVMyOpenOrders.Items
                        Dim MyOrder As S_Order = LVI.Tag

                        Dim My_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(MyOrder.FirstTransaction)

                        If My_OSList.Count <> 0 Then
                            Dim My_OS As ClsOrderSettings = My_OSList(0)

                            If My_OS.AutoSendInfotext Then

                                If Not GetAutosignalTXFromINI(MyOrder.FirstTransaction) Then

                                    If MyOrder.AT = RM_ATID Then

                                        If MyOrder.Collateral = 0.0 Then

                                            Dim ULngList As List(Of ULong) = New List(Of ULong)({SigAPI.ReferenceInjectResponder, RM_AccIDULong})
                                            Dim MsgStr As String = SigAPI.ULngList2DataStr(ULngList)

                                            Dim Response As String = SigAPI.SendMessage2BLSAT(RM_ATID, 1.0, ULngList)

                                            If Response.Contains(Application.ProductName + "-error") Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs): -> " + vbCrLf + Response)
                                            Else

                                                Dim PayInfo As String = GetPaymentInfoFromOrderSettings(MyOrder.FirstTransaction, MyOrder.Quantity, MyOrder.XAmount, MyOrder.XItem)

                                                Dim KnownAcc As String = SigAPI.GetAccountPublicKeyFromAccountID_RS(RM_AccountID)

                                                If KnownAcc.Contains(Application.ProductName + "-error") Then
                                                    'cant find account in blockchain, send message with pubkey to activate account

                                                    If Not PayInfo.Trim = "" Then

                                                        If PayInfo.Contains("PayPal-E-Mail=") Then
                                                            Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                            Dim ColWordsString As String = ColWords.GenerateColloquialWords(MyOrder.FirstTransaction, True, "-", 5)

                                                            PayInfo += " Reference/Note=" + ColWordsString
                                                        End If

                                                        Dim T_MsgStr As String = "Account activation" ' "AT=" + MyOrder.ATRS + " TX=" + MyOrder.FirstTransaction + " " + Dbl2LVStr(MyOrder.XAmount, Decimals) + " " + MyOrder.XItem + " " + PayInfo
                                                        Dim TXr As String = SigAPI.SendMessage(RM_AccountID, T_MsgStr,, True,, RM_AccountPublicKey)

                                                        If TXr.Contains(Application.ProductName + "-error") Then
                                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                            out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs): -> " + vbCrLf + TXr)
                                                        Else

                                                        End If

                                                    End If

                                                End If

                                            End If

                                            RelMsg.Setted = True
                                            RelevantMsgsBuffer(ii) = RelMsg
                                        End If

                                        Exit For

                                    End If

                                End If

                            End If

                        End If

                    Next

                Else

                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectedMsgs): -> ASK=" + RM_Ask + " ACCID=" + RM_AccIDULong.ToString + " Pubkey=" + RM_AccountPublicKey)

                End If

            End If

        Next


    End Sub

    Sub GetNuBlock(ByVal Node As Object)
        Dim BCR As ClsSignumAPI = New ClsSignumAPI(Node)
        NuBlock = BCR.GetCurrentBlock
    End Sub

    Sub BlockFeeThread(ByVal Node As Object)

        Dim BCR As ClsSignumAPI = New ClsSignumAPI(Node)
        MultiInvoker(StatusBlockLabel, "Text", "New Blockheight: " + Block.ToString)

        Fee = BCR.GetSlotFee
        MultiInvoker(StatusFeeLabel, "Text", "Current Slotfee: " + String.Format("{0:#0.00000000}", Fee))

        UTXList = BCR.C_UTXList

    End Sub

    ''' <summary>
    ''' The Thread who coordinates (loadbalance) the API Request subthreads
    ''' </summary>
    Sub GetThread()

        Dim NuNodeList As List(Of String) = New List(Of String)(NodeList.ToArray)

        Dim WExitCom As Boolean = True
        While WExitCom

            Try

                Dim Cnt As Integer = APIRequestList.Count

                For i As Integer = 0 To Cnt - 1

                    If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                        Continue While
                    End If

                    Dim Request As S_APIRequest = New S_APIRequest

                    Try
                        Request = APIRequestList(i)
                    Catch ex As Exception
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While1): -> " + ex.Message)
                    End Try


                    If Request.Command = "Exit()" Then
                        Exit While
                    End If

                    If Request.Status = "Wait..." Then

                        If NuNodeList.Count > 0 Then
                            Request.Node = NuNodeList(0)

                            Request.RequestThread = New Threading.Thread(AddressOf SubGetThread)
                            Request.Status = "Requesting..."
                            Request.RequestThread.Start({i, Request})

                            NuNodeList.RemoveAt(0)

                            If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                                Continue While
                            End If

                            Try
                                APIRequestList(i) = Request
                            Catch ex As Exception
                                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While2): -> " + ex.Message)
                            End Try

                            Continue For

                        End If

                    ElseIf Request.Status = "Not Ready..." Then

                        NuNodeList.Remove(Request.Node)
                        NodeList.Remove(Request.Node)
                        NodeList.Add(Request.Node)

                        If NuNodeList.Count > 0 Then
                            Request.Node = NuNodeList(0)
                        End If

                        Request.RequestThread = New Threading.Thread(AddressOf SubGetThread)
                        Request.Status = "Requesting..."

                        If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                            Continue While
                        End If

                        Request.RequestThread.Start({i, Request})

                        Try
                            APIRequestList(i) = Request
                        Catch ex As Exception
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While3): -> " + ex.Message)
                        End Try

                    ElseIf Request.Status = "Requesting..." Then
                        'loadbalancing
                        NuNodeList.Remove(Request.Node)
                    ElseIf Request.Status = "Responsed" Then

                        Request.Status = "Ready"

                        Dim founded As Boolean = False

                        For Each Node In NuNodeList
                            If Node = Request.Node Then
                                founded = True

                                If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                                    Continue While
                                End If

                                Try
                                    APIRequestList(i) = Request
                                Catch ex As Exception
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While4): -> " + ex.Message)
                                End Try

                                Exit For
                            End If
                        Next

                        If Not founded Then
                            'loadbalancing
                            NuNodeList.Add(Request.Node)
                        End If

                    End If

                Next

            Catch ex As Exception
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While): -> " + ex.Message)

                'MultiInvoker(StatusLabel, "Text", "GetThread(): " + ex.Message)
            End Try

        End While

        If InfoOut Then
            Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
            IOut.Info2File(Application.ProductName + "-info from GetThread(): -> end while")
        End If

        Try

            Dim DelIdx As Integer = -1
            For i As Integer = 0 To APIRequestList.Count - 1

                Dim Request As S_APIRequest = APIRequestList(i)

                If Request.Command = "Exit()" Then
                    DelIdx = i
                Else
                    If Request.RequestThread.IsAlive Then
                        Request.RequestThread.Abort()
                    End If

                End If
            Next

            If DelIdx <> -1 Then
                APIRequestList.RemoveAt(DelIdx)
            End If

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in GetThread(): -> " + ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' The SubThread who process the request
    ''' </summary>
    ''' <param name="Input">The input to work with Input(0) = List-Index; Input(1) = APIRequest</param>
    Sub SubGetThread(ByVal Input As Object)

        Try

            Dim Index As Integer = DirectCast(Input(0), Integer)
            Dim Request As S_APIRequest = DirectCast(Input(1), S_APIRequest)

            Dim Command As String = Request.Command

            Dim Parameter As String = Between(Command, "(", ")", GetType(String), True)
            Command = Command.Replace(Parameter, "")
            Dim ParameterList As List(Of String) = New List(Of String)

            If Parameter.Contains(",") Then
                ParameterList.AddRange(Parameter.Split(","))
            Else
                ParameterList.Add(Parameter)
            End If

            Select Case Command

                Case "GetDetails()"

                    Dim SignaAPI As ClsSignumAPI = New ClsSignumAPI(Request.Node)
                    Dim ATStrList As List(Of String) = SignaAPI.GetATDetails(ParameterList(0))

                    If Not ATStrList.Count = 0 Then

                        Dim AT As String = BetweenFromList(ATStrList, "<at>", "</at>")
                        Dim ATRS As String = BetweenFromList(ATStrList, "<atRS>", "</atRS>")
                        Dim REFMC As String = BetweenFromList(ATStrList, "<referenceMachineCode>", "</referenceMachineCode>")

                        If REFMC.Trim <> "True" And REFMC.Trim <> "False" Then
                            REFMC = "False"
                        End If

                        Dim ReferenceMachineCode As Boolean = CBool(REFMC)

                        If ReferenceMachineCode Then

#Region "get AT details"

                            Dim Creator As String = BetweenFromList(ATStrList, "<creator>", "</creator>")
                            Dim CreatorRS As String = BetweenFromList(ATStrList, "<creatorRS>", "</creatorRS>")
                            Dim Name As String = BetweenFromList(ATStrList, "<name>", "</name>")
                            Dim Description As String = BetweenFromList(ATStrList, "<description>", "</description>")

                            Dim Balance As String = BetweenFromList(ATStrList, "<balanceNQT>", "</balanceNQT>")
                            If Balance = "" Then
                                Balance = "0"
                            End If

                            Dim BalDbl As Double = SignaAPI.Planck2Dbl(CULng(Balance))

                            Dim MachineData As String = BetweenFromList(ATStrList, "<machineData>", "</machineData>")

                            Dim MachineDataULongList As List(Of ULong) = SignaAPI.DataStr2ULngList(MachineData)

                            '0 = Address Initiator = null
                            '1 = Address Responder = null

                            '2 = Long InitiatorsCollateral 
                            '3 = Long RespondersCollateral
                            '4 = Long BuySellAmount

                            '5 = Boolean SellOrder = False

                            Dim Initiator As ULong = 0
                            Dim InitiatorRS As String = ""
                            Dim Responser As ULong = 0
                            Dim ResponserRS As String = ""

                            Dim InitiatorCollateral As Double = 0.0
                            Dim ResponserCollateral As Double = 0.0

                            Dim BuySellAmount As Double = 0.0

                            Dim SellOrder As Boolean = False

                            If MachineDataULongList.Count > 0 Then
                                Initiator = MachineDataULongList(0)

                                If Initiator = CULng(0) Then
                                    'InitiatorRS = ""
                                Else
                                    InitiatorRS = BetweenFromList(SignaAPI.RSConvert(Initiator.ToString), "<accountRS>", "</accountRS>")
                                End If

                                Responser = MachineDataULongList(1)

                                If Responser = CULng(0) Then
                                    ResponserRS = ""
                                Else
                                    ResponserRS = BetweenFromList(SignaAPI.RSConvert(Responser.ToString), "<accountRS>", "</accountRS>")
                                End If

                                InitiatorCollateral = SignaAPI.Planck2Dbl(MachineDataULongList(2))
                                ResponserCollateral = SignaAPI.Planck2Dbl(MachineDataULongList(3))

                                BuySellAmount = SignaAPI.Planck2Dbl(MachineDataULongList(4))

                                SellOrder = False

                                If MachineDataULongList(7) = CULng(1) Then
                                    SellOrder = True
                                End If

                            End If



                            Dim Frozen As Boolean = False
                            Dim FrozenStr As String = BetweenFromList(ATStrList, "<frozen>", "</frozen>")

                            If Not FrozenStr = "" Then
                                Frozen = CBool(FrozenStr)
                            End If


                            Dim Running As Boolean = False
                            Dim RunningStr As String = BetweenFromList(ATStrList, "<running>", "</running>")

                            If Not RunningStr = "" Then
                                Running = CBool(RunningStr)
                            End If


                            Dim Stopped As Boolean = False
                            Dim StoppedStr As String = BetweenFromList(ATStrList, "<stopped>", "</stopped>")

                            If Not StoppedStr = "" Then
                                Stopped = CBool(StoppedStr)
                            End If



                            Dim Finished As Boolean = False
                            Dim FinishedStr As String = BetweenFromList(ATStrList, "<finished>", "</finished>")

                            If Not FinishedStr = "" Then
                                Finished = CBool(FinishedStr)
                            End If


                            Dim Dead As Boolean = False
                            Dim DeadStr As String = BetweenFromList(ATStrList, "<dead>", "</dead>")

                            If Not DeadStr = "" Then
                                Dead = CBool(DeadStr)
                            End If


                            Dim SBLSAT As PFPForm.S_PFPAT = New PFPForm.S_PFPAT With {
                                    .AT = AT,
                                    .ATRS = ATRS,
                                    .Creator = Creator,
                                    .CreatorRS = CreatorRS,
                                    .Description = Description,
                                    .Name = Name,
                                    .Sellorder = SellOrder,
                                    .Initiator = InitiatorRS,
                                    .InitiatorsCollateral = InitiatorCollateral,
                                    .ResponsersCollateral = ResponserCollateral,
                                    .BuySellAmount = BuySellAmount,
                                    .Responser = ResponserRS,
                                    .Balance = Balance,
                                    .Frozen = Frozen,
                                    .Running = Running,
                                    .Stopped = Stopped,
                                    .Finished = Finished,
                                    .Dead = Dead
                                }


#End Region

#Region "get AT TXs"
                            Dim UseBuffer As Boolean = False
                            If DEXATList.Count > 0 Then
                                UseBuffer = True
                            End If


                            Dim BLSTXs As List(Of S_PFPAT_TX) = GetAccountTXList(ParameterList(0), Request.Node,, UseBuffer) 'TODO: FromTimestamp

                            SBLSAT.AT_TXList = New List(Of PFPForm.S_PFPAT_TX)(BLSTXs.ToArray)
                            SBLSAT.Status = GetCurrentBLSATStatus(BLSTXs)
                            SBLSAT.XItem = GetCurrentBLSATXItem(BLSTXs)
                            SBLSAT.XAmount = GetCurrentBLSATXAmount(BLSTXs)

#End Region

#Region "Convert AT TXs to Orders"
                            SBLSAT.AT_OrderList = ConvertTXs2Orders(SBLSAT)
#End Region

                            Request.Result = SBLSAT
                            Request.Status = "Responsed"
                        Else
                            Request.Result = AT + "," + ATRS + ",False"
                            Request.Status = "Responsed"
                        End If
                    Else
                        Request.Status = "Not Ready..."
                    End If

                Case Else

            End Select

            APIRequestList(Index) = Request

        Catch ex As Exception

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in SubGetThread(): -> " + ex.Message)
        End Try

    End Sub

    Function MultithreadMonitor() As Boolean

        Dim Out As ClsOut = New ClsOut(Application.StartupPath)

        Try
            StatusBar.Visible = True
            StatusBar.Maximum = APIRequestList.Count

            Dim StillRun = True
            Dim LastIDX As Integer = 0
            While StillRun
                StillRun = False

                Dim APIRequests As List(Of S_APIRequest) = New List(Of S_APIRequest)(APIRequestList.ToArray)

                Dim HowManyReady As Integer = 0
                Dim LoadTxt As String = ""
                For i As Integer = 0 To APIRequests.Count - 1

                    Dim APIRequest As S_APIRequest = APIRequests(i)

                    If APIRequest.Status = "Ready" Then
                        HowManyReady += 1
                        LastIDX = i
                    Else
                        If LoadTxt.Trim = "" And LastIDX < i Then
                            LoadTxt = APIRequest.Command + "###" + APIRequest.Status + " " + APIRequest.Node
                        End If

                        StillRun = True
                    End If

                Next

                StatusLabel.Text = LoadTxt.Replace("###", " (" + HowManyReady.ToString + "/" + APIRequests.Count.ToString + ") ")
                StatusBar.Value = HowManyReady

                Application.DoEvents()

            End While

            StatusBar.Visible = False

            Return True

        Catch ex As Exception
            Out.ErrorLog2File(Application.ProductName + "-error in MultithreadMonitor(): -> " + ex.Message)
        End Try

        Return False

    End Function

#End Region

#Region "Multiinvoker"

    Delegate Sub MultiDelegate(ByVal params As Object)
    Sub MultiInvoker(ByVal obj As Object, ByVal prop As Object, ByVal val As Object)

        Try

            Dim RequireInvoke As Boolean = True

            'Select Case obj.GetType.Name
            '    Case GetType(Label).Name
            '        Dim T_Lab As Label = DirectCast(obj, Label)
            '        RequireInvoke = T_Lab.InvokeRequired

            '    Case GetType(ListView).Name
            '        Dim T_LV As ListView = DirectCast(obj, ListView)
            '        RequireInvoke = T_LV.InvokeRequired

            '    Case GetType(GroupBox).Name
            '        Dim T_GB As GroupBox = DirectCast(obj, GroupBox)
            '        RequireInvoke = T_GB.InvokeRequired

            '    Case GetType(ListBox).Name
            '        Dim T_LiBo As ListBox = DirectCast(obj, ListBox)
            '        RequireInvoke = T_LiBo.InvokeRequired

            '    Case Else

            'End Select




            If RequireInvoke Then
                Dim paramList As List(Of Object) = New List(Of Object)

                paramList.Add(obj)
                paramList.Add(prop)
                paramList.Add(val)

                Me.Invoke(New MultiDelegate(AddressOf invoker), paramList)
                ' Else
                'SetPropertyValueByName(obj, prop, val)
            End If


        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in MultiInvoker(): -> " + ex.Message)
        End Try

    End Sub
    Sub invoker(ByVal params As List(Of Object))
        SetPropertyValueByName(params.Item(0), params.Item(1), params.Item(2))
    End Sub
    Public Function SetPropertyValueByName(obj As Object, name As String, value As Object) As Boolean

        Try

            Dim prop As Object = obj.GetType().GetProperty(name, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            If IsNothing(prop) Then
                Return False
            End If

            If obj.GetType = GetType(ListView) And name = "Items" Then

                If value(0) = "Clear" Then
                    obj.Items.Clear()
                ElseIf value(0) = "Add" Then
                    Dim Val = value(1)
                    obj.Items.Add(Val)
                ElseIf value(0) = "Insert" Then
                    Dim Val = value.Item(2)
                    Dim Idx = value.Item(1)
                    obj.Items.Insert(Idx, Val)
                End If

                Return True

            ElseIf obj.GetType = GetType(ListBox) And name = "Items" Then

                If value(0) = "Clear" Then
                    obj.Items.Clear()
                ElseIf value(0) = "Add" Then
                    Dim Val = value(1)
                    obj.Items.Add(Val)
                ElseIf value(0) = "Insert" Then
                    Dim Val = value(2)
                    Dim Idx = value(1)
                    obj.Items.Insert(Idx, Val)
                End If

                Return True

            Else

                If prop.CanWrite Then
                    prop.SetValue(obj, value, Nothing)
                    Return True
                End If
            End If

            Return False

        Catch ex As Exception

            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in SetPropertyValueByName(): -> " + ex.Message)
            Return False
        End Try

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtChartGFXOnOff.Click

        If TTTL.TradeTrackTimer.Enabled Then
            TTTL.TradeTrackTimer.Enabled = False
            SplitContainer2.Panel1.Visible = False
        Else
            SplitContainer2.Panel1.Visible = True
            TTTL.TradeTrackTimer.Enabled = True
        End If

    End Sub

#End Region

#Region "Test"



#End Region 'Test

End Class


'####################################################################################################
'############################## Für die Spaltensortierung in ListViews ##############################
'# Benutzbeispiele für die Sortierung von ListView-Spalten
'#
'# Beispiel 1:
'# Dim SortList As List(Of Object) = New List(Of Object)
'# SortList.Add({0, 0})
'# SortList.Add({1, 0})
'# SortList.Add({3, 1})
'# ListView1.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, ListView1, SortList)
'#
'# Beispiel 2:
'# Dim SortArrayItem1() = {0, 0}
'# Dim SortArrayItem2() = {1, 0}
'# Dim SortArrayItem3() = {3, 1}
'# Dim SortArray() = {SortArrayItem1, SortArrayItem2, SortArrayItem3}
'# ListView1.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, ListView1, SortArray)
'#
'# Beispiel 3:
'# Dim SortArray = {New Object() {"ColumnText1", 0}, New Object() {"ColumnText2", 0}, New Object() {"ColumnText3", 1}}
'# ListView1.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, ListView1, SortArray)
'#
'# Beispiel 4(inline):
'# ListView1.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, ListView1, New List(Of Object)({New Object() {"ColumnText1", 0}, New Object() {"ColumnText2", 0}, New Object() {"ColumnText3", 1}}))
'#
'####################################################################################################

''' <summary>
''' Beispiel Standard:
''' ListView1.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0, 0, 1, 0, 3, 1)
''' </summary>
Public Class ListViewItemExtremeSorter
    Implements IComparer

    Private SortReihenfolge As SortOrder

    Public Structure Sortiereigenschaften
        Dim Index As Integer
        Dim Alphanum As Integer
    End Structure

    Dim SortierList As List(Of Sortiereigenschaften) = New List(Of Sortiereigenschaften)


    ''' <summary>
    ''' Sortiert die angegebene(n) Spalte(n) der ListView
    ''' </summary>
    ''' <param name="Sortfolge">Die SortierRehenfolge (Aufsteigend oder Absteigend)</param>
    ''' <param name="SortList">Liste mit Indexschlüssel als Integer und Alphanumerische Angaben als Integer (z.b. New List(Of Object) ({New Object() {0, 0}, New Object() {7, 0}, New Object() {8, 1}}))</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sortfolge As SortOrder, ByVal SortList As List(Of Object))

        SortReihenfolge = Sortfolge

        For Each SortItem In SortList
            Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

            Itemeigenschaft.Index = SortItem(0)
            Itemeigenschaft.Alphanum = SortItem(1)
            SortierList.Add(Itemeigenschaft)
        Next

    End Sub


    ''' <summary>
    ''' Sortiert die angegebene(n) Spalte(n) der ListView
    ''' </summary>
    ''' <param name="Sortfolge">Die SortierRehenfolge (Aufsteigend oder Absteigend)</param>
    ''' <param name="LV">Die ListView, aus der die ColumnIDX bezogen werden können</param>
    ''' <param name="SortList">Liste mit Indexschlüssel als Integer und Alphanumerische Angaben als Integer (z.b. New List(Of Object)({New Object() {"Datum", 0}, New Object() {"Mitarbeiter", 0}, New Object() {"Reihenfolge", 1}}))</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sortfolge As SortOrder, ByRef LV As ListView, ByVal SortList As List(Of Object)) 'Erste Überladung von New

        SortReihenfolge = Sortfolge

        For Each SortItem In SortList

            If SortItem(0).GetType.Name = GetType(String).Name Then
                Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

                Itemeigenschaft.Index = GetLVColNameFromSubItem(LV, SortItem(0))
                Itemeigenschaft.Alphanum = SortItem(1)
                SortierList.Add(Itemeigenschaft)
            Else
                Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

                Itemeigenschaft.Index = SortItem(0)
                Itemeigenschaft.Alphanum = SortItem(1)
                SortierList.Add(Itemeigenschaft)

            End If

        Next

    End Sub


    ''' <summary>
    ''' Sortiert die angegebene(n) Spalte(n) der ListView
    ''' </summary>
    ''' <param name="Sortfolge">Die SortierRehenfolge (Aufsteigend oder Absteigend)</param>
    ''' <param name="Pri">Primärschlüssel: Index der 1. Spalte (muss vorhanden sein)</param>
    ''' <param name="AlphaNrPri">Optionaler Parameter für Alphanummerische sortierung für Pri</param>
    ''' <param name="Sek">Optionaler Sekundärschlüssel: Index der 2. Spalte (-1, falls nicht genutzt)</param>
    ''' <param name="AlphaNrSek">Optionaler Parameter für Alphanummerische sortierung für Sek</param>
    ''' <param name="Ter">Optionaler Tertiärschlüssel: Index der 3. Spalte (-1, falls nicht genutzt)</param>
    ''' <param name="AlphaNrTer">Optionaler Parameter für Alphanummerische sortierung für Ter</param>
    ''' <param name="Qua">Optionaler Quartärschlüssel: Index der 4. Spalte (-1, falls nicht genutzt)</param>
    ''' <param name="AlphaNrQua">Optionaler Parameter für Alphanummerische sortierung für Qua</param>
    ''' <param name="Qui">Optionaler Quintärschlüssel: Index der 5. Spalte (-1, falls nicht genutzt)</param>
    ''' <param name="AlphaNrQui">Optionaler Parameter für Alphanummerische sortierung für Qui</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sortfolge As SortOrder, ByVal Pri As Integer, Optional ByVal AlphaNrPri As Integer = 1, Optional ByVal Sek As Integer = -1, Optional ByVal AlphaNrSek As Integer = 1, Optional ByVal Ter As Integer = -1, Optional ByVal AlphaNrTer As Integer = 1, Optional ByVal Qua As Integer = -1, Optional ByVal AlphaNrQua As Integer = 1, Optional ByVal Qui As Integer = -1, Optional ByVal AlphaNrQui As Integer = 1) 'Zweite Überladung von New
        SortReihenfolge = Sortfolge

        Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

        Itemeigenschaft.Index = Pri
        Itemeigenschaft.Alphanum = AlphaNrPri
        SortierList.Add(Itemeigenschaft)

        Itemeigenschaft.Index = Sek
        Itemeigenschaft.Alphanum = AlphaNrSek
        SortierList.Add(Itemeigenschaft)

        Itemeigenschaft.Index = Ter
        Itemeigenschaft.Alphanum = AlphaNrTer
        SortierList.Add(Itemeigenschaft)

        Itemeigenschaft.Index = Qua
        Itemeigenschaft.Alphanum = AlphaNrQua
        SortierList.Add(Itemeigenschaft)

        Itemeigenschaft.Index = Qui
        Itemeigenschaft.Alphanum = AlphaNrQui
        SortierList.Add(Itemeigenschaft)

    End Sub



    ''' <summary>
    ''' Vergleich von Spalten (Primär-/Sekündär-/Tertiärspalte)
    ''' Benennungen von lateinischen Ordnungszahlen: 1=Primär, 2=Sekundär, 3=Tertiär, 4=Quartär, 5=Quintär, 6=Sextär, 7=Septimär, 8=Oktaviär, 9=Noniär, 10=Dezimär, 11=Un(o)dezimär, 12=Duodezimär, 13=Ter(Tres)dezimär
    ''' zur Unterstützung des Sorts im ListView
    ''' </summary>
    ''' <param name="Element1">Element1 als ListViewItem</param>
    ''' <param name="Element2">Element2 als ListViewItem</param>
    ''' <returns>-1, 0 oder 1 (gemäß Vergleich)</returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal Element1 As Object, ByVal Element2 As Object) As Integer Implements IComparer.Compare

        If SortReihenfolge = SortOrder.None Then
            Return 0
        End If

        Try

            Dim SortierVergleich As Integer = 0

            For Each SortItem In SortierList
                If SortItem.Index = -1 Then Exit For

                Dim Element1TempSpalte As String = CType(Element1, ListViewItem).SubItems(SortItem.Index).Text.Trim
                Dim Element2TempSpalte As String = CType(Element2, ListViewItem).SubItems(SortItem.Index).Text.Trim

                If Element1TempSpalte.IndexOf(".") = 2 And Element1TempSpalte.LastIndexOf(".") = 5 Then
                    If Element1TempSpalte.Contains(" ") Then
                        Element1TempSpalte = Element1TempSpalte.Remove(Element1TempSpalte.IndexOf(" "))
                    End If

                    If Element2TempSpalte.Contains(" ") Then
                        Element2TempSpalte = Element2TempSpalte.Remove(Element2TempSpalte.IndexOf(" "))
                    End If

                    Element1TempSpalte = DateUSToGer(Element1TempSpalte).ToString.Replace(".", "")
                    Element2TempSpalte = DateUSToGer(Element2TempSpalte).ToString.Replace(".", "")
                Else
                    If SortItem.Alphanum = 1 Then
                        If Element1TempSpalte.Length < Element2TempSpalte.Length Then
                            For i As Integer = 1 To (Element2TempSpalte.Length - Element1TempSpalte.Length)
                                Element1TempSpalte = "0" + Element1TempSpalte
                            Next
                        End If
                        If Element1TempSpalte.Length > Element2TempSpalte.Length Then
                            For i As Integer = 1 To (Element1TempSpalte.Length - Element2TempSpalte.Length)
                                Element2TempSpalte = "0" + Element2TempSpalte
                            Next
                        End If

                    Else

                        If Element1TempSpalte.Length < Element2TempSpalte.Length Then
                            For i As Integer = 1 To (Element2TempSpalte.Length - Element1TempSpalte.Length)
                                Element1TempSpalte += "0"
                            Next
                        End If
                        If Element1TempSpalte.Length > Element2TempSpalte.Length Then
                            For i As Integer = 1 To (Element1TempSpalte.Length - Element2TempSpalte.Length)
                                Element2TempSpalte += "0"
                            Next
                        End If

                    End If
                End If

                SortierVergleich = String.Compare(Element1TempSpalte, Element2TempSpalte, True) ' Rückgabe: -1, 0 oder 1

                If SortReihenfolge = SortOrder.Descending Then
                    SortierVergleich = -SortierVergleich
                End If

                If SortierVergleich <> 0 Then
                    Return SortierVergleich
                End If

            Next

            Return SortierVergleich

        Catch ex As ArgumentOutOfRangeException
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in ListViewItemExtremeSorter.Compare()(): -> " + ex.Message)
            Return 0
        End Try

    End Function


    Function DateUSToGer(datum As String, Optional ByVal PlusTage As Integer = 0)

        Try


            If PlusTage = 0 Then
                'von YY.MM.DD zu DD.MM.YY (und DD.MM.YY zu YY.MM.DD)
                If datum.Length = 8 Then 'richtung egal da alles 2-stellig ist
                    Return datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2)
                End If

                'Umconvert (nur DD.MM.YYYY zu YY.MM.DD)
                If datum.IndexOf(".") = 2 And datum.LastIndexOf(".") = 5 Then
                    If datum.Length = 10 Then
                        Return datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2)
                    End If
                End If
            End If

            If PlusTage <> 0 Then
                Dim dat As Date = datum

                dat = dat.AddDays(PlusTage)

                datum = DateUSToGer(DateUSToGer(dat.ToShortDateString, 0), 0)

            End If

            Return datum 'wenn die länge nicht passt, das zurückgeben, was die funktion bekommen hat

        Catch ex As Exception
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.ErrorLog2File(Application.ProductName + "-error in DateUSToGer(): -> " + ex.Message)
            Return datum
        End Try

    End Function

End Class