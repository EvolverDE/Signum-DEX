Option Strict On
Option Explicit On
Option Infer Off
Imports Microsoft.SqlServer
Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Diagnostics.Contracts
Imports SnipSwap.ClsOrderSettings
Imports SnipSwap.SnipSwapForm
Imports System.Linq
Imports SnipSwap.ClsAPIRequest

Public Class SnipSwapForm

#Region "Properties"

    Private ReadOnly Property C_SignumDarkBlue As Color = Color.FromArgb(255, 0, 102, 255)
    Private ReadOnly Property C_SignumBlue As Color = Color.FromArgb(255, 0, 153, 255)
    Private ReadOnly Property C_SignumLightGreen As Color = Color.FromArgb(255, 0, 255, 136)

    Public Property PrimaryNode() As String = ""
    Private ReadOnly Property C_SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

    Private Property C_NuBlock As ULong = 1UL
    Private Property C_Block As ULong = 0UL
    Private Property C_Fees As List(Of Double) = New List(Of Double)
    Private Property C_Fee As Double = 0.0
    Private Property C_UTXList As List(Of List(Of String))
    Private Property C_RefreshTime As Integer = 60


    Private Property C_MarketIsCrypto As Boolean = False
    Private Property C_Decimals As Integer = 8

    Private Property C_Boottime As Integer = 0

    Private Property C_CSVSmartContractList() As List(Of S_SmartContract) = New List(Of S_SmartContract)
    Private Property C_DEXSmartContractList As List(Of String) = New List(Of String)
    Private Property C_OffchainBuyOrder As S_OffchainBuyOrder = New S_OffchainBuyOrder(,,,,,,) 'TODO: clear when accepted by a seller


    Private Property C_TradeTrackerSplitContainer As SplitContainer = New SplitContainer
    Private Property C_PanelForTradeTrackerSlot As Panel = New Panel

    Private Property C_TimeLineSplitContainer As SplitContainer = New SplitContainer
    Private Property C_TTTL As TradeTrackerTimeLine = New TradeTrackerTimeLine

    Private Property C_CoBxChartSelectedItem As Integer = 0
    Private Property C_CoBxTickSelectedItem As Integer = 0

    Private Property C_InfoOut As Boolean = False

    Public Property C_DEXContractList As List(Of ClsDEXContract) = New List(Of ClsDEXContract)
    Public Property NodeList() As List(Of String) = New List(Of String)

    Public Property APIRequestList As List(Of S_APIRequest) = New List(Of S_APIRequest)

#End Region 'Properties

    Private Function GetPaymentInfoFromOrderSettings(ByVal TXID As ULong, Optional ByVal Quantity As Double = 0.0, Optional ByVal XAmount As Double = 0.0, Optional ByVal Market As String = "") As String

        Dim PaymentInfo As String = ""

        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(TXID)

        If T_OSList.Count > 0 Then
            Dim T_OS As ClsOrderSettings = T_OSList(0)
            T_OS.SetPayType()

            If ClsDEXContract.CurrencyIsCrypto(Market) Then
                T_OS.SetPayType(E_PayType.AtomicSwap.ToString)
            End If

            If T_OS.AutoSendInfotext Or ClsDEXContract.CurrencyIsCrypto(Market) Then

                Select Case T_OS.PayType
                    Case ClsOrderSettings.E_PayType.Bankaccount
                        PaymentInfo = "Bankdetails=" + T_OS.Infotext.Replace(",", ";")
                    Case ClsOrderSettings.E_PayType.PayPal_E_Mail
                        PaymentInfo = "PayPal-E-Mail=" + T_OS.Infotext.Replace(",", ";")
                    'Case ClsOrderSettings.E_PayType.PayPal_Order

                    '    Dim APIOK As String = CheckPayPalAPI()

                    '    If APIOK = "True" Then
                    '        Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal With {
                    '            .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                    '            .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                    '        }

                    '        Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", Quantity, XAmount, Market)
                    '        Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
                    '        PaymentInfo = "PayPal-Order=" + PPOrderID
                    '    End If

                    Case ClsOrderSettings.E_PayType.Self_Pickup, ClsOrderSettings.E_PayType.Other
                        PaymentInfo = "PaymentInfotext=" + GetINISetting(E_Setting.PaymentInfoText, "").Replace(",", ";")
                    Case ClsOrderSettings.E_PayType.AtomicSwap

                        If ClsDEXContract.CurrencyIsCrypto(Market) Then
                            PaymentInfo = "AtomicSwap=" + Market + ":" + ClsXItemAdapter.GetXItemAddress(Market) 'AtomicSwap: Coinaddress
                        Else
                            PaymentInfo = "PaymentInfotext=" + GetINISetting(E_Setting.PaymentInfoText, "").Replace(",", ";")
                        End If
                    Case Else
                        PaymentInfo = "PaymentInfotext=" + GetINISetting(E_Setting.PaymentInfoText, "").Replace(",", ";")
                End Select

            End If

        End If

        Return PaymentInfo

    End Function
    'Function CheckPayPalOrder(ByVal DEXContract As ClsDEXContract, ByVal PayPalOrder As String) As String

    '    Dim Status As String = ""

    '    If Not PayPalOrder = "0" And Not PayPalOrder = "" Then

    '        Dim APIOK As String = CheckPayPalAPI()
    '        If APIOK = "True" Then

    '            'PayPal Approving check
    '            Dim PPAPI As ClsPayPal = New ClsPayPal With {
    '                .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
    '                .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
    '            }

    '            Dim OrderDetails As List(Of String) = PPAPI.GetOrderDetails(PayPalOrder)
    '            Dim PayPalStatus As String = GetStringBetweenFromList(OrderDetails, "<status>", "</status>")

    '            If PayPalStatus = "APPROVED" Then
    '                PPAPI = New ClsPayPal With {
    '                    .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
    '                    .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
    '                }

    '                OrderDetails = PPAPI.CaptureOrder(PayPalOrder)
    '                PayPalStatus = GetStringBetweenFromList(OrderDetails, "<status>", "</status>")

    '                If PayPalStatus = "COMPLETED" Then

    '                    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

    '                    Dim MasterKeys As List(Of String) = GetPassPhrase()
    '                    If MasterKeys.Count > 0 Then

    '                        Dim Response As String = DEXContract.FinishOrder(MasterKeys(0), Fee)

    '                        If Response.Contains(Application.ProductName + "-error") Then

    '                            If GetINISetting(E_Setting.InfoOut, False) Then
    '                                Dim out As ClsOut = New ClsOut(Application.StartupPath)
    '                                out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalOrder(1): -> " + vbCrLf + Response)
    '                            End If

    '                        Else

    '                            Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
    '                            Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
    '                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
    '                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
    '                            Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

    '                            If TX.Contains(Application.ProductName + "-error") Then

    '                                If GetINISetting(E_Setting.InfoOut, False) Then
    '                                    Dim out As ClsOut = New ClsOut(Application.StartupPath)
    '                                    out.ErrorLog2File(Application.ProductName + "-error in CheckPayPalOrder(2): -> " + vbCrLf + TX)
    '                                End If

    '                            Else
    '                                Status = "COMPLETED"
    '                            End If

    '                        End If


    '                    Else

    '                        If GetINISetting(E_Setting.InfoOut, False) Then
    '                            Dim out As ClsOut = New ClsOut(Application.StartupPath)
    '                            out.WarningLog2File(Application.ProductName + "-warning in CheckPayPalOrder(3): -> no Keys")
    '                        End If

    '                    End If

    '                Else
    '                    Status = PayPalStatus
    '                End If

    '            ElseIf PayPalStatus = "COMPLETED" Then

    '                Status = "COMPLETED"


    '            ElseIf PayPalStatus = "CREATED" Then

    '                Status = "PayPal Order created"

    '            Else
    '                'TODO: auto Recreate PayPal Order
    '                Status = "No Payment received"
    '            End If

    '        End If

    '    End If

    '    Return Status

    'End Function
    Private Function CheckPayPalTransaction(ByVal DEXContract As ClsDEXContract) As String

        Dim Status As String = ""

        Dim APIOK As String = CheckPayPalAPI()
        If APIOK = "True" Then


            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode, )
            'Dim CheckAttachment As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

            If Not DEXContract.CheckForUTX() And Not DEXContract.CheckForTX() Then

                If Not GetAutosignalTXFromINI(DEXContract.CurrentCreationTransaction) Then 'Check for autosignal-TX in Settings.ini and skip if founded

                    'PayPal Approving check
                    Dim PPAPI_GetPayPalTX_to_Autosignal_SmartContract As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork) With {
                        .Client_ID = GetINISetting(E_Setting.PayPalAPIUser, ""),
                        .Secret = GetINISetting(E_Setting.PayPalAPISecret, "")
                    }

                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                    Dim TXDetails As List(Of List(Of String)) = PPAPI_GetPayPalTX_to_Autosignal_SmartContract.GetTransactionList(ColWords.GenerateColloquialWords(DEXContract.CurrentCreationTransaction.ToString(), True, "-", 5))

                    If TXDetails.Count > 0 Then

                        Dim PayPalStatus As String = GetStringBetweenFromList(TXDetails(0), "<transaction_status>", "</transaction_status>")
                        Dim Amount As String = GetStringBetweenFromList(TXDetails(0), "<transaction_amount>", "</transaction_amount>")

                        If Convert.ToDouble(Amount) >= DEXContract.CurrentXAmount And PayPalStatus.ToLower().Trim() = "s" Then
                            'complete

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = DEXContract.FinishOrder(MasterKeys(0), C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in CheckPayPalTransaction(1): -> " + vbCrLf, True) Then

                                    'Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    'Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(Response, MasterKeys(1))
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in CheckPayPalTransaction(2): -> " + vbCrLf, True) Then
                                        Status = "COMPLETED"
                                        SetAutosignalTX2INI(DEXContract.CurrentCreationTransaction)   'Set autosignal-TX in Settings.ini
                                    End If

                                End If

                            Else
                                IsErrorOrWarning(Application.ProductName + "-warning in CheckPayPalTransaction(3): -> no Keys")
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

#Region "Structures"

    Structure S_SmartContract
        Dim ID As ULong
        Dim IsDEX_SC As Boolean
        Dim HistoryOrders As String
    End Structure
    Structure S_OffchainBuyOrder

        Sub New(Optional ByVal TSCID As String = "", Optional ByVal TPubKey As String = "", Optional ByVal TAsk As String = "", Optional ByVal TAmount As Double = 0.0, Optional ByVal TXAmount As Double = 0.0, Optional ByVal TXItem As String = "", Optional ByVal TMethod As String = "")
            SCID = TSCID
            PubKey = TPubKey
            Ask = TAsk
            Amount = TAmount
            XAmount = TXAmount
            XItem = TXItem
            Method = TMethod
        End Sub



        Dim SCID As String
        Dim PubKey As String
        Dim Ask As String
        Dim Amount As Double
        Dim XAmount As Double
        Dim XItem As String
        Dim Method As String
    End Structure
    Structure S_APIRequest
        Dim RequestThread As Threading.Thread
        Dim Command As String
        Dim CommandAttachment As Object
        Dim Node As String
        Dim Status As String
        Dim Result As Object
    End Structure

#End Region 'Structures

#Region "GUI Control"

#Region "General"

    Private Property LoadRunning As Boolean = False
    Private Property Shutdown As Boolean = False
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If C_InfoOut Then
            Dim IOut As ClsOut = New ClsOut(Application.StartupPath)
            IOut.Info2File(Application.ProductName + "-info from Form1_FormClosing(): -> app close (" + sender.ToString + " -> " + e.ToString + ")")
        End If


        Dim Wait As Boolean = TCPAPI.StopAPIServer()
        If Not DEXNET Is Nothing Then
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

    End Sub

    Private Property MaxWidth As Integer = Screen.PrimaryScreen.Bounds.Width
    Private Property MaxHeight As Integer = Screen.PrimaryScreen.Bounds.Height

    Private Property TTS As TradeTrackerSlot = New TradeTrackerSlot

    Private Property GlobalTabPages As List(Of TabPage) = New List(Of TabPage)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim StartupArguments As List(Of String) = Environment.GetCommandLineArgs().ToList

        For Each StartupArg As String In StartupArguments
            If StartupArg.Contains("-test") Then
                BitcoinAddressPrefix = "6f"
            End If
        Next

        For Each TP As TabPage In TabControl1.TabPages
            GlobalTabPages.Add(TP)
        Next

        TabPageRemover("TP_AS_")

        Dim Wait As Boolean = InitiateINI()

        C_InfoOut = GetINISetting(E_Setting.InfoOut, False)
        TCPAPI = New ClsTCPAPI(GetINISetting(E_Setting.TCPAPIServerPort, 8130), GetINISetting(E_Setting.TCPAPIShowStatus, False))

        Dim GasFee As Double = ClsSignumAPI.Planck2Dbl(ClsDEXContract._GasFeeNQT)

        Label16.Text = "+ " + Dbl2LVStr(GasFee, 3) + " Signa Gas fee"

#Region "TradeTracker"

        C_TradeTrackerSplitContainer.BackColor = Color.Transparent

        C_TradeTrackerSplitContainer.Dock = DockStyle.Fill
        C_TradeTrackerSplitContainer.Orientation = Orientation.Horizontal

        C_TradeTrackerSplitContainer.SplitterDistance = 70

        C_TradeTrackerSplitContainer.FixedPanel = FixedPanel.Panel1

        C_PanelForTradeTrackerSlot.AutoScroll = True
        C_PanelForTradeTrackerSlot.Dock = DockStyle.Fill
        C_PanelForTradeTrackerSlot.BackColor = Color.Transparent


        C_TradeTrackerSplitContainer.BorderStyle = BorderStyle.FixedSingle

        C_TimeLineSplitContainer.BackColor = Color.Transparent

        C_TimeLineSplitContainer.Dock = DockStyle.Fill
        C_TimeLineSplitContainer.IsSplitterFixed = True
        C_TimeLineSplitContainer.BorderStyle = BorderStyle.FixedSingle
        'TLS.SplitterDistance = 32
        C_TimeLineSplitContainer.Size = New Size(0, 70)
        'WTTL.Dock = DockStyle.Fill
        C_TTTL.Height = C_TimeLineSplitContainer.Height
        C_TTTL.TradeTrackTimerEnable = False

        AddHandler C_TTTL.TimerTick, AddressOf TradeTrackerTimeLine1_TimerTick

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

        C_CoBxChartSelectedItem = Convert.ToInt32(CoBxChart.Items(1))
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

        C_CoBxTickSelectedItem = Convert.ToInt32(CoBxTick.Items(2))
        AddHandler CoBxTick.DropDownClosed, AddressOf CoBx_Tick_Handler

        C_TimeLineSplitContainer.Panel1.Controls.AddRange({LabChart, LabTick, CoBxChart, CoBxTick})
        C_TimeLineSplitContainer.Panel2.Controls.Add(C_TTTL)

        TTS.TTTL = C_TTTL

        TTS.Location = New Point(0, 0)
        TTS.Dock = DockStyle.Fill
        TTS.LabExch.Text = "booting..."
        TTS.LabPair.Text = "booting..."

        C_TradeTrackerSplitContainer.Panel1.Controls.Add(C_TimeLineSplitContainer)
        C_PanelForTradeTrackerSlot.Controls.Add(TTS)
        C_TradeTrackerSplitContainer.Panel2.Controls.Add(C_PanelForTradeTrackerSlot)

        SplitContainer2.Panel1.Controls.Add(C_TradeTrackerSplitContainer)

#End Region 'TradeTracker

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
                GlobalAddress = GlobalSignumPrefix + ClsReedSolomon.Encode(GlobalAccountID)


            End If

        Else

            If CheckPIN() Then
                Dim MasterKeys As List(Of String) = GetMasterKeys(T_PassPhrase)
                GlobalPublicKey = MasterKeys(0)
                GlobalAccountID = GetAccountID(GlobalPublicKey)
                GlobalAddress = GlobalSignumPrefix + ClsReedSolomon.Encode(GlobalAccountID)

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
                    GlobalAddress = GlobalSignumPrefix + ClsReedSolomon.Encode(GlobalAccountID)
                End If
            End If

        End If

        BlockTimer.Enabled = True

        TBSNOAddress.Text = GlobalAddress

#End Region 'check PassPhrase and Address

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

        ChBxBuyFilterShowOffChainOrders.Checked = GetINISetting(E_Setting.BuyFilterOffchainOrders, False)


        PrimaryNode = GetINISetting(E_Setting.DefaultNode, ClsSignumAPI._DefaultNode)

        C_RefreshTime = GetINISetting(E_Setting.RefreshMinutes, 1) * 60

        If GetINISetting(E_Setting.TCPAPIEnable, False) Then
            TCPAPI.StartAPIServer()
        End If


        Dim T_DEXContractList As List(Of List(Of String)) = GetDEXContractsFromCSV()
        C_DEXSmartContractList.Clear()

        For Each T_DEX As List(Of String) In T_DEXContractList
            If T_DEX(1) = "True" Then
                C_DEXSmartContractList.Add(T_DEX(0))
            End If
        Next


        Dim GetThr As Threading.Thread = New Threading.Thread(AddressOf GetThread)
        GetThr.Start()

        ResetLVColumns()
        ForceReload = True

        'Dim Currencies As List(Of String) = New List(Of String)({"AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"})

        Dim Currency_Day_TickList As List(Of List(Of String)) = GetTCPAPICurrencyDaysTicks()

        Dim GetCandlesInfo As String = ""
        For Each CDT As List(Of String) In Currency_Day_TickList
            GetCandlesInfo += "{""pair"":""" + CDT(0) + """, ""days"":""" + CDT(1) + """, ""tickmin"":""" + CDT(2) + """},"
        Next

        GetCandlesInfo = GetCandlesInfo.Remove(GetCandlesInfo.Length - 1)
        GetCandlesInfo += ""

        Dim DEXAPIInfo As String = ""
        'DEXAPIInfo += "{""openapi"":""3.0.3"","
        'DEXAPIInfo += """info"":{"
        'DEXAPIInfo += """title"":""SnipSwapDEXAPI"","
        'DEXAPIInfo += """version"":""1.0.0"","
        'DEXAPIInfo += """contact"":{"
        'DEXAPIInfo += """email"":""development@signum.network""},"
        'DEXAPIInfo += """license"":{"
        'DEXAPIInfo += """name"":""Apache 2.0"","
        'DEXAPIInfo += """url"":""http://www.apache.org/licenses/LICENSE-2.0.html""}},"
        'DEXAPIInfo += """paths"": {"

        'DEXAPIInfo += """/API/v1/Info"":{"
        'DEXAPIInfo += """GET"":{"
        'DEXAPIInfo += """operationId"": ""info"","
        'DEXAPIInfo += """summary"": ""shows this info"","
        'DEXAPIInfo += """responses"":{"
        'DEXAPIInfo += """200"":{"
        'DEXAPIInfo += """description"": ""The OpenAPI V3 description of this API""}}}},"

        'DEXAPIInfo += """/API/v1/Candles"":{"
        'DEXAPIInfo += """GET"":{"
        'DEXAPIInfo += """operationId"": ""candles"","
        'DEXAPIInfo += """summary"": ""response candleentries for chart plotting"","
        'DEXAPIInfo += """responses"":{"
        'DEXAPIInfo += """200"":{"
        'DEXAPIInfo += """description"": ""abcd""}}}}"

        'DEXAPIInfo += "}}"

        DEXAPIInfo += "{""application"":""SnipSwapDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""primaryNode"":""" + PrimaryNode + """,""response"":""Info"","
        DEXAPIInfo += """requests"":{"
        DEXAPIInfo += """Info"":{""method"":""GET"",""description"":""shows this info"",""queryExample"":""/API/v1/Info""},"
        DEXAPIInfo += """Candles"":[{""method"":""GET"",""description"":""response candleentries for chart plotting"",""queryExample"":""/API/v1/Candles/USD_SIGNA?days=3&tickmin=60""},"
        DEXAPIInfo += GetCandlesInfo
        DEXAPIInfo += "],"

        DEXAPIInfo += """SmartContracts"":[{""command"":""show"",""method"":""GET"",""description"":""shows the list of the DEX contracts"","
        DEXAPIInfo += """queryExample"":""/API/v1/SmartContract{?ID={long[,long,long]} (OPTIONAL)}"","
        DEXAPIInfo += """queryExample2"":""/API/v1/SmartContract"","
        DEXAPIInfo += """queryBody2"":{""id"":[""{long}"",""{long (OPTIONAL)}"",""{long (OPTIONAL)}""]}},"

        DEXAPIInfo += "{""command"":""create"",""method"":""POST"",""description"":""creates a new DEX smart contract"","
        DEXAPIInfo += """queryExample"":""/API/v1/SmartContract{?privateKey={32 bytes in Hex (NOT RECOMMENDED)} (OPTIONAL)}"","
        DEXAPIInfo += """queryBody"":{""privateKey"":""{32 bytes in Hex (NOT RECOMMENDED)}""},"
        DEXAPIInfo += """queryExample2"":""/API/v1/SmartContract{?publicKey={32 bytes in Hex} (OPTIONAL)}"","
        DEXAPIInfo += """queryBody2"":{""publicKey"":""{32 bytes in Hex}""}}"

        DEXAPIInfo += "],"

        DEXAPIInfo += """Orders"":[{""command"":""show"",""method"":""GET"",""description"":""shows the list of open orders"",""queryExample"":""/API/v1/Orders""},"

        DEXAPIInfo += "{""command"":""create"",""method"":""GET,POST"",""description"":""creates a new order on a contract"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders{/id or address (OPTIONAL)}?PublicKey={32 bytes in Hex}&Type=SellOrder&AmountNQT={unsigned long}&CollateralNQT={unsigned long}&XAmountNQT={unsigned long}&XItem=USD"","
        DEXAPIInfo += """queryExample2"":""/API/v1/Orders{/id or address (OPTIONAL)}?PassPhrase={string (NOT RECOMMENDED)}&Type=SellOrder&AmountNQT={unsigned long}&CollateralNQT={unsigned long}&XAmountNQT={unsigned long}&XItem=USD""},"

        DEXAPIInfo += "{""command"":""create"",""method"":""POST"",""description"":""creates a new order on a DEX contract"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""contract"":""{id or address} (OPTIONAL)"",""type"":""SellOrder"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}"",""amountNQT"":""{unsigned long}"",""collateralNQT"":""{unsigned long}"",""xAmountNQT"":""{unsigned long}"",""xItem"":""USD""}},"

        DEXAPIInfo += "{""command"":""accept"",""method"":""GET,POST"",""description"":""accepts an order"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders/{id or address}?PublicKey={32 bytes in Hex}"","
        DEXAPIInfo += """queryExample2"":""/API/v1/Orders/{id or address}?PassPhrase={string (NOT RECOMMENDED)}""},"


        DEXAPIInfo += "{""command"":""accept"",""method"":""POST"",""description"":""accepts an order (only sellorders)"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""orderId"":""{id or address}"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}""}},"

        DEXAPIInfo += "{""command"":""injectResponder"",""method"":""POST"",""description"":""injects an responder (only seller) (only sellorders)"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""orderId"":""{id or address}"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}"",""injectPublicKey"":""{32 bytes in Hex}""}},"

        DEXAPIInfo += "{""command"":""injectChainSwapHash"",""method"":""POST"",""description"":""injects a chain swap hash into an reserved order (only seller) (only sellorders) (only crypto to crypto)"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""orderId"":""{id or address}"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}"",""injectChainSwapHash"":""{32 bytes in Hex}""}},"

        DEXAPIInfo += "{""command"":""finishOrder"",""method"":""POST"",""description"":""finishes an reserved order"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""orderId"":""{id or address}"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}""}},"

        DEXAPIInfo += "{""command"":""finishOrderWithChainSwapKey"",""method"":""POST"",""description"":""finishes an reserved order with an chain swap key (only crypto to crypto)"","
        DEXAPIInfo += """queryExample"":""/API/v1/Orders"","
        DEXAPIInfo += """bodyExample"":{""orderId"":""{id or address}"",""passPhrase"":""{string (NOT RECOMMENDED)} (OPTIONAL)"",""publicKey"":""{32 bytes in Hex}"",""chainSwapKey"":""{32 bytes in Hex}""}}"


        DEXAPIInfo += "],"

        DEXAPIInfo += """Broadcast"":[{""command"":""broadcast"",""method"":""GET,POST"",""description"":""broadcasts an signed transaction to the network of the token"",""queryExample"":""/API/v1/Broadcast/{token (eg. Signum,BTC)}?SignedTransactionBytes={hex string}"","
        DEXAPIInfo += """queryExample2"":""/API/v1/Broadcast?Token={token (eg. Signum,BTC)}&SignedTransactionBytes={hex string}"","
        DEXAPIInfo += """queryExample3"":""/API/v1/Broadcast"","
        DEXAPIInfo += """bodyExample"":{""token"":""BTC"",""signedTransactionBytes"":""{hex string}""}}"
        DEXAPIInfo += "],"

        DEXAPIInfo += """Bitcoin"":[{""command"":""create"",""method"":""GET,POST"",""description"":""creates an bitcoin transaction"",""queryExample"":""/API/v1/Bitcoin/Transaction/{32 bytes in Hex}?BitcoinOutputType=TimeLockChainSwapHash&BitcoinSenderAddress={27 to 34 chars}&privateKey={32 bytes in Hex (NOT RECOMMENDED)}&BitcoinRecipientAddress={27 to 34 chars}&BitcoinChainSwapHash={32 bytes in Hex}&BitcoinAmountNQT={unsigned long}""},"
        DEXAPIInfo += "{""command"":""create"",""method"":""POST"",""description"":""creates an bitcoin transaction"",""queryExample"":""/API/v1/Bitcoin/Transaction"",""bodyExample"":{""inputs"":[{""transaction"":""{32 bytes in Hex}"",""index"":0,""script"":""{hex string}"",""privateKey"":""{32 bytes in Hex (NOT RECOMMENDED)}"",""address"":""{32 bytes in Hex}""}],""outputs"":[{""type"":""TimeLockChainSwapHash"",""recipient"":""{27 to 34 chars}"",""change"":""{27 to 34 chars}"",""chainSwapHash"":""{32 bytes in Hex}"",""amount"":0.00001234}]}}"
        DEXAPIInfo += "]}}"

        Dim DEXAPIGetInfoResponse As ClsTCPAPI.API_Response = New ClsTCPAPI.API_Response
        DEXAPIGetInfoResponse.API_Interface = "API"
        DEXAPIGetInfoResponse.API_Version = "v1"
        DEXAPIGetInfoResponse.API_Command = "Info"
        DEXAPIGetInfoResponse.API_Response = DEXAPIInfo
        DEXAPIGetInfoResponse.API_Parameters = New List(Of String)({""})

        TCPAPI.ResponseMSGList.Add(DEXAPIGetInfoResponse)

    End Sub

    Private Sub BtSetPIN_Click(sender As Object, e As EventArgs) Handles BtSetPIN.Click

        Dim PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.ChangePIN)
        PINForm.ShowDialog()

    End Sub
    Private Sub NUDSNOAmount_KeyDown(sender As Object, e As KeyEventArgs) Handles NUDSNOItemAmount.KeyDown, NUDSNOCollateral.KeyDown, NUDSNOAmount.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TSSCryptStatus_Click(sender As Object, e As EventArgs) Handles TSSCryptStatus.Click

        Dim PassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

        If PassPhrase = "" Then
            Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()
        Else

            If Not IsNothing(TSSCryptStatus.Tag) Then

                If TSSCryptStatus.Tag.ToString = "encrypted" Then
                    Dim PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.EnterPINOnly)
                    PINForm.ShowDialog()
                Else
                    GlobalPIN = ""
                End If
            Else
                GlobalPIN = ""
            End If

        End If

        If CheckPIN() Then
            TSSCryptStatus.Image = My.Resources.status_decrypted
            TSSCryptStatus.Tag = "decrypted"

            TSSCryptStatus.AutoToolTip = True
            TSSCryptStatus.ToolTipText = "the SnipSwap is Unlocked" + vbCrLf + "automation working"

        Else
            TSSCryptStatus.Image = My.Resources.status_encrypted
            TSSCryptStatus.Tag = "encrypted"

            TSSCryptStatus.AutoToolTip = True
            TSSCryptStatus.ToolTipText = "the SnipSwap is Locked" + vbCrLf + "there is no automation"

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
            IsErrorOrWarning(Application.ProductName + "-error in AutoResizeWindow(): -> " + ex.Message)
        End Try

        Return True

    End Function

#Region "TradeTracker"

    Private Sub CoBx_Chart_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxChart As ComboBox = DirectCast(sender, ComboBox)
        C_CoBxChartSelectedItem = Convert.ToInt32(CoBxChart.SelectedItem)

        ForceReload = True
        'BlockTimer_Tick(True, Nothing)

    End Sub
    Private Sub CoBx_Tick_Handler(ByVal sender As Object, ByVal e As EventArgs)

        Dim CoBxTick As ComboBox = DirectCast(sender, ComboBox)
        C_CoBxTickSelectedItem = Convert.ToInt32(CoBxTick.SelectedItem)

        ForceReload = True

        'BlockTimer_Tick(True, Nothing)

    End Sub
    Private Sub TradeTrackerTimeLine1_TimerTick(sender As Object)

        For Each TTSlot As TradeTrackerSlot In C_PanelForTradeTrackerSlot.Controls

            Dim TempSC As SplitContainer = CType(C_TradeTrackerSplitContainer.Panel1.Controls(0), SplitContainer)
            Dim TempTimeLine As TradeTrackerTimeLine = CType(TempSC.Panel2.Controls(0), TradeTrackerTimeLine)
            Try

                C_TimeLineSplitContainer.SplitterDistance = TTSlot.SplitterDistance
            Catch ex As Exception

            End Try
            TTSlot.Dock = DockStyle.Fill
            C_TTTL.Dock = DockStyle.Fill

            TTSlot.Chart_EMA_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.Chart_EMA_EndDate = TempTimeLine.SkalaEndDate

            TTSlot.MACD_RSI_TR_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.MACD_RSI_TR_EndDate = TempTimeLine.SkalaEndDate

        Next

    End Sub

#End Region 'TradeTracker

    Private Sub BlockTimer_Tick(sender As Object, e As EventArgs) Handles BlockTimer.Tick

        C_Boottime += 1

        'StatusBar.Visible = True
        'StatusBar.Maximum = RefreshTime
        'StatusBar.Value = RefreshTime - Boottime

        SetDEXNETRelevantMsgsToLVs()

        'MsgBox(SplitContainer2.Panel2.Width.ToString)
        If SplitContainer2.Panel1.Width < 500 Then
            If Not C_TimeLineSplitContainer.Panel1Collapsed Then
                C_TimeLineSplitContainer.Panel1Collapsed = True
                TTS.SlotSplitter.Panel1Collapsed = True
            End If
        Else
            If C_TimeLineSplitContainer.Panel1Collapsed Then
                C_TimeLineSplitContainer.Panel1Collapsed = False
                TTS.SlotSplitter.Panel1Collapsed = False
            End If
        End If


        Dim Wait As Boolean = False

        If C_Boottime >= C_RefreshTime Or ForceReload Then

            C_TTTL.TradeTrackTimer.Enabled = False
            'SplitContainer2.Panel1.Visible = False

            'Me.Text = "Codename: Perls for Pigs (TestNet) " & MaxWidth.ToString & "/" & MaxHeight.ToString + " TO " + Me.Size.Width.ToString + "/" + Me.Size.Height.ToString

            Wait = ReloadINI()

            If MaxWidth < Me.Size.Width Or MaxHeight < Me.Size.Height Then
                Wait = AutoResizeWindow()
            End If

            C_Boottime = 0

            If Checknodes() > 0 Then

                BlockTimer.Enabled = False

                TSSStatusImage.Text = "in Sync..."
                TSSStatusImage.Image = My.Resources.status_wait

                If CheckPIN() Then
                    TSSCryptStatus.Image = My.Resources.status_decrypted
                    TSSCryptStatus.Tag = "decrypted"

                    TSSCryptStatus.AutoToolTip = True
                    TSSCryptStatus.ToolTipText = "the SnipSwap is Unlocked" + vbCrLf + "automation working"

                Else
                    TSSCryptStatus.Image = My.Resources.status_encrypted
                    TSSCryptStatus.Tag = "encrypted"

                    TSSCryptStatus.AutoToolTip = True
                    TSSCryptStatus.ToolTipText = "the SnipSwap is Locked" + vbCrLf + "there is no automation"

                End If


                If Not DEXNET Is Nothing Then

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

                If C_NuBlock > C_Block Or ForceReload Then
                    C_Block = C_NuBlock
                    ForceReload = False

                    LoadRunning = True

                    Application.DoEvents()

                    Wait = XItemLoader()
                    Wait = Loading()
                    Wait = SetInLVs()
                    DelBlockedSmartContract()

                    'resend offchain order
                    If C_OffchainBuyOrder.Ask = "WantToBuy" Then
                        Dim Masterkeys As List(Of String) = GetPassPhrase()
                        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                        If Masterkeys.Count > 0 Then
                            DEXNET.BroadcastMessage("<SCID>0</SCID><Ask>" + C_OffchainBuyOrder.Ask + "</Ask><Amount>" + C_OffchainBuyOrder.Amount.ToString + "</Amount><XAmount>" + C_OffchainBuyOrder.XAmount.ToString + "</XAmount><XItem>" + C_OffchainBuyOrder.XItem + "</XItem><Method>" + C_OffchainBuyOrder.Method + "</Method>", Masterkeys(1), Masterkeys(2), Masterkeys(0))
                        End If
                    End If

                    C_RefreshTime = GetINISetting(E_Setting.RefreshMinutes, 1) * 60

                    Dim CoBxChartVal As Integer = 1
                    Dim CoBxTickVal As Integer = 1

                    For Each CTRL As Object In C_TimeLineSplitContainer.Panel1.Controls

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

            C_TTTL.TradeTrackTimer.Enabled = True

            Dim Status As E_ConnectionStatus = GetConnectionStatus(PrimaryNode, DEXNET)

            Select Case Status
                Case E_ConnectionStatus.Offline
                    TSSStatusImage.Text = "Offline"
                    TSSStatusImage.Image = My.Resources.status_offline
                Case E_ConnectionStatus.InSync
                    TSSStatusImage.Text = "in Sync..."
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

    Private Property ForceReload As Boolean = False
    Private Sub CoBxMarket_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxMarket.SelectedIndexChanged, CoBxMarket.DropDownClosed

        Dim NuMarket As String = Convert.ToString(CoBxMarket.SelectedItem)

        If ClsDEXContract.CurrencyIsCrypto(NuMarket) Then

            Dim XItem As AbsClsXItem = ClsXItemAdapter.NewXItem(NuMarket)
            Dim Info As String = XItem.GetXItemInfo()

            If Not IsErrorOrWarning(Info) And Not Info.Trim() = "" Then
                If (Not GlobalPIN.Trim() = "" And Not GetINISetting(E_Setting.PINFingerPrint, "").Trim() = "") Or GetINISetting(E_Setting.PINFingerPrint, "").Trim() = "" Then

                    'TODO: XItem adapter
                    If Not GetINISetting(E_Setting.BitcoinAccounts, "").Trim() = "" Then
                        CurrentMarket = NuMarket

                        TabPageAdder("TP_AS_" + NuMarket)

                        SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)
                        ResetLVColumns()
                        ForceReload = True
                        TSSCryptStatus.Enabled = False

                        LabCollateralPercent.Enabled = False
                        LabColPercentage.Enabled = False

                        TBarCollateralPercent.Value = 0
                        TBarCollateralPercent.Enabled = False

                        'NUDSNOAmount.Maximum = Decimal.MaxValue '79228162514264337593543950335

                        NUDSNOCollateral.Minimum = 0
                        NUDSNOCollateral.Maximum = 1
                        NUDSNOCollateral.Value = 0

                        NUDSNOCollateral.Enabled = False

                        Label16.Enabled = False

                    Else
                        Dim Message As String = "There are no Bitcoin-Accounts in the Settings " + vbCrLf
                        Message += "you have to add at least one Bitcoin-Account in the Settings"
                        ClsMsgs.MBox(Message, "Error",,, ClsMsgs.Status.Erro, 3, ClsMsgs.Timer_Type.ButtonEnable)

                        TabPageRemover("TP_AS_" + NuMarket)

                        CoBxMarket.SelectedItem = "USD"

                        SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)
                        ResetLVColumns()
                        ForceReload = True
                        TSSCryptStatus.Enabled = True

                        LabCollateralPercent.Enabled = True
                        LabColPercentage.Enabled = True

                        TBarCollateralPercent.Value = 0
                        TBarCollateralPercent.Enabled = True

                        SetFeeControls()
                        NUDSNOCollateral.Enabled = True

                        Label16.Enabled = True

                    End If

                Else

                    Dim Message As String = "You need to unlock the DEX (the lock on the bottom right corner) " + vbCrLf
                    Message += "so the DEX can interact with both Blockchains for timelocked " + vbCrLf + "AtomicSwaps automatically!"
                    ClsMsgs.MBox(Message, "Error",,, ClsMsgs.Status.Erro, 3, ClsMsgs.Timer_Type.ButtonEnable)

                    TabPageRemover("TP_AS_" + NuMarket)

                    CoBxMarket.SelectedItem = "USD"

                    SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)
                    ResetLVColumns()
                    ForceReload = True
                    TSSCryptStatus.Enabled = True

                    LabCollateralPercent.Enabled = True
                    LabColPercentage.Enabled = True

                    TBarCollateralPercent.Value = 0
                    TBarCollateralPercent.Enabled = True

                    SetFeeControls()
                    NUDSNOCollateral.Enabled = True

                    Label16.Enabled = True

                End If

            Else

                'TODO: XItem adapter
                Dim Message As String = NuMarket + "-Node not reachable on " + GetINISetting(E_Setting.BitcoinAPINode, "https://127.0.0.1") + vbCrLf
                Message += "please make sure your Node is up and running and have the right settings." + vbCrLf + vbCrLf
                Message += "please make also sure the DEX is unlocked(the lock at the bottom right" + vbCrLf + "corner) "
                Message += "so the DEX can interact with both Blockchains for timelocked " + vbCrLf + "AtomicSwaps automatically!"
                ClsMsgs.MBox(Message, "Error",,, ClsMsgs.Status.Erro, 3, ClsMsgs.Timer_Type.ButtonEnable)

                TabPageRemover("TP_AS_" + NuMarket)

                CoBxMarket.SelectedItem = "USD"

                SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)
                ResetLVColumns()
                ForceReload = True
                TSSCryptStatus.Enabled = True

                LabCollateralPercent.Enabled = True
                LabColPercentage.Enabled = True

                TBarCollateralPercent.Value = 0
                TBarCollateralPercent.Enabled = True

                SetFeeControls()
                NUDSNOCollateral.Enabled = True

                Label16.Enabled = True

            End If

        Else
            TabPageRemover("TP_AS_")
            CurrentMarket = NuMarket
            SetINISetting(E_Setting.LastMarketViewed, CurrentMarket)
            ResetLVColumns()
            ForceReload = True
            TSSCryptStatus.Enabled = True

            LabCollateralPercent.Enabled = True
            LabColPercentage.Enabled = True

            TBarCollateralPercent.Value = 0
            TBarCollateralPercent.Enabled = True

            SetFeeControls()
            NUDSNOCollateral.Enabled = True

            Label16.Enabled = True

        End If

    End Sub

    Private Sub SetFeeControls()

        Dim T_Amount As Decimal = NUDSNOAmount.Value
        Dim T_Percentage As Decimal = Convert.ToDecimal(28 + (If(TBarCollateralPercent.Value = 0D, 1, TBarCollateralPercent.Value) * 2))

        If T_Amount > 100 Then
            NUDSNOCollateral.Minimum = (T_Amount / 100) * 30
            NUDSNOCollateral.Maximum = (T_Amount / 100) * 50
            NUDSNOCollateral.Value = (T_Amount / 100) * T_Percentage
        Else
            NUDSNOCollateral.Minimum = 0
            NUDSNOCollateral.Maximum = 1
            NUDSNOCollateral.Value = 0
        End If

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

