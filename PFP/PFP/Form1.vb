
Option Strict On
Option Explicit On

Public Class PFPForm

    Dim SignumDarkBlue As Color = Color.FromArgb(255, 0, 102, 255)
    Dim SignumBlue As Color = Color.FromArgb(255, 0, 153, 255)
    Dim SignumLightGreen As Color = Color.FromArgb(255, 0, 255, 136)

    Property NodeList() As List(Of String) = New List(Of String)
    Property PrimaryNode() As String = ""

    Property NuBlock As Integer = 1
    Property Block() As Integer = 0
    Property Fee() As Double = 0.0
    Property UTXList() As List(Of List(Of String))
    Property RefreshTime() As Integer = 60

    Property CurrentMarket As String = ""
    Property MarketIsCrypto() As Boolean = False
    Property Decimals() As Integer = 8

    Property Boottime As Integer = 0


    Property CSVATList() As List(Of S_AT) = New List(Of S_AT)
    Property DEXATList As List(Of String) = New List(Of String)
    Property DEXContractList As List(Of ClsDEXContract) = New List(Of ClsDEXContract)

    Property APIRequestList As List(Of S_APIRequest) = New List(Of S_APIRequest)
    Property TCPAPITradeHistoryList As List(Of List(Of String)) = New List(Of List(Of String))

    Dim TradeTrackerSplitContainer As SplitContainer = New SplitContainer
    Dim PanelForTradeTrackerSlot As Panel = New Panel

    Dim TimeLineSplitContainer As SplitContainer = New SplitContainer
    Dim TTTL As TradeTrackerTimeLine = New TradeTrackerTimeLine

    Property CoBxChartSelectedItem As Integer = 0
    Property CoBxTickSelectedItem As Integer = 0

    Property InfoOut As Boolean = False


    Function GetPaymentInfoFromOrderSettings(ByVal TXID As ULong, Optional ByVal Quantity As Double = 0.0, Optional ByVal XAmount As Double = 0.0, Optional ByVal Market As String = "") As String

        Dim PaymentInfo As String = ""

        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(TXID)

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
                            Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal With {
                                .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                                .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                            }

                            Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", Quantity, XAmount, Market)
                            Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
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
    Function CheckPayPalOrder(ByVal ATID As ULong, ByVal PayPalOrder As String) As String

        Dim Status As String = ""

        If Not PayPalOrder = "0" And Not PayPalOrder = "" Then

            Dim APIOK As String = CheckPayPalAPI()
            If APIOK = "True" Then

                'PayPal Approving check
                Dim PPAPI As ClsPayPal = New ClsPayPal With {
                    .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                    .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                }

                Dim OrderDetails As List(Of String) = PPAPI.GetOrderDetails(PayPalOrder)
                Dim PayPalStatus As String = GetStringBetweenFromList(OrderDetails, "<status>", "</status>")

                If PayPalStatus = "APPROVED" Then
                    PPAPI = New ClsPayPal With {
                        .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                        .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                    }

                    OrderDetails = PPAPI.CaptureOrder(PayPalOrder)
                    PayPalStatus = GetStringBetweenFromList(OrderDetails, "<status>", "</status>")

                    If PayPalStatus = "COMPLETED" Then

                        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                        If MasterKeys.Count > 0 Then

                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), ATID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalOrder(1): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalOrder(2): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    Status = "COMPLETED"
                                End If

                            End If


                        Else

                            If GetINISetting(E_Setting.InfoOut, False) Then
                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                out.WarningLog2File(Application.ProductName + "-warning in CheckPayPalOrder(3): -> no Keys")
                            End If

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
    Function CheckPayPalTransaction(ByVal DEXContract As ClsDEXContract) As String

        Dim Status As String = ""

        Dim APIOK As String = CheckPayPalAPI()
        If APIOK = "True" Then

            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
            Dim CheckAttachment As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

            If Not DEXContract.CheckForUTX And Not DEXContract.CheckForTX Then

                If Not GetAutosignalTXFromINI(DEXContract.CurrentCreationTransaction) Then 'Check for autosignal-TX in Settings.ini and skip if founded

                    'PayPal Approving check
                    Dim PPAPI_GetPayPalTX_to_Autosignal_AT As ClsPayPal = New ClsPayPal With {
                        .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                        .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                    }

                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                    Dim TXDetails As List(Of List(Of String)) = PPAPI_GetPayPalTX_to_Autosignal_AT.GetTransactionList(ColWords.GenerateColloquialWords(DEXContract.CurrentCreationTransaction.ToString, True, "-", 5))

                    If TXDetails.Count > 0 Then

                        Dim PayPalStatus As String = GetStringBetweenFromList(TXDetails(0), "<transaction_status>", "</transaction_status>")
                        Dim Amount As String = GetStringBetweenFromList(TXDetails(0), "<transaction_amount>", "</transaction_amount>")

                        If Convert.ToDouble(Amount) >= DEXContract.CurrentXAmount And PayPalStatus.ToLower.Trim = "s" Then
                            'complete

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalTransaction(1): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalTransaction(2): -> " + vbCrLf + TX)
                                        End If

                                    Else

                                        Status = "COMPLETED"

                                        If SetAutosignalTX2INI(DEXContract.CurrentCreationTransaction) Then 'Set autosignal-TX in Settings.ini
                                            'ok
                                        End If
                                    End If

                                End If

                            Else

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.WarningLog2File(Application.ProductName + "-warning in CheckPayPalTransaction(3): -> no Keys")
                                End If

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
        Dim ID As ULong
        'Dim ATRS As String
        Dim IsDEX_AT As Boolean
        Dim HistoryOrders As String
    End Structure

    Structure S_APIRequest
        Dim RequestThread As Threading.Thread
        Dim Command As String
        Dim CommandAttachment As Object
        Dim Node As String
        Dim Status As String
        Dim Result As Object
    End Structure

#Region "GUI Control"

#Region "General"

    Property LoadRunning As Boolean = False
    Property Shutdown As Boolean = False
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If InfoOut Then
            Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
            IOut.Info2File(Application.ProductName + "-info from Form1_FormClosing(): -> app close (" + sender.ToString + " -> " + e.ToString + ")")
        End If

        'If ChBxTCPAPI.Checked Then
        Dim Wait As Boolean = TCPAPI.StopAPIServer()
        If Not IsNothing(DEXNET) Then
            DEXNET.StopServer()
        End If

        StopINIClass()

        If LoadRunning Then
            ClsMsgs.MBox("Shutting down, please wait...", "Shutdown", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.OneOnly),, ClsMsgs.Status.Information, 3, ClsMsgs.Timer_Type.AutoOK)
            e.Cancel = True
            Shutdown = True
        Else
            Dim TestMultiExit As S_APIRequest = New S_APIRequest With {
                .Command = "Exit()"
            }
            APIRequestList.Add(TestMultiExit)
        End If
        'End If

    End Sub

    Dim MaxWidth As Integer = Screen.PrimaryScreen.Bounds.Width
    Dim MaxHeight As Integer = Screen.PrimaryScreen.Bounds.Height

    Dim TTS As TradeTrackerSlot = New TradeTrackerSlot

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Me.Text = "Codename: Perls for Pigs (TestNet) " & MaxWidth.ToString & "/" & MaxHeight.ToString + " TO " + Me.Size.Width.ToString + "/" + Me.Size.Height.ToString

        Dim Wait As Boolean = InitiateINI()

        'Wait = ReloadINI()

        InfoOut = GetINISetting(E_Setting.InfoOut, False)
        TCPAPI = New ClsTCPAPI(GetINISetting(E_Setting.TCPAPIServerPort, 8130), GetINISetting(E_Setting.TCPAPIShowStatus, False))

        Dim GasFee As Double = ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)

        Label16.Text = "+ " + Dbl2LVStr(GasFee, 3) + " Signa Gas fee"

#Region "TradeTracker"

        TradeTrackerSplitContainer.BackColor = Color.Transparent

        TradeTrackerSplitContainer.Dock = DockStyle.Fill
        TradeTrackerSplitContainer.Orientation = Orientation.Horizontal

        TradeTrackerSplitContainer.SplitterDistance = 70

        TradeTrackerSplitContainer.FixedPanel = FixedPanel.Panel1

        PanelForTradeTrackerSlot.AutoScroll = True
        PanelForTradeTrackerSlot.Dock = DockStyle.Fill
        PanelForTradeTrackerSlot.BackColor = Color.Transparent


        TradeTrackerSplitContainer.BorderStyle = BorderStyle.FixedSingle

        TimeLineSplitContainer.BackColor = Color.Transparent

        TimeLineSplitContainer.Dock = DockStyle.Fill
        TimeLineSplitContainer.IsSplitterFixed = True
        TimeLineSplitContainer.BorderStyle = BorderStyle.FixedSingle
        'TLS.SplitterDistance = 32
        TimeLineSplitContainer.Size = New Size(0, 70)
        'WTTL.Dock = DockStyle.Fill
        TTTL.Height = TimeLineSplitContainer.Height
        TTTL.TradeTrackTimerEnable = False

        AddHandler TTTL.TimerTick, AddressOf TradeTrackerTimeLine1_TimerTick

        Dim LabChart As Label = New Label With {
            .Text = "Chart (Days): "
        }
        LabChart.Font = New Font(LabChart.Font, FontStyle.Bold)
        LabChart.Location = New Point(0, 10)

        Dim CoBxChart As ComboBox = New ComboBox With {
            .Location = New Point(LabChart.Width, 10)
        }
        CoBxChart.Font = New Font(CoBxChart.Font, FontStyle.Bold)
        CoBxChart.Size = New Size(50, CoBxChart.Size.Height)
        CoBxChart.DropDownStyle = ComboBoxStyle.DropDownList
        CoBxChart.Name = "CoBxChart"
        CoBxChart.Items.AddRange({1, 3, 7, 15, 30})
        CoBxChart.SelectedItem = CoBxChart.Items(1)

        CoBxChartSelectedItem = Convert.ToInt32(CoBxChart.Items(1))
        AddHandler CoBxChart.DropDownClosed, AddressOf CoBx_Chart_Handler

        'Dim BtRightLeft As Button = New Button
        'BtRightLeft.Location = New Point(CoBxChart.Location.X + CoBxChart.Width, 10)
        'BtRightLeft.Text = "<"
        'BtRightLeft.Size = New Size(20, 20)
        'BtRightLeft.Font = New Font(BtRightLeft.Font, FontStyle.Bold)
        'BtRightLeft.Name = "BtRightLeft"



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

        CoBxTickSelectedItem = Convert.ToInt32(CoBxTick.Items(2))
        AddHandler CoBxTick.DropDownClosed, AddressOf CoBx_Tick_Handler

        TimeLineSplitContainer.Panel1.Controls.AddRange({LabChart, LabTick, CoBxChart, CoBxTick})
        TimeLineSplitContainer.Panel2.Controls.Add(TTTL)

        TTS.TTTL = TTTL

        TTS.Location = New Point(0, 0)
        TTS.Dock = DockStyle.Fill
        TTS.LabExch.Text = "booting..."
        TTS.LabPair.Text = "booting..."

        TradeTrackerSplitContainer.Panel1.Controls.Add(TimeLineSplitContainer)
        PanelForTradeTrackerSlot.Controls.Add(TTS)
        TradeTrackerSplitContainer.Panel2.Controls.Add(PanelForTradeTrackerSlot)

        SplitContainer2.Panel1.Controls.Add(TradeTrackerSplitContainer)
#End Region

