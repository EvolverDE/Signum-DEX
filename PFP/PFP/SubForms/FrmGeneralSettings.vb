
Public Class FrmGeneralSettings

    Dim C_MainForm As PFPForm

    'Public Function GetPayTypes() As List(Of String)
    '    Dim k = System.Enum.GetNames(GetType(ClsOrderSettings.E_PayType)).ToList

    '    Return New List(Of String)({ClsOrderSettings.E_PayType.Bankaccount.ToString, ClsOrderSettings.E_PayType.PayPal_E_Mail.ToString.Replace("_", "-"), ClsOrderSettings.E_PayType.PayPal_Order.ToString.Replace("_", "-"), ClsOrderSettings.E_PayType.Self_Pickup.ToString.Replace("_", " "), ClsOrderSettings.E_PayType.Other.ToString})
    'End Function

    Sub New(ByVal MainForm As PFPForm)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_MainForm = MainForm

        CoBxPayType.Items.Clear()
        CoBxPayType.Items.AddRange(ClsOrderSettings.GetPayTypes.ToArray)

    End Sub



    Private Sub ChBxTCPAPI_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTCPAPI.CheckedChanged

        If ChBxTCPAPI.Checked Then
            If Not C_MainForm.TCPAPI.AlreadyStarted Then
                C_MainForm.TCPAPI.StartAPIServer()
            End If
        Else
            C_MainForm.TCPAPI.StopAPIServer()
        End If

        SetINISetting(E_Setting.TCPAPIEnable, ChBxTCPAPI.Checked)

    End Sub


    Private Sub FrmGeneralSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        C_MainForm.PrimaryNode = GetINISetting(E_Setting.DefaultNode, "http://nivbox.co.uk:6876/burst")

        CoBxRefresh.SelectedItem = GetINISetting(E_Setting.RefreshMinutes, "1")

        Dim Nodes As String = GetINISetting(E_Setting.Nodes, "http://nivbox.co.uk:6876/burst;https://testnet.burstcoin.network:6876/burst") 'http://nivbox.co.uk:6876/burst;https://testnet.burstcoin.network:6876/burst;https://octalsburstnode.ddns.net:6876/burst;https://testnetwallet.burstcoin.ro/burst

        Dim NodeList As List(Of String) = New List(Of String)
        If Nodes.Contains(";") Then
            NodeList.AddRange(Nodes.Split(";"))
        Else
            NodeList.Add(Nodes)
        End If
        CoBxNode.Items.Clear()
        CoBxNode.Items.AddRange(NodeList.ToArray)


        CoBxNode.SelectedItem = GetINISetting(E_Setting.DefaultNode, "http://nivbox.co.uk:6876/burst")
        'PrimaryNode = CoBxNode.SelectedItem

        If CoBxNode.Text.Trim = "" Then
            CoBxNode.SelectedItem = CoBxNode.Items(0)
            'PrimaryNode = CoBxNode.Items(0)
        End If

        'RefreshTime = CInt(CoBxRefresh.Text) * 600

        ChBxAutoSendPaymentInfo.Checked = GetINISetting(E_Setting.AutoSendPaymentInfo, False)
        ChBxCheckXItemTX.Checked = GetINISetting(E_Setting.AutoCheckAndFinishAT, False)


        ChBxTCPAPI.Checked = GetINISetting(E_Setting.TCPAPIEnable, False)
        TBTCPAPIPort.Text = GetINISetting(E_Setting.TCPAPIServerPort, 8130)
        TBDEXNETPort.Text = GetINISetting(E_Setting.DEXNETServerPort, 8131)

        TBPaymentInfo.Text = GetINISetting(E_Setting.PaymentType, "Self Pickup")
        TBPaymentInfo.Text = GetINISetting(E_Setting.PaymentInfoText, "Infotext")

        Dim RBs As String = GetINISetting(E_Setting.PayPalChoice, "EMail")

        If RBs.Trim = "EMail" Then
            RBPayPalEMail.Checked = True
            'ElseIf RBs.Trim = "AccountID" Then
            '    RBPayPalAccID.Checked = True
        ElseIf RBs.Trim = "Order" Then
            RBPayPalOrder.Checked = True
        Else
            RBPayPalEMail.Checked = True
        End If

        TBPayPalEMail.Text = GetINISetting(E_Setting.PayPalEMail, "test@test.com")

        TBPayPalAPIUser.Text = GetINISetting(E_Setting.PayPalAPIUser, "1234")
        TBPayPalAPISecret.Text = GetINISetting(E_Setting.PayPalAPISecret, "abcd")

        'If TBSNOPassPhrase.Text.Trim = "" Then
        '    BlockTimer.Enabled = False
        '    Exit Sub
        'End If

        'Test PayPal Business API
        'If Not CheckPayPalAPI() = "True" Then
        '    RBPayPalEMail.Checked = True
        '    RBPayPalOrder.Checked = False
        '    RBPayPalOrder.Enabled = False
        'End If


        Dim NodesStr As String = C_MainForm.PrimaryNode + ";"
        NodeList.Remove(C_MainForm.PrimaryNode)
        CoBxNode.Items.Clear()
        CoBxNode.Items.Add(C_MainForm.PrimaryNode)

        For Each Nod As String In NodeList
            CoBxNode.Items.Add(Nod)
            NodesStr += Nod + ";"
        Next

        CoBxNode.SelectedItem = C_MainForm.PrimaryNode

        NodesStr = NodesStr.Remove(NodesStr.Length - 1)

        SetINISetting(E_Setting.Nodes, NodesStr)

    End Sub

    Private Sub FrmGeneralSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim Changes As Boolean = False

        'If C_MainForm.TBSNOPassPhrase.Text <> GetINISetting(E_Setting.PassPhrase, "") Then
        '    Changes = True
        'End If

        If C_MainForm.CoBxMarket.SelectedItem <> GetINISetting(E_Setting.LastMarketViewed, "USD") Then
            Changes = True
        End If

        If CoBxRefresh.SelectedItem <> GetINISetting(E_Setting.RefreshMinutes, "1") Then
            Changes = True
        End If

        If CoBxNode.SelectedItem <> GetINISetting(E_Setting.DefaultNode, "http://nivbox.co.uk:6876/burst") Then
            Changes = True
        End If


        If ChBxAutoSendPaymentInfo.Checked <> GetINISetting(E_Setting.AutoSendPaymentInfo, False) Then
            Changes = True
        End If

        If ChBxCheckXItemTX.Checked <> GetINISetting(E_Setting.AutoCheckAndFinishAT, False) Then
            Changes = True
        End If

        'If RBUseMyOrdersSettings.Checked <> GetINISetting(E_Setting.UsePayPalSettings, False) Then
        '    Changes = True
        'End If

        If ChBxTCPAPI.Checked <> GetINISetting(E_Setting.TCPAPIEnable, False) Then
            Changes = True
        End If

        If TBTCPAPIPort.Text <> GetINISetting(E_Setting.TCPAPIServerPort, 8130) Then
            Changes = True
        End If

        If TBDEXNETPort.Text <> GetINISetting(E_Setting.DEXNETServerPort, 8131) Then
            Changes = True
        End If


        If TBPaymentInfo.Text <> GetINISetting(E_Setting.PaymentInfoText, "Infotext") Then
            'Changes = True
        End If

        If CoBxPayType.SelectedItem <> GetINISetting(E_Setting.PaymentType, "Self Pickup") Then

        End If

        Dim RBs As String = ""
        If RBPayPalEMail.Checked Then
            RBs = "EMail"
        ElseIf RBPayPalOrder.Checked Then
            RBs = "Order"
        Else
            RBs = "EMail"
        End If

        If RBs <> GetINISetting(E_Setting.PayPalChoice, "EMail") Then
            Changes = True
        End If


        If TBPayPalEMail.Text <> GetINISetting(E_Setting.PayPalEMail, "test@test.com") Then
            Changes = True
        End If


        If TBPayPalAPIUser.Text <> GetINISetting(E_Setting.PayPalAPIUser, "1234") Then
            Changes = True
        End If

        If TBPayPalAPISecret.Text <> GetINISetting(E_Setting.PayPalAPISecret, "abcd") Then
            Changes = True
        End If



        If Changes Then

            Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("There are changes in the settings, overwrite it?", "Settings changed", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

            If Result = ClsMsgs.CustomDialogResult.Yes Then

                'SetINISetting(E_Setting.PassPhrase, C_MainForm.TBSNOPassPhrase.Text)
                SetINISetting(E_Setting.LastMarketViewed, C_MainForm.CurrentMarket)
                SetINISetting(E_Setting.RefreshMinutes, CoBxRefresh.Text)
                SetINISetting(E_Setting.DefaultNode, C_MainForm.PrimaryNode)

                SetINISetting(E_Setting.AutoSendPaymentInfo, ChBxAutoSendPaymentInfo.Checked)
                SetINISetting(E_Setting.AutoCheckAndFinishAT, ChBxCheckXItemTX.Checked)

                SetINISetting(E_Setting.DEXNETServerPort, TBDEXNETPort.Text)
                SetINISetting(E_Setting.TCPAPIServerPort, TBTCPAPIPort.Text)
                SetINISetting(E_Setting.TCPAPIEnable, ChBxTCPAPI.Checked)

                SetINISetting(E_Setting.PaymentType, CoBxPayType.SelectedItem)
                SetINISetting(E_Setting.PaymentInfoText, TBPaymentInfo.Text.Trim)

                SetINISetting(E_Setting.PayPalChoice, RBs)

                SetINISetting(E_Setting.PayPalEMail, TBPayPalEMail.Text)

                SetINISetting(E_Setting.PayPalAPIUser, TBPayPalAPIUser.Text)
                SetINISetting(E_Setting.PayPalAPISecret, TBPayPalAPISecret.Text)


            End If

        End If

    End Sub

    Private Sub BtSaveSettings_Click(sender As Object, e As EventArgs) Handles BtSaveSettings.Click

        'SetINISetting(E_Setting.PassPhrase, C_MainForm.TBSNOPassPhrase.Text)
        SetINISetting(E_Setting.LastMarketViewed, C_MainForm.CurrentMarket)
        SetINISetting(E_Setting.RefreshMinutes, CoBxRefresh.Text)
        SetINISetting(E_Setting.DefaultNode, C_MainForm.PrimaryNode)

        Dim Nodes As String = ""

        For Each Nod As String In CoBxNode.Items
            Nodes += Nod + ";"
        Next

        Nodes = Nodes.Remove(Nodes.Length - 1)

        SetINISetting(E_Setting.Nodes, Nodes)


        SetINISetting(E_Setting.AutoSendPaymentInfo, ChBxAutoSendPaymentInfo.Checked)
        SetINISetting(E_Setting.AutoCheckAndFinishAT, ChBxCheckXItemTX.Checked)

        SetINISetting(E_Setting.DEXNETServerPort, TBDEXNETPort.Text)
        SetINISetting(E_Setting.TCPAPIServerPort, TBTCPAPIPort.Text)
        SetINISetting(E_Setting.TCPAPIEnable, ChBxTCPAPI.Checked)

        SetINISetting(E_Setting.PaymentType, CoBxPayType.SelectedItem)
        SetINISetting(E_Setting.PaymentInfoText, TBPaymentInfo.Text.Trim)

        Dim RBs As String = ""
        If RBPayPalEMail.Checked Then
            RBs = "EMail"
            'ElseIf RBPayPalAccID.Checked Then
            '    RBs = "AccountID"
        ElseIf RBPayPalOrder.Checked Then
            RBs = "Order"
        Else
            RBs = "Order"
        End If

        SetINISetting(E_Setting.PayPalChoice, RBs)

        SetINISetting(E_Setting.PayPalEMail, TBPayPalEMail.Text)

        SetINISetting(E_Setting.PayPalAPIUser, TBPayPalAPIUser.Text)
        SetINISetting(E_Setting.PayPalAPISecret, TBPayPalAPISecret.Text)

    End Sub

    Private Sub RBPayPalEMail_CheckedChanged(sender As Object, e As EventArgs)

        Dim PaymentInfo As String = ""

        If RBPayPalEMail.Checked Then

            TBPayPalEMail.Enabled = True
            PaymentInfo = TBPayPalEMail.Text

        ElseIf RBPayPalOrder.Checked Then

            TBPayPalEMail.Enabled = False
            PaymentInfo = ""

        End If

    End Sub

    Private Sub BtCheckPayPalBiz_Click(sender As Object, e As EventArgs) Handles BtCheckPayPalBiz.Click

        Dim Status As String = CheckPayPalAPI()

        If Status = "True" Then
            RBPayPalOrder.Enabled = True
            ClsMsgs.MBox("PayPal Business OK", "Checking PayPal-API",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
        Else
            RBPayPalEMail.Checked = True
            RBPayPalOrder.Checked = False
            RBPayPalOrder.Enabled = False

            ClsMsgs.MBox(Status, "Fail",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)

        End If

    End Sub


    Private Sub CoBxNode_DropDownClosed(sender As Object, e As EventArgs)

        If CoBxNode.Text <> olditem Then

            Dim MsgResult As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("Nodechanges causes reload of the List of ATs" + vbCrLf + vbCrLf + "Do you really want to change the Primary-Node?", "Reload AT-List?", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList._YesNo),, ClsMsgs.Status.Question)

            If MsgResult = ClsMsgs.CustomDialogResult.Yes Then
                'Dim wait = SaveATsToCSV(New List(Of S_AT))
                'BlockTimer_Tick(True, Nothing)

            Else
                CoBxNode.SelectedItem = olditem
                'PrimaryNode = CoBxNode.SelectedItem
            End If
        End If

    End Sub


    Dim olditem As String = ""

    Private Sub CoBxNode_DropDown(sender As Object, e As EventArgs)
        olditem = CoBxNode.Text
    End Sub


    Private Sub TBPayPalEMail_KeyPress(sender As Object, e As KeyPressEventArgs)

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

    Private Sub TBTCPAPIPort_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBTCPAPIPort.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)
        Dim TBx As TextBox = DirectCast(sender, TextBox)

        Select Case keys
            Case 48 To 57, 8
                ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                SetINISetting(E_Setting.TCPAPIServerPort, TBTCPAPIPort.Text)
                ChBxTCPAPI.Checked = False
                ChBxTCPAPI.Checked = True

                e.Handled = True
            Case Else
                ' alle anderen Eingaben unterdrücken
                e.Handled = True
        End Select

    End Sub

    Private Sub TBDEXNETPort_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBDEXNETPort.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Select Case keys
            Case 48 To 57, 8
                ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                SetINISetting(E_Setting.DEXNETServerPort, TBDEXNETPort.Text)
                If Not IsNothing(C_MainForm.DEXNET) Then
                    C_MainForm.DEXNET.StopServer()
                    C_MainForm.InitiateDEXNET()
                Else
                    C_MainForm.InitiateDEXNET()
                End If

                e.Handled = True
            Case Else
                ' alle anderen Eingaben unterdrücken
                e.Handled = True
        End Select

    End Sub


End Class