#End Region 'General

#Region "TabPages"

    Private Sub TabPageAdder(ByVal T_TabPageName As String)

        For Each TP As TabPage In GlobalTabPages

            If TP.Name.Contains(T_TabPageName) Then

                If Not TabControl1.TabPages.Contains(TP) Then
                    TabControl1.TabPages.Add(TP)
                    Exit For
                End If

            End If

        Next

    End Sub

    Private Sub TabPageRemover(ByVal T_TabPageName As String)

        Dim TabPagesToRemove As List(Of TabPage) = New List(Of TabPage)

        For Each TP As TabPage In TabControl1.TabPages
            If TP.Name.Contains(T_TabPageName) Then
                TabPagesToRemove.Add(TP)
            End If
        Next

        For Each tp As TabPage In TabPagesToRemove
            TabControl1.TabPages.Remove(tp)
        Next

    End Sub

#End Region 'TabPages

#Region "Marketdetails - Controls"

    Private Sub BtCreateNewSmartContract_Click(sender As Object, e As EventArgs) Handles BtCreateNewSmartContract.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

        Dim BalList As List(Of String) = C_SignumAPI.GetBalance(TBSNOAddress.Text)

        Dim Available As Double = 0.0
        Dim AvaStr As String = GetStringBetweenFromList(BalList, "<available>", "</available>")

        If AvaStr.Trim = "" Then

        Else
            Available = Val(AvaStr.Replace(",", "."))
        End If

        If Available > ClsSignumAPI.Planck2Dbl(ClsDEXContract._DeployFeeNQT) And Available > 0.0 Then

            Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new Payment channel?", "Create Smart Contract", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

            If MsgResult = ClsMsgs.CustomDialogResult.Yes Then


                Dim MasterKeys As List(Of String) = GetPassPhrase()

                If MasterKeys.Count > 0 Then

                    Dim Response As String = C_SignumAPI.CreateSmartContract(MasterKeys(0), ClsSignumSmartContract.CreationMachineData)

                    If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtCreateNewSmartContract_Click(1): -> " + vbCrLf, True) Then

                        Dim NuList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                        Response = GetStringBetweenFromList(NuList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                        Dim UTX As String = Response
                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtCreateNewSmartContract_Click(2): -> " + vbCrLf, True) Then

                            'Dim NuSmartContract As S_SmartContract = New S_SmartContract
                            'NuSmartContract.ID = GetULongBetweenFromList(NuList, "<transaction>", "</transaction>")
                            'Dim AccRS As List(Of String) = SignumAPI.RSConvert(NuSmartContract.ID)
                            'NuSmartContract.IsDEX_SC = True

                            ClsMsgs.MBox("New Smart Contract Created", "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                        End If

                    End If


                Else


                    Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                    Dim Response As String = ""
                    If Not GlobalPublicKey.Trim = "" Then

                        Response = C_SignumAPI.CreateSmartContract(GlobalPublicKey, ClsSignumSmartContract.CreationMachineData)

                        If IsErrorOrWarning(Response, Application.ProductName + "-error in BtCreateNewSmartContract_Click(2a): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            Dim NuList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                            Response = GetStringBetweenFromList(NuList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                            Dim UTX As String = Response
                            PinForm.TBUnsignedBytes.Text = UTX

                        End If
                    End If

                    PinForm.ShowDialog()

                    If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                        Response = C_SignumAPI.CreateSmartContract(PinForm.PublicKey, ClsSignumSmartContract.CreationMachineData) ' T_DEXContract.AcceptSellOrder(PinForm.PublicKey, Collateral, Fee)

                        If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtCreateNewSmartContract_Click(2a): -> " + vbCrLf, True) Then

                            Dim NuList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                            Response = GetStringBetweenFromList(NuList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                            Dim UTX As String = Response

                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                            Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtCreateNewSmartContract_Click(2b): -> " + vbCrLf, True) Then

                                Dim NuSmartContract As S_SmartContract = New S_SmartContract
                                NuSmartContract.ID = GetULongBetweenFromList(NuList, "<transaction>", "</transaction>")
                                Dim AccRS As List(Of String) = C_SignumAPI.RSConvert(NuSmartContract.ID)
                                NuSmartContract.IsDEX_SC = True

                                ClsMsgs.MBox("New Smart Contract Created" + vbCrLf + vbCrLf + "TX: " + NuSmartContract.ID.ToString, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                            End If

                        End If

                    Else
                        ClsMsgs.MBox("Smart Contract creation canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                    End If


                End If

            End If

        Else
            'not enough balance
            Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance for the Smart Contract creation.", "not enough balance")

        End If


    End Sub

    Private Sub RBSNOSell_CheckedChanged(sender As Object, e As EventArgs) Handles RBSNOSell.CheckedChanged, RBSNOBuy.CheckedChanged

        If RBSNOSell.Checked Then
            LabWTX.Text = "WantToSell:"
        ElseIf RBSNOBuy.Checked Then
            LabWTX.Text = "WantToBuy:"
        End If

    End Sub

    Private Property oldAmount As Decimal = 0D

    Private Sub TBarCollateralPercent_Scroll(sender As Object, e As EventArgs) Handles TBarCollateralPercent.Scroll

        If TBarCollateralPercent.Enabled Then

            If TBarCollateralPercent.Value = 0 Then
                NUDSNOCollateral.Minimum = 0
                NUDSNOCollateral.Maximum = 0

                If NUDSNOAmount.Value > 100D Then
                    oldAmount = NUDSNOAmount.Value
                    NUDSNOAmount.Value = 100D
                End If

                NUDSNOCollateral.Value = 0.0D
                LabColPercentage.Text = "0 %"

            Else

                NUDSNOAmount.Value = If(NUDSNOAmount.Value <= 100D, oldAmount, NUDSNOAmount.Value)

                Dim T_Amount As Decimal = NUDSNOAmount.Value
                Dim T_Percentage As Decimal = Convert.ToDecimal(8 + (TBarCollateralPercent.Value * 2))

                If T_Percentage > 50D Then
                    T_Percentage = 50D
                End If

                LabColPercentage.Text = T_Percentage.ToString + " %"

                If T_Amount > 0 Then
                    NUDSNOCollateral.Minimum = (T_Amount / 100) * 10
                    NUDSNOCollateral.Maximum = (T_Amount / 100) * 50
                    NUDSNOCollateral.Value = (T_Amount / 100) * T_Percentage
                Else
                    NUDSNOCollateral.Minimum = 0
                    NUDSNOCollateral.Maximum = 1
                    NUDSNOCollateral.Value = 0
                End If

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

        If NUDSNOAmount.Value > 100D Then
            If TBarCollateralPercent.Enabled And TBarCollateralPercent.Value = 0 Then
                TBarCollateralPercent.Value = 1
            End If
        End If

        TBarCollateralPercent_Scroll(sender, Nothing)

    End Sub

    Private Sub TBarFee_Scroll(sender As Object, e As EventArgs) Handles TBarFee.Scroll

        If C_Fees.Count > 0 Then
            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            C_Fees = C_SignumAPI.GetFees()
        End If

        LabMinFee.Text = C_Fees(TBarFee.Value).ToString() + " Signa"

    End Sub

    Private Sub BtSNOSetOrder_Click(sender As Object, e As EventArgs) Handles BtSNOSetOrder.Click

        BtSNOSetOrder.Text = "Wait..."
        BtSNOSetOrder.Enabled = False

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)

        Try

            Dim BalList As List(Of String) = C_SignumAPI.GetBalance(TBSNOAddress.Text)
            TBSNOBalance.Text = GetDoubleBetweenFromList(BalList, "<available>", "</available>").ToString

            Dim MinAmount As Double = Convert.ToDouble(NUDSNOAmount.Value) '100,00000000
            Dim XItemMinAmount As Double = Convert.ToDouble(NUDSNOItemAmount.Value) '1,00000000

            Dim Amount As Double = Convert.ToDouble(NUDSNOAmount.Value)
            Dim CurrentFee As Integer = Convert.ToInt32(TBarFee.Value)

            Dim FeeArray As Array = [Enum].GetValues(GetType(ClsSignumAPI.E_Fee))
            Dim FeeList As List(Of ClsSignumAPI.E_Fee) = New List(Of ClsSignumAPI.E_Fee)

            For Each FeeEnum As ClsSignumAPI.E_Fee In FeeArray
                FeeList.Add(FeeEnum)
            Next

            Dim Fee As Double = C_SignumAPI.GetTXFee("", FeeList(CurrentFee)) ' Convert.ToDouble(NUDSNOTXFee.Value)
            Dim Collateral As Double = Convert.ToDouble(NUDSNOCollateral.Value)
            Dim Item As String = CurrentMarket
            Dim ItemAmount As Double = Convert.ToDouble(NUDSNOItemAmount.Value)

            If MinAmount > 0.0 And XItemMinAmount > 0.0 Then

                Dim MinXAmount As Double = 1.0

                For i As Integer = 1 To C_Decimals
                    MinXAmount *= 0.1
                Next

                MinXAmount = Math.Round(MinXAmount, 8)

                'If MarketIsCrypto Then
                If XItemMinAmount / MinAmount < MinXAmount Then
                    ClsMsgs.MBox("Minimum for one SIGNA must be greater than " + Dbl2LVStr(MinXAmount, C_Decimals) + " " + CurrentMarket + "!", "Value to low",,, ClsMsgs.Status.Erro)
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


            Dim AccAmount As Double = GetDoubleBetweenFromList(C_SignumAPI.GetBalance(TBSNOAddress.Text), "<available>", "</available>")

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
                            If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Or (T_DEXContract.Deniability And Not ChBxDeniability.Checked) Or (Not T_DEXContract.Deniability And ChBxDeniability.Checked) Then
                                T_DEXContract = Nothing 'New clsDEXContract
                            Else
                                Exit For
                            End If
                        End If
                    Next
                End If


                If T_DEXContract Is Nothing Then
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

                If RBSNOSell.Checked Then

                    If AccAmount > Amount + Fee + Collateral Then
                        'enough balance

                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new SellOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, C_Decimals) + " " + Item, "Create SellOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = T_DEXContract.CreateSellOrder(MasterKeys(0), Amount, Collateral, Item, ItemAmount, Fee)

                                If IsErrorOrWarning(Response) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else

                                    'Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                    Dim UTX As String = Response ' GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell1): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else
                                        ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If

                                End If
                            Else

                                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                                Dim Response As String = ""

                                If Not GlobalPublicKey.Trim = "" Then

                                    Response = T_DEXContract.CreateSellOrder(GlobalPublicKey, Amount, Collateral, Item, ItemAmount, Fee)

                                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell2): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else

                                        Dim UTX As String = Response
                                        PinForm.TBUnsignedBytes.Text = UTX

                                    End If
                                End If

                                PinForm.ShowDialog()

                                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                    Response = T_DEXContract.CreateSellOrder(PinForm.PublicKey, Amount, Collateral, Item, ItemAmount, Fee)

                                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell3): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else

                                        'Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = Response ' GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(Sell4): -> " + vbCrLf, True) Then
                                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                        Else
                                            ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                        End If

                                    End If

                                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(Sell5): -> " + vbCrLf, True) Then
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

                        If T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ Then
                            Collateral += 0.438 '0 0000
                        Else
                            Collateral += 0.393 '0 0000
                        End If

                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new BuyOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, C_Decimals) + " " + Item, "Create BuyOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then
                                Dim Response As String = T_DEXContract.CreateBuyOrder(MasterKeys(0), Amount, Collateral, Item, ItemAmount, Fee)

                                If IsErrorOrWarning(Response) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else

                                    Dim UTX As String = Response
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtSNOSetOrder_Click(Buy1): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else
                                        ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                    End If

                                End If

                            Else

                                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                                Dim Response As String = ""

                                If Not GlobalPublicKey.Trim = "" Then

                                    Response = T_DEXContract.CreateBuyOrder(PinForm.PublicKey, Amount, Collateral, Item, ItemAmount, Fee)

                                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Buy2): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else
                                        Dim UTX As String = Response
                                        PinForm.TBUnsignedBytes.Text = UTX
                                    End If
                                End If

                                PinForm.ShowDialog()

                                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                    Response = T_DEXContract.CreateBuyOrder(PinForm.PublicKey, Amount, Collateral, Item, ItemAmount, Fee)

                                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Buy3): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else

                                        'Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
                                        Dim UTX As String = Response ' GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

                                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtSNOSetOrder_Click(Buy4): -> " + vbCrLf, True) Then
                                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                        Else
                                            ClsMsgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                        End If

                                    End If

                                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtSNOSetOrder_Click(Buy5): -> " + vbCrLf, True) Then
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

                        Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance to open an order." + vbCrLf + "do you like to create the order offchain?", "not enough balance", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Question)

                        If Result = ClsMsgs.CustomDialogResult.Yes Then

                            Dim Masterkeys As List(Of String) = GetPassPhrase()
                            '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                            If Masterkeys.Count > 0 Then
                                Dim T_Method As String = GetINISetting(E_Setting.PaymentType, "")
                                C_OffchainBuyOrder = New S_OffchainBuyOrder("0", GlobalPublicKey, "WantToBuy", Amount, ItemAmount, Item, T_Method)
                                DEXNET.BroadcastMessage("<SCID>0</SCID><Ask>WantToBuy</Ask><Amount>" + Amount.ToString + "</Amount><XAmount>" + ItemAmount.ToString + "</XAmount><XItem>" + Item + "</XItem><Method>" + T_Method + "</Method>", Masterkeys(1), Masterkeys(2), Masterkeys(0))
                            Else
                                TSSCryptStatus_Click(Nothing, Nothing)
                                Masterkeys = GetPassPhrase()
                                If Masterkeys.Count > 0 Then
                                    'DEXNET.AddRelevantKey("<Answer>")
                                    Dim T_Method As String = GetINISetting(E_Setting.PaymentType, "")
                                    C_OffchainBuyOrder = New S_OffchainBuyOrder("0", GlobalPublicKey, "WantToBuy", Amount, ItemAmount, Item, T_Method)
                                    DEXNET.BroadcastMessage("<SCID>0</SCID><Ask>WantToBuy</Ask><Amount>" + Amount.ToString + "</Amount><XAmount>" + ItemAmount.ToString + "</XAmount><XItem>" + Item + "</XItem><Method>" + T_Method + "</Method>", Masterkeys(1), Masterkeys(2), Masterkeys(0))

                                Else
                                    ClsMsgs.MBox("offchain order canceled", "jour offchain order was canceled.",,, ClsMsgs.Status.Warning, 3, ClsMsgs.Timer_Type.AutoOK)
                                End If
                            End If

                        Else
                            ClsMsgs.MBox("offchain order canceled", "jour offchain order was canceled.",,, ClsMsgs.Status.Warning, 3, ClsMsgs.Timer_Type.AutoOK)
                        End If

                    End If

                End If

            Else

                If RBSNOBuy.Checked Then

                    If AccAmount = 0.0 Then
                        Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance to open an order." + vbCrLf + "do you like to create the order offchain?", "not enough balance", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Question)

                        If Result = ClsMsgs.CustomDialogResult.Yes Then

                            Dim Masterkeys As List(Of String) = GetPassPhrase()
                            '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                            If Masterkeys.Count > 0 Then
                                Dim T_Method As String = GetINISetting(E_Setting.PaymentType, "")
                                C_OffchainBuyOrder = New S_OffchainBuyOrder("0", GlobalPublicKey, "WantToBuy", Amount, ItemAmount, Item, T_Method)
                                DEXNET.BroadcastMessage("<SCID>0</SCID><Ask>WantToBuy</Ask><Amount>" + Amount.ToString + "</Amount><XAmount>" + ItemAmount.ToString + "</XAmount><XItem>" + Item + "</XItem><Method>" + T_Method + "</Method>", Masterkeys(1), Masterkeys(2), Masterkeys(0))
                            End If

                        End If

                    Else
                        ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                        BtSNOSetOrder.Text = "Set Order"
                        BtSNOSetOrder.Enabled = True
                        Exit Sub
                    End If

                Else
                    ClsMsgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, ClsMsgs.Status.Information)
                    BtSNOSetOrder.Text = "Set Order"
                    BtSNOSetOrder.Enabled = True
                    Exit Sub

                End If

            End If

        Catch ex As Exception
            ClsMsgs.MBox(ex.Message, "Error",,, ClsMsgs.Status.Erro)
        End Try

        BtSNOSetOrder.Text = "Set Order"
        BtSNOSetOrder.Enabled = True

    End Sub

    Private Sub LVSellorders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVSellorders.MouseUp
        BtBuy.Text = "Buy"
        LVSellorders.ContextMenuStrip = Nothing

        If LVSellorders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVSellorders.SelectedItems(0).Tag, ClsDEXContract)

            If T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then
                BtBuy.Text = "cancel"
            Else

                'Dim SignumApi As ClsSignumAPI = New ClsSignumAPI("")

                Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                LVCMItemSellerPubKey.Text = "copy seller public key"
                LVCMItemSellerPubKey.Tag = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress)

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

            Dim T_DEXContract As ClsDEXContract = TryCast(LVBuyorders.SelectedItems(0).Tag, ClsDEXContract)
            Dim T_OffchainOrder As S_OffchainBuyOrder = New S_OffchainBuyOrder(,,,,,,)
            If T_DEXContract Is Nothing Then
                T_OffchainOrder = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_OffchainBuyOrder)
            End If

            If T_DEXContract Is Nothing Then

                'TODO: Offchain Rightclickmenu
                If T_OffchainOrder.PubKey = GlobalPublicKey Then
                    'Me
                    BtSell.Text = "cancel"
                Else
                    'The other
                    BtSell.Text = "Sell"
                End If

            Else

                If T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then
                    BtSell.Text = "cancel"
                Else

                    'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")

                    Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

                    Dim LVCMItemSellerPubKey As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItemSellerPubKey.Text = "copy buyer public key"
                    LVCMItemSellerPubKey.Tag = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress)

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


            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

            Dim Collateral As Double = T_DEXContract.CurrentInitiatorsCollateral


            If Not T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then

                Dim BalList As List(Of String) = C_SignumAPI.GetBalance(TBSNOAddress.Text)

                Dim Available As Double = 0.0
                Dim AvaStr As String = GetStringBetweenFromList(BalList, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Collateral + ClsSignumAPI.Planck2Dbl(ClsDEXContract._GasFeeNQT) And Available > 0.0 Then

                    Dim MBoxMsg As String = "Do you really want to Buy " + Dbl2LVStr(T_DEXContract.CurrentBuySellAmount) + " Signa for " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " "
                    MBoxMsg += "from Seller: " + T_DEXContract.CurrentInitiatorAddress + "?" + vbCrLf + vbCrLf
                    MBoxMsg += "collateral: " + Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) + " Signa" + vbCrLf
                    MBoxMsg += "gas fees: " + Dbl2LVStr(ClsSignumAPI.Planck2Dbl(ClsDEXContract._GasFeeNQT)) + " Signa" + vbCrLf + vbCrLf
                    MBoxMsg += "this transaction will take effect in 1-3 Blocks (4-12 minutes)"

                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox(MBoxMsg, "Buy Order from Smart Contract: " + T_DEXContract.Address, ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()

                        If MasterKeys.Count > 0 Then

                            Dim Response As String = T_DEXContract.AcceptSellOrder(MasterKeys(0), Collateral, C_Fee,)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(1): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(2): -> " + vbCrLf, True) Then
                                    MBoxMsg = "SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX + vbCrLf + vbCrLf
                                    MBoxMsg += "please wait 1-2 Blocks (4-8 minutes) to get the payment-infos from Seller" 'TODO: only if autoinfo is on

                                    ClsMsgs.MBox(MBoxMsg, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            Dim Response As String = ""

                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.AcceptSellOrder(GlobalPublicKey, Collateral, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX

                                End If
                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.AcceptSellOrder(PinForm.PublicKey, Collateral, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(2a): -> " + vbCrLf, True) Then

                                    Dim UTX As String = Response

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(2b): -> " + vbCrLf, True) Then

                                        MBoxMsg = "SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX + vbCrLf + vbCrLf
                                        MBoxMsg += "please wait 1-2 Blocks (4-8 minutes) to get the payment-infos from Seller" 'TODO: only if autoinfo is on

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
                    Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("it seems you have not enough balance for the Smart Contract acception." + vbCrLf + "do you like to ask the seller for accepting this offer offchain?", "not enough balance", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Question)

                    If Result = ClsMsgs.CustomDialogResult.Yes Then
                        'TODO: Check RecipientPublicKey

                        Dim Masterkeys As List(Of String) = GetPassPhrase()
                        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                        If Masterkeys.Count > 0 Then
                            DEXNET.BroadcastMessage("<SCID>" + T_DEXContract.ID.ToString + "</SCID><Ask>AcceptOrderRequest</Ask><AnswerPublicKey>" + DEXNET.DEXNET_PublicKeyHEX + "</AnswerPublicKey>", Masterkeys(1), Masterkeys(2), Masterkeys(0), C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress))
                        Else
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.EnterPINOnly)
                            PinForm.ShowDialog()

                            Masterkeys = GetPassPhrase()

                            If Masterkeys.Count > 0 Then
                                DEXNET.BroadcastMessage("<SCID>" + T_DEXContract.ID.ToString + "</SCID><Ask>AcceptOrderRequest</Ask><AnswerPublicKey>" + DEXNET.DEXNET_PublicKeyHEX + "</AnswerPublicKey>", Masterkeys(1), Masterkeys(2), Masterkeys(0), C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(T_DEXContract.CurrentInitiatorAddress))
                                GlobalPIN = ""
                            Else
                                ClsMsgs.MBox("request canceled", "jour request was canceled.",,, ClsMsgs.Status.Warning, 3, ClsMsgs.Timer_Type.AutoOK)
                            End If

                        End If
                    Else
                        ClsMsgs.MBox("request canceled", "jour request was canceled.",,, ClsMsgs.Status.Warning, 3, ClsMsgs.Timer_Type.AutoOK)

                    End If

                End If

            Else

                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the SellOrder?", "Cancel SellOrder?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                    Dim MasterKeys As List(Of String) = GetPassPhrase()

                    If MasterKeys.Count > 0 Then

                        Dim Response As String = T_DEXContract.AcceptSellOrder(MasterKeys(0), 1.0, C_Fee)

                        If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(cancel1): -> " + vbCrLf, True) Then

                            Dim UTX As String = Response
                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                            Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(cancel2): -> " + vbCrLf, True) Then
                                ClsMsgs.MBox("SellOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                            End If

                        End If

                    Else

                        Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                        Dim Response As String = ""
                        If Not GlobalPublicKey.Trim = "" Then

                            Response = T_DEXContract.AcceptSellOrder(GlobalPublicKey, 1.0, C_Fee)

                            If IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(cancel2a): -> " + vbCrLf, True) Then
                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                            Else
                                Dim UTX As String = Response
                                PinForm.TBUnsignedBytes.Text = UTX

                            End If
                        End If

                        PinForm.ShowDialog()

                        If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                            Response = T_DEXContract.AcceptSellOrder(PinForm.PublicKey, 1.0, C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtBuy_Click(cancel1a): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response

                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(cancel1b): -> " + vbCrLf, True) Then
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


            Dim T_DEXContract As ClsDEXContract = TryCast(LVBuyorders.SelectedItems(0).Tag, ClsDEXContract)
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)
            Dim T_OffchainOrder As S_OffchainBuyOrder = New S_OffchainBuyOrder(,,,,,,)
            If T_DEXContract Is Nothing Then
                T_OffchainOrder = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_OffchainBuyOrder)
            End If

            If T_DEXContract Is Nothing Then

                If T_OffchainOrder.PubKey = GlobalPublicKey Then
                    'Me

                    C_OffchainBuyOrder = New S_OffchainBuyOrder(,,,,,,)
                    Dim Masterkeys As List(Of String) = GetPassPhrase()
                    '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                    DEXNET.BroadcastMessage("<SCID>0</SCID><Ask>CancelBuyOrder</Ask>", Masterkeys(1), Masterkeys(2), Masterkeys(0))
                    SetFitlteredPublicOrdersInLVs()

                Else
                    'The other

                    Dim founded As Boolean = False

                    For Each T_DEXCo As ClsDEXContract In C_DEXContractList

                        If T_DEXCo.PendingResponderID = GetAccountID(T_OffchainOrder.PubKey) Then
                            'already pending
                            founded = True
                            Exit For
                        End If

                    Next

                    If Not founded Then

                        If LVOpenChannels.Items.Count > 0 Then

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
                                        If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Or (T_DEXContract.Deniability And Not ChBxDeniability.Checked) Or (Not T_DEXContract.Deniability And ChBxDeniability.Checked) Then
                                            T_DEXContract = Nothing
                                        Else
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If

                            If T_DEXContract Is Nothing Then
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

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)

                            Dim AccAmount As Double = GetDoubleBetweenFromList(C_SignumAPI.GetBalance(TBSNOAddress.Text), "<available>", "</available>")

                            Dim Recipient As ULong = GetAccountID(T_OffchainOrder.PubKey)
                            Dim Amount As Double = T_OffchainOrder.Amount
                            Dim Collateral As Double = 0.0
                            Dim Item As String = T_OffchainOrder.XItem
                            Dim ItemAmount As Double = T_OffchainOrder.XAmount


                            If AccAmount > Amount + C_Fee + Collateral Then
                                'enough balance

                                Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to create a new SellOrder?" + vbCrLf + vbCrLf + "Amount: " + Dbl2LVStr(Amount, 8) + " SIGNA" + vbCrLf + "XItem: " + Dbl2LVStr(ItemAmount, C_Decimals) + " " + Item, "Create SellOrder", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                                If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                                    Dim MasterKeys As List(Of String) = GetPassPhrase()

                                    Dim TX As String = ""

                                    If MasterKeys.Count > 0 Then
                                        Dim Response As String = T_DEXContract.CreateOrderWithResponder(MasterKeys(0), Amount, Recipient, Item, ItemAmount) '.CreateSellOrder(MasterKeys(0), Amount, Collateral, Item, ItemAmount, Fee)

                                        If IsErrorOrWarning(Response) Then
                                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                        Else

                                            Dim UTX As String = Response
                                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                            TX = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                            If IsErrorOrWarning(TX, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell1): -> " + vbCrLf, True) Then
                                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                            Else
                                                ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                            End If

                                        End If

                                    Else 'no Masterkeys

                                        Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                                        Dim Response As String = ""

                                        If Not GlobalPublicKey.Trim = "" Then

                                            Response = T_DEXContract.CreateOrderWithResponder(GlobalPublicKey, Amount, Recipient, Item, ItemAmount)

                                            If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell2): -> " + vbCrLf, True) Then
                                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                            Else

                                                Dim UTX As String = Response
                                                PinForm.TBUnsignedBytes.Text = UTX

                                            End If
                                        End If

                                        PinForm.ShowDialog()

                                        If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                            Response = T_DEXContract.CreateOrderWithResponder(PinForm.PublicKey, Amount, Recipient, Item, ItemAmount)

                                            If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSNOSetOrder_Click(Sell3): -> " + vbCrLf, True) Then
                                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                            Else

                                                Dim UTX As String = Response

                                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                                TX = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                                If IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(Sell4): -> " + vbCrLf, True) Then
                                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                                Else
                                                    ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                                End If

                                            End If

                                        ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                                            TX = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                                            If IsErrorOrWarning(TX, Application.ProductName + "-error in BtBuy_Click(Sell5): -> " + vbCrLf, True) Then
                                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                            Else
                                                ClsMsgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                                            End If

                                        Else
                                            ClsMsgs.MBox("SellOrder creation canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                                        End If

                                    End If

                                    'TODO: send msg to buyer and his pubkey to blockchain

                                    Dim PayInfo As String = T_OffchainOrder.Method

                                    If Not PayInfo.Trim = "" Then

                                        If PayInfo.Contains("PayPal-E-Mail=") Then
                                            Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                            Dim ColWordsString As String = ColWords.GenerateColloquialWords(TX, True, "-", 5)

                                            PayInfo += " Reference/Note=" + ColWordsString
                                        End If

                                        Dim TXr As String = T_Interactions.SendBillingInfos(Recipient, PayInfo, True, True, T_OffchainOrder.PubKey) 'TODO: need offchain pubkey

                                        ClsMsgs.MBox("Billing info send" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information)

                                    End If


                                End If

                            Else
                                'not enough balance
                                ClsMsgs.MBox("not enough balance", "Error",,, ClsMsgs.Status.Erro)

                            End If

                        End If

                    End If

                End If

            Else ' DEXContract <> Nothing

                Dim PayInfo As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Method", LVBuyorders.SelectedItems(0)))
                Dim AutoInfo As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Autoinfo", LVBuyorders.SelectedItems(0)))
                Dim Autofinish As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Autofinish", LVBuyorders.SelectedItems(0)))

                If T_DEXContract.CheckForUTX Then
                    ClsMsgs.MBox("One Transaction is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                    BtSell.Text = OldTxt
                    BtSell.Enabled = True
                    Exit Sub
                End If


                'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

                Dim BuyAmount As Double = T_DEXContract.CurrentBuySellAmount
                Dim XItem As String = T_DEXContract.CurrentXItem
                Dim XAmount As Double = T_DEXContract.CurrentXAmount


                If Not T_DEXContract.CurrentInitiatorAddress = TBSNOAddress.Text Then

                    Dim BalList As List(Of String) = C_SignumAPI.GetBalance(TBSNOAddress.Text)

                    Dim Available As Double = 0.0
                    Dim AvaStr As String = GetStringBetweenFromList(BalList, "<available>", "</available>")

                    If AvaStr.Trim = "" Then

                    Else
                        Available = Val(AvaStr.Replace(",", "."))
                    End If

                    If Available > BuyAmount + 1.0 Then

                        Dim MBoxMsg As String = "Do you really want to Sell " + Dbl2LVStr(T_DEXContract.CurrentBuySellAmount) + " Signa for " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " "
                        MBoxMsg += "to Buyer: " + T_DEXContract.CurrentInitiatorAddress + "?" + vbCrLf + vbCrLf
                        MBoxMsg += "collateral: " + Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) + " Signa" + vbCrLf
                        MBoxMsg += "gas fees: " + Dbl2LVStr(ClsSignumAPI.Planck2Dbl(ClsDEXContract._GasFeeNQT)) + " Signa" + vbCrLf + vbCrLf
                        MBoxMsg += "this transaction will take effect in 1-2 Blocks (4-8 minutes)" + vbCrLf

                        If AutoInfo = "True" Then
                            MBoxMsg += "you will also inform the buyer with payment info!" + vbCrLf
                        End If

                        If Autofinish = "True" Then
                            MBoxMsg += "you also accept the Autofinishing!" + vbCrLf
                        End If


                        Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox(MBoxMsg, "Sell Order to Smart Contract: " + T_DEXContract.Address, ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                        If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                            Dim MasterKeys As List(Of String) = GetPassPhrase()

                            If MasterKeys.Count > 0 Then

                                Dim Response As String = T_DEXContract.AcceptBuyOrder(MasterKeys(0),,, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(1): -> " + vbCrLf, True) Then

                                    Dim UTX As String = Response
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtSell_Click(2): -> " + vbCrLf, True) Then

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

                                        If Not T_DEXContract.CurrencyIsCrypto() Then

                                            'TODO: OwnPayType 
                                            Dim OwnPayType As String = GetINISetting(E_Setting.PaymentType, "Self Pickup")

                                            If PayInfo.Contains(OwnPayType) Then

                                                If PayInfo.Contains("PayPal-E-Mail") Then

                                                    Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                    Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                    PayInfo = "PayPal-E-Mail=" + GetINISetting(E_Setting.PayPalEMail, "test@test.com") + " Reference/Note=" + ColWordsString

                                                    'ElseIf PayInfo.Contains("PayPal-Order") Then

                                                    '    Dim APIOK As String = CheckPayPalAPI()

                                                    '    If APIOK = "True" Then
                                                    '        Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal
                                                    '        PPAPI_Autoinfo.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                                                    '        PPAPI_Autoinfo.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                                                    '        Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", BuyAmount, XAmount, T_DEXContract.CurrentXItem)
                                                    '        Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
                                                    '        PayInfo = "PayPal-Order=" + PPOrderID
                                                    '    End If

                                                End If

                                                Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(XAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo

                                                Dim TXr As String = T_Interactions.SendBillingInfos(T_DEXContract.CurrentInitiatorID, T_MsgStr, True, True)

                                                If Not IsErrorOrWarning(TXr) Then
                                                    MBoxMsg += "Payment instructions sended" + vbCrLf
                                                    MBoxMsg += "TX: " + TXr + vbCrLf + vbCrLf
                                                    MBoxMsg += "Recipient: " + T_DEXContract.CurrentInitiatorAddress + vbCrLf
                                                    MBoxMsg += "SmartContract: " + T_DEXContract.Address + vbCrLf
                                                    MBoxMsg += "Order-Transaction: " + T_DEXContract.CurrentCreationTransaction.ToString + vbCrLf
                                                    MBoxMsg += "payment request: " + Dbl2LVStr(XAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + vbCrLf
                                                    MBoxMsg += PayInfo + vbCrLf + vbCrLf
                                                    MBoxMsg += "please wait for requested payment from buyer"
                                                End If

                                            End If

                                        Else
                                            'AtomicSwap: send XItem address to buyer

                                            If T_DEXContract.CurrentXItem = "BTC" Then



                                            End If

                                        End If

                                        ClsMsgs.MBox(MBoxMsg, "Transaction(s) created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)

                                    End If

                                End If

                            Else

                                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                                Dim Response As String = ""
                                If Not GlobalPublicKey.Trim = "" Then

                                    Response = T_DEXContract.AcceptBuyOrder(GlobalPublicKey,,, C_Fee)

                                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(1aa): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                    Else
                                        Dim UTX As String = Response
                                        PinForm.TBUnsignedBytes.Text = UTX

                                    End If
                                End If

                                PinForm.ShowDialog()

                                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                    Response = T_DEXContract.AcceptBuyOrder(PinForm.PublicKey,,, C_Fee)

                                    If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(1a): -> " + vbCrLf, True) Then

                                        Dim UTX As String = Response

                                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                        If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtSell_Click(1b): -> " + vbCrLf, True) Then

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


                                            If Not T_DEXContract.CurrencyIsCrypto() Then
                                                'TODO: OwnPayType 

                                                Dim OwnPayType As String = GetINISetting(E_Setting.PaymentType, "Self Pickup")

                                                If PayInfo.Contains(OwnPayType) Then

                                                    If PayInfo.Contains("PayPal-E-Mail") Then

                                                        Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                        Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                        PayInfo = "PayPal-E-Mail=" + GetINISetting(E_Setting.PayPalEMail, "test@test.com") + " Reference/Note=" + ColWordsString

                                                        'ElseIf PayInfo.Contains("PayPal-Order") Then

                                                        '    Dim APIOK As String = CheckPayPalAPI()

                                                        '    If APIOK = "True" Then
                                                        '        Dim PPAPI_Autoinfo As ClsPayPal = New ClsPayPal
                                                        '        PPAPI_Autoinfo.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
                                                        '        PPAPI_Autoinfo.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

                                                        '        Dim PPOrderIDList As List(Of String) = PPAPI_Autoinfo.CreateOrder("Signa", BuyAmount, XAmount, T_DEXContract.CurrentXItem)
                                                        '        Dim PPOrderID As String = GetStringBetweenFromList(PPOrderIDList, "<id>", "</id>")
                                                        '        PayInfo = "PayPal-Order=" + PPOrderID
                                                        '    End If


                                                    End If

                                                    Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(XAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo

                                                    Dim TXr As String = T_Interactions.SendBillingInfos(T_DEXContract.CurrentInitiatorID, T_MsgStr, True, True)

                                                    If Not IsErrorOrWarning(TXr) Then

                                                        MBoxMsg += "Payment instructions sended" + vbCrLf
                                                        MBoxMsg += "TX: " + TXr + vbCrLf + vbCrLf
                                                        MBoxMsg += "Recipient: " + T_DEXContract.CurrentInitiatorAddress + vbCrLf
                                                        MBoxMsg += "SmartContract: " + T_DEXContract.Address + vbCrLf
                                                        MBoxMsg += "Order-Transaction: " + T_DEXContract.CurrentCreationTransaction.ToString + vbCrLf
                                                        MBoxMsg += "payment request: " + Dbl2LVStr(XAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + vbCrLf
                                                        MBoxMsg += PayInfo + vbCrLf + vbCrLf
                                                        MBoxMsg += "please wait for requested payment from buyer"
                                                    End If

                                                End If

                                            Else
                                                'AtomicSwap: send XItem address to buyer


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

                            Dim Response As String = T_DEXContract.AcceptBuyOrder(MasterKeys(0), 1.0, 1.0, C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(cancel1): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtSell_Click(cancel2): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("BuyOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                            Dim Response As String = ""
                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.AcceptBuyOrder(GlobalPublicKey,,, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(cancel2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX

                                End If
                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.AcceptBuyOrder(PinForm.PublicKey,,, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtSell_Click(cancel1a): -> " + vbCrLf, True) Then
                                    Dim UTX As String = Response

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtSell_Click(cancel1b): -> " + vbCrLf, True) Then
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

        End If

        BtSell.Text = OldTxt
        BtSell.Enabled = True

    End Sub

#Region "Filter"

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

    Private Sub ChBxFilter_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxSellFilterShowAutoinfo.CheckedChanged, ChBxSellFilterShowAutofinish.CheckedChanged, ChBxSellFilterShowPayable.CheckedChanged, ChBxBuyFilterShowAutoinfo.CheckedChanged, ChBxBuyFilterShowAutofinish.CheckedChanged, ChBxBuyFilterShowPayable.CheckedChanged, ChBxBuyFilterShowOffChainOrders.CheckedChanged

        SetFitlteredPublicOrdersInLVs()

        If Not sender Is Nothing Then
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
                Case ChBxBuyFilterShowOffChainOrders.Name
                    SetINISetting(E_Setting.BuyFilterOffchainOrders, ChBxBuyFilterShowOffChainOrders.Checked)

                    If ChBxBuyFilterShowOffChainOrders.Checked Then
                        If Not DEXNET Is Nothing Then
                            DEXNET.AddRelevantKey("<Ask>WantToBuy</Ask>", True, "PublicKey")
                            DEXNET.AddRelevantKey("<Ask>CancelBuyOrder</Ask>", True)

                        End If
                    Else
                        If Not DEXNET Is Nothing Then
                            DEXNET.DelRelevantKey("<Ask>WantToBuy</Ask>")
                            DEXNET.DelRelevantKey("<Ask>CancelBuyOrder</Ask>")
                            C_BuyOrderLVOffChainEList.Clear()

                        End If
                    End If

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

    Private Sub SetMethodFilter(ByVal MethodFilterListBox As CheckedListBox, ByVal CheckedMethods As List(Of String))

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

        If Not sender Is Nothing Then
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

#End Region 'Filter

#End Region 'Marketdetails - Controls

#Region "MyOrders - Controls"

#Region "MySmartContracts - Controls"

    Private Sub LVMySmartContracts_MouseDown(sender As Object, e As MouseEventArgs) Handles LVMySmartContracts.MouseDown

        LVMySmartContracts.ContextMenuStrip = Nothing

        BtMediatorDeActivateDeniability.Visible = False

        LabProposalPercent.Visible = False
        NUDMediatePercentage.Visible = False
        BtMediateDispute.Visible = False

        LabMediatorMsg.Visible = False
        TBMediatorManuMsg.Visible = False

        ChBxMediatorEncMsg.Visible = False
        CoBxMediatorCandidateToSend.Visible = False
        BtSendMediatorMsg.Visible = False

    End Sub
    Private Sub LVMySmartContracts_MouseUp(sender As Object, e As MouseEventArgs) Handles LVMySmartContracts.MouseUp
        LVMySmartContracts.ContextMenuStrip = Nothing
        SplitContainer20.Panel2Collapsed = True

        If LVMySmartContracts.SelectedItems.Count > 0 Then

            SplitContainer20.Panel2Collapsed = False
            Dim LVi As ListViewItem = LVMySmartContracts.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            'Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            'Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            'LVCMItem.Text = "copy status"
            'LVCMItem.Tag = Status

            'AddHandler LVCMItem.Click, AddressOf Copy2CB
            'LVContextMenu.Items.Add(LVCMItem)

            LabProposalPercent.Visible = T_DexContract.Dispute
            NUDMediatePercentage.Visible = T_DexContract.Dispute
            BtMediateDispute.Visible = T_DexContract.Dispute


            CoBxMediatorCandidateToSend.Items.Clear()

            If T_DexContract.Status = ClsDEXContract.E_Status.DISPUTE Then
                'Controls to handle the Dispute on Smart Contract
                LabProposalPercent.Visible = True
                NUDMediatePercentage.Visible = True
                BtMediateDispute.Visible = True
            End If


            If T_DexContract.Status = ClsDEXContract.E_Status.OPEN Or T_DexContract.Status = ClsDEXContract.E_Status.DISPUTE Or T_DexContract.Status = ClsDEXContract.E_Status.RESERVED Or T_DexContract.Status = ClsDEXContract.E_Status.TX_PENDING Or T_DexContract.Status = ClsDEXContract.E_Status.UTX_PENDING Then
                'Controls to send messages to Candidates
                LabMediatorMsg.Visible = True
                TBMediatorManuMsg.Visible = True
                ChBxMediatorEncMsg.Visible = True
                CoBxMediatorCandidateToSend.Visible = True
                BtSendMediatorMsg.Visible = True
            End If

            If T_DexContract.Status = ClsDEXContract.E_Status.FREE Then
                'Control to handle the Deniability of the Smart Contract
                BtMediatorDeActivateDeniability.Visible = True
            End If

            If T_DexContract.CurrentSellerAddress.Trim <> "" Then
                CoBxMediatorCandidateToSend.Items.Add(T_DexContract.CurrentSellerAddress)
                CoBxMediatorCandidateToSend.SelectedItem = CoBxMediatorCandidateToSend.Items(0)
            End If

            If T_DexContract.CurrentBuyerAddress.Trim <> "" Then
                CoBxMediatorCandidateToSend.Items.Add(T_DexContract.CurrentBuyerAddress)
                CoBxMediatorCandidateToSend.SelectedItem = CoBxMediatorCandidateToSend.Items(0)
            End If

            'LVMyOpenOrders.ContextMenuStrip = LVContextMenu
        End If

    End Sub

    Private Sub BtMediatorRefuelGas_Click(sender As Object, e As EventArgs) Handles BtMediatorRefuelGas.Click

        If LVMySmartContracts.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMySmartContracts.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            BtMediatorRefuelGas.Text = "Wait..."
            BtMediatorRefuelGas.Enabled = False

            Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to lubricate your smart contract?", "Refuelling Contract", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

            If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
                Dim MasterKeys As List(Of String) = GetPassPhrase()

                If MasterKeys.Count > 0 Then

                    Dim Response As String = T_DexContract.SendGasFee(MasterKeys(0), 1.0,)

                    If IsErrorOrWarning(Response) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else

                        Dim UTX As String = Response
                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorRefuelGas_Click(1): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Refuel gas send" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    End If

                Else 'Masterkeys = 0

                    Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                    Dim Response As String = ""

                    If Not GlobalPublicKey.Trim = "" Then

                        Response = T_DexContract.SendGasFee(GlobalPublicKey, 1.0,)

                        If IsErrorOrWarning(Response, Application.ProductName + "-error in BtMediatorRefuelGas_Click(2): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            Dim UTX As String = Response
                            PinForm.TBUnsignedBytes.Text = UTX
                        End If
                    End If

                    PinForm.ShowDialog()

                    If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                        Response = T_DexContract.SendGasFee(PinForm.PublicKey, 1.0,)

                        If IsErrorOrWarning(Response, Application.ProductName + "-error in BtMediatorRefuelGas_Click(3): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else

                            Dim UTX As String = Response

                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                            Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorRefuelGas_Click(4): -> " + vbCrLf, True) Then
                                ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                            Else
                                ClsMsgs.MBox("Refuel gas send" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                            End If

                        End If

                    ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                        Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorRefuelGas_Click(5): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Refuel gas send" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    Else
                        ClsMsgs.MBox("Refuel gas send canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                    End If

                End If

            End If

            BtMediatorRefuelGas.Text = "refuel gas"
            BtMediatorRefuelGas.Enabled = True

        End If

    End Sub

    Private Sub BtMediatorDeActivateDeniability_Click(sender As Object, e As EventArgs) Handles BtMediatorDeActivateDeniability.Click

        If LVMySmartContracts.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMySmartContracts.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            BtMediatorDeActivateDeniability.Text = "Wait..."
            BtMediatorDeActivateDeniability.Enabled = False

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
            Dim MasterKeys As List(Of String) = GetPassPhrase()

            If MasterKeys.Count > 0 Then
                Dim Response As String = T_DexContract.DeActivateDeniability(MasterKeys(0),,)

                If IsErrorOrWarning(Response) Then
                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                Else

                    Dim UTX As String = Response
                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorDeActivateDeniability_Click(1): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Toggle Deniability" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                End If

            Else 'Masterkeys = 0

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                Dim Response As String = ""

                If Not GlobalPublicKey.Trim = "" Then

                    Response = T_DexContract.DeActivateDeniability(GlobalPublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtMediatorDeActivateDeniability_Click(2): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        Dim UTX As String = Response
                        PinForm.TBUnsignedBytes.Text = UTX
                    End If
                End If

                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                    Response = T_DexContract.DeActivateDeniability(PinForm.PublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtMediatorDeActivateDeniability_Click(3): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else

                        Dim UTX As String = Response

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorDeActivateDeniability_Click(4): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Toggle deniability" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    End If

                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtMediatorDeActivateDeniability_Click(5): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Toggle deniability" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                Else
                    ClsMsgs.MBox("Toggle deniability canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                End If

            End If

            BtMediatorDeActivateDeniability.Text = "de/activate Deniability"
            BtMediatorDeActivateDeniability.Enabled = True

        End If

    End Sub
    Private Sub BtMediateDispute_Click(sender As Object, e As EventArgs) Handles BtMediateDispute.Click

        If LVMySmartContracts.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMySmartContracts.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            BtMediateDispute.Text = "Wait..."
            BtMediateDispute.Enabled = False

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
            Dim MasterKeys As List(Of String) = GetPassPhrase()

            If MasterKeys.Count > 0 Then
                Dim Response As String = T_DexContract.MediateDispute(MasterKeys(0), NUDMediatePercentage.Value,,)

                If IsErrorOrWarning(Response) Then
                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                Else

                    Dim UTX As String = Response
                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(1): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Proposal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                End If

            Else 'Masterkeys = 0

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                Dim Response As String = ""

                If Not GlobalPublicKey.Trim = "" Then

                    Response = T_DexContract.MediateDispute(GlobalPublicKey, NUDMediatePercentage.Value,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(2): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        Dim UTX As String = Response
                        PinForm.TBUnsignedBytes.Text = UTX
                    End If
                End If

                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                    Response = T_DexContract.MediateDispute(PinForm.PublicKey, NUDMediatePercentage.Value,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(3): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else

                        Dim UTX As String = Response

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(4): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Proposal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    End If

                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(5): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Proposal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                Else
                    ClsMsgs.MBox("Proposal canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                End If

            End If

            BtMediateDispute.Text = "mediate dispute"
            BtMediateDispute.Enabled = True

        End If

    End Sub

    Private Sub BtSendMediatorMsg_Click(sender As Object, e As EventArgs) Handles BtSendMediatorMsg.Click

        If LVMySmartContracts.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMySmartContracts.SelectedItems(0).Tag, ClsDEXContract)
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)

            Dim T_Infotext As String = "Infotext=" + TBManuMsg.Text.Replace(",", ";").Replace(":", ";").Replace("""", ";")
            Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + T_Infotext

            Dim Recipient As String = CoBxMediatorCandidateToSend.SelectedItem.ToString

            Dim TXr As String = T_Interactions.SendBillingInfos(Recipient, T_MsgStr, True, ChBxEncMsg.Checked)

            If Not IsErrorOrWarning(TXr) Then

                If ChBxEncMsg.Checked Then
                    ClsMsgs.MBox("encrypted Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                Else
                    ClsMsgs.MBox("public Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

                TBManuMsg.Text = ""

            End If

        End If

    End Sub

#End Region 'MySmartContracts - Controls

#Region "MyOpenOrders - Controls"

    Private Sub LVMyOpenOrders_MouseDown(sender As Object, e As MouseEventArgs) Handles LVMyOpenOrders.MouseDown

        LVMyOpenOrders.ContextMenuStrip = Nothing

        BtExecuteOrder.Visible = False
        BtOpenDispute.Visible = False
        BtAppeal.Visible = False
        BtExecuteWithChainSwapKey.Visible = False

        LabMsg.Visible = False
        TBManuMsg.Visible = False
        ChBxEncMsg.Visible = False
        CoBxCandidateToSend.Visible = False
        BtSendMsg.Visible = False

    End Sub
    Private Sub LVMyOpenOrders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVMyOpenOrders.MouseUp
        LVMyOpenOrders.ContextMenuStrip = Nothing

        SplitContainer16.Panel2Collapsed = True

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            SplitContainer16.Panel2Collapsed = False

            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)

            Dim Seller As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVi))
            Dim Buyer As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVi))
            Dim Infotext As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Infotext", LVi))

            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            If T_DexContract.CurrencyIsCrypto() Then
                BtExecuteWithChainSwapKey.Enabled = True
                If Buyer = "Me" Then
                    BtExecuteWithChainSwapKey.Text = "execute with chainswapkey"
                ElseIf Seller = "Me" Then
                    If T_DexContract.BlocksLeft <= 0 Then
                        BtExecuteWithChainSwapKey.Text = "cancel AtomicSwap"
                    Else
                        BtExecuteWithChainSwapKey.Enabled = False
                    End If
                End If

            Else
                BtExecuteWithChainSwapKey.Enabled = False
            End If

            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Infotext"
            LVCMItem.Tag = Infotext

            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            If Infotext.Contains("http") Then
                'only shows on buyer side

                If Buyer = "Me" Then

                    BtExecuteOrder.Text = "cancel Smart Contract"
                    BtExecuteOrder.Visible = True
                    BtPayOrder.Visible = True

                    Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem1.Text = "cancel Smart Contract"
                    AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                    LVContextMenu.Items.Add(LVCMItem1)

                    Dim LVCMItem2 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem2.Text = "pay Order"
                    AddHandler LVCMItem2.Click, AddressOf BtPayOrder_Click
                    LVContextMenu.Items.Add(LVCMItem2)

                ElseIf Seller = "Me" Then

                    BtExecuteOrder.Text = "fulfill Smart Contract"
                    BtExecuteOrder.Visible = True

                    Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                    LVCMItem1.Text = "fulfill Smart Contract"
                    AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                    LVContextMenu.Items.Add(LVCMItem1)

                End If

            ElseIf Infotext = "No Payment received" Then
                'only shows on seller side

                If Buyer = "Me" Then
                    BtExecuteOrder.Text = "cancel Smart Contract"

                    If Buyer.Trim = "" Or Seller.Trim = "" Then
                        BtExecuteOrder.Visible = False
                    Else
                        BtExecuteOrder.Visible = True
                        BtReCreatePayPalOrder.Visible = False
                        BtPayOrder.Visible = False

                        Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                        LVCMItem1.Text = "cancel Smart Contract"
                        AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                        LVContextMenu.Items.Add(LVCMItem1)

                    End If

                ElseIf Seller = "Me" Then

                    BtExecuteOrder.Text = "fulfill Smart Contract"

                    If Buyer.Trim = "" Or Seller.Trim = "" Then
                        BtExecuteOrder.Visible = False
                    Else
                        BtExecuteOrder.Visible = True
                        BtReCreatePayPalOrder.Visible = True
                        BtPayOrder.Visible = False

                        Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem
                        LVCMItem1.Text = "fulfill Smart Contract"
                        AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                        LVContextMenu.Items.Add(LVCMItem1)

                    End If

                End If


            Else
                'shows on both sides

                BtPayOrder.Visible = False
                Dim LVCMItem1 As ToolStripMenuItem = New ToolStripMenuItem

                If Buyer = "Me" Then
                    BtExecuteOrder.Text = "cancel Smart Contract"
                    LVCMItem1.Text = "cancel Smart Contract"
                ElseIf Seller = "Me" Then
                    BtExecuteOrder.Text = "fulfill Smart Contract"
                    LVCMItem1.Text = "fulfill Smart Contract"
                End If

                If Buyer.Trim = "" Or Seller.Trim = "" Then
                    BtExecuteOrder.Text = "cancel Smart Contract"
                    LVCMItem1.Text = "cancel Smart Contract"
                    BtExecuteOrder.Visible = True

                Else

                    BtExecuteOrder.Visible = True
                    BtOpenDispute.Visible = T_DexContract.Deniability And Not T_DexContract.Dispute

                    If T_DexContract.CurrentDisputeTimeout <> 0UL And Not T_DexContract.CurrencyIsCrypto Then
                        BtAppeal.Visible = True
                    ElseIf T_DexContract.CurrentDisputeTimeout <> 0UL And T_DexContract.CurrencyIsCrypto Then
                        BtExecuteWithChainSwapKey.Visible = True
                    End If



                    LabMsg.Visible = True
                    TBManuMsg.Visible = True
                    ChBxEncMsg.Visible = True
                    CoBxCandidateToSend.Visible = True

                    CoBxCandidateToSend.Items.Clear()
                    CoBxCandidateToSend.Items.Add(T_DexContract.CreatorAddress)

                    If Buyer = "Me" Then
                        CoBxCandidateToSend.Items.Add(T_DexContract.CurrentSellerAddress)
                    Else 'If Seller = "Me" Then
                        CoBxCandidateToSend.Items.Add(T_DexContract.CurrentBuyerAddress)
                    End If

                    CoBxCandidateToSend.SelectedItem = CoBxCandidateToSend.Items(CoBxCandidateToSend.Items.Count - 1)

                    BtSendMsg.Visible = True

                End If

                AddHandler LVCMItem1.Click, AddressOf BtExecuteOrder_Click
                LVContextMenu.Items.Add(LVCMItem1)

                Dim LVCMItem2 As ToolStripMenuItem = New ToolStripMenuItem

                LVCMItem2.Text = "show messages"
                AddHandler LVCMItem2.Click, AddressOf ShowCurrentChat
                LVContextMenu.Items.Add(LVCMItem2)
            End If

            LVMyOpenOrders.ContextMenuStrip = LVContextMenu

        End If

    End Sub

    Private Sub ShowCurrentChat(sender As Object, e As EventArgs)

        Dim T_Frm As Form = New Form With {.Name = "FrmMessage", .Text = "Messages", .StartPosition = FormStartPosition.CenterScreen, .Width = 800, .Icon = Me.Icon}
        Dim RTB As RichTextBox = New RichTextBox
        RTB.Dock = DockStyle.Fill

        Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)
        Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

        For Each ChatEntry As ClsDEXContract.S_Chat In T_DexContract.CurrentChat
            Dim DateTime As Date = ConvertULongMSToDate(ChatEntry.Timestamp)
            Dim DateTimeString As String = DateTime.ToShortDateString() + " " + DateTime.ToShortTimeString()
            RTB.AppendText("(" + DateTimeString + ") From " + ChatEntry.SenderAddress + " To " + ChatEntry.RecipientAddress + ":" + vbCrLf)
            RTB.AppendText(ChatEntry.Attachment + vbCrLf)
            RTB.AppendText(vbCrLf)
        Next

        T_Frm.Controls.Add(RTB)
        T_Frm.Show()

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


            If T_DEXContract.IsSellOrder Then ' .Type = "SellOrder" Then

                If T_DEXContract.CurrentBuyerAddress.Trim = "" Then
                    'cancel Smart Contract
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the Smart Contract?", "cancel Smart Contract?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                        If MasterKeys.Count > 0 Then

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = T_DEXContract.AcceptSellOrder(MasterKeys(0), 1.0, C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(1): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(2): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "Transaction: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                            Dim Response As String = ""

                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.AcceptSellOrder(GlobalPublicKey,, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX
                                End If
                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.AcceptSellOrder(PinForm.PublicKey,, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(3): -> " + vbCrLf, True) Then
                                    Dim UTX As String = Response

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(4): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If

                    End If

                Else
                    'execute Smart Contract
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to fulfill the Smart Contract?", "fulfill Smart Contract?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                        If MasterKeys.Count > 0 Then

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = T_DEXContract.FinishOrder(MasterKeys(0), C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(3): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(5): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                            Dim Response As String = ""

                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.FinishOrder(GlobalPublicKey, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX
                                End If

                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.FinishOrder(PinForm.PublicKey, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(6): -> " + vbCrLf, True) Then

                                    Dim UTX As String = Response
                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(7): -> " + vbCrLf, True) Then
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

                    'cancel Smart Contract
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to cancel the Smart Contract?", "cancel Smart Contract?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()


                        If MasterKeys.Count > 0 Then
                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = T_DEXContract.AcceptBuyOrder(MasterKeys(0), 1.0, 1.0, C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(8): -> " + vbCrLf, True) Then

                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(9): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                            Dim Response As String = ""

                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.AcceptBuyOrder(GlobalPublicKey,,, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX
                                End If

                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.AcceptBuyOrder(PinForm.PublicKey, 1.0, 1.0, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(10): -> " + vbCrLf, True) Then
                                    Dim UTX As String = Response

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(11): -> " + vbCrLf, True) Then
                                        ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                    End If

                                End If

                            Else
                                ClsMsgs.MBox("Order cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                            End If

                        End If

                    End If

                Else
                    'execute Smart Contract
                    Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Do you really want to fulfill the Smart Contract?", "fulfill Smart Contract?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

                    If MsgResult = ClsMsgs.CustomDialogResult.Yes Then

                        Dim MasterKeys As List(Of String) = GetPassPhrase()

                        If MasterKeys.Count > 0 Then

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim Response As String = T_DEXContract.FinishOrder(MasterKeys(0), C_Fee)

                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(12): -> " + vbCrLf, True) Then
                                Dim UTX As String = Response
                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(13): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                                End If

                            End If

                        Else

                            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                            Dim Response As String = ""

                            If Not GlobalPublicKey.Trim = "" Then

                                Response = T_DEXContract.FinishOrder(GlobalPublicKey, C_Fee)

                                If IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(2a): -> " + vbCrLf, True) Then
                                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                                Else
                                    Dim UTX As String = Response
                                    PinForm.TBUnsignedBytes.Text = UTX
                                End If

                            End If

                            PinForm.ShowDialog()

                            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                                Response = T_DEXContract.FinishOrder(PinForm.PublicKey, C_Fee)

                                If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteOrder_Click(14): -> " + vbCrLf, True) Then
                                    Dim UTX As String = Response

                                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                    If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteOrder_Click(15): -> " + vbCrLf, True) Then
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

    Private Sub BtExecuteWithChainSwapKey_Click(sender As Object, e As EventArgs) Handles BtExecuteWithChainSwapKey.Click
        'TODO: execute with chainswapkey / cancel the reservation

        Dim OldTxt As String = BtExecuteWithChainSwapKey.Text

        BtExecuteWithChainSwapKey.Text = "Wait..."
        BtExecuteWithChainSwapKey.Enabled = False

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, ClsDEXContract)
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)

            Dim Seller As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVMyOpenOrders.SelectedItems(0)))
            Dim Buyer As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVMyOpenOrders.SelectedItems(0)))

            If T_DEXContract.CheckForUTX Or T_DEXContract.CheckForTX Then
                ClsMsgs.MBox("One TX is already Pending for this Order", "Order not available",,, ClsMsgs.Status.Attention, 5, ClsMsgs.Timer_Type.AutoOK)
                BtExecuteWithChainSwapKey.Text = OldTxt
                BtExecuteWithChainSwapKey.Enabled = True
                Exit Sub
            End If

            If Seller = "Me" Then
                Dim MasterKeys As List(Of String) = GetPassPhrase()
                If MasterKeys.Count > 0 Then

                    'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                    Dim Response As String = T_Interactions.RejectResponder() ' T_DEXContract.RejectResponder(MasterKeys(0), Fee)

                    If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteWithChainSwapKey_Click(1): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "Transaction: " + Response, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                    End If

                Else

                    'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                    Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)

                    Dim Response As String = ""

                    If Not GlobalPublicKey.Trim = "" Then

                        Response = T_DEXContract.RejectResponder(GlobalPublicKey, C_Fee)

                        If IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteWithChainSwapKey_Click(2a): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            Dim UTX As String = Response
                            PinForm.TBUnsignedBytes.Text = UTX
                        End If
                    End If

                    PinForm.ShowDialog()

                    If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                        Response = T_DEXContract.RejectResponder(PinForm.PublicKey, C_Fee)

                        If Not IsErrorOrWarning(Response, Application.ProductName + "-error in BtExecuteWithChainSwapKey_Click(3): -> " + vbCrLf, True) Then
                            Dim UTX As String = Response

                            Dim SignumNET As ClsSignumNET = New ClsSignumNET
                            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                            Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                            If Not IsErrorOrWarning(TX, Application.ProductName + "-error in BtExecuteWithChainSwapKey_Click(4): -> " + vbCrLf, True) Then
                                ClsMsgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                            End If

                        End If

                    Else
                        ClsMsgs.MBox("Order cancelation aborted.", "Canceled",,, ClsMsgs.Status.Erro)
                    End If

                End If

            ElseIf Buyer = "Me" Then

                Dim XItem As AbsClsXItem = ClsXItemAdapter.NewXItem(T_DEXContract.CurrentXItem)
                Dim T_XItemTransactionID As String = XItem.GetXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction)
                If Not T_XItemTransactionID.Trim = "" Then

                    Dim ChainSwapHash As String = XItem.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTransactionID)
                    IsErrorOrWarning(XItem.GetBackXItemTransaction(T_XItemTransactionID, ChainSwapHash))

                End If

            End If

        End If

        BtExecuteWithChainSwapKey.Text = OldTxt
        BtExecuteWithChainSwapKey.Enabled = True

    End Sub

    Private Sub BtOpenDispute_Click(sender As Object, e As EventArgs) Handles BtOpenDispute.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            BtOpenDispute.Text = "Wait..."
            BtOpenDispute.Enabled = False

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
            Dim MasterKeys As List(Of String) = GetPassPhrase()

            If MasterKeys.Count > 0 Then
                Dim Response As String = T_DexContract.OpenDispute(MasterKeys(0),,)

                If IsErrorOrWarning(Response) Then
                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                Else

                    Dim UTX As String = Response
                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(1): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Dispute opened" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                End If

            Else 'Masterkeys = 0

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                Dim Response As String = ""

                If Not GlobalPublicKey.Trim = "" Then

                    Response = T_DexContract.OpenDispute(GlobalPublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(2): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        Dim UTX As String = Response
                        PinForm.TBUnsignedBytes.Text = UTX
                    End If

                End If

                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                    Response = T_DexContract.OpenDispute(PinForm.PublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(3): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else

                        Dim UTX As String = Response

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(4): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Dispute opened" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    End If

                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(5): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Dispute opened" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                Else
                    ClsMsgs.MBox("Open Dispute canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                End If

            End If

            BtOpenDispute.Text = "open dispute"
            BtOpenDispute.Enabled = True

        End If

    End Sub
    Private Sub BtAppeal_Click(sender As Object, e As EventArgs) Handles BtAppeal.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then
            Dim LVi As ListViewItem = LVMyOpenOrders.SelectedItems(0)
            Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

            BtAppeal.Text = "Wait..."
            BtAppeal.Enabled = False

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode,)
            Dim MasterKeys As List(Of String) = GetPassPhrase()

            If MasterKeys.Count > 0 Then
                Dim Response As String = T_DexContract.Appeal(MasterKeys(0),,)

                If IsErrorOrWarning(Response) Then
                    ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                Else

                    Dim UTX As String = Response
                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                    Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(1): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Appeal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                End If

            Else 'Masterkeys = 0

                Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
                Dim Response As String = ""

                If Not GlobalPublicKey.Trim = "" Then

                    Response = T_DexContract.Appeal(GlobalPublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(2): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        Dim UTX As String = Response
                        PinForm.TBUnsignedBytes.Text = UTX
                    End If

                End If

                PinForm.ShowDialog()

                If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" Then

                    Response = T_DexContract.Appeal(PinForm.PublicKey,,)

                    If IsErrorOrWarning(Response, Application.ProductName + "-error in BtOpenDispute_Click(3): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else

                        Dim UTX As String = Response

                        Dim SignumNET As ClsSignumNET = New ClsSignumNET
                        Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
                        Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                        If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(4): -> " + vbCrLf, True) Then
                            ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                        Else
                            ClsMsgs.MBox("Appeal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                        End If

                    End If

                ElseIf Not PinForm.TBSignedBytes.Text.Trim = "" Then

                    Dim TX As String = C_SignumAPI.BroadcastTransaction(PinForm.TBSignedBytes.Text.Trim)

                    If IsErrorOrWarning(TX, Application.ProductName + "-error in BtOpenDispute_Click(5): -> " + vbCrLf, True) Then
                        ClsMsgs.MBox("An error has occured." + vbCrLf + Response, "Error",,, ClsMsgs.Status.Erro)
                    Else
                        ClsMsgs.MBox("Appeal sended" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, ClsMsgs.Status.Information)
                    End If

                Else
                    ClsMsgs.MBox("Appeal canceled.", "Canceled",,, ClsMsgs.Status.Erro)
                End If

            End If

            BtAppeal.Text = "appeal"
            BtAppeal.Enabled = True

        End If

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
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)

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

                Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo
                Dim TXr As String = T_Interactions.SendBillingInfos(T_DEXContract.CurrentBuyerID, T_MsgStr, True, True)

                If Not IsErrorOrWarning(TXr) Then
                    ClsMsgs.MBox("New PayPal Order sended as encrypted Message", "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

            End If

        End If

    End Sub

    Private Sub BtSendMsg_Click(sender As Object, e As EventArgs) Handles BtSendMsg.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim T_DEXContract As ClsDEXContract = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, ClsDEXContract)
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)

            Dim T_Infotext As String = "Infotext=" + TBManuMsg.Text.Replace(",", ";").Replace(":", ";").Replace("""", ";")
            Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + T_Infotext

            Dim Recipient As String = CoBxCandidateToSend.SelectedItem.ToString

            'If T_DEXContract.CurrentBuyerAddress = TBSNOAddress.Text Then
            '    Recipient = T_DEXContract.CurrentSellerAddress
            'End If

            Dim TXr As String = T_Interactions.SendBillingInfos(Recipient, T_MsgStr, True, ChBxEncMsg.Checked)

            If Not IsErrorOrWarning(TXr) Then

                If ChBxEncMsg.Checked Then
                    ClsMsgs.MBox("encrypted Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                Else
                    ClsMsgs.MBox("public Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                End If

                TBManuMsg.Text = ""

            End If

        End If

    End Sub

#End Region 'MyOpenOrders - Controls

#Region "MyClosedOrders - Controls"
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


            'Dim LVCMItem3 As ToolStripMenuItem = New ToolStripMenuItem
            'LVCMItem3.Text = "show messages"
            'AddHandler LVCMItem3.Click, AddressOf ShowHistoryChat
            'LVContextMenu.Items.Add(LVCMItem3)

            LVMyClosedOrders.ContextMenuStrip = LVContextMenu

        End If


    End Sub

    Private Sub ShowHistoryChat(sender As Object, e As EventArgs)

        'Dim T_Frm As Form = New Form With {.Name = "FrmMessage", .Text = "Messages", .StartPosition = FormStartPosition.CenterScreen, .Width = 800}
        'Dim RTB As RichTextBox = New RichTextBox
        'RTB.Dock = DockStyle.Fill

        'Dim LVi As ListViewItem = LVMyClosedOrders.SelectedItems(0)
        'Dim T_DexContract As ClsDEXContract = DirectCast(LVi.Tag, ClsDEXContract)

        'For Each ChatEntry As ClsDEXContract.S_Chat In T_DexContract.CurrentChat
        '    Dim DateTime As Date = ConvertULongMSToDate(ChatEntry.Timestamp)
        '    Dim DateTimeString As String = DateTime.ToShortDateString() + " " + DateTime.ToShortTimeString()
        '    RTB.AppendText("(" + DateTimeString + ") From " + ChatEntry.SenderAddress + " To " + ChatEntry.RecipientAddress + ":" + vbCrLf)
        '    RTB.AppendText(ChatEntry.Attachment + vbCrLf)
        '    RTB.AppendText(vbCrLf)
        'Next

        'T_Frm.Controls.Add(RTB)
        'T_Frm.Show()

    End Sub

#End Region 'MyClosedOrders - Controls

    Private Sub Copy2CB(sender As Object, e As EventArgs)

        Try
            If sender.GetType.Name = GetType(ToolStripMenuItem).Name Then

                Dim T_TSMI As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

                If Not T_TSMI.Tag Is Nothing Then
                    Clipboard.SetText(T_TSMI.Tag.ToString)
                Else

                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

#End Region 'MyOrders - Controls

#Region "Settings - Controls"
    'Private Property OldPaymentinfoText As String = Nothing

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

                If GlobalInDevelope Then
                    Dim FrmDev As New FrmDevelope(Me)
                    FrmDev.Dock = DockStyle.Fill
                    FrmDev.TopMost = True
                    FrmDev.TopLevel = False

                    SCSettings.Panel2.Controls.Add(FrmDev)
                    FrmDev.Show()
                End If

        End Select

    End Sub
    Private Function GetSubForm(ByVal Template As String) As Form

        If SCSettings.Panel2.Controls.Count > 0 Then
            Dim T_Frm As Form = DirectCast(SCSettings.Panel2.Controls(0), Form)

            If T_Frm.Name = Template Then
                Return T_Frm
            End If

        End If

        Return Nothing

    End Function

#End Region 'Settings - Controls

#Region "Bitcoin"

    Private Function LoadBitcoin() As Boolean

        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_Addresses As List(Of String) = GetBitcoinAddresses()

        LVBitcoinAddresses.Items.Clear()
        For Each T_Addr As String In T_Addresses

            Dim UTo As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = BitNET.GetUnspent(T_Addr)

            Dim MaxBal As Double = 0.0

            For Each ut As ClsBitcoinNET.S_UnspentTransactionOutput In UTo
                MaxBal += Satoshi2Dbl(ut.AmountNQT)
            Next

            With LVBitcoinAddresses.Items.Add(T_Addr)
                .SubItems.Add(Dbl2LVStr(MaxBal))
            End With
        Next
        LVBitcoinAddresses.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        Dim BitcoinAccounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")
        Dim BitcoinINITransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        'BitTxs = "<DEXContract>8372890871867317775</DEXContract><OrderID>18042278600640588977</OrderID><BitcoinTransactionID>f8845dd268e8edeb939daad0716228b53296ededaa13530c39983e2f29f82d28</BitcoinTransactionID><ChainSwapKey></ChainSwapKey><RedeemScript>63a820bc29d1020e37003e8a9dc688b4ec082e7eebda2dc3198ad0d347ee829b00b2548876a9148562fc5e57b965f8a62deaafeee84a7fc457c1d6675cb27576a914f57a0b8ba144b133dd900a2f9e8bb59c8f6ebea36888ac</RedeemScript>;"
        'BitTxs = "<BitcoinTransactionID>6eeea06efeafa34ae0552b1701944d09b143e663db3f859078f59742ffbc3941</BitcoinTransactionID><RedeemScript>63a8205682f300723e84bc877b0ebce916618415edc43663930cff67ef2381bedc36188876a9148562fc5e57b965f8a62deaafeee84a7fc457c1d6675cb27576a914f57a0b8ba144b133dd900a2f9e8bb59c8f6ebea36888ac</RedeemScript>"

        Dim UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = BitNET.GetUnspent(GetBitcoinMainAddress())

        If BitcoinINITransactions.Trim <> "" Then

            If BitcoinINITransactions.Substring(BitcoinINITransactions.Length - 1) = ";"c Then
                BitcoinINITransactions = BitcoinINITransactions.Remove(BitcoinINITransactions.Length - 1)
            End If

            Dim BitcoinTransactions As List(Of String) = New List(Of String)

            If BitcoinINITransactions.Contains(";") Then
                BitcoinTransactions.AddRange(BitcoinINITransactions.Split(";"c))
            Else
                BitcoinTransactions.Add(BitcoinINITransactions)
            End If


            For Each BitcoinTransaction As String In BitcoinTransactions

                Dim BitcoinTransactionID As String = GetStringBetween(BitcoinTransaction, "<BitcoinTransactionID>", "</BitcoinTransactionID>")
                Dim T_RedeemScript As String = GetStringBetween(BitcoinTransaction, "<RedeemScript>", "</RedeemScript>")

                Dim T_TX As ClsBitcoinTransaction = New ClsBitcoinTransaction(BitcoinTransactionID, New List(Of ClsBitcoinTransaction.S_Address), T_RedeemScript)

                Dim VoutIDX As Integer = -1
                Dim AmountNQT As ULong = 0UL
                Dim Confirmations As Integer = -1
                Dim Spendable As Boolean = False
                Dim Script As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

                For Each j As ClsUnspentOutput In T_TX.Inputs

                    If j.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
                        VoutIDX = j.InputIndex
                        AmountNQT = j.AmountNQT
                        Confirmations = j.Confirmations
                        Spendable = j.Spendable
                        Script = j.Script

                        Exit For
                    End If

                Next

                Dim T_Script As List(Of ClsScriptEntry) = ClsBitcoinTransaction.ConvertLockingScriptStrToList(T_RedeemScript)

                Dim RIPE160Sender As String = ClsBitcoinNET.GetXFromScript(T_Script, ClsScriptEntry.E_OP_Code.RIPE160Sender)
                Dim RIPE160Recipient As String = ClsBitcoinNET.GetXFromScript(T_Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient)

                Dim T_P2SH As ClsBitcoinNET.S_UnspentTransactionOutput = New ClsBitcoinNET.S_UnspentTransactionOutput()

                T_P2SH.TransactionID = BitcoinTransactionID
                T_P2SH.Typ = AbsClsOutputs.E_Type.Pay2ScriptHash

                Dim T_Addrs As List(Of String) = New List(Of String)({RIPE160ToAddress(RIPE160Sender, BitcoinAddressPrefix), RIPE160ToAddress(RIPE160Recipient, BitcoinAddressPrefix)})
                T_P2SH.Addresses = T_Addrs

                T_P2SH.VoutIDX = VoutIDX
                T_P2SH.AmountNQT = AmountNQT
                T_P2SH.Confirmations = Confirmations
                T_P2SH.LockingScript = Script

                If Confirmations > 0 Then
                    UTXOs.Add(T_P2SH)
                End If

            Next

        End If

        LVBitcoin.Items.Clear()
        Dim TotalBalance As Double = 0.0
        For Each UO As ClsBitcoinNET.S_UnspentTransactionOutput In UTXOs

            If UO.TransactionID Is Nothing Then
                Exit For
            End If

            With LVBitcoin.Items.Add(UO.Confirmations.ToString)
                .SubItems.Add(UO.TransactionID.ToString)
                .SubItems.Add(UO.VoutIDX.ToString)
                .SubItems.Add(UO.Typ.ToString)
                .SubItems.Add(UO.Addresses(0))
                Dim TempDbl As Double = Satoshi2Dbl(UO.AmountNQT)
                TotalBalance += TempDbl
                .SubItems.Add(Dbl2LVStr(TempDbl))
                .Tag = UO
            End With

        Next

        LVBitcoin.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        LabTotalBitcoin.Text = "Total: " + Dbl2LVStr(TotalBalance) + " BTC"

        Return True

    End Function

    Private Sub LVBitcoin_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LVBitcoin.SelectedIndexChanged

        If LVBitcoin.SelectedItems.Count > 0 Then

            Dim UO As ClsBitcoinNET.S_UnspentTransactionOutput = DirectCast(LVBitcoin.SelectedItems(0).Tag, ClsBitcoinNET.S_UnspentTransactionOutput)
            LVBitcoinLockingScript.Items.Clear()

            For Each x As ClsScriptEntry In UO.LockingScript
                With LVBitcoinLockingScript.Items.Add(x.Key.ToString)
                    .SubItems.Add(x.ValueHex)
                End With
            Next

        End If

        LVBitcoinLockingScript.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

    End Sub

    Private Sub RBBitcoinOptionX_CheckedChanged(sender As Object, e As EventArgs) Handles RBBitcoinOptionStandard.CheckedChanged, RBBitcoinOptionLockTime.CheckedChanged, RBBitcoinOptionCSHLT.CheckedChanged, RBBitcoinOptionChainSwapHash.CheckedChanged

        Select Case DirectCast(sender, RadioButton).Name
            Case RBBitcoinOptionStandard.Name
                LabBitcoinChainSwapHash.Enabled = False
                LabBitcoinLockTime.Enabled = False
                TBBitcoinChainSwapHash.Enabled = False
                NUDBitcoinLockTime.Enabled = False
            Case RBBitcoinOptionChainSwapHash.Name
                LabBitcoinChainSwapHash.Enabled = True
                LabBitcoinLockTime.Enabled = False
                TBBitcoinChainSwapHash.Enabled = True
                NUDBitcoinLockTime.Enabled = False
            Case RBBitcoinOptionLockTime.Name
                LabBitcoinChainSwapHash.Enabled = False
                LabBitcoinLockTime.Enabled = True
                TBBitcoinChainSwapHash.Enabled = False
                NUDBitcoinLockTime.Enabled = True
            Case RBBitcoinOptionCSHLT.Name
                LabBitcoinChainSwapHash.Enabled = True
                LabBitcoinLockTime.Enabled = True
                TBBitcoinChainSwapHash.Enabled = True
                NUDBitcoinLockTime.Enabled = True
        End Select

    End Sub

    Private Sub BtSendBitcoin_Click(sender As Object, e As EventArgs) Handles BtSendBitcoin.Click

        BtSendBitcoin.Text = "Wait..."
        BtSendBitcoin.Enabled = False

        Dim Bitcoin As ClsBitcoin = New ClsBitcoin()
        Dim Transaction As ClsBitcoinTransaction = Bitcoin.CreateBitcoinTransaction(NUDBitcoinAmount.Value, TBBitcoinRecipient.Text, GetBitcoinMainAddress(), 12)

        If RBBitcoinOptionStandard.Checked Then
            Transaction = Bitcoin.CreateBitcoinTransaction(NUDBitcoinAmount.Value, TBBitcoinRecipient.Text, GetBitcoinMainAddress())
        ElseIf RBBitcoinOptionChainSwapHash.Checked Then
            Transaction = Bitcoin.CreateBitcoinTransaction(NUDBitcoinAmount.Value, TBBitcoinRecipient.Text, GetBitcoinMainAddress(), TBBitcoinChainSwapHash.Text)
        ElseIf RBBitcoinOptionLockTime.Checked Then
            Transaction = Bitcoin.CreateBitcoinTransaction(NUDBitcoinAmount.Value, TBBitcoinRecipient.Text, GetBitcoinMainAddress(), 12)
        ElseIf RBBitcoinOptionCSHLT.Checked Then
            Transaction = Bitcoin.CreateBitcoinTransaction(NUDBitcoinAmount.Value, TBBitcoinRecipient.Text, GetBitcoinMainAddress(), TBBitcoinChainSwapHash.Text, 12)
        End If

        Dim TXID As String = Bitcoin.SignBitcoinTransaction(Transaction, GetBitcoinMainPrivateKey(True).ToLower())

        TXID = Bitcoin.SendRawBitcoinTransaction(TXID)

        If IsErrorOrWarning(TXID) Then
            ClsMsgs.MBox("The Transaction cant be signed", "Error",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.ButtonEnable)
        Else
            ClsMsgs.MBox("Transaction successfully send." + vbCrLf + " ID = " + TXID + vbCrLf + "Details in " + TXID + ".log", "Success!",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
            Dim out As ClsOut = New ClsOut()
            Dim TXOut As String = ""

            For Each OP As ClsOutput In Transaction.Outputs
                TXOut += "TX = " + TXID + vbCrLf
                TXOut += "OutputType = " + OP.OutputType.ToString() + vbCrLf
                TXOut += "Script = " + OP.ScriptHex + vbCrLf + vbCrLf
            Next

            out.ToFile(TXOut, TXID + ".log")

        End If

        BtSendBitcoin.Text = "send"
        BtSendBitcoin.Enabled = True

    End Sub

#End Region 'Bitcoin

#End Region 'GUI Control

#Region "Methods/Functions"

    Private Function Checknodes() As Integer

        Dim NodeCNT As Integer = 0

        For Each Node As String In NodeList

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node)

            Dim Blockheight As ULong = C_SignumAPI.GetCurrentBlock()
            If Blockheight > 0UL Then
                NodeCNT += 1
            End If

        Next

        Return NodeCNT

    End Function

    Private Function GetAndCheckSmartContracts() As Boolean

        Try

            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim SmartContractList As List(Of String) = C_SignumAPI.GetSmartContractIds(GlobalReferenceSignumSmartContract.MachineCodeHashId)
            Dim CSVSmartContractList As List(Of List(Of String)) = GetSmartContractsFromCSV()


            Dim Nu_SmartContractList As List(Of S_SmartContract) = New List(Of S_SmartContract) ' New List(Of String)

            For Each SmartContract As String In SmartContractList

                Dim Found As Boolean = False
                For Each CSVSmartContract As List(Of String) In CSVSmartContractList

                    If SmartContract.Trim = CSVSmartContract(0).Trim Then
                        Found = True
                        Exit For
                    End If
                Next

                If Not Found Then
                    Dim T_SmartContract As S_SmartContract = New S_SmartContract With {.ID = Convert.ToUInt64(SmartContract)}
                    Nu_SmartContractList.Add(T_SmartContract)
                End If

            Next


            For Each CSVSmartContract As List(Of String) In CSVSmartContractList

                If CSVSmartContract(1).Trim = "True" Then
                    Dim T_SmartContract As S_SmartContract = New S_SmartContract With {.ID = Convert.ToUInt64(CSVSmartContract(0).Trim), .IsDEX_SC = Convert.ToBoolean(CSVSmartContract(1).Trim), .HistoryOrders = CSVSmartContract(2)}
                    Nu_SmartContractList.Add(T_SmartContract)
                End If

            Next


            APIRequestList.Clear()

            For Each SmartContract As S_SmartContract In Nu_SmartContractList

                Dim TestMulti As S_APIRequest = New S_APIRequest
                Dim TempContract As ClsDEXContract = Nothing
                Dim Found As Boolean = False

                For Each T_DEXContract As ClsDEXContract In C_DEXContractList

                    If T_DEXContract.ID = SmartContract.ID Then
                        TempContract = T_DEXContract
                        Found = True
                        Exit For
                    End If

                Next

                If Not Found Then
                    TestMulti.Command = "GetDetails(" + SmartContract.ID.ToString.Trim + ")"
                    TestMulti.Status = "Wait..."
                    TestMulti.CommandAttachment = SmartContract.HistoryOrders
                Else
                    TestMulti.Command = "RefreshContract(" + SmartContract.ID.ToString.Trim + ")"
                    TestMulti.Status = "Wait..."
                    TestMulti.CommandAttachment = TempContract
                End If

                APIRequestList.Add(TestMulti)

            Next

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in GetAndCheckSmartContracts(): -> " + ex.Message)
        End Try

        Return True

    End Function

    Private Function ResetLVColumns() As Boolean

        'LVSellorders.Columns.Clear()
        'LVSellorders.Columns.Add("Price (" + CurrentMarket + ")")
        'LVSellorders.Columns.Add("Amount (" + CurrentMarket + ")")
        'LVSellorders.Columns.Add("Total (Signa)")
        'LVSellorders.Columns.Add("Collateral (Signa)")
        'LVSellorders.Columns.Add("Method")
        'LVSellorders.Columns.Add("Autoinfo")
        'LVSellorders.Columns.Add("Autofinish")
        'LVSellorders.Columns.Add("Deniability")
        'LVSellorders.Columns.Add("Seller")
        'LVSellorders.Columns.Add("Smart Contract")


        LVSellorders.Columns.Clear()
        LVSellorders.Columns.Add("Smart Contract")
        LVSellorders.Columns.Add("Seller")
        LVSellorders.Columns.Add("Deniability")
        LVSellorders.Columns.Add("Autofinish")
        LVSellorders.Columns.Add("Autoinfo")
        LVSellorders.Columns.Add("Method")
        LVSellorders.Columns.Add("Collateral (Signa)")
        LVSellorders.Columns.Add("Total (Signa)")
        LVSellorders.Columns.Add("Amount (" + CurrentMarket + ")")
        LVSellorders.Columns.Add("Price (" + CurrentMarket + ")")


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
        LVBuyorders.Columns.Add("Deniability")
        LVBuyorders.Columns.Add("Buyer")
        LVBuyorders.Columns.Add("Smart Contract")


        LVMySmartContracts.Columns.Clear()
        LVMySmartContracts.Columns.Add("Smart Contract")
        LVMySmartContracts.Columns.Add("Current Balance")
        LVMySmartContracts.Columns.Add("Type")
        LVMySmartContracts.Columns.Add("Current Seller")
        LVMySmartContracts.Columns.Add("Current Buyer")
        LVMySmartContracts.Columns.Add("Buy/Sell Amount")
        LVMySmartContracts.Columns.Add("Sellers Collateral")
        LVMySmartContracts.Columns.Add("Buyers Collateral")
        LVMySmartContracts.Columns.Add("XItem")
        LVMySmartContracts.Columns.Add("Price")
        LVMySmartContracts.Columns.Add("Deniability/Dispute")
        LVMySmartContracts.Columns.Add("My Collateral")
        LVMySmartContracts.Columns.Add("Conciliation Amount")
        LVMySmartContracts.Columns.Add("Current Status")
        LVMySmartContracts.Columns.Add("Infotext")


        LVMyOpenOrders.Columns.Clear()
        LVMyOpenOrders.Columns.Add("Confirmations")
        LVMyOpenOrders.Columns.Add("Smart Contract")
        'LVMyOpenOrders.Columns.Add("Contract Creator")
        LVMyOpenOrders.Columns.Add("Type")
        LVMyOpenOrders.Columns.Add("Method")
        LVMyOpenOrders.Columns.Add("Autoinfo/Autofinish")
        LVMyOpenOrders.Columns.Add("Seller")
        LVMyOpenOrders.Columns.Add("Buyer")
        LVMyOpenOrders.Columns.Add("XItem")
        LVMyOpenOrders.Columns.Add("Buy/Sell Amount")
        LVMyOpenOrders.Columns.Add("Collateral")
        LVMyOpenOrders.Columns.Add("Price")
        LVMyOpenOrders.Columns.Add("Deniability/Dispute")
        LVMyOpenOrders.Columns.Add("Status")
        LVMyOpenOrders.Columns.Add("Infotext")


        LVMyClosedOrders.Columns.Clear()
        LVMyClosedOrders.Columns.Add("First Transaction")
        LVMyClosedOrders.Columns.Add("Last Transaction")
        LVMyClosedOrders.Columns.Add("Date")
        LVMyClosedOrders.Columns.Add("Smart Contract")
        LVMyClosedOrders.Columns.Add("Type")
        ' LVMyClosedOrders.Columns.Add("Method")
        LVMyClosedOrders.Columns.Add("Seller")
        LVMyClosedOrders.Columns.Add("Buyer")
        LVMyClosedOrders.Columns.Add("XItem")
        'LVMyClosedOrders.Columns.Add("XAmount")
        LVMyClosedOrders.Columns.Add("Buy/Sell Amount")
        LVMyClosedOrders.Columns.Add("Price")
        LVMyClosedOrders.Columns.Add("Status")
        'LVMyClosedOrders.Columns.Add("Conditions")

        Return True

    End Function

    Private Function SetInLVs() As Boolean

        'Try

#Region "set LVs"

        C_BuyOrderLVEList.Clear()
        C_SellOrderLVEList.Clear()

        C_OpenChannelLVIList.Clear()
        'BuyOrderLVIList.Clear()
        'SellOrderLVIList.Clear()
        C_MySmartContractLVIList.Clear()
        C_MyOpenOrderLVIList.Clear()
        C_MyClosedOrderLVIList.Clear()

        Dim Wait As Boolean = ProcessingSmartContracts()

        BtBuy.Text = "Buy"
        BtSell.Text = "Sell"

        BtPayOrder.Visible = False
        BtReCreatePayPalOrder.Visible = False
        BtExecuteOrder.Visible = False

        LabMsg.Visible = False 'Sendmessage MyOpenOrders
        TBManuMsg.Visible = False
        BtSendMsg.Visible = False
        ChBxEncMsg.Visible = False

        LVOpenChannels.Visible = False

        LVMyOpenOrders.Visible = False
        LVMyClosedOrders.Visible = False

        SplitContainer20.Panel2Collapsed = True
        SplitContainer16.Panel2Collapsed = True 'hide interaction-buttons


        LVOpenChannels.Items.Clear()
        For Each OpenChannel As ListViewItem In C_OpenChannelLVIList
            MultiInvoker(LVOpenChannels, "Items", New List(Of Object)({"Add", OpenChannel}))
        Next

        Wait = SetFitlteredPublicOrdersInLVs()

        LVMySmartContracts.Items.Clear()
        For Each MySmartContract As ListViewItem In C_MySmartContractLVIList
            MultiInvoker(LVMySmartContracts, "Items", New List(Of Object)({"Add", MySmartContract}))
        Next

        LVMyOpenOrders.Items.Clear()
        For Each MyOpenOrder As ListViewItem In C_MyOpenOrderLVIList
            MultiInvoker(LVMyOpenOrders, "Items", New List(Of Object)({"Add", MyOpenOrder}))
        Next

        C_MyClosedOrderLVIList = C_MyClosedOrderLVIList.OrderByDescending(Function(T_Order As S_DEXOrder) T_Order.Order.StartTimestamp).ToList()

        LVMyClosedOrders.Items.Clear()
        For i As Integer = 0 To C_MyClosedOrderLVIList.Count - 1

            If i > 100 Then
                Exit For
            End If

            Dim HistoryOrder As S_DEXOrder = C_MyClosedOrderLVIList(i)

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
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.XAmount, C_Decimals) + " " + HistoryOrder.Order.XItem) 'xitem
            'MyClosedOrder.SubItems.Add() 'xamount
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.Amount))  'quantity
            MyClosedOrder.SubItems.Add(Dbl2LVStr(HistoryOrder.Order.Price, C_Decimals) + " " + HistoryOrder.Order.XItem) 'price

            If ClsDEXContract.CurrencyIsCrypto(HistoryOrder.Order.XItem) And Not HistoryOrder.Order.ChainSwapKey.Trim = "" Then
                MyClosedOrder.SubItems.Add(HistoryOrder.Order.Status.ToString + " ChainSwapKey:" + HistoryOrder.Order.ChainSwapKey) 'status
            Else
                MyClosedOrder.SubItems.Add(HistoryOrder.Order.Status.ToString) 'status
            End If


            'MyClosedOrder.SubItems.Add(Order.Attachment.ToString) 'conditions
            MyClosedOrder.Tag = HistoryOrder.Contract

            MultiInvoker(LVMyClosedOrders, "Items", New List(Of Object)({"Add", MyClosedOrder}))

        Next

        Dim MasterKeys As List(Of String) = GetPassPhrase()
        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
        If MasterKeys.Count > 0 Then
            For Each BroadcastMsg As String In C_BroadcastMsgs
                If Not DEXNET Is Nothing Then
                    DEXNET.BroadcastMessage(BroadcastMsg, MasterKeys(1),, MasterKeys(0))
                End If
            Next
        End If

        C_BroadcastMsgs.Clear()

        LVMyOpenOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Ascending, 0)
        LVMyClosedOrders.ListViewItemSorter = New ListViewItemExtremeSorter(SortOrder.Descending, 2)

        LVOpenChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        LVMySmartContracts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        LVMyOpenOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        LVMyClosedOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

#End Region 'setLVs

        Dim T_BlockFeeThread As Threading.Thread = New Threading.Thread(AddressOf BlockFeeThread)

        StatusBlockLabel.Text = "loading Blockheight..."
        'StatusFeeLabel.Text = "loading Current Fee..."
        StatusStrip1.Refresh()

        T_BlockFeeThread.Start(DirectCast(PrimaryNode, Object))

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

    Private Function SetFitlteredPublicOrdersInLVs() As Boolean

        LVSellorders.Visible = False
        LVBuyorders.Visible = False

        LVBuyorders.Items.Clear()

        If Not C_OffchainBuyOrder.SCID = "" Then

            Dim T_Price As Double = C_OffchainBuyOrder.XAmount / C_OffchainBuyOrder.Amount

            Dim T_LVI As ListViewItem = New ListViewItem

            T_LVI.Text = Dbl2LVStr(T_Price, 2) 'price
            T_LVI.SubItems.Add(Dbl2LVStr(C_OffchainBuyOrder.XAmount, 2)) 'amount
            T_LVI.SubItems.Add(Dbl2LVStr(C_OffchainBuyOrder.Amount)) 'total
            T_LVI.SubItems.Add(Dbl2LVStr(0.0)) 'collateral
            T_LVI.SubItems.Add(C_OffchainBuyOrder.Method) 'payment method
            T_LVI.SubItems.Add("False") 'autosend infotext
            T_LVI.SubItems.Add("False") 'autocomplete at
            T_LVI.SubItems.Add("False") 'deniability
            T_LVI.SubItems.Add("Me") 'buyer
            T_LVI.SubItems.Add("0") 'at

            T_LVI.BackColor = Color.Magenta
            T_LVI.Tag = C_OffchainBuyOrder

            MultiInvoker(LVBuyorders, "Items", New List(Of Object)({"Add", T_LVI}))

        End If


        Dim T_BuyOrderEntryList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)

        T_BuyOrderEntryList.AddRange(C_BuyOrderLVEList.ToArray)
        T_BuyOrderEntryList.AddRange(C_BuyOrderLVOffChainEList.ToArray)

        T_BuyOrderEntryList = T_BuyOrderEntryList.OrderBy(Function(T_PublicOrder As S_PublicOrdersListViewEntry) T_PublicOrder.Price).ToList

        For i As Integer = 0 To T_BuyOrderEntryList.Count - 1

            Dim BuyOrder As S_PublicOrdersListViewEntry = T_BuyOrderEntryList(i)

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

                If Not ChBxBuyFilterShowOffChainOrders.Checked Then
                    If BuyOrder.SmartContract.Contains("OffchainOrder") Then
                        Continue For
                    End If
                End If



                Dim ValidBuyMethods As String = LoadMethodFilterFromINI(E_Setting.BuyFilterMethods)
                If (Not ValidBuyMethods.Contains(BuyOrder.Method) Or ValidBuyMethods.Trim = "") And BuyOrder.Method <> "AtomicSwap" Then
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
            T_LVI.SubItems.Add(BuyOrder.Deniability) 'deniability
            T_LVI.SubItems.Add(BuyOrder.Seller_Buyer) 'buyer
            T_LVI.SubItems.Add(BuyOrder.SmartContract) 'at

            T_LVI.BackColor = BuyOrder.Backcolor
            T_LVI.Tag = BuyOrder.Tag

            MultiInvoker(LVBuyorders, "Items", New List(Of Object)({"Add", T_LVI}))

        Next

        'For Each BuyOrder As ListViewItem In BuyOrderLVIList
        '    MultiInvoker(LVBuyorders, "Items", {"Add", BuyOrder})
        'Next


        LVSellorders.Items.Clear()

        C_SellOrderLVEList = C_SellOrderLVEList.OrderBy(Function(T_PublicOrder As S_PublicOrdersListViewEntry) T_PublicOrder.Price).ToList
        C_SellOrderLVEList.Reverse()

        For i As Integer = 0 To C_SellOrderLVEList.Count - 1

            Dim SellOrder As S_PublicOrdersListViewEntry = C_SellOrderLVEList(i)

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
                If (Not ValidSellMethods.Contains(SellOrder.Method) Or ValidSellMethods.Trim = "") And SellOrder.Method <> "AtomicSwap" Then
                    Continue For
                End If

            End If


            Dim T_LVI As ListViewItem = New ListViewItem

            T_LVI.Text = SellOrder.SmartContract  'at
            'T_LVI.SubItems.Add(SellOrder.SmartContract) 'at
            T_LVI.SubItems.Add(SellOrder.Seller_Buyer) 'buyer
            T_LVI.SubItems.Add(SellOrder.Deniability) 'deniability
            T_LVI.SubItems.Add(SellOrder.AutoFinish) 'autocomplete at
            T_LVI.SubItems.Add(SellOrder.AutoInfo) 'autosend infotext
            T_LVI.SubItems.Add(SellOrder.Method) 'payment method
            T_LVI.SubItems.Add(SellOrder.Collateral) 'collateral
            T_LVI.SubItems.Add(SellOrder.Total) 'total
            T_LVI.SubItems.Add(SellOrder.Amount) 'amount
            T_LVI.SubItems.Add(SellOrder.Price) 'price

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

    Private Function ProcessingSmartContracts() As Boolean

        Try

            'OrderList.Clear()
            C_MTSSmartContract2LVList.Clear()

            OrderSettingsBuffer.Clear()
            OrderSettingsBuffer = GetOrderSettings()

            For i As Integer = 0 To C_DEXContractList.Count - 1

                Dim T_DEXContract As ClsDEXContract = C_DEXContractList(i)

                'OrderList.AddRange(T_DEXContract.ContractOrderHistoryList)

                Dim T_SetLVThreadStruc As S_MultiThreadSetSC2LV = New S_MultiThreadSetSC2LV

                T_SetLVThreadStruc.SubThread = New Threading.Thread(AddressOf MultiThreadSetSmartContract2LV)

                Dim Index As Integer = C_MTSSmartContract2LVList.Count
                T_SetLVThreadStruc.DEXContract = T_DEXContract
                T_SetLVThreadStruc.Market = CurrentMarket

                Application.DoEvents()

                C_MTSSmartContract2LVList.Add(T_SetLVThreadStruc)

                T_SetLVThreadStruc.SubThread.Start(New List(Of Object)({Index}))

            Next


            Dim exiter As Boolean = False
            While Not exiter
                exiter = True

                Application.DoEvents()

                For Each S_TH As S_MultiThreadSetSC2LV In C_MTSSmartContract2LVList

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
            IsErrorOrWarning(Application.ProductName + "-error in ProcessingSmartContracts(): -> " + ex.Message)
        End Try

        Return True

    End Function

    Public Structure S_MultiThreadSetSC2LV
        Dim SubThread As Threading.Thread
        Dim DEXContract As ClsDEXContract
        Dim Market As String
    End Structure

    Private Property C_MTSSmartContract2LVList As List(Of S_MultiThreadSetSC2LV) = New List(Of S_MultiThreadSetSC2LV)

    Public Structure S_PublicOrdersListViewEntry
        Dim Price As String
        Dim Amount As String
        Dim Total As String
        Dim Collateral As String
        Dim Backcolor As Color
        Dim Method As String
        Dim AutoInfo As String
        Dim AutoFinish As String
        Dim Deniability As String
        Dim Seller_Buyer As String
        Dim SmartContract As String
        Dim Tag As Object

        Sub New(Optional ByVal P_Price As String = "", Optional ByVal P_Amount As String = "", Optional ByVal P_Total As String = "", Optional ByVal P_Collateral As String = "", Optional ByVal P_Method As String = "", Optional ByVal P_AutoInfo As String = "", Optional ByVal P_AutoFinish As String = "", Optional ByVal P_Deniability As String = "", Optional ByVal P_Seller_Buyer As String = "", Optional ByVal P_SmartContract As String = "")

            Price = P_Price
            Amount = P_Amount
            Total = P_Total
            Collateral = P_Collateral
            Backcolor = SystemColors.Control
            Method = P_Method
            AutoInfo = P_AutoInfo
            AutoFinish = P_AutoFinish
            Deniability = P_Deniability
            Seller_Buyer = P_Seller_Buyer
            SmartContract = P_SmartContract
            Tag = Nothing

        End Sub

    End Structure

    Private Property C_SellOrderLVEList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)
    Private Property C_BuyOrderLVEList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)
    Private Property C_BuyOrderLVOffChainEList As List(Of S_PublicOrdersListViewEntry) = New List(Of S_PublicOrdersListViewEntry)

    Private Property C_OpenChannelLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Private Property C_MySmartContractLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Private Property C_MyOpenOrderLVIList As List(Of ListViewItem) = New List(Of ListViewItem)
    Private Property C_MyClosedOrderLVIList As List(Of S_DEXOrder) = New List(Of S_DEXOrder)

    Public Structure S_DEXOrder
        Dim ContractAddress As String
        Dim Order As ClsDEXContract.S_Order
        Dim Contract As ClsDEXContract
    End Structure

    Private Property C_BroadcastMsgs As List(Of String) = New List(Of String)

    Private Sub MultiThreadSetSmartContract2LV(ByVal T_Input As Object)

        Try

#Region "Main"

            Dim Input As List(Of Object) = New List(Of Object)

            If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_Input, List(Of Object))
            Else
                IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(DirectCast): -> " + T_Input.GetType.Name)
            End If

            Dim Index As Integer = DirectCast(Input(0), Integer)
            Dim S_MTSetSC2LV As S_MultiThreadSetSC2LV = C_MTSSmartContract2LVList(Index)

            Dim Market As String = S_MTSetSC2LV.Market
            Dim T_DEXContract As ClsDEXContract = S_MTSetSC2LV.DEXContract
            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(T_DEXContract, PrimaryNode)

            If TBSNOAddress.Text = T_DEXContract.CreatorAddress Then

                Dim Tsc_LVI As ListViewItem = New ListViewItem
                Tsc_LVI.Text = T_DEXContract.Address ' T_DEXContract.ID.ToString  'id
                'Tsc_LVI.SubItems.Add(T_DEXContract.Address) 'address
                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBalance)) 'balance

                If T_DEXContract.IsSellOrder Then
                    Tsc_LVI.SubItems.Add("SellOrder") 'type
                Else
                    Tsc_LVI.SubItems.Add("BuyOrder")  'type
                End If

                Tsc_LVI.SubItems.Add(T_DEXContract.CurrentSellerAddress) 'seller
                Tsc_LVI.SubItems.Add(T_DEXContract.CurrentBuyerAddress) 'buyer
                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'buysellamount

                If T_DEXContract.IsSellOrder Then
                    Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'sellers collateral
                    Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentRespondersCollateral)) 'buyers collateral
                Else
                    Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentRespondersCollateral)) 'sellers collateral
                    Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'buyers collateral
                End If

                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem) 'XItem

                Dim T_Price As Double = T_DEXContract.CurrentXAmount / T_DEXContract.CurrentBuySellAmount

                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_Price, C_Decimals)) 'price
                Tsc_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString) 'deniability/dispute
                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentMediatorsDeposit)) 'my collateral
                Tsc_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentConciliationAmount)) 'conciliation amount
                Tsc_LVI.SubItems.Add(T_DEXContract.Status.ToString) 'status

                Tsc_LVI.SubItems.Add(T_DEXContract.GetLastDecryptedMessageFromChat(TBSNOAddress.Text, True)) 'infotext

                If T_DEXContract.Deniability And T_DEXContract.Dispute Then
                    Tsc_LVI.BackColor = Color.Magenta
                End If

                'TODO: Chatwindow
                'Tsc_LVI.SubItems.Add(T_DEXContract.Status.ToString + " chatcount:" + T_DEXContract.CurrentChat.Count.ToString) 'status
                Tsc_LVI.Tag = T_DEXContract ' Order

                C_MySmartContractLVIList.Add(Tsc_LVI)

            End If



            'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
            Dim T_LVI As ListViewItem = New ListViewItem

            If T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ Then

                If T_DEXContract.CreatorAddress = TBSNOAddress.Text Then

                    T_LVI.Text = T_DEXContract.Address 'SmartContract
                    T_LVI.SubItems.Add("Reserved for you") 'Status
                    T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString)
                    T_LVI.Tag = T_DEXContract

                    C_OpenChannelLVIList.Add(T_LVI)

                End If

            Else 'Status <> NEW_

                Dim SmartContractChannelOpen As Boolean = True