#Region "check PassPhrase and Address"
        BlockTimer.Enabled = False

        Dim T_PassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")
        Dim T_Address As String = GetINISetting(E_Setting.Address, "")

        If T_PassPhrase.Trim = "" And T_Address.Trim = "" Then

            Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()

            T_PassPhrase = GetINISetting(E_Setting.PassPhrase, "")
            T_Address = GetINISetting(E_Setting.Address, "")
        End If

        Dim T_AddressList As List(Of String) = ConvertAddress(T_Address)

        If T_PassPhrase.Trim = "" Then

            If T_AddressList.Count = 0 Then

                ClsMsgs.MBox("No PassPhrase or Address set, program will close.", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                Application.Exit()
                Exit Sub

            ElseIf T_AddressList.Count = 1 Then
                GlobalPublicKey = ""
                GlobalAccountID = ULong.Parse(ConvertAddress(T_Address)(0))
                GlobalAddress = T_Address
            Else
                GlobalPublicKey = T_AddressList(1)
                GlobalAccountID = ULong.Parse(T_AddressList(0))
                GlobalAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(GlobalAccountID)


            End If

        Else

            If CheckPIN() Then
                Dim MasterKeys As List(Of String) = GetMasterKeys(T_PassPhrase)
                GlobalPublicKey = MasterKeys(0)
                GlobalAccountID = GetAccountID(GlobalPublicKey)
                GlobalAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(GlobalAccountID)

                If Not T_Address.Trim.Contains(GlobalAddress.Trim) Then

                    ClsMsgs.MBox("PassPhrase don't match Address, program will close.", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                    Application.Exit()
                    Exit Sub

                End If

            Else

                If T_AddressList.Count = 0 Then

                    ClsMsgs.MBox("Can't get Address from PassPhrase because its encrypted, program will close. Please make sure there is an Address in the Settings.ini", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                    Application.Exit()
                    Exit Sub

                ElseIf T_AddressList.Count = 1 Then
                    GlobalPublicKey = ""
                    GlobalAccountID = ULong.Parse(ConvertAddress(T_Address)(0))
                    GlobalAddress = T_Address
                Else
                    GlobalPublicKey = T_AddressList(1)
                    GlobalAccountID = ULong.Parse(T_AddressList(0))
                    GlobalAddress = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(GlobalAccountID)
                End If
            End If

        End If

        BlockTimer.Enabled = True

        TBSNOAddress.Text = GlobalAddress

#End Region

#Region "deprecated"
        'If T_PassPhrase.Trim = "" And GlobalAddress.Trim = "" Then

        '         BlockTimer.Enabled = False
        '         Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()

        '         T_PassPhrase = GetINISetting(E_Setting.PassPhrase, "")

        '         Dim T_AddressExtended As String = GetINISetting(E_Setting.Address, "")

        '         GlobalAddress = GetAccountRSFromID(ConvertAddress(T_AddressExtended)(0))

        '         If T_PassPhrase.Trim = "" And GlobalAddress.Trim = "" Then
        '             ClsMsgs.MBox("No PassPhrase or Address set, program will close.", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
        '             Application.Exit()
        '             Exit Sub
        '         End If

        '         TBSNOAddress.Text = GlobalAddress

        '         BlockTimer.Enabled = True

        '     ElseIf T_PassPhrase.Trim = "" And Not GlobalAddress.Trim = "" Then




        '     ElseIf Not T_PassPhrase.Trim = "" And GlobalAddress.Trim = "" Then

        '         Dim MasterKeys As List(Of String) = GetMasterKeys(T_PassPhrase)
        '         TBSNOAddress.Text = "TS-" + ClsReedSolomon.Encode(GetAccountID(MasterKeys(0))) 'TODO: remove TS- Prefix

        '     Else 'If Not T_PassPhrase.Trim = "" And Not GlobalAddress.Trim = ""

        '         If CheckPIN() Then
        '             Dim MasterKeys As List(Of String) = GetMasterKeys(T_PassPhrase)
        '             Dim MT_Address As String = "TS-" + ClsReedSolomon.Encode(GetAccountID(MasterKeys(0))) 'TODO: remove TS- Prefix

        '             If MT_Address.Trim = TBSNOAddress.Text.Trim Then
        '                 TBSNOAddress.Text = MT_Address
        '             Else

        '                 ClsMsgs.MBox("PassPhrase don't match Address, program will close.", "Unknown Address",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
        '                 Application.Exit()
        '                 Exit Sub

        '             End If
        '         End If

        '     End If

#End Region

        CurrentMarket = GetINISetting(E_Setting.LastMarketViewed, "USD")
        CoBxMarket.SelectedItem = CurrentMarket


        Dim Nodes As String = GetINISetting(E_Setting.Nodes, ClsSignumAPI._Nodes)

        NodeList.Clear()
        If Nodes.Contains(";") Then
            NodeList.AddRange(Nodes.Split(";"c))
        Else
            NodeList.Add(Nodes)
        End If

        SplitContainerSellFilter.Panel1Collapsed = True
        SplitContainerBuyFilter.Panel1Collapsed = True

        SetMethodFilter(ChLBSellFilterMethods, LoadMethodFilter(E_Setting.SellFilterMethods))
        SetMethodFilter(ChLBBuyFilterMethods, LoadMethodFilter(E_Setting.BuyFilterMethods))

        CoBxSellFilterMaxOrders.SelectedItem = GetINISetting(E_Setting.ShowMaxSellOrders, 10).ToString
        CoBxBuyFilterMaxOrders.SelectedItem = GetINISetting(E_Setting.ShowMaxBuyOrders, 10).ToString

        ChBxSellFilterShowAutoinfo.Checked = GetINISetting(E_Setting.SellFilterAutoinfo, False)
        ChBxBuyFilterShowAutoinfo.Checked = GetINISetting(E_Setting.BuyFilterAutoinfo, False)

        ChBxSellFilterShowAutofinish.Checked = GetINISetting(E_Setting.SellFilterAutofinish, False)
        ChBxBuyFilterShowAutofinish.Checked = GetINISetting(E_Setting.BuyFilterAutofinish, False)

        ChBxSellFilterShowPayable.Checked = GetINISetting(E_Setting.SellFilterPayable, False)
        ChBxBuyFilterShowPayable.Checked = GetINISetting(E_Setting.BuyFilterPayable, False)

        'InitiateDEXNET()

        PrimaryNode = GetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)

        RefreshTime = GetINISetting(E_Setting.RefreshMinutes, 1) * 60

        If GetINISetting(E_Setting.TCPAPIEnable, False) Then
            TCPAPI.StartAPIServer()
        End If

        'If TBSNOPassPhrase.Text.Trim = "" Then
        '    BlockTimer.Enabled = False
        '    Exit Sub
        'End If

        Dim T_DEXATList As List(Of List(Of String)) = GetDEXATsFromCSV()
        DEXATList.Clear()

        For Each T_DEX As List(Of String) In T_DEXATList
            If T_DEX(1) = "True" Then
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


#Region "TradeTracker"

    Sub CoBx_Chart_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxChart As ComboBox = DirectCast(sender, ComboBox)
        CoBxChartSelectedItem = Convert.ToInt32(CoBxChart.SelectedItem)

        ForceReload = True
        'BlockTimer_Tick(True, Nothing)

    End Sub
    Sub CoBx_Tick_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxTick As ComboBox = DirectCast(sender, ComboBox)
        CoBxTickSelectedItem = Convert.ToInt32(CoBxTick.SelectedItem)

        ForceReload = True

        'BlockTimer_Tick(True, Nothing)

    End Sub
    Private Sub TradeTrackerTimeLine1_TimerTick(sender As Object)

        For Each TTSlot As TradeTrackerSlot In PanelForTradeTrackerSlot.Controls

            Dim TempSC As SplitContainer = CType(TradeTrackerSplitContainer.Panel1.Controls(0), SplitContainer)
            Dim TempTimeLine As TradeTrackerTimeLine = CType(TempSC.Panel2.Controls(0), TradeTrackerTimeLine)
            Try

                TimeLineSplitContainer.SplitterDistance = TTSlot.SplitterDistance
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

#End Region

    Private Sub BlockTimer_Tick(sender As Object, e As EventArgs) Handles BlockTimer.Tick

        Boottime += 1

        'StatusBar.Visible = True
        'StatusBar.Maximum = RefreshTime
        'StatusBar.Value = RefreshTime - Boottime


        SetDEXNETRelevantMsgsToLVs()

        'MsgBox(SplitContainer2.Panel2.Width.ToString)
        If SplitContainer2.Panel1.Width < 500 Then
            If Not TimeLineSplitContainer.Panel1Collapsed Then
                TimeLineSplitContainer.Panel1Collapsed = True
                TTS.SlotSplitter.Panel1Collapsed = True
            End If
        Else
            If TimeLineSplitContainer.Panel1Collapsed Then
                TimeLineSplitContainer.Panel1Collapsed = False
                TTS.SlotSplitter.Panel1Collapsed = False
            End If
        End If


        Dim Wait As Boolean = False

        If Boottime >= RefreshTime Or ForceReload Then

            TTTL.TradeTrackTimer.Enabled = False
            'SplitContainer2.Panel1.Visible = False

            'Me.Text = "Codename: Perls for Pigs (TestNet) " & MaxWidth.ToString & "/" & MaxHeight.ToString + " TO " + Me.Size.Width.ToString + "/" + Me.Size.Height.ToString

            ReloadINI()

            If MaxWidth < Me.Size.Width Or MaxHeight < Me.Size.Height Then
                Wait = AutoResizeWindow()
            End If

            Boottime = 0

            If Checknodes() > 0 Then

                BlockTimer.Enabled = False

                TSSStatusImage.Text = "in Synchronization..."
                TSSStatusImage.Image = My.Resources.status_wait

                If CheckPIN() Then
                    TSSCryptStatus.Image = My.Resources.status_decrypted
                    TSSCryptStatus.Tag = "decrypted"

                    TSSCryptStatus.AutoToolTip = True
                    TSSCryptStatus.ToolTipText = "the PFP is Unlocked" + vbCrLf + "automation working"

                Else
                    TSSCryptStatus.Image = My.Resources.status_encrypted
                    TSSCryptStatus.Tag = "encrypted"

                    TSSCryptStatus.AutoToolTip = True
                    TSSCryptStatus.ToolTipText = "the PFP is Locked" + vbCrLf + "there is no automation"

                End If


                If Not IsNothing(DEXNET) Then

                    For i As Integer = 0 To DEXNET.Peers.Count - 1
                        Dim Peer As ClsDEXNET.S_Peer = DEXNET.Peers(i)

                        If Peer.Timeout = -1 Then
                            Continue For
                        End If

                        If Peer.Timeout = 0 Then
                            Peer.TCPClient.Close()
                            DEXNET.Peers(i) = Peer
                        Else
                            Peer.Timeout -= 1
                            DEXNET.Peers(i) = Peer
                        End If

                    Next

                    If DEXNET.Peers.Count = 0 Then
                        DEXNET.StopServer()
                        InitiateDEXNET()
                    Else
                        DEXNET.GetPing()
                    End If

                Else
                    InitiateDEXNET()
                End If

                Dim T_NuBlockThread As Threading.Thread = New Threading.Thread(AddressOf GetNuBlock)
                T_NuBlockThread.Start(DirectCast(PrimaryNode, Object))

                While T_NuBlockThread.IsAlive
                    Application.DoEvents()
                End While

                If NuBlock > Block Or ForceReload Then
                    Block = NuBlock
                    ForceReload = False

                    LoadRunning = True

                    Application.DoEvents()

                    Wait = Loading()
                    Wait = SetInLVs()

                    RefreshTime = GetINISetting(E_Setting.RefreshMinutes, 1) * 60

                    Dim CoBxChartVal As Integer = 1
                    Dim CoBxTickVal As Integer = 1

                    For Each CTRL As Object In TimeLineSplitContainer.Panel1.Controls


                        If CTRL.GetType.Name = GetType(ComboBox).Name Then

                            Dim T_CoBx As ComboBox = DirectCast(CTRL, ComboBox)

                            If T_CoBx.Name = "CoBxChart" Then
                                'Dim CoBxChart As ComboBox = DirectCast(CTRL, ComboBox)
                                CoBxChartVal = Convert.ToInt32(T_CoBx.SelectedItem)
                            End If

                            If T_CoBx.Name = "CoBxTick" Then
                                'Dim CoBxTick As ComboBox = DirectCast(CTRL, ComboBox)
                                CoBxTickVal = Convert.ToInt32(T_CoBx.SelectedItem)
                            End If

                        End If

                    Next

                    Dim ViewThread As Threading.Thread = New Threading.Thread(AddressOf LoadHistory)
                    ViewThread.Start(New List(Of Object)({CoBxChartVal, CoBxTickVal, CurrentMarket}))


                    'BlockTimer.Enabled = True

                    LoadRunning = False

                    If Shutdown Then
                        Me.Close()
                    End If

                Else

                End If

                BlockTimer.Enabled = True
            End If

            TTTL.TradeTrackTimer.Enabled = True

            Dim Status As E_ConnectionStatus = GetConnectionStatus(PrimaryNode, DEXNET)

            Select Case Status
                Case E_ConnectionStatus.Offline
                    TSSStatusImage.Text = "Offline"
                    TSSStatusImage.Image = My.Resources.status_offline
                Case E_ConnectionStatus.InSync
                    TSSStatusImage.Text = "in Synchronization..."
                    TSSStatusImage.Image = My.Resources.status_wait
                Case E_ConnectionStatus.NoDEXNETPeers
                    TSSStatusImage.Text = "no DEXNET-Connection"
                    TSSStatusImage.Image = My.Resources.status_wait
                Case E_ConnectionStatus.NoSignumAPI
                    TSSStatusImage.Text = "no DefaultNode-Connection"
                    TSSStatusImage.Image = My.Resources.status_wait
                Case E_ConnectionStatus.Online
                    TSSStatusImage.Text = "Online"
                    TSSStatusImage.Image = My.Resources.status_online
                Case Else
                    TSSStatusImage.Text = "Offline"
                    TSSStatusImage.Image = My.Resources.status_offline
            End Select

            'If GFXFlag Then
            '    SplitContainer2.Panel1.Visible = True
            'End If

        End If

    End Sub

    Property ForceReload As Boolean = False
    Private Sub CoBxMarket_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxMarket.SelectedIndexChanged, CoBxMarket.DropDownClosed

        CurrentMarket = Convert.ToString(CoBxMarket.SelectedItem)
        SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)

        ResetLVColumns()

        ForceReload = True

    End Sub
    Private Sub TBSNOPassPhrase_KeyPress(sender As Object, e As KeyPressEventArgs)

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
        CoBxMarket_SelectedIndexChanged(Nothing, Nothing)
    End Sub

#End Region

#Region "Marketdetails - Controls"

    Private Sub BtCreateNewAT_Click(sender As Object, e As EventArgs) Handles BtCreateNewAT.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new Payment channel" + vbCrLf + "(AT=Automated Transaction)?", "Create AT", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

            Dim NuList As List(Of String) = New List(Of String)

            Dim PublicKey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(TBSNOAddress.Text)

            If PublicKey.Contains(Application.ProductName + "-error") Then
                ClsMsgs.MBox("This account/address is not in Blockchain and has no Balance to create AT" + vbCrLf + vbCrLf + "Try to buy your first Signa from Sellorders without collateral.", "Account not in Blockchain",,, ClsMsgs.Status.Erro)
                Exit Sub
            End If

            Dim SignumResponse As String = SignumAPI.CreateAT(PublicKey)

            If SignumResponse.Contains(Application.ProductName + "-error") Then

                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in CreateAT(): -> " + SignumResponse)
                End If

                ClsMsgs.MBox("something went wrong:" + vbCrLf + SignumResponse, "Error",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                Exit Sub
            End If

            NuList = ClsSignumAPI.ConvertUnsignedTXToList(SignumResponse)
            Dim UTX As String = GetStringBetweenFromList(NuList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
            Dim SignumNET As ClsSignumNET = New ClsSignumNET()

            Dim Masterkeys As List(Of String) = GetPassPhrase()
            If Masterkeys.Count > 0 Then
                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, Masterkeys(1))
                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                NuList.Add("<transaction>" + TX + "</transaction>")

            Else

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" Then
                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                    NuList.Add("<transaction>" + TX + "</transaction>")
                Else
                    ClsMsgs.MBox("AT creation canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                    Exit Sub
                End If

            End If

            If NuList.Count = 0 Then
                ClsMsgs.MBox("Error creating new AT", "Error",,, ClsMsgs.Status.Erro)
                Exit Sub
            End If

            ' Dim NuATList As List(Of S_AT) = New List(Of S_AT)
            Dim NuAT As S_AT = New S_AT
            NuAT.ID = GetULongBetweenFromList(NuList, "<transaction>", "</transaction>")

            Dim AccRS As List(Of String) = SignumAPI.RSConvert(NuAT.ID)
            'NuAT.ATRS = GetStringBetweenFromList(AccRS, "<accountRS>", "</accountRS>")
            NuAT.IsDEX_AT = True

            'NuATList.Add(NuAT)
            'SaveATsToCSV(NuATList)

            ClsMsgs.MBox("New AT Created" + vbCrLf + vbCrLf + "TX: " + NuAT.ID.ToString, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

        End If

    End Sub
    Private Sub TBarCollateralPercent_Scroll(sender As Object, e As EventArgs) Handles TBarCollateralPercent.Scroll

        If TBarCollateralPercent.Value = 0 Then
            NUDSNOCollateral.Minimum = 0
            NUDSNOCollateral.Maximum = 0
            NUDSNOAmount.Maximum = 100
            NUDSNOCollateral.Value = 0.0D
            LabColPercentage.Text = "0 %"

        Else

            NUDSNOAmount.Maximum = Decimal.MaxValue '79228162514264337593543950335

            Dim T_Amount As Decimal = NUDSNOAmount.Value
            Dim T_Percentage As Decimal = Convert.ToDecimal(28 + (TBarCollateralPercent.Value * 2))

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

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, GlobalAccountID,)

        Try

            Dim BalList As List(Of String) = SignumAPI.GetBalance(TBSNOAddress.Text)
            TBSNOBalance.Text = GetDoubleBetweenFromList(BalList, "<available>", "</available>").ToString


            Dim MinAmount As Double = Convert.ToDouble(NUDSNOAmount.Value) '100,00000000
            Dim XItemMinAmount As Double = Convert.ToDouble(NUDSNOItemAmount.Value) '1,00000000


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


            Dim AccAmount As Double = GetDoubleBetweenFromList(SignumAPI.GetBalance(TBSNOAddress.Text), "<available>", "</available>")

            If LVOpenChannels.Items.Count > 0 Then

                Dim T_DEXContract As ClsDEXContract = Nothing

                Dim FoundOne As Boolean = False

                For Each LVi As ListViewItem In LVOpenChannels.Items

                    Dim Status As String = Convert.ToString(GetLVColNameFromSubItem(LVOpenChannels, "Status", LVi))

                    If Status = "Reserved for you" Then
                        FoundOne = True
                        T_DEXContract = DirectCast(LVi.Tag, ClsDEXContract)
                        Exit For
                    End If

                Next


                If Not FoundOne Then

                    For Each LVi As ListViewItem In LVOpenChannels.Items

                        If Not LVi.BackColor = Color.Crimson Then
                            T_DEXContract = DirectCast(LVi.Tag, ClsDEXContract)

                            If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Then
                                T_DEXContract = Nothing ' New clsDEXContract
                            Else
                                Exit For
                            End If

                        End If

                    Next
                End If


                If IsNothing(T_DEXContract) Then
                    ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                    BtSNOSetOrder.Text = "Set Order"
                    BtSNOSetOrder.Enabled = True
                    Exit Sub
                Else
                    If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Then
                        ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                        BtSNOSetOrder.Text = "Set Order"
                        BtSNOSetOrder.Enabled = True
                        Exit Sub
                    End If
                End If


                Dim Recipient As ULong = T_DEXContract.ID
                Dim Amount As Double = Convert.ToDouble(NUDSNOAmount.Value)
                Dim Fee As Double = Convert.ToDouble(NUDSNOTXFee.Value)
                Dim Collateral As Double = Convert.ToDouble(NUDSNOCollateral.Value)
                Dim Item As String = CurrentMarket


                Dim ItemAmount As Double = Convert.ToDouble(NUDSNOItemAmount.Value)

                If RBSNOSell.Checked Then

                    If AccAmount > Amount + Fee + Collateral Then
                        'enough balance

                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new SellOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, Decimals) + " " + Item, "Create SellOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = SignumAPI.SetBLSATSellOrder(MasterKeys(0), Recipient, Amount + Collateral, Collateral, Item, ItemAmount, Fee)

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Response)
                                    End If

                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Sell1): -> " + vbCrLf + TX)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If

                                End If
                            Else
                                'TODO: show Pinform

                                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                                Dim Response As String = ""

                                If Not GlobalPublicKey.Trim = "" Then

                                    Response = SignumAPI.SetBLSATSellOrder(GlobalPublicKey, Recipient, Amount + Collateral, Collateral, Item, ItemAmount, Fee)
                                    If Response.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Sell2): -> " + vbCrLf + Response)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        PinForm.TBUnsignedBytes.Text = UTX

                                    End If
                                End If

                                PinForm.ShowDialog()

                                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                    Response = SignumAPI.SetBLSATSellOrder(PinForm.PublicKey, Recipient, Amount + Collateral, Collateral, Item, ItemAmount, Fee)

                                    If Response.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Sell3): -> " + vbCrLf + Response)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else

                                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                        Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                        If TX.Contains(Application.ProductName + "-error") Then

                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(Sell4): -> " + vbCrLf + TX)
                                            End If

                                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                        Else
                                            ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                        End If

                                    End If

                                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                                    Dim TX As String = SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(Sell5): -> " + vbCrLf + TX)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If


                                Else

                                    ClsMsgs.MBox("SellOrder creation canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                                End If

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

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = SignumAPI.SetBLSATBuyOrder(MasterKeys(0), Recipient, Amount, Collateral, Item, ItemAmount, Fee)

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Response)
                                    End If

                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Buy1): -> " + vbCrLf + TX)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If

                                End If

                            Else
                                'TODO: show pinform

                                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)


                                Dim Response As String = ""

                                If Not GlobalPublicKey.Trim = "" Then

                                    Response = SignumAPI.SetBLSATSellOrder(GlobalPublicKey, Recipient, Amount + Collateral, Collateral, Item, ItemAmount, Fee)
                                    If Response.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Buy2): -> " + vbCrLf + Response)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        PinForm.TBUnsignedBytes.Text = UTX

                                    End If
                                End If


                                PinForm.ShowDialog()

                                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                    Response = SignumAPI.SetBLSATBuyOrder(PinForm.PublicKey, Recipient, Amount, Collateral, Item, ItemAmount, Fee)

                                    If Response.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Buy3): -> " + vbCrLf + Response)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else

                                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                        Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                        If TX.Contains(Application.ProductName + "-error") Then

                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Buy4): -> " + vbCrLf + TX)
                                            End If

                                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                        Else
                                            ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                        End If

                                    End If

                                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                                    Dim TX As String = SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSNOSetOrder_Click(Buy5): -> " + vbCrLf + TX)
                                        End If

                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)

                                    Else
                                        ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If

                                Else
                                    ClsMsgs.MBox("BuyOrder creation canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                                End If

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

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVSellorders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then
                BtBuy.Text = "cancel"
            Else

                Dim SAPI As ClsSignumAPI = New ClsSignumAPI

                Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerPubKey.Text = "copy seller public key"
                LVCMItemSellerPubKey.Tag = SAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress)

                AddHandler LVCMItemSellerPubKey.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerPubKey)

                Dim LVCMItemSellerID As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerID.Text = "copy sellerID"
                LVCMItemSellerID.Tag = GetAccountIDFromRS(T_DEXContract.CurrentInitiatorAddress)

                AddHandler LVCMItemSellerID.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerID)

                Dim LVCMItemSellerRS As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerRS.Text = "copy sellerRS"
                LVCMItemSellerRS.Tag = T_DEXContract.CurrentInitiatorAddress

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

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVBuyorders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then
                BtSell.Text = "cancel"
            Else

                Dim SAPI As ClsSignumAPI = New ClsSignumAPI

                Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerPubKey.Text = "copy buyer public key"
                LVCMItemSellerPubKey.Tag = SAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress)

                AddHandler LVCMItemSellerPubKey.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerPubKey)

                Dim LVCMItemSellerID As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerID.Text = "copy buyerID"
                LVCMItemSellerID.Tag = GetAccountIDFromRS(T_DEXContract.CurrentInitiatorAddress)

                AddHandler LVCMItemSellerID.Click, AddressOf Copy2CB
                LVContextMenu.Items.Add(LVCMItemSellerID)

                Dim LVCMItemSellerRS As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerRS.Text = "copy buyerRS"
                LVCMItemSellerRS.Tag = T_DEXContract.CurrentInitiatorAddress

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

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVSellorders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CheckForUTX Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtBuy.Text = OldTxt
                BtBuy.Enabled = True
                Exit Sub
            End If


            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            'Dim BLSAT_TX As S_PFPAT_TX = GetLastTXWithValues(T_DEXContract.AT_TXList, CurrentMarket)
            Dim Collateral As Double = T_DEXContract.CurrentInitiatorsCollateral ' ClsSignumAPI.Planck2Dbl(ULong.Parse(Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))


            If Not T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then

                Dim BalList As List(Of String) = SignumAPI.GetBalance(TBSNOAddress.Text)

                Dim Available As Double = 0.0
                Dim AvaStr As String = GetStringBetweenFromList(BalList, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Collateral + ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT) And Available > 0.0 Then

                    Dim MBoxMsg As String = "Do you really want to Buy " + Dbl2LVStr(T_DEXContract.CurrentBuySellAmount) + " Signa for " + Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " "
                    MBoxMsg += "from Seller: " + T_DEXContract.CurrentInitiatorAddress + "?" + vbCrLf + vbCrLf
                    MBoxMsg += "collateral: " + Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) + " Signa" + vbCrLf
                    MBoxMsg += "gas fees: " + Dbl2LVStr(ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)) + " Signa" + vbCrLf + vbCrLf
                    MBoxMsg += "this transaction will take effect in 1-3 Blocks (4-12 minutes)"

                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox(MBoxMsg, "Buy Order from AT: " + T_DEXContract.Address, ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()

                        If MasterKeys.Count > 0 Then

                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, Collateral, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(1): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(2): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    MBoxMsg = "SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX + vbCrLf + vbCrLf
                                    MBoxMsg += "please wait 1-2 Blocks (4-8 minutes) to get the payment-infos from Seller"

                                    ClsMsgs.MBox(MBoxMsg, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else
                            'TODO: Show PINForm

                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, Collateral, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(2a): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(2b): -> " + vbCrLf + TX)
                                        End If

                                    Else
                                        MBoxMsg = "SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX + vbCrLf + vbCrLf
                                        MBoxMsg += "please wait 1-2 Blocks (4-8 minutes) to get the payment-infos from Seller"

                                        ClsMsgs.MBox(MBoxMsg, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("BuyOrder acception canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If

                    End If

                Else
                    'not enough balance
                    Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance for the AT acception." + vbCrLf + "do you like to ask the seller for accepting this offer offchain?", "not enough balance", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Question)

                    If Result = ClsMsgs.CustomDialogResult.Yes Then
                        'TODO: Check RecipientPublicKey

                        Dim Masterkeys As List(Of String) = GetPassPhrase()
                        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                        If Masterkeys.Count > 0 Then
                            DEXNET.BroadcastMessage("<ATID>" + T_DEXContract.ID.ToString + "</ATID><Ask>WTB</Ask>", Masterkeys(1), Masterkeys(2), Masterkeys(0), SignumAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress))
                        End If

                    End If

                End If

            Else

                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the SellOrder?", "Cancel SellOrder?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                    Dim MasterKeys As List(Of String) = GetPassPhrase()

                    If MasterKeys.Count > 0 Then

                        Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                        If Response.Contains(Application.ProductName + "-error") Then

                            If GetINISetting(E_Setting.InfoOut, False) Then
                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(cancel1): -> " + vbCrLf + Response)
                            End If

                        Else

                            Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                            Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                            Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If TX.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(cancel2): -> " + vbCrLf + TX)
                                End If

                            Else
                                ClsMsgs.MBox("SellOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                            End If

                        End If

                    Else
                        'TODO: Show PINFrom

                        Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                        PinForm.ShowDialog()

                        If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                            Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(cancel1a): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtBuy_Click(cancel1b): -> " + vbCrLf + TX)
                                    End If

                                Else

                                    ClsMsgs.MBox("SellOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                                End If

                            End If

                        Else
                            ClsMsgs.MBox("SellOrder cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                        End If


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

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVBuyorders.SelectedItems(0).Tag, ClsDEXContract)

            Dim PayInfo As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Method", LVBuyorders.SelectedItems(0)))
            Dim AutoInfo As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Autoinfo", LVBuyorders.SelectedItems(0)))
            Dim Autofinish As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Autofinish", LVBuyorders.SelectedItems(0)))

            If T_DEXContract.CheckForUTX Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtSell.Text = OldTxt
                BtSell.Enabled = True
                Exit Sub
            End If


            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            'Dim BLSAT_TX As S_PFPAT_TX = GetLastTXWithValues(T_DEXContract.AT_TXList, CurrentMarket)
            Dim BuyAmount As Double = T_DEXContract.CurrentBuySellAmount ' ClsSignumAPI.Planck2Dbl(ULong.Parse(Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))
            Dim XItem As String = T_DEXContract.CurrentXItem ' Between(BLSAT_TX.Attachment, "<xItem>", "</xItem>", GetType(String))
            Dim XAmount As Double = T_DEXContract.CurrentXAmount ' ClsSignumAPI.Planck2Dbl(ULong.Parse(Between(BLSAT_TX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
            Dim Sum As Double = BuyAmount + T_DEXContract.CurrentInitiatorsCollateral - ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)

            If Not T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then

                Dim BalList As List(Of String) = SignumAPI.GetBalance(TBSNOAddress.Text)

                Dim Available As Double = 0.0
                Dim AvaStr As String = GetStringBetweenFromList(BalList, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > BuyAmount + 1.0 Then

                    Dim MBoxMsg As String = "Do you really want to Sell " + Dbl2LVStr(T_DEXContract.CurrentBuySellAmount) + " Signa for " + Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " "
                    MBoxMsg += "to Buyer: " + T_DEXContract.CurrentInitiatorAddress + "?" + vbCrLf + vbCrLf
                    MBoxMsg += "collateral: " + Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) + " Signa" + vbCrLf
                    MBoxMsg += "gas fees: " + Dbl2LVStr(ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)) + " Signa" + vbCrLf + vbCrLf
                    MBoxMsg += "this transaction will take effect in 1-2 Blocks (4-8 minutes)" + vbCrLf

                    If AutoInfo = "True" Then
                        MBoxMsg += "you will also inform the buyer with payment info!" + vbCrLf
                    End If

                    If Autofinish = "True" Then
                        MBoxMsg += "you also accept the Autofinishing!" + vbCrLf
                    End If


                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox(MBoxMsg, "Sell Order to AT: " + T_DEXContract.Address, ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then


                        Dim MasterKeys As List(Of String) = GetPassPhrase()


                        If MasterKeys.Count > 0 Then

                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, Sum, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(1): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(2): -> " + vbCrLf + TX)
                                    End If

                                Else

                                    MBoxMsg = "BuyOrder Accepted" + vbCrLf
                                    MBoxMsg += "TX: " + TX + vbCrLf
                                    MBoxMsg += "Recipient: " + T_DEXContract.Address + vbCrLf + vbCrLf

                                    'Public Enum E_PayType
                                    '    Bankaccount = 0
                                    '    PayPal_E_Mail = 1
                                    '    PayPal_Order = 2
                                    '    Self_Pickup = 3
                                    '    Other = 4
                                    'End Enum

                                    'TODO: OwnPayType 

                                    Dim OwnPayType As String = GetINISetting(E_Setting.PaymentType, "Self Pickup")

                                    If PayInfo.Contains(OwnPayType) Then

                                        If PayInfo.Contains("PayPal-E-Mail") Then

                                            Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                            Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                            PayInfo = "PayPal-E-Mail=" + GetINISetting(E_Setting.PayPalEMail, "test@test.com") + " Reference/Note=" + ColWordsString

                                        ElseIf PayInfo.Contains("PayPal-Order") Then

                                            Dim APIOK As String = CheckPayPalAPI()

                                            If APIOK = "True" Then
                                                Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal
                                                PPAPI_Autoinfo.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                                                PPAPI_Autoinfo.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                                                Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", BuyAmount, XAmount, T_DEXContract.CurrentXItem)
                                                Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
                                                PayInfo = "PayPal-Order=" + PPOrderID
                                            End If

                                        End If

                                        Dim T_MsgStr As String = "AT=" + T_DEXContract.Address + " TX=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(XAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo

                                        Dim TXr As String = SendBillingInfos(T_DEXContract.CurrentInitiatorID, T_MsgStr, True, True)

                                        If TXr.Contains(Application.ProductName + "-error") Then
                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.ErrorLog2File(TXr)
                                            End If

                                        ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                out.WarningLog2File(TXr)
                                            End If
                                        Else

                                            MBoxMsg += "Payment instructions sended" + vbCrLf
                                            MBoxMsg += "TX: " + TXr + vbCrLf + vbCrLf
                                            MBoxMsg += "Recipient: " + T_DEXContract.CurrentInitiatorAddress + vbCrLf  ' BLSAT_TX.SenderRS
                                            MBoxMsg += "AT: " + T_DEXContract.Address + vbCrLf
                                            MBoxMsg += "Order-Transaction: " + T_DEXContract.CurrentCreationTransaction.ToString + vbCrLf
                                            MBoxMsg += "payment request: " + Dbl2LVStr(XAmount, Decimals) + " " + T_DEXContract.CurrentXItem + vbCrLf
                                            MBoxMsg += PayInfo + vbCrLf + vbCrLf
                                            MBoxMsg += "please wait for requested payment from buyer"
                                        End If

                                    End If

                                    ClsMsgs.MBox(MBoxMsg, "Transaction(s) created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                                End If

                            End If

                        Else
                            'TODO: Show PINForm

                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, Sum, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(1a): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(1b): -> " + vbCrLf + TX)
                                        End If

                                    Else

                                        MBoxMsg = "BuyOrder Accepted" + vbCrLf
                                        MBoxMsg += "TX: " + TX + vbCrLf
                                        MBoxMsg += "Recipient: " + T_DEXContract.Address + vbCrLf + vbCrLf

                                        'Public Enum E_PayType
                                        '    Bankaccount = 0
                                        '    PayPal_E_Mail = 1
                                        '    PayPal_Order = 2
                                        '    Self_Pickup = 3
                                        '    Other = 4
                                        'End Enum

                                        'TODO: OwnPayType 

                                        Dim OwnPayType As String = GetINISetting(E_Setting.PaymentType, "Self Pickup")

                                        If PayInfo.Contains(OwnPayType) Then

                                            If PayInfo.Contains("PayPal-E-Mail") Then

                                                Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                PayInfo = "PayPal-E-Mail=" + GetINISetting(E_Setting.PayPalEMail, "test@test.com") + " Reference/Note=" + ColWordsString

                                            ElseIf PayInfo.Contains("PayPal-Order") Then

                                                Dim APIOK As String = CheckPayPalAPI()

                                                If APIOK = "True" Then
                                                    Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal
                                                    PPAPI_Autoinfo.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                                                    PPAPI_Autoinfo.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                                                    Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", BuyAmount, XAmount, T_DEXContract.CurrentXItem)
                                                    Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
                                                    PayInfo = "PayPal-Order=" + PPOrderID
                                                End If

                                            End If

                                            Dim T_MsgStr As String = "AT=" + T_DEXContract.Address + " TX=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(XAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo

                                            Dim TXr As String = SendBillingInfos(T_DEXContract.CurrentInitiatorID, T_MsgStr, True, True)

                                            If TXr.Contains(Application.ProductName + "-error") Then
                                                If GetINISetting(E_Setting.InfoOut, False) Then
                                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                    out.ErrorLog2File(TXr)
                                                End If
                                            ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                                                If GetINISetting(E_Setting.InfoOut, False) Then
                                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                    out.WarningLog2File(TXr)
                                                End If
                                            Else
                                                MBoxMsg += "Payment instructions sended" + vbCrLf
                                                MBoxMsg += "TX: " + TXr + vbCrLf + vbCrLf
                                                MBoxMsg += "Recipient: " + T_DEXContract.CurrentInitiatorAddress + vbCrLf
                                                MBoxMsg += "AT: " + T_DEXContract.Address + vbCrLf
                                                MBoxMsg += "Order-Transaction: " + T_DEXContract.CurrentCreationTransaction.ToString + vbCrLf
                                                MBoxMsg += "payment request: " + Dbl2LVStr(XAmount, Decimals) + " " + T_DEXContract.CurrentXItem + vbCrLf
                                                MBoxMsg += PayInfo + vbCrLf + vbCrLf
                                                MBoxMsg += "please wait for requested payment from buyer"
                                            End If

                                        End If

                                        ClsMsgs.MBox(MBoxMsg, "Transaction(s) created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("BuyOrder acception canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If


                    End If

                Else
                    'not enough balance
                    ClsMsgs.MBox("not enough balance", "Error",,, ClsMsgs.Status.Erro)
                End If

            Else

                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the BuyOrder?", "Cancel BuyOrder?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                    Dim MasterKeys As List(Of String) = GetPassPhrase()

                    If MasterKeys.Count > 0 Then

                        Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                        If Response.Contains(Application.ProductName + "-error") Then

                            If GetINISetting(E_Setting.InfoOut, False) Then
                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(cancel1): -> " + vbCrLf + Response)
                            End If

                        Else

                            Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                            Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                            Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If TX.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(cancel2): -> " + vbCrLf + TX)
                                End If

                            Else
                                ClsMsgs.MBox("BuyOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                            End If

                        End If

                    Else
                        'TODO: Show PINFrom

                        Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                        PinForm.ShowDialog()

                        If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                            Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, Sum, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(cancel1a): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtSell_Click(cancel1b): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    ClsMsgs.MBox("BuyOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                                End If

                            End If

                        Else
                            ClsMsgs.MBox("BuyOrder cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                        End If

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

        SplitContainer16.Panel2Collapsed = True

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            SplitContainer16.Panel2Collapsed = False

            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)

            Dim Seller As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVi))
            Dim Buyer As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVi))
            Dim Status As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Status", LVi))

            'Dim Order As S_Order = LVi.Tag


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

                    BtExecuteOrder.Text = "fulfill AT"
                    BtExecuteOrder.Visible = True

                    Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem1.Text = "fulfill AT"
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

                    BtExecuteOrder.Text = "fulfill AT"

                    If Buyer.Trim = "" Or Seller.Trim = "" Then
                        BtExecuteOrder.Visible = False
                    Else
                        BtExecuteOrder.Visible = True
                        BtReCreatePayPalOrder.Visible = True
                        BtPayOrder.Visible = False

                        Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                        LVCMItem1.Text = "fulfill AT"
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
                    BtExecuteOrder.Text = "fulfill AT"
                    LVCMItem1.Text = "fulfill AT"
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

            Dim FirstTransaction As String = Convert.ToString(GetLVColNameFromSubItem(LVMyClosedOrders, "First Transaction", LVi))
            Dim LastTransaction As String = Convert.ToString(GetLVColNameFromSubItem(LVMyClosedOrders, "Last Transaction", LVi))
            Dim Seller As String = Convert.ToString(GetLVColNameFromSubItem(LVMyClosedOrders, "Seller", LVi))
            Dim Buyer As String = Convert.ToString(GetLVColNameFromSubItem(LVMyClosedOrders, "Buyer", LVi))


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
            If sender.GetType.Name = GetType(ToolStripMenuItem).Name Then

                Dim T_TSMI As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

                If Not IsNothing(T_TSMI.Tag) Then
                    Clipboard.SetText(T_TSMI.Tag.ToString)

                    'Dim Out As out = New out(Application.StartupPath)

                    'For Each LVI As ListViewItem In LVMyClosedOrders.Items
                    '    Out.ToFile(LVI.SubItems(0).Text + vbCrLf)
                    '    Out.ToFile(LVI.SubItems(1).Text + vbCrLf)
                    'Next

                Else

                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub BtExecuteOrder_Click(sender As Object, e As EventArgs) Handles BtExecuteOrder.Click

        Dim OldTxt As String = BtExecuteOrder.Text

        BtExecuteOrder.Text = "Wait..."
        BtExecuteOrder.Enabled = False

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtExecuteOrder.Text = OldTxt
                BtExecuteOrder.Enabled = True
                Exit Sub
            End If

            'If CheckForUTX(, T_DEXContract.ATRS) Or CheckATforTX(T_DEXContract.ATID) Then
            '    ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
            '    BtExecuteOrder.Text = OldTxt
            '    BtExecuteOrder.Enabled = True
            '    Exit Sub
            'End If


            If T_DEXContract.IsSellOrder Then ' .Type = "SellOrder" Then

                If T_DEXContract.CurrentBuyerAddress.Trim = "" Then
                    'cancel AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the AT?", "cancel AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                        If MasterKeys.Count > 0 Then

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(1): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(2): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'TODO: show PINForm

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(3): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(4): -> " + vbCrLf + TX)
                                        End If

                                    Else
                                        ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If

                    End If

                Else
                    'execute AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to fulfill the AT?", "fulfill AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                        If MasterKeys.Count > 0 Then

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(3): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(5): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else
                            'TODO: show PINForm

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(6): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(7): -> " + vbCrLf + TX)
                                        End If

                                    Else
                                        ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order finishing aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If


                    End If

                End If

            Else 'BuyOrder

                If T_DEXContract.CurrentSellerAddress.Trim = "" Then '.SellerRS.Trim = "" Then

                    'cancel AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the AT?", "cancel AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()


                        If MasterKeys.Count > 0 Then
                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(8): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(9): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'TODO: show PINForm

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceAcceptOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(10): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(11): -> " + vbCrLf + TX)
                                        End If

                                    Else
                                        ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If

                    End If

                Else
                    'execute AT
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to fulfill the AT?", "fulfill AT?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()

                        If MasterKeys.Count > 0 Then

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                            If Response.Contains(Application.ProductName + "-error") Then

                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(12): -> " + vbCrLf + Response)
                                End If

                            Else

                                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If TX.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(13): -> " + vbCrLf + TX)
                                    End If

                                Else
                                    ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'TODO: show PINForm

                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Dim Response As String = SignumAPI.SendMessage2BLSAT(PinForm.PublicKey, T_DEXContract.ID, 1.0, New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                                If Response.Contains(Application.ProductName + "-error") Then

                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(14): -> " + vbCrLf + Response)
                                    End If

                                Else

                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If TX.Contains(Application.ProductName + "-error") Then

                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                            out.ErrorLog2File(Application.ProductName + "-error in BtExecuteOrder_Click(15): -> " + vbCrLf + TX)
                                        End If

                                    Else
                                        ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order finishing aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

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

            Dim Status As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Status", LVi))

            Process.Start(Status)

        End If

    End Sub
    Private Sub BtReCreatePayPalOrder_Click(sender As Object, e As EventArgs) Handles BtReCreatePayPalOrder.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CheckForUTX Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                Exit Sub
            End If


            Dim PayInfo As String = GetPaymentInfoFromOrderSettings(T_DEXContract.CurrentCreationTransaction, T_DEXContract.CurrentBuySellAmount, T_DEXContract.CurrentXAmount, CurrentMarket)

            If Not PayInfo.Trim = "" Then

                If PayInfo.Contains("PayPal-E-Mail=") Then
                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                    PayInfo += " Reference/Note=" + ColWordsString
                End If

                Dim T_MsgStr As String = "AT=" + T_DEXContract.Address + " TX=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo
                Dim TXr As String = SendBillingInfos(T_DEXContract.CurrentBuyerID, T_MsgStr, True, True)

                If TXr.Contains(Application.ProductName + "-error") Then
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                        out.ErrorLog2File(TXr)
                    End If
                ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                        out.WarningLog2File(TXr)
                    End If
                Else
                    ClsMsgs.MBox("New PayPal Order sended as encrypted Message", "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

            End If

        End If

    End Sub
    Private Sub BtSendMsg_Click(sender As Object, e As EventArgs) Handles BtSendMsg.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, ClsDEXContract)

            Dim T_Infotext As String = "Infotext=" + TBManuMsg.Text.Replace(",", ";").Replace(":", ";").Replace("""", ";")
            Dim T_MsgStr As String = "AT=" + T_DEXContract.Address + " TX=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " " + T_Infotext

            Dim Recipient As String = T_DEXContract.CurrentBuyerAddress

            If T_DEXContract.CurrentBuyerAddress = TBSNOAddress.Text Then
                Recipient = T_DEXContract.CurrentSellerAddress
            End If

            Dim TXr As String = SendBillingInfos(Recipient, T_MsgStr, True, ChBxEncMsg.Checked)

            If TXr.Contains(Application.ProductName + "-error") Then
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                    out.ErrorLog2File(TXr)
                End If
            ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                    out.WarningLog2File(TXr)
                End If
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
            Dim T_FRM As Form = DirectCast(SCSettings.Panel2.Controls(0), Form)
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
            Dim T_Frm As Form = DirectCast(SCSettings.Panel2.Controls(0), Form)

            If T_Frm.Name = Template Then
                Return T_Frm
            End If

        End If

        Return Nothing

    End Function

#End Region

#End Region

#Region "Methods/Functions"

    Function Checknodes() As Integer

        Dim NodeCNT As Integer = 0

        For Each Node As String In NodeList

            Dim SignaAPI As ClsSignumAPI = New ClsSignumAPI(Node)

            Dim Blockheight As Integer = SignaAPI.GetCurrentBlock()
            If Blockheight > 0 Then
                NodeCNT += 1
            End If

        Next

        Return NodeCNT

    End Function

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

            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim ATList As List(Of String) = SignumAPI.GetATIds()
            Dim CSVATList As List(Of List(Of String)) = GetATsFromCSV()


            Dim Nu_ATList As List(Of S_AT) = New List(Of S_AT) ' New List(Of String)

            For Each AT As String In ATList

                Dim Found As Boolean = False
                For Each CSVAT As List(Of String) In CSVATList

                    If AT.Trim = CSVAT(0).Trim Then
                        Found = True
                        Exit For
                    End If
                Next

                If Not Found Then
                    Dim T_AT As S_AT = New S_AT With {.ID = Convert.ToUInt64(AT)}
                    Nu_ATList.Add(T_AT)
                End If

            Next


            For Each CSVAT As List(Of String) In CSVATList

                If CSVAT(1).Trim = "True" Then
                    Dim T_AT As S_AT = New S_AT With {.ID = Convert.ToUInt64(CSVAT(0).Trim), .IsDEX_AT = Convert.ToBoolean(CSVAT(1).Trim), .HistoryOrders = CSVAT(2)}
                    Nu_ATList.Add(T_AT)
                End If

            Next


            APIRequestList.Clear()

            For Each AT As S_AT In Nu_ATList

                Dim TestMulti As S_APIRequest = New S_APIRequest
                Dim TempContract As ClsDEXContract = Nothing
                Dim Found As Boolean = False

                For Each T_DEXContract As ClsDEXContract In DEXContractList

                    If T_DEXContract.ID = AT.ID Then
                        TempContract = T_DEXContract
                        Found = True
                        Exit For
                    End If

                Next

                If Not Found Then
                    TestMulti.Command = "GetDetails(" + AT.ID.ToString.Trim + ")"
                    TestMulti.Status = "Wait..."
                    TestMulti.CommandAttachment = AT.HistoryOrders
                Else
                    TestMulti.Command = "RefreshContract(" + AT.ID.ToString.Trim + ")"
                    TestMulti.Status = "Wait..."
                    TestMulti.CommandAttachment = TempContract
                End If

                APIRequestList.Add(TestMulti)

            Next

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in GetAndCheckATs(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function

    Function ResetLVColumns() As Boolean

        LVSellorders.Columns.Clear()
        LVSellorders.Columns.Add("Price (" + CurrentMarket + ")")
        LVSellorders.Columns.Add("Amount (" + CurrentMarket + ")")
        LVSellorders.Columns.Add("Total (Signa)")
        LVSellorders.Columns.Add("Collateral (Signa)")
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
        With LVBuyorders.Columns.Add("Total (Signa)")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Collateral (Signa)")
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
        LVMyOpenOrders.Columns.Add("Collateral")
        LVMyOpenOrders.Columns.Add("Price")
        LVMyOpenOrders.Columns.Add("Status")



        LVMyClosedOrders.Columns.Clear()
        LVMyClosedOrders.Columns.Add("First Transaction")
        LVMyClosedOrders.Columns.Add("Last Transaction")
        LVMyClosedOrders.Columns.Add("Date")
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

    Function SetInLVs() As Boolean

        'Try

#Region "set LVs"

        BuyOrderLVEList.Clear()
        SellOrderLVEList.Clear()

        OpenChannelLVIList.Clear()
        'BuyOrderLVIList.Clear()
        'SellOrderLVIList.Clear()
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

        LVMyOpenOrders.Visible = False
        LVMyClosedOrders.Visible = False

        SplitContainer16.Panel2Collapsed = True 'hide interaction-buttons

        LVOpenChannels.Items.Clear()
        For Each OpenChannel As ListViewItem In OpenChannelLVIList
            MultiInvoker(LVOpenChannels, "Items", New List(Of Object)({"Add", OpenChannel}))
        Next

        Wait = SetFitlteredPublicOrdersInLVs()

        LVMyOpenOrders.Items.Clear()
        For Each MyOpenOrder As ListViewItem In MyOpenOrderLVIList
            MultiInvoker(LVMyOpenOrders, "Items", New List(Of Object)({"Add", MyOpenOrder}))
        Next

        MyClosedOrderLVIList = MyClosedOrderLVIList.OrderByDescending(Function(T_Order As S_DEXOrder) T_Order.Order.StartTimestamp).ToList

        LVMyClosedOrders.Items.Clear()
        For i As Integer = 0 To MyClosedOrderLVIList.Count - 1

            If i > 100 Then
                Exit For
            End If

            Dim HistoryOrder As S_DEXOrder = MyClosedOrderLVIList(i)

            If HistoryOrder.Order.Status <> ClsDEXContract.E_Status.CLOSED And HistoryOrder.Order.Status <> ClsDEXContract.E_Status.CANCELED Then
                Continue For
            End If

            Dim MyClosedOrder As ListViewItem = New ListViewItem

            MyClosedOrder.Text = HistoryOrder.Order.CreationTransaction.ToString
            MyClosedOrder.SubItems.Add(HistoryOrder.Order.LastTransaction.ToString)

            MyClosedOrder.SubItems.Add(ClsSignumAPI.UnixToTime(HistoryOrder.Order.StartTimestamp.ToString).ToString) 'confirms
            MyClosedOrder.SubItems.Add(HistoryOrder.ContractAddress)  'at

            If HistoryOrder.Order.WasSellOrder Then
                MyClosedOrder.SubItems.Add("SellOrder") 'type
            Else
                MyClosedOrder.SubItems.Add("BuyOrder") 'type
            End If

            MyClosedOrder.SubItems.Add(HistoryOrder.Order.SellerRS) 'seller
            MyClosedOrder.SubItems.Add(HistoryOrder.Order.BuyerRS)  'buyer
            MyClosedOrder.SubItems.Add(HistoryOrder.Order.XItem) 'xitem
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.XAmount, Decimals)) 'xamount
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.Amount))  'quantity
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.Price, Decimals)) 'price
            MyClosedOrder.SubItems.Add(HistoryOrder.Order.Status.ToString) 'status
            'MyClosedOrder.SubItems.Add(Order.Attachment.ToString) 'conditions
            MyClosedOrder.Tag = HistoryOrder.Contract

            MultiInvoker(LVMyClosedOrders, "Items", New List(Of Object)({"Add", MyClosedOrder}))

        Next

        Dim MasterKeys As List(Of String) = GetPassPhrase()
        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
        If MasterKeys.Count > 0 Then
            For Each BroadcastMsg As String In BroadcastMsgs
                If Not IsNothing(DEXNET) Then
                    DEXNET.BroadcastMessage(BroadcastMsg, MasterKeys(1),, MasterKeys(0))
                End If
            Next
        End If



        BroadcastMsgs.Clear()

        LVMyOpenOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0)
        LVMyClosedOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Descending, 2)

        LVOpenChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        LVMyOpenOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        LVMyClosedOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

#End Region
        'setLVs

        Dim T_BlockFeeThread As Threading.Thread = New Threading.Thread(AddressOf BlockFeeThread)

        StatusBlockLabel.Text = "loading Blockheight..."
        'StatusFeeLabel.Text = "loading Current Fee..."
        StatusStrip1.Refresh()

        T_BlockFeeThread.Start(DirectCast(PrimaryNode, Object))

        'BLSAT_TX_List.Clear()
        'BLSAT_TX_List.AddRange(T_BLSAT_TX_List.ToArray)
        'BLSAT_TX_List = BLSAT_TX_List.OrderBy(Function(BLSAT_TX As S_BLSAT_TX) BLSAT_TX.Timestamp).ToList

        StatusBar.Value = 0
        StatusBar.Maximum = 100
        StatusBar.Visible = False
        StatusLabel.Text = ""

        LVOpenChannels.Visible = True

        LVMyOpenOrders.Visible = True
        LVMyClosedOrders.Visible = True

        LVMyOpenOrders.Size = New Size(0, 0)
        LVMyOpenOrders.Size = New Size(0, 0)

        'SplitContainer2.Panel1.Visible = True

        'Catch ex As Exception
        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        '        Out.ErrorLog2File(Application.ProductName + "-error in SetInLVs(): -> " + ex.Message)
        '    End If

        'End Try

        Return True

    End Function


    Function SetFitlteredPublicOrdersInLVs() As Boolean

        LVSellorders.Visible = False
        LVBuyorders.Visible = False

        LVBuyorders.Items.Clear()

        BuyOrderLVEList = BuyOrderLVEList.OrderBy(Function(T_PublicOrder As S_PublicOrdersListViewEntry) T_PublicOrder.Price).ToList

        For i As Integer = 0 To BuyOrderLVEList.Count - 1

            Dim BuyOrder As S_PublicOrdersListViewEntry = BuyOrderLVEList(i)

            If Not BuyOrder.Backcolor = Color.Magenta Then

                If i + 1 > GetINISetting(E_Setting.ShowMaxBuyOrders, 10) Then
                    Exit For
                End If

                If ChBxBuyFilterShowAutoinfo.Checked Then
                    If BuyOrder.AutoInfo.ToUpper.Trim <> ChBxBuyFilterShowAutoinfo.Checked.ToString.ToUpper.Trim And BuyOrder.AutoInfo.ToUpper.Trim <> "" Then
                        Continue For
                    End If
                End If

                If ChBxBuyFilterShowAutofinish.Checked Then
                    If BuyOrder.AutoFinish.ToUpper.Trim <> ChBxBuyFilterShowAutofinish.Checked.ToString.ToUpper.Trim And BuyOrder.AutoFinish.ToUpper.Trim <> "" Then
                        Continue For
                    End If
                End If

                If ChBxBuyFilterShowPayable.Checked Then
                    If Convert.ToDouble(BuyOrder.Collateral) + Convert.ToDouble(BuyOrder.Amount) > Convert.ToDouble(TBSNOBalance.Text) Then
                        Continue For
                    End If
                End If


                Dim ValidBuyMethods As String = LoadMethodFilterFromINI(E_Setting.BuyFilterMethods)
                If Not ValidBuyMethods.Contains(BuyOrder.Method) Or ValidBuyMethods.Trim = "" Then
                    Continue For
                End If

            End If


            Dim T_LVI As ListViewItem = New ListViewItem

            T_LVI.Text = BuyOrder.Price 'price
            T_LVI.SubItems.Add(BuyOrder.Amount) 'amount
            T_LVI.SubItems.Add(BuyOrder.Total) 'total
            T_LVI.SubItems.Add(BuyOrder.Collateral) 'collateral
            T_LVI.SubItems.Add(BuyOrder.Method) 'payment method
            T_LVI.SubItems.Add(BuyOrder.AutoInfo) 'autosend infotext
            T_LVI.SubItems.Add(BuyOrder.AutoFinish) 'autocomplete at
            T_LVI.SubItems.Add(BuyOrder.Seller_Buyer) 'buyer
            T_LVI.SubItems.Add(BuyOrder.AT) 'at

            T_LVI.BackColor = BuyOrder.Backcolor
            T_LVI.Tag = BuyOrder.Tag

            MultiInvoker(LVBuyorders, "Items", New List(Of Object)({"Add", T_LVI}))

        Next

        'For Each BuyOrder As ListViewItem In BuyOrderLVIList
        '    MultiInvoker(LVBuyorders, "Items", {"Add", BuyOrder})
        'Next


        LVSellorders.Items.Clear()

        SellOrderLVEList = SellOrderLVEList.OrderBy(Function(T_PublicOrder As S_PublicOrdersListViewEntry) T_PublicOrder.Price).ToList
        SellOrderLVEList.Reverse()

        For i As Integer = 0 To SellOrderLVEList.Count - 1

            Dim SellOrder As S_PublicOrdersListViewEntry = SellOrderLVEList(i)

            If Not SellOrder.Backcolor = Color.Magenta Then

                If i + 1 > GetINISetting(E_Setting.ShowMaxSellOrders, 10) Then
                    Exit For
                End If

                If ChBxSellFilterShowAutoinfo.Checked Then
                    If SellOrder.AutoInfo.ToUpper.Trim <> ChBxSellFilterShowAutoinfo.Checked.ToString.ToUpper.Trim And SellOrder.AutoInfo.ToUpper.Trim <> "" Then
                        Continue For
                    End If
                End If

                If ChBxSellFilterShowAutofinish.Checked Then
                    If SellOrder.AutoFinish.ToUpper.Trim <> ChBxSellFilterShowAutofinish.Checked.ToString.ToUpper.Trim And SellOrder.AutoFinish.ToUpper.Trim <> "" Then
                        Continue For
                    End If
                End If

                If ChBxSellFilterShowPayable.Checked Then
                    If Convert.ToDouble(SellOrder.Collateral) > Convert.ToDouble(TBSNOBalance.Text) Then
                        Continue For
                    End If
                End If

                Dim ValidSellMethods As String = LoadMethodFilterFromINI(E_Setting.SellFilterMethods)
                If Not ValidSellMethods.Contains(SellOrder.Method) Or ValidSellMethods.Trim = "" Then
                    Continue For
                End If

            End If


            Dim T_LVI As ListViewItem = New ListViewItem

            T_LVI.Text = SellOrder.Price 'price
            T_LVI.SubItems.Add(SellOrder.Amount) 'amount
            T_LVI.SubItems.Add(SellOrder.Total) 'total
            T_LVI.SubItems.Add(SellOrder.Collateral) 'collateral
            T_LVI.SubItems.Add(SellOrder.Method) 'payment method
            T_LVI.SubItems.Add(SellOrder.AutoInfo) 'autosend infotext
            T_LVI.SubItems.Add(SellOrder.AutoFinish) 'autocomplete at
            T_LVI.SubItems.Add(SellOrder.Seller_Buyer) 'buyer
            T_LVI.SubItems.Add(SellOrder.AT) 'at

            T_LVI.BackColor = SellOrder.Backcolor
            T_LVI.Tag = SellOrder.Tag

            MultiInvoker(LVSellorders, "Items", New List(Of Object)({"Add", T_LVI}))

        Next

        'For Each OpeSellOrdernChannel As ListViewItem In SellOrderLVIList
        '   MultiInvoker(LVSellorders, "Items", {"Add", OpeSellOrdernChannel})
        'Next


        LVSellorders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0)
        LVSellorders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        LVBuyorders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Descending, 0)
        LVBuyorders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        LVSellorders.Visible = True
        LVBuyorders.Visible = True

        LVSellorders.Size = New Size(0, 0)
        LVBuyorders.Size = New Size(0, 0)

        Return True

    End Function


    Function ProcessingATs() As Boolean

        Try

            'OrderList.Clear()
            MTSAT2LVList.Clear()

            OrderSettingsBuffer.Clear()
            OrderSettingsBuffer = GetOrderSettings()

            For i As Integer = 0 To DEXContractList.Count - 1

                Dim T_DEXContract As ClsDEXContract = DEXContractList(i)

                'OrderList.AddRange(T_DEXContract.ContractOrderHistoryList)

                Dim T_SetLVThreadStruc As S_MultiThreadSetAT2LV = New S_MultiThreadSetAT2LV

                T_SetLVThreadStruc.SubThread = New Threading.Thread(AddressOf MultiThreadSetAT2LV)

                Dim Index As Integer = MTSAT2LVList.Count
                T_SetLVThreadStruc.DEXContract = T_DEXContract
                T_SetLVThreadStruc.Market = CurrentMarket

                Application.DoEvents()

                MTSAT2LVList.Add(T_SetLVThreadStruc)

                T_SetLVThreadStruc.SubThread.Start(New List(Of Object)({Index}))

            Next


            Dim exiter As Boolean = False
            While Not exiter
                exiter = True

                Application.DoEvents()

                For Each S_TH As S_MultiThreadSetAT2LV In MTSAT2LVList

                    Application.DoEvents()

                    If S_TH.SubThread.IsAlive Then

                        MultiInvoker(StatusLabel, "Text", "Processing " + S_TH.DEXContract.Address)
                        Application.DoEvents()

                        exiter = False
                        Exit For
                    End If

                Next

            End While

            SaveOrderSettingsToCSV(OrderSettingsBuffer)

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in ProcessingATs(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function


    Structure S_MultiThreadSetAT2LV
        Dim SubThread As Threading.Thread
        Dim DEXContract As ClsDEXContract
        'Dim ATListIndex As Integer
        Dim Market As String
    End Structure

    Property MTSAT2LVList As List(Of S_MultiThreadSetAT2LV) = New List(Of S_MultiThreadSetAT2LV)


    Structure S_PublicOrdersListViewEntry
        Dim Price As String
        Dim Amount As String
        Dim Total As String
        Dim Collateral As String
        Dim Backcolor As Color
        Dim Method As String
        Dim AutoInfo As String
        Dim AutoFinish As String
        Dim Seller_Buyer As String
        Dim AT As String
        Dim Tag As Object

        Sub New(Optional ByVal P_Price As String = "", Optional ByVal P_Amount As String = "", Optional ByVal P_Total As String = "", Optional ByVal P_Collateral As String = "", Optional ByVal P_Method As String = "", Optional ByVal P_AutoInfo As String = "", Optional ByVal P_AutoFinish As String = "", Optional ByVal P_Seller_Buyer As String = "", Optional ByVal P_AT As String = "")

            Price = P_Price
            Amount = P_Amount
            Total = P_Total
            Collateral = P_Collateral
            Backcolor = SystemColors.Control
            Method = P_Method
            AutoInfo = P_AutoInfo
            AutoFinish = P_AutoFinish
            Seller_Buyer = P_Seller_Buyer
            AT = P_AT
            Tag = Nothing

        End Sub

    End Structure

    Property SellOrderLVEList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)
    Property BuyOrderLVEList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)

    Property OpenChannelLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    'Property BuyOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    'Property SellOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property MyOpenOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Property MyClosedOrderLVIList As List(Of S_DEXOrder) = New List(Of S_DEXOrder)

    Public Structure S_DEXOrder
        Dim ContractAddress As String
        Dim Order As ClsDEXContract.S_Order
        Dim Contract As ClsDEXContract
    End Structure

    Property BroadcastMsgs As List(Of String) = New List(Of String)

    Sub MultiThreadSetAT2LV(ByVal T_Input As Object)

        Try

#Region "Main"

            Dim Input As List(Of Object) = New List(Of Object)

            If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_Input, List(Of Object))
            Else
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(DirectCast): -> " + T_Input.GetType.Name)
                End If
            End If

            Dim Index As Integer = DirectCast(Input(0), Integer)
            Dim S_MTSetAT2LV As S_MultiThreadSetAT2LV = MTSAT2LVList(Index)

            Dim Market As String = S_MTSetAT2LV.Market
            Dim T_DEXContract As ClsDEXContract = S_MTSetAT2LV.DEXContract


            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim T_LVI As ListViewItem = New ListViewItem

            If T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ Then '.AT_TXList.Count = 0 Then

                If T_DEXContract.CreatorAddress = TBSNOAddress.Text Then

                    T_LVI.Text = T_DEXContract.Address 'SmartContract
                    T_LVI.SubItems.Add("Reserved for you") 'Status
                    T_LVI.Tag = T_DEXContract

                    OpenChannelLVIList.Add(T_LVI)

                End If

            Else 'Status <> NEW_

                Dim ATChannelOpen As Boolean = True


#Region "Set To LVs"

#Region "Confirmations"
                Dim Confirms As String = T_DEXContract.CurrentConfirmations.ToString
#End Region

#Region "XItem/Payment Method"

                Dim XItem As String = T_DEXContract.CurrentXItem
                Dim PayMet As String = "Unknown"

                'If XItem.Contains("-") Then
                '    PayMet = XItem.Split("-")(1)
                '    XItem = XItem.Split("-")(0)
                'End If

#End Region

                If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then

                    ATChannelOpen = False

                    Try

                        If T_DEXContract.CurrentXItem.Contains(Market) Then

#Region "Market - GUI"

                            Dim Autosendinfotext As String = "False"
                            Dim AutocompleteAT As String = "False"

                            If Not T_DEXContract.CheckForUTX And Not T_DEXContract.CheckForTX Then ' CheckForUTX(, Order.ATRS) And Not CheckATforTX(T_DEXContract.ATID) Then

                                'TODO: Debug Double OPEN And RESERVED Orders by Injected Recipients

                                If T_DEXContract.IsSellOrder Then ' Order.Type = "SellOrder" Then

                                    T_LVI = New ListViewItem
                                    Dim T_LVE As S_PublicOrdersListViewEntry = New S_PublicOrdersListViewEntry

                                    'T_LVI.Text = Dbl2LVStr(Order.Price, Decimals) 'price
                                    T_LVE.Price = Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals) 'price

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                    T_LVE.Amount = Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals)

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                    T_LVE.Total = Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral
                                    T_LVE.Collateral = Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) '  Order.Collateral)

                                    Dim T_SellerRS As String = T_DEXContract.CurrentSellerAddress ' Order.SellerRS
                                    If T_SellerRS = TBSNOAddress.Text Then

                                        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)

                                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status) ' Order.ATID, Order.FirstTransaction, Order.Type, Order.Status)

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
                                        'T_LVI.BackColor = Color.Magenta
                                        T_LVE.Backcolor = Color.Magenta

                                    Else

#Region "SellOrder-Info from DEXNET"
                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not IsNothing(DEXNET) Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If


                                        Dim RelKeyFounded As Boolean = False

                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey = "<ATID>" + T_DEXContract.ID.ToString + "</ATID>" Then
                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If Not T_SellerRS = TBSNOAddress.Text Then

                                                        If T_DEXContract.CurrentSellerID = GetAccountID(PublicKey) Then

                                                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                            PayMet = PayMethod

                                                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                            Autosendinfotext = T_Autosendinfotext

                                                            Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                                                            AutocompleteAT = T_AutocompleteAT

                                                        End If

                                                    End If


                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not IsNothing(DEXNET) Then
                                                DEXNET.AddRelevantKey("<ATID>" + T_DEXContract.ID.ToString + "</ATID>")
                                            End If
                                        End If


#End Region

                                    End If

                                    'T_LVI.SubItems.Add(PayMet) 'payment method
                                    T_LVE.Method = PayMet

                                    'T_LVI.SubItems.Add(Autosendinfotext) 'autosend infotext
                                    T_LVE.AutoInfo = Autosendinfotext

                                    'T_LVI.SubItems.Add(AutocompleteAT) 'autocomplete at
                                    T_LVE.AutoFinish = AutocompleteAT

                                    'T_LVI.SubItems.Add(T_SellerRS) 'seller
                                    T_LVE.Seller_Buyer = T_SellerRS

                                    'T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                    T_LVE.AT = T_DEXContract.Address

                                    'T_LVI.Tag = PFPAT
                                    T_LVE.Tag = T_DEXContract

                                    'SellOrderLVIList.Add(T_LVI)
                                    SellOrderLVEList.Add(T_LVE)


                                Else 'BuyOrder


                                    'T_LVI = New ListViewItem
                                    Dim T_LVE As S_PublicOrdersListViewEntry = New S_PublicOrdersListViewEntry

                                    'T_LVI.Text = Dbl2LVStr(Order.Price, Decimals) 'price
                                    T_LVE.Price = Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals) 'price

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                    T_LVE.Amount = Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) 'amount

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                    T_LVE.Total = Dbl2LVStr(T_DEXContract.CurrentXAmount) 'total

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral
                                    T_LVE.Collateral = Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) 'collateral


                                    Dim T_BuyerRS As String = T_DEXContract.CurrentBuyerAddress ' Order.BuyerRS
                                    If T_BuyerRS = TBSNOAddress.Text Then

                                        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)
                                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status) ' Order.ATID, Order.FirstTransaction, Order.Type, Order.Status)

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
                                        'T_LVI.BackColor = Color.Magenta
                                        T_LVE.Backcolor = Color.Magenta
                                    Else

#Region "BuyOrder-Info from DEXNET"
                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not IsNothing(DEXNET) Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If

                                        Dim RelKeyFounded As Boolean = False
                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey = "<ATID>" + T_DEXContract.ID.ToString + "</ATID>" Then
                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If Not T_BuyerRS = TBSNOAddress.Text Then

                                                        If T_DEXContract.CurrentBuyerID = GetAccountID(PublicKey) Then
                                                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                            PayMet = PayMethod

                                                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                            Autosendinfotext = T_Autosendinfotext

                                                            Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                                                            AutocompleteAT = T_AutocompleteAT

                                                        End If

                                                    End If

                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not IsNothing(DEXNET) Then
                                                DEXNET.AddRelevantKey("<ATID>" + T_DEXContract.ID.ToString + "</ATID>")
                                            End If
                                        End If
#End Region

                                    End If


                                    'T_LVI.SubItems.Add(PayMet) 'payment method
                                    T_LVE.Method = PayMet

                                    'T_LVI.SubItems.Add(Autosendinfotext) 'autosend infotext
                                    T_LVE.AutoInfo = Autosendinfotext

                                    'T_LVI.SubItems.Add(AutocompleteAT) 'autocomplete at
                                    T_LVE.AutoFinish = AutocompleteAT

                                    'T_LVI.SubItems.Add(T_BuyerRS) 'buyer
                                    T_LVE.Seller_Buyer = T_BuyerRS

                                    'T_LVI.SubItems.Add(PFPAT.ATRS) 'at
                                    T_LVE.AT = T_DEXContract.Address

                                    'T_LVI.Tag = PFPAT
                                    T_LVE.Tag = T_DEXContract

                                    'BuyOrderLVIList.Add(T_LVI)
                                    BuyOrderLVEList.Add(T_LVE)


                                End If

                            End If

#End Region

#Region "MyOPENOrders - GUI"

                            If TBSNOAddress.Text = T_DEXContract.CurrentSellerAddress Then ' Order.SellerRS Then

                                'Broadcast info over DEXNET
                                BroadcastMsgs.Add("<ATID>" + T_DEXContract.ID.ToString + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                T_LVI = New ListViewItem
                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(T_DEXContract.Address) 'at

                                If T_DEXContract.IsSellOrder Then
                                    T_LVI.SubItems.Add("SellOrder") ' Order.Type) 'type
                                Else
                                    T_LVI.SubItems.Add("BuyOrder") ' Order.Type) 'type
                                End If

                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                T_LVI.SubItems.Add("Me") 'seller
                                T_LVI.SubItems.Add(T_DEXContract.CurrentBuyerAddress) ' Order.BuyerRS) 'buyer
                                T_LVI.SubItems.Add(XItem) 'xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals)) ' Order.XAmount, Decimals)) 'xamount
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) ' Order.Quantity)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) ' Order.Collateral)) 'collateral
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals)) ' Order.Price, Decimals)) 'price
                                T_LVI.SubItems.Add("OPEN") 'status
                                T_LVI.Tag = T_DEXContract ' Order

                                MyOpenOrderLVIList.Add(T_LVI)

                            ElseIf TBSNOAddress.Text = T_DEXContract.CurrentBuyerAddress Then ' Order.BuyerRS Then

                                'Broadcast info over DEXNET
                                BroadcastMsgs.Add("<ATID>" + T_DEXContract.ID.ToString + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                T_LVI = New ListViewItem
                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(T_DEXContract.Address) 'at

                                If T_DEXContract.IsSellOrder Then
                                    T_LVI.SubItems.Add("SellOrder") 'type
                                Else
                                    T_LVI.SubItems.Add("BuyOrder") 'type
                                End If

                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                T_LVI.SubItems.Add(T_DEXContract.CurrentSellerAddress) '  Order.SellerRS) 'seller
                                T_LVI.SubItems.Add("Me") 'buyer
                                T_LVI.SubItems.Add(XItem) 'xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals)) '  Order.XAmount, Decimals)) 'xamount
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) ' Order.Quantity)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) ' Order.Collateral)) 'collateral
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals)) ' Order.Price, Decimals)) 'price
                                T_LVI.SubItems.Add("OPEN") 'status
                                T_LVI.Tag = T_DEXContract

                                MyOpenOrderLVIList.Add(T_LVI)

                            End If 'myaddress

#End Region

                        End If 'market(USD)

                    Catch ex As Exception

                        If GetINISetting(E_Setting.InfoOut, False) Then
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(OPEN): -> " + ex.Message)
                        End If

                    End Try

                ElseIf T_DEXContract.Status = ClsDEXContract.E_Status.RESERVED Then

                    ATChannelOpen = False

                    Try

#Region "MyRESERVEDOrders - GUI"

                        If TBSNOAddress.Text = T_DEXContract.CurrentSellerAddress Then

                            Try

                                'Save/Update my RESERVED SellOrder to cache2.dat (MyOrders Settings)
                                Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction)
                                Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status)

                                Dim Autosendinfotext As String = "False"
                                Dim AutocompleteAT As String = "False"

                                If T_DEXContract.IsSellOrder Then

                                    If T_OSList.Count = 0 Then
                                        T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                        T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                    Else
                                        T_OS = T_OSList(0)
                                        T_OS.SetPayType()
                                        T_OS.Status = T_DEXContract.Status.ToString
                                        PayMet = T_OS.PaytypeString
                                        Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                        AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                    End If

                                    OrderSettingsBuffer.Add(T_OS)
                                    'Broadcast info over DEXNET
                                    BroadcastMsgs.Add("<ATID>" + T_DEXContract.ID.ToString + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                                Else

#Region "SellOrder-Info from DEXNET"
                                    Try

                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not IsNothing(DEXNET) Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If

                                        Dim RelKeyFounded As Boolean = False

                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey = "<ATID>" + T_DEXContract.ID.ToString + "</ATID>" Then

                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If T_DEXContract.CurrentBuyerID = GetAccountID(PublicKey) Then

                                                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                        PayMet = PayMethod

                                                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                        Autosendinfotext = T_Autosendinfotext

                                                        Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                                                        AutocompleteAT = T_AutocompleteAT

                                                    End If


                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not IsNothing(DEXNET) Then
                                                DEXNET.AddRelevantKey("<ATID>" + T_DEXContract.ID.ToString + "</ATID>")
                                            End If
                                        End If

                                    Catch ex As Exception
                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED->SellOrder-Info from DEXNET): -> " + ex.Message)
                                        End If
                                    End Try
#End Region

                                End If

                                Dim AlreadySend As String = CheckBillingInfosAlreadySend(T_DEXContract)

#Region "Automentation"
                                If AlreadySend.Contains(Application.ProductName + "-error") Then
                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                        Out.ErrorLog2File(AlreadySend)
                                    End If

                                    AlreadySend = "ENCRYPTED"


                                ElseIf AlreadySend.Contains(Application.ProductName + "-warning") Then
                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                        out.WarningLog2File(AlreadySend)
                                    End If

                                    AlreadySend = "ENCRYPTED"

                                ElseIf AlreadySend.Trim = "" Then

                                    Dim SearchAttachmentHEX As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                                    If T_DEXContract.CheckForUTX Then
                                        AlreadySend = "PENDING"
                                    Else
                                        AlreadySend = "RESERVED"
                                    End If

                                    'If CheckForUTX(Order.SellerRS, Order.ATRS, SearchAttachmentHEX) Then
                                    '    AlreadySend = "PENDING"
                                    'ElseIf CheckForUTX(Order.BuyerRS, Order.ATRS) Then
                                    'Else
                                    '    AlreadySend = "RESERVED"
                                    'End If

                                    If T_OS.AutoSendInfotext Then 'autosend info to Buyer

#Region "AutoSend PaymentInfotext"
                                        Try

                                            If Not GetAutoinfoTXFromINI(T_DEXContract.CurrentCreationTransaction) Then ' Order.FirstTransaction) Then 'Check for autosend-info-TX in Settings.ini and skip if founded

                                                Dim PayInfo As String = GetPaymentInfoFromOrderSettings(T_DEXContract.CurrentCreationTransaction, T_DEXContract.CurrentBuySellAmount, T_DEXContract.CurrentXAmount, CurrentMarket) ' Order.FirstTransaction, Order.Quantity, Order.XAmount, CurrentMarket)

                                                If PayInfo.Contains("PayPal-E-Mail=") Then
                                                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                    PayInfo += " Reference/Note=" + ColWordsString
                                                End If

                                                Dim T_MsgStr As String = "AT=" + T_DEXContract.Address + " TX=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo
                                                Dim TXr As String = SendBillingInfos(T_DEXContract.CurrentBuyerID, T_MsgStr, False, True)

                                                If TXr.Contains(Application.ProductName + "-error") Then
                                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                        out.ErrorLog2File(TXr)
                                                    End If

                                                ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                        out.WarningLog2File(TXr)
                                                    End If
                                                Else

                                                    If SetAutoinfoTX2INI(T_DEXContract.CurrentCreationTransaction) Then 'Set autosend-info-TX in Settings.ini
                                                        'ok
                                                    End If
                                                End If

                                            End If

                                        Catch ex As Exception
                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                                Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED->Autosend PaymentInfotext): -> " + ex.Message)
                                            End If
                                        End Try
#End Region

                                    End If

                                Else 'BillingInfo Already send, check for aproving

                                    Dim PayInfoList As List(Of String) = New List(Of String)

                                    If AlreadySend.Contains(" ") Then
                                        PayInfoList.AddRange(AlreadySend.Split(" "c))
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


                                    If DelAutoinfoTXFromINI(T_DEXContract.CurrentCreationTransaction) Then 'Delete autosend-info-TX from Settings.ini
                                        'ok
                                    End If

                                    If T_OS.AutoCompleteAT Then 'autosignal AT

#Region "AutoComplete AT with PayPalInfo"
                                        Try

                                            If T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_E_Mail Then

                                                Dim PayPalStatus As String = CheckPayPalTransaction(T_DEXContract)

                                                If Not PayPalStatus.Trim = "" Then
                                                    AlreadySend = PayPalStatus
                                                End If

                                            ElseIf T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_Order Then

                                                If AlreadySend.Contains("PayPal-Order=") Then
                                                    Dim PayPalOrder As String = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim
                                                    If PayPalOrder.Contains(" ") Then
                                                        PayPalOrder = PayPalOrder.Remove(PayPalOrder.IndexOf(" "))
                                                    End If

                                                    Dim PayPalStatus As String = CheckPayPalOrder(T_DEXContract.ID, PayPalOrder)

                                                    If Not PayPalStatus.Trim = "" Then
                                                        AlreadySend = PayPalStatus
                                                    End If

                                                End If

                                            End If

                                        Catch ex As Exception
                                            If GetINISetting(E_Setting.InfoOut, False) Then
                                                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                                Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED->Autocomplete): -> " + ex.Message)
                                            End If
                                        End Try
#End Region

                                    End If

                                End If

#End Region

                                T_LVI = New ListViewItem

                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(T_DEXContract.Address) 'at
                                If T_DEXContract.IsSellOrder Then
                                    T_LVI.SubItems.Add("SellOrder") 'type
                                Else
                                    T_LVI.SubItems.Add("BuyOrder") 'type
                                End If

                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                                T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                                T_LVI.SubItems.Add("Me") 'seller
                                T_LVI.SubItems.Add(T_DEXContract.CurrentBuyerAddress) 'buyer
                                T_LVI.SubItems.Add(XItem) 'xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals)) 'xamount
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'collateral
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals)) 'price
                                T_LVI.SubItems.Add(AlreadySend) 'status
                                T_LVI.Tag = T_DEXContract

                                MyOpenOrderLVIList.Add(T_LVI)

                            Catch ex As Exception
                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED->Seller): -> " + ex.Message)
                                End If
                            End Try

                        ElseIf TBSNOAddress.Text = T_DEXContract.CurrentBuyerAddress Then ' Order.BuyerRS Then

                            'Save/ Update() my RESERVED SellOrder to cache2.dat (MyOrders Settings)
                            Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)
                            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status) ' Order.ATID, Order.FirstTransaction, Order.Type, Order.Status)

                            Dim Autosendinfotext As String = "False"
                            Dim AutocompleteAT As String = "False"

                            If Not T_DEXContract.IsSellOrder Then ' Order.Type = "BuyOrder" Then
                                If T_OSList.Count = 0 Then
                                    T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                    T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                Else
                                    T_OS = T_OSList(0)
                                    T_OS.Status = T_DEXContract.Status.ToString ' Order.Status
                                    PayMet = T_OS.PaytypeString
                                    Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                    AutocompleteAT = T_OS.AutoCompleteAT.ToString
                                End If

                                OrderSettingsBuffer.Add(T_OS)
                                'Broadcast info over DEXNET
                                BroadcastMsgs.Add("<ATID>" + T_DEXContract.ID.ToString + "</ATID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteAT>" + AutocompleteAT + "</AutocompleteAT>")

                            Else

#Region "BuyOrder-Info from DEXNET"

                                Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                If Not IsNothing(DEXNET) Then
                                    RelMsgs = DEXNET.GetRelevantMsgs()
                                End If

                                Dim RelKeyFounded As Boolean = False

                                For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                    If RelMsg.RelevantKey = "<ATID>" + T_DEXContract.ID.ToString + "</ATID>" Then

                                        RelKeyFounded = True

                                        If Not RelMsg.RelevantMessage.Trim = "" Then

                                            Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                            If T_DEXContract.CurrentSellerID = GetAccountID(PublicKey) Then
                                                Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                PayMet = PayMethod

                                                Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                Autosendinfotext = T_Autosendinfotext

                                                Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                                                AutocompleteAT = T_AutocompleteAT

                                            End If

                                        End If

                                    End If

                                Next

                                If Not RelKeyFounded Then
                                    If Not IsNothing(DEXNET) Then
                                        DEXNET.AddRelevantKey("<ATID>" + T_DEXContract.ID.ToString + "</ATID>")
                                    End If
                                End If
#End Region

                            End If


                            Dim AlreadySend As String = CheckBillingInfosAlreadySend(T_DEXContract)

                            If AlreadySend.Contains(Application.ProductName + "-error") Then
                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                    Out.ErrorLog2File(AlreadySend)
                                End If

                                AlreadySend = "ENCRYPTED"


                            ElseIf AlreadySend.Contains(Application.ProductName + "-warning") Then
                                If GetINISetting(E_Setting.InfoOut, False) Then
                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                    out.WarningLog2File(AlreadySend)
                                End If

                                AlreadySend = "ENCRYPTED"

                            ElseIf AlreadySend = "" Then

                                Dim SearchAttachmentHEX As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))


                                If T_DEXContract.CheckForUTX() Then
                                    AlreadySend = "PENDING"
                                Else
                                    AlreadySend = "RESERVED"
                                End If

                                'If CheckForUTX(Order.SellerRS, Order.ATRS, SearchAttachmentHEX) Then
                                '    AlreadySend = "PENDING"
                                'ElseIf CheckForUTX(Order.BuyerRS, Order.ATRS) Then

                                'Else

                                '    AlreadySend = "RESERVED"

                                'End If

                            ElseIf AlreadySend.Trim <> "" Then

                                Dim PayPalOrder As String = ""
                                If AlreadySend.Contains("PayPal-Order=") Then
                                    PayPalOrder = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim

                                    If PayPalOrder.Trim <> "" Then
                                        Dim PPAPI As ClsPayPal = New ClsPayPal()

                                        AlreadySend = PPAPI.URL + "/checkoutnow?token=" + PayPalOrder '"https://www.sandbox.paypal.com/checkoutnow?token=" 
                                        'Process.Start(AlreadySend)
                                    End If

                                Else

                                    Dim PayInfoList As List(Of String) = New List(Of String)

                                    If AlreadySend.Contains(" ") Then
                                        PayInfoList.AddRange(AlreadySend.Split(" "c))
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

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim CheckAttachment As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))
                            'Dim UTXCheck As Boolean = CheckForUTX(Order.SellerRS, Order.ATRS, CheckAttachment)
                            'Dim TXCheck As Boolean = CheckForTX(Order.SellerRS, Order.ATRS, Order.FirstTimestamp, CheckAttachment)

                            If Not T_DEXContract.CheckForUTX And Not T_DEXContract.CheckForTX Then

                            Else
                                AlreadySend = "PENDING"
                            End If

                            T_LVI = New ListViewItem

                            T_LVI.Text = Confirms 'confirms
                            T_LVI.SubItems.Add(T_DEXContract.Address) 'at

                            If T_DEXContract.IsSellOrder Then
                                T_LVI.SubItems.Add("SellOrder") 'type
                            Else
                                T_LVI.SubItems.Add("BuyOrder") 'type
                            End If

                            T_LVI.SubItems.Add(PayMet) 'method
                            T_LVI.SubItems.Add(Autosendinfotext) 'autoinfo
                            T_LVI.SubItems.Add(AutocompleteAT) 'autofinish
                            T_LVI.SubItems.Add(T_DEXContract.CurrentSellerAddress) ' Order.SellerRS) 'seller
                            T_LVI.SubItems.Add("Me") 'buyer
                            T_LVI.SubItems.Add(XItem) 'xitem
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, Decimals)) 'xamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'quantity
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'Order.Collateral)) 'collateral
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, Decimals)) 'price
                            T_LVI.SubItems.Add(AlreadySend) 'status
                            T_LVI.Tag = T_DEXContract

                            MyOpenOrderLVIList.Add(T_LVI)

                        End If 'myaddress

#End Region

                    Catch ex As Exception
                        If GetINISetting(E_Setting.InfoOut, False) Then
                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(RESERVED): -> " + ex.Message)
                        End If

                    End Try

                Else 'CLOSED or CANCELED

                End If


                For Each HistoryOrder As ClsDEXContract.S_Order In T_DEXContract.ContractOrderHistoryList

                    If HistoryOrder.Status = ClsDEXContract.E_Status.CLOSED Or HistoryOrder.Status = ClsDEXContract.E_Status.CANCELED Then

#Region "MyClosedOrders - GUI"

                        Try

                            If DelAutosignalTXFromINI(HistoryOrder.CreationTransaction) Then  'Delete autosignal-TX from Settings.ini
                                'ok
                            End If

                        Catch ex As Exception
                            If GetINISetting(E_Setting.InfoOut, False) Then
                                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(Delete): -> " + ex.Message)
                            End If

                        End Try


                        Dim TOSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(HistoryOrder.CreationTransaction)

                        Try

                            If TOSList.Count > 0 Then
                                Dim DelTOSID As ULong = 0
                                For Each T_TOS In TOSList
                                    If T_TOS.TXID = HistoryOrder.CreationTransaction Then
                                        DelTOSID = T_TOS.ATID
                                        Exit For
                                    End If
                                Next

                                If Not DelTOSID = 0 Then
                                    DelOrderSettings(DelTOSID)
                                End If

                            End If

                        Catch ex As Exception
                            If GetINISetting(E_Setting.InfoOut, False) Then
                                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(DeleteOrdersettingbuffer): -> " + ex.Message)
                            End If

                        End Try

                        If HistoryOrder.XItem.Contains(Market) Then

                            If TBSNOAddress.Text = HistoryOrder.SellerRS Then

                                Try

                                    'If MyClosedOrderLVIList.Count < 100 Then

                                    Dim T_DEXOrder As S_DEXOrder = New S_DEXOrder
                                    T_DEXOrder.ContractAddress = T_DEXContract.Address
                                    T_DEXOrder.Order = HistoryOrder
                                    T_DEXOrder.Contract = T_DEXContract

                                    'T_LVI.Text = HistoryOrder.CreationTransaction.ToString
                                    'T_LVI.SubItems.Add(HistoryOrder.LastTransaction.ToString)

                                    'T_LVI.SubItems.Add(HistoryOrder.Confirmations.ToString) 'confirms
                                    'T_LVI.SubItems.Add(T_DEXContract.Address)  'at

                                    'If HistoryOrder.WasSellOrder Then
                                    '    T_LVI.SubItems.Add("SellOrder") 'type
                                    'Else
                                    '    T_LVI.SubItems.Add("BuyOrder") 'type
                                    'End If

                                    'T_LVI.SubItems.Add("Me") 'seller
                                    'T_LVI.SubItems.Add(HistoryOrder.BuyerRS)  'buyer
                                    'T_LVI.SubItems.Add(XItem) 'xitem
                                    'T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.XAmount, Decimals)) 'xamount
                                    'T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.Amount))  'quantity
                                    'T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.Price, Decimals)) 'price
                                    'T_LVI.SubItems.Add(HistoryOrder.Status.ToString) 'status
                                    ''T_LVI.SubItems.Add(Order.Attachment.ToString) 'conditions
                                    'T_LVI.Tag = T_DEXContract

                                    MyClosedOrderLVIList.Add(T_DEXOrder)

                                    'End If

                                Catch ex As Exception
                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                        Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(SetClosedSellOrder): -> " + ex.Message)
                                    End If

                                End Try

                            ElseIf TBSNOAddress.Text = HistoryOrder.BuyerRS Then ' Order.BuyerRS Then

                                Try

                                    'If MyClosedOrderLVIList.Count < 100 Then

                                    Dim T_DEXOrder As S_DEXOrder = New S_DEXOrder
                                    T_DEXOrder.ContractAddress = T_DEXContract.Address
                                    T_DEXOrder.Order = HistoryOrder
                                    T_DEXOrder.Contract = T_DEXContract

                                    'T_LVI = New ListViewItem

                                    '    T_LVI.Text = HistoryOrder.CreationTransaction.ToString  'first transaction
                                    '    T_LVI.SubItems.Add(HistoryOrder.LastTransaction.ToString)


                                    '    T_LVI.SubItems.Add(HistoryOrder.Confirmations.ToString) 'confirms
                                    '    T_LVI.SubItems.Add(T_DEXContract.Address)  'at

                                    '    If HistoryOrder.WasSellOrder Then
                                    '        T_LVI.SubItems.Add("SellOrder") 'type
                                    '    Else
                                    '        T_LVI.SubItems.Add("BuyOrder") 'type
                                    '    End If

                                    '    T_LVI.SubItems.Add(HistoryOrder.SellerRS) 'seller
                                    '    T_LVI.SubItems.Add("Me") 'buyer
                                    '    T_LVI.SubItems.Add(XItem) 'xitem
                                    '    T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.XAmount, Decimals)) 'xamount
                                    '    T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.Amount)) 'quantity
                                    '    T_LVI.SubItems.Add(Dbl2LVStr(HistoryOrder.Price, Decimals)) 'price
                                    '    T_LVI.SubItems.Add(HistoryOrder.Status.ToString)  'status
                                    '    'T_LVI.SubItems.Add(Order.Attachment.ToString) 'conditions
                                    '    T_LVI.Tag = T_DEXContract

                                    MyClosedOrderLVIList.Add(T_DEXOrder)

                                    'End If

                                Catch ex As Exception
                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                        Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(SetClosedBuyOrder): -> " + ex.Message)
                                    End If

                                End Try

                            End If 'myaddress

                        End If 'market

#End Region

                    End If

                Next

#End Region


                If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or T_DEXContract.Status = ClsDEXContract.E_Status.CLOSED Or T_DEXContract.Status = ClsDEXContract.E_Status.CANCELED Then

                    If T_DEXContract.IsFrozen.ToString.ToLower = "true" And T_DEXContract.IsFinished.ToString.ToLower = "true" And T_DEXContract.IsDead.ToString.ToLower <> "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = T_DEXContract.Address  'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.Tag = T_DEXContract

                        OpenChannelLVIList.Add(T_LVI)

                    ElseIf T_DEXContract.IsDead.ToString.ToLower = "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = T_DEXContract.Address  'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.BackColor = Color.Crimson
                        T_LVI.ForeColor = Color.White
                        T_LVI.Tag = T_DEXContract

                        OpenChannelLVIList.Add(T_LVI)

                    End If

                End If


            End If 'BLS.AT_TXList.Count

#End Region

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in MultiThreadSetAT2LV(): -> " + ex.ToString)
            End If

        End Try

    End Sub




    Function Loading() As Boolean

        'Try

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        Dim x As List(Of String) = SignumAPI.GetBalance(TBSNOAddress.Text)
        TBSNOBalance.Text = GetDoubleBetweenFromList(x, "<available>", "</available>").ToString

        LabXItem.Text = CurrentMarket
        LabXitemAmount.Text = CurrentMarket + " Amount: "

        MarketIsCrypto = GetMarketCurrencyIsCrypto(CurrentMarket)
        Decimals = GetCurrencyDecimals(CurrentMarket)

        NUDSNOItemAmount.DecimalPlaces = Decimals


        CSVATList.Clear()

        OrderSettingsBuffer.Clear()

        Dim Wait As Boolean = GetAndCheckATs()
        Wait = MultithreadMonitor()

        DEXContractList.Clear()

        Dim T_DEXATList As List(Of List(Of String)) = GetDEXATsFromCSV()

        OrderSettingsBuffer = GetOrderSettings()

        Dim HistoryOrderToXMLThreadList As List(Of Threading.Thread) = New List(Of Threading.Thread)

        For Each APIRequest As S_APIRequest In APIRequestList



            Dim HistoryOrderToXMLThread As Threading.Thread = New Threading.Thread(AddressOf HistoryOrderToXML)
            HistoryOrderToXMLThread.Start(APIRequest)
            HistoryOrderToXMLThreadList.Add(HistoryOrderToXMLThread)

#Region "deprecaded"

            '            If Not IsNothing(APIRequest.Result) Then
            '                If APIRequest.Result.GetType = GetType(String) Then

            '                    Dim AT_ATRS_BLSAT As List(Of String) = New List(Of String)(APIRequest.Result.ToString.Split(","c))

            '                    Dim SAT As S_AT = New S_AT With {.ID = Convert.ToUInt64(AT_ATRS_BLSAT(0)), .IsDEX_AT = False} ', .ATRS = AT_ATRS_BLSAT(1)
            '                    CSVATList.Add(SAT)

            '                Else

            '                    Dim T_DEXContract As ClsDEXContract = DirectCast(APIRequest.Result, ClsDEXContract)
            '                    DEXContractList.Add(T_DEXContract)

            '#Region "HistoryOrders"

            '                    sdfdsfsdf


            '                    Dim HistoryOrdersString As String = ""

            '                    StatusLabel.Text = "Refreshing HistoryOrders from " + T_DEXContract.Address + "...(" + T_DEXContract.ContractOrderHistoryList.Count.ToString + ")"

            '                    For i As Integer = 0 To T_DEXContract.ContractOrderHistoryList.Count - 1

            '                        Application.DoEvents()

            '                        Dim HistoryOrder As ClsDEXContract.S_Order = T_DEXContract.ContractOrderHistoryList(i)

            '                        HistoryOrdersString += "<" + i.ToString + ">"
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.CreationTransaction.ToString, "CreationTransaction")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")
            '                        'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.Confirmations.ToString, "Confirmations")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.WasSellOrder.ToString, "WasSellOrder")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.StartTimestamp.ToString, "StartTimestamp")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.EndTimestamp.ToString, "EndTimestamp")

            '                        'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")
            '                        'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")

            '                        'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.SellerRS.ToString, "SellerRS")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.SellerID.ToString, "SellerID")

            '                        'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.BuyerRS.ToString, "BuyerRS")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.BuyerID.ToString, "BuyerID")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Amount).ToString, "Amount")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Collateral).ToString, "Collateral")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.XAmount).ToString, "XAmount")
            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.XItem.ToString, "XItem")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Price).ToString, "Price")

            '                        HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.Status.ToString, "Status")
            '                        HistoryOrdersString += "</" + i.ToString + ">"

            '                    Next

            '#End Region

            '                    Dim SAT As S_AT = New S_AT With {.ID = T_DEXContract.ID, .IsDEX_AT = True, .HistoryOrders = HistoryOrdersString} ', .ATRS = T_DEXContract.Address
            '                    CSVATList.Add(SAT)

            '                End If
            '            End If

#End Region

        Next


        StatusLabel.Text = "Refreshing HistoryOrders..."

        Dim WExit As Boolean = False
        While Not WExit
            WExit = True
            For i As Integer = 0 To HistoryOrderToXMLThreadList.Count - 1

                Dim XMLThread As Threading.Thread = HistoryOrderToXMLThreadList(i)

                Application.DoEvents()

                If XMLThread.IsAlive Then
                    WExit = False
                    Exit For
                End If
            Next

        End While

        StatusLabel.Text = "Refreshing HistoryOrders finished"

        SaveATsToCSV(CSVATList, OrderSettingsBuffer)

        If T_DEXATList.Count = 0 Then
            T_DEXATList = GetDEXATsFromCSV()
        End If

        If T_DEXATList.Count <> 0 Then
            DEXATList.Clear()
        End If

        If DEXATList.Count = 0 Then
            For Each T_DEX In T_DEXATList
                If T_DEX(1) = "True" Then
                    DEXATList.Add(T_DEX(0))
                End If
            Next
        End If

        'Catch ex As Exception

        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        '        Out.ErrorLog2File(Application.ProductName + "-error in Loading(): -> " + ex.Message)
        '    End If

        'End Try

        Return True

    End Function

    Sub HistoryOrderToXML(ByVal Input As Object)

        Dim APIRequest As S_APIRequest = DirectCast(Input, S_APIRequest)

        If Not IsNothing(APIRequest.Result) Then
            If APIRequest.Result.GetType = GetType(String) Then

                Dim AT_ATRS_BLSAT As List(Of String) = New List(Of String)(APIRequest.Result.ToString.Split(","c))

                Dim SAT As S_AT = New S_AT With {.ID = Convert.ToUInt64(AT_ATRS_BLSAT(0)), .IsDEX_AT = False} ', .ATRS = AT_ATRS_BLSAT(1)
                CSVATList.Add(SAT)

            Else

                Dim T_DEXContract As ClsDEXContract = DirectCast(APIRequest.Result, ClsDEXContract)
                DEXContractList.Add(T_DEXContract)

#Region "HistoryOrders"

                Dim HistoryOrdersString As String = ""

                For i As Integer = 0 To T_DEXContract.ContractOrderHistoryList.Count - 1

                    Application.DoEvents()

                    Dim HistoryOrder As ClsDEXContract.S_Order = T_DEXContract.ContractOrderHistoryList(i)

                    HistoryOrdersString += "<" + i.ToString + ">"
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.CreationTransaction.ToString, "CreationTransaction")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")
                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.Confirmations.ToString, "Confirmations")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.WasSellOrder.ToString, "WasSellOrder")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.StartTimestamp.ToString, "StartTimestamp")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.EndTimestamp.ToString, "EndTimestamp")

                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")
                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.LastTransaction.ToString, "LastTransaction")

                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.SellerRS.ToString, "SellerRS")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.SellerID.ToString, "SellerID")

                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.BuyerRS.ToString, "BuyerRS")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.BuyerID.ToString, "BuyerID")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Amount).ToString, "Amount")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Collateral).ToString, "Collateral")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.XAmount).ToString, "XAmount")
                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.XItem.ToString, "XItem")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(ClsSignumAPI.Dbl2Planck(HistoryOrder.Price).ToString, "Price")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.Status.ToString, "Status")
                    HistoryOrdersString += "</" + i.ToString + ">"

                Next

#End Region

                Dim SAT As S_AT = New S_AT With {.ID = T_DEXContract.ID, .IsDEX_AT = True, .HistoryOrders = HistoryOrdersString} ', .ATRS = T_DEXContract.Address
                CSVATList.Add(SAT)

            End If
        End If


    End Sub





    Sub LoadHistory(ByVal T_input As Object)

        Try


            Dim Input As List(Of Object) = New List(Of Object)

            If T_input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_input, List(Of Object))
            Else
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in HistoryLoad(DirectCast): -> " + T_input.GetType.Name)
                End If
            End If


            Dim CoBxChartVal As Integer = DirectCast(Input(0), Integer)
            Dim CoBxTickVal As Integer = DirectCast(Input(1), Integer)

            Dim Xitem As String = DirectCast(Input(2), String)

            Dim T_OrderList As List(Of ClsDEXContract.S_Order) = New List(Of ClsDEXContract.S_Order)
            Dim T_OpenDEXContractList As List(Of ClsDEXContract) = New List(Of ClsDEXContract)


            For i As Integer = 0 To DEXContractList.Count - 1

                Dim DEXContract As ClsDEXContract = DEXContractList(i)

                If DEXContract.Status = ClsDEXContract.E_Status.OPEN Then
                    T_OpenDEXContractList.Add(DEXContract)
                End If

                For ii As Integer = 0 To DEXContract.ContractOrderHistoryList.Count - 1

                    Dim T_HistoryOrder As ClsDEXContract.S_Order = DEXContract.ContractOrderHistoryList(ii)

                    If T_HistoryOrder.Status = ClsDEXContract.E_Status.CLOSED Then
                        T_OrderList.Add(T_HistoryOrder)
                        'ElseIf HisOrder.Status = "OPEN" Then
                        '    T_OpenOrderList.Add(T_Order)
                    End If

                Next


            Next


            If GetINISetting(E_Setting.TCPAPIEnable, True) Then ' ChBxTCPAPI.Checked Then
                LoadTCPAPIOpenOrders(T_OpenDEXContractList)
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
                PanelForTradeTrackerSlot.Controls(0).Tag = New List(Of Object)({Xitem, Minval, Maxval, Chart(Chart.Count - 1).CloseValue, Chart})

                TradeTrackCalcs(PanelForTradeTrackerSlot.Controls(0))
            End If

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in LoadHistory(): -> " + ex.Message)
            End If

        End Try

    End Sub

    Function TradeTrackCalcs(ByVal input As Object) As Boolean

        Try

            Dim TTSlot As TradeTrackerSlot = DirectCast(input, TradeTrackerSlot)

            Dim Upper_Big_Graph As Graph = New Graph
            Dim Lower_Small_Graph As Graph = New Graph

            '0=XItem; 1=MinVal; 2=MaxVal; 3=LastVal; 4=List(date, close)
            Dim MetaList As List(Of Object) = DirectCast(TTSlot.Tag, List(Of Object))

            Dim XItem As String = Convert.ToString(MetaList(0))
            Dim MinVals As Double = Convert.ToDouble(MetaList(1))
            Dim MaxVals As Double = Convert.ToDouble(MetaList(2))
            Dim LastVal As Double = Convert.ToDouble(MetaList(3))

            MultiInvoker(TTSlot.LabExch, "Text", "")
            MultiInvoker(TTSlot.LabPair, "Text", XItem)

            TTSlot.Pair = XItem
            TTSlot.Refresher()

            Dim Chart As List(Of S_Candle) = DirectCast(MetaList(4), List(Of S_Candle))

            Dim Extra As List(Of Graph.S_Extra) = New List(Of Graph.S_Extra)
            Dim XList As List(Of Object) = New List(Of Object)

            For Each Candle As S_Candle In Chart
                XList.Add(New List(Of Object)({Candle.CloseDat.ToString, Candle.CloseValue, Candle.HighValue, Candle.LowValue, Candle.Volume}))
            Next

            Upper_Big_Graph.GraphValuesList = New List(Of Graph.S_Graph)
            Lower_Small_Graph.GraphValuesList = New List(Of Graph.S_Graph)

            Dim CandleGraph As Graph.S_Graph = New Graph.S_Graph
            CandleGraph.PieceGraphList = DateIntToCandle(XList)

            Dim VolumeGraph As Graph.S_Graph = New Graph.S_Graph
            VolumeGraph.PieceGraphList = DateIntToCandle(XList)

            If TTSlot.ChBxCandles.Checked = True Then

                CandleGraph.MinValue = MinVals
                CandleGraph.MaxValue = MaxVals
                CandleGraph.Extra = Extra
                CandleGraph.GraphArt = Graph.E_GraphArt.Candle

                Upper_Big_Graph.GraphValuesList.Add(CandleGraph)

            Else

            End If


            Dim MinVolume As Double = Double.MaxValue
            Dim MaxVolume As Double = 0.0

            For Each Vol As Graph.S_PieceGraph In VolumeGraph.PieceGraphList

                Dim T_Vol As Double = Vol.Volume

                If T_Vol > MaxVolume Then
                    MaxVolume = T_Vol
                End If

                If T_Vol < MinVolume Then
                    MinVolume = T_Vol
                End If

            Next

            If TTSlot.ChBxVolume.Checked = True Then

                VolumeGraph.MinValue = MinVolume
                VolumeGraph.MaxValue = MaxVolume
                VolumeGraph.Extra = Extra
                VolumeGraph.GraphArt = Graph.E_GraphArt.Volume

                Lower_Small_Graph.GraphValuesList.Add(VolumeGraph)
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


            Dim EMAFastList = EMAx(XList, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDEMA1.Text) * 1440) / 5)) '1440 min * textbox (12) / 5min candle
            If TTSlot.ChBxEMA1.Checked = True Then

                Dim EMAFastGraph As Graph.S_Graph = New Graph.S_Graph
                EMAFastGraph.GraphArt = Graph.E_GraphArt.EMAFast
                EMAFastGraph.MinValue = MinVals
                EMAFastGraph.MaxValue = MaxVals

                EMAFastGraph.PieceGraphList = DateIntToCandle(EMAFastList)

                Upper_Big_Graph.GraphValuesList.Add(EMAFastGraph)
            Else

            End If


            Dim EMASlowList = EMAx(XList, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDEMA2.Text) * 1440) / 5)) '1440 min * textbox (26) / 5min candle
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

                Dim T_MAC As List(Of Object) = New List(Of Object)
                If MAC.GetType.Name = GetType(List(Of Object)).Name Then
                    T_MAC = DirectCast(MAC, List(Of Object))
                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + MAC.GetType.Name)
                    End If
                End If

                If T_MAC.Count > 0 Then
                    Dim MACVal As Double = Convert.ToDouble(T_MAC(1))

                    If MACVal > MaxMACD Then
                        MaxMACD = MACVal
                    End If

                    If MACVal < MinMACD Then
                        MinMACD = MACVal
                    End If

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



            Dim SignalList = EMAx(MACDL, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDSig.Text) * 1440) / 5))
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

                Dim TMSigi As List(Of Object) = New List(Of Object)

                If MSigi.GetType.Name = GetType(List(Of Object)).Name Then
                    TMSigi = DirectCast(MSigi, List(Of Object))
                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + MSigi.GetType.Name)
                    End If
                End If

                If TMSigi.Count > 0 Then

                    Dim ZMACVal As Double = Convert.ToDouble(TMSigi(1))

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


            Dim RSIL = RSIx(CandleGraph.PieceGraphList, Integer.Parse(TTSlot.TBRSICandles.Text) * 10)

            Dim MinRSI As Double = Double.MaxValue
            Dim MaxRSI As Double = 0.0

            For Each RSIi In RSIL

                Dim TRSIi As List(Of Object) = New List(Of Object)
                If RSIi.GetType.Name = GetType(List(Of Object)).Name Then
                    TRSIi = DirectCast(RSIi, List(Of Object))
                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + RSIi.GetType.Name)
                    End If
                End If

                If TRSIi.Count > 0 Then

                    Dim RSIVal As Double = Convert.ToDouble(TRSIi(1))

                    If RSIVal > MaxRSI Then
                        MaxRSI = RSIVal
                    End If

                    If RSIVal < MinRSI Then
                        MinRSI = RSIVal
                    End If

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
                MultiInvoker(TTSlot.LabLast, "ForeColor", SignumLightGreen)

            ElseIf HGTrend = "↓" Then
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Red)
            Else
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Black)
            End If

            MultiInvoker(TTSlot.LabMIN, "Text", String.Format("{0:#0.00000000}", MinVals))

            TTSlot.Chart_EMA_Graph = Upper_Big_Graph
            TTSlot.MACD_RSI_TR_GraphList = Lower_Small_Graph


        Catch ex As Exception

            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in TradeTrackCalcs(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function


#Region "TCP API"

    Property TCPAPI As ClsTCPAPI

    Sub LoadTCPAPIOpenOrders(ByVal OpenDEXContracts As List(Of ClsDEXContract))

        Try

            Dim OpenOrdersJSON As String = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""GetOpenOrders"",""data"":["

            For Each T_Order As ClsDEXContract In OpenDEXContracts

                Dim Contract As String = T_Order.Address
                Dim Type As String = ""
                If T_Order.IsSellOrder Then
                    Type = "SellOrder"
                Else
                    Type = "BuyOrder"
                End If


                Dim Seller As String = T_Order.CurrentSellerAddress
                Dim Buyer As String = T_Order.CurrentBuyerAddress

                Dim Collateral As String = T_Order.CurrentInitiatorsCollateral.ToString.Replace(",", ".")
                Dim SignaAmount As String = T_Order.CurrentBuySellAmount.ToString.Replace(",", ".")
                Dim Price As String = T_Order.CurrentPrice.ToString.Replace(",", ".")
                Dim XItem As String = T_Order.CurrentXItem + "_SIGNA"
                Dim XAmount As String = T_Order.CurrentXAmount.ToString.Replace(",", ".")

                OpenOrdersJSON += "{""at"":""" + Contract + ""","
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
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIOpenOrders(): -> " + ex.Message)
            End If

        End Try

    End Sub
    Sub LoadTCPAPIHistorys(ByVal OrderList As List(Of ClsDEXContract.S_Order))

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

            Dim T_FrmDevelope As FrmDevelope = DirectCast(GetSubForm("FrmDevelope"), FrmDevelope)
            If Not IsNothing(T_FrmDevelope) Then

                If TCPAPI.StatusMSG.Count > 0 Then

                    For i As Integer = 0 To TCPAPI.StatusMSG.Count - 1
                        Dim MSG As String = TCPAPI.StatusMSG(i)

                        MultiInvoker(T_FrmDevelope.LiBoTCPAPIStatus, "Items", New List(Of Object)({"Insert", 0, MSG}))
                    Next

                    TCPAPI.StatusMSG.Clear()

                End If
            End If

            'LabClients.Text = "Clients: " + TCPAPI.ConnectionList.Count.ToString

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIHistorys(): -> " + ex.Message)
            End If

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
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day.ToString, Tick.ToString})
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
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day.ToString, Tick.ToString})
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
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day.ToString, Tick.ToString})
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
                        Dim T_t As List(Of String) = New List(Of String)({FullCur, Day.ToString, Tick.ToString})
                        Currency_Day_Tick.Add(T_t)
                    Next

                Next

            Next

            Return Currency_Day_Tick

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in GetTCPAPICurrencyDaysTicks(): -> " + ex.Message)
            End If

            Return New List(Of List(Of String))
        End Try

    End Function
    Sub LoadTCPAPIHistory(ByVal T_Input As Object)

        Try

            Dim Input As List(Of Object) = New List(Of Object)

            If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_Input, List(Of Object))
            Else
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIHistory(DirectCast): -> " + T_Input.GetType.Name)
                End If
            End If


            Dim XItem As String = DirectCast(Input(0), String)
            Dim OrderList As List(Of ClsDEXContract.S_Order) = DirectCast(Input(1), List(Of ClsDEXContract.S_Order))
            Dim Days As String = DirectCast(Input(2), String)
            Dim Tick As String = DirectCast(Input(3), String)


            Dim Chart As List(Of S_Candle) = GetCandles(XItem.Remove(XItem.IndexOf("_")), OrderList, Integer.Parse(Days), Integer.Parse(Tick))

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

#Region "deprecated"
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
#End Region

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in LoadTCPAPIHistory(): -> " + ex.Message)
            End If

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
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in RefreshTCPAPIResponse(): -> " + ex.Message)
            End If

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
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in CompareLists(): -> " + ex.Message)
            End If

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
                    'DEXNET.DEXNET_AgreeKeyHEX = T_AgreementKeyHEX
                Else
                    If DEXNET.DEXNETClose = True Then
                        DEXNET = New ClsDEXNET(GetINISetting(E_Setting.DEXNETServerPort, 8131), GetINISetting(E_Setting.DEXNETShowStatus, False))
                        'DEXNET.DEXNET_AgreeKeyHEX = T_AgreementKeyHEX
                    End If
                End If


                Dim DEXNETNodesString As String = GetINISetting(E_Setting.DEXNETNodes, "signum.zone:8131")
                Dim DEXNETMyHost As String = GetINISetting(E_Setting.DEXNETMyHost, "")


                Dim DEXNETNodes As List(Of String) = New List(Of String)
                If DEXNETNodesString.Contains(";") Then
                    DEXNETNodes.AddRange(DEXNETNodesString.Split(";"c))
                Else
                    DEXNETNodes.Add(DEXNETNodesString)
                End If

                For Each DNNode As String In DEXNETNodes

                    If Not DEXNETMyHost = "" Then
                        If Not DEXNET.CheckHostIsNotIP(DNNode, DEXNETMyHost) Then
                            Continue For
                        End If
                    End If

                    Dim HostIP As String = ""
                    Dim RemotePort As Integer = 0
                    If DNNode.Contains(":") Then
                        HostIP = DNNode.Remove(DNNode.IndexOf(":"))
                        RemotePort = Integer.Parse(DNNode.Substring(DNNode.IndexOf(":") + 1))
                    End If

                    DEXNET.Connect(HostIP, RemotePort, DEXNETMyHost)

                Next

            End If


        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in InitiateDEXNET(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function

    Property RelevantMsgsBuffer As List(Of S_RelevantMsg) = New List(Of S_RelevantMsg)

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

                Dim RM_ATID As String = GetStringBetween(RelMsg.RelevantMessage, "<ATID>", "</ATID>")
                Dim SigAPI As ClsSignumAPI = New ClsSignumAPI()
                Dim ATIDRSList As List(Of String) = SigAPI.RSConvert(ULong.Parse(RM_ATID))
                Dim RM_ATRS As String = GetStringBetweenFromList(ATIDRSList, "<accountRS>", "</accountRS>")

                Dim RM_AccountPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")
                Dim RM_AccountPublicKeyList As List(Of String) = SigAPI.RSConvert(GetAccountID(RM_AccountPublicKey))
                Dim RM_AccountRS As String = GetStringBetweenFromList(RM_AccountPublicKeyList, "<accountRS>", "</accountRS>")

                Dim ForContinue As Boolean = False
                For Each LVI As ListViewItem In LVSellorders.Items
                    Dim LVI_ATRS As String = Convert.ToString(GetLVColNameFromSubItem(LVSellorders, "AT", LVI))
                    Dim LVI_SellerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVSellorders, "Seller", LVI))

                    If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_SellerRS Then

                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                        SetLVColName2SubItem(LVSellorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                        SetLVColName2SubItem(LVSellorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
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
                    Dim LVI_ATRS As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "AT", LVI))
                    Dim LVI_BuyerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Buyer", LVI))

                    If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_BuyerRS Then

                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                        SetLVColName2SubItem(LVBuyorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                        SetLVColName2SubItem(LVBuyorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
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

                    Dim LVI_ATRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "AT", LVI))
                    Dim LVI_Type As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Type", LVI))
                    Dim LVI_SellerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVI))
                    Dim LVI_BuyerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVI))


                    If LVI_Type = "SellOrder" Then

                        If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_SellerRS Then

                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteAT)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    Else

                        If RM_ATRS = LVI_ATRS And RM_AccountRS = LVI_BuyerRS Then

                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteAT As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteAT>", "</AutocompleteAT>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteAT)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    End If



                Next

#End Region

            ElseIf RelMsg.RelevantMessage.Contains("<ATID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<Ask>") Then
                Dim RM_ATID As ULong = GetULongBetween(RelMsg.RelevantMessage, "<ATID>", "</ATID>")
                Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                Dim RM_AccountPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")
                Dim RM_AccountPublicKeyList As List(Of String) = SignumAPI.RSConvert(GetAccountID(RM_AccountPublicKey))
                Dim RM_AccountID As ULong = GetULongBetweenFromList(RM_AccountPublicKeyList, "<account>", "</account>")
                'Dim RM_AccIDULong As ULong = 0UL

                'Try
                '    RM_AccIDULong = RM_AccountID
                'Catch ex As Exception

                '    If GetINISetting(E_Setting.InfoOut, False) Then
                '        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                '        out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(RM_AccointID=" + RM_AccountID.ToString + "): -> " + vbCrLf + ex.Message)
                '    End If

                'End Try


                'TODO: Processing UnexpectedMsgs from DEXNET

                Dim RM_Ask As String = GetStringBetween(RelMsg.RelevantMessage, "<Ask>", "</Ask>")

                If RM_Ask = "WTB" And Not RM_AccountID = 0 And Not RM_AccountPublicKey = "" Then

                    For Each LVI As ListViewItem In LVMyOpenOrders.Items
                        Dim MyOpenDEXContract As ClsDEXContract = DirectCast(LVI.Tag, ClsDEXContract)

                        Dim My_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(MyOpenDEXContract.CurrentCreationTransaction) '.FirstTransaction)

                        If My_OSList.Count <> 0 Then
                            Dim My_OS As ClsOrderSettings = My_OSList(0)

                            If My_OS.AutoSendInfotext Then

                                If Not GetAutosignalTXFromINI(MyOpenDEXContract.CurrentCreationTransaction) Then '.FirstTransaction) Then

                                    If MyOpenDEXContract.ID = RM_ATID Then

                                        If MyOpenDEXContract.CurrentInitiatorsCollateral = 0.0 Then

                                            Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceInjectResponder, RM_AccountID})
                                            Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)

                                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                                            If MasterKeys.Count > 0 Then

                                                Dim Response As String = SignumAPI.SendMessage2BLSAT(MasterKeys(0), RM_ATID, 1.0, ULngList)

                                                If Response.Contains(Application.ProductName + "-error") Then

                                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                        out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs2): -> " + vbCrLf + Response)
                                                    End If

                                                Else

                                                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                                    If TX.Contains(Application.ProductName + "-error") Then

                                                        If GetINISetting(E_Setting.InfoOut, False) Then
                                                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                            out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs3): -> " + vbCrLf + TX)
                                                        End If

                                                    Else

                                                        Dim PayInfo As String = GetPaymentInfoFromOrderSettings(MyOpenDEXContract.CurrentCreationTransaction, MyOpenDEXContract.CurrentBuySellAmount, MyOpenDEXContract.CurrentXAmount, MyOpenDEXContract.CurrentXItem)

                                                        Dim KnownAcc As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(RM_AccountID.ToString)

                                                        If KnownAcc.Contains(Application.ProductName + "-error") Then
                                                            'cant find account in blockchain, send message with pubkey to activate account

                                                            If Not PayInfo.Trim = "" Then

                                                                If PayInfo.Contains("PayPal-E-Mail=") Then
                                                                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(MyOpenDEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                                    PayInfo += " Reference/Note=" + ColWordsString
                                                                End If

                                                                Dim T_MsgStr As String = "Account activation" ' "AT=" + MyOrder.ATRS + " TX=" + MyOrder.FirstTransaction + " " + Dbl2LVStr(MyOrder.XAmount, Decimals) + " " + MyOrder.XItem + " " + PayInfo
                                                                Dim TXr As String = SendBillingInfos(RM_AccountID, T_MsgStr, False, True) ' SignumAPI.SendMessage(MasterKeys(0), MasterKeys(1), MasterKeys(2), RM_AccountID, T_MsgStr,, True,, RM_AccountPublicKey)


                                                                If TXr.Contains(Application.ProductName + "-error") Then
                                                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                                        out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs4): -> " + vbCrLf + TXr)
                                                                    End If

                                                                ElseIf TXr.Contains(Application.ProductName + "-warning") Then
                                                                    If GetINISetting(E_Setting.InfoOut, False) Then
                                                                        Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                                        out.WarningLog2File(Application.ProductName + "-warning in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs4): -> " + vbCrLf + TXr)
                                                                    End If
                                                                Else


                                                                End If

                                                            End If

                                                        End If

                                                    End If




                                                End If

                                                RelMsg.Setted = True
                                                RelevantMsgsBuffer(ii) = RelMsg
                                            Else

                                                If GetINISetting(E_Setting.InfoOut, False) Then
                                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                    out.WarningLog2File(Application.ProductName + "-warning in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs1): -> no Keys")
                                                End If

                                            End If


                                        End If

                                        Exit For

                                    End If

                                End If

                            End If

                        End If

                    Next

                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectedMsgs): -> ASK=" + RM_Ask + " ACCID=" + RM_AccountID.ToString + " Pubkey=" + RM_AccountPublicKey)
                    End If

                End If

            End If

        Next


    End Sub

#End Region


#Region "AT Interactions"
    'Function GetLastTXWithValues(ByVal BLSTX As List(Of S_PFPAT_TX), Optional ByVal Xitem As String = "", Optional ByVal AttSearch As String = "<xItem>") As S_PFPAT_TX

    '    Dim BLSAT_TX As S_PFPAT_TX = New S_PFPAT_TX

    '    Dim NuBLSTX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)
    '    NuBLSTX.AddRange(BLSTX.ToArray)
    '    NuBLSTX.Reverse()

    '    Dim Found As Boolean = False
    '    Dim ReferenceSenderID As ULong = 0UL
    '    Dim ReferenceSenderRS As String = ""


    '    For i As Integer = 0 To NuBLSTX.Count - 1
    '        Dim TTX As S_PFPAT_TX = NuBLSTX(i)
    '        BLSAT_TX = TTX


    '        If Not IsNothing(BLSAT_TX.Attachment) Then
    '            If Not BLSAT_TX.Attachment.Contains(Xitem) And Not Xitem.Trim = "" Then
    '                Found = False
    '            End If

    '            If Not ReferenceSenderRS = BLSAT_TX.SenderRS Then

    '                If Not BLSAT_TX.Attachment.Contains("<injectedResponser>" + ReferenceSenderID.ToString + "</injectedResponser>") Then
    '                    Found = False
    '                Else

    '                    Found = True

    '                    Try
    '                        BLSAT_TX.SenderID = ReferenceSenderID
    '                    Catch ex As Exception

    '                    End Try

    '                    BLSAT_TX.SenderRS = ReferenceSenderRS

    '                End If

    '            Else
    '                Found = True
    '            End If

    '        End If

    '        If Found Then
    '            Exit For
    '        End If

    '        If TTX.Type = "BLSTX" Then

    '            If TTX.Attachment.Contains(AttSearch) Then
    '                Found = True
    '                ReferenceSenderRS = TTX.RecipientRS
    '                ReferenceSenderID = TTX.RecipientID

    '            End If

    '        End If

    '    Next

    '    If Found Then
    '        Return BLSAT_TX
    '    Else
    '        Return New S_PFPAT_TX("")
    '    End If


    'End Function

    'Function ConvertTXs2Orders(ByVal BLSAT As S_PFPAT) As List(Of S_Order)

    '    Dim NuOrderList As List(Of S_Order) = New List(Of S_Order)
    '    Dim TXSplitList As List(Of List(Of S_PFPAT_TX)) = GetDeals(BLSAT.AT_TXList, "")

    '    For Each TXOrder As List(Of S_PFPAT_TX) In TXSplitList

    '        Dim Order As S_Order = ConvertTXs2Order(BLSAT, TXOrder)

    '        If Not IsNothing(Order) Then
    '            NuOrderList.Add(Order)
    '        End If

    '    Next

    '    If NuOrderList.Count > 1 Then
    '        For i As Integer = 1 To NuOrderList.Count - 1
    '            Dim NuOrder As S_Order = NuOrderList(i)

    '            If NuOrder.Status <> "CLOSED" And NuOrder.Status <> "CANCELED" Then
    '                NuOrder.Status = "CANCELED"
    '                NuOrderList(i) = NuOrder
    '            End If

    '        Next
    '    End If

    '    Return NuOrderList

    'End Function
    'Function GetDeals(ByVal BLSTX As List(Of S_PFPAT_TX), ByVal MyAddress As String, Optional ByVal Xitem As String = "") As List(Of List(Of S_PFPAT_TX))

    '    Dim RetList As List(Of List(Of S_PFPAT_TX)) = New List(Of List(Of S_PFPAT_TX))

    '    Dim BLSAT_TX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

    '    Dim NuBLSTX As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)
    '    NuBLSTX.AddRange(BLSTX.ToArray)
    '    NuBLSTX.Reverse()

    '    Dim FoundDeal As Boolean = False
    '    Dim FoundMyDeal As Boolean = False

    '    Dim Initiator As String = ""
    '    Dim CheckAttachment As String = ""

    '    If MyAddress.Trim = "" Then
    '        FoundMyDeal = True
    '    End If

    '    For i As Integer = 0 To NuBLSTX.Count - 1
    '        Dim TTX As S_PFPAT_TX = NuBLSTX(i)
    '        BLSAT_TX.Add(TTX)

    '        If TTX.SenderRS = MyAddress Or TTX.RecipientRS = MyAddress Then
    '            FoundMyDeal = True
    '        End If

    '        If FoundDeal And TTX.SenderRS = Initiator And TTX.Attachment = CheckAttachment Then
    '            If (TTX.Attachment.Contains(Xitem) Or Xitem.Trim = "") And FoundMyDeal Then
    '                BLSAT_TX.Reverse()
    '                RetList.Add(BLSAT_TX)
    '                FoundDeal = False

    '                If Not MyAddress.Trim = "" Then
    '                    FoundMyDeal = False
    '                End If

    '                BLSAT_TX = New List(Of S_PFPAT_TX)
    '                Continue For
    '            Else
    '                FoundDeal = False
    '                BLSAT_TX = New List(Of S_PFPAT_TX)
    '            End If
    '        End If

    '        If TTX.Type = "BLSTX" Then

    '            If TTX.Attachment.Contains("<xItem>") Then
    '                FoundDeal = True
    '                Initiator = TTX.RecipientRS
    '                CheckAttachment = TTX.Attachment
    '            End If

    '        End If

    '    Next

    '    Return RetList

    'End Function
    'Function ConvertTXs2Order(ByVal BLSAT As S_PFPAT, ByVal TXList As List(Of S_PFPAT_TX)) As S_Order

    '    'TODO: Fix OPEN and RESERVED TXs

    '    Dim NuOrder As S_Order = New S_Order
    '    Dim FirstTX As S_PFPAT_TX = New S_PFPAT_TX

    '    If TXList.Count > 0 Then
    '        FirstTX = GetLastTXWithValues(TXList, "",)
    '        If FirstTX.Attachment.Trim = "" Then
    '            Return Nothing
    '        End If
    '    End If

    '    Dim FirstAmount As Double = ClsSignumAPI.Planck2Dbl(FirstTX.AmountNQT)

    '    Dim Xitem As String = Between(FirstTX.Attachment, "<xItem>", "</xItem>", GetType(String))
    '    Dim Xamount As Double = ClsSignumAPI.Planck2Dbl(ULong.Parse(Between(FirstTX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
    '    Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(ULong.Parse(Between(FirstTX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))

    '    Dim SellOrder As Boolean = False
    '    If FirstAmount > Collateral Then
    '        SellOrder = True
    '    End If


    '    Dim ResponserRS As String = GetLastTXWithValues(TXList, "", "accepted").SenderRS
    '    Dim ResponserID As ULong = GetLastTXWithValues(TXList, "", "accepted").SenderID
    '    Dim Status As String = GetLastTXWithValues(TXList, "", "finished").TransactionID.ToString


    '    If Status.Trim = "0" Then
    '        If ResponserRS.Trim = "" Then
    '            Status = "OPEN"
    '        Else
    '            Status = "RESERVED"
    '        End If

    '    Else

    '        Dim LastSender As String = GetLastTXWithValues(TXList, "", "finished").SenderRS

    '        If FirstTX.Type = "SellOrder" Then
    '            If FirstTX.SenderRS = LastSender And Not ResponserRS = "" Then
    '                'Order CLOSED
    '                Status = "CLOSED"
    '            Else
    '                'Order CANCELED
    '                Status = "CANCELED"
    '            End If
    '        Else
    '            If FirstTX.SenderRS = LastSender Then
    '                'Order CANCELED
    '                Status = "CANCELED"
    '            Else
    '                'Order CLOSED
    '                Status = "CLOSED"
    '            End If

    '        End If

    '    End If



    '    Dim LastTX As S_PFPAT_TX = New S_PFPAT_TX

    '    If Status = "CANCELED" And ResponserRS.Trim = "" Then

    '        For Each ConfirmedTX As S_PFPAT_TX In TXList

    '            If ConfirmedTX.Attachment.Contains("finished") Then
    '                LastTX = ConfirmedTX
    '                Exit For
    '            End If

    '        Next

    '    Else

    '        For Each ConfirmedTX As S_PFPAT_TX In TXList

    '            If ConfirmedTX.Type = "BLSTX" And ClsSignumAPI.Planck2Dbl(ConfirmedTX.AmountNQT) > 0.0 Then

    '                If Status = "CLOSED" Then

    '                    If SellOrder Then
    '                        'responser = buyer
    '                        If ConfirmedTX.RecipientRS = ResponserRS Then
    '                            LastTX = ConfirmedTX
    '                            Exit For
    '                        End If
    '                    Else
    '                        'initiator = buyer
    '                        If ConfirmedTX.RecipientRS = FirstTX.SenderRS Then
    '                            LastTX = ConfirmedTX
    '                            Exit For
    '                        End If
    '                    End If

    '                ElseIf Status = "CANCELED" Then

    '                    If SellOrder Then
    '                        'responser = buyer
    '                        If ConfirmedTX.RecipientRS = ResponserRS Then
    '                            LastTX = ConfirmedTX
    '                            Exit For
    '                        End If
    '                    Else
    '                        'initiator = buyer
    '                        If ConfirmedTX.RecipientRS = FirstTX.SenderRS Then
    '                            LastTX = ConfirmedTX
    '                            Exit For
    '                        End If
    '                    End If

    '                End If

    '            End If

    '        Next

    '    End If



    '    If FirstTX.Type = "SellOrder" Then

    '        Dim WTSAmount As Double = ClsSignumAPI.Planck2Dbl(FirstTX.AmountNQT) - Collateral - ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)

    '        NuOrder = New S_Order With {
    '                .ATID = BLSAT.ATID,
    '                .ATRS = BLSAT.ATRS,
    '                .Type = FirstTX.Type,
    '                .SellerRS = FirstTX.SenderRS,
    '                .SellerID = FirstTX.SenderID,
    '                .BuyerRS = ResponserRS,
    '                .BuyerID = ResponserID,
    '                .XItem = Xitem,
    '                .XAmount = Xamount,
    '                .Quantity = WTSAmount,
    '                .Price = Xamount / WTSAmount,
    '                .Collateral = Collateral,
    '                .Status = Status,
    '                .Attachment = FirstTX.Attachment.ToString,
    '                .FirstTransaction = FirstTX.TransactionID,
    '                .FirstTimestamp = FirstTX.Timestamp,
    '                .FirstTX = FirstTX,
    '                .LastTX = LastTX
    '            }


    '    ElseIf FirstTX.Type = "BuyOrder" Then

    '        NuOrder = New S_Order With {
    '                .ATID = BLSAT.ATID,
    '                .ATRS = BLSAT.ATRS,
    '                .Type = FirstTX.Type,
    '                .SellerRS = ResponserRS,
    '                .SellerID = ResponserID,
    '                .BuyerRS = FirstTX.SenderRS,
    '                .BuyerID = FirstTX.SenderID,
    '                .XItem = Xitem,
    '                .XAmount = Xamount,
    '                .Quantity = Collateral,
    '                .Price = Xamount / Collateral,
    '                .Collateral = ClsSignumAPI.Planck2Dbl(FirstTX.AmountNQT - ClsSignumAPI._GasFeeNQT),
    '                .Status = Status,
    '                .Attachment = FirstTX.Attachment.ToString,
    '                .FirstTransaction = FirstTX.TransactionID,
    '                .FirstTimestamp = FirstTX.Timestamp,
    '                .FirstTX = FirstTX,
    '                .LastTX = LastTX
    '            }

    '    End If

    '    Return NuOrder

    'End Function

    'Function GetAccountTXList(ByVal ATID As ULong, Optional ByVal Node As String = "", Optional FromTimestamp As ULong = 0UL, Optional ByVal UseBuffer As Boolean = True) As List(Of S_PFPAT_TX)

    '    If Node = "" Then
    '        Node = PrimaryNode
    '    End If

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node)
    '    SignumAPI.DEXATList = DEXATList


    '    Dim ATTXs As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(ATID, FromTimestamp, UseBuffer)
    '    Dim BLSTXs As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

    '    For Each ATTX As List(Of String) In ATTXs

    '        Dim TX_Type As String = BetweenFromList(ATTX, "<type>", "</type>",, GetType(String))
    '        Dim TX_Timestamp As ULong = BetweenFromList(ATTX, "<timestamp>", "</timestamp>",, GetType(ULong))
    '        Dim TX_RecipientID As ULong = BetweenFromList(ATTX, "<recipient>", "</recipient>",, GetType(ULong))
    '        Dim TX_RecipientRS As String = BetweenFromList(ATTX, "<recipientRS>", "</recipientRS>",, GetType(String))
    '        Dim TX_AmountNQT As ULong = BetweenFromList(ATTX, "<amountNQT>", "</amountNQT>",, GetType(ULong))
    '        Dim TX_FeeNQT As ULong = BetweenFromList(ATTX, "<feeNQT>", "</feeNQT>",, GetType(ULong))
    '        Dim TX_Transaction As ULong = BetweenFromList(ATTX, "<transaction>", "</transaction>",, GetType(ULong))

    '        'StatusLabel.Text = "checking AT TX: " + TX_Transaction

    '        MultiInvoker(StatusLabel, "Text", "checking AT TX: " + TX_Transaction.ToString)


    '        Dim TX_Attachment As String = BetweenFromList(ATTX, "<attachment>", "</attachment>",, GetType(String))
    '        If TX_Attachment.Trim = "" Then
    '            TX_Attachment = BetweenFromList(ATTX, "<message>", "</message>",, GetType(String))
    '        End If


    '        Dim TX_SenderID As ULong = BetweenFromList(ATTX, "<sender>", "</sender>",, GetType(ULong))
    '        Dim TX_SenderRS As String = BetweenFromList(ATTX, "<senderRS>", "</senderRS>",, GetType(String))
    '        Dim TX_Confirmations As ULong = BetweenFromList(ATTX, "<confirmations>", "</confirmations>",, GetType(ULong))


    '        Dim BLSTX As S_PFPAT_TX = New S_PFPAT_TX With {
    '                .Type = TX_Type,
    '                .Timestamp = TX_Timestamp,
    '                .RecipientID = TX_RecipientID,
    '                .RecipientRS = TX_RecipientRS,
    '                .AmountNQT = TX_AmountNQT,
    '                .FeeNQT = TX_FeeNQT,
    '                .TransactionID = TX_Transaction,
    '                .Attachment = TX_Attachment,
    '                .SenderID = TX_SenderID,
    '                .SenderRS = TX_SenderRS,
    '                .Confirmations = TX_Confirmations
    '            }
    '        BLSTXs.Add(BLSTX)

    '    Next

    '    BLSTXs = BLSTXs.OrderBy(Function(BLSAT_TX As S_PFPAT_TX) BLSAT_TX.Timestamp).ToList

    '    Return BLSTXs

    'End Function

    'Function GetCurrentBLSATStatus(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As String

    '    Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

    '    NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

    '    NU_BLSAT_TXList.Reverse()


    '    For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

    '        If BLSATTX.Type = "BLSTX" Then

    '            If BLSATTX.Attachment.Contains("<colBuyAmount>") Then

    '                Return "OPEN"

    '            ElseIf BLSATTX.Attachment.Contains("accepted") Then

    '                Return "RESERVED"

    '            ElseIf BLSATTX.Attachment.Contains("finished") Then

    '                Return "CLOSED"

    '            End If

    '        End If

    '    Next

    '    Return "CLOSED"

    'End Function
    'Function GetCurrentBLSATXItem(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As String

    '    Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

    '    NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

    '    NU_BLSAT_TXList.Reverse()


    '    For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

    '        If BLSATTX.Type = "BLSTX" Then

    '            If BLSATTX.Attachment.Contains("<xItem>") Then

    '                Return Between(BLSATTX.Attachment, "<xItem>", "</xItem>", GetType(String))

    '            End If

    '        End If

    '    Next

    '    Return ""

    'End Function
    'Function GetCurrentBLSATXAmount(ByVal BLSAT_TXList As List(Of S_PFPAT_TX)) As Double

    '    Dim NU_BLSAT_TXList As List(Of S_PFPAT_TX) = New List(Of S_PFPAT_TX)

    '    NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

    '    NU_BLSAT_TXList.Reverse()


    '    For Each BLSATTX As S_PFPAT_TX In NU_BLSAT_TXList

    '        If BLSATTX.Type = "BLSTX" Then

    '            If BLSATTX.Attachment.Contains("<xAmount>") Then

    '                Return Convert.ToDouble(Between(BLSATTX.Attachment, "<xAmount>", "</xAmount>", GetType(String)))

    '            End If

    '        End If

    '    Next

    '    Return 0.0

    'End Function

    Function SendBillingInfos(ByVal RecipientAddress As String, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True) As String
        Dim RecipientID As ULong = ClsReedSolomon.Decode(RecipientAddress)
        Return SendBillingInfos(RecipientID, Message, ShowPINForm, Encrypt)
    End Function

    Function SendBillingInfos(ByVal RecipientID As ULong, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True) As String
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

        Dim Masterkeys As List(Of String) = GetPassPhrase()

        If Masterkeys.Count > 0 Then
            Dim Response As String = SignumAPI.SendMessage(Masterkeys(0), Masterkeys(2), RecipientID, Message,, Encrypt)

            Dim JSON As ClsJSON = New ClsJSON
            Dim RespList As Object = JSON.JSONRecursive(Response)
            Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

            If Error0.GetType.Name = GetType(Boolean).Name Then
                'TX OK
            ElseIf Error0.GetType.Name = GetType(String).Name Then
                Return Application.ProductName + "-error in SendBillingInfos(1): -> " + vbCrLf + Response
            End If


            If Response.Contains(Application.ProductName + "-error") Then
                Return Application.ProductName + "-error in SendBillingInfos(2): -> " + vbCrLf + Response
            Else

                Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, Masterkeys(1))
                Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                If TX.Contains(Application.ProductName + "-error") Then
                    Return Application.ProductName + "-error in SendBillingInfos(3): -> " + vbCrLf + TX
                Else
                    Return TX
                End If

            End If

        Else

            Dim Returner As String = Application.ProductName + "-warning in SendBillingInfos(4): -> no Keys"

            If ShowPINForm Then

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then

                    Dim Response As String = SignumAPI.SendMessage(PinForm.PublicKey, PinForm.AgreeKey, RecipientID, Message,, Encrypt)

                    If Response.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in SendBillingInfos(5): -> " + vbCrLf + Response
                    Else

                        Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                        Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If TX.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in SendBillingInfos(6): -> " + vbCrLf + TX
                        Else
                            Return TX
                        End If

                    End If

                Else
                    Return Returner
                End If

            Else
                Return Returner
            End If

        End If

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
    Function GetCandles(ByVal XItem As String, ByVal Orders As List(Of ClsDEXContract.S_Order), ByVal ChartRangeDays As Integer, Optional ByVal ResolutionMin As Integer = 1) As List(Of S_Candle)

        Orders = Orders.OrderBy(Function(Order) Order.StartTimestamp).ToList

        Dim GridNow As Date = GetGriddedTime(Now, ResolutionMin)

        Dim RangeStart As Date = GridNow.AddDays(-ChartRangeDays)

        Dim CandleList As List(Of S_Candle) = New List(Of S_Candle)

        Dim OldCandle As S_Candle = New S_Candle

        While Not RangeStart.ToLongDateString + " " + RangeStart.ToShortTimeString = GridNow.ToLongDateString + " " + GridNow.ToShortTimeString

            Dim CurrentDay As Date = RangeStart.AddMinutes(ResolutionMin)

            Dim T_TickTrades As List(Of S_Trade) = New List(Of S_Trade)

            Dim FoundSome As Boolean = False
            For Each Order As ClsDEXContract.S_Order In Orders

                If Order.Status = ClsDEXContract.E_Status.OPEN Or Order.Status = ClsDEXContract.E_Status.CANCELED Or Not Order.XItem.Contains(XItem) Then
                    Continue For
                End If

                Dim TempDay As Date = ConvertULongMSToDate(Order.EndTimestamp)

                If TempDay > RangeStart And TempDay < CurrentDay Then

                    FoundSome = True
                    Dim STrade As S_Trade = New S_Trade

                    STrade.OpenDat = CDate(RangeStart.ToShortDateString + " " + RangeStart.ToShortTimeString)
                    STrade.CloseDat = CDate(CurrentDay.ToShortDateString + " " + CurrentDay.ToShortTimeString)

                    STrade.Price = Order.Price
                    STrade.Quantity = Order.Amount

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

    'Function CheckForUTX(Optional ByVal SenderRS As String = "", Optional ByVal RecipientRS As String = "", Optional ByVal SearchAttachmentHEX As String = "") As Boolean

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

    '    Dim T_UTXList As List(Of List(Of String)) = SignumAPI.GetUnconfirmedTransactions()

    '    For Each UTX In T_UTXList

    '        Dim TX_SenderRS As String = BetweenFromList(UTX, "<senderRS>", "</senderRS>",, GetType(String))
    '        Dim TX_RecipientRS As String = BetweenFromList(UTX, "<recipientRS>", "</recipientRS>",, GetType(String))

    '        If (TX_SenderRS = SenderRS Or SenderRS = "") And (RecipientRS = TX_RecipientRS Or RecipientRS = "") And Not (SenderRS = "" And RecipientRS = "") Then

    '            Dim T_Msg As String = BetweenFromList(UTX, "<message>", "</message>",, GetType(String))
    '            If T_Msg.ToLower.Trim = SearchAttachmentHEX.ToLower.Trim Or SearchAttachmentHEX = "" Then
    '                Return True
    '            End If

    '        End If


    '    Next

    '    Return False

    'End Function

    'Function CheckForTX(ByVal SenderRS As String, ByVal RecipientRS As String, ByVal FromTimestamp As ULong, ByVal SearchAttachment As String) As Boolean

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI
    '    SignumAPI.DEXATList = DEXATList
    '    Dim AccountAddressList As List(Of String) = New List(Of String)

    '    AccountAddressList = SignumAPI.RSConvert(SenderRS)

    '    Dim T_Account As String = Between(AccountAddressList(0), "<account>", "</account>")

    '    Dim TXList As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(T_Account, FromTimestamp, True)

    '    Dim AnswerTX As List(Of String) = New List(Of String)

    '    For Each T_TX In TXList

    '        Dim TX_SenderRS As String = BetweenFromList(T_TX, "<senderRS>", "</senderRS>",, GetType(String))
    '        Dim TX_RecipientRS As String = BetweenFromList(T_TX, "<recipientRS>", "</recipientRS>",, GetType(String))


    '        If TX_SenderRS = SenderRS And RecipientRS = TX_RecipientRS And Not (SenderRS = "" And RecipientRS = "") Then

    '            Dim T_Msg As String = BetweenFromList(T_TX, "<message>", "</message>",, GetType(String))

    '            If T_Msg.Trim = "" Then
    '                T_Msg = BetweenFromList(T_TX, "<method>", "</method>",, GetType(String))
    '            End If

    '            If T_Msg.ToLower.Trim = SearchAttachment.ToLower.Trim Then
    '                Return True
    '            End If

    '        End If

    '    Next

    '    Return False

    'End Function
    'Function CheckForTX(ByVal SenderRS As String, ByVal RecipientRS As String, ByVal FromTimestamp As ULong, ByVal SearchAttachment As ULong) As Boolean

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
    '    Dim CheckAttachment As String = SignumAPI.ULngList2DataStr(New List(Of ULong)({SearchAttachment}))

    '    Return CheckForTX(SenderRS, RecipientRS, FromTimestamp, CheckAttachment)

    'End Function

    'Function CheckATforTX(ByVal ATID As ULong) As Boolean

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
    '    Dim T_Timestamp As ULong = SignumAPI.TimeToUnix(Now.AddDays(-1))

    '    Dim ATTXList As List(Of S_PFPAT_TX) = GetAccountTXList(ATID,, T_Timestamp)

    '    ATTXList.Reverse()

    '    If ATTXList.Count > 0 Then
    '        If ATTXList(0).Type = "ResponseOrder" Then
    '            Return True
    '        End If
    '    Else

    '    End If

    '    Return False

    'End Function

    Function CheckBillingInfosAlreadySend(ByVal OpenDEXContract As ClsDEXContract) As String
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        SignumAPI.DEXATList = DEXATList

        Dim SignumNET As ClsSignumNET = New ClsSignumNET()

        Dim T_UTXList As List(Of List(Of String)) = SignumAPI.GetUnconfirmedTransactions()
        T_UTXList.Reverse()

        For Each UTX As List(Of String) In T_UTXList

            'Dim TX As String = BetweenFromListi(UTX, "<transaction>", "</transaction>")
            Dim Sender As String = GetStringBetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = GetStringBetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If Sender = OpenDEXContract.CurrentSellerAddress And Recipient = OpenDEXContract.CurrentBuyerAddress Then

                Dim Data As String = GetStringBetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = GetStringBetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else

                    Dim VSAcc As ULong = OpenDEXContract.CurrentBuyerID
                    If GetAccountIDFromRS(TBSNOAddress.Text.Trim) = VSAcc Then
                        VSAcc = OpenDEXContract.CurrentSellerID
                    End If

                    Dim PubKey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(VSAcc.ToString)

                    Dim DecryptedMsg As String = SignumNET.DecryptFrom(PubKey, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                            Dim DCAT As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
                            Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")

                            If DCAT = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                                Return DecryptedMsg
                            End If

                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(1): -> " + DecryptedMsg
                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                            Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(1): -> " + DecryptedMsg
                        End If

                    Else
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(1): -> " + DecryptedMsg
                    End If

                End If

            ElseIf Sender = OpenDEXContract.CurrentBuyerAddress And Recipient = OpenDEXContract.CurrentSellerAddress Then

                Dim Data As String = GetStringBetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = GetStringBetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else

                    Dim VSAcc As ULong = OpenDEXContract.CurrentSellerID
                    If GetAccountIDFromRS(TBSNOAddress.Text.Trim) = VSAcc Then
                        VSAcc = OpenDEXContract.CurrentBuyerID
                    End If

                    Dim PubKey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(VSAcc.ToString)



                    Dim DecryptedMsg As String = SignumNET.DecryptFrom(PubKey, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                            Dim DCAT As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
                            Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")

                            If DCAT = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                                Return DecryptedMsg
                            End If

                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                            Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(2): -> " + DecryptedMsg
                        ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                            Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(2): -> " + DecryptedMsg
                        End If
                    Else
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(2): -> " + DecryptedMsg
                    End If

                End If

            End If

        Next


        Dim TXList As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(OpenDEXContract.CurrentBuyerID, OpenDEXContract.CurrentTimestamp)
        TXList.Reverse()

        For Each SearchTX As List(Of String) In TXList

            Dim TX As ULong = GetULongBetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim SenderRS As String = GetStringBetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim RecipientRS As String = GetStringBetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If SenderRS = OpenDEXContract.CurrentSellerAddress And RecipientRS = OpenDEXContract.CurrentBuyerAddress Then

                Dim DecryptedMsg As String = SignumAPI.ReadMessage(TX, GetAccountIDFromRS(TBSNOAddress.Text))

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then
                        Dim DCATRS As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
                        Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")

                        If DCATRS = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                        Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                    Else
                        Return DecryptedMsg
                    End If

                Else
                    Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                End If

            ElseIf SenderRS = OpenDEXContract.CurrentBuyerAddress And RecipientRS = OpenDEXContract.CurrentSellerAddress Then

                Dim DecryptedMsg As String = SignumAPI.ReadMessage(TX, GetAccountIDFromRS(TBSNOAddress.Text))

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

                        Dim DCATRS As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
                        Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")

                        If DCATRS = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                        Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                    Else
                        Return DecryptedMsg
                    End If

                Else
                    Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                End If

            End If

        Next

        Return ""

    End Function

    'Function CheckPaymentAlreadySend(ByVal OpenDEXContract As ClsDEXContract) As String

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
    '    SignumAPI.DEXATList = DEXATList

    '    Dim SignumNET As ClsSignumNET = New ClsSignumNET()

    '    Dim T_UTXList As List(Of List(Of String)) = SignumAPI.GetUnconfirmedTransactions()
    '    T_UTXList.Reverse()

    '    Dim TXList As List(Of List(Of String)) = SignumAPI.GetAccountTransactions(OpenDEXContract.CurrentSellerID, OpenDEXContract.CurrentTimestamp)
    '    TXList.Reverse()

    '    For Each UTX As List(Of String) In T_UTXList

    '        'Dim TXID As ulong = BetweenFromList(UTX, "<transaction>", "</transaction>",,GetType(ulong))
    '        Dim SenderRS As String = GetStringBetweenFromList(UTX, "<senderRS>", "</senderRS>")
    '        Dim RecipientRS As String = GetStringBetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

    '        If SenderRS = OpenDEXContract.CurrentBuyerAddress And RecipientRS = OpenDEXContract.CurrentSellerAddress Then

    '            Dim Data As String = GetStringBetweenFromList(UTX, "<data>", "</data>")
    '            Dim Nonce As String = GetStringBetweenFromList(UTX, "<nonce>", "</nonce>")

    '            If Data.Trim = "" Or Nonce.Trim = "" Then
    '                'no
    '            Else

    '                Dim PubKey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(OpenDEXContract.CurrentSellerAddress)

    '                Dim DecryptedMsg As String = SignumNET.DecryptFrom(PubKey, Data, Nonce)

    '                If Not MessageIsHEXString(DecryptedMsg) Then

    '                    If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

    '                        Dim DCAT As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
    '                        Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")


    '                        If DCAT = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
    '                            Return DecryptedMsg
    '                        End If

    '                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
    '                        Return Application.ProductName + "-error in CheckPaymentAlreadySend(): -> " + DecryptedMsg
    '                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
    '                        Return Application.ProductName + "-warning in CheckPaymentAlreadySend(): -> " + DecryptedMsg
    '                    End If

    '                    Return DecryptedMsg
    '                Else
    '                    Return Application.ProductName + "-error in CheckPaymentAlreadySend(): -> " + DecryptedMsg
    '                End If

    '            End If

    '        End If

    '    Next



    '    For Each SearchTX As List(Of String) In TXList

    '        Dim TXID As ULong = GetULongBetweenFromList(SearchTX, "<transaction>", "</transaction>")
    '        Dim SenderRS As String = GetStringBetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
    '        Dim RecipientRS As String = GetStringBetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

    '        If SenderRS = OpenDEXContract.CurrentBuyerAddress And RecipientRS = OpenDEXContract.CurrentSellerAddress Then

    '            Dim DecryptedMsg As String = SignumAPI.ReadMessage(TXID, GetAccountIDFromRS(TBSNOAddress.Text))

    '            If DecryptedMsg.Contains("AT=") And DecryptedMsg.Contains("TX=") Then

    '                Dim DCATRS As String = GetStringBetween(DecryptedMsg, "AT=", " TX=")
    '                Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "TX=", " ")

    '                If DCATRS = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
    '                    Return DecryptedMsg
    '                End If
    '            ElseIf DecryptedMsg.Contains("-error") Then
    '                If GetINISetting(E_Setting.InfoOut, False) Then
    '                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
    '                    Out.ErrorLog2File(DecryptedMsg)
    '                End If
    '            ElseIf DecryptedMsg.Contains("-warning") Then
    '                If GetINISetting(E_Setting.InfoOut, False) Then
    '                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
    '                    Out.WarningLog2File(DecryptedMsg)
    '                End If

    '            End If

    '        End If

    '    Next

    '    Return ""


    'End Function


    Function SetAutoinfoTX2INI(ByVal TXID As ULong) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")

        If AutoinfoTXStr.Trim = "" Then
            AutoinfoTXStr = TXID.ToString + ";"
        ElseIf AutoinfoTXStr.Contains(";") Then
            AutoinfoTXStr += TXID.ToString + ";"
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return True

    End Function
    Function GetAutoinfoTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")
        Dim AutoinfoTXList As List(Of String) = New List(Of String)

        If AutoinfoTXStr.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(AutoinfoTXStr.Split(";"c))
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
            If TXID.ToString = AutoinfoTX Then
                Return True
            End If
        Next

        Return False

    End Function
    Function DelAutoinfoTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")
        Dim AutoinfoTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutoinfoTXStr.Contains(TXID.ToString + ";") Then
            Returner = True
            AutoinfoTXStr = AutoinfoTXStr.Replace(TXID.ToString + ";", "")
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return Returner

    End Function


    Function SetAutosignalTX2INI(ByVal TXID As ULong) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")

        If AutosignalTXStr.Trim = "" Then
            AutosignalTXStr = TXID.ToString + ";"
        ElseIf AutosignalTXStr.Contains(";") Then
            AutosignalTXStr += TXID.ToString + ";"
        End If

        SetINISetting(E_Setting.AutoSignalTransactions, AutosignalTXStr.Trim)

        Return True

    End Function
    Function GetAutosignalTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Dim AutosignalTXList As List(Of String) = New List(Of String)

        If AutosignalTXStr.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(AutosignalTXStr.Split(";"c))
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
            If TXID.ToString = AutosignalTX Then
                Return True
            End If
        Next

        Return False

    End Function
    Function DelAutosignalTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")
        Dim AutosignalTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutosignalTXStr.Contains(TXID.ToString + ";") Then
            Returner = True
            AutosignalTXStr = AutosignalTXStr.Replace(TXID.ToString + ";", "")
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
    Function ConvertULongMSToDate(ByVal LongS As ULong) As Date
        Dim StartDate As Date = New DateTime(2014, 8, 11, 4, 0, 0)
        StartDate = StartDate.AddSeconds(Convert.ToDouble(LongS))
        Return StartDate
    End Function

#End Region

#End Region

#Region "Multithreadings"

    Structure S_RelevantMsg
        Dim Setted As Boolean
        Dim RelevantMessage As String
    End Structure

    Sub GetNuBlock(ByVal Node As Object)
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node.ToString)
        NuBlock = SignumAPI.GetCurrentBlock
    End Sub

    Sub BlockFeeThread(ByVal Node As Object)

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node.ToString)
        MultiInvoker(StatusBlockLabel, "Text", "New Blockheight: " + Block.ToString)

        'Fee = SignumAPI.GetTXFee
        'MultiInvoker(StatusFeeLabel, "Text", "Current Slotfee: " + String.Format("{0:#0.00000000}", Fee))

        UTXList = SignumAPI.C_UTXList

    End Sub

    ''' <summary>
    ''' The Thread who coordinates (loadbalance) the API Request subthreads
    ''' </summary>
    Sub GetThread()

        Dim NuNodeList As List(Of String) = New List(Of String)(NodeList.ToArray)

        Dim WExitCom As Boolean = True
        While WExitCom

            Threading.Thread.Sleep(1)

            'Try

            Dim Cnt As Integer = APIRequestList.Count

            For i As Integer = 0 To Cnt - 1

                Dim Request As S_APIRequest = New S_APIRequest

                If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                    Continue While
                End If


                Try
                    Request = APIRequestList(i)
                Catch ex As Exception
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While1): -> " + ex.Message)
                    End If

                End Try


                If Request.Command = "Exit()" Then

                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
                        IOut.Info2File(Application.ProductName + "-info from GetThread(): -> Exit()")
                    End If

                    Exit While
                End If

                If Request.Status = "Wait..." Then

                    If NuNodeList.Count > 0 Then
                        Request.Node = NuNodeList(0)

                        Request.RequestThread = New Threading.Thread(AddressOf SubGetThread)
                        Request.Status = "Requesting..."
                        Request.RequestThread.Start(New List(Of Object)({i, Request}))

                        NuNodeList.RemoveAt(0)

                        If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                            Continue While
                        End If

                        'Try
                        APIRequestList(i) = Request
                        'Catch ex As Exception
                        '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        '    Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While2): -> " + ex.Message)
                        'End Try

                        Continue For

                    End If

                ElseIf Request.Status = "Not Ready..." Then


                    NuNodeList.Remove(Request.Node)
                    NodeList.Remove(Request.Node) 'remove from list
                    NodeList.Add(Request.Node) 'set node to last place

                    If NuNodeList.Count > 0 Then
                        Request.Node = NuNodeList(0)
                    Else
                        LoadRunning = False
                    End If

                    Request.RequestThread = New Threading.Thread(AddressOf SubGetThread)
                    Request.Status = "Requesting..."

                    If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                        Continue While
                    End If

                    Request.RequestThread.Start(New List(Of Object)({i, Request}))

                    'Try
                    APIRequestList(i) = Request
                    'Catch ex As Exception
                    '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    '    Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While3): -> " + ex.Message)
                    'End Try

                ElseIf Request.Status = "Requesting..." Then
                    'loadbalancing
                    NuNodeList.Remove(Request.Node)
                ElseIf Request.Status = "Responsed" Then

                    Request.Status = "Ready"

                    Dim founded As Boolean = False

                    For Each T_Node In NuNodeList
                        If T_Node = Request.Node Then
                            founded = True

                            If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                                Continue While
                            End If

                            'Try
                            APIRequestList(i) = Request
                            'Catch ex As Exception
                            '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                            '    Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While4): -> " + ex.Message)
                            'End Try

                            Exit For
                        End If
                    Next

                    If Not founded Then
                        'loadbalancing
                        NuNodeList.Add(Request.Node)
                    End If

                End If

            Next

            'Catch ex As Exception
            '    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            '    Out.ErrorLog2File(Application.ProductName + "-error in GetThread(While): -> " + ex.Message)

            '    'MultiInvoker(StatusLabel, "Text", "GetThread(): " + ex.Message)
            'End Try

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

                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
                    IOut.Info2File(Application.ProductName + "-info from GetThread(): -> DelIDX <> -1 -> Exit()")
                End If

            End If

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in GetThread(): -> " + ex.Message)
            End If

        End Try

    End Sub

    ''' <summary>
    ''' The SubThread who process the request
    ''' </summary>
    ''' <param name="T_Input">The input to work with Input(0) = List-Index; Input(1) = APIRequest</param>
    Sub SubGetThread(ByVal T_Input As Object)

        'Try


        Dim Input As List(Of Object) = New List(Of Object)

        If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
            Input = DirectCast(T_Input, List(Of Object))
        Else
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in SubGetThread(DirectCast): -> " + T_Input.GetType.Name)
            End If
        End If


        Dim Index As Integer = DirectCast(Input(0), Integer)
        Dim Request As S_APIRequest = DirectCast(Input(1), S_APIRequest)

        Dim Command As String = Request.Command

        Dim Parameter As String = GetStringBetween(Command, "(", ")", True)
        Command = Command.Replace(Parameter, "")
        Dim ParameterList As List(Of String) = New List(Of String)

        If Parameter.Contains(",") Then
            ParameterList.AddRange(Parameter.Split(","c))
        Else
            ParameterList.Add(Parameter)
        End If

        Dim T_ATID As ULong = ULong.Parse(ParameterList(0))

        Select Case Command

            Case "GetDetails()"

                Dim T_Contract As ClsDEXContract

                If IsNothing(Request.CommandAttachment) Then
                    T_Contract = New ClsDEXContract(Me, Request.Node, T_ATID, Now.AddDays(-30))
                Else

                    Dim HistoryOrdersXML As String = Request.CommandAttachment.ToString
                    Dim HistoryOrderXMLList As List(Of String) = New List(Of String)

                    Dim WExit As Boolean = False
                    Dim Cnter As ULong = 0
                    While Not WExit

                        Dim HistoryOrderXML As String = ClsDEXNET.GetXMLMessage(HistoryOrdersXML, Cnter.ToString)

                        If HistoryOrderXML.Trim = "" Then
                            WExit = True
                        Else
                            HistoryOrderXMLList.Add(HistoryOrderXML)
                        End If

                        Cnter += 1UL

                    End While

                    Dim HistoryOrderList As List(Of ClsDEXContract.S_Order) = New List(Of ClsDEXContract.S_Order)
                    For Each HistoryOrderXML As String In HistoryOrderXMLList

                        Dim T_HistoryOrder As ClsDEXContract.S_Order = New ClsDEXContract.S_Order
                        T_HistoryOrder.CreationTransaction = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "CreationTransaction"))
                        T_HistoryOrder.LastTransaction = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "LastTransaction"))
                        T_HistoryOrder.Confirmations = 0UL
                        T_HistoryOrder.WasSellOrder = Convert.ToBoolean(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "WasSellOrder"))
                        T_HistoryOrder.StartTimestamp = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "StartTimestamp"))
                        T_HistoryOrder.EndTimestamp = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "EndTimestamp"))
                        T_HistoryOrder.SellerID = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "SellerID"))
                        T_HistoryOrder.SellerRS = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(T_HistoryOrder.SellerID)

                        T_HistoryOrder.BuyerID = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "BuyerID"))
                        T_HistoryOrder.BuyerRS = ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(T_HistoryOrder.BuyerID)

                        T_HistoryOrder.Amount = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Amount")))
                        T_HistoryOrder.Collateral = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Collateral")))
                        T_HistoryOrder.XAmount = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "XAmount")))
                        T_HistoryOrder.XItem = ClsDEXNET.GetXMLMessage(HistoryOrderXML, "XItem")
                        T_HistoryOrder.Price = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Price")))

                        Dim T_StrStatus As String = ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Status")

                        Select Case T_StrStatus
                            Case ClsDEXContract.E_Status.CLOSED.ToString
                                T_HistoryOrder.Status = ClsDEXContract.E_Status.CLOSED
                            Case ClsDEXContract.E_Status.CANCELED.ToString
                                T_HistoryOrder.Status = ClsDEXContract.E_Status.CANCELED
                            Case Else
                                T_HistoryOrder.Status = ClsDEXContract.E_Status.CANCELED
                        End Select

                        HistoryOrderList.Add(T_HistoryOrder)

                    Next

                    T_Contract = New ClsDEXContract(Me, Request.Node, T_ATID, HistoryOrderList)

                End If


                If T_Contract.IsReady Then
                    If T_Contract.IsDEXContract Then
                        Request.Result = T_Contract
                        Request.Status = "Responsed"
                    Else
                        Request.Result = T_Contract.ID.ToString + "," + T_Contract.Address + ",False"
                        Request.Status = "Responsed"
                    End If
                Else
                    Request.Status = "Not Ready..."
                End If

            Case "RefreshContract()"

                Dim T_Contract As ClsDEXContract = DirectCast(Request.CommandAttachment, ClsDEXContract)

                Dim FirstTransactionTimestamp As ULong = ClsSignumAPI.TimeToUnix(Now)

                If T_Contract.ContractOrderHistoryList.Count > 0 Then
                    FirstTransactionTimestamp = T_Contract.ContractOrderHistoryList(0).StartTimestamp
                End If

                If FirstTransactionTimestamp > ClsSignumAPI.TimeToUnix(Now.AddDays(-CoBxChartSelectedItem)) Then
                    T_Contract = New ClsDEXContract(Me, Request.Node, T_ATID, Now.AddDays(-CoBxChartSelectedItem))
                Else
                    T_Contract.Node = Request.Node
                    T_Contract.Refresh()
                End If


                If T_Contract.IsReady Then
                    Request.Status = "Responsed"
                Else
                    Request.Status = "Not Ready..."
                End If

                Request.Result = T_Contract

            Case Else

#Region "deprecaded"
                '                Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Request.Node)
                '                Dim ATStrList As List(Of String) = SignumAPI.GetATDetails(T_ATID)

                '                If Not ATStrList.Count = 0 Then

                '                    Dim ATID As ULong = BetweenFromList(ATStrList, "<at>", "</at>",, GetType(ULong))

                '                    'Try
                '                    '    ATID =
                '                    'Catch ex As Exception
                '                    '    If GetINISetting(E_Setting.InfoOut, False) Then
                '                    '        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                '                    '        Out.ErrorLog2File(Application.ProductName + "-error in SubGetThread(AT): -> " + ex.Message)
                '                    '    End If
                '                    'End Try

                '                    Dim ATRS As String = BetweenFromList(ATStrList, "<atRS>", "</atRS>",, GetType(String))
                '                    Dim REFMC As String = BetweenFromList(ATStrList, "<referenceMachineCode>", "</referenceMachineCode>",, GetType(String))

                '                    If REFMC.Trim <> "True" And REFMC.Trim <> "False" Then
                '                        REFMC = "False"
                '                    End If

                '                    Dim ReferenceMachineCode As Boolean = CBool(REFMC)

                '                    If ReferenceMachineCode Then

                '#Region "get AT details"

                '                        Dim CreatorID As ULong = BetweenFromList(ATStrList, "<creator>", "</creator>",, GetType(ULong))

                '                        'Try
                '                        '    CreatorID = ULong.Parse()
                '                        'Catch ex As Exception
                '                        '    If GetINISetting(E_Setting.InfoOut, False) Then
                '                        '        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                '                        '        Out.ErrorLog2File(Application.ProductName + "-error in SubGetThread(CreatorID): -> " + ex.Message)
                '                        '    End If
                '                        'End Try


                '                        Dim CreatorRS As String = BetweenFromList(ATStrList, "<creatorRS>", "</creatorRS>",, GetType(String))
                '                        Dim Name As String = BetweenFromList(ATStrList, "<name>", "</name>",, GetType(String))
                '                        Dim Description As String = BetweenFromList(ATStrList, "<description>", "</description>",, GetType(String))

                '                        Dim Balance As ULong = BetweenFromList(ATStrList, "<balanceNQT>", "</balanceNQT>",, GetType(ULong))


                '                        Dim BalDbl As Double = ClsSignumAPI.Planck2Dbl(Balance)

                '                        Dim MachineData As String = BetweenFromList(ATStrList, "<machineData>", "</machineData>",, GetType(String))

                '                        Dim MachineDataULongList As List(Of ULong) = ClsSignumAPI.DataStr2ULngList(MachineData)

                '                        '0 = Address Initiator = null
                '                        '1 = Address Responder = null

                '                        '2 = Long InitiatorsCollateral 
                '                        '3 = Long RespondersCollateral
                '                        '4 = Long BuySellAmount

                '                        '5 = Boolean SellOrder = False

                '                        Dim Initiator As ULong = 0
                '                        Dim InitiatorRS As String = ""
                '                        Dim Responser As ULong = 0
                '                        Dim ResponserRS As String = ""

                '                        Dim InitiatorCollateral As Double = 0.0
                '                        Dim ResponserCollateral As Double = 0.0

                '                        Dim BuySellAmount As Double = 0.0

                '                        Dim SellOrder As Boolean = False

                '                        If MachineDataULongList.Count > 0 Then
                '                            Initiator = MachineDataULongList(0)

                '                            If Initiator = ULong.Parse(0) Then
                '                                'InitiatorRS = ""
                '                            Else
                '                                InitiatorRS = BetweenFromList(SignumAPI.RSConvert(Initiator.ToString), "<accountRS>", "</accountRS>",, GetType(String))
                '                            End If

                '                            Responser = MachineDataULongList(1)

                '                            If Responser = ULong.Parse(0) Then
                '                                ResponserRS = ""
                '                            Else
                '                                ResponserRS = BetweenFromList(SignumAPI.RSConvert(Responser.ToString), "<accountRS>", "</accountRS>",, GetType(String))
                '                            End If

                '                            InitiatorCollateral = ClsSignumAPI.Planck2Dbl(MachineDataULongList(2)) '- ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)
                '                            ResponserCollateral = ClsSignumAPI.Planck2Dbl(MachineDataULongList(3)) '- ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)

                '                            BuySellAmount = ClsSignumAPI.Planck2Dbl(MachineDataULongList(4)) '- ClsSignumAPI.Planck2Dbl(ClsSignumAPI._GasFeeNQT)

                '                            SellOrder = False

                '                            If MachineDataULongList(7) = ULong.Parse(1) Then
                '                                SellOrder = True
                '                            End If

                '                        End If


                '                        Dim Frozen As Boolean = BetweenFromList(ATStrList, "<frozen>", "</frozen>",, GetType(Boolean))
                '                        Dim Running As Boolean = BetweenFromList(ATStrList, "<running>", "</running>",, GetType(Boolean))
                '                        Dim Stopped As Boolean = BetweenFromList(ATStrList, "<stopped>", "</stopped>",, GetType(Boolean))
                '                        Dim Finished As Boolean = BetweenFromList(ATStrList, "<finished>", "</finished>",, GetType(Boolean))
                '                        Dim Dead As Boolean = BetweenFromList(ATStrList, "<dead>", "</dead>",, GetType(Boolean))

                '                        Dim SBLSAT As PFPForm.S_PFPAT = New PFPForm.S_PFPAT With {
                '                            .ATID = ATID,
                '                            .ATRS = ATRS,
                '                            .CreatorID = CreatorID,
                '                            .CreatorRS = CreatorRS,
                '                            .Description = Description,
                '                            .Name = Name,
                '                            .Sellorder = SellOrder,
                '                            .InitiatorRS = InitiatorRS,
                '                            .InitiatorsCollateral = InitiatorCollateral,
                '                            .ResponsersCollateral = ResponserCollateral,
                '                            .BuySellAmount = BuySellAmount,
                '                            .ResponserRS = ResponserRS,
                '                            .Balance = Balance,
                '                            .Frozen = Frozen,
                '                            .Running = Running,
                '                            .Stopped = Stopped,
                '                            .Finished = Finished,
                '                            .Dead = Dead
                '                        }


                '#End Region

                '#Region "get AT TXs"
                '                        Dim UseBuffer As Boolean = False
                '                        If DEXATList.Count > 0 Then
                '                            UseBuffer = True
                '                        End If


                '                        Dim BLSTXs As List(Of S_PFPAT_TX) = GetAccountTXList(T_ATID, Request.Node,, UseBuffer) 'TODO: FromTimestamp

                '                        SBLSAT.AT_TXList = New List(Of PFPForm.S_PFPAT_TX)(BLSTXs.ToArray)
                '                        SBLSAT.Status = GetCurrentBLSATStatus(BLSTXs)
                '                        SBLSAT.XItem = GetCurrentBLSATXItem(BLSTXs)
                '                        SBLSAT.XAmount = GetCurrentBLSATXAmount(BLSTXs)

                '#End Region

                '#Region "Convert AT TXs to Orders"
                '                        SBLSAT.AT_OrderList = ConvertTXs2Orders(SBLSAT)
                '#End Region

                '                        Request.Result = SBLSAT
                '                        Request.Status = "Responsed"
                '                    Else
                '                        Request.Result = ATID.ToString + "," + ATRS + ",False"
                '                        Request.Status = "Responsed"
                '                    End If
                '                Else
                '                    Request.Status = "Not Ready..."
                '                End If

#End Region

        End Select

        APIRequestList(Index) = Request

        'Catch ex As Exception
        '    If GetINISetting(E_Setting.InfoOut, False) Then
        '        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
        '        Out.ErrorLog2File(Application.ProductName + "-error in SubGetThread(M): -> " + ex.Message)
        '    End If

        'End Try

    End Sub

    Function MultithreadMonitor() As Boolean

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
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in MultithreadMonitor(): -> " + ex.Message)
            End If

        End Try

        Return False

    End Function

#End Region

#Region "Multiinvoker"

    Public Enum E_MainFormControls
        LabDebug = 0
        StatusLabel = 1
    End Enum

    Public Delegate Sub MultiDelegate(ByVal params As List(Of Object))

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
                Dim paramList As List(Of Object) = New List(Of Object) From {
                    obj,
                    prop,
                    val
                }

                Me.Invoke(New MultiDelegate(AddressOf Invoker), paramList)
                ' Else
                'SetPropertyValueByName(obj, prop, val)
            End If


        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in MultiInvoker(): -> " + ex.Message)
            End If

        End Try

    End Sub


    Sub MultiInvoker(ByVal MainFormControl As E_MainFormControls, ByVal ControlProperty As Object, ByVal Value As Object)

        Dim T_Control As Object = Nothing
        Dim FoundControl As Boolean = False

        Select Case MainFormControl
            Case E_MainFormControls.LabDebug
                T_Control = LabDebug
                FoundControl = True
            Case E_MainFormControls.StatusLabel
                T_Control = StatusLabel
                FoundControl = True
        End Select

        If FoundControl Then
            MultiInvoker(T_Control, ControlProperty, Value)
        End If

    End Sub


    Sub Invoker(ByVal params As List(Of Object))
        SetPropertyValueByName(params.Item(0), params.Item(1).ToString, params.Item(2))
    End Sub
    Public Function SetPropertyValueByName(ControlObject As Object, PropertyName As String, value As Object) As Boolean

        Try

            Dim T_PropertyInfo As Reflection.PropertyInfo = ControlObject.GetType().GetProperty(PropertyName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            If IsNothing(T_PropertyInfo) Then
                Return False
            End If

            If ControlObject.GetType.Name = GetType(ListView).Name And PropertyName = "Items" Then

                Dim T_LV As ListView = DirectCast(ControlObject, ListView)
                Dim T_Values As List(Of Object) = New List(Of Object)

                If value.GetType.Name = GetType(List(Of Object)).Name Then
                    T_Values = DirectCast(value, List(Of Object))
                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in SetPropertyValueByName(DirectCast): -> " + value.GetType.Name)
                    End If
                End If

                If T_Values.Count > 0 Then

                    If T_Values(0).ToString = "Clear" Then
                        T_LV.Items.Clear()
                    ElseIf T_Values(0).ToString = "Add" Then
                        Dim Val As ListViewItem = DirectCast(T_Values(1), ListViewItem)
                        T_LV.Items.Add(Val)
                    ElseIf T_Values(0).ToString = "Insert" Then
                        Dim Idx As Integer = Convert.ToInt32(T_Values.Item(1))
                        Dim Val As ListViewItem = DirectCast(T_Values.Item(2), ListViewItem)
                        T_LV.Items.Insert(Idx, Val)
                    End If

                End If

                Return True

            ElseIf ControlObject.GetType.name = GetType(ListBox).name And PropertyName = "Items" Then

                Dim T_LB As ListBox = DirectCast(ControlObject, ListBox)
                Dim T_Values As List(Of Object) = New List(Of Object)

                If value.GetType.Name = GetType(List(Of Object)).Name Then
                    T_Values = DirectCast(value, List(Of Object))
                Else
                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in SetPropertyValueByName(DirectCast): -> " + value.GetType.Name)
                    End If
                End If


                If T_Values(0).ToString = "Clear" Then
                    T_LB.Items.Clear()
                ElseIf T_Values(0).ToString = "Add" Then
                    Dim Val = T_Values(1)
                    T_LB.Items.Add(Val)
                ElseIf T_Values(0).ToString = "Insert" Then
                    Dim Idx As Integer = Convert.ToInt32(T_Values(1))
                    Dim Val = T_Values(2)
                    T_LB.Items.Insert(Idx, Val)
                End If

                Return True

            Else

                If T_PropertyInfo.CanWrite Then
                    T_PropertyInfo.SetValue(ControlObject, value, Nothing)
                    Return True
                End If
            End If

            Return False

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in SetPropertyValueByName(): -> " + ex.Message)
            End If

            Return False
        End Try

    End Function

    Dim GFXFlag As Boolean = True

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtChartGFXOnOff.Click

        If GFXFlag Then
            GFXFlag = False
            TTTL.TradeTrackTimer.Enabled = False
            SplitContainer2.Panel1.Visible = False
        Else
            GFXFlag = True
            SplitContainer2.Panel1.Visible = True
            TTTL.TradeTrackTimer.Enabled = True
        End If

    End Sub

#End Region

#Region "Test"

    Private Sub BtShowSellFilter_Click(sender As Object, e As EventArgs) Handles BtShowSellFilter.Click

        If SplitContainerSellFilter.Panel1Collapsed = True Then

            BtShowSellFilter.Text = "hide Filter"
            SplitContainerSellFilter.Panel1Collapsed = False

        Else
            BtShowSellFilter.Text = "show Filter"
            SplitContainerSellFilter.Panel1Collapsed = True

        End If

    End Sub
    Private Sub BtShowBuyFilter_Click(sender As Object, e As EventArgs) Handles BtShowBuyFilter.Click

        If SplitContainerBuyFilter.Panel1Collapsed = True Then

            BtShowBuyFilter.Text = "hide Filter"
            SplitContainerBuyFilter.Panel1Collapsed = False

        Else
            BtShowBuyFilter.Text = "show Filter"
            SplitContainerBuyFilter.Panel1Collapsed = True

        End If

    End Sub

    Private Sub ChBxFilter_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxSellFilterShowAutoinfo.CheckedChanged, ChBxSellFilterShowAutofinish.CheckedChanged, ChBxSellFilterShowPayable.CheckedChanged, ChBxBuyFilterShowAutoinfo.CheckedChanged, ChBxBuyFilterShowAutofinish.CheckedChanged, ChBxBuyFilterShowPayable.CheckedChanged

        SetFitlteredPublicOrdersInLVs()

        If Not IsNothing(sender) Then
            Select Case DirectCast(sender, Control).Name

                Case ChBxSellFilterShowAutoinfo.Name
                    SetINISetting(E_Setting.SellFilterAutoinfo, ChBxSellFilterShowAutoinfo.Checked)

                Case ChBxSellFilterShowAutofinish.Name
                    SetINISetting(E_Setting.SellFilterAutofinish, ChBxSellFilterShowAutofinish.Checked)

                Case ChBxSellFilterShowPayable.Name
                    SetINISetting(E_Setting.SellFilterPayable, ChBxSellFilterShowPayable.Checked)

                Case ChBxBuyFilterShowAutoinfo.Name
                    SetINISetting(E_Setting.BuyFilterAutoinfo, ChBxBuyFilterShowAutoinfo.Checked)

                Case ChBxBuyFilterShowAutofinish.Name
                    SetINISetting(E_Setting.BuyFilterAutofinish, ChBxBuyFilterShowAutofinish.Checked)

                Case ChBxBuyFilterShowPayable.Name
                    SetINISetting(E_Setting.BuyFilterPayable, ChBxBuyFilterShowPayable.Checked)

                Case Else
                    'do nothing
            End Select
        End If

    End Sub

    Private Sub ChLBSellFilterMethods_MouseUp(sender As Object, e As MouseEventArgs) Handles ChLBSellFilterMethods.MouseUp

        Dim CheckedMethods As String = ""

        For i As Integer = 0 To ChLBSellFilterMethods.Items.Count - 1

            Dim Method As String = Convert.ToString(ChLBSellFilterMethods.Items(i))

            If ChLBSellFilterMethods.GetItemChecked(i) Then
                CheckedMethods += Method + ";"
            End If

        Next

        If Not CheckedMethods.Trim = "" Then
            CheckedMethods = CheckedMethods.Remove(CheckedMethods.Length - 1)
        End If

        SetINISetting(E_Setting.SellFilterMethods, CheckedMethods)

        SetFitlteredPublicOrdersInLVs()

    End Sub
    Private Sub ChLBBuyFilterMethods_MouseUp(sender As Object, e As MouseEventArgs) Handles ChLBBuyFilterMethods.MouseUp

        Dim CheckedMethods As String = ""

        For i As Integer = 0 To ChLBBuyFilterMethods.Items.Count - 1

            Dim Method As String = Convert.ToString(ChLBBuyFilterMethods.Items(i))

            If ChLBBuyFilterMethods.GetItemChecked(i) Then
                CheckedMethods += Method + ";"
            End If

        Next

        If Not CheckedMethods.Trim = "" Then
            CheckedMethods = CheckedMethods.Remove(CheckedMethods.Length - 1)
        End If

        SetINISetting(E_Setting.BuyFilterMethods, CheckedMethods)

        SetFitlteredPublicOrdersInLVs()

    End Sub


    Sub SetMethodFilter(ByVal MethodFilterListBox As CheckedListBox, ByVal CheckedMethods As List(Of String))

        MethodFilterListBox.Items.Clear()
        MethodFilterListBox.Items.Add("Unknown")
        MethodFilterListBox.Items.AddRange(ClsOrderSettings.GetPayTypes.ToArray)

        For Each CheckedMethod As String In CheckedMethods

            For i As Integer = 0 To MethodFilterListBox.Items.Count - 1
                Dim MethodFilter As String = Convert.ToString(MethodFilterListBox.Items(i))
                If MethodFilter = CheckedMethod Then
                    MethodFilterListBox.SetItemChecked(i, True)
                    Exit For
                End If

            Next

        Next

    End Sub

    Private Function LoadMethodFilter(ByVal Setting As E_Setting) As List(Of String)

        Dim RetList As List(Of String) = New List(Of String)

        If Setting <> E_Setting.BuyFilterMethods And Setting <> E_Setting.SellFilterMethods Then
            Return RetList
        End If

        Dim Methods As String = LoadMethodFilterFromINI(Setting)

        RetList.AddRange(Methods.Split(";"c))

        Return RetList

    End Function
    Private Function LoadMethodFilterFromINI(ByVal Setting As E_Setting) As String

        If Setting <> E_Setting.BuyFilterMethods And Setting <> E_Setting.SellFilterMethods Then
            Return ""
        End If

        Dim DefaultMethodList As List(Of String) = New List(Of String)(ClsOrderSettings.GetPayTypes.ToArray)

        Dim DefaultMethods As String = "Unknown;"
        For Each DefMet As String In DefaultMethodList
            DefaultMethods += DefMet + ";"
        Next

        DefaultMethods = DefaultMethods.Remove(DefaultMethods.Length - 1)

        Dim Methods As String = GetINISetting(Setting, DefaultMethods)

        Return Methods

    End Function

    Private Sub CoBxSellFilterMaxOrders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxSellFilterMaxOrders.SelectedIndexChanged, CoBxBuyFilterMaxOrders.SelectedIndexChanged

        SetFitlteredPublicOrdersInLVs()

        If Not IsNothing(sender) Then
            Select Case DirectCast(sender, Control).Name

                Case CoBxSellFilterMaxOrders.Name
                    SetINISetting(E_Setting.ShowMaxSellOrders, CoBxSellFilterMaxOrders.SelectedItem.ToString)

                Case CoBxBuyFilterMaxOrders.Name
                    SetINISetting(E_Setting.ShowMaxBuyOrders, CoBxBuyFilterMaxOrders.SelectedItem.ToString)

                Case Else
                    'do nothing

            End Select
        End If

    End Sub

    Private Sub BtSetPIN_Click(sender As Object, e As EventArgs) Handles BtSetPIN.Click

        Dim PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.ChangePIN)
        PINForm.ShowDialog()

    End Sub

    Private Sub TSSCryptStatus_Click(sender As Object, e As EventArgs) Handles TSSCryptStatus.Click

        Dim PassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

        If PassPhrase = "" Then
            Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()
        Else
            If TSSCryptStatus.Tag.ToString = "encrypted" Then
                Dim PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.EnterPINOnly)
                PINForm.ShowDialog()
            Else
                GlobalPIN = ""
            End If

        End If

        If CheckPIN() Then
            TSSCryptStatus.Image = My.Resources.status_decrypted
            TSSCryptStatus.Tag = "decrypted"

            TSSCryptStatus.AutoToolTip = True
            TSSCryptStatus.ToolTipText = "the PFP is Unlocked" + vbCrLf + "automation working"

        Else
            TSSCryptStatus.Image = My.Resources.status_encrypted
            TSSCryptStatus.Tag = "encrypted"

            TSSCryptStatus.AutoToolTip = True
            TSSCryptStatus.ToolTipText = "the PFP is Locked" + vbCrLf + "there is no automation"

        End If

    End Sub


    Private Function AutoResizeWindow() As Boolean
        Try

            'Me.WindowState = FormWindowState.Maximized

            Dim MaxSize As Drawing.Size = Me.Size

            'For i As Integer = 0 To 1000
            '    MaxSize = Me.Size
            '    Threading.Thread.Sleep(1)
            '    Application.DoEvents()
            '    Me.Refresh()
            'Next

            Dim Wid As Integer = MaxWidth
            Dim Hei As Integer = MaxHeight

            Wid -= Convert.ToInt32(Wid / 1000)
            Hei -= Convert.ToInt32((Hei / 100) * 10)

            MaxSize.Width = Wid
            MaxSize.Height = Hei

            'Me.WindowState = FormWindowState.Normal

            Me.Size = MaxSize
            Me.Location = New Point(0, 0)

            'For i As Integer = 0 To 1000
            '    Threading.Thread.Sleep(1)
            '    Application.DoEvents()
            '    Me.Refresh()
            'Next

        Catch ex As Exception

            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in AutoResizeWindow(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function



    Private Sub NUDSNOAmount_KeyPress(sender As Object, e As KeyEventArgs) Handles NUDSNOTXFee.KeyDown, NUDSNOItemAmount.KeyDown, NUDSNOCollateral.KeyDown, NUDSNOAmount.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
        End If

    End Sub



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

    Private ReadOnly SortReihenfolge As SortOrder

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

            Dim T_SortItem As List(Of Object) = New List(Of Object)

            If SortItem.GetType.Name = GetType(List(Of Object)).Name Then
                T_SortItem = DirectCast(SortItem, List(Of Object))
            Else
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in ListViewItemExtremeSorter(DirectCast): -> " + SortItem.GetType.Name)
                End If
            End If

            If T_SortItem.Count > 0 Then

                Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

                Itemeigenschaft.Index = Convert.ToInt32(T_SortItem(0))
                Itemeigenschaft.Alphanum = Convert.ToInt32(T_SortItem(1))
                SortierList.Add(Itemeigenschaft)

            End If

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

            Dim T_SortItem As List(Of Object) = New List(Of Object)

            If SortItem.GetType.Name = GetType(List(Of Object)).Name Then
                T_SortItem = DirectCast(SortItem, List(Of Object))
            Else
                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in ListViewItemExtremeSorter(DirectCast): -> " + SortItem.GetType.Name)
                End If
            End If

            If T_SortItem(0).GetType.Name = GetType(String).Name Then
                Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

                Itemeigenschaft.Index = Convert.ToInt32(GetLVColNameFromSubItem(LV, T_SortItem(0).ToString))
                Itemeigenschaft.Alphanum = Convert.ToInt32(T_SortItem(1))
                SortierList.Add(Itemeigenschaft)
            Else
                Dim Itemeigenschaft As Sortiereigenschaften = New Sortiereigenschaften

                Itemeigenschaft.Index = Convert.ToInt32(T_SortItem(0))
                Itemeigenschaft.Alphanum = Convert.ToInt32(T_SortItem(1))
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
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in ListViewItemExtremeSorter.Compare()(): -> " + ex.Message)
            End If

            Return 0
        End Try

    End Function


    Function DateUSToGer(datum As String, Optional ByVal PlusTage As Integer = 0) As String

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
                Dim dat As Date = Convert.ToDateTime(datum)

                dat = dat.AddDays(PlusTage)

                datum = DateUSToGer(DateUSToGer(dat.ToShortDateString, 0), 0)

            End If

            Return datum 'wenn die länge nicht passt, das zurückgeben, was die funktion bekommen hat

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in DateUSToGer(): -> " + ex.Message)
            End If

            Return datum
        End Try

    End Function

End Class