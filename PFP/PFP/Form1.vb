
'Imports System.Security.Permissions
'<PermissionSet(SecurityAction.Demand, Name:="FullTrust")> <System.Runtime.InteropServices.ComVisibleAttribute(True)>
Public Class PFPForm

    Property ReferenceMachineCode() As String = ""

    Property Block() As Integer = 0
    Property Fee() As Double = 0.0
    Property UTXList() As List(Of List(Of String))
    Property RefreshTime() As Integer = 600

    Property MarketIsCrypto() As Boolean = False
    Property Decimals() As Integer = 8

    Property AccountID() As String

    Property Boottime As Integer = 0

    Private _PaymentInfo As String = ""
    ReadOnly Property PaymentInfoJSON() As String
        Get
            If ChBxAutoSendPaymentInfo.Checked Then

                If ChBxUsePayPalSettings.Checked Then

                    If RBPayPalEMail.Checked Then
                        _PaymentInfo = ", ""ppem"":""" + TBPayPalEMail.Text.Replace(",", ";") + """"
                    ElseIf RBPayPalAccID.Checked Then
                        _PaymentInfo = ", ""ppacid"":""" + TBPayPalAccID.Text.Replace(",", ";") + """"
                    ElseIf RBPayPalOrder.Checked Then
                        _PaymentInfo = ", ""ppodr"":""" + "<PAYPALORDER>" + """"
                    End If

                Else
                    _PaymentInfo = ", ""info"":""" + TBPaymentInfo.Text.Replace(",", ";") + """"
                End If

            Else
                _PaymentInfo = ""
            End If

            Return _PaymentInfo
        End Get
        'Set(value As String)
        '    _PaymentInfo = value
        'End Set
    End Property

    Structure S_AT
        Dim AT As String
        Dim ATRS As String
        Dim IsBLS_AT As Boolean
    End Structure
    Property ATList() As List(Of S_AT) = New List(Of S_AT)

    Structure S_BLSAT
        Dim AT As String
        Dim ATRS As String
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
        Dim AT_TXList As List(Of S_BLSAT_TX)
        Dim AT_OrderList As List(Of S_Order)
        Dim Status As String
        Dim Frozen As Boolean
        Dim Running As Boolean
        Dim Stopped As Boolean
        Dim Finished As Boolean
        Dim Dead As Boolean
    End Structure
    Property BLSList() As List(Of S_BLSAT) = New List(Of S_BLSAT)

    Structure S_BLSAT_TX

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
    'Property BLSAT_TX_List() As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

    Structure S_Order
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
        Dim FirstTX As S_BLSAT_TX
        Dim LastTX As S_BLSAT_TX
    End Structure
    Property OrderList() As List(Of S_Order) = New List(Of S_Order)

    Dim SplitPanel As SplitContainer = New SplitContainer
    Dim PanelForSplitPanel As Panel = New Panel

    Dim TLS As SplitContainer = New SplitContainer
    Dim WTTL As TradeTrackerTimeLine = New TradeTrackerTimeLine

#Region "GUI Control"

#Region "General"

    Dim NoPrompt As Boolean = False
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not NoPrompt Then

            Dim Changes As Boolean = False

            If TBSNOPassPhrase.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "Passphrase", " ") Then
                Changes = True
            End If

            If CoBxMarket.SelectedItem <> INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "LastMarketViewed", "EUR") Then
                Changes = True
            End If

            If CoBxRefresh.SelectedItem <> INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "RefreshMinutes", "1") Then
                Changes = True
            End If

            If CoBxNode.SelectedItem <> INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "Node", "http://nivbox.co.uk:6876/burst") Then
                Changes = True
            End If


            If ChBxAutoSendPaymentInfo.Checked <> CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "AutoSendPaymentInfo", "False")) Then
                Changes = True
            End If

            If ChBxCheckXItemTX.Checked <> CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "AutoCheckAndFinishAT", "False")) Then
                Changes = True
            End If

            If ChBxUsePayPalSettings.Checked <> CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "UsePayPalSettings", "False")) Then
                Changes = True
            End If


            If TBPaymentInfo.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "PaymentInfo", "PaymentInfoText", "self pickup") Then
                'Changes = True
            End If


            Dim RBs As String = ""
            If RBPayPalEMail.Checked Then
                RBs = "EMail"
            ElseIf RBPayPalAccID.Checked Then
                RBs = "AccountID"
            ElseIf RBPayPalOrder.Checked Then
                RBs = "Order"
            Else
                RBs = "Order"
            End If

            If RBs <> INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalChoice", "Order") Then
                Changes = True
            End If


            If TBPayPalEMail.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalEMail", "test@test.com") Then
                Changes = True
            End If

            If TBPayPalAccID.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalID", "a1b2c3d4") Then
                Changes = True
            End If

            If TBPayPalAPIUser.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPIUser", "1234") Then
                Changes = True
            End If

            If TBPayPalAPISecret.Text <> INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPISecret", "abcd") Then
                Changes = True
            End If



            If Changes Then

                Dim Result As msgs.CustomDialogResult = msgs.MBox("There are changes in the settings, overwrite it?", "Settings changed", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                If Result = msgs.CustomDialogResult.Yes Then

                    INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "Passphrase", TBSNOPassPhrase.Text)
                    INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "LastMarketViewed", CoBxMarket.Text)
                    INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "RefreshMinutes", CoBxRefresh.Text)
                    INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "Node", CoBxNode.Text)

                    INISetValue(Application.StartupPath + "/Settings.ini", "General", "AutoSendPaymentInfo", ChBxAutoSendPaymentInfo.Checked.ToString)
                    INISetValue(Application.StartupPath + "/Settings.ini", "General", "AutoCheckAndFinishAT", ChBxCheckXItemTX.Checked.ToString)
                    'ChBxCheckItemXTX.Checked = CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "AutoCheckAndFinishAT", "False"))
                    INISetValue(Application.StartupPath + "/Settings.ini", "General", "UsePayPalSettings", ChBxUsePayPalSettings.Checked.ToString)

                    INISetValue(Application.StartupPath + "/Settings.ini", "PaymentInfo", "PaymentInfoText", TBPaymentInfo.Text)

                    INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalChoice", RBs)

                    INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalEMail", TBPayPalEMail.Text)
                    INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalID", TBPayPalAccID.Text)

                    INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPIUser", TBPayPalAPIUser.Text)
                    INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPISecret", TBPayPalAPISecret.Text)


                End If

            End If

        End If

    End Sub

    Property T_PassPhrase() As String = ""

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
        WTTL.Height = TLS.Height
        WTTL.WorkTrackTimerEnable = False

        AddHandler WTTL.TimerTick, AddressOf WorkTrackerTimeLine1_TimerTick

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

        TLS.Panel1.Controls.AddRange({LabChart, LabTick, CoBxChart, CoBxTick})
        TLS.Panel2.Controls.Add(WTTL)

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


        TBSNOPassPhrase.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "Passphrase", " ")

        If TBSNOPassPhrase.Text.Trim = "" Then

            'Using FrmManual
            '    FrmManual.ShowDialog()
            'End Using
            BlockTimer.Enabled = False

            Dim Result As FrmManual.CustomDialogResult = FrmManual.MBox()


            If T_PassPhrase.Trim = "" Then
                msgs.MBox("No PassPhrase set or unknown Address, program will close.", "Unknown Address",,, msgs.Status.Erro, 5, msgs.Timer_Art.AutoOK)

                NoPrompt = True
                Application.Exit()
                Exit Sub
            Else
                BlockTimer.Enabled = True
            End If

            TBSNOPassPhrase.Text = T_PassPhrase

        End If

        CoBxMarket.SelectedItem = INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "LastMarketViewed", "EUR")
        CoBxRefresh.SelectedItem = INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "RefreshMinutes", "1")
        CoBxNode.SelectedItem = INIGetValue(Application.StartupPath + "/Settings.ini", "Basic", "Node", "http://nivbox.co.uk:6876/burst")

        If CoBxNode.Text.Trim = "" Then
            CoBxNode.SelectedItem = CoBxNode.Items(0)
        End If

        RefreshTime = CInt(CoBxRefresh.Text) * 600

        ChBxAutoSendPaymentInfo.Checked = CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "AutoSendPaymentInfo", "False"))
        ChBxCheckXItemTX.Checked = CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "AutoCheckAndFinishAT", "False"))
        ChBxUsePayPalSettings.Checked = CBool(INIGetValue(Application.StartupPath + "/Settings.ini", "General", "UsePayPalSettings", "False"))


        TBPaymentInfo.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "PaymentInfo", "PaymentInfoText", "self pickup")

        Dim RBs As String = INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalChoice", "EMail")

        If RBs.Trim = "EMail" Then
            RBPayPalEMail.Checked = True
        ElseIf RBs.Trim = "AccountID" Then
            RBPayPalAccID.Checked = True
        ElseIf RBs.Trim = "Order" Then
            RBPayPalOrder.Checked = True
        Else
            RBPayPalEMail.Checked = True
        End If


        TBPayPalEMail.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalEMail", "test@test.com")
        TBPayPalAccID.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalID", "a1b2c3d4")

        TBPayPalAPIUser.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPIUser", "1234")
        TBPayPalAPISecret.Text = INIGetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPISecret", "abcd")



        'BtCheckAddress.PerformClick()

        If TBSNOPassPhrase.Text.Trim = "" Then
            BlockTimer.Enabled = False
            Exit Sub
        End If

        'If RBPayPalOrder.Checked Then
        'Test PayPal Business API
        If Not CheckPayPalAPI() = "True" Then
            RBPayPalEMail.Checked = True
            RBPayPalOrder.Checked = False
            RBPayPalOrder.Enabled = False
        End If

        'End If


        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}

        ReferenceMachineCode = BCR.C_ReferenceMachineCode

        ATList = GetATsFromCSV()

        BlockTimer_Tick(90, Nothing)

    End Sub

    Private Sub WorkTrackerTimeLine1_TimerTick(sender As Object)

        For Each TTSlot As TradeTrackerSlot In PanelForSplitPanel.Controls

            Dim TempSC As SplitContainer = CType(SplitPanel.Panel1.Controls(0), SplitContainer)
            Dim TempTimeLine As TradeTrackerTimeLine = CType(TempSC.Panel2.Controls(0), TradeTrackerTimeLine)

            TLS.SplitterDistance = TTSlot.SplitterDistance

            TTSlot.Dock = DockStyle.Fill
            WTTL.Dock = DockStyle.Fill

            TTSlot.Chart_EMA_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.Chart_EMA_EndDate = TempTimeLine.SkalaEndDate

            TTSlot.MACD_RSI_TR_StartDate = TempTimeLine.SkalaStartDate
            TTSlot.MACD_RSI_TR_EndDate = TempTimeLine.SkalaEndDate

        Next

    End Sub

    Private Sub BlockTimer_Tick(sender As Object, e As EventArgs) Handles BlockTimer.Tick

        If sender.GetType.Name = GetType(Integer).Name Then
            Boottime = RefreshTime - (RefreshTime / sender)
        End If

        Boottime += 1

        StatusBar.Visible = True
        StatusBar.Maximum = RefreshTime
        StatusBar.Value = RefreshTime - Boottime

        If Boottime >= RefreshTime Then

            Boottime = 0
            BlockTimer.Enabled = False
            WTTL.WorkTrackTimer.Enabled = False
            SplitContainer2.Panel1.Visible = False

            Dim Wait As Boolean = Loading()
            RefreshTime = CInt(CoBxRefresh.Text) * 600

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
            ViewThread.Start(New List(Of Object)({CoBxChartVal, CoBxTickVal, CoBxMarket.Text, CoBxNode.Text}))

            SplitContainer2.Panel1.Visible = True
            WTTL.WorkTrackTimer.Enabled = True
            BlockTimer.Enabled = True
        End If

    End Sub

    Private Sub Form1_ResizeBegin(sender As Object, e As EventArgs) Handles MyBase.ResizeBegin
        SplitContainer12.Visible = False
    End Sub
    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        SplitContainer12.Visible = True
    End Sub


    Private Sub CoBxMarket_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxMarket.SelectedIndexChanged, CoBxMarket.DropDownClosed
        BlockTimer_Tick(90, Nothing)
    End Sub
    Private Sub TBSNOPassPhrase_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBSNOPassPhrase.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Select Case keys
            'Case 48 To 57, 44, 8
                ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                BtCheckAddress.PerformClick()
            Case Else
                ' alle anderen Eingaben unterdrücken
                'e.Handled = True
        End Select

    End Sub
    Private Sub BtCheckAddress_Click(sender As Object, e As EventArgs) Handles BtCheckAddress.Click

        'Dim BCR As ClsBurstAPI = New ClsBurstAPI With {.C_PassPhrase = TBSNOPassPhrase.Text}

        'Dim x As List(Of String) = BCR.GetAccountFromPassPhrase()

        'TBSNOAddress.Text = BCR.BetweenFromList(x, "<address>", "</address>")
        'TBSNOBalance.Text = BCR.BetweenFromList(x, "<available>", "</available>")
        'AccountID = BCR.BetweenFromList(x, "<account>", "</account>")

        CoBxMarket_SelectedIndexChanged(Nothing, Nothing)

    End Sub

#End Region

#Region "Marketdetails - Controls"

    Private Sub BtCreateNewAT_Click(sender As Object, e As EventArgs) Handles BtCreateNewAT.Click

        Dim BSR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text)

        Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to create a new Payment channel" + vbCrLf + "(AT=Automated Transaction)?", "Create AT", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

        If MsgResult = msgs.CustomDialogResult.Yes Then

            Dim NuList As List(Of String) = BSR.CreateAT()

            Dim NuATList As List(Of S_AT) = New List(Of S_AT)

            Dim NuAT As S_AT = New S_AT

            NuAT.AT = BSR.BetweenFromList(NuList, "<transaction>", "</transaction>")

            Dim AccRS As List(Of String) = BSR.RSConvert(NuAT.AT)
            NuAT.ATRS = BSR.BetweenFromList(AccRS, "<accountRS>", "</accountRS>")
            NuAT.IsBLS_AT = True

            NuATList.Add(NuAT)

            SaveATsToCSV(NuATList)

            msgs.MBox("New AT Created" + vbCrLf + vbCrLf + "TX: " + NuAT.AT, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)

        End If

    End Sub

    Private Sub RBSNOSell_CheckedChanged(sender As Object, e As EventArgs) Handles RBSNOSell.CheckedChanged, RBSNOBuy.CheckedChanged

        If RBSNOSell.Checked Then
            LabWTX.Text = "WantToSell:"
        ElseIf RBSNOBuy.Checked Then
            LabWTX.Text = "WantToBuy:"
        End If

    End Sub
    Private Sub BtSNOSetOrder_Click(sender As Object, e As EventArgs) Handles BtSNOSetOrder.Click

        Dim BSR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text,, AccountID) ' With {.C_Node = CoBxNode.Text, .C_AccountID = AccountID}

        Try

            Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
            Dim x As List(Of String) = BCR.GetAccountFromPassPhrase()

            TBSNOAddress.Text = BCR.BetweenFromList(x, "<address>", "</address>")
            TBSNOBalance.Text = BCR.BetweenFromList(x, "<available>", "</available>")
            AccountID = BCR.BetweenFromList(x, "<account>", "</account>")


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
                    msgs.MBox("Minimum for one Burst must be greater than " + Dbl2LVStr(MinXAmount, Decimals) + " " + CoBxMarket.Text + "!", "Value too low",,, msgs.Status.Erro)

                    Exit Sub
                End If

            Else
                msgs.MBox("Minimum values must be greater than 0.0!", "Value too low",,, msgs.Status.Erro)
                Exit Sub
            End If


            Dim AccAmount As Double = BSR.BetweenFromList(BSR.GetBalance(TBSNOAddress.Text), "<available>", "</available>")

            If LVOpenChannels.Items.Count > 0 Then

                Dim BLS As S_BLSAT = Nothing ' New S_BLSAT ' DirectCast(LVOpenChannels.Items(0).Tag, S_BLSAT)

                For Each LVi As ListViewItem In LVOpenChannels.Items

                    If Not LVi.BackColor = Color.Crimson Then
                        BLS = DirectCast(LVi.Tag, S_BLSAT)

                        If CheckForUTX(, BLS.ATRS) Or CheckATforTX(BLS.ATRS) Then
                            BLS = Nothing ' New S_BLSAT
                        Else
                            Exit For
                        End If

                    End If

                Next

                If IsNothing(BLS.ATRS) Then
                    msgs.MBox("All Payment Channels are in Use.", "No free Payment Channel found",,, msgs.Status.Information)
                    Exit Sub
                End If


                Dim Recipient As String = BLS.AT  ' GetLVColName2SubItem(LVOpenChannels, "SmartContract", LVOpenChannels.Items(0))
                Dim Amount As Double = CDbl(NUDSNOAmount.Value)
                Dim Fee As Double = CDbl(NUDSNOTXFee.Value)
                Dim Collateral As Double = CDbl(NUDSNOCollateral.Value)
                Dim Item As String = CoBxMarket.Text

                If ChBxAutoSendPaymentInfo.Checked Then
                    'autosignal

                    If ChBxUsePayPalSettings.Checked And RBPayPalOrder.Checked Then
                        Item += "-PB"
                    ElseIf ChBxUsePayPalSettings.Checked And (RBPayPalEMail.Checked Or RBPayPalAccID.Checked) Then
                        Item += "-PA"
                    Else

                    End If

                Else
                    'no autosignal

                End If


                Dim ItemAmount As Double = CDbl(NUDSNOItemAmount.Value)

                Dim PassPhrase As String = TBSNOPassPhrase.Text

                BSR.C_PassPhrase = PassPhrase

                If RBSNOSell.Checked Then

                    If AccAmount > Amount + Fee + Collateral Then
                        'enough balance

                        Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to create a new SellOrder?", "Create SellOrder", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                        If MsgResult = msgs.CustomDialogResult.Yes Then
                            Dim TX As String = BSR.SetBLSATSellOrder(Recipient, Amount + Collateral + 1.0, Collateral, Item, ItemAmount, Fee)
                            msgs.MBox("SellOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, msgs.Status.Information)
                        End If

                    Else
                        'not enough balance
                        msgs.MBox("not enough balance", "Error",,, msgs.Status.Erro)
                    End If

                ElseIf RBSNOBuy.Checked Then

                    If AccAmount > Collateral + Fee Then
                        'enough balance

                        Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to create a new BuyOrder?", "Create BuyOrder", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                        If MsgResult = msgs.CustomDialogResult.Yes Then
                            Dim TX As String = BSR.SetBLSATBuyOrder(Recipient, Amount, Collateral + 1.0, Item, ItemAmount, Fee)
                            msgs.MBox("BuyOrder Created" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, msgs.Status.Information)
                        End If

                    Else
                        'not enough balance
                        msgs.MBox("not enough balance", "Error",,, msgs.Status.Erro)
                    End If

                End If

            End If

        Catch ex As Exception
            msgs.MBox(ex.Message, "Error",,, msgs.Status.Erro)
        End Try

    End Sub

    Private Sub BtSNOSetCurFee_Click(sender As Object, e As EventArgs) Handles BtSNOSetCurFee.Click
        NUDSNOTXFee.Value = CDec(Fee)
    End Sub



    Private Sub LVSellorders_MouseDown(sender As Object, e As MouseEventArgs) Handles LVSellorders.MouseDown
        'BtBuy.Text = "Buy"
    End Sub
    Private Sub LVSellorders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVSellorders.MouseUp
        BtBuy.Text = "Buy"
        If LVSellorders.SelectedItems.Count > 0 Then

            Dim Order As S_BLSAT = DirectCast(LVSellorders.SelectedItems(0).Tag, S_BLSAT)

            If Order.Initiator = TBSNOAddress.Text Then
                BtBuy.Text = "cancel"
            Else
                BtBuy.Text = "Buy"
            End If

        End If
    End Sub

    'Private Sub LVSellorders_Click(sender As Object, e As EventArgs) Handles LVSellorders.Click

    '    If LVSellorders.SelectedItems.Count > 0 Then

    '        Dim Order As S_BLSAT = DirectCast(LVSellorders.SelectedItems(0).Tag, S_BLSAT)

    '        If Order.Initiator = TBSNOAddress.Text Then
    '            BtBuy.Text = "cancel"
    '        Else
    '            BtBuy.Text = "Buy"
    '        End If

    '    End If

    'End Sub

    Private Sub LVBuyorders_MouseDown(sender As Object, e As MouseEventArgs) Handles LVBuyorders.MouseDown
        'BtSell.Text = "Sell"
    End Sub
    Private Sub LVBuyorders_MouseUp(sender As Object, e As MouseEventArgs) Handles LVBuyorders.MouseUp
        BtSell.Text = "Sell"
        If LVBuyorders.SelectedItems.Count > 0 Then

            Dim Order As S_BLSAT = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_BLSAT)

            If Order.Initiator = TBSNOAddress.Text Then
                BtSell.Text = "cancel"
            Else
                BtSell.Text = "Sell"
            End If

        End If
    End Sub
    'Private Sub LVBuyorders_Click(sender As Object, e As EventArgs) Handles LVBuyorders.Click

    '    If LVBuyorders.SelectedItems.Count > 0 Then

    '        Dim Order As S_BLSAT = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_BLSAT)

    '        If Order.Initiator = TBSNOAddress.Text Then
    '            BtSell.Text = "cancel"
    '        Else
    '            BtSell.Text = "Sell"
    '        End If

    '    End If

    'End Sub

    Private Sub BtBuy_Click(sender As Object, e As EventArgs) Handles BtBuy.Click

        If LVSellorders.SelectedItems.Count > 0 Then

            Dim BLS As S_BLSAT = DirectCast(LVSellorders.SelectedItems(0).Tag, S_BLSAT)

            If CheckForUTX("", BLS.ATRS) Then
                msgs.MBox("One TX is already Pending for this Order", "Order not available",,, msgs.Status.Attention, 5, msgs.Timer_Art.AutoOK)
                Exit Sub
            End If

            Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
            Dim BLSAT_TX As S_BLSAT_TX = GetLastTXWithValues(BLS.AT_TXList, CoBxMarket.Text)
            Dim Amount As Double = BCR.Planck2Dbl(CULng(BCR.Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))


            If Not BLS.Initiator = TBSNOAddress.Text Then
                Dim BCR2 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

                'AccountID = BCR2.BetweenFromList(x, "<account>", "</account>")
                'TBSNOAddress.Text = BCR2.BetweenFromList(x, "<address>", "</address>")

                Dim Available As Double = 0.0
                Dim AvaStr As String = BCR2.BetweenFromList(x, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Amount + 1.0 Then

                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to Buy " + Dbl2LVStr(BLS.BuySellAmount) + " BURST for " + Dbl2LVStr(BCR.Planck2Dbl(BLS.XAmount), Decimals) + " " + BLS.XItem + "?" + vbCrLf + vbCrLf + "collateral: " + Dbl2LVStr(BLS.InitiatorsCollateral) + " BURST" + vbCrLf + "handling fees: " + Dbl2LVStr(1.0) + " BURST", "Buy Order?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then
                        Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, Amount + 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))
                        msgs.MBox("SellOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
                    End If

                Else
                    'not enough balance
                    msgs.MBox("not enough balance", "Error",,, msgs.Status.Erro)
                End If
            Else

                Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to cancel the SellOrder?", "Cancel SellOrder?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                If MsgResult = msgs.CustomDialogResult.Yes Then
                    Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))
                    msgs.MBox("SellOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)

                End If

            End If



        End If

    End Sub
    Private Sub BtSell_Click(sender As Object, e As EventArgs) Handles BtSell.Click

        If LVBuyorders.SelectedItems.Count > 0 Then

            Dim BLS As S_BLSAT = DirectCast(LVBuyorders.SelectedItems(0).Tag, S_BLSAT)

            If CheckForUTX(, BLS.ATRS) Then
                msgs.MBox("One TX is already Pending for this Order", "Order not available",,, msgs.Status.Attention, 5, msgs.Timer_Art.AutoOK)
                Exit Sub
            End If

            Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
            Dim BLSAT_TX As S_BLSAT_TX = GetLastTXWithValues(BLS.AT_TXList, CoBxMarket.Text)
            Dim Amount As Double = BCR.Planck2Dbl(CULng(BCR.Between(BLSAT_TX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))
            Dim XItem As String = BCR.Between(BLSAT_TX.Attachment, "<xItem>", "</xItem>", GetType(String))
            Dim XAmount As Double = BCR.Planck2Dbl(CULng(BCR.Between(BLSAT_TX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
            Dim Sum As Double = Amount + BCR.Planck2Dbl(CULng(BLSAT_TX.AmountNQT))

            If Not BLS.Initiator = TBSNOAddress.Text Then

                Dim BCR2 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

                'AccountID = BCR2.BetweenFromList(x, "<account>", "</account>")
                'TBSNOAddress.Text = BCR2.BetweenFromList(x, "<address>", "</address>")

                Dim Available As Double = 0.0
                Dim AvaStr As String = BCR2.BetweenFromList(x, "<available>", "</available>")

                If AvaStr.Trim = "" Then

                Else
                    Available = Val(AvaStr.Replace(",", "."))
                End If

                If Available > Amount + 1.0 Then

                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to Sell " + Dbl2LVStr(BLS.BuySellAmount) + " BURST for " + Dbl2LVStr(BCR.Planck2Dbl(BLS.XAmount), Decimals) + " " + BLS.XItem + "?" + vbCrLf + vbCrLf + "collateral: " + Dbl2LVStr(BLS.InitiatorsCollateral) + " BURST" + vbCrLf + "handling fees: " + Dbl2LVStr(1.0) + " BURST", "Sell Order?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then

                        Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, Sum + 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                        If TX.Trim = "error" Then

                        Else

                            If PaymentInfoJSON.Trim = "" Then

                            Else

                                Dim T_PaymentInfoJSON As String = PaymentInfoJSON

                                If T_PaymentInfoJSON.Trim.Contains("ppodr") Then

                                    Dim PPAPI As ClsPayPal = New ClsPayPal
                                    PPAPI.Client_ID = TBPayPalAPIUser.Text
                                    PPAPI.Secret = TBPayPalAPISecret.Text

                                    Dim PPOrderID As List(Of String) = PPAPI.CreateOrder("Burst", Amount, XAmount, "EUR")
                                    T_PaymentInfoJSON = T_PaymentInfoJSON.Replace("<PAYPALORDER>", PPAPI.BetweenFromList(PPOrderID, "<id>", "</id>"))

                                End If

                                Dim T_MsgStr As String = "{""at"":""" + BLS.AT + """, ""tx"":""" + BLSAT_TX.Transaction + """" + T_PaymentInfoJSON + "}"
                                Dim TXr As String = SendBillingInfos(BLSAT_TX.Sender, T_MsgStr)

                            End If

                            msgs.MBox("BuyOrder Accepted" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)

                        End If

                    End If

                Else
                    'not enough balance
                    msgs.MBox("not enough balance", "Error",,, msgs.Status.Erro)
                End If

            Else

                Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to cancel the BuyOrder?", "Cancel BuyOrder?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                If MsgResult = msgs.CustomDialogResult.Yes Then
                    Dim TX As String = BCR.SendMessage2BLSAT(BLS.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))
                    msgs.MBox("BuyOrder canceled" + vbCrLf + vbCrLf + "TX: " + TX, "Transaction created", ,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)

                End If

            End If

        End If

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
    'Private Sub LVMyOpenOrders_Click(sender As Object, e As EventArgs) Handles LVMyOpenOrders.Click


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


    'End Sub
    Private Sub Copy2CB(sender As Object, e As EventArgs)

        Try
            If Not IsNothing(sender.tag) Then
                Clipboard.SetText(sender.tag.ToString)
            Else

            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub BtExecuteOrder_Click(sender As Object, e As EventArgs) Handles BtExecuteOrder.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim TX As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            If CheckForUTX(, TX.ATRS) Or CheckATforTX(TX.ATRS) Then
                msgs.MBox("One TX is already Pending for this Order", "Order not available",,, msgs.Status.Attention, 5, msgs.Timer_Art.AutoOK)
                Exit Sub
            End If


            If TX.Type = "SellOrder" Then

                If TX.Buyer.Trim = "" Then
                    'cancel AT
                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to cancel the AT?", "cancel AT?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))
                        msgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
                    End If

                Else
                    'execute AT
                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to execute the AT?", "execute AT?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceFinishOrder}))
                        msgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
                    End If

                End If

            Else

                If TX.Seller.Trim = "" Then
                    'cancel AT
                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to cancel the AT?", "cancel AT?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceAcceptOrder}))

                        msgs.MBox("Order Canceled" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
                    End If

                Else
                    'execute AT
                    Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Do you really want to execute the AT?", "execute AT?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

                    If MsgResult = msgs.CustomDialogResult.Yes Then
                        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                        Dim TXStr As String = BCR.SendMessage2BLSAT(TX.AT, 1.0, New List(Of ULong)({BCR.ReferenceFinishOrder}))
                        msgs.MBox("Order Finished" + vbCrLf + vbCrLf + "TX: " + TXStr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
                    End If

                End If

            End If

        End If



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

            Dim BLP_Order As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            If CheckForUTX(, BLP_Order.ATRS) Then
                msgs.MBox("One TX is already Pending for this Order", "Order not available",,, msgs.Status.Attention, 5, msgs.Timer_Art.AutoOK)
                Exit Sub
            End If

            Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}


            Dim Amount As Double = BCR.Planck2Dbl(CULng(BCR.Between(BLP_Order.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))
            Dim XItem As String = BCR.Between(BLP_Order.Attachment, "<xItem>", "</xItem>", GetType(String))
            Dim XAmount As Double = BCR.Planck2Dbl(CULng(BCR.Between(BLP_Order.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
            Dim Sum As Double = Amount + BCR.Planck2Dbl(CULng(BLP_Order.FirstTX.AmountNQT))


            If PaymentInfoJSON.Trim = "" Then

            Else

                Dim T_PaymentInfoJSON As String = PaymentInfoJSON

                If Not T_PaymentInfoJSON.Trim = "" Then

                    Dim PPAPI As ClsPayPal = New ClsPayPal
                    PPAPI.Client_ID = TBPayPalAPIUser.Text
                    PPAPI.Secret = TBPayPalAPISecret.Text


                    Dim PPOrderID As List(Of String) = PPAPI.CreateOrder("Burst", Amount, XAmount, "EUR")
                    T_PaymentInfoJSON = T_PaymentInfoJSON.Replace("<PAYPALORDER>", PPAPI.BetweenFromList(PPOrderID, "<id>", "</id>"))

                    Dim T_MsgStr As String = "{""at"":""" + BLP_Order.AT + """, ""tx"":""" + BLP_Order.FirstTX.Transaction + """" + T_PaymentInfoJSON + "}"
                    Dim TXr As String = SendBillingInfos(BLP_Order.FirstTX.Sender, T_MsgStr)

                    msgs.MBox("New PayPal Order sended as encrypted Message", "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)

                End If

            End If

        End If

    End Sub
    Private Sub BtSendMsg_Click(sender As Object, e As EventArgs) Handles BtSendMsg.Click

        If LVMyOpenOrders.SelectedItems.Count > 0 Then

            Dim Order As S_Order = DirectCast(LVMyOpenOrders.SelectedItems(0).Tag, S_Order)

            Dim T_PaymentInfoJSON As String = ", ""info"":""" + TBManuMsg.Text.Replace(",", ";").Replace(":", ";").Replace("""", ";") + """" 'no commas, no colon and no quotation marks!

            Dim T_MsgStr As String = "{""at"":""" + Order.AT + """, ""tx"":""" + Order.FirstTransaction + """" + T_PaymentInfoJSON + "}"

            Dim Recipient As String = Order.Buyer

            If Order.Buyer = TBSNOAddress.Text Then
                Recipient = Order.Seller
            End If

            Dim TXr As String = SendBillingInfos(Recipient, T_MsgStr, ChBxEncMsg.Checked)

            If ChBxEncMsg.Checked Then
                msgs.MBox("encrypted Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
            Else
                msgs.MBox("public Message sended" + vbCrLf + vbCrLf + "TX: " + TXr, "Transaction created",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
            End If

            TBManuMsg.Text = ""

        End If

    End Sub

#End Region

#Region "Settings - Controls"
    Dim OldPaymentinfoText As String = Nothing

    Private Sub ChBxAutoSendPaymentInfo_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxAutoSendPaymentInfo.CheckedChanged

        If ChBxAutoSendPaymentInfo.Checked Then
            LabPaymentInfo.Enabled = True
            TBPaymentInfo.Enabled = True
            ChBxUsePayPalSettings.Enabled = True
        Else
            LabPaymentInfo.Enabled = False
            TBPaymentInfo.Enabled = False
            ChBxUsePayPalSettings.Enabled = False
        End If

    End Sub

    Private Sub ChBxUsePayPalSettings_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxUsePayPalSettings.CheckedChanged

        If ChBxUsePayPalSettings.Checked Then
            LabPaymentInfo.Enabled = False
            TBPaymentInfo.Enabled = False
            OldPaymentinfoText = TBPaymentInfo.Text
        Else
            LabPaymentInfo.Enabled = True
            TBPaymentInfo.Enabled = True
        End If

        RBPayPalEMail_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub RBPayPalEMail_CheckedChanged(sender As Object, e As EventArgs) Handles RBPayPalEMail.CheckedChanged, RBPayPalAccID.CheckedChanged, RBPayPalOrder.CheckedChanged

        Dim PaymentInfo As String = ""

        If RBPayPalEMail.Checked Then

            TBPayPalEMail.Enabled = True
            TBPayPalAccID.Enabled = False
            PaymentInfo = TBPayPalEMail.Text

        ElseIf RBPayPalAccID.Checked Then

            TBPayPalEMail.Enabled = False
            TBPayPalAccID.Enabled = True
            PaymentInfo = TBPayPalAccID.Text

        ElseIf RBPayPalOrder.Checked Then

            TBPayPalEMail.Enabled = False
            TBPayPalAccID.Enabled = False
            PaymentInfo = ""

        End If


        If ChBxUsePayPalSettings.Checked Then
            TBPaymentInfo.Text = PaymentInfo
        Else
            If Not IsNothing(OldPaymentinfoText) Then
                TBPaymentInfo.Text = OldPaymentinfoText
            End If
        End If

    End Sub

    Private Sub TBPayPalEMail_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBPayPalEMail.KeyPress, TBPayPalAccID.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Dim TBx As TextBox = DirectCast(sender, TextBox)

        TBx.BackColor = Color.Yellow

        Select Case keys
            'Case 48 To 57, 44, 8
                ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                TBx.BackColor = SystemColors.Window
                RBPayPalEMail_CheckedChanged(Nothing, Nothing)

            Case Else
                ' alle anderen Eingaben unterdrücken
                'e.Handled = True
        End Select

    End Sub

    Private Sub BtCheckPayPalBiz_Click(sender As Object, e As EventArgs) Handles BtCheckPayPalBiz.Click

        Dim Status As String = CheckPayPalAPI()

        If Status = "True" Then
            RBPayPalOrder.Enabled = True
            msgs.MBox("PayPal Business OK", "Checking PayPal-API",,, msgs.Status.Information, 5, msgs.Timer_Art.AutoOK)
        Else
            RBPayPalEMail.Checked = True
            RBPayPalOrder.Checked = False
            RBPayPalOrder.Enabled = False

            msgs.MBox(Status, "Fail",,, msgs.Status.Erro, 5, msgs.Timer_Art.AutoOK)

        End If

    End Sub

    Private Sub BtSaveSettings_Click(sender As Object, e As EventArgs) Handles BtSaveSettings.Click

        INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "Passphrase", TBSNOPassPhrase.Text)
        INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "LastMarketViewed", CoBxMarket.Text)
        INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "RefreshMinutes", CoBxRefresh.Text)
        INISetValue(Application.StartupPath + "/Settings.ini", "Basic", "Node", CoBxNode.Text)


        INISetValue(Application.StartupPath + "/Settings.ini", "General", "AutoSendPaymentInfo", ChBxAutoSendPaymentInfo.Checked.ToString)
        INISetValue(Application.StartupPath + "/Settings.ini", "General", "AutoCheckAndFinishAT", ChBxCheckXItemTX.Checked.ToString)
        INISetValue(Application.StartupPath + "/Settings.ini", "General", "UsePayPalSettings", ChBxUsePayPalSettings.Checked.ToString)

        INISetValue(Application.StartupPath + "/Settings.ini", "PaymentInfo", "PaymentInfoText", TBPaymentInfo.Text)

        Dim RBs As String = ""
        If RBPayPalEMail.Checked Then
            RBs = "EMail"
        ElseIf RBPayPalAccID.Checked Then
            RBs = "AccountID"
        ElseIf RBPayPalOrder.Checked Then
            RBs = "Order"
        Else
            RBs = "Order"
        End If

        INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalChoice", RBs)

        INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalEMail", TBPayPalEMail.Text)
        INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalID", TBPayPalAccID.Text)

        INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPIUser", TBPayPalAPIUser.Text)
        INISetValue(Application.StartupPath + "/Settings.ini", "PayPal", "PayPalAPISecret", TBPayPalAPISecret.Text)

    End Sub

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

    Function Loading() As Boolean

        Dim BCR2 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}

        Dim x As List(Of String) = BCR2.GetAccountFromPassPhrase()

        TBSNOAddress.Text = BCR2.BetweenFromList(x, "<address>", "</address>")
        TBSNOBalance.Text = BCR2.BetweenFromList(x, "<available>", "</available>")
        AccountID = BCR2.BetweenFromList(x, "<account>", "</account>")

        LabXItem.Text = CoBxMarket.Text
        LabXitemAmount.Text = CoBxMarket.Text + " Amount: "

        MarketIsCrypto = GetMarketCurrencyIsCrypto(CoBxMarket.Text)
        Decimals = GetCurrencyDecimals(CoBxMarket.Text)

        NUDSNOItemAmount.DecimalPlaces = Decimals


        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}

        StatusBar.Visible = True

        SplitContainer12.Enabled = False

        Dim ATStrList As List(Of String) = BCR.GetATIds()

        StatusBar.Value = 0
        StatusBar.Maximum = ATStrList.Count


        For i As Integer = 0 To ATList.Count - 1
            Dim SAT As S_AT = ATList(i)

            If SAT.IsBLS_AT Then

                Dim ATDetails = BCR.GetATDetails(SAT.AT)
                Dim MachineCode As String = BCR.BetweenFromList(ATDetails, "<machineCode>", "</machineCode>")

                Application.DoEvents()

                If ReferenceMachineCode.Trim = MachineCode.Trim Then
                    SAT.IsBLS_AT = True
                Else
                    SAT.IsBLS_AT = False
                End If

                ATList(i) = SAT

            End If

        Next



        Dim ATFilterList As List(Of String) = New List(Of String)
        For i As Integer = 0 To ATStrList.Count - 1
            Dim ATid As String = ATStrList(i)

            StatusBar.Value = i

            StatusLabel.Text = "Filter AT(" + i.ToString + "/" + ATStrList.Count.ToString + "): " + ATid

            Dim ContinueFor As Boolean = False
            For Each SAT As S_AT In ATList

                If SAT.AT = ATid Then
                    ContinueFor = True
                    Exit For
                End If

            Next

            If ContinueFor Then Continue For

            ATFilterList.Add(ATid)
        Next

        StatusBar.Value = 0
        StatusBar.Maximum = ATFilterList.Count

        Dim ThreadPool As List(Of Threading.Thread) = New List(Of Threading.Thread)
        For i As Integer = 0 To ATFilterList.Count - 1
            Dim ATid As String = ATFilterList(i)
            StatusBar.Value = i

            StatusLabel.Text = "Checking AT(" + i.ToString + "/" + ATFilterList.Count.ToString + "): " + ATid
            Application.DoEvents()

            Dim CheckThread As Threading.Thread = New Threading.Thread(AddressOf ATCheckThread)

            ThreadPool.Add(CheckThread)
            ThreadPool(ThreadPool.Count - 1).Start({ATid, CoBxNode.Text})

            Threading.Thread.Sleep(100)

        Next

        StatusBar.Value = 0
        StatusBar.Maximum = ThreadPool.Count

        StatusLabel.Text = "loading ATs..."

        Dim StillRun As Boolean = True
        While StillRun
            StillRun = False

            Dim ThrCnt As Integer = 0
            For Each th In ThreadPool
                If th.IsAlive Then
                    StillRun = True
                    'Exit For
                Else
                    ThrCnt += 1
                End If
            Next

            StatusBar.Value = ThrCnt
            Application.DoEvents()

        End While

        SaveATsToCSV(ATList)
        BLSList.Clear()

        StatusBar.Value = 0
        StatusBar.Maximum = ATList.Count
        StatusLabel.Text = "searching BLSATs..."

        Dim NuATList As List(Of S_AT) = New List(Of S_AT)

        For i As Integer = 0 To ATList.Count - 1
            Dim SAT As S_AT = ATList(i)

            StatusBar.Value = i

            If Not SAT.IsBLS_AT Then
                Continue For
            End If
            NuATList.Add(SAT)
        Next

        StatusBar.Value = 0
        StatusBar.Maximum = NuATList.Count

        For i As Integer = 0 To NuATList.Count - 1
            Dim SAT As S_AT = NuATList(i)

            StatusBar.Value = i

            StatusLabel.Text = "Get ATDetails for: " + SAT.ATRS

            Application.DoEvents()

#Region "get AT Details"

            Dim ATDetails As List(Of String) = BCR.GetATDetails(SAT.AT)

            Dim AT As String = BCR.BetweenFromList(ATDetails, "<at>", "</at>")
            Dim ATRS As String = BCR.BetweenFromList(ATDetails, "<atRS>", "</atRS>")
            Dim Name As String = BCR.BetweenFromList(ATDetails, "<name>", "</name>")
            Dim Description As String = BCR.BetweenFromList(ATDetails, "<description>", "</description>")

            Dim Balance As String = BCR.BetweenFromList(ATDetails, "<balanceNQT>", "</balanceNQT>")
            If Balance = "" Then
                Balance = "0"
            End If

            Dim BalDbl As Double = BCR.Planck2Dbl(CULng(Balance))

            Dim MachineData As String = BCR.BetweenFromList(ATDetails, "<machineData>", "</machineData>")

            Dim MachineDataULongList As List(Of ULong) = BCR.DataStr2ULngList(MachineData)

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
                    InitiatorRS = BCR.BetweenFromList(BCR.RSConvert(Initiator.ToString), "<accountRS>", "</accountRS>")
                End If

                Responser = MachineDataULongList(1)

                If Responser = CULng(0) Then
                    ResponserRS = ""
                Else
                    ResponserRS = BCR.BetweenFromList(BCR.RSConvert(Responser.ToString), "<accountRS>", "</accountRS>")
                End If

                InitiatorCollateral = BCR.Planck2Dbl(MachineDataULongList(2))
                ResponserCollateral = BCR.Planck2Dbl(MachineDataULongList(3))

                BuySellAmount = BCR.Planck2Dbl(MachineDataULongList(4))

                SellOrder = False

                If MachineDataULongList(7) = CULng(1) Then
                    SellOrder = True
                End If

            End If



            Dim Frozen As Boolean = False
            Dim FrozenStr As String = BCR.BetweenFromList(ATDetails, "<frozen>", "</frozen>")

            If Not FrozenStr = "" Then
                Frozen = CBool(FrozenStr)
            End If


            Dim Running As Boolean = False
            Dim RunningStr As String = BCR.BetweenFromList(ATDetails, "<running>", "</running>")

            If Not RunningStr = "" Then
                Running = CBool(RunningStr)
            End If


            Dim Stopped As Boolean = False
            Dim StoppedStr As String = BCR.BetweenFromList(ATDetails, "<stopped>", "</stopped>")

            If Not StoppedStr = "" Then
                Stopped = CBool(StoppedStr)
            End If



            Dim Finished As Boolean = False
            Dim FinishedStr As String = BCR.BetweenFromList(ATDetails, "<finished>", "</finished>")

            If Not FinishedStr = "" Then
                Finished = CBool(FinishedStr)
            End If


            Dim Dead As Boolean = False
            Dim DeadStr As String = BCR.BetweenFromList(ATDetails, "<dead>", "</dead>")

            If Not DeadStr = "" Then
                Dead = CBool(DeadStr)
            End If


            StatusLabel.Text = "checking AT Details for " + ATRS

            Dim SBLSAT As S_BLSAT = New S_BLSAT With {
                .AT = AT,
                .ATRS = ATRS,
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

            StatusLabel.Text = "Get AT TX for: " + SAT.ATRS
            Application.DoEvents()

            Dim BLSTXs As List(Of S_BLSAT_TX) = GetAccountTXList(SAT.AT)

            SBLSAT.AT_TXList = New List(Of S_BLSAT_TX)(BLSTXs.ToArray)
            SBLSAT.Status = GetCurrentBLSATStatus(BLSTXs)
            SBLSAT.XItem = GetCurrentBLSATXItem(BLSTXs)
            SBLSAT.XAmount = GetCurrentBLSATXAmount(BLSTXs)

            'T_BLSAT_TX_List.AddRange(BLSTXs.ToArray)
#End Region

#Region "Convert AT TXs to Orders"
            SBLSAT.AT_OrderList = ConvertTXs2Orders(SBLSAT)

            For Each Order As S_Order In SBLSAT.AT_OrderList

                Order = Order

            Next


#End Region

            BLSList.Add(SBLSAT)

        Next

#Region "set LVs"

        BtBuy.Text = "Buy"
        BtSell.Text = "Sell"

        BtPayOrder.Visible = False
        BtReCreatePayPalOrder.Visible = False
        BtExecuteOrder.Visible = False

        Label15.Visible = False
        TBManuMsg.Visible = False
        BtSendMsg.Visible = False
        ChBxEncMsg.Visible = False

        LVSellorders.Columns.Clear()
        LVSellorders.Columns.Add("Price (" + CoBxMarket.Text + ")")
        LVSellorders.Columns.Add("Amount (" + CoBxMarket.Text + ")")
        LVSellorders.Columns.Add("Total (BURST)")
        LVSellorders.Columns.Add("Collateral (BURST)")
        LVSellorders.Columns.Add("Method")
        LVSellorders.Columns.Add("Seller")
        LVSellorders.Columns.Add("AT")



        LVBuyorders.Columns.Clear()
        With LVBuyorders.Columns.Add("Price (" + CoBxMarket.Text + ")")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Amount (" + CoBxMarket.Text + ")")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Total (BURST)")
            .TextAlign = HorizontalAlignment.Right
        End With
        With LVBuyorders.Columns.Add("Collateral (BURST)")
            .TextAlign = HorizontalAlignment.Right
        End With
        LVBuyorders.Columns.Add("Method")
        LVBuyorders.Columns.Add("Buyer")
        LVBuyorders.Columns.Add("AT")


        LVSellorders.Items.Clear()
        LVBuyorders.Items.Clear()
        LVOpenChannels.Items.Clear()


        LVMyOpenOrders.Columns.Clear()
        LVMyOpenOrders.Columns.Add("Confirmations")
        LVMyOpenOrders.Columns.Add("AT")
        LVMyOpenOrders.Columns.Add("Type")
        LVMyOpenOrders.Columns.Add("Method")
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
        LVMyClosedOrders.Columns.Add("Method")
        LVMyClosedOrders.Columns.Add("Seller")
        LVMyClosedOrders.Columns.Add("Buyer")
        LVMyClosedOrders.Columns.Add("XItem")
        LVMyClosedOrders.Columns.Add("XAmount")
        LVMyClosedOrders.Columns.Add("Quantity")
        LVMyClosedOrders.Columns.Add("Price")
        LVMyClosedOrders.Columns.Add("Status")


        LVMyOpenOrders.Items.Clear()
        LVMyClosedOrders.Items.Clear()

        OrderList.Clear()

        For Each BLS As S_BLSAT In BLSList

            If BLS.AT_TXList.Count = 0 Then

                With LVOpenChannels.Items.Add(BLS.ATRS) 'SmartContract
                    .SubItems.Add(BLS.Name) 'Name
                    .SubItems.Add(BLS.Description) 'Description
                    .SubItems.Add(BLS.Status) 'Status
                    .Tag = BLS
                End With

            Else 'TXList > 0

                'Dim T_OrderList As List(Of S_Order) = ConvertTXs2Orders(BLS)

                OrderList.AddRange(BLS.AT_OrderList)

                Dim ATChannelOpen As Boolean = True


                For Each Order As S_Order In BLS.AT_OrderList

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
                    Dim PayMet As String = "Payment Data"

                    If XItem.Contains("-") Then
                        PayMet = XItem.Split("-")(1)
                        XItem = XItem.Split("-")(0)
                    End If


                    If PayMet <> "Payment Data" Then

                        Select Case PayMet
                            Case "PA"
                                PayMet = "PayPal Account"
                            Case "PB"
                                PayMet = "PayPal Business"
                            Case Else
                                PayMet = PayMet

                        End Select

                    End If
#End Region


                    If Order.Status = "OPEN" Then

                        ATChannelOpen = False

                        If Order.XItem.Contains(CoBxMarket.Text) Then

                            If Not CheckForUTX(, Order.ATRS) And Not CheckATforTX(BLS.ATRS) Then

                                If Order.Type = "SellOrder" Then

                                    With LVSellorders.Items.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                        .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                        .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                        .SubItems.Add(Dbl2LVStr(Order.Collateral)) 'collateral
                                        .SubItems.Add(PayMet) 'payment method

                                        Dim T_SellerRS As String = Order.Seller
                                        If T_SellerRS = TBSNOAddress.Text Then T_SellerRS = "Me"
                                        .SubItems.Add(T_SellerRS) 'Seller

                                        .SubItems.Add(BLS.ATRS) 'AT
                                        .Tag = BLS
                                    End With

                                Else 'BuyOrder

                                    With LVBuyorders.Items.Add(Dbl2LVStr(Order.Price, Decimals)) 'price
                                        .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'amount
                                        .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'total
                                        .SubItems.Add(Dbl2LVStr(Order.Collateral - 1.0)) 'collateral
                                        .SubItems.Add(PayMet) 'payment method

                                        Dim T_BuyerRS As String = Order.Buyer
                                        If T_BuyerRS = TBSNOAddress.Text Then T_BuyerRS = "Me"
                                        .SubItems.Add(T_BuyerRS) 'Buyer

                                        .SubItems.Add(BLS.ATRS) 'at
                                        .Tag = BLS
                                    End With

                                End If

                            End If



                            If TBSNOAddress.Text = Order.Seller Then

                                With LVMyOpenOrders.Items.Add(Confirms) 'confirms
                                    .SubItems.Add(BLS.ATRS) 'AT
                                    .SubItems.Add(Order.Type) 'type
                                    .SubItems.Add(PayMet) 'method
                                    .SubItems.Add("Me") 'Seller
                                    .SubItems.Add(Order.Buyer) 'Buyer
                                    .SubItems.Add(XItem) 'Xitem
                                    .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                    .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                    .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                    '.SubItems.Add(Order.Attachment.ToString) 'Conditions
                                    .SubItems.Add("OPEN") 'Status
                                    .Tag = Order
                                End With

                            ElseIf TBSNOAddress.Text = Order.Buyer Then

                                With LVMyOpenOrders.Items.Add(Confirms) 'confirms
                                    .SubItems.Add(BLS.ATRS) 'AT
                                    .SubItems.Add(Order.Type) 'type
                                    .SubItems.Add(PayMet) 'method
                                    .SubItems.Add(Order.Seller) 'Seller
                                    .SubItems.Add("Me") 'Buyer
                                    .SubItems.Add(XItem) 'Xitem
                                    .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                    .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                    .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                    '.SubItems.Add(Order.Attachment.ToString) 'Conditions
                                    .SubItems.Add("OPEN") 'Status
                                    .Tag = Order
                                End With

                            End If 'myaddress

                        End If 'market


                    ElseIf Order.Status = "RESERVED" Then

                        ATChannelOpen = False

                        If TBSNOAddress.Text = Order.Seller Then

                            Dim AlreadySend As String = CheckBillingInfosAlreadySend(Order)
                            If AlreadySend.Trim = "" Then

                                If Not GetAutoinfoTXFromINI(Order.FirstTransaction) Then 'Check for autosend-info-TX in Settings.ini and skip if founded

                                    If ChBxAutoSendPaymentInfo.Checked Then 'autosend info to Buyer

                                        Dim T_PaymentInfoJSON As String = PaymentInfoJSON

                                        If T_PaymentInfoJSON.Trim.Contains("ppodr") Then

                                            Dim APIOK As String = CheckPayPalAPI()

                                            If APIOK = "True" Then
                                                Dim PPAPI_Autosignal As ClsPayPal = New ClsPayPal
                                                PPAPI_Autosignal.Client_ID = TBPayPalAPIUser.Text
                                                PPAPI_Autosignal.Secret = TBPayPalAPISecret.Text

                                                Dim PPOrderID As List(Of String) = PPAPI_Autosignal.CreateOrder("Burst", Order.Quantity, Order.XAmount, "EUR")
                                                T_PaymentInfoJSON = T_PaymentInfoJSON.Replace("<PAYPALORDER>", PPAPI_Autosignal.BetweenFromList(PPOrderID, "<id>", "</id>"))
                                            End If

                                        End If

                                        Dim T_MsgStr As String = "{""at"":""" + Order.AT + """, ""tx"":""" + Order.FirstTransaction + """" + T_PaymentInfoJSON + "}"
                                        Dim TXr As String = SendBillingInfos(Order.BuyerID, T_MsgStr)


                                        If SetAutoinfoTX2INI(Order.FirstTransaction) Then 'Set autosend-info-TX in Settings.ini
                                            'ok
                                        End If

                                    End If

                                End If

                            Else 'BillingInfo Already send, check for aproving

                                If DelAutoinfoTXFromINI(Order.FirstTransaction) Then 'Delete autosend-info-TX from Settings.ini
                                    'ok
                                End If

                                Dim BillingInfo As String = BCR.Between(AlreadySend, "<info>", "</info>", GetType(String))
                                Dim PayPalEMail As String = BCR.Between(AlreadySend, "<ppem>", "</ppem>", GetType(String))
                                Dim PayPalAccID As String = BCR.Between(AlreadySend, "<ppacid>", "</ppacid>", GetType(String))
                                Dim PayPalOrder As String = BCR.Between(AlreadySend, "<ppodr>", "</ppodr>", GetType(String))


                                If BillingInfo.Trim <> "" Then
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " Payment Info: " + BillingInfo
                                End If
                                If PayPalEMail.Trim <> "" Then
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " PayPal-EMail: " + PayPalEMail
                                End If
                                If PayPalAccID.Trim <> "" Then
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " PayPal-AccountID: " + PayPalAccID
                                End If


                                If ChBxCheckXItemTX.Checked Then 'autosignal AT

                                    If ChBxUsePayPalSettings.Checked Then

                                        Dim APIOK As String = CheckPayPalAPI()
                                        If APIOK = "True" Then

                                            If RBPayPalOrder.Checked Then
#Region "PayPal Order Automation"

                                                If Not PayPalOrder = "0" And Not PayPalOrder = "" Then

                                                    'PayPal Approving check
                                                    Dim PPAPI As ClsPayPal = New ClsPayPal
                                                    PPAPI.Client_ID = TBPayPalAPIUser.Text
                                                    PPAPI.Secret = TBPayPalAPISecret.Text

                                                    Dim OrderDetails As List(Of String) = PPAPI.GetOrderDetails(PayPalOrder)
                                                    Dim Status As String = PPAPI.BetweenFromList(OrderDetails, "<status>", "</status>")

                                                    If Status = "APPROVED" Then
                                                        PPAPI = New ClsPayPal
                                                        PPAPI.Client_ID = TBPayPalAPIUser.Text
                                                        PPAPI.Secret = TBPayPalAPISecret.Text

                                                        OrderDetails = PPAPI.CaptureOrder(PayPalOrder)
                                                        Status = PPAPI.BetweenFromList(OrderDetails, "<status>", "</status>")

                                                        If Status = "COMPLETED" Then
                                                            Dim BCR1 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
                                                            Dim TXStr As String = BCR1.SendMessage2BLSAT(Order.AT, 1.0, New List(Of ULong)({BCR1.ReferenceFinishOrder}))
                                                            AlreadySend = "COMPLETED"
                                                        Else
                                                            AlreadySend = Status
                                                        End If

                                                    ElseIf Status = "COMPLETED" Then

                                                        'Dim BoolUTX As Boolean = CheckForUTX(, Order.ATRS)
                                                        'Dim BoolTX As Boolean = CheckForTX(Order.ATRS, Order.Seller, Order.FirstTimestamp, "order accepted", True)

                                                        'If BoolUTX = False And BoolTX = False Then
                                                        '    AlreadySend = "COMPLETED"
                                                        'Else
                                                        AlreadySend = "COMPLETED"
                                                        'End If

                                                    ElseIf Status = "CREATED" Then

                                                        AlreadySend = "PayPal Order created"

                                                    Else
                                                        'TODO: auto Recreate PayPal Order
                                                        AlreadySend = "No Payment received"
                                                    End If

                                                End If

#End Region
                                            Else 'If RBPayPalAccID.Checked Then
#Region "PayPal Check TX Automation"
                                                Dim BCR1 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text)
                                                Dim CheckAttachment As String = BCR1.ULngList2DataStr(New List(Of ULong)({BCR1.ReferenceFinishOrder}))
                                                Dim UTXCheck As Boolean = CheckForUTX(Order.Seller, Order.ATRS, CheckAttachment)
                                                Dim TXCheck As Boolean = CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, CheckAttachment)


                                                If Not UTXCheck And Not TXCheck Then

                                                    If Not GetAutosignalTXFromINI(Order.FirstTransaction) Then 'Check for autosignal-TX in Settings.ini and skip if founded

                                                        'PayPal Approving check
                                                        Dim PPAPI As ClsPayPal = New ClsPayPal
                                                        PPAPI.Client_ID = TBPayPalAPIUser.Text
                                                        PPAPI.Secret = TBPayPalAPISecret.Text

                                                        Dim TXDetails As List(Of List(Of String)) = PPAPI.GetTransactionList(Order.FirstTransaction)

                                                        If TXDetails.Count > 0 Then

                                                            Dim Status As String = PPAPI.BetweenFromList(TXDetails(0), "<transaction_status>", "</transaction_status>")
                                                            Dim Amount As String = PPAPI.BetweenFromList(TXDetails(0), "<transaction_amount>", "</transaction_amount>")

                                                            If CDbl(Amount) >= Order.XAmount And Status.ToLower.Trim = "s" Then
                                                                'complete
                                                                Dim TXStr As String = BCR1.SendMessage2BLSAT(Order.AT, 1.0, New List(Of ULong)({BCR1.ReferenceFinishOrder}))
                                                                AlreadySend = "COMPLETED"


                                                                If SetAutosignalTX2INI(Order.FirstTransaction) Then 'Set autosignal-TX in Settings.ini
                                                                    'ok
                                                                End If

                                                            End If

                                                        End If

                                                    End If

                                                Else

                                                    AlreadySend = "PENDING"

                                                End If

                                                ' End If
#End Region
                                            End If

                                        End If

                                    End If

                                End If


                            End If


                            With LVMyOpenOrders.Items.Add(Confirms) 'confirms
                                .SubItems.Add(BLS.ATRS) 'AT
                                .SubItems.Add(Order.Type) 'type
                                .SubItems.Add(PayMet) 'method
                                .SubItems.Add("Me") 'Seller
                                .SubItems.Add(Order.Buyer) 'Buyer
                                .SubItems.Add(XItem) 'Xitem
                                .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                '.SubItems.Add(Order.Attachment.ToString) 'Conditions
                                .SubItems.Add(AlreadySend) 'Status
                                .Tag = Order
                            End With



                        ElseIf TBSNOAddress.Text = Order.Buyer Then

                            Dim SellerTX = BCR.GetAccountTransactions(Order.SellerID, Order.FirstTimestamp)

                            Dim AlreadySend As String = CheckBillingInfosAlreadySend(Order)
                            If AlreadySend.Trim <> "" Then

                                Dim BillingInfo As String = BCR.Between(AlreadySend, "<info>", "</info>", GetType(String))
                                Dim PayPalEMail As String = BCR.Between(AlreadySend, "<ppem>", "</ppem>", GetType(String))
                                Dim PayPalAccID As String = BCR.Between(AlreadySend, "<ppacid>", "</ppacid>", GetType(String))
                                Dim PayPalOrder As String = BCR.Between(AlreadySend, "<ppodr>", "</ppodr>", GetType(String))

                                If BillingInfo.Trim <> "" Then
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " Payment Info: " + BillingInfo
                                End If
                                If PayPalEMail.Trim <> "" Then
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " PayPal-EMail: " + PayPalEMail
                                End If
                                If PayPalAccID.Trim <> "" Then
                                    'TODO: Send Automatic PayPal Payment with CreateBatchPayOut function only once
                                    Dim ATStr As String = BCR.Between(AlreadySend, "<at>", "</at>", GetType(String))
                                    Dim TXStr As String = BCR.Between(AlreadySend, "<tx>", "</tx>", GetType(String))
                                    AlreadySend = "Payment Channel: " + ATStr + " Payment Reference: " + TXStr + " PayPal-AccountID: " + PayPalAccID
                                End If
                                If PayPalOrder.Trim <> "" Then
                                    Dim PPAPI As ClsPayPal = New ClsPayPal

                                    AlreadySend = "https://www.sandbox.paypal.com" + "/checkoutnow?token=" + PayPalOrder '"https://www.sandbox.paypal.com/checkoutnow?token=" 
                                    'Process.Start(AlreadySend)
                                End If

                            End If

                            Dim BCR1 As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text)
                            Dim CheckAttachment As String = BCR1.ULngList2DataStr(New List(Of ULong)({BCR.ReferenceFinishOrder}))
                            Dim UTXCheck As Boolean = CheckForUTX(Order.Seller, Order.ATRS, CheckAttachment)
                            Dim TXCheck As Boolean = CheckForTX(Order.Seller, Order.ATRS, Order.FirstTimestamp, CheckAttachment)

                            If Not UTXCheck And Not TXCheck Then

                            Else
                                AlreadySend = "PENDING"
                            End If



                            With LVMyOpenOrders.Items.Add(Confirms) 'confirms
                                .SubItems.Add(BLS.ATRS) 'AT
                                .SubItems.Add(Order.Type) 'type
                                .SubItems.Add(PayMet) 'method
                                .SubItems.Add(Order.Seller) 'Seller
                                .SubItems.Add("Me") 'Buyer
                                .SubItems.Add(XItem) 'Xitem
                                .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                '.SubItems.Add(Order.Attachment.ToString) 'Conditions
                                .SubItems.Add(AlreadySend) 'Status
                                .Tag = Order
                            End With

                        End If 'myaddress

                    Else 'CLOSED or CANCELED


                        If DelAutosignalTXFromINI(Order.FirstTransaction) Then 'Delete autosignal-TX from Settings.ini
                            'ok
                        End If


                        If Order.XItem.Contains(CoBxMarket.Text) Then

                            If TBSNOAddress.Text = Order.Seller Then

                                With LVMyClosedOrders.Items.Add(Order.FirstTransaction) 'first transaction
                                    .SubItems.Add(Order.LastTX.Transaction) 'last transaction
                                    .SubItems.Add(Order.LastTX.Confirmations.ToString) 'confirms
                                    .SubItems.Add(Order.ATRS) 'AT
                                    .SubItems.Add(Order.Type) 'type
                                    .SubItems.Add(PayMet) 'method
                                    .SubItems.Add("Me") 'Seller
                                    .SubItems.Add(Order.Buyer) 'Buyer
                                    .SubItems.Add(XItem) 'Xitem
                                    .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                    .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                    .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                    .SubItems.Add(Order.Status) 'Status
                                    .SubItems.Add(Order.Attachment.ToString) 'Conditions
                                    .Tag = Order
                                End With

                            ElseIf TBSNOAddress.Text = Order.Buyer Then

                                With LVMyClosedOrders.Items.Add(Order.FirstTransaction) 'first transaction
                                    .SubItems.Add(Order.LastTX.Transaction) 'last transaction
                                    .SubItems.Add(Order.LastTX.Confirmations.ToString) 'confirms
                                    .SubItems.Add(Order.ATRS) 'AT
                                    .SubItems.Add(Order.Type) 'type
                                    .SubItems.Add(PayMet) 'method
                                    .SubItems.Add(Order.Seller) 'Seller
                                    .SubItems.Add("Me") 'Buyer
                                    .SubItems.Add(XItem) 'Xitem
                                    .SubItems.Add(Dbl2LVStr(Order.XAmount, Decimals)) 'XAmount
                                    .SubItems.Add(Dbl2LVStr(Order.Quantity)) 'Quantity
                                    .SubItems.Add(Dbl2LVStr(Order.Price, Decimals)) 'Price
                                    .SubItems.Add(Order.Status) 'Status
                                    .SubItems.Add(Order.Attachment.ToString) 'Conditions
                                    .Tag = Order
                                End With

                            End If 'myaddress

                        End If 'market

                    End If


                Next

                If ATChannelOpen Then

                    If BLS.Frozen.ToString.ToLower = "true" And BLS.Finished.ToString.ToLower = "true" And BLS.Dead.ToString.ToLower <> "true" Then
                        With LVOpenChannels.Items.Add(BLS.ATRS) 'SmartContract
                            .SubItems.Add(BLS.Name) 'Name
                            .SubItems.Add(BLS.Description) 'Description
                            .SubItems.Add(BLS.Status) 'Status
                            .Tag = BLS
                        End With

                    ElseIf BLS.Dead.ToString.ToLower = "true" Then

                        With LVOpenChannels.Items.Add(BLS.ATRS) 'SmartContract
                            .SubItems.Add(BLS.Name) 'Name
                            .SubItems.Add(BLS.Description) 'Description
                            .SubItems.Add("DEAD") 'Status
                            .BackColor = Color.Crimson
                            .ForeColor = Color.White
                        End With

                    End If

                End If


            End If 'BLS.AT_TXList.Count

        Next

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

        Dim T_BlockFeeThread As Threading.Thread = New Threading.Thread(AddressOf BlockFeeThread)

        StatusBlockLabel.Text = "loading Blockheight..."
        StatusFeeLabel.Text = "loading Current Slotfee..."
        StatusStrip1.Refresh()

        T_BlockFeeThread.Start(CoBxNode.Text)


        'BLSAT_TX_List.Clear()
        'BLSAT_TX_List.AddRange(T_BLSAT_TX_List.ToArray)
        'BLSAT_TX_List = BLSAT_TX_List.OrderBy(Function(BLSAT_TX As S_BLSAT_TX) BLSAT_TX.Timestamp).ToList


        StatusBar.Value = 0
        StatusBar.Maximum = 100
        StatusBar.Visible = False
        StatusLabel.Text = ""

        SplitContainer12.Enabled = True

        Return True

    End Function
    Sub LoadHistory(ByVal input As Object)

        Dim CoBxChartVal As Integer = DirectCast(input(0), Integer)
        Dim CoBxTickVal As Integer = DirectCast(input(1), Integer)

        Dim Xitem As String = DirectCast(input(2), String)
        Dim BSR As ClsBurstAPI = New ClsBurstAPI(DirectCast(input(3), String)) ' With {.C_Node = DirectCast(input(3), String)}

        Dim T_OrderList As List(Of S_Order) = New List(Of S_Order)

        For Each HisOrder As S_Order In OrderList

            Dim T_Order As S_Order = New S_Order
            T_Order = HisOrder

            Dim Status As String = "CLOSED"

            If HisOrder.Status = "CLOSED" Then
                T_OrderList.Add(T_Order)
            End If

        Next


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

        '0=MinVal; 1=MaxVal; 2=LastVal; 3=List(date, close)
        PanelForSplitPanel.Controls(0).Tag = New List(Of Object)({Xitem, Minval, Maxval, Chart(Chart.Count - 1).CloseValue, Chart})

        TradeTrackCalcs(PanelForSplitPanel.Controls(0))

    End Sub
    Function TradeTrackCalcs(ByVal input As Object) As Boolean

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
            XList.Add(New List(Of Object)({Candle.CloseDat.ToString, Candle.CloseValue}))
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

        Return True

    End Function


#Region "AT Interactions"
    Function GetLastTXWithValues(ByVal BLSTX As List(Of S_BLSAT_TX), Optional ByVal Xitem As String = "", Optional ByVal AttSearch As String = "<xItem>") As S_BLSAT_TX

        Dim BLSAT_TX As S_BLSAT_TX = New S_BLSAT_TX

        Dim NuBLSTX As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)
        NuBLSTX.AddRange(BLSTX.ToArray)
        NuBLSTX.Reverse()

        Dim Found As Boolean = False
        Dim ReferenceSender As String = ""


        For i As Integer = 0 To NuBLSTX.Count - 1
            Dim TTX As S_BLSAT_TX = NuBLSTX(i)
            BLSAT_TX = TTX


            If Not IsNothing(BLSAT_TX.Attachment) Then
                If Not BLSAT_TX.Attachment.Contains(Xitem) And Not Xitem.Trim = "" Then
                    Found = False
                End If

                If Not ReferenceSender = BLSAT_TX.SenderRS Then
                    Found = False
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
                    ReferenceSender = TTX.RecipientRS
                End If

            End If

        Next

        If Found Then
            Return BLSAT_TX
        Else
            Return New S_BLSAT_TX("")
        End If


    End Function

    Function ConvertTXs2Orders(ByVal BLSAT As S_BLSAT) As List(Of S_Order)

        Dim NuOrderList As List(Of S_Order) = New List(Of S_Order)
        Dim TXSplitList As List(Of List(Of S_BLSAT_TX)) = GetDeals(BLSAT.AT_TXList, "")

        For Each TXOrder As List(Of S_BLSAT_TX) In TXSplitList

            Dim Order As S_Order = ConvertTXs2Order(BLSAT, TXOrder)

            If Not IsNothing(Order) Then
                NuOrderList.Add(Order)
            End If

        Next

        Return NuOrderList

    End Function
    Function GetDeals(ByVal BLSTX As List(Of S_BLSAT_TX), ByVal MyAddress As String, Optional ByVal Xitem As String = "") As List(Of List(Of S_BLSAT_TX))

        Dim RetList As List(Of List(Of S_BLSAT_TX)) = New List(Of List(Of S_BLSAT_TX))

        Dim BLSAT_TX As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

        Dim NuBLSTX As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)
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
            Dim TTX As S_BLSAT_TX = NuBLSTX(i)
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

                    BLSAT_TX = New List(Of S_BLSAT_TX)
                    Continue For
                Else
                    FoundDeal = False
                    BLSAT_TX = New List(Of S_BLSAT_TX)
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
    Function ConvertTXs2Order(ByVal BLSAT As S_BLSAT, ByVal TXList As List(Of S_BLSAT_TX)) As S_Order

        Dim NuOrder As S_Order = New S_Order
        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}
        Dim FirstTX As S_BLSAT_TX = New S_BLSAT_TX

        If TXList.Count > 0 Then
            FirstTX = GetLastTXWithValues(TXList, "",)
            If FirstTX.Attachment.Trim = "" Then
                Return Nothing
            End If
        End If

        Dim FirstAmount As Double = BCR.Planck2Dbl(CULng(FirstTX.AmountNQT))

        Dim Xitem As String = BCR.Between(FirstTX.Attachment, "<xItem>", "</xItem>", GetType(String))
        Dim Xamount As Double = BCR.Planck2Dbl(CULng(BCR.Between(FirstTX.Attachment, "<xAmount>", "</xAmount>", GetType(String))))
        Dim Collateral As Double = BCR.Planck2Dbl(CULng(BCR.Between(FirstTX.Attachment, "<colBuyAmount>", "</colBuyAmount>", GetType(String))))

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



        Dim LastTX As S_BLSAT_TX = New S_BLSAT_TX

        If Status = "CANCELED" And ResponserRS.Trim = "" Then

            For Each ConfirmedTX As S_BLSAT_TX In TXList

                If ConfirmedTX.Attachment.Contains("finished") Then
                    LastTX = ConfirmedTX
                    Exit For
                End If

            Next

        Else

            For Each ConfirmedTX As S_BLSAT_TX In TXList

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

    Function GetAccountTXList(ByVal ATID As String) As List(Of S_BLSAT_TX)

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}

        Dim ATTXs As List(Of List(Of String)) = BCR.GetAccountTransactions(ATID)
        Dim BLSTXs As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

        For Each ATTX As List(Of String) In ATTXs

            Dim TX_Type As String = BCR.BetweenFromList(ATTX, "<type>", "</type>")
            Dim TX_Timestamp As String = BCR.BetweenFromList(ATTX, "<timestamp>", "</timestamp>")
            Dim TX_Recipient As String = BCR.BetweenFromList(ATTX, "<recipient>", "</recipient>")
            Dim TX_RecipientRS As String = BCR.BetweenFromList(ATTX, "<recipientRS>", "</recipientRS>")
            Dim TX_AmountNQT As String = BCR.BetweenFromList(ATTX, "<amountNQT>", "</amountNQT>")
            Dim TX_FeeNQT As String = BCR.BetweenFromList(ATTX, "<feeNQT>", "</feeNQT>")
            Dim TX_Transaction As String = BCR.BetweenFromList(ATTX, "<transaction>", "</transaction>")

            StatusLabel.Text = "checking AT TX: " + TX_Transaction

            Dim TX_Attachment As String = BCR.BetweenFromList(ATTX, "<attachment>", "</attachment>")
            If TX_Attachment.Trim = "" Then
                TX_Attachment = BCR.BetweenFromList(ATTX, "<message>", "</message>")
            End If

            Dim TX_Sender As String = BCR.BetweenFromList(ATTX, "<sender>", "</sender>")
            Dim TX_SenderRS As String = BCR.BetweenFromList(ATTX, "<senderRS>", "</senderRS>")
            Dim TX_Confirmations As String = BCR.BetweenFromList(ATTX, "<confirmations>", "</confirmations>")


            Dim BLSTX As S_BLSAT_TX = New S_BLSAT_TX With {
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

        BLSTXs = BLSTXs.OrderBy(Function(BLSAT_TX As S_BLSAT_TX) BLSAT_TX.Timestamp).ToList

        Return BLSTXs

    End Function

    Function GetCurrentBLSATStatus(ByVal BLSAT_TXList As List(Of S_BLSAT_TX)) As String

        Dim NU_BLSAT_TXList As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_BLSAT_TX In NU_BLSAT_TXList

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
    Function GetCurrentBLSATXItem(ByVal BLSAT_TXList As List(Of S_BLSAT_TX)) As String

        Dim NU_BLSAT_TXList As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_BLSAT_TX In NU_BLSAT_TXList

            If BLSATTX.Type = "BLSTX" Then

                If BLSATTX.Attachment.Contains("<xItem>") Then

                    Return Between(BLSATTX.Attachment, "<xItem>", "</xItem>", GetType(String))

                End If

            End If

        Next

        Return ""

    End Function
    Function GetCurrentBLSATXAmount(ByVal BLSAT_TXList As List(Of S_BLSAT_TX)) As Double

        Dim NU_BLSAT_TXList As List(Of S_BLSAT_TX) = New List(Of S_BLSAT_TX)

        NU_BLSAT_TXList.AddRange(BLSAT_TXList.ToArray)

        NU_BLSAT_TXList.Reverse()


        For Each BLSATTX As S_BLSAT_TX In NU_BLSAT_TXList

            If BLSATTX.Type = "BLSTX" Then

                If BLSATTX.Attachment.Contains("<xAmount>") Then

                    Return CDbl(Between(BLSATTX.Attachment, "<xAmount>", "</xAmount>", GetType(String)))

                End If

            End If

        Next

        Return 0.0

    End Function

    Function SendBillingInfos(ByVal Recipient As String, ByVal Message As String, Optional ByVal Encrypt As Boolean = True)
        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}
        Dim TX As String = BCR.SendMessage(Recipient, Message,, Encrypt)
        Return TX
    End Function

#End Region

#Region "PayPal Interactions"
    Function CheckPayPalAPI() As String

        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = TBPayPalAPIUser.Text
        PPAPI.Secret = TBPayPalAPISecret.Text

        Dim PPOrderID As List(Of String) = PPAPI.GetAuthToken()

        If PPOrderID.Count > 0 Then
            If PPOrderID(0).Contains("<error>") Then
                Return Between(PPOrderID(0), "<error>", "</error>", GetType(String))
            End If
        End If


        Return "True"

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

        Dim RangeStart As Date = Now.AddDays(-ChartRangeDays)

        Dim CandleList As List(Of S_Candle) = New List(Of S_Candle)

        Dim OldCandle As S_Candle = New S_Candle

        While Not RangeStart.ToLongDateString + " " + RangeStart.ToShortTimeString = Now.ToLongDateString + " " + Now.ToShortTimeString

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

#End Region
#Region "Checking UTX/TX"
    Function CheckForUTX(Optional ByVal SenderRS As String = "", Optional ByVal RecipientRS As String = "", Optional ByVal SearchAttachment As String = "") As Boolean

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()

        For Each UTX In T_UTXList

            Dim TX_SenderRS As String = BCR.BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim TX_RecipientRS As String = BCR.BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If (TX_SenderRS = SenderRS Or SenderRS = "") And (RecipientRS = TX_RecipientRS Or RecipientRS = "") And Not (SenderRS = "" And RecipientRS = "") Then

                Dim T_Msg As String = BCR.BetweenFromList(UTX, "<message>", "</message>")
                If T_Msg.ToLower.Trim = SearchAttachment.ToLower.Trim Or SearchAttachment = "" Then
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

        Dim BCR As ClsBurstAPI = New ClsBurstAPI
        Dim AccountAddressList As List(Of String) = New List(Of String)

        'If Not SenderRS.Trim = "" Then
        AccountAddressList = BCR.RSConvert(SenderRS)
        'Else
        '    AccountAddressList = BCR.RSConvert(RecipientRS)
        'End If


        Dim T_Account As String = BCR.Between(AccountAddressList(0), "<account>", "</account>")

        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(T_Account, FromTimestamp)

        Dim AnswerTX As List(Of String) = New List(Of String)

        For Each T_TX In TXList

            Dim TX_SenderRS As String = BCR.BetweenFromList(T_TX, "<senderRS>", "</senderRS>")
            Dim TX_RecipientRS As String = BCR.BetweenFromList(T_TX, "<recipientRS>", "</recipientRS>")


            If TX_SenderRS = SenderRS And RecipientRS = TX_RecipientRS And Not (SenderRS = "" And RecipientRS = "") Then

                Dim T_Msg As String = BCR.BetweenFromList(T_TX, "<message>", "</message>")

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

    Function CheckATforTX(ByVal ATID As String) As Boolean

        Dim ATTXList As List(Of S_BLSAT_TX) = GetAccountTXList(ATID)

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
        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text) ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = TBSNOPassPhrase.Text}

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()
        T_UTXList.Reverse()

        For Each UTX As List(Of String) In T_UTXList

            Dim TX As String = BCR.BetweenFromList(UTX, "<transaction>", "</transaction>")
            Dim Sender As String = BCR.BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BCR.BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Seller And Recipient = Order.Buyer Then

                Dim Data As String = BCR.BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BCR.BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else
                    Dim DecryptedMsg As String = BCR.DecryptFrom(Order.Buyer, Data, Nonce)

                    If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                        Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                        Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                        If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                            Return DecryptedMsg
                        End If

                    End If

                End If

            ElseIf Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim Data As String = BCR.BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BCR.BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else
                    Dim DecryptedMsg As String = BCR.DecryptFrom(Order.Seller, Data, Nonce)

                    If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                        Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                        Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                        If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                            Return DecryptedMsg
                        End If

                    End If

                End If

            End If

        Next



        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(Order.BuyerID, Order.FirstTimestamp)
        TXList.Reverse()

        For Each SearchTX As List(Of String) In TXList

            Dim TX As String = BCR.BetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim Sender As String = BCR.BetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BCR.BetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Seller And Recipient = Order.Buyer Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                    Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                    Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                    If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                        Return DecryptedMsg
                    End If

                End If

            ElseIf Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                    Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                    Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                    If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                        Return DecryptedMsg
                    End If

                End If

            End If

        Next

        Return ""

    End Function

    Function CheckPaymentAlreadySend(ByVal Order As S_Order) As String

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, TBSNOPassPhrase.Text)

        Dim T_UTXList As List(Of List(Of String)) = BCR.GetUnconfirmedTransactions()
        T_UTXList.Reverse()

        Dim TXList As List(Of List(Of String)) = BCR.GetAccountTransactions(Order.SellerID, Order.FirstTimestamp)
        TXList.Reverse()

        For Each UTX As List(Of String) In T_UTXList

            Dim TX As String = BCR.BetweenFromList(UTX, "<transaction>", "</transaction>")
            Dim Sender As String = BCR.BetweenFromList(UTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BCR.BetweenFromList(UTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim Data As String = BCR.BetweenFromList(UTX, "<data>", "</data>")
                Dim Nonce As String = BCR.BetweenFromList(UTX, "<nonce>", "</nonce>")

                If Data.Trim = "" Or Nonce.Trim = "" Then
                    'no
                Else
                    Dim DecryptedMsg As String = BCR.DecryptFrom(Order.Seller, Data, Nonce)

                    If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                        Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                        Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                        'Dim BillingInfo As String = BCR.Between(DecryptedMsg, "<info>", "</info>", GetType(String))
                        'Dim PayPalEMail As String = BCR.Between(DecryptedMsg, "<ppem>", "</ppem>", GetType(String))
                        'Dim PayPalAccID As String = BCR.Between(DecryptedMsg, "<ppacid>", "</ppacid>", GetType(String))
                        'Dim PayPalOrder As String = BCR.Between(DecryptedMsg, "<ppodr>", "</ppodr>", GetType(String))

                        If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                            Return DecryptedMsg
                        End If

                    End If

                End If

                'Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                'If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                '    Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                '    Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                '    'Dim BillingInfo As String = BCR.Between(DecryptedMsg, "<info>", "</info>", GetType(String))
                '    'Dim PayPalEMail As String = BCR.Between(DecryptedMsg, "<ppem>", "</ppem>", GetType(String))
                '    'Dim PayPalAccID As String = BCR.Between(DecryptedMsg, "<ppacid>", "</ppacid>", GetType(String))
                '    'Dim PayPalOrder As String = BCR.Between(DecryptedMsg, "<ppodr>", "</ppodr>", GetType(String))

                '    If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                '        Return DecryptedMsg
                '    End If
                'Else

                '    DecryptedMsg = DecryptedMsg
                '    DecryptedMsg = BCR.DecryptFrom(Order.Buyer, Data, Nonce)
                '    DecryptedMsg = DecryptedMsg

                'End If

            End If

        Next



        For Each SearchTX As List(Of String) In TXList

            Dim TX As String = BCR.BetweenFromList(SearchTX, "<transaction>", "</transaction>")
            Dim Sender As String = BCR.BetweenFromList(SearchTX, "<senderRS>", "</senderRS>")
            Dim Recipient As String = BCR.BetweenFromList(SearchTX, "<recipientRS>", "</recipientRS>")

            If Sender = Order.Buyer And Recipient = Order.Seller Then

                Dim DecryptedMsg As String = BCR.ReadMessage(TX)

                If DecryptedMsg.Contains("<at>") And DecryptedMsg.Contains("<tx>") Then

                    Dim DCAT As String = BCR.Between(DecryptedMsg, "<at>", "</at>", GetType(String))
                    Dim DCTransaction As String = BCR.Between(DecryptedMsg, "<tx>", "</tx>", GetType(String))

                    'Dim BillingInfo As String = BCR.Between(DecryptedMsg, "<info>", "</info>", GetType(String))
                    'Dim PayPalEMail As String = BCR.Between(DecryptedMsg, "<ppem>", "</ppem>", GetType(String))
                    'Dim PayPalAccID As String = BCR.Between(DecryptedMsg, "<ppacid>", "</ppacid>", GetType(String))
                    'Dim PayPalOrder As String = BCR.Between(DecryptedMsg, "<ppodr>", "</ppodr>", GetType(String))

                    If DCAT = Order.AT And DCTransaction = Order.FirstTransaction Then
                        Return DecryptedMsg
                    End If

                End If

            End If

        Next

        Return ""


    End Function


    Function SetAutoinfoTX2INI(ByVal TX As String) As Boolean

        Dim AutoinfoTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autoinfotransactions", " ")

        If AutoinfoTXStr.Trim = "" Then
            AutoinfoTXStr = TX + ";"
        ElseIf AutoinfoTXStr.Contains(";") Then
            AutoinfoTXStr += TX + ";"
        End If

        INISetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autoinfotransactions", AutoinfoTXStr.Trim)

        Return True

    End Function
    Function GetAutoinfoTXFromINI(ByVal TX As String) As Boolean

        Dim AutoinfoTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autoinfotransactions", " ")
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

        Dim AutoinfoTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autoinfotransactions", " ")
        Dim AutoinfoTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutoinfoTXStr.Contains(TX + ";") Then
            Returner = True
            AutoinfoTXStr = AutoinfoTXStr.Replace(TX + ";", "")
        End If

        INISetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autoinfotransactions", AutoinfoTXStr.Trim)

        Return Returner

    End Function


    Function SetAutosignalTX2INI(ByVal TX As String) As Boolean

        Dim AutosignalTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autosignaltransactions", " ")

        If AutosignalTXStr.Trim = "" Then
            AutosignalTXStr = TX + ";"
        ElseIf AutosignalTXStr.Contains(";") Then
            AutosignalTXStr += TX + ";"
        End If

        INISetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autosignaltransactions", AutosignalTXStr.Trim)

        Return True

    End Function
    Function GetAutosignalTXFromINI(ByVal TX As String) As Boolean

        Dim AutosignalTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autosignaltransactions", " ")
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

        Dim AutosignalTXStr As String = INIGetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autosignaltransactions", " ")
        Dim AutosignalTXList As List(Of String) = New List(Of String)

        Dim Returner As Boolean = False

        If AutosignalTXStr.Contains(TX + ";") Then
            Returner = True
            AutosignalTXStr = AutosignalTXStr.Replace(TX + ";", "")
        End If

        INISetValue(Application.StartupPath + "/Settings.ini", "Temp", "Autosignaltransactions", AutosignalTXStr.Trim)

        Return Returner

    End Function

#End Region
#Region "Tools"
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
                            Return Val(input.Replace(",", "."))
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

#Region "ListView Specials"

    ''' <summary>
    ''' Von einer ListView das subitem aus dem item lesen
    ''' </summary>
    ''' <param name="LV">Die ListView, aus der gelesen werden soll</param>
    ''' <param name="ColName">Der Spaltenname, aus dem gelesen werden soll</param>
    ''' <param name="LVItem">Die Zeile bzw. das item aus dem gelesen werden soll</param>
    ''' <param name="index">Alternativ das item an index stelle in der ListView</param>
    ''' <returns>Vorzugsweise einen String, andernfalls den index der Spalte</returns>
    ''' <remarks></remarks>
    Function GetLVColNameFromSubItem(ByRef LV As ListView, ByVal ColName As String, Optional ByVal LVItem As ListViewItem = Nothing, Optional ByVal index As Integer = -1) As Object

        If index > -1 Then
            LVItem = LV.Items.Item(index)
        End If

        If LVItem Is Nothing Then

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return i
                End If
            Next

        Else

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return LVItem.SubItems.Item(i).Text
                End If
            Next

        End If

        Return -1

    End Function
    Sub SetLVColName2SubItem(ByRef LV As ListView, ByRef LVItem As ListViewItem, ByVal ColName As String, ByVal SetStr As String, Optional ByVal ForeColor As Color = Nothing, Optional ByVal BackColor As Color = Nothing)

        Dim IDX As Integer = GetLVColNameFromSubItem(LV, ColName)

        If Not IDX < 0 Then
            If IsNothing(ForeColor) Or IsNothing(BackColor) Then
                LVItem.SubItems(IDX).Text = SetStr
            Else
                LVItem.SubItems(IDX).Text = SetStr

                If Not IsNothing(ForeColor) Then
                    LVItem.SubItems(IDX).ForeColor = ForeColor
                End If

                If Not IsNothing(BackColor) Then
                    LVItem.SubItems(IDX).BackColor = BackColor
                End If

            End If

        End If

    End Sub

#End Region
#End Region

#End Region

#Region "CSV Specials"
    Private Function GetATsFromCSV() As List(Of S_AT)

        Try

            Dim AT_CSV_FilePath As String = Application.StartupPath + "\ATs.CSV"

            Dim New_CSV_ATList As List(Of S_AT) = New List(Of S_AT)

            If IO.File.Exists(AT_CSV_FilePath) Then

                Dim CSV_ATList As CSVTool.CSVReader = New CSVTool.CSVReader(AT_CSV_FilePath)

                For Each ItemAry As String() In CSV_ATList.Lists

                    If ItemAry.Length >= 2 Then
                        Dim T_AT As S_AT = New S_AT
                        T_AT.AT = ItemAry(0)
                        T_AT.ATRS = ItemAry(1)
                        T_AT.IsBLS_AT = ItemAry(2)
                        New_CSV_ATList.Add(T_AT)
                    End If
                Next

            End If

            Return New_CSV_ATList

        Catch ex As Exception
            Return New List(Of S_AT)
        End Try

    End Function
    Function SaveATsToCSV(T_ATList As List(Of S_AT)) As Boolean

        Dim AT_CSV_FilePath As String = Application.StartupPath + "\ATs.CSV"

        If T_ATList.Count = 0 Then
            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, New List(Of String()),, "create")
            ATList = New List(Of S_AT)

            Return True

        End If

        Dim CSV_ATList As List(Of S_AT) = GetATsFromCSV()
        Dim New_CSV_ATList As List(Of S_AT) = New List(Of S_AT)

        For Each NEW_AT As S_AT In T_ATList

            Dim NewAT As Boolean = True
            For Each AT As S_AT In CSV_ATList

                If NEW_AT.AT = AT.AT Then
                    NewAT = False
                    Exit For
                End If

            Next

            If NewAT Then
                New_CSV_ATList.Add(NEW_AT)
            End If

        Next


        For Each CSV_AT As S_AT In CSV_ATList
            Dim T_AT As S_AT = CSV_AT

            For Each NEW_AT As S_AT In T_ATList
                If T_AT.AT = NEW_AT.AT Then

                    If T_AT.IsBLS_AT = NEW_AT.IsBLS_AT Then

                    Else
                        T_AT.IsBLS_AT = NEW_AT.IsBLS_AT
                    End If

                End If
            Next

            New_CSV_ATList.Add(T_AT)

        Next


        Dim CSVList As List(Of String()) = New List(Of String())
        For Each SAT As S_AT In New_CSV_ATList
            Dim Line As String() = {SAT.AT, SAT.ATRS, SAT.IsBLS_AT.ToString}
            CSVList.Add(Line)
        Next

        If CSVList.Count > 0 Then


            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, CSVList,, "create")

        End If

        Return True

    End Function

#End Region

#Region "Multithreadings"
    Sub ATCheckThread(ByVal input As Object)

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(DirectCast(input(1), String)) ' With {.C_Node = DirectCast(input(1), String)}

        Dim ATDetails = BCR.GetATDetails(DirectCast(input(0), String))
        Dim AT As String = BCR.BetweenFromList(ATDetails, "<at>", "</at>")
        Dim ATRS As String = BCR.BetweenFromList(ATDetails, "<atRS>", "</atRS>")
        Dim MachineCode As String = BCR.BetweenFromList(ATDetails, "<machineCode>", "</machineCode>")

        Application.DoEvents()

        Dim SAT As S_AT = New S_AT With {.AT = AT, .ATRS = ATRS}

        If ReferenceMachineCode.Trim = MachineCode.Trim Then
            SAT.IsBLS_AT = True
        Else
            SAT.IsBLS_AT = False
        End If


        ATList.Add(SAT)

    End Sub
    Sub BlockFeeThread(ByVal Node As Object)
        Dim BCR As ClsBurstAPI = New ClsBurstAPI(Node) ' With {.C_Node = Node}
        Block = BCR.GetCurrentBlock
        MultiInvoker(StatusBlockLabel, "Text", "New Blockheight: " + Block.ToString)

        Fee = BCR.GetSlotFee
        MultiInvoker(StatusFeeLabel, "Text", "Current Slotfee: " + String.Format("{0:#0.00000000}", Fee))

        UTXList = BCR.C_UTXList

    End Sub

#End Region

#Region "Multiinvoker"

    Delegate Sub MultiDelegate(ByVal params As Object)
    Sub MultiInvoker(ByVal obj As Object, ByVal prop As Object, ByVal val As Object)
        Try

            Dim paramList As List(Of Object) = New List(Of Object)

            paramList.Add(obj)
            paramList.Add(prop)
            paramList.Add(val)

            Me.Invoke(New MultiDelegate(AddressOf invoker), paramList)

        Catch ex As Exception
            'throw the error away
        End Try
    End Sub
    Sub invoker(ByVal params As List(Of Object))
        SetPropertyValueByName(params.Item(0), params.Item(1), params.Item(2))
    End Sub
    Public Function SetPropertyValueByName(obj As Object, name As String, value As Object) As Boolean

        Dim prop As Object = obj.GetType().GetProperty(name, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

        If IsNothing(prop) Then
            Return False
        End If

        If obj.GetType = GetType(ListView) And name = "Items" Then

            If value.Item(0) = "Clear" Then
                obj.Items.Clear()
            ElseIf value.Item(0) = "Add" Then
                Dim x = value.Item(1)
                obj.Items.Add(x)
            End If

            Return True
        Else
            If prop.CanWrite Then
                prop.SetValue(obj, value, Nothing)
                Return True
            End If
        End If

        Return False

    End Function


#End Region

#Region "Test"

    Private Sub BtTestCreate_Click(sender As Object, e As EventArgs) Handles BtTestCreate.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, "supertest") ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = "supertest"}

        Dim FeeNQT As ULong = BCR.Dbl2Planck(BCR.GetSlotFee)

        'AT: 2556199170550828612
        'createOrder: 716726961670769723
        'acceptOrder: 4714436802908501638
        'finishOrder: 3125596792462301675

        Dim ULngList As List(Of ULong) = New List(Of ULong)({716726961670769723, 3000000000, 100000000, BCR.String2ULng("EUR")})
        Dim MsgStr As String = BCR.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + "2556199170550828612" + "&amountNQT=" + "13000000000" + "&secretPhrase=" + BCR.C_PassPhrase.Trim + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = BCR.BurstRequest(postDataRL)

        Response = Response

    End Sub

    Private Sub BtTestAccept_Click(sender As Object, e As EventArgs) Handles BtTestAccept.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, "supertest2") ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = "supertest2"}

        Dim FeeNQT As ULong = BCR.Dbl2Planck(BCR.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({4714436802908501638})
        Dim MsgStr As String = BCR.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + "2556199170550828612" + "&amountNQT=" + "3100000000" + "&secretPhrase=" + BCR.C_PassPhrase.Trim + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = BCR.BurstRequest(postDataRL)

        Response = Response

    End Sub

    Private Sub BtTestFinish_Click(sender As Object, e As EventArgs) Handles BtTestFinish.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text, "supertest") ' With {.C_Node = CoBxNode.Text, .C_PassPhrase = "supertest"}

        Dim FeeNQT As ULong = BCR.Dbl2Planck(BCR.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({3125596792462301675})
        Dim MsgStr As String = BCR.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + "2556199170550828612" + "&amountNQT=" + "100000000" + "&secretPhrase=" + BCR.C_PassPhrase.Trim + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = BCR.BurstRequest(postDataRL)

        Response = Response

    End Sub

    Private Sub BtTestConvert_Click(sender As Object, e As EventArgs) Handles BtTestConvert.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}


        Try
            TBTestConvert.Text = BCR.String2ULng(TBTestConvert.Text)
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try


    End Sub

    Private Sub BtTestConvert2_Click(sender As Object, e As EventArgs) Handles BtTestConvert2.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}


        Try
            TBTestConvert.Text = BCR.ULng2String(TBTestConvert.Text)
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim BCR As ClsBurstAPI = New ClsBurstAPI(CoBxNode.Text) ' With {.C_Node = CoBxNode.Text}

        Dim DataLngList = BCR.DataStr2ULngList(TBTestConvert.Text)

        Dim DataStr As String = ""

        For Each DataLng In DataLngList
            DataStr += DataLng.ToString + " "
        Next


        TBTestConvert.Text = DataStr

    End Sub

    Private Sub CoBxNode_DropDownClosed(sender As Object, e As EventArgs) Handles CoBxNode.DropDownClosed

        If CoBxNode.Text <> olditem Then

            Dim MsgResult As msgs.CustomDialogResult = msgs.MBox("Nodechanges causes reload of the List of ATs" + vbCrLf + vbCrLf + "Do you really want to change the Node?", "Reload AT-List?", msgs.DefaultButtonMaker(msgs.DBList._YesNo),, msgs.Status.Question)

            If MsgResult = msgs.CustomDialogResult.Yes Then
                Dim wait = SaveATsToCSV(New List(Of S_AT))
                BlockTimer_Tick(90, Nothing)

            Else
                CoBxNode.SelectedItem = olditem
            End If
        End If

    End Sub


    Dim olditem As String = ""

    Private Sub CoBxNode_DropDown(sender As Object, e As EventArgs) Handles CoBxNode.DropDown
        olditem = CoBxNode.Text
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim beit() As Byte = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

        ListBox1.Items.Clear()

        For i As Integer = 0 To 1000



            Dim alicePrivate = Elliptic.Curve25519.ClampPrivateKey(beit)
            Dim alicePublic = Elliptic.Curve25519.GetPublicKey(alicePrivate)

            Dim bobPrivate = Elliptic.Curve25519.ClampPrivateKey(beit)
            Dim bobPublic = Elliptic.Curve25519.GetPublicKey(bobPrivate)

            Dim aliceShared = Elliptic.Curve25519.GetSharedSecret(alicePrivate, bobPublic)
            Dim bobShared = Elliptic.Curve25519.GetSharedSecret(bobPrivate, alicePublic)

            ListBox1.Items.Add("alicePrv: " + ByteAry2HEX(alicePrivate))
            ListBox1.Items.Add("alicePub: " + ByteAry2HEX(alicePublic))
            ListBox1.Items.Add("")
            ListBox1.Items.Add("bobPrv: " + ByteAry2HEX(bobPrivate))
            ListBox1.Items.Add("bobPub: " + ByteAry2HEX(bobPublic))
            ListBox1.Items.Add("")
            ListBox1.Items.Add("aliceShared: " + ByteAry2HEX(aliceShared))
            ListBox1.Items.Add("bobShared: " + ByteAry2HEX(bobShared))
            ListBox1.Items.Add("--------------------------")

        Next





    End Sub


    Function GetHash(Optional ByVal Salt As Integer = 0)

        Dim sha As New System.Security.Cryptography.SHA1CryptoServiceProvider
        Dim bytesToHash() As Byte
        bytesToHash = System.Text.Encoding.ASCII.GetBytes(Now.ToLongDateString + " " + Now.ToLongTimeString + " " + Salt.ToString)
        bytesToHash = sha.ComputeHash(bytesToHash)
        Dim encPassword As String = ""
        For Each b As Byte In bytesToHash
            encPassword += b.ToString("x2")
        Next
        encPassword = encPassword.Substring(encPassword.Length - 32)

        Return encPassword

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

    Private Sub BtPPAPI_Click(sender As Object, e As EventArgs) Handles BtTestPPAPI.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = TBPayPalAPIUser.Text
        PPAPI.Secret = TBPayPalAPISecret.Text

        'Dim h = PPAPI.CheckPayOuts

        Dim k As List(Of List(Of String)) = PPAPI.GetTransactionList("pom")




    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click


        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = TBPayPalAPIUser.Text
        PPAPI.Secret = TBPayPalAPISecret.Text

        Dim PPOrderID As List(Of String) = PPAPI.CreateOrder("Burst", 10, 12, "EUR")

        PPOrderID = PPOrderID

    End Sub

    Private Sub BtTestSetTXINI_Click(sender As Object, e As EventArgs) Handles BtTestSetTXINI.Click

        MsgBox(SetAutoinfoTX2INI(TBTestSetTXINI.Text).ToString)

    End Sub

    Private Sub BtTestGetTXINI_Click(sender As Object, e As EventArgs) Handles BtTestGetTXINI.Click

        MsgBox(GetAutoinfoTXFromINI(TBTestGetTXINI.Text).ToString)

    End Sub

    Private Sub BtTestDelTXINI_Click(sender As Object, e As EventArgs) Handles BtTestDelTXINI.Click

        MsgBox(DelAutoinfoTXFromINI(TBTestDelTXINI.Text).ToString)

    End Sub

#End Region

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

                Itemeigenschaft.Index = PFPForm.GetLVColNameFromSubItem(LV, SortItem(0))
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

        ' *** Keine Sortierung gewünscht ***
        If SortReihenfolge = SortOrder.None Then
            Return 0
        End If

        Try ' Damit Index-Fehler gefangen werden

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

        Catch ex As ArgumentOutOfRangeException ' Falls ein Index nicht im gültigen Bereich
            Return 0
        End Try

    End Function


    Function DateUSToGer(datum As String, Optional ByVal PlusTage As Integer = 0)

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

    End Function

End Class