#Region "Set To LVs"

#Region "Confirmations"
                Dim Confirms As String = T_DEXContract.CurrentConfirmations.ToString
#End Region

#Region "XItem/Payment Method"

                Dim XItemTicker As String = T_DEXContract.CurrentXItem
                Dim PayMet As String = "Unknown"

                'If XItem.Contains("-") Then
                '    PayMet = XItem.Split("-")(1)
                '    XItem = XItem.Split("-")(0)
                'End If

#End Region

                If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then

                    SmartContractChannelOpen = False

                    Try

                        Dim Autosendinfotext As String = "False"
                        Dim AutocompleteSmartContract As String = "False"

                        Dim T_SellerRS As String = T_DEXContract.CurrentSellerAddress ' Order.SellerRS
                        If TBSNOAddress.Text = T_SellerRS Then

                            Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)

                            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status)

                            If T_OSList.Count = 0 Then
                                T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")

                                Dim T_OrderSettingFromTX As List(Of ClsOrderSettings) = GetOrderSettings(T_OS.TransactionID.ToString)

                                If T_OrderSettingFromTX.Count > 0 Then
                                    T_OS.AutoCompleteSmartContract = T_OrderSettingFromTX(0).AutoCompleteSmartContract
                                    T_OS.AutoSendInfotext = T_OrderSettingFromTX(0).AutoSendInfotext
                                    T_OS.Infotext = T_OrderSettingFromTX(0).Infotext
                                    T_OS.PayType = T_OrderSettingFromTX(0).PayType
                                    T_OS.PaytypeString = T_OrderSettingFromTX(0).PaytypeString
                                End If

                                OrderSettingsBuffer.Add(T_OS)
                            Else
                                T_OS = T_OSList(0)
                                T_OS.SetPayType()
                                PayMet = T_OS.PaytypeString
                                Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                AutocompleteSmartContract = T_OS.AutoCompleteSmartContract.ToString
                            End If

                        End If

                        If T_DEXContract.CurrentXItem.Contains(Market) Then

#Region "Market - GUI"

                            If Not T_DEXContract.CheckForUTX And Not T_DEXContract.CheckForTX Then

                                If T_DEXContract.IsSellOrder Then ' Order.Type = "SellOrder" Then

                                    T_LVI = New ListViewItem
                                    Dim T_LVE As S_PublicOrdersListViewEntry = New S_PublicOrdersListViewEntry

                                    'T_LVI.Text = Dbl2LVStr(Order.Price, Decimals) 'price
                                    T_LVE.Price = Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals) 'price

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                    T_LVE.Amount = Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals)

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                    T_LVE.Total = Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)

                                    'T_LVI.SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral
                                    T_LVE.Collateral = Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral) '  Order.Collateral)

                                    '
                                    If T_SellerRS = TBSNOAddress.Text Then

                                        T_SellerRS = "Me"
                                        'T_LVI.BackColor = Color.Magenta
                                        T_LVE.Backcolor = Color.Magenta

                                    Else

#Region "SellOrder-Info from DEXNET"
                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not DEXNET Is Nothing Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If


                                        Dim RelKeyFounded As Boolean = False

                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey.Name = "<SCID>" + T_DEXContract.ID.ToString + "</SCID>" Then
                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If Not T_SellerRS = TBSNOAddress.Text Then

                                                        If T_DEXContract.CurrentSellerID = GetAccountID(PublicKey) Then

                                                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                            PayMet = PayMethod

                                                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                            Autosendinfotext = T_Autosendinfotext

                                                            Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                                                            AutocompleteSmartContract = T_AutocompleteSmartContract

                                                        End If

                                                    End If

                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not DEXNET Is Nothing Then
                                                DEXNET.AddRelevantKey("<SCID>" + T_DEXContract.ID.ToString + "</SCID>")
                                            End If
                                        End If

#End Region

                                    End If

                                    If T_DEXContract.CurrencyIsCrypto Then
                                        T_LVE.Method = "AtomicSwap"
                                        'T_LVE.AutoInfo = "Auto"
                                        'T_LVE.AutoFinish = "Auto"
                                    Else
                                        T_LVE.Method = PayMet
                                    End If

                                    T_LVE.AutoInfo = Autosendinfotext
                                    T_LVE.AutoFinish = AutocompleteSmartContract
                                    T_LVE.Deniability = T_DEXContract.Deniability.ToString
                                    T_LVE.Seller_Buyer = T_SellerRS
                                    T_LVE.SmartContract = T_DEXContract.Address
                                    T_LVE.Tag = T_DEXContract
                                    C_SellOrderLVEList.Add(T_LVE)

                                Else 'BuyOrder

                                    Dim T_LVE As S_PublicOrdersListViewEntry = New S_PublicOrdersListViewEntry
                                    T_LVE.Price = Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals)
                                    T_LVE.Amount = Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals)
                                    T_LVE.Total = Dbl2LVStr(T_DEXContract.CurrentBuySellAmount) 'TODO: Total = CurrentXAmount?
                                    T_LVE.Collateral = Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)

                                    Dim T_BuyerRS As String = T_DEXContract.CurrentBuyerAddress ' Order.BuyerRS
                                    If T_BuyerRS = TBSNOAddress.Text Then

                                        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)
                                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status)

                                        If T_OSList.Count = 0 Then
                                            T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                            T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                            OrderSettingsBuffer.Add(T_OS)
                                        Else
                                            T_OS = T_OSList(0)
                                            T_OS.SetPayType()
                                            PayMet = T_OS.PaytypeString
                                            Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                            AutocompleteSmartContract = T_OS.AutoCompleteSmartContract.ToString
                                        End If

                                        T_BuyerRS = "Me"
                                        T_LVE.Backcolor = Color.Magenta
                                    Else

#Region "BuyOrder-Info from DEXNET"
                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not DEXNET Is Nothing Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If

                                        Dim RelKeyFounded As Boolean = False
                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey.Name = "<SCID>" + T_DEXContract.ID.ToString + "</SCID>" Then
                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If Not T_BuyerRS = TBSNOAddress.Text Then

                                                        If T_DEXContract.CurrentBuyerID = GetAccountID(PublicKey) Then
                                                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                            PayMet = PayMethod

                                                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                            Autosendinfotext = T_Autosendinfotext

                                                            Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                                                            AutocompleteSmartContract = T_AutocompleteSmartContract

                                                        End If

                                                    End If

                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not DEXNET Is Nothing Then
                                                DEXNET.AddRelevantKey("<SCID>" + T_DEXContract.ID.ToString + "</SCID>")
                                            End If
                                        End If
#End Region

                                    End If

                                    If T_DEXContract.CurrencyIsCrypto() Then
                                        T_LVE.Method = "AtomicSwap"
                                        'T_LVE.AutoInfo = "Auto"
                                        'T_LVE.AutoFinish = "Auto"
                                    Else
                                        T_LVE.Method = PayMet
                                    End If

                                    T_LVE.AutoInfo = Autosendinfotext
                                    T_LVE.AutoFinish = AutocompleteSmartContract
                                    T_LVE.Deniability = T_DEXContract.Deniability.ToString()
                                    T_LVE.Seller_Buyer = T_BuyerRS
                                    T_LVE.SmartContract = T_DEXContract.Address
                                    T_LVE.Tag = T_DEXContract

                                    C_BuyOrderLVEList.Add(T_LVE)

                                End If

                            End If

#End Region

                        End If 'market(USD)

#Region "MyOPENOrders - GUI"

                        If TBSNOAddress.Text = T_DEXContract.CurrentSellerAddress Then ' Order.SellerRS Then

                            'Broadcast info over DEXNET
                            If T_DEXContract.CurrencyIsCrypto() Then
                                C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString() + "</SCID><PayType>AtomicSwap</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")
                            Else
                                C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString() + "</SCID><PayType>" + PayMet.Trim() + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")
                            End If

                            T_LVI = New ListViewItem
                            T_LVI.Text = Confirms 'confirms
                            T_LVI.SubItems.Add(T_DEXContract.Address) 'SmartContract
                            'T_LVI.SubItems.Add(T_DEXContract.CreatorAddress) 'creator

                            If T_DEXContract.IsSellOrder Then
                                T_LVI.SubItems.Add("SellOrder") ' Order.Type) 'type
                            Else
                                T_LVI.SubItems.Add("BuyOrder") ' Order.Type) 'type
                            End If

                            T_LVI.SubItems.Add(PayMet) 'method
                            T_LVI.SubItems.Add(Autosendinfotext + "/" + AutocompleteSmartContract) 'autoinfo/autofinish
                            T_LVI.SubItems.Add("Me") 'seller
                            T_LVI.SubItems.Add(T_DEXContract.CurrentBuyerAddress)  'buyer
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + XItemTicker) 'xitem/xamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'buysellamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral))  'collateral
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals) + " " + XItemTicker)  'price

                            T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString) 'deniability/dispute
                            T_LVI.SubItems.Add("OPEN") 'status
                            T_LVI.SubItems.Add("") 'infotext
                            T_LVI.Tag = T_DEXContract ' Order

                            C_MyOpenOrderLVIList.Add(T_LVI)

                        ElseIf TBSNOAddress.Text = T_DEXContract.CurrentBuyerAddress Then ' Order.BuyerRS Then

                            'Broadcast info over DEXNET
                            If T_DEXContract.CurrencyIsCrypto Then
                                C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString + "</SCID><PayType>AtomicSwap</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")
                            Else
                                C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString + "</SCID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")
                            End If

                            T_LVI = New ListViewItem
                            T_LVI.Text = Confirms 'confirms
                            T_LVI.SubItems.Add(T_DEXContract.Address) 'smartcontract
                            'T_LVI.SubItems.Add(T_DEXContract.CreatorAddress) 'creator

                            If T_DEXContract.IsSellOrder Then
                                T_LVI.SubItems.Add("SellOrder") 'type
                            Else
                                T_LVI.SubItems.Add("BuyOrder") 'type
                            End If

                            T_LVI.SubItems.Add(PayMet) 'method
                            T_LVI.SubItems.Add(Autosendinfotext + "/" + AutocompleteSmartContract) 'autoinfo/autofinish

                            T_LVI.SubItems.Add(T_DEXContract.CurrentSellerAddress)  'seller
                            T_LVI.SubItems.Add("Me") 'buyer
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + XItemTicker) 'xitem/xamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount))  'buysellamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral))  'collateral
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals) + " " + XItemTicker) 'price

                            T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString) 'deniability/dispute
                            T_LVI.SubItems.Add("OPEN") 'status
                            T_LVI.SubItems.Add("") 'infotext
                            T_LVI.Tag = T_DEXContract

                            C_MyOpenOrderLVIList.Add(T_LVI)

                        End If 'myaddress

#End Region

                    Catch ex As Exception
                        IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(OPEN): -> " + ex.Message)
                    End Try

                ElseIf T_DEXContract.Status = ClsDEXContract.E_Status.RESERVED Or T_DEXContract.Status = ClsDEXContract.E_Status.DISPUTE Or T_DEXContract.Status = ClsDEXContract.E_Status.UTX_PENDING Or T_DEXContract.Status = ClsDEXContract.E_Status.TX_PENDING Then

                    SmartContractChannelOpen = False

                    Try

#Region "MyRESERVEDOrders - GUI"

                        If TBSNOAddress.Text = T_DEXContract.CurrentSellerAddress Then

                            Try

                                'Save/Update my RESERVED SellOrder to cache.dat (MyOrders Settings)
                                Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction)
                                Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status)

                                Dim Autosendinfotext As String = "False"
                                Dim AutocompleteSmartContract As String = "False"

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
                                        AutocompleteSmartContract = T_OS.AutoCompleteSmartContract.ToString
                                    End If

                                    OrderSettingsBuffer.Add(T_OS)
                                    'Broadcast info over DEXNET
                                    C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString + "</SCID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")

                                Else 'IsBuyOrder

#Region "SellOrder-Info from DEXNET"
                                    Try

                                        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                        If Not DEXNET Is Nothing Then
                                            RelMsgs = DEXNET.GetRelevantMsgs()
                                        End If

                                        Dim RelKeyFounded As Boolean = False

                                        For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                            If RelMsg.RelevantKey.Name = "<SCID>" + T_DEXContract.ID.ToString + "</SCID>" Then

                                                RelKeyFounded = True

                                                If Not RelMsg.RelevantMessage.Trim = "" Then

                                                    Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                                    If T_DEXContract.CurrentBuyerID = GetAccountID(PublicKey) Then

                                                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                        PayMet = PayMethod

                                                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                        Autosendinfotext = T_Autosendinfotext

                                                        Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                                                        AutocompleteSmartContract = T_AutocompleteSmartContract

                                                    End If


                                                End If

                                            End If

                                        Next

                                        If Not RelKeyFounded Then
                                            If Not DEXNET Is Nothing Then
                                                DEXNET.AddRelevantKey("<SCID>" + T_DEXContract.ID.ToString + "</SCID>")
                                            End If
                                        End If

                                    Catch ex As Exception
                                        IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->SellOrder-Info from DEXNET): -> " + ex.Message)
                                    End Try
#End Region

                                    If T_DEXContract.CurrencyIsCrypto() Then
                                        If T_OSList.Count = 0 Then
                                            T_OS.PaytypeString = ClsOrderSettings.E_PayType.AtomicSwap.ToString ' GetINISetting(E_Setting.PaymentType, "Other")
                                            T_OS.Infotext = "AtomicSwap=" + T_DEXContract.CurrentXItem + ":" + ClsXItemAdapter.GetXItemAddress(T_DEXContract.CurrentXItem)
                                        Else
                                            T_OS = T_OSList(0)
                                            T_OS.SetPayType()
                                            T_OS.Status = T_DEXContract.Status.ToString
                                            'PayMet = T_OS.PaytypeString
                                            'Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                            'AutocompleteSmartContract = T_OS.AutoCompleteSmartContract.ToString
                                        End If

                                        OrderSettingsBuffer.Add(T_OS)
                                    End If

                                End If

                                Dim AlreadySend As String = CheckBillingInfosAlreadySend(T_DEXContract)

#Region "Automentation"

                                If IsErrorOrWarning(AlreadySend) Then
                                    AlreadySend = "ENCRYPTED"
                                ElseIf AlreadySend.Trim = "" Then

                                    'Dim SearchAttachmentHEX As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({SignumAPI.ReferenceFinishOrder}))

                                    'If T_DEXContract.CheckForUTX Then
                                    '    AlreadySend = "PENDING"
                                    'Else
                                    '    AlreadySend = "RESERVED"
                                    'End If


                                    If T_OS.AutoSendInfotext Or T_DEXContract.CurrencyIsCrypto() Then 'autosend info to Buyer

#Region "AutoSend PaymentInfotext"
                                        Try

                                            If Not T_DEXContract.Status = ClsDEXContract.E_Status.UTX_PENDING And Not T_DEXContract.Status = ClsDEXContract.E_Status.TX_PENDING Then 'skip pendings

                                                If Not GetAutoinfoTXFromINI(T_DEXContract.CurrentCreationTransaction) Then 'Check for autosend-info-TX in Settings.ini and skip if founded (already sended)
                                                    'TODO: No info in Settings
                                                    Dim PayInfo As String = GetPaymentInfoFromOrderSettings(T_DEXContract.CurrentCreationTransaction, T_DEXContract.CurrentBuySellAmount, T_DEXContract.CurrentXAmount, T_DEXContract.CurrentXItem)

                                                    If Not PayInfo.Trim = "" Then
                                                        If PayInfo.Contains("PayPal-E-Mail=") Then
                                                            Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                            Dim ColWordsString As String = ColWords.GenerateColloquialWords(T_DEXContract.CurrentCreationTransaction.ToString, True, "-", 5)

                                                            PayInfo += " Reference/Note=" + ColWordsString
                                                        End If

                                                        Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + T_DEXContract.CurrentXItem + " " + PayInfo
                                                        Dim TXr As String = T_Interactions.SendBillingInfos(T_DEXContract.CurrentBuyerID, T_MsgStr, False, True)

                                                        If Not IsErrorOrWarning(TXr) Then
                                                            SetAutoinfoTX2INI(T_DEXContract.CurrentCreationTransaction) 'Set autosend-info-TX in Settings.ini
                                                        End If
                                                    End If


                                                End If
                                            End If

                                        Catch ex As Exception
                                            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->Autosend PaymentInfotext): -> " + ex.Message)
                                        End Try
#End Region

                                    End If

                                Else 'BillingInfo Already send, check for aproving

#Region "Inject ChainSwapHash into SmartContract for AtomicSwap"

                                    Dim Message As String = T_DEXContract.GetLastDecryptedMessageFromChat(TBSNOAddress.Text, True)
                                    If Message.Contains(T_DEXContract.CurrentXItem + ":") And Message.Contains(" ChainSwapHash=") Then

                                        Dim T_XItemTransactionID As String = GetStringBetween(Message, T_DEXContract.CurrentXItem + ":", " ChainSwapHash=")

                                        If T_DEXContract.BlocksLeft <= 2 And T_DEXContract.Status = ClsDEXContract.E_Status.RESERVED Then

                                            'AtomicSwap: RejectResponder / cancel broken swap

                                            If Not T_DEXContract.CheckForUTX() And Not T_DEXContract.CheckForTX() Then

                                                Dim Response As String = T_Interactions.RejectResponder(False)

                                                If Not IsErrorOrWarning(Response) Then
                                                    Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(XItemTicker)
                                                    Dim T_ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTransactionID)
                                                    XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTransactionID, T_ChainSwapHash, "RejectResponder: " + Response)
                                                End If

                                            End If

                                        Else

                                            'AtomicSwap: Get ChainSwapHash from XCryptoTXID from message and inject into smart contract
                                            Dim T_ChainSwapHash As String = Message.Substring(Message.IndexOf(" ChainSwapHash=") + " ChainSwapHash=".Length)

                                            If T_DEXContract.BlocksLeft > 8 Then
                                                Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(T_DEXContract.CurrentXItem)
                                                If XItem2.CheckXItemTransactionConditions(T_XItemTransactionID, T_ChainSwapHash) Then

                                                    Dim ChainSwapHashINI As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTransactionID)
                                                    If ChainSwapHashINI.Trim() = "" And T_DEXContract.CurrentChainSwapHash.Trim() = "" Then

                                                        Dim SCTX As String = T_Interactions.InjectChainSwapHash(XItem2.ChainSwapHash, False)

                                                        If Not IsErrorOrWarning(SCTX) Then

                                                            XItem2.SetXItemTransactionToINI(T_DEXContract, T_XItemTransactionID, T_ChainSwapHash)

                                                            'If GetINISetting(E_Setting.InfoOut, False) Then
                                                            '    Dim out As ClsOut = New ClsOut(Application.StartupPath)
                                                            '    out.Info2File(ChainSwapHash + " successfully injected in " + T_DEXContract.ID.ToString + " with " + SCTX)
                                                            'End If
                                                        End If

                                                    End If

                                                End If

                                            End If

                                        End If

                                    Else
                                        'TODO: handle Errors and warnings
                                        Message = T_DEXContract.GetLastDecryptedMessageFromChat(T_DEXContract.CurrentBuyerAddress, True)
                                        Message = Message

                                    End If

#End Region

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

                                    DelAutoinfoTXFromINI(T_DEXContract.CurrentCreationTransaction)  'Delete autosend-info-TX from Settings.ini

                                    If T_OS.AutoCompleteSmartContract And Not T_DEXContract.CurrencyIsCrypto() Then 'autosignal SmartContract

#Region "AutoComplete SmartContract with PayPalInfo"
                                        Try

                                            If T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_E_Mail Then

                                                Dim PayPalStatus As String = CheckPayPalTransaction(T_DEXContract)

                                                If Not PayPalStatus.Trim = "" Then
                                                    AlreadySend = PayPalStatus
                                                End If

                                                'ElseIf T_OS.PayType = ClsOrderSettings.E_PayType.PayPal_Order Then

                                                '    If AlreadySend.Contains("PayPal-Order=") Then
                                                '        Dim PayPalOrder As String = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim
                                                '        If PayPalOrder.Contains(" ") Then
                                                '            PayPalOrder = PayPalOrder.Remove(PayPalOrder.IndexOf(" "))
                                                '        End If

                                                '        Dim PayPalStatus As String = CheckPayPalOrder(T_DEXContract, PayPalOrder)

                                                '        If Not PayPalStatus.Trim = "" Then
                                                '            AlreadySend = PayPalStatus
                                                '        End If

                                                '    End If

                                            End If

                                        Catch ex As Exception
                                            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->Autocomplete): -> " + ex.Message)
                                        End Try
#End Region

                                    End If

                                End If

#End Region

                                T_LVI = New ListViewItem

                                T_LVI.Text = Confirms 'confirms
                                T_LVI.SubItems.Add(T_DEXContract.Address) 'smartcontract
                                'T_LVI.SubItems.Add(T_DEXContract.CreatorAddress) 'creator

                                If T_DEXContract.IsSellOrder Then
                                    T_LVI.SubItems.Add("SellOrder") 'type
                                Else
                                    T_LVI.SubItems.Add("BuyOrder") 'type
                                End If

                                T_LVI.SubItems.Add(PayMet) 'method
                                T_LVI.SubItems.Add(Autosendinfotext + "/" + AutocompleteSmartContract) 'autoinfo
                                T_LVI.SubItems.Add("Me") 'seller
                                T_LVI.SubItems.Add(T_DEXContract.CurrentBuyerAddress) 'buyer
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + XItemTicker) 'xamount + xitem
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'quantity
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'collateral
                                T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals) + " " + XItemTicker) 'price

#Region "Dispute"

                                Dim Proposal As String = T_DEXContract.StatusMessage

                                'If T_DEXContract.CurrentDisputeTimeout <> 0UL And Not T_DEXContract.CurrencyIsCrypto() Then ' ClsDEXContract.GetMarketCurrencyIsCrypto(T_DEXContract.CurrentXItem) Then

                                '    Dim percentage As Double = 100 / T_DEXContract.CurrentBuySellAmount * T_DEXContract.CurrentConciliationAmount
                                '    Proposal = " (Proposal:" + Dbl2LVStr(T_DEXContract.CurrentBuySellAmount - T_DEXContract.CurrentConciliationAmount, 2) + " Signa = " + Dbl2LVStr(100.0 - percentage, 2) + "%)"
                                '    Dim diffblock As Long = 0
                                '    diffblock = CLng(T_DEXContract.CurrentDisputeTimeout) - CLng(Block)

                                '    If diffblock < 0 Then
                                '        Proposal += " accepted"
                                '    ElseIf diffblock <= 1 Then
                                '        Proposal += " not appealable"
                                '    Else
                                '        Proposal += " autoaccept in: ~" + CStr(diffblock * 4) + " Min"
                                '    End If
                                'ElseIf T_DEXContract.CurrentDisputeTimeout <> 0UL And ClsDEXContract.CurrencyIsCrypto(T_DEXContract.CurrentXItem) Then
                                '    Dim diffblock As Long = 0
                                '    diffblock = CLng(T_DEXContract.CurrentDisputeTimeout) - CLng(Block)
                                '    AlreadySend += "reserved for ~" + CStr(diffblock * 4) + " Min"
                                'End If

                                If Proposal.Trim = "" Then
                                    T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString) 'deniability/dispute
                                Else
                                    T_LVI.SubItems.Add(Proposal) 'deniability/dispute
                                End If

#End Region

                                T_LVI.SubItems.Add(T_DEXContract.Status.ToString) 'status
                                T_LVI.SubItems.Add(AlreadySend) 'infotext

                                T_LVI.Tag = T_DEXContract

                                C_MyOpenOrderLVIList.Add(T_LVI)

                            Catch ex As Exception
                                IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->Seller): -> " + ex.Message)
                            End Try

                        ElseIf TBSNOAddress.Text = T_DEXContract.CurrentBuyerAddress Then ' Order.BuyerRS Then

                            'Save/ Update() my RESERVED SellOrder to cache.dat (MyOrders Settings)
                            Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(T_DEXContract.CurrentCreationTransaction) ' Order.FirstTransaction)
                            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.IsSellOrder, T_DEXContract.Status)

                            Dim Autosendinfotext As String = "False"
                            Dim AutocompleteSmartContract As String = "False"

                            If Not T_DEXContract.IsSellOrder Then ' Order.Type = "BuyOrder" Then
                                If T_OSList.Count = 0 Then
                                    T_OS.PaytypeString = GetINISetting(E_Setting.PaymentType, "Other")
                                    T_OS.Infotext = GetINISetting(E_Setting.PaymentInfoText, "Unknown")
                                Else
                                    T_OS = T_OSList(0)
                                    T_OS.Status = T_DEXContract.Status.ToString ' Order.Status
                                    PayMet = T_OS.PaytypeString
                                    Autosendinfotext = T_OS.AutoSendInfotext.ToString
                                    AutocompleteSmartContract = T_OS.AutoCompleteSmartContract.ToString
                                End If

                                OrderSettingsBuffer.Add(T_OS)
                                'Broadcast info over DEXNET
                                C_BroadcastMsgs.Add("<SCID>" + T_DEXContract.ID.ToString + "</SCID><PayType>" + PayMet.Trim + "</PayType><Autosendinfotext>" + Autosendinfotext + "</Autosendinfotext><AutocompleteSC>" + AutocompleteSmartContract + "</AutocompleteSC>")

                            Else

#Region "BuyOrder-Info from DEXNET"

                                Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

                                If Not DEXNET Is Nothing Then
                                    RelMsgs = DEXNET.GetRelevantMsgs()
                                End If

                                Dim RelKeyFounded As Boolean = False

                                For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                                    If RelMsg.RelevantKey.Name = "<SCID>" + T_DEXContract.ID.ToString + "</SCID>" Then

                                        RelKeyFounded = True

                                        If Not RelMsg.RelevantMessage.Trim = "" Then

                                            Dim PublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")

                                            If T_DEXContract.CurrentSellerID = GetAccountID(PublicKey) Then
                                                Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                                                PayMet = PayMethod

                                                Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                                                Autosendinfotext = T_Autosendinfotext

                                                Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                                                AutocompleteSmartContract = T_AutocompleteSmartContract

                                            End If

                                        End If

                                    End If

                                Next

                                If Not RelKeyFounded Then
                                    If Not DEXNET Is Nothing Then
                                        DEXNET.AddRelevantKey("<SCID>" + T_DEXContract.ID.ToString + "</SCID>")
                                    End If
                                End If
#End Region

                            End If


                            Dim AlreadySend As String = CheckBillingInfosAlreadySend(T_DEXContract)

                            If IsErrorOrWarning(AlreadySend) Then
                                AlreadySend = "ENCRYPTED"
                            ElseIf AlreadySend = "" Then

                                'T_DEXContract.Refresh()
                                'If T_DEXContract.Status = ClsDEXContract.E_Status.TX_PENDING Or T_DEXContract.Status = ClsDEXContract.E_Status.UTX_PENDING Then
                                '    AlreadySend = "PENDING"
                                'Else
                                '    AlreadySend = "RESERVED"
                                'End If


                            ElseIf AlreadySend.Trim <> "" Then

                                'Dim PayPalOrder As String = ""
                                'If AlreadySend.Contains("PayPal-Order=") Then
                                'PayPalOrder = AlreadySend.Substring(AlreadySend.IndexOf("PayPal-Order=") + 13).Trim

                                'If PayPalOrder.Trim <> "" Then
                                '    Dim PPAPI As ClsPayPal = New ClsPayPal()

                                '    AlreadySend = PPAPI.URL + "/checkoutnow?token=" + PayPalOrder '"https://www.sandbox.paypal.com/checkoutnow?token=" 
                                '    'Process.Start(AlreadySend)
                                'End If

                                'End If

#Region "ChainSwap/AtomicSwap"

                                If Not T_DEXContract.Status = ClsDEXContract.E_Status.UTX_PENDING And Not T_DEXContract.Status = ClsDEXContract.E_Status.TX_PENDING Then

                                    If (AlreadySend.Contains("AtomicSwap=" + T_DEXContract.CurrentXItem) Or T_DEXContract.CurrentChainSwapHash <> "") And T_DEXContract.CurrencyIsCrypto() Then

                                        If T_DEXContract.CurrentChainSwapHash = "" Then

                                            Dim T_ChainSwapKey As String = ByteArrayToHEXString(RandomBytes(31)) ' "aaaa1111aaaa1111" ' 'aaaa1111aaaa1111bbbb2222bbbb2222cccc3333cccc3333dddd4444dddd4444
                                            'T_ChainSwapKey += "bbbb2222bbbb2222"
                                            'T_ChainSwapKey += "cccc3333cccc3333"
                                            'T_ChainSwapKey += "dddd4444dddd4444"

                                            Dim T_FullChainSwapHash As String = GetSHA256HashString(T_ChainSwapKey).ToLower() '5682f300723e84bc877b0ebce916618415edc43663930cff67ef2381bedc3618
                                            Dim T_Key As String = "AtomicSwap=" + T_DEXContract.CurrentXItem + ":"
                                            Dim T_XCryptoAddress As String = AlreadySend.Substring(AlreadySend.IndexOf(T_Key) + T_Key.Length).Trim
                                            Dim PayInfo As String = "Infotext=" + T_DEXContract.CurrentXItem + ":"
                                            Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(T_DEXContract.CurrentXItem)
                                            Dim T_XItemTX As String = XItem2.GetXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction)

                                            If T_DEXContract.BlocksLeft > 8 Then

                                                If T_XItemTX.Trim = "" Then

                                                    Dim T_S_TX As ClsBitcoin.S_Transaction = XItem2.CreateXItemTransactionWithChainSwapHash(T_XCryptoAddress, T_DEXContract.CurrentXAmount, T_FullChainSwapHash)

                                                    'AtomicSwap: create chainswapkey/chainswaphash pair and send XItemTX

                                                    If Not IsErrorOrWarning(T_S_TX.TransactionID) And Not IsErrorOrWarning(T_S_TX.ScriptHex) Then
                                                        PayInfo += T_S_TX.TransactionID
                                                        PayInfo += " ChainSwapHash=" + T_S_TX.ScriptHex

                                                        Dim T_MsgStr As String = "SmartContract=" + T_DEXContract.Address + " Transaction=" + T_DEXContract.CurrentCreationTransaction.ToString + " " + PayInfo
                                                        Dim TXr As String = T_Interactions.SendBillingInfos(T_DEXContract.CurrentSellerID, T_MsgStr, False, True)

                                                        If Not IsErrorOrWarning(TXr) Then
                                                            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                                            Out.Info2File("ChainSwapHash BTC TX:" + T_S_TX.TransactionID)

                                                            XItem2.SetXItemTransactionToINI(T_DEXContract, T_S_TX.TransactionID, T_ChainSwapKey, T_S_TX.ScriptHex)
                                                        End If

                                                    End If

                                                End If

                                            Else
                                                'getting xitem back
                                                If Not T_XItemTX.Trim = "" Then

                                                    Dim ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTX)

                                                    If Not IsErrorOrWarning(XItem2.GetBackXItemTransaction(T_XItemTX, ChainSwapHash)) Then
                                                        'AtomicSwap: cancel sellers Contract ?

                                                        XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_XItemTX, ChainSwapHash, "GetBackXItem")

                                                    End If

                                                End If

                                            End If

                                        Else 'T_DEXContract.CurrentChainSwapHash <> ""
                                            'AtmicSwap: Redeem Smart Contract with ChainSwapKey
                                            Dim T_ChainSwapKey As String = ""

                                            Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(T_DEXContract.CurrentXItem)

                                            Dim T_BitcoinTransactionID As String = XItem2.GetXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.CurrentChainSwapHash)

                                            If Not T_BitcoinTransactionID.Trim = "" Then

                                                If T_DEXContract.BlocksLeft > 2L Then '504446
                                                    T_ChainSwapKey = XItem2.GetXItemChainSwapKeyFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_BitcoinTransactionID)

                                                    If Not T_ChainSwapKey.Trim = "" Then

                                                        Dim Response As String = T_Interactions.FinishWithChainSwapKey(T_ChainSwapKey, False)

                                                        If Not IsErrorOrWarning(Response, Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->FinishOrderWithChainSwapKey) -> " + vbCrLf, True) Then
                                                            XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_BitcoinTransactionID, T_DEXContract.CurrentChainSwapHash, "FinishWithChainSwapKey: " + Response)
                                                        End If

                                                    End If

                                                Else 'reject Order

                                                    Dim ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_BitcoinTransactionID)

                                                    If Not IsErrorOrWarning(XItem2.GetBackXItemTransaction(T_BitcoinTransactionID, ChainSwapHash)) Then
                                                        Dim MasterKeys As List(Of String) = GetPassPhrase()
                                                        If MasterKeys.Count > 0 Then

                                                            Dim Response As String = T_DEXContract.RejectResponder(MasterKeys(0), C_Fee)

                                                            If Not IsErrorOrWarning(Response, Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->RejectResponder): -> " + vbCrLf, True) Then

                                                                Dim UTX As String = Response
                                                                Dim SignumNET As ClsSignumNET = New ClsSignumNET
                                                                Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, MasterKeys(1))
                                                                Dim TX As String = C_SignumAPI.BroadcastTransaction(STX.SignedTransaction)

                                                                IsErrorOrWarning(TX, Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->RejectResponder2): -> " + vbCrLf, True)

                                                            End If
                                                        Else
                                                            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED->RejectResponder) -> No Keys")
                                                        End If
                                                    End If

                                                End If

                                            End If


                                        End If

                                    Else ' AlreadySend has no AtomicSwap-string and DEXContract has no ChainSwapHash

                                        If T_DEXContract.CurrencyIsCrypto() Then

                                            Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(XItemTicker)

                                            Dim T_BitcoinTransactionID As String = XItem2.GetXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction)

                                            If Not T_BitcoinTransactionID.Trim = "" Then

                                                If T_DEXContract.BlocksLeft <= 3L Then

                                                    Dim ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_BitcoinTransactionID)

                                                    'Dim T_BTCTransaction As ClsTransaction = XItem2.RedeemBitcoinTransaction(T_BitcoinTransactionID, GetBitcoinMainAddress(), T_RedeemScript)

                                                    'Dim T_BitcoinRAWTX As String = XItem2.SignBitcoinTransaction(T_BTCTransaction, GetBitcoinMainPrivateKey().ToLower())

                                                    'Dim T_BitcoinTXID As String = ""
                                                    'If Not T_BitcoinRAWTX.Trim = "" Then
                                                    '    T_BitcoinTXID = XItem2.SendRawBitcoinTransaction(T_BitcoinRAWTX)
                                                    'End If

                                                    If Not IsErrorOrWarning(XItem2.GetBackXItemTransaction(T_BitcoinTransactionID, ChainSwapHash)) Then
                                                        XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_BitcoinTransactionID, T_DEXContract.CurrentChainSwapHash, "GetBackXItemTransaction: noChainSwapHash")
                                                    End If

                                                End If

                                            End If

                                        Else

                                        End If

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

                                ElseIf Not T_DEXContract.Status = ClsDEXContract.E_Status.RESERVED Then

                                    If T_DEXContract.CurrencyIsCrypto Then

                                        'Dim T_BitcoinTransactionID As String = GetBitcoinTXFromINI(T_DEXContract.ID, T_DEXContract.CurrentCreationTransaction, T_DEXContract.CurrentChainSwapHash)

                                        'If Not T_BitcoinTransactionID.Trim = "" Then

                                        '    If T_DEXContract.BlocksLeft <= 3L Then

                                        '        Dim T_BTCTransaction As ClsTransaction = RedeemBitcoinTransaction(T_BitcoinTransactionID, GetBitcoinMainAddress())

                                        '        Dim T_BitcoinRAWTX As String = SignBitcoinTransaction(T_BTCTransaction, GetBitcoinMainPrivateKey())

                                        '        Dim T_BitcoinTXID As String = ""
                                        '        If Not T_BitcoinRAWTX.Trim = "" Then
                                        '            T_BitcoinTXID = SendRawBitcoinTransaction(T_BitcoinRAWTX)
                                        '        End If

                                        '        If Not IsErrorOrWarning(T_BitcoinTXID) Then

                                        '        End If

                                        '    End If

                                        'End If

                                    Else

                                    End If

                                End If

#End Region

                            End If

                            Dim CheckAttachment As String = ClsSignumAPI.ULngList2DataStr(New List(Of ULong)({C_SignumAPI.ReferenceFinishOrder}))


                            T_LVI = New ListViewItem

                            T_LVI.Text = Confirms 'confirms
                            T_LVI.SubItems.Add(T_DEXContract.Address) 'SmartContract
                            'T_LVI.SubItems.Add(T_DEXContract.CreatorAddress) 'creator

                            If T_DEXContract.IsSellOrder Then
                                T_LVI.SubItems.Add("SellOrder") 'type
                            Else
                                T_LVI.SubItems.Add("BuyOrder") 'type
                            End If

                            T_LVI.SubItems.Add(PayMet) 'method

                            'If MarketIsCrypto Then
                            '    T_LVI.SubItems.Add("Auto/Auto") 'autoinfo
                            'Else
                            T_LVI.SubItems.Add(Autosendinfotext + "/" + AutocompleteSmartContract) 'autoinfo
                            'End If

                            T_LVI.SubItems.Add(T_DEXContract.CurrentSellerAddress)  'seller
                            T_LVI.SubItems.Add("Me") 'buyer

                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentXAmount, C_Decimals) + " " + XItemTicker) 'xamount + xitem
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentBuySellAmount)) 'buysellamount
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentInitiatorsCollateral)) 'collateral
                            T_LVI.SubItems.Add(Dbl2LVStr(T_DEXContract.CurrentPrice, C_Decimals) + " " + XItemTicker) 'price

#Region "Dispute"

                            Dim Proposal As String = T_DEXContract.StatusMessage

                            'If T_DEXContract.CurrentDisputeTimeout <> 0UL And Not ClsDEXContract.CurrencyIsCrypto(T_DEXContract.CurrentXItem) Then

                            '    Dim percentage As Double = 100 / T_DEXContract.CurrentBuySellAmount * T_DEXContract.CurrentConciliationAmount
                            '    Proposal = " (Proposal:" + Dbl2LVStr(T_DEXContract.CurrentConciliationAmount, 2) + " Signa =" + Dbl2LVStr(percentage, 2) + "%)"
                            '    Dim diffblock As Long = 0
                            '    diffblock = CLng(T_DEXContract.CurrentDisputeTimeout) - CLng(Block)

                            '    If diffblock < 0 Then
                            '        Proposal += " accepted"
                            '    ElseIf diffblock <= 1 Then
                            '        Proposal += " not appealable"
                            '    Else
                            '        Proposal += " autoaccept in: ~" + CStr(diffblock * 4) + " Min"
                            '    End If
                            'ElseIf T_DEXContract.CurrentDisputeTimeout <> 0UL And ClsDEXContract.CurrencyIsCrypto(T_DEXContract.CurrentXItem) Then
                            '    Dim diffblock As Long = 0
                            '    diffblock = CLng(T_DEXContract.CurrentDisputeTimeout) - CLng(Block)
                            '    AlreadySend += "reserved for ~" + CStr(diffblock * 4) + " Min"
                            'End If

                            'T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString + " " + Proposal) 'deniability/dispute

                            If Proposal.Trim = "" Then
                                T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString + "/" + T_DEXContract.Dispute.ToString) 'deniability/dispute
                            Else
                                T_LVI.SubItems.Add(Proposal) 'deniability/dispute
                            End If

#End Region
                            T_LVI.SubItems.Add(T_DEXContract.Status.ToString) 'status
                            T_LVI.SubItems.Add(AlreadySend) 'infotext

                            T_LVI.Tag = T_DEXContract

                            C_MyOpenOrderLVIList.Add(T_LVI)

                        End If 'myaddress

#End Region

                    Catch ex As Exception
                        IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(RESERVED): -> " + ex.Message)
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
                            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(Delete): -> " + ex.Message)
                        End Try


                        Dim TOSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(HistoryOrder.CreationTransaction)

                        Try

                            If TOSList.Count > 0 Then
                                Dim DelTOSID As ULong = 0
                                For Each T_TOS As ClsOrderSettings In TOSList
                                    If T_TOS.TransactionID = HistoryOrder.CreationTransaction Then
                                        DelTOSID = T_TOS.SmartContractID
                                        Exit For
                                    End If
                                Next

                                If Not DelTOSID = 0 Then
                                    DelOrderSettings(DelTOSID)
                                End If

                            End If

                        Catch ex As Exception
                            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(DeleteOrdersettingbuffer): -> " + ex.Message)
                        End Try

                        'If HistoryOrder.XItem.Contains(Market) Then

                        If TBSNOAddress.Text = HistoryOrder.SellerRS Then

                            Try

                                'Dim BTCTX As ClsTransaction = CreateBitcoinTransaction(HistoryOrder.XAmount, GetBitcoinMainAddress(), GetBitcoinMainAddress())
                                'BTCTX = BTCTX

                                'If MyClosedOrderLVIList.Count < 100 Then

                                Dim T_DEXOrder As S_DEXOrder = New S_DEXOrder
                                T_DEXOrder.ContractAddress = T_DEXContract.Address
                                T_DEXOrder.Order = HistoryOrder
                                T_DEXOrder.Contract = T_DEXContract

                                If Not HistoryOrder.ChainSwapKey.Trim = "" Then
                                    'AtomicSwap: get last msg (with ChainSwapKey) and redeem XItem

                                    Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(HistoryOrder.XItem)

                                    Dim BitcoinTX As String = XItem2.GetXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction)
                                    Dim ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, BitcoinTX)

                                    If Not BitcoinTX.Trim() = "" Then

                                        Dim T_BitcoinTXID As String = XItem2.ClaimXItemTransactionWithChainSwapKey(BitcoinTX, ChainSwapHash, HistoryOrder.ChainSwapKey)
                                        If Not T_BitcoinTXID.Trim() = "" And Not IsErrorOrWarning(T_BitcoinTXID, "Redeem BTC TX:", True) Then  '.GetBackXItemTransaction(GetBitcoinMainAddress(), RedeemScript)
                                            'Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                            'Out.Info2File("Redeem BTC TX:" + T_BitcoinTXID)
                                        End If

                                        XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, BitcoinTX, GetSHA256HashString(HistoryOrder.ChainSwapKey.Trim()), "ClaimXItemTransactionWithChainSwapKey: " + HistoryOrder.ChainSwapKey)

#Region "old"
                                        'Dim T_BTCTX As ClsTransaction = XItem2.RedeemBitcoinTransaction(BitcoinTX, GetBitcoinMainAddress(), RedeemScript)

                                        'Dim BitcoinPrivateKey As String = GetBitcoinMainPrivateKey()

                                        'If Not BitcoinPrivateKey = "" Then
                                        '    Dim T_BitcoinTXID As String = XItem2.SignBitcoinTransactionWithChainSwapKeyAndRedeemScript(T_BTCTX, BitcoinPrivateKey.ToLower(), HistoryOrder.ChainSwapKey) ' SignBitcoinTransactionWithChainSwapKey(T_BTCTX, BitcoinPrivateKey.ToLower(), HistoryOrder.ChainSwapKey) 'TODO: ###AtomicSwap Sign BitcoinTX with PrivateKey and ChainSwapKey

                                        '    If Not T_BitcoinTXID.Trim = "" Then

                                        '        T_BitcoinTXID = XItem2.SendRawBitcoinTransaction(T_BitcoinTXID)

                                        '        If Not IsErrorOrWarning(T_BitcoinTXID, "Redeem BTC TX:", True) Then
                                        '            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                                        '            Out.Info2File("Redeem BTC TX:" + T_BitcoinTXID)

                                        '            XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, BitcoinTX, GetSHA256HashString(HistoryOrder.ChainSwapKey.Trim))
                                        '        End If

                                        '    Else
                                        '        IsErrorOrWarning(Application.ProductName + "-warning in MultiThreadSetSmartContract2LV(AtomicSwapError) -> No BitcoinTransactionID")
                                        '    End If
                                        'Else
                                        '    IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(AtomicSwapError) -> No Keys")
                                        'End If
#End Region

                                    End If

                                End If

#Region "old"
                                'If Not HistoryOrder.ChainSwapHash.Trim = "" Then
                                '    HistoryOrder = HistoryOrder
                                'End If

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
#End Region

                                C_MyClosedOrderLVIList.Add(T_DEXOrder)

                                'End If

                            Catch ex As Exception
                                IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(SetClosedSellOrder): -> " + ex.Message)
                            End Try

                        ElseIf TBSNOAddress.Text = HistoryOrder.BuyerRS Then ' Order.BuyerRS Then

                            Try

                                'If MyClosedOrderLVIList.Count < 100 Then

                                Dim XItem2 As AbsClsXItem = ClsXItemAdapter.NewXItem(HistoryOrder.XItem)

                                If Not IsNothing(XItem2) Then

                                    Dim T_XItemTransaction As String = XItem2.GetXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction)

                                    If Not T_XItemTransaction.Trim = "" Then
                                        Dim ChainSwapHash As String = XItem2.GetXItemChainSwapHashFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, T_XItemTransaction)

                                        If Not IsErrorOrWarning(XItem2.GetBackXItemTransaction(T_XItemTransaction, ChainSwapHash)) Then

                                        End If

                                        XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, T_XItemTransaction, ChainSwapHash, "HistoryOrder-ChainSwapHash: " + XItem2.ChainSwapHash)

#Region "deprecaded"
                                        'Dim T_BitcoinTransaction As ClsTransaction = New ClsTransaction(T_BTCTX, New List(Of String), RedeemScript)

                                        'If T_BitcoinTransaction.IsTimeOut() Then

                                        '    Dim BitcoinPrivateKey As String = GetBitcoinMainPrivateKey().ToLower()

                                        '    If Not BitcoinPrivateKey = "" Then

                                        '        T_BitcoinTransaction.C_FeesNQTPerByte = 5

                                        '        Dim T_BitcoinRAWTX As String = XItem2.SignBitcoinTransactionWithRedeemScript(T_BitcoinTransaction, BitcoinPrivateKey)

                                        '        Dim T_BitcoinTXID As String = ""
                                        '        If Not T_BitcoinRAWTX.Trim = "" Then
                                        '            T_BitcoinTXID = XItem2.SendRawBitcoinTransaction(T_BitcoinRAWTX)
                                        '        End If

                                        '        If Not IsErrorOrWarning(T_BitcoinTXID) Then
                                        '            Dim T_INIScript As String = XItem2.GetXItemRedeemScriptFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, T_BTCTX)
                                        '            XItem2.DelXItemTransactionFromINI(T_DEXContract.ID, HistoryOrder.CreationTransaction, T_BTCTX, T_INIScript)
                                        '        End If

                                        '    End If

                                        'End If
#End Region

                                    End If

                                End If

                                Dim T_DEXOrder As S_DEXOrder = New S_DEXOrder
                                T_DEXOrder.ContractAddress = T_DEXContract.Address
                                T_DEXOrder.Order = HistoryOrder
                                T_DEXOrder.Contract = T_DEXContract

#Region "old"
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
#End Region

                                C_MyClosedOrderLVIList.Add(T_DEXOrder)

                                'End If

                            Catch ex As Exception
                                IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(SetClosedBuyOrder): -> " + ex.Message)
                            End Try

                        End If 'myaddress

                        'End If 'market

#End Region

                    End If

                Next

#End Region


                If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or T_DEXContract.Status = ClsDEXContract.E_Status.CLOSED Or T_DEXContract.Status = ClsDEXContract.E_Status.CANCELED Then

                    If T_DEXContract.IsFrozen.ToString.ToLower = "true" And T_DEXContract.IsFinished.ToString.ToLower = "true" And T_DEXContract.IsDead.ToString.ToLower <> "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = T_DEXContract.Address  'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString)
                        T_LVI.Tag = T_DEXContract

                        C_OpenChannelLVIList.Add(T_LVI)

                    ElseIf T_DEXContract.IsDead.ToString.ToLower = "true" Then

                        T_LVI = New ListViewItem

                        T_LVI.Text = T_DEXContract.Address  'SmartContract
                        T_LVI.SubItems.Add("Free to use") 'Status
                        T_LVI.SubItems.Add(T_DEXContract.Deniability.ToString)
                        T_LVI.BackColor = Color.Crimson
                        T_LVI.ForeColor = Color.White
                        T_LVI.Tag = T_DEXContract

                        C_OpenChannelLVIList.Add(T_LVI)

                    End If

                End If

            End If

#End Region

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in MultiThreadSetSmartContract2LV(): -> " + ex.ToString)
        End Try

    End Sub

    Private Function Loading() As Boolean

        'Try

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        Dim x As List(Of String) = C_SignumAPI.GetBalance(TBSNOAddress.Text)
        TBSNOBalance.Text = GetDoubleBetweenFromList(x, "<available>", "</available>").ToString

        LabXItem.Text = CurrentMarket
        LabXitemAmount.Text = CurrentMarket + " Amount: "

        C_Fees = C_SignumAPI.GetFees()
        C_Fee = C_SignumAPI.GetFee(ClsSignumAPI.E_Fee.Standard)

        LabMinFee.Text = C_Fee.ToString() + " Signa"

        C_MarketIsCrypto = ClsDEXContract.CurrencyIsCrypto(CurrentMarket)
        C_Decimals = ClsDEXContract.GetCurrencyDecimals(CurrentMarket)

        NUDSNOItemAmount.DecimalPlaces = C_Decimals
        C_CSVSmartContractList.Clear()
        OrderSettingsBuffer.Clear()

        Dim Wait As Boolean = GetAndCheckSmartContracts()
        Wait = MultithreadMonitor()

        C_DEXContractList.Clear()

        Dim T_DEXSmartContractList As List(Of List(Of String)) = GetDEXContractsFromCSV()

        OrderSettingsBuffer = GetOrderSettings()

        Dim HistoryOrderToXMLThreadList As List(Of Threading.Thread) = New List(Of Threading.Thread)

        For Each APIRequest As S_APIRequest In APIRequestList

            Dim HistoryOrderToXMLThread As Threading.Thread = New Threading.Thread(AddressOf HistoryOrderToXML)
            HistoryOrderToXMLThread.Start(APIRequest)
            HistoryOrderToXMLThreadList.Add(HistoryOrderToXMLThread)

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

        SaveSmartContractsToCSV(C_CSVSmartContractList, OrderSettingsBuffer)

        If T_DEXSmartContractList.Count = 0 Then
            T_DEXSmartContractList = GetDEXContractsFromCSV()
        End If

        If T_DEXSmartContractList.Count <> 0 Then
            C_DEXSmartContractList.Clear()
        End If

        If C_DEXSmartContractList.Count = 0 Then
            For Each T_DEX As List(Of String) In T_DEXSmartContractList
                If T_DEX(1) = "True" Then
                    C_DEXSmartContractList.Add(T_DEX(0))
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

    Private Function XItemLoader() As Boolean

        For Each XItem As String In CoBxMarket.Items

            If ClsDEXContract.CurrencyIsCrypto(XItem) Then

                Select Case XItem
                    Case "BTC"
                        Dim Bitcoin As AbsClsXItem = ClsXItemAdapter.NewXItem(XItem)
                        Dim Info As String = Bitcoin.GetXItemInfo()

                        If Not IsErrorOrWarning(Info) And Not Info.Trim() = "" Then
                            Dim Wait As Boolean = LoadBitcoin()
                        End If
                    Case Else
                        ' TODO: Other CryptoCurrencies

                End Select

            End If

        Next

        Return True

    End Function

    Private Sub HistoryOrderToXML(ByVal Input As Object)

        Dim APIRequest As S_APIRequest = DirectCast(Input, S_APIRequest)

        If Not APIRequest.Result Is Nothing Then
            If APIRequest.Result.GetType = GetType(String) Then

                Dim T_SmartContractList As List(Of String) = New List(Of String)(APIRequest.Result.ToString.Split(","c))

                Dim T_SmartContract As S_SmartContract = New S_SmartContract With {.ID = Convert.ToUInt64(T_SmartContractList(0)), .IsDEX_SC = False}
                C_CSVSmartContractList.Add(T_SmartContract)

            Else

                Dim T_DEXContract As ClsDEXContract = DirectCast(APIRequest.Result, ClsDEXContract)
                C_DEXContractList.Add(T_DEXContract)

#Region "HistoryOrders"

                Dim HistoryOrdersString As String = ""

                For i As Integer = 0 To T_DEXContract.ContractOrderHistoryList.Count - 1

                    Application.DoEvents()

                    Dim HistoryOrder As ClsDEXContract.S_Order = T_DEXContract.ContractOrderHistoryList(i)

                    If HistoryOrder.StartTimestamp <= ClsSignumAPI.TimeToUnix(Now.AddDays(-31)) Then
                        Continue For
                    End If

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

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.ChainSwapKey.ToString, "ChainSwapKey")
                    'HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.ChainSwapHash.ToString, "ChainSwapHash")

                    HistoryOrdersString += ClsDEXNET.CreateXMLMessage(HistoryOrder.Status.ToString, "Status")
                    HistoryOrdersString += "</" + i.ToString + ">"

                Next

#End Region

                Dim T_SmartContract As S_SmartContract = New S_SmartContract With {.ID = T_DEXContract.ID, .IsDEX_SC = True, .HistoryOrders = HistoryOrdersString}
                C_CSVSmartContractList.Add(T_SmartContract)

            End If
        End If

    End Sub

    Private Sub LoadHistory(ByVal T_input As Object)

        Try

            Dim Input As List(Of Object) = New List(Of Object)

            If T_input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_input, List(Of Object))
            Else
                IsErrorOrWarning(Application.ProductName + "-error in HistoryLoad(DirectCast): -> " + T_input.GetType.Name)
            End If


            Dim CoBxChartVal As Integer = DirectCast(Input(0), Integer)
            Dim CoBxTickVal As Integer = DirectCast(Input(1), Integer)

            Dim Xitem As String = DirectCast(Input(2), String)

            Dim T_OrderList As List(Of ClsDEXContract.S_Order) = New List(Of ClsDEXContract.S_Order)
            Dim T_OpenDEXContractList As List(Of ClsDEXContract) = New List(Of ClsDEXContract)


            For i As Integer = 0 To C_DEXContractList.Count - 1

                Dim DEXContract As ClsDEXContract = C_DEXContractList(i)

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
                LoadTCPAPIDEXContracts(C_DEXContractList)
                LoadTCPAPIOpenOrders(T_OpenDEXContractList)
                LoadTCPAPIHistorys(T_OrderList)
            End If


            Dim Chart As List(Of S_Candle) = GetCandles(Xitem, T_OrderList, CoBxChartVal, CoBxTickVal)

            Dim Minval As Double = Double.MaxValue
            Dim Maxval As Double = 0.0

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
                C_PanelForTradeTrackerSlot.Controls(0).Tag = New List(Of Object)({Xitem, Minval, Maxval, Chart(Chart.Count - 1).CloseValue, Chart})

                TradeTrackCalcs(C_PanelForTradeTrackerSlot.Controls(0))
            End If

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in LoadHistory(): -> " + ex.Message)
        End Try

    End Sub

    Private Function TradeTrackCalcs(ByVal input As Object) As Boolean

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


            Dim EMAFastList As List(Of Object) = EMAx(XList, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDEMA1.Text) * 1440) / 5)) '1440 min * textbox (12) / 5min candle
            If TTSlot.ChBxEMA1.Checked = True Then

                Dim EMAFastGraph As Graph.S_Graph = New Graph.S_Graph
                EMAFastGraph.GraphArt = Graph.E_GraphArt.EMAFast
                EMAFastGraph.MinValue = MinVals
                EMAFastGraph.MaxValue = MaxVals

                EMAFastGraph.PieceGraphList = DateIntToCandle(EMAFastList)

                Upper_Big_Graph.GraphValuesList.Add(EMAFastGraph)
            Else

            End If


            Dim EMASlowList As List(Of Object) = EMAx(XList, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDEMA2.Text) * 1440) / 5)) '1440 min * textbox (26) / 5min candle
            If TTSlot.ChBxEMA2.Checked = True Then

                Dim EMASlowGraph As Graph.S_Graph = New Graph.S_Graph
                EMASlowGraph.MinValue = MinVals
                EMASlowGraph.MaxValue = MaxVals
                EMASlowGraph.GraphArt = Graph.E_GraphArt.EMASlow
                EMASlowGraph.PieceGraphList = DateIntToCandle(EMASlowList)

                Upper_Big_Graph.GraphValuesList.Add(EMASlowGraph)
            Else

            End If

            Dim MACDL As List(Of Object) = MACDx(EMAFastList, EMASlowList)
            Dim MinMACD As Double = Double.MaxValue
            Dim MaxMACD As Double = 0.0

            For Each MAC As Object In MACDL

                Dim T_MAC As List(Of Object) = New List(Of Object)
                If MAC.GetType.Name = GetType(List(Of Object)).Name Then
                    T_MAC = DirectCast(MAC, List(Of Object))
                Else
                    IsErrorOrWarning(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + MAC.GetType.Name)
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



            Dim SignalList As List(Of Object) = EMAx(MACDL, Convert.ToInt32((Integer.Parse(TTSlot.TBMACDSig.Text) * 1440) / 5))
            If TTSlot.ChBxMACDSig.Checked = True Then

                Dim SignalGraph As Graph.S_Graph = New Graph.S_Graph
                SignalGraph.MinValue = MinMACD
                SignalGraph.MaxValue = MaxMACD
                SignalGraph.GraphArt = Graph.E_GraphArt.Signal
                SignalGraph.PieceGraphList = DateIntToCandle(SignalList)

                Lower_Small_Graph.GraphValuesList.Add(SignalGraph)

            Else

            End If



            Dim MSigL As List(Of Object) = MACDx(MACDL, SignalList)

            Dim MinMSig As Double = Double.MaxValue
            Dim MaxMSig As Double = 0.0

            Dim PosHG As Integer = 0
            Dim NegHG As Integer = 0

            For Each MSigi As Object In MSigL

                Dim TMSigi As List(Of Object) = New List(Of Object)

                If MSigi.GetType.Name = GetType(List(Of Object)).Name Then
                    TMSigi = DirectCast(MSigi, List(Of Object))
                Else
                    IsErrorOrWarning(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + MSigi.GetType.Name)
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


            Dim RSIL As List(Of Object) = RSIx(CandleGraph.PieceGraphList, Integer.Parse(TTSlot.TBRSICandles.Text) * 10)

            Dim MinRSI As Double = Double.MaxValue
            Dim MaxRSI As Double = 0.0

            For Each RSIi As Object In RSIL

                Dim TRSIi As List(Of Object) = New List(Of Object)
                If RSIi.GetType.Name = GetType(List(Of Object)).Name Then
                    TRSIi = DirectCast(RSIi, List(Of Object))
                Else
                    IsErrorOrWarning(Application.ProductName + "-error in TradeTrackCalcs(DirectCast): -> " + RSIi.GetType.Name)
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
                MultiInvoker(TTSlot.LabLast, "ForeColor", C_SignumLightGreen)

            ElseIf HGTrend = "↓" Then
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Red)
            Else
                MultiInvoker(TTSlot.LabLast, "ForeColor", Color.Black)
            End If

            MultiInvoker(TTSlot.LabMIN, "Text", String.Format("{0:#0.00000000}", MinVals))

            TTSlot.Chart_EMA_Graph = Upper_Big_Graph
            TTSlot.MACD_RSI_TR_GraphList = Lower_Small_Graph


        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in TradeTrackCalcs(): -> " + ex.Message)
        End Try

        Return True

    End Function


#Region "TCP API"

    Public Property TCPAPI As ClsTCPAPI

    Private Sub LoadTCPAPIDEXContracts(ByVal DEXContracts As List(Of ClsDEXContract))

        Try

            Dim ChannelsJSON As String = "{""application"":""SnipSwapDEX"",""interface"":""API"",""version"":""1"",""contentType"":""application/json"",""response"":""SmartContract"",""data"":[ "

            For Each T_Channel As ClsDEXContract In DEXContracts

                ChannelsJSON += "{"
                ChannelsJSON += """at"":""" + T_Channel.ID.ToString() + ""","
                ChannelsJSON += """atRS"":""" + T_Channel.Address + ""","
                ChannelsJSON += """creatorID"":""" + T_Channel.CreatorID.ToString() + ""","
                ChannelsJSON += """creatorRS"":""" + T_Channel.CreatorAddress + ""","
                ChannelsJSON += """status"":""" + T_Channel.Status.ToString() + ""","
                ChannelsJSON += """deniability"":""" + T_Channel.Deniability.ToString().ToLower() + """"
                ChannelsJSON += "},"

            Next

            ChannelsJSON = ChannelsJSON.Remove(ChannelsJSON.Length - 1)
            ChannelsJSON += "]}"

            Dim Parameters As List(Of String) = New List(Of String) '({"type=" + Type, "pair=" + XItem})
            RefreshTCPAPIResponse("SmartContract", Parameters, ChannelsJSON)

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in LoadTCPAPIOpenOrders(): -> " + ex.Message)
        End Try

    End Sub

    Private Sub LoadTCPAPIOpenOrders(ByVal OpenDEXContracts As List(Of ClsDEXContract))

        Try

            Dim OpenOrdersJSON As String = "{""application"":""SnipSwapDEX"",""interface"":""API"",""version"":""1"",""contentType"":""application/json"",""response"":""Orders"",""data"":[ "

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

                Dim Collateral As String = String.Format("{0:#0.00000000}", T_Order.CurrentInitiatorsCollateral).Replace(",", ".")
                Dim SignaAmount As String = String.Format("{0:#0.00000000}", T_Order.CurrentBuySellAmount).Replace(",", ".")
                Dim Price As String = String.Format("{0:#0.00000000}", T_Order.CurrentPrice).Replace(",", ".")
                Dim XItem As String = T_Order.CurrentXItem + "_SIGNA"
                Dim XAmount As String = String.Format("{0:#0.00000000}", T_Order.CurrentXAmount).Replace(",", ".")

                OpenOrdersJSON += "{""at"":""" + Contract + ""","
                OpenOrdersJSON += """type"":""" + Type + ""","
                OpenOrdersJSON += """seller"":""" + Seller + ""","
                OpenOrdersJSON += """buyer"":""" + Buyer + ""","
                OpenOrdersJSON += """signaCollateral"": " + Collateral + ","
                OpenOrdersJSON += """signa"": " + SignaAmount + ","
                OpenOrdersJSON += """xItem"":""" + XItem + ""","
                OpenOrdersJSON += """xAmount"": " + XAmount + ","
                OpenOrdersJSON += """price"": " + Price + "},"

            Next


            For Each OffchainBuyOrder As S_PublicOrdersListViewEntry In C_BuyOrderLVOffChainEList

                Dim T_OffChainBuyOrder As S_OffchainBuyOrder = DirectCast(OffchainBuyOrder.Tag, S_OffchainBuyOrder)

                Dim Type As String = ""
                If T_OffChainBuyOrder.Ask = "WantToBuy" Then
                    Type = "BuyOrder"
                Else
                    Type = "SellOrder"
                End If

                Dim Seller As String = ""
                Dim AccRS As String = ClsReedSolomon.Encode(GetAccountID(T_OffChainBuyOrder.PubKey))
                Dim Buyer As String = GlobalSignumPrefix + AccRS + "-" + ClsBase36.EncodeHexToBase36(T_OffChainBuyOrder.PubKey)

                Dim SignaAmount As String = String.Format("{0:#0.00000000}", T_OffChainBuyOrder.Amount).Replace(",", ".")
                Dim XItem As String = T_OffChainBuyOrder.XItem + "_SIGNA"
                Dim XAmount As String = String.Format("{0:#0.00000000}", T_OffChainBuyOrder.XAmount).Replace(",", ".")
                Dim Price As String = String.Format("{0:#0.00000000}", T_OffChainBuyOrder.XAmount / T_OffChainBuyOrder.Amount).Replace(",", ".")

                OpenOrdersJSON += "{""at"":""OffChainOrder"","
                OpenOrdersJSON += """type"":""" + Type + ""","
                OpenOrdersJSON += """seller"":""" + Seller + ""","
                OpenOrdersJSON += """buyer"":""" + Buyer + ""","
                OpenOrdersJSON += """signaCollateral"": 0.0,"
                OpenOrdersJSON += """signa"": " + SignaAmount + ","
                OpenOrdersJSON += """xItem"":""" + XItem + ""","
                OpenOrdersJSON += """xAmount"": " + XAmount + ","
                OpenOrdersJSON += """price"": " + Price + "},"

            Next

            If C_OffchainBuyOrder.Ask.Trim() <> "" Then

                Dim AccRS As String = ClsReedSolomon.Encode(GetAccountID(C_OffchainBuyOrder.PubKey))
                Dim Buyer As String = GlobalSignumPrefix + AccRS + "-" + ClsBase36.EncodeHexToBase36(C_OffchainBuyOrder.PubKey)

                Dim SignaAmount As String = String.Format("{0:#0.00000000}", C_OffchainBuyOrder.Amount).Replace(",", ".")
                Dim XItem As String = C_OffchainBuyOrder.XItem
                Dim XAmount As String = String.Format("{0:#0.00000000}", C_OffchainBuyOrder.XAmount).Replace(",", ".")
                Dim Price As String = String.Format("{0:#0.00000000}", C_OffchainBuyOrder.XAmount / C_OffchainBuyOrder.Amount).Replace(",", ".")

                OpenOrdersJSON += "{""at"":""OffChainOrder"","
                OpenOrdersJSON += """type"":""BuyOrder"","
                OpenOrdersJSON += """seller"":"""","
                OpenOrdersJSON += """buyer"":""" + Buyer + ""","
                OpenOrdersJSON += """signaCollateral"": 0.0,"
                OpenOrdersJSON += """signa"": " + SignaAmount + ","
                OpenOrdersJSON += """xItem"":""" + XItem + ""","
                OpenOrdersJSON += """xAmount"": " + XAmount + ","
                OpenOrdersJSON += """price"": " + Price + "},"

            End If

            OpenOrdersJSON = OpenOrdersJSON.Remove(OpenOrdersJSON.Length - 1)
            OpenOrdersJSON += "]}"

            Dim Parameters As List(Of String) = New List(Of String) '({"type=" + Type, "pair=" + XItem})
            RefreshTCPAPIResponse("Orders", Parameters, OpenOrdersJSON)

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in LoadTCPAPIOpenOrders(): -> " + ex.Message)
        End Try

    End Sub
    Private Sub LoadTCPAPIHistorys(ByVal OrderList As List(Of ClsDEXContract.S_Order))

        Try

            'Dim Currencies As List(Of String) = New List(Of String)({"AUD", "BRL", "BTC", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"})
            'CoBxChart.Items.AddRange({1, 3, 7, 15, 30})
            'CoBxTick.Items.AddRange({1, 5, 15, 30, 60, 360, 720})

            Dim Currency_Day_TickList As List(Of List(Of String)) = GetTCPAPICurrencyDaysTicks(SupportedCurrencies)

            Try

                For Each CDT As List(Of String) In Currency_Day_TickList

                    Dim Curr As String = CDT(0)
                    Dim Day As String = CDT(1)
                    Dim Tick As String = CDT(2)

                    Dim ViewThread As Threading.Thread = New Threading.Thread(AddressOf LoadTCPAPIHistory)
                    ViewThread.Start(New List(Of Object)({Curr, OrderList, Day.ToString, Tick.ToString}))
                    'Threading.Thread.Sleep(10)
                Next

            Catch ex As Exception
                TCPAPI.StopAPIServer()
                MultiInvoker(StatusLabel, "Text", "LoadTCPAPIHistorys(): " + ex.Message + " TCPAPIServer stopped")
            End Try


            'LabMSGs.Text = "MSGs: " + TCPAPI.StatusMSG.Count.ToString

            Dim T_FrmDevelope As FrmDevelope = DirectCast(GetSubForm("FrmDevelope"), FrmDevelope)
            If Not T_FrmDevelope Is Nothing Then

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
            IsErrorOrWarning(Application.ProductName + "-error in LoadTCPAPIHistorys(): -> " + ex.Message)
        End Try

    End Sub
    Private Function GetTCPAPICurrencyDaysTicks(Optional ByVal OCurrencys As List(Of String) = Nothing) As List(Of List(Of String))

        Try

            Dim Currency_Day_Tick As List(Of List(Of String)) = New List(Of List(Of String))

            Dim Currencys As List(Of String) = New List(Of String)

            If OCurrencys Is Nothing Then
                Currencys.Add(CurrentMarket)
            Else
                Currencys.AddRange(OCurrencys.ToArray)
            End If


            Dim Days As List(Of Integer) = New List(Of Integer)({1})
            Dim TickMins As List(Of Integer) = New List(Of Integer)({5, 15, 30})

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


            Days = New List(Of Integer)({7})
            TickMins = New List(Of Integer)({360, 720})


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
            TickMins = New List(Of Integer)({360, 720})

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
            IsErrorOrWarning(Application.ProductName + "-error in GetTCPAPICurrencyDaysTicks(): -> " + ex.Message)
            Return New List(Of List(Of String))
        End Try

    End Function
    Private Sub LoadTCPAPIHistory(ByVal T_Input As Object)

        Try

            Dim Input As List(Of Object) = New List(Of Object)

            If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
                Input = DirectCast(T_Input, List(Of Object))
            Else
                IsErrorOrWarning(Application.ProductName + "-error in LoadTCPAPIHistory(DirectCast): -> " + T_Input.GetType.Name)
            End If


            Dim XItem As String = DirectCast(Input(0), String)
            Dim OrderList As List(Of ClsDEXContract.S_Order) = DirectCast(Input(1), List(Of ClsDEXContract.S_Order))
            Dim Days As String = DirectCast(Input(2), String)
            Dim Tick As String = DirectCast(Input(3), String)


            Dim Chart As List(Of S_Candle) = GetCandles(XItem.Remove(XItem.IndexOf("_")), OrderList, Integer.Parse(Days), Integer.Parse(Tick))

            Dim TradeHistoryJSON As String = "{""application"":""SnipSwapDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""GetCandles"",""pair"":""" + XItem.Trim + """,""days"":""" + Days.Trim + """,""tickmin"":""" + Tick.Trim + """,""data"":[ "

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
            RefreshTCPAPIResponse("Candles", Parameters, TradeHistoryJSON)

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in LoadTCPAPIHistory(): -> " + ex.Message)
            MultiInvoker(StatusLabel, "Text", "LoadTCPAPIHistory(): " + ex.Message)
        End Try

    End Sub
    Private Sub RefreshTCPAPIResponse(ByVal Command As String, ByVal Parameters As List(Of String), ByVal ResponseMSG As String)

        Try

            Dim FoundCommand As Boolean = False
            For i As Integer = 0 To TCPAPI.ResponseMSGList.Count - 1
                Dim T_Response As ClsTCPAPI.API_Response = TCPAPI.ResponseMSGList(i)

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
                Response.API_Version = "v1"
                Response.API_Command = Command
                Response.API_Response = ResponseMSG
                Response.API_Parameters = New List(Of String)(Parameters.ToArray)

                TCPAPI.ResponseMSGList.Add(Response)

            End If

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in RefreshTCPAPIResponse(): -> " + ex.Message)
        End Try

    End Sub
    Private Function CompareLists(ByVal List1 As List(Of String), List2 As List(Of String)) As Boolean

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
            IsErrorOrWarning(Application.ProductName + "-error in CompareLists(): -> " + ex.Message)
        End Try

        Return True

    End Function

#End Region 'TCP API


#Region "DEXNET"
    Public Property DEXNET As ClsDEXNET

    Public Function InitiateDEXNET() As Boolean

        Try

            If GetINISetting(E_Setting.DEXNETEnable, True) Then

                If DEXNET Is Nothing Then
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

                If ChBxBuyFilterShowOffChainOrders.Checked Then
                    If Not DEXNET Is Nothing Then
                        DEXNET.AddRelevantKey("<Ask>WantToBuy</Ask>", True, "PublicKey")
                        DEXNET.AddRelevantKey("<Ask>CancelBuyOrder</Ask>", True)
                    End If
                End If

            End If

        Catch ex As Exception
            IsErrorOrWarning(Application.ProductName + "-error in InitiateDEXNET(): -> " + ex.Message)
        End Try

        Return True

    End Function

    Private Property RelevantMsgsBuffer As List(Of S_RelevantMsg) = New List(Of S_RelevantMsg)

    Private Sub SetDEXNETRelevantMsgsToLVs()

        Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = New List(Of ClsDEXNET.S_RelevantMessage)

        If Not DEXNET Is Nothing Then
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

#End Region 'Clean RelevantMsgBuffer

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

#End Region 'Fill RelevantMsgBuffer

        C_BuyOrderLVOffChainEList.Clear()

        For ii As Integer = 0 To RelevantMsgsBuffer.Count - 1
            Dim RelMsg As S_RelevantMsg = RelevantMsgsBuffer(ii)

            If RelMsg.Setted Then
                Continue For
            End If

            If RelMsg.RelevantMessage.Contains("<SCID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<PayType>") And RelMsg.RelevantMessage.Contains("<Autosendinfotext>") And RelMsg.RelevantMessage.Contains("<AutocompleteSC>") Then

#Region "Refresh LVs"

                Dim RM_SmartContractID As String = GetStringBetween(RelMsg.RelevantMessage, "<SCID>", "</SCID>")
                'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")
                Dim SmartContractIDRSList As List(Of String) = C_SignumAPI.RSConvert(ULong.Parse(RM_SmartContractID))
                Dim RM_SmartContractRS As String = GetStringBetweenFromList(SmartContractIDRSList, "<accountRS>", "</accountRS>")

                Dim RM_AccountPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")
                Dim RM_AccountPublicKeyList As List(Of String) = C_SignumAPI.RSConvert(GetAccountID(RM_AccountPublicKey))
                Dim RM_AccountRS As String = GetStringBetweenFromList(RM_AccountPublicKeyList, "<accountRS>", "</accountRS>")

                Dim ForContinue As Boolean = False
                For Each LVI As ListViewItem In LVSellorders.Items
                    Dim LVI_SmartContractRS As String = Convert.ToString(GetLVColNameFromSubItem(LVSellorders, "Smart Contract", LVI))
                    Dim LVI_SellerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVSellorders, "Seller", LVI))

                    If RM_SmartContractRS = LVI_SmartContractRS And RM_AccountRS = LVI_SellerRS Then

                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                        SetLVColName2SubItem(LVSellorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                        SetLVColName2SubItem(LVSellorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                        SetLVColName2SubItem(LVSellorders, LVI, "Autofinish", T_AutocompleteSmartContract)

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
                    Dim LVI_SmartContractRS As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Smart Contract", LVI))
                    Dim LVI_BuyerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVBuyorders, "Buyer", LVI))

                    If RM_SmartContractRS = LVI_SmartContractRS And RM_AccountRS = LVI_BuyerRS Then

                        Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                        SetLVColName2SubItem(LVBuyorders, LVI, "Method", PayMethod)

                        Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                        SetLVColName2SubItem(LVBuyorders, LVI, "Autoinfo", T_Autosendinfotext)

                        Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                        SetLVColName2SubItem(LVBuyorders, LVI, "Autofinish", T_AutocompleteSmartContract)

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

                    Dim LVI_SmartContractRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Smart Contract", LVI))
                    Dim LVI_Type As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Type", LVI))
                    Dim LVI_SellerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Seller", LVI))
                    Dim LVI_BuyerRS As String = Convert.ToString(GetLVColNameFromSubItem(LVMyOpenOrders, "Buyer", LVI))


                    If LVI_Type = "SellOrder" Then

                        If RM_SmartContractRS = LVI_SmartContractRS And RM_AccountRS = LVI_SellerRS Then

                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteSmartContract)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    Else

                        If RM_SmartContractRS = LVI_SmartContractRS And RM_AccountRS = LVI_BuyerRS Then

                            Dim PayMethod As String = GetStringBetween(RelMsg.RelevantMessage, "<PayType>", "</PayType>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Method", PayMethod)

                            Dim T_Autosendinfotext As String = GetStringBetween(RelMsg.RelevantMessage, "<Autosendinfotext>", "</Autosendinfotext>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autoinfo", T_Autosendinfotext)

                            Dim T_AutocompleteSmartContract As String = GetStringBetween(RelMsg.RelevantMessage, "<AutocompleteSC>", "</AutocompleteSC>")
                            SetLVColName2SubItem(LVMyOpenOrders, LVI, "Autofinish", T_AutocompleteSmartContract)

                            RelMsg.Setted = True
                            RelevantMsgsBuffer(ii) = RelMsg

                            Exit For

                        End If

                    End If

                Next

#End Region 'Refresh LVs

            ElseIf RelMsg.RelevantMessage.Contains("<SCID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<Ask>") Then

                Dim RM_SmartContractID As ULong = GetULongBetween(RelMsg.RelevantMessage, "<SCID>", "</SCID>")
                'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
                Dim RM_AccountPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")
                Dim RM_AnswerPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<AnswerPublicKey>", "</AnswerPublicKey>")
                Dim RM_AccountPublicKeyList As List(Of String) = C_SignumAPI.RSConvert(GetAccountID(RM_AccountPublicKey))
                Dim RM_AccountID As ULong = GetULongBetweenFromList(RM_AccountPublicKeyList, "<account>", "</account>")
                Dim RM_Address As String = GetStringBetweenFromList(RM_AccountPublicKeyList, "<accountRS>", "</accountRS>")

                'Processing UnexpectedMsgs from DEXNET

                Dim RM_Ask As String = GetStringBetween(RelMsg.RelevantMessage, "<Ask>", "</Ask>")

                If RM_Ask = "AcceptOrderRequest" And Not RM_AccountID = 0 And Not RM_AccountPublicKey = "" Then

                    Dim Temp_Balance As Double = GetDoubleBetweenFromList(C_SignumAPI.GetBalance(RM_AccountID), "<balance>", "</balance>")
                    If Temp_Balance <= 1.0 Then

                        For Each LVI As ListViewItem In LVMyOpenOrders.Items
                            Dim MyOpenDEXContract As ClsDEXContract = DirectCast(LVI.Tag, ClsDEXContract)
                            Dim T_Interactions As ClsSignumInteractions = New ClsSignumInteractions(MyOpenDEXContract, PrimaryNode)

                            Dim My_OSList As List(Of ClsOrderSettings) = GetOrderSettingsFromBuffer(MyOpenDEXContract.CurrentCreationTransaction)

                            If My_OSList.Count <> 0 Then
                                Dim My_OS As ClsOrderSettings = My_OSList(0)

                                If My_OS.AutoSendInfotext Then

                                    If Not GetAutosignalTXFromINI(MyOpenDEXContract.CurrentCreationTransaction) Then

                                        If MyOpenDEXContract.ID = RM_SmartContractID Then

                                            If Not GetBlockedSmartContract(MyOpenDEXContract.ID) Then

                                                If MyOpenDEXContract.CurrentInitiatorsCollateral = 0.0 Then

                                                    Dim KnownAcc As String = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(RM_AccountID.ToString())
                                                    Dim MasterKeys As List(Of String) = GetPassPhrase()

                                                    Dim Execute As Boolean = True

                                                    If Not KnownAcc.Contains(Application.ProductName + "-error") Then
                                                        'known account
                                                        Execute = GetINISetting(E_Setting.AllowKnownAccountsAcceptOrdersOverDEXNET, False)
                                                    End If

                                                    If Execute Then
                                                        Execute = False
                                                        If MasterKeys.Count > 0 And Not RM_AccountPublicKey.Trim() = "" Then

                                                            Dim Response As String = MyOpenDEXContract.InjectResponder(MasterKeys(0), RM_AccountID, C_Fee, MasterKeys(1))
                                                            If Not IsErrorOrWarning(Response, "-error in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs2): -> " + vbCrLf, True) Then

                                                                SetBlockedSmartContract(MyOpenDEXContract.ID)

                                                                Dim PayInfo As String = GetPaymentInfoFromOrderSettings(MyOpenDEXContract.CurrentCreationTransaction, MyOpenDEXContract.CurrentBuySellAmount, MyOpenDEXContract.CurrentXAmount, MyOpenDEXContract.CurrentXItem)

                                                                If Not PayInfo.Trim() = "" Then

                                                                    If PayInfo.Contains("PayPal-E-Mail=") Then
                                                                        Dim ColWords As ClsColloquialWords = New ClsColloquialWords
                                                                        Dim ColWordsString As String = ColWords.GenerateColloquialWords(MyOpenDEXContract.CurrentCreationTransaction.ToString(), True, "-", 5)

                                                                        PayInfo += " Reference/Note=" + ColWordsString
                                                                    End If

                                                                    Dim TXr As String = T_Interactions.SendBillingInfos(RM_AccountID, PayInfo, False, True, RM_AccountPublicKey)
                                                                    Execute = Not IsErrorOrWarning(TXr, "-warning in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs4): -> " + vbCrLf, True)

                                                                End If

                                                            End If

                                                        Else
                                                            IsErrorOrWarning(Application.ProductName + "-warning in SetDEXNETRelevantMsgsToLVs(UnexpectetMsgs1): -> no Keys")
                                                        End If
                                                    End If

                                                    If Execute Then
                                                        DEXNET.BroadcastMessage("<SCID>" + MyOpenDEXContract.ID.ToString() + "</SCID><Answer>request accepted</Answer>", MasterKeys(1), MasterKeys(2), MasterKeys(0), RM_AnswerPublicKey)
                                                    Else
                                                        DEXNET.BroadcastMessage("<SCID>" + MyOpenDEXContract.ID.ToString() + "</SCID><Answer>request rejected</Answer>", MasterKeys(1), MasterKeys(2), MasterKeys(0), RM_AnswerPublicKey)
                                                    End If

                                                    RelMsg.Setted = True
                                                    RelevantMsgsBuffer(ii) = RelMsg

                                                End If

                                                Exit For
                                            End If

                                        End If

                                    End If

                                End If

                            End If

                        Next

                    End If

                ElseIf RM_Ask = "WantToBuy" And Not RM_AccountID = 0 And Not RM_AccountPublicKey = "" Then

                    Dim KnownAcc As String = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(RM_AccountID.ToString())

                    If KnownAcc.Contains(Application.ProductName + "-error") Then
                        'cant find account in blockchain

                        Dim T_Amount As Double = GetDoubleBetween(RelMsg.RelevantMessage, "<Amount>", "</Amount>")
                        Dim T_XAmount As Double = GetDoubleBetween(RelMsg.RelevantMessage, "<XAmount>", "</XAmount>")
                        Dim T_XItem As String = GetStringBetween(RelMsg.RelevantMessage, "<XItem>", "</XItem>")
                        Dim T_Method As String = GetStringBetween(RelMsg.RelevantMessage, "<Method>", "</Method>")
                        Dim T_Price As Double = T_XAmount / T_Amount

                        If CurrentMarket = T_XItem Then
                            Dim T_LVE As S_PublicOrdersListViewEntry = New S_PublicOrdersListViewEntry
                            T_LVE.Price = Dbl2LVStr(T_Price, C_Decimals)
                            T_LVE.Amount = Dbl2LVStr(T_XAmount, C_Decimals)
                            T_LVE.Total = Dbl2LVStr(T_Amount)
                            T_LVE.Collateral = Dbl2LVStr(0.0)
                            T_LVE.Method = T_Method
                            T_LVE.AutoInfo = "False"
                            T_LVE.AutoFinish = "False"
                            T_LVE.Deniability = "False"
                            T_LVE.Seller_Buyer = RM_Address
                            T_LVE.SmartContract = "OffchainOrder"
                            T_LVE.Tag = New S_OffchainBuyOrder(RM_SmartContractID.ToString, RM_AccountPublicKey, RM_Ask, T_Amount, T_XAmount, T_XItem, T_Method)

                            Dim AddingOrder As Boolean = True
                            For Each x As S_PublicOrdersListViewEntry In C_BuyOrderLVOffChainEList
                                If x.SmartContract = "OffchainOrder" And x.Seller_Buyer = RM_Address Then
                                    AddingOrder = False
                                    Exit For
                                End If
                            Next

                            If AddingOrder Then
                                C_BuyOrderLVOffChainEList.Add(T_LVE)
                            End If

                        End If

                    Else
                        Dim Temp_Balance As Double = GetDoubleBetweenFromList(C_SignumAPI.GetBalance(RM_AccountID), "<balance>", "</balance>")
                        If Temp_Balance <= 1.0 Then
                            'TODO: handle balance < 1.0 in OffchainOrders


                        End If

                    End If

                ElseIf RM_Ask = "CancelBuyOrder" And Not RM_AccountID = 0 And Not RM_AccountPublicKey = "" Then

                    Dim FoundOrder As Integer = -1
                    For i As Integer = 0 To C_BuyOrderLVOffChainEList.Count - 1
                        Dim T_Off As S_PublicOrdersListViewEntry = C_BuyOrderLVOffChainEList(i)
                        If T_Off.SmartContract = "OffchainOrder" And T_Off.Seller_Buyer = RM_Address Then
                            FoundOrder = i
                            Exit For
                        End If
                    Next

                    If FoundOrder <> -1 Then
                        C_BuyOrderLVOffChainEList.RemoveAt(FoundOrder)
                        DEXNET.DelRelevantKey("<Ask>WantToBuy</Ask>", "<PublicKey>" + RM_AccountPublicKey + "</PublicKey>")
                    End If

                Else
                    IsErrorOrWarning(Application.ProductName + "-error in SetDEXNETRelevantMsgsToLVs(UnexpectedMsgs): -> ASK=" + RM_Ask + " ACCID=" + RM_AccountID.ToString() + " Pubkey=" + RM_AccountPublicKey)
                End If

            ElseIf RelMsg.RelevantMessage.Contains("<SCID>") And RelMsg.RelevantMessage.Contains("<PublicKey>") And RelMsg.RelevantMessage.Contains("<Answer>") Then

                RelMsg.Setted = True

                Dim RM_SmartContractID As ULong = GetULongBetween(RelMsg.RelevantMessage, "<SCID>", "</SCID>")
                Dim RM_AccountPublicKey As String = GetStringBetween(RelMsg.RelevantMessage, "<PublicKey>", "</PublicKey>")
                Dim RM_Answer As String = GetStringBetween(RelMsg.RelevantMessage, "<Answer>", "</Answer>")

                StatusLabel.Text = "The Seller " + GlobalSignumPrefix + ClsReedSolomon.Encode(GetAccountID(RM_AccountPublicKey)) + " from Contract " + GlobalSignumPrefix + ClsReedSolomon.Encode(RM_SmartContractID) + " answered """ + RM_Answer + """"
                'ClsMsgs.MBox("Request answer", "The Seller " + ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(GetAccountID(RM_AccountPublicKey)) + " from Contract " + ClsSignumAPI._AddressPreFix + ClsReedSolomon.Encode(RM_SmartContractID) + " answered """ + RM_Answer + """", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
                'DEXNET.DelRelevantKey("<Answer>")

                RelevantMsgsBuffer(ii) = RelMsg

            End If


        Next

    End Sub

#End Region 'DEXNET


#Region "SmartContract + Patie Interactions"

    'Function SendBillingInfos(ByVal RecipientAddress As String, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String
    '    Dim RecipientID As ULong = ClsReedSolomon.Decode(RecipientAddress)
    '    Return SendBillingInfos(RecipientID, Message, ShowPINForm, Encrypt, RecipientPublicKey)
    'End Function

    'Function SendBillingInfos(ByVal RecipientID As ULong, ByVal Message As String, ByVal ShowPINForm As Boolean, Optional ByVal Encrypt As Boolean = True, Optional ByVal RecipientPublicKey As String = "") As String
    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)

    '    Dim Masterkeys As List(Of String) = GetPassPhrase()

    '    If Masterkeys.Count > 0 Then
    '        Dim Response As String = SignumAPI.SendMessage(Masterkeys(0), Masterkeys(2), RecipientID, Message,, Encrypt,, RecipientPublicKey)

    '        Dim JSON As ClsJSON = New ClsJSON
    '        Dim RespList As Object = JSON.JSONRecursive(Response)
    '        Dim Error0 As Object = JSON.RecursiveListSearch(DirectCast(RespList, List(Of Object)), "errorCode")

    '        If Error0.GetType.Name = GetType(Boolean).Name Then
    '            'TX OK
    '        ElseIf Error0.GetType.Name = GetType(String).Name Then
    '            Return Application.ProductName + "-error in SendBillingInfos(1): -> " + vbCrLf + Response
    '        End If


    '        If Response.Contains(Application.ProductName + "-error") Then
    '            Return Application.ProductName + "-error in SendBillingInfos(2): -> " + vbCrLf + Response
    '        Else

    '            Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
    '            Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")
    '            Dim SignumNET As ClsSignumNET = New ClsSignumNET
    '            Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, Masterkeys(1))
    '            Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

    '            If TX.Contains(Application.ProductName + "-error") Then
    '                Return Application.ProductName + "-error in SendBillingInfos(3): -> " + vbCrLf + TX
    '            Else
    '                Return TX
    '            End If

    '        End If

    '    Else

    '        Dim Returner As String = Application.ProductName + "-warning in SendBillingInfos(4): -> no Keys"

    '        If ShowPINForm Then

    '            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
    '            PinForm.ShowDialog()

    '            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then

    '                Dim Response As String = SignumAPI.SendMessage(PinForm.PublicKey, PinForm.AgreeKey, RecipientID, Message,, Encrypt,, RecipientPublicKey)

    '                If Response.Contains(Application.ProductName + "-error") Then
    '                    Return Application.ProductName + "-error in SendBillingInfos(5): -> " + vbCrLf + Response
    '                Else

    '                    Dim UTXList As List(Of String) = ClsSignumAPI.ConvertUnsignedTXToList(Response)
    '                    Dim UTX As String = GetStringBetweenFromList(UTXList, "<unsignedTransactionBytes>", "</unsignedTransactionBytes>")

    '                    Dim SignumNET As ClsSignumNET = New ClsSignumNET
    '                    Dim STX As ClsSignumNET.S_Signature = SignumNET.SignHelper(UTX, PinForm.SignKey)
    '                    Dim TX As String = SignumAPI.BroadcastTransaction(STX.SignedTransaction)

    '                    If TX.Contains(Application.ProductName + "-error") Then
    '                        Return Application.ProductName + "-error in SendBillingInfos(6): -> " + vbCrLf + TX
    '                    Else
    '                        Return TX
    '                    End If

    '                End If

    '            Else
    '                Return Returner
    '            End If

    '        Else
    '            Return Returner
    '        End If

    '    End If

    'End Function

    'Function InjectChainSwapHash(ByVal DEXContract As ClsDEXContract, ByVal ChainSwapHash As String, Optional ByVal ShowPINForm As Boolean = True) As String
    '    Dim Masterkeys As List(Of String) = GetPassPhrase()

    '    If Masterkeys.Count > 0 Then
    '        Return DEXContract.InjectChainSwapHash(GlobalPublicKey, ChainSwapHash,, Masterkeys(1))
    '    Else

    '        Dim Returner As String = Application.ProductName + "-warning in InjectChainSwapHash(Form): -> no Keys"

    '        If ShowPINForm Then

    '            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
    '            PinForm.ShowDialog()

    '            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
    '                Return DEXContract.InjectChainSwapHash(GlobalPublicKey, ChainSwapHash,, PinForm.SignKey)
    '            Else
    '                Return Returner
    '            End If

    '        Else
    '            Return Returner
    '        End If

    '    End If

    'End Function

    'Function RejectResponder(ByVal DEXContract As ClsDEXContract, Optional ByVal ShowPINForm As Boolean = True) As String

    '    Dim Masterkeys As List(Of String) = GetPassPhrase()

    '    If Masterkeys.Count > 0 Then
    '        Return DEXContract.RejectResponder(GlobalPublicKey,, Masterkeys(1))
    '    Else

    '        Dim Returner As String = Application.ProductName + "-warning in RejectResponder(Form): -> no Keys"

    '        If ShowPINForm Then

    '            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
    '            PinForm.ShowDialog()

    '            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
    '                Return DEXContract.RejectResponder(GlobalPublicKey, , PinForm.SignKey)
    '            Else
    '                Return Returner
    '            End If

    '        Else
    '            Return Returner
    '        End If

    '    End If


    'End Function

    'Function FinishWithChainSwapKey(ByVal DEXContract As ClsDEXContract, ByVal ChainSwapKey As String, Optional ByVal ShowPINForm As Boolean = True) As String

    '    Dim Masterkeys As List(Of String) = GetPassPhrase()

    '    If Masterkeys.Count > 0 Then
    '        Return DEXContract.FinishOrderWithChainSwapKey(GlobalPublicKey, ChainSwapKey,, Masterkeys(1))
    '    Else

    '        Dim Returner As String = Application.ProductName + "-warning in FinishWithChainSwapKey(Form): -> no Keys"

    '        If ShowPINForm Then

    '            Dim PinForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.SignMessage)
    '            PinForm.ShowDialog()

    '            If Not PinForm.SignKey = "" And Not PinForm.PublicKey = "" And Not PinForm.AgreeKey = "" Then
    '                Return DEXContract.FinishOrderWithChainSwapKey(GlobalPublicKey, ChainSwapKey,, PinForm.SignKey)
    '            Else
    '                Return Returner
    '            End If

    '        Else
    '            Return Returner
    '        End If

    '    End If

    'End Function

#End Region 'SmartContract + Patie Interactions


#Region "Candles"

#Region "Structures"
    Public Structure S_Candle
        Dim OpenDat As Date
        Dim CloseDat As Date

        Dim OpenValue As Double
        Dim CloseValue As Double

        Dim HighValue As Double
        Dim LowValue As Double

        Dim Volume As Double
    End Structure
    Public Structure S_Trade

        Dim OpenDat As Date
        Dim CloseDat As Date

        Dim Price As Double
        Dim Quantity As Double

    End Structure
#End Region 'Structures

    Private Function GetCandles(ByVal XItem As String, ByVal Orders As List(Of ClsDEXContract.S_Order), ByVal ChartRangeDays As Integer, Optional ByVal ResolutionMin As Integer = 1) As List(Of S_Candle)

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

    Private Function GetGriddedTime(ByVal Time As Date, ByVal TickMin As Integer) As Date

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

#End Region 'Candles

#Region "temporary info handling"
    Private Function CheckBillingInfosAlreadySend(ByVal OpenDEXContract As ClsDEXContract) As String
        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        C_SignumAPI.DEXSmartContractList = C_DEXSmartContractList

        Dim SignumNET As ClsSignumNET = New ClsSignumNET()

        Dim T_UTXList As List(Of List(Of String)) = C_SignumAPI.GetUnconfirmedTransactions()
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

                    Dim PubKey As String = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(VSAcc.ToString)

                    Dim DecryptedMsg As String = SignumNET.DecryptFrom(PubKey, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("SmartContract=") And DecryptedMsg.Contains("Transaction=") Then

                            Dim DCSmartContract As String = GetStringBetween(DecryptedMsg, "SmartContract=", " Transaction=")
                            Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "Transaction=", " ")

                            If DCSmartContract = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
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

                    Dim PubKey As String = C_SignumAPI.GetAccountPublicKeyFromAccountID_RS(VSAcc.ToString)



                    Dim DecryptedMsg As String = SignumNET.DecryptFrom(PubKey, Data, Nonce)

                    If Not MessageIsHEXString(DecryptedMsg) Then

                        If DecryptedMsg.Contains("SmartContract=") And DecryptedMsg.Contains("Transaction=") Then

                            Dim DCSmartContract As String = GetStringBetween(DecryptedMsg, "SmartContract=", " Transaction=")
                            Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "Transaction=", " ")

                            If DCSmartContract = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
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


        Dim TXList As List(Of List(Of String)) = C_SignumAPI.GetAccountTransactions(OpenDEXContract.CurrentBuyerID, OpenDEXContract.CurrentTimestamp)
        TXList.Reverse()

        For Each SearchTX As List(Of String) In TXList

            Dim TX As ULong = GetULongBetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim SenderRS As String = GetStringBetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim RecipientRS As String = GetStringBetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If SenderRS = OpenDEXContract.CurrentSellerAddress And RecipientRS = OpenDEXContract.CurrentBuyerAddress Then

                Dim DecryptedMsg As String = C_SignumAPI.ReadMessage(TX, GetAccountIDFromRS(TBSNOAddress.Text))

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("SmartContract=") And DecryptedMsg.Contains("Transaction=") Then
                        Dim DCSmartContractRS As String = GetStringBetween(DecryptedMsg, "SmartContract=", " Transaction=")
                        Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "Transaction=", " ")

                        If DCSmartContractRS = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                        Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                    ElseIf DecryptedMsg = "False" Then
                        Continue For
                    Else
                        Return DecryptedMsg
                    End If

                Else
                    Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(3): -> " + DecryptedMsg
                End If

            ElseIf SenderRS = OpenDEXContract.CurrentBuyerAddress And RecipientRS = OpenDEXContract.CurrentSellerAddress Then

                Dim DecryptedMsg As String = C_SignumAPI.ReadMessage(TX, GetAccountIDFromRS(TBSNOAddress.Text))

                If Not MessageIsHEXString(DecryptedMsg) Then

                    If DecryptedMsg.Contains("SmartContract=") And DecryptedMsg.Contains("Transaction=") Then

                        Dim DCSmartContractRS As String = GetStringBetween(DecryptedMsg, "SmartContract=", " Transaction=")
                        Dim DCTransaction As ULong = GetULongBetween(DecryptedMsg, "Transaction=", " ")

                        If DCSmartContractRS = OpenDEXContract.Address And DCTransaction = OpenDEXContract.CurrentCreationTransaction Then
                            Return DecryptedMsg
                        End If
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-error") Then
                        Return Application.ProductName + "-error in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                    ElseIf DecryptedMsg.Contains(Application.ProductName + "-warning") Then
                        Return Application.ProductName + "-warning in CheckBillingInfosAlreadySend(4): -> " + DecryptedMsg
                    ElseIf DecryptedMsg = "False" Then
                        Continue For
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

    Public Function SetAutoinfoTX2INI(ByVal TXID As ULong) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")

        If AutoinfoTXStr.Trim = "" Then
            AutoinfoTXStr = TXID.ToString + ";"
        ElseIf AutoinfoTXStr.Contains(";") Then
            AutoinfoTXStr += TXID.ToString + ";"
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return True

    End Function
    Public Function GetAutoinfoTXFromINI(ByVal TXID As ULong) As Boolean

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
    Public Function DelAutoinfoTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutoinfoTXStr As String = GetINISetting(E_Setting.AutoInfoTransactions, "")

        Dim Returner As Boolean = False

        If AutoinfoTXStr.Contains(TXID.ToString + ";") Then
            Returner = True
            AutoinfoTXStr = AutoinfoTXStr.Replace(TXID.ToString + ";", "")
        End If

        SetINISetting(E_Setting.AutoInfoTransactions, AutoinfoTXStr.Trim)

        Return Returner

    End Function


    Public Function SetAutosignalTX2INI(ByVal TXID As ULong) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")

        If AutosignalTXStr.Trim = "" Then
            AutosignalTXStr = TXID.ToString + ";"
        ElseIf AutosignalTXStr.Contains(";") Then
            AutosignalTXStr += TXID.ToString + ";"
        End If

        SetINISetting(E_Setting.AutoSignalTransactions, AutosignalTXStr.Trim)

        Return True

    End Function
    Public Function GetAutosignalTXFromINI(ByVal TXID As ULong) As Boolean

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
    Public Function DelAutosignalTXFromINI(ByVal TXID As ULong) As Boolean

        Dim AutosignalTXStr As String = GetINISetting(E_Setting.AutoSignalTransactions, "")

        Dim Returner As Boolean = False

        If AutosignalTXStr.Contains(TXID.ToString + ";") Then
            Returner = True
            AutosignalTXStr = AutosignalTXStr.Replace(TXID.ToString + ";", "")
        End If

        SetINISetting(E_Setting.AutoSignalTransactions, AutosignalTXStr.Trim)

        Return Returner

    End Function


    Public Structure S_BlockedSmartContract
        Dim SmartContract As ULong
        Dim Blockheight As ULong
        Public Sub New(Optional ByVal sc As ULong = 0UL, Optional ByVal bh As ULong = 0UL)
            SmartContract = sc
            Blockheight = bh
        End Sub
    End Structure

    Private Property BlockedSmartContracts As List(Of S_BlockedSmartContract) = New List(Of S_BlockedSmartContract)

    Private Function GetBlockedSmartContract(ByVal SmartContract As ULong) As Boolean

        Dim BSC As S_BlockedSmartContract = BlockedSmartContracts.FirstOrDefault(Function(sc As S_BlockedSmartContract) sc.SmartContract = SmartContract)
        Dim free As Boolean = BSC.SmartContract = 0UL And BSC.Blockheight = 0UL
        Return Not free

    End Function

    Private Sub SetBlockedSmartContract(ByVal SmartContract As ULong)
        BlockedSmartContracts.Add(New S_BlockedSmartContract(SmartContract, C_Block))
    End Sub

    Private Sub DelBlockedSmartContract()
        BlockedSmartContracts = BlockedSmartContracts.Where(Function(sc As S_BlockedSmartContract) sc.Blockheight >= C_Block).ToList()
    End Sub

    'Public Function IsTXBlocking(ByVal SmartContract As ULong, ByVal TransactionID As ULong) As Boolean

    '    Dim TXInINI As Boolean = GetBlockingTX(SmartContract, TransactionID)

    '    Dim T_Transaction As ClsSignumTransaction = New ClsSignumTransaction(TransactionID)

    '    If T_Transaction.Confirmations > 0 Then
    '        If TXInINI Then
    '            DelBlockingTXForSC(SmartContract, TransactionID)
    '            TXInINI = False
    '        End If
    '    Else
    '        If Not TXInINI Then
    '            SetBlockingTXForSC(SmartContract, TransactionID)
    '            TXInINI = True
    '        End If
    '    End If

    '    Return T_Transaction.Confirmations <= 0 And TXInINI

    'End Function

    'Private Function SetBlockingTXForSC(ByVal SmartContract As ULong, ByVal TransacionID As ULong) As Boolean

    '    Dim BlockingTransactions As String = GetINISetting(E_Setting.BlockingTransactions, "")

    '    If BlockingTransactions.Trim = "" Then
    '        BlockingTransactions = SmartContract.ToString() + ":" + TransacionID.ToString() + ";"
    '    ElseIf BlockingTransactions.Contains(";") Then
    '        BlockingTransactions += SmartContract.ToString() + ":" + TransacionID.ToString() + ";"
    '    End If

    '    SetINISetting(E_Setting.BlockingTransactions, BlockingTransactions.Trim)

    '    Return True

    'End Function
    'Private Function GetBlockingTX(ByVal SmartContract As ULong, ByVal TransactionID As ULong) As Boolean

    '    Dim BlockingTransactions As String = GetINISetting(E_Setting.BlockingTransactions, "")

    '    If BlockingTransactions.Contains(";") Then
    '        Dim TempList As List(Of String) = New List(Of String)(BlockingTransactions.Split(";"c))

    '        For Each TempTX As String In TempList
    '            If TempTX.Contains(":") Then
    '                Dim SCTX As List(Of String) = TempTX.Split(":"c).ToList()

    '                If SCTX(0) = SmartContract.ToString() And SCTX(1) = TransactionID.ToString() Then
    '                    Return True
    '                End If
    '            End If
    '        Next
    '    Else
    '        If Not BlockingTransactions.Trim() = "" Then
    '            If BlockingTransactions.Contains(":") Then

    '                Dim SCTX As List(Of String) = BlockingTransactions.Split(":"c).ToList()

    '                If SCTX(0) = SmartContract.ToString() And SCTX(1) = TransactionID.ToString() Then
    '                    Return True
    '                End If

    '            End If
    '        End If
    '    End If

    '    Return False

    'End Function
    'Private Function GetBlockingSCTXs() As List(Of List(Of ULong))

    '    Dim BlockingTransactions As String = GetINISetting(E_Setting.BlockingTransactions, "")

    '    Dim BlockingCSTXList As List(Of List(Of ULong)) = New List(Of List(Of ULong))

    '    If BlockingTransactions.Trim().Contains("") Then

    '    ElseIf BlockingTransactions.Contains(";") Then
    '        Dim TXStrList As List(Of String) = New List(Of String)(BlockingTransactions.Split(";"c).ToArray())
    '        For Each SCTXStr As String In TXStrList
    '            If SCTXStr.Contains(":"c) Then
    '                Dim SCTX As List(Of String) = New List(Of String)(SCTXStr.Split(":"c).ToArray())
    '                BlockingCSTXList.Add(New List(Of ULong)({Convert.ToUInt64(SCTX(0)), Convert.ToUInt64(SCTX(1))}))
    '            End If
    '        Next
    '    Else
    '        If BlockingTransactions.Contains(":"c) Then
    '            Dim SCTX As List(Of String) = New List(Of String)(BlockingTransactions.Split(":"c).ToArray())
    '            BlockingCSTXList.Add(New List(Of ULong)({Convert.ToUInt64(SCTX(0)), Convert.ToUInt64(SCTX(1))}))
    '        End If
    '    End If

    '    Return BlockingCSTXList

    'End Function


    'Public Function GetContractIDIsBlockedByTX(ByVal AccountContractID As ULong) As Boolean

    '    Dim BlockingSCTXs As List(Of List(Of ULong)) = GetBlockingSCTXs()

    '    For Each SCTX As List(Of ULong) In BlockingSCTXs
    '        If SCTX(0) = AccountContractID Then
    '            If IsTXBlocking(SCTX(0), SCTX(1)) Then
    '                Return True
    '            End If
    '        End If
    '    Next

    '    Return False

    'End Function

    'Private Function DelBlockingTXForSC(ByVal SmartContract As ULong, ByVal TransactionID As ULong) As Boolean

    '    Dim BlockingTransactions As String = GetINISetting(E_Setting.BlockingTransactions, "")

    '    Dim Returner As Boolean = False

    '    If BlockingTransactions.Contains(SmartContract.ToString() + ":" + TransactionID.ToString() + ";") Then
    '        Returner = True
    '        BlockingTransactions = BlockingTransactions.Replace(SmartContract.ToString() + ":" + TransactionID.ToString() + ";", "")
    '    End If

    '    SetINISetting(E_Setting.BlockingTransactions, BlockingTransactions.Trim)

    '    Return Returner

    'End Function

    'Private Sub ClearBlockingTXs()
    '    SetINISetting(E_Setting.BlockingTransactions, "")
    'End Sub

#End Region'temporary info handling

#Region "Tools"
    Public Shared Function Dbl2LVStr(ByVal Dbl As Double, Optional ByVal Decs As Integer = 8) As String

        Dim DecStr As String = ""
        For i As Integer = 1 To Decs
            DecStr += "0"
        Next

        Return String.Format("{0:#0." + DecStr + "}", Dbl)
    End Function
    Private Function ConvertULongMSToDate(ByVal LongS As ULong) As Date
        Dim StartDate As Date = New DateTime(2014, 8, 11, 4, 0, 0)
        StartDate = StartDate.AddSeconds(Convert.ToDouble(LongS))
        Return StartDate
    End Function

    'Function GetLastDecryptedMessageFromChat(ByVal Chats As List(Of ClsDEXContract.S_Chat), Optional ByVal Shorten As Boolean = False) As String

    '    Chats.Reverse()

    '    For Each Chat As ClsDEXContract.S_Chat In Chats

    '        If Chat.RecipientAddress = TBSNOAddress.Text Then

    '            Dim DecryptedMessage As String = GetStringBetween(Chat.Attachment, "<data>", "</data>")
    '            Dim Nonce As String = GetStringBetween(Chat.Attachment, "<nonce>", "</nonce>")

    '            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI()
    '            Dim SenderPubkey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(Chat.SenderAddress)

    '            Dim SignumNET As ClsSignumNET = New ClsSignumNET
    '            Dim Message As String = SignumNET.DecryptFrom(SenderPubkey, DecryptedMessage, Nonce)

    '            If Not IsErrorOrWarning(Message) Then

    '                If Not Message.Trim = "" Then

    '                    If Shorten Then

    '                        If Message.Contains("SmartContract=") And Message.Contains("Transaction=") Then
    '                            Dim DCSmartContract As String = GetStringBetween(Message, "SmartContract=", " Transaction=")
    '                            Dim DCTransaction As ULong = GetULongBetween(Message, "Transaction=", " ")
    '                            Dim T_SCTX As String = "SmartContract=" + DCSmartContract + " Transaction=" + DCTransaction.ToString + " "

    '                            Message = Message.Substring(Message.IndexOf(T_SCTX) + T_SCTX.Length)
    '                        End If

    '                        If Message.Contains("Infotext=") Then
    '                            Message = Message.Substring(Message.IndexOf("Infotext=") + 9)
    '                        End If

    '                        Return Message

    '                    Else
    '                        Return Message
    '                    End If

    '                End If
    '            End If

    '        End If

    '    Next

    '    Return ""

    'End Function

#End Region 'Tools

#End Region 'Methods/Functions

#Region "Multithreadings"

    Public Structure S_RelevantMsg
        Dim Setted As Boolean
        Dim RelevantMessage As String
    End Structure

    Private Sub GetNuBlock(ByVal Node As Object)
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node.ToString())
        C_NuBlock = SignumAPI.GetCurrentBlock()
    End Sub

    Private Sub BlockFeeThread(ByVal Node As Object)

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(Node.ToString())
        MultiInvoker(StatusBlockLabel, "Text", "New Blockheight: " + C_Block.ToString)

        'Fee = SignumAPI.GetTXFee
        'MultiInvoker(StatusFeeLabel, "Text", "Current Slotfee: " + String.Format("{0:#0.00000000}", Fee))

        C_UTXList = SignumAPI.C_UTXList

    End Sub

    ''' <summary>
    ''' The Thread who coordinates (loadbalance) the API Request subthreads
    ''' </summary>
    Private Sub GetThread()

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
                    IsErrorOrWarning(Application.ProductName + "-error in GetThread(While1): -> " + ex.Message)
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

                        APIRequestList(i) = Request

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

                    APIRequestList(i) = Request

                ElseIf Request.Status = "Requesting..." Then
                    'loadbalancing
                    NuNodeList.Remove(Request.Node)
                ElseIf Request.Status = "Responsed" Then

                    Request.Status = "Ready"

                    Dim founded As Boolean = False

                    For Each T_Node As String In NuNodeList
                        If T_Node = Request.Node Then
                            founded = True

                            If APIRequestList.Count < Cnt And i >= APIRequestList.Count Then
                                Continue While
                            End If

                            APIRequestList(i) = Request

                            Exit For
                        End If
                    Next

                    If Not founded Then
                        'loadbalancing
                        NuNodeList.Add(Request.Node)
                    End If

                End If

            Next

        End While

        If C_InfoOut Then
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
            IsErrorOrWarning(Application.ProductName + "-error in GetThread(): -> " + ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' The SubThread who process the request
    ''' </summary>
    ''' <param name="T_Input">The input to work with Input(0) = List-Index; Input(1) = APIRequest</param>
    Private Sub SubGetThread(ByVal T_Input As Object)

        'Try

        Dim Input As List(Of Object) = New List(Of Object)

        If T_Input.GetType.Name = GetType(List(Of Object)).Name Then
            Input = DirectCast(T_Input, List(Of Object))
        Else
            IsErrorOrWarning(Application.ProductName + "-error in SubGetThread(DirectCast): -> " + T_Input.GetType.Name)
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

        Dim T_SmartContractID As ULong = ULong.Parse(ParameterList(0))

        Select Case Command

            Case "GetDetails()"

                Dim T_Contract As ClsDEXContract

                If Request.CommandAttachment Is Nothing Then
                    T_Contract = New ClsDEXContract(Me, Request.Node, T_SmartContractID, Now.AddDays(-30))
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
                        T_HistoryOrder.SellerRS = GlobalSignumPrefix + ClsReedSolomon.Encode(T_HistoryOrder.SellerID)

                        T_HistoryOrder.BuyerID = Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "BuyerID"))
                        T_HistoryOrder.BuyerRS = GlobalSignumPrefix + ClsReedSolomon.Encode(T_HistoryOrder.BuyerID)

                        T_HistoryOrder.Amount = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Amount")))
                        T_HistoryOrder.Collateral = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Collateral")))
                        T_HistoryOrder.XAmount = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "XAmount")))
                        T_HistoryOrder.XItem = ClsDEXNET.GetXMLMessage(HistoryOrderXML, "XItem")
                        T_HistoryOrder.Price = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(ClsDEXNET.GetXMLMessage(HistoryOrderXML, "Price")))

                        T_HistoryOrder.ChainSwapKey = ClsDEXNET.GetXMLMessage(HistoryOrderXML, "ChainSwapKey")
                        'T_HistoryOrder.ChainSwapHash = ClsDEXNET.GetXMLMessage(HistoryOrderXML, "ChainSwapHash")

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

                    T_Contract = New ClsDEXContract(Me, Request.Node, T_SmartContractID, HistoryOrderList)

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

                If T_Contract.ContractOrderHistoryList.Count > 0 Then
                    T_Contract.Node = Request.Node
                    T_Contract.Refresh()
                Else
                    T_Contract = New ClsDEXContract(Me, Request.Node, T_SmartContractID, Now.AddDays(-30))
                End If


                If T_Contract.IsReady Then
                    Request.Status = "Responsed"
                Else
                    Request.Status = "Not Ready..."
                End If

                Request.Result = T_Contract

            Case Else

        End Select

        APIRequestList(Index) = Request

        'Catch ex As Exception
        ' IsErrorOrWarning(Application.ProductName + "-error in SubGetThread(M): -> " + ex.Message)
        'End Try

    End Sub

    Private Function MultithreadMonitor() As Boolean

        Try
            StatusBar.Visible = True
            StatusBar.Maximum = APIRequestList.Count

            Dim StillRun As Boolean = True
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
            IsErrorOrWarning(Application.ProductName + "-error in MultithreadMonitor(): -> " + ex.Message)
        End Try

        Return False

    End Function

#End Region 'Multithreadings

#Region "Multiinvoker"

    Public Enum E_MainFormControls
        LabDebug = 0
        StatusLabel = 1
    End Enum

    Private Delegate Sub MultiDelegate(ByVal params As List(Of Object))

    Public Sub MultiInvoker(ByVal obj As Object, ByVal prop As Object, ByVal val As Object)

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
            IsErrorOrWarning(Application.ProductName + "-error in MultiInvoker(): -> " + ex.Message)
        End Try

    End Sub

    'Sub MultiInvoker(ByVal MainFormControl As E_MainFormControls, ByVal ControlProperty As Object, ByVal Value As Object)

    '    Dim T_Control As Object = Nothing
    '    Dim FoundControl As Boolean = False

    '    Select Case MainFormControl
    '        Case E_MainFormControls.LabDebug
    '            T_Control = LabDebug
    '            FoundControl = True
    '        Case E_MainFormControls.StatusLabel
    '            T_Control = StatusLabel
    '            FoundControl = True
    '    End Select

    '    If FoundControl Then
    '        MultiInvoker(T_Control, ControlProperty, Value)
    '    End If

    'End Sub

    Private Sub Invoker(ByVal params As List(Of Object))
        SetPropertyValueByName(params.Item(0), params.Item(1).ToString, params.Item(2))
    End Sub
    Private Function SetPropertyValueByName(ControlObject As Object, PropertyName As String, value As Object) As Boolean

        Try

            Dim T_PropertyInfo As Reflection.PropertyInfo = ControlObject.GetType().GetProperty(PropertyName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            If T_PropertyInfo Is Nothing Then
                Return False
            End If

            If ControlObject.GetType.Name = GetType(ListView).Name And PropertyName = "Items" Then

                Dim T_LV As ListView = DirectCast(ControlObject, ListView)
                Dim T_Values As List(Of Object) = New List(Of Object)

                If value.GetType.Name = GetType(List(Of Object)).Name Then
                    T_Values = DirectCast(value, List(Of Object))
                Else
                    IsErrorOrWarning(Application.ProductName + "-error in SetPropertyValueByName(DirectCast): -> " + value.GetType.Name)
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

            ElseIf ControlObject.GetType.Name = GetType(ListBox).Name And PropertyName = "Items" Then

                Dim T_LB As ListBox = DirectCast(ControlObject, ListBox)
                Dim T_Values As List(Of Object) = New List(Of Object)

                If value.GetType.Name = GetType(List(Of Object)).Name Then
                    T_Values = DirectCast(value, List(Of Object))
                Else
                    IsErrorOrWarning(Application.ProductName + "-error in SetPropertyValueByName(DirectCast): -> " + value.GetType.Name)
                End If


                If T_Values(0).ToString = "Clear" Then
                    T_LB.Items.Clear()
                ElseIf T_Values(0).ToString = "Add" Then
                    Dim Val As Object = T_Values(1)
                    T_LB.Items.Add(Val)
                ElseIf T_Values(0).ToString = "Insert" Then
                    Dim Idx As Integer = Convert.ToInt32(T_Values(1))
                    Dim Val As Object = T_Values(2)
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
            IsErrorOrWarning(Application.ProductName + "-error in SetPropertyValueByName(): -> " + ex.Message)
            Return False
        End Try

    End Function

    Private Property GFXFlag As Boolean = True

    Private Sub BtChartGFXOnOff_Click(sender As Object, e As EventArgs) Handles BtChartGFXOnOff.Click

        If GFXFlag Then
            GFXFlag = False
            C_TTTL.TradeTrackTimer.Enabled = False
            SplitContainer2.Panel1.Visible = False
        Else
            GFXFlag = True
            SplitContainer2.Panel1.Visible = True
            C_TTTL.TradeTrackTimer.Enabled = True
        End If

    End Sub

#End Region 'Multiinvoker

#Region "Testsection"

    Private Sub BtBitcoinGenerateChainSwapKeyHash_Click(sender As Object, e As EventArgs) Handles BtBitcoinGenerateChainSwapKeyHash.Click
        TBBitcoinChainSwapKey.Text = ByteArrayToHEXString(RandomBytes(32))
        TBBitcoinChainSwapHash.Text = GetSHA256HashString(TBBitcoinChainSwapKey.Text).ToLower()
    End Sub

    Private Sub LVBitcoinAddresses_MouseDown(sender As Object, e As MouseEventArgs) Handles LVBitcoinAddresses.MouseDown
        LVBitcoinAddresses.ContextMenuStrip = Nothing
    End Sub

    Private Sub LVBitcoinAddresses_MouseUp(sender As Object, e As MouseEventArgs) Handles LVBitcoinAddresses.MouseUp

        LVBitcoinAddresses.ContextMenuStrip = Nothing

        If LVBitcoinAddresses.SelectedItems.Count > 0 Then

            Dim LVi As ListViewItem = LVBitcoinAddresses.SelectedItems(0)

            Dim Address As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoinAddresses, "Address", LVi))
            Dim Balance As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoinAddresses, "Balance", LVi))

            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem.Text = "copy address"
            LVCMItem.Tag = Address
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy balance"
            LVCMItem.Tag = Balance
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVBitcoinAddresses.ContextMenuStrip = LVContextMenu

        End If

    End Sub

    Private Sub LVBitcoin_MouseDown(sender As Object, e As MouseEventArgs) Handles LVBitcoin.MouseDown
        LVBitcoin.ContextMenuStrip = Nothing
    End Sub

    Private Sub LVBitcoin_MouseUp(sender As Object, e As MouseEventArgs) Handles LVBitcoin.MouseUp

        LVBitcoin.ContextMenuStrip = Nothing

        If LVBitcoin.SelectedItems.Count > 0 Then

            Dim LVi As ListViewItem = LVBitcoin.SelectedItems(0)

            Dim Confirmations As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Confirmations", LVi))
            Dim TransactionID As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Transaction ID", LVi))
            Dim OutputIDX As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Output Index", LVi))
            Dim Type As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Type", LVi))
            Dim Address As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Address", LVi))
            Dim Amount As String = Convert.ToString(GetLVColNameFromSubItem(LVBitcoin, "Amount", LVi))

            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Confirmations"
            LVCMItem.Tag = Confirmations
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Transaction ID"
            LVCMItem.Tag = TransactionID
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Output Index"
            LVCMItem.Tag = OutputIDX
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Type"
            LVCMItem.Tag = Type
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Address"
            LVCMItem.Tag = Address
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVCMItem = New ToolStripMenuItem
            LVCMItem.Text = "copy Amount"
            LVCMItem.Tag = Amount
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVBitcoin.ContextMenuStrip = LVContextMenu

        End If

    End Sub

    Private Sub Bt_BitcoinClaimTransaction_Click(sender As Object, e As EventArgs) Handles Bt_BitcoinClaimTransaction.Click

        BtSendBitcoin.Text = "Wait..."
        BtSendBitcoin.Enabled = False

        Dim Bitcoin As ClsBitcoin = New ClsBitcoin()
        Dim Transaction As ClsBitcoinTransaction = Bitcoin.ClaimBitcoinTransaction(TB_BitcoinClaimTransaction.Text, TB_BitcoinClaimUnlockingScript.Text)

        Dim TXID As String = Bitcoin.SignBitcoinTransaction(Transaction, GetBitcoinMainPrivateKey(True).ToLower())
        TXID = Bitcoin.SendRawBitcoinTransaction(TXID)

        If Not IsErrorOrWarning(TXID) Then
            ClsMsgs.MBox("Transaction successfully send." + vbCrLf + " ID = " + TXID, "Success!",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
        End If

        BtSendBitcoin.Text = "claim"
        BtSendBitcoin.Enabled = True

    End Sub

    Private Sub Bt_BitcoinClaimGetInfo_Click(sender As Object, e As EventArgs) Handles Bt_BitcoinClaimGetInfo.Click
        Dim Transaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(TB_BitcoinClaimTransaction.Text, New List(Of ClsBitcoinTransaction.S_Address))

        LV_BitcoinClaim.Items.Clear()

        For Each Inp As ClsUnspentOutput In Transaction.Inputs
            With LV_BitcoinClaim.Items.Add(Inp.InputIndex.ToString()) ' index
                .SubItems.Add(Inp.OutputType.ToString()) 'typ
                .SubItems.Add(Dbl2LVStr(Satoshi2Dbl(Inp.AmountNQT))) ' amount
                .SubItems.Add(Inp.Spendable.ToString()) 'spendable
            End With
        Next

        LV_BitcoinClaim.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

    End Sub

    Private Sub LV_BitcoinClaim_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LV_BitcoinClaim.SelectedIndexChanged

        If LV_BitcoinClaim.SelectedItems.Count > 0 Then

            Dim InputType As String = GetLVColNameFromSubItem(LV_BitcoinClaim, "Type", LV_BitcoinClaim.SelectedItems(0)).ToString()

            Select Case InputType
                Case ClsOutput.E_Type.Pay2ScriptHash.ToString()
                    Lab_BitcoinClaimChainSwapKey.Enabled = True
                    Lab_BitcoinClaimUnlockScript.Enabled = True
                    TB_BitcoinClaimUnlockingScript.Enabled = True
                    TB_BitcoinClaimChainSwapKey.Enabled = True
                Case ClsOutput.E_Type.ChainSwapHash.ToString()
                    Lab_BitcoinClaimChainSwapKey.Enabled = True
                    Lab_BitcoinClaimUnlockScript.Enabled = False
                    TB_BitcoinClaimUnlockingScript.Text = ""
                    TB_BitcoinClaimUnlockingScript.Enabled = False
                    TB_BitcoinClaimChainSwapKey.Enabled = True
                Case Else
                    Lab_BitcoinClaimChainSwapKey.Enabled = False
                    Lab_BitcoinClaimUnlockScript.Enabled = False
                    TB_BitcoinClaimUnlockingScript.Text = ""
                    TB_BitcoinClaimChainSwapKey.Text = ""
                    TB_BitcoinClaimUnlockingScript.Enabled = False
                    TB_BitcoinClaimChainSwapKey.Enabled = False
            End Select

        End If

    End Sub

#End Region 'Testsection

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

        For Each SortItem As Object In SortList

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

        For Each SortItem As Object In SortList

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

            For Each SortItem As Sortiereigenschaften In SortierList
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
