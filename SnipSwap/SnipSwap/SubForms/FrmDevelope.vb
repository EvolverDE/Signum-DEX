Option Strict On
Option Explicit On

Public Class FrmDevelope

    Dim C_SignumAPI As ClsSignumAPI = Nothing ' New ClsSignumAPI(C_MainForm.PrimaryNode)
    Dim C_MainForm As SnipSwapForm = CType(Me.ParentForm, SnipSwapForm)
    Dim SpecialTimer As Integer = 0

    Private Property CurrentContract As ClsDEXContract = Nothing

    Private Property DEXContractList As List(Of ClsDEXContract) = New List(Of ClsDEXContract)
    Private Property T_DEXContractList As List(Of List(Of String)) = New List(Of List(Of String))

    Sub New(ByVal MainForm As SnipSwapForm)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_MainForm = MainForm
        C_SignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        T_DEXContractList = GetDEXContractsFromCSV(False)

        For Each T_DEX As List(Of String) In T_DEXContractList
            Dim T_Contract = New ClsDEXContract(MainForm, MainForm.PrimaryNode, ULong.Parse(T_DEX(0)))
            DEXContractList.Add(T_Contract)
        Next

        CoBxTestATComATID.Items.Clear()

        For Each x In DEXContractList
            CoBxTestATComATID.Items.Add(x.Address)
        Next

        If CoBxTestATComATID.Items.Count > 0 Then
            CoBxTestATComATID.SelectedItem = CoBxTestATComATID.Items(0)
            CurrentContract = DEXContractList(0)
        End If

    End Sub
    Private Sub FrmDevelope_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CoBxTestATComATID.Items.Clear()

        For Each T_DEX As List(Of String) In T_DEXContractList
            'If T_DEX(1) = "True" Then
            '    DEXSmartContractList.Add(T_DEX(0))
            'End If

            'Dim DEXAT As String = C_MainForm.DEXSmartContractList(i)
            CoBxTestATComATID.Items.Add(ClsReedSolomon.Encode(ULong.Parse(T_DEX(0))))

        Next

        'For i As Integer = 0 To C_MainForm.DEXSmartContractList.Count - 1
        '    Dim DEXAT As String = C_MainForm.DEXSmartContractList(i)
        '    CoBxTestATComATID.Items.Add(ClsReedSolomon.Encode(ULong.Parse(DEXAT)))
        'Next

        If CoBxTestATComATID.Items.Count > 0 Then
            CoBxTestATComATID.SelectedIndex = 0
        End If


        'TBTestMyPublicKey.Text = C_MainForm.T_PublicKeyHEX
        'TBTestMySignKey.Text = C_MainForm.T_SignkeyHEX
        'TBTestMyAgreeKey.Text = C_MainForm.T_AgreementKeyHEX
        'TBTestMyPassPhrase.Text = C_MainForm.T_PassPhrase


        'TBTestATID.Text = ClsSignumAPI._ReferenceTX.ToString
        ChBxTestDEXNETEnable.Checked = GetINISetting(E_Setting.DEXNETEnable, True)
        ChBxTestDEXNETShowStatus.Checked = GetINISetting(E_Setting.DEXNETShowStatus, False)

        Dim PPAPI As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork)
        CoBxTestPPCurrency.Items.AddRange(PPAPI.Currencys.ToArray)

        CoBxTestPPCurrency.SelectedItem = "USD"

    End Sub
    Private Sub DevTimer_Tick(sender As Object, e As EventArgs) Handles DevTimer.Tick

        If Not C_MainForm.DEXNET Is Nothing Then

            Dim DEXNETStatusMsgs As List(Of String) = C_MainForm.DEXNET.GetStatusMessages

            If DEXNETStatusMsgs.Count > 0 Then
                LiBoDEXNETStatus.Items.AddRange(DEXNETStatusMsgs.ToArray)
            End If

            Dim RelKeys As List(Of ClsDEXNET.S_RelevantKey) = C_MainForm.DEXNET.GetRelevantKeys
            Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = C_MainForm.DEXNET.GetRelevantMsgs

            If RelKeys.Count <> LiBoTestRelKeys.Items.Count Then
                LiBoTestRelKeys.Items.Clear()
            End If

            For Each RelKey As ClsDEXNET.S_RelevantKey In RelKeys

                Dim FoundedKey As Boolean = False

                For Each LiBoRelKey As String In LiBoTestRelKeys.Items
                    Dim T_Key As String = LiBoRelKey.Remove(LiBoRelKey.IndexOf("("))
                    Dim T_SubKey As String = GetStringBetween(LiBoRelKey, "(", ")")
                    If T_Key = RelKey.Name And T_SubKey = RelKey.Different Then
                        FoundedKey = True
                        Exit For
                    End If
                Next

                If Not FoundedKey Then
                    LiBoTestRelKeys.Items.Add(RelKey.Name + "(" + RelKey.Different + ")")
                End If

            Next


            SetRefreshListsInListBox(LiBoTestRelMsgs, GetRefreshLists(RelMsgs.Select(Function(msg) msg.RelevantMessage).ToList(), GetListBoxItems(LiBoTestRelMsgs)))

            'Dim Empty_CNT As Integer = 0
            'Dim CNT As Integer = 0
            'For Each x As String In LiBoTestRelMsgs.Items
            '    If x.Trim = "" Then
            '        Empty_CNT += 1
            '    Else
            '        CNT += 1
            '    End If
            'Next

            'Dim Empty_CNT1 As Integer = 0
            'Dim CNT1 As Integer = 0
            'For Each x As ClsDEXNET.S_RelevantMessage In RelMsgs
            '    If x.RelevantMessage.Trim = "" Then
            '        Empty_CNT1 += 1
            '    Else
            '        CNT += 1
            '    End If
            'Next

            ''If Empty_CNT <> Empty_CNT1 And CNT <> CNT1 Then
            'LiBoTestRelMsgs.Items.Clear()

            ''    For i As Integer = 0 To Empty_CNT1 - 1
            ''        LiBoTestRelMsgs.Items.Add("")
            ''    Next

            ''End If

            'For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

            '    'If RelMsg.RelevantMessage.Trim = "" Then
            '    '    Continue For
            '    'End If

            '    'Dim FoundedMsg As Boolean = False
            '    'For Each LiBoRelMsg As String In LiBoTestRelMsgs.Items

            '    '    If LiBoRelMsg.Trim = "" Then
            '    '        Continue For
            '    '    End If

            '    '    If LiBoRelMsg.Contains("<PublicKey>") Then

            '    '        Dim T_Trigger As String = GetStringBetween(LiBoRelMsg, "<PublicKey>", "</PublicKey>")

            '    '        If RelMsg.RelevantMessage.Contains(T_Trigger) Then
            '    '            FoundedMsg = True
            '    '            Exit For
            '    '        End If

            '    '    End If

            '    'Next

            '    'If Not FoundedMsg Then
            '    LiBoTestRelMsgs.Items.Add(RelMsg.RelevantMessage)
            '    'End If

            'Next


            Dim Peers As List(Of ClsDEXNET.S_Peer) = C_MainForm.DEXNET.GetPeers
            SetRefreshListsInListBox(LiBoTestPeers, GetRefreshLists(Peers.Select(Function(pe) pe.PossibleHost + ":" + pe.Port.ToString()).ToList(), GetListBoxItems(LiBoTestPeers)))
            SetRefreshListsInListBox(LiBoDEXNETPublicKeys, GetRefreshLists(Peers.Select(Function(pe) pe.PublicKey).ToList(), GetListBoxItems(LiBoDEXNETPublicKeys)))


            'LiBoTestPeers.Items.Clear()
            'For Each Peer As ClsDEXNET.S_Peer In Peers
            '    LiBoTestPeers.Items.Add(Peer.PossibleHost + ":" + Peer.Port.ToString)
            'Next

        End If

        If ChBxAutoRefreshMultiThreads.Checked Then

            If SpecialTimer >= 10 Then
                SpecialTimer = 0


                Dim ActiveNodes As Integer = C_MainForm.NodeList.Count

                LabActiveNodes.Text = ActiveNodes.ToString
                LVActiveNodes.Items.Clear()

                For i As Integer = 0 To C_MainForm.NodeList.Count - 1
                    Dim NodeName As String = C_MainForm.NodeList(i)
                    LVActiveNodes.Items.Add(NodeName)
                Next



                Dim APIReqList As List(Of SnipSwapForm.S_APIRequest) = New List(Of SnipSwapForm.S_APIRequest)(C_MainForm.APIRequestList.ToArray)

                Dim LVIList As List(Of ListViewItem) = New List(Of ListViewItem)


                For i As Integer = 0 To APIReqList.Count - 1
                    Dim APIRequest As SnipSwapForm.S_APIRequest = APIReqList(i)

                    Dim T_LVI As ListViewItem = New ListViewItem
                    T_LVI.Tag = APIRequest.Command
                    T_LVI.Text = APIRequest.Node
                    T_LVI.SubItems.Add(APIRequest.Command) 'Command

                    If Not APIRequest.RequestThread Is Nothing Then
                        T_LVI.SubItems.Add(APIRequest.RequestThread.ManagedThreadId.ToString) 'ThreadID
                    Else
                        T_LVI.SubItems.Add("no Thread")
                    End If

                    T_LVI.SubItems.Add(APIRequest.Status) 'Status
                    If APIRequest.Status = "Ready" Then

                        If Not APIRequest.Result Is Nothing Then
                            T_LVI.SubItems.Add(APIRequest.Result.ToString) 'Result
                        Else
                            T_LVI.SubItems.Add("Nothing") 'Result
                        End If

                    Else
                        T_LVI.SubItems.Add("no Result")
                    End If

                    LVIList.Add(T_LVI)

                Next


                If LVTestMulti.Items.Count = 0 Then
                    LVTestMulti.Items.AddRange(LVIList.ToArray)
                End If



                For i As Integer = 0 To LVTestMulti.Items.Count - 1
                    Dim T_LVITest As ListViewItem = LVTestMulti.Items(i)

                    For ii As Integer = 0 To LVIList.Count - 1
                        Dim T_LVI As ListViewItem = LVIList(ii)

                        If T_LVITest.Tag.ToString = T_LVI.Tag.ToString Then

                            Dim Refresh As Boolean = False

                            For j As Integer = 0 To T_LVI.SubItems.Count - 1

                                If T_LVITest.SubItems(j).Text <> T_LVI.SubItems(j).Text Then
                                    T_LVITest.SubItems(j).Text = T_LVI.SubItems(j).Text
                                    Refresh = True
                                End If

                            Next

                            If Refresh Then
                                LVTestMulti.Items(i) = T_LVITest
                            End If

                        End If

                    Next


                Next

            End If

            SpecialTimer += 1

        End If

    End Sub


    Private Function GetListBoxItems(ByVal LiBo As ListBox) As List(Of String)

        Dim CurrentList As List(Of String) = New List(Of String)
        For Each Entry As String In LiBo.Items
            CurrentList.Add(Entry)
        Next

        Return CurrentList

    End Function
    Private Function GetRefreshLists(ByVal NewList As List(Of String), ByVal CurrentList As List(Of String)) As List(Of List(Of String))

        Dim AddList As List(Of String) = NewList.Where(Function(li) Not CurrentList.Contains(li) And li.Trim() <> "").ToList()
        If CurrentList.Count = 0 Then
            Return New List(Of List(Of String))({New List(Of String), AddList})
        End If

        Dim DeleteList As List(Of String) = CurrentList.Where(Function(p) Not AddList.Contains(p) And Not NewList.Contains(p)).ToList()

        Return New List(Of List(Of String))({DeleteList, AddList})

    End Function
    Private Sub SetRefreshListsInListBox(ByVal LiBo As ListBox, ByVal RefreshLists As List(Of List(Of String)))

        Dim CurrentList As List(Of String) = GetListBoxItems(LiBo)

        For Each Del As String In RefreshLists(0)
            LiBo.Items.Remove(Del)
        Next

        For Each add As String In RefreshLists(1)
            LiBo.Items.Add(add)
        Next

    End Sub


#Region "Convertings"

    Private Sub BtTestDatStr2ULngList_Click(sender As Object, e As EventArgs) Handles BtTestDatStr2ULngList.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({ClsSignumAPI.String2ULng(TBTestConvert.Text.Trim)})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)

        TBTestConvert.Text = MsgStr

    End Sub
    Private Sub BtTestConvert_Click(sender As Object, e As EventArgs) Handles BtTestConvert.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.String2ULng(TBTestConvert.Text).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub
    Private Sub BtTestConvert2_Click(sender As Object, e As EventArgs) Handles BtTestConvert2.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.ULng2String(Convert.ToUInt64(TBTestConvert.Text)).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub
    Private Sub BtTestTimeConvert_Click(sender As Object, e As EventArgs) Handles BtTestTimeConvert.Click
        ' Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)
        TBTestTime.Text = ClsSignumAPI.UnixToTime((139296583).ToString).ToString
    End Sub
    Private Sub BtTestCreateCollWord_Click(sender As Object, e As EventArgs) Handles BtTestCreateCollWord.Click

        Dim ColWord As ClsColloquialWords = New ClsColloquialWords

        If TBTestCollWordInput.Text.Trim = "" Then
            TBTestCollWordOutput.Text = ColWord.RandomWord
        Else
            TBTestCollWordOutput.Text = ColWord.GenerateColloquialWords(TBTestCollWordInput.Text.Trim, True, "-", 6)
        End If

    End Sub
    Private Sub BtTestJSONToXML_Click(sender As Object, e As EventArgs) Handles BtTestJSONToXML.Click
        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(TBTestJSONInput.Text, ClsJSONAndXMLConverter.E_ParseType.JSON)
        TBTestXMLOutput.Text = Converter.XMLString
    End Sub

    Private Sub BtTestRecursiveXMLSearch_Click(sender As Object, e As EventArgs) Handles BtTestRecursiveXMLSearch.Click
        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(TBTestJSONInput.Text, ClsJSONAndXMLConverter.E_ParseType.JSON)
        TBTestXMLOutput.Text = Converter.FirstValue(TBTestRecursiveXMLSearch.Text).ToString()
    End Sub

#End Region

#Region "INI-Tools tests"

    Private Sub BtTestSetTXINI_Click(sender As Object, e As EventArgs) Handles BtTestSetTXINI.Click
        MsgBox(C_MainForm.SetAutoinfoTX2INI(Convert.ToUInt64(TBTestSetTXINI.Text)).ToString)
    End Sub
    Private Sub BtTestGetTXINI_Click(sender As Object, e As EventArgs) Handles BtTestGetTXINI.Click
        MsgBox(C_MainForm.GetAutoinfoTXFromINI(Convert.ToUInt64(TBTestGetTXINI.Text)).ToString)
    End Sub
    Private Sub BtTestDelTXINI_Click(sender As Object, e As EventArgs) Handles BtTestDelTXINI.Click
        MsgBox(C_MainForm.DelAutoinfoTXFromINI(Convert.ToUInt64(TBTestDelTXINI.Text)).ToString)
    End Sub

#End Region

#Region "PayPal Tests"

    Private Sub BtTestPPCheckAPI_Click(sender As Object, e As EventArgs) Handles BtTestPPCheckAPI.Click

        Dim Status As String = CheckPayPalAPI()

        LiBoPayPalComs.Items.Add(Status)

        'If Status = "True" Then
        '    ClsMsgs.MBox("PayPal Business OK", "Checking PayPal-API",,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
        'Else
        '    ClsMsgs.MBox(Status, "Fail",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
        'End If

    End Sub
    Private Sub BtPPAPI_Click(sender As Object, e As EventArgs) Handles BtTestPPGetAllTX.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork)
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

        Dim TXList As List(Of List(Of String)) = PPAPI.GetTransactionList("")

        LiBoPayPalComs.Items.Clear()

        For Each TX As List(Of String) In TXList
            For Each SubTX In TX
                LiBoPayPalComs.Items.Add(SubTX)
            Next
        Next

    End Sub
    Private Sub BtTestPPGetTXWithNote_Click(sender As Object, e As EventArgs) Handles BtTestPPGetTXWithNote.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork)
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

        Dim TXList As List(Of List(Of String)) = PPAPI.GetTransactionList(TBTestPPTXNote.Text)

        LiBoPayPalComs.Items.Clear()

        For Each TX As List(Of String) In TXList
            For Each SubTX In TX
                LiBoPayPalComs.Items.Add(SubTX)
            Next
        Next

    End Sub
    Private Sub BtTestCreatePPOrder_Click(sender As Object, e As EventArgs) Handles BtTestCreatePPOrder.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork)
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

        Dim PPOrderID As List(Of String) = PPAPI.CreateOrder(TBTestPPXItem.Text, Double.Parse(TBTestPPXAmount.Text), Double.Parse(TBTestPPPrice.Text), CoBxTestPPCurrency.SelectedItem.ToString)

        LiBoPayPalComs.Items.Clear()

        For Each TX As String In PPOrderID
            LiBoPayPalComs.Items.Add(TX)
        Next

    End Sub

    Private Sub BtTestPPPayout_Click(sender As Object, e As EventArgs) Handles BtTestPPPayout.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal(GlobalPayPalNetwork)
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

        PPAPI.CreateBatchPayOut(TBTestPPPORecipient.Text, Double.Parse(TBTestPPPOAmount.Text), TBTestPPPOCurrency.Text, TBTestPPPONote.Text)

    End Sub


    Private Sub LiBoPayPalComs_DoubleClick(sender As Object, e As EventArgs) Handles LiBoPayPalComs.DoubleClick

        If Not LiBoPayPalComs.SelectedItem Is Nothing Then

            Dim T_Frm As Form = New Form With {.Name = "FrmMessage", .Text = "FrmMessage", .StartPosition = FormStartPosition.CenterScreen}
            Dim RTB As RichTextBox = New RichTextBox
            RTB.Dock = DockStyle.Fill
            RTB.AppendText(LiBoPayPalComs.SelectedItem.ToString)
            T_Frm.Controls.Add(RTB)
            T_Frm.Show()

        End If

    End Sub


#End Region

#Region "AT Communications"

    Private Sub BtTestDeActivateDeniability_Click(sender As Object, e As EventArgs) Handles BtTestDeActivateDeniability.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.DeActivateDeniability(Masterkeys(0),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestCreateWithResponder_Click(sender As Object, e As EventArgs) Handles BtTestCreateWithResponder.Click

        If Not CurrentContract Is Nothing Then

            Dim Collateral As Double = Convert.ToDouble(NUDTestCollateral.Value)
            Dim Amount As Double = Convert.ToDouble(NUDTestAmount.Value)

            Dim XAmount As Double = Convert.ToDouble(NUDTestXAmount.Value)
            Dim Price As Double = XAmount / Convert.ToDouble(NUDTestAmount.Value)

            Dim Recipient As ULong = Convert.ToUInt64(TB_TestCreateWithResponder.Text)

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.CreateOrderWithResponder(Masterkeys(0), Amount, Recipient, TBTestXItem.Text, XAmount,, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

        End If

    End Sub

    Private Sub BtTestCreate_Click(sender As Object, e As EventArgs) Handles BtTestCreate.Click

        If Not CurrentContract Is Nothing Then

            Dim Collateral As Double = Convert.ToDouble(NUDTestCollateral.Value)
            Dim Amount As Double = Convert.ToDouble(NUDTestAmount.Value)

            Dim XAmount As Double = Convert.ToDouble(NUDTestXAmount.Value)
            Dim Price As Double = XAmount / Convert.ToDouble(NUDTestAmount.Value)

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = ""

            If ChBxSellOrder.Checked Then
                TXID = CurrentContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, TBTestXItem.Text, XAmount,, Masterkeys(1))
            Else
                TXID = CurrentContract.CreateBuyOrder(Masterkeys(0), Amount, Collateral, TBTestXItem.Text, XAmount,, Masterkeys(1))
            End If

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With

        End If
    End Sub

    Private Sub BtTestAccept_Click(sender As Object, e As EventArgs) Handles BtTestAccept.Click

        If Not CurrentContract Is Nothing Then

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = ""

            If CurrentContract.IsSellOrder Then
                TXID = CurrentContract.AcceptSellOrder(Masterkeys(0),,, Masterkeys(1))
            Else
                TXID = CurrentContract.AcceptBuyOrder(Masterkeys(0),,,, Masterkeys(1))
            End If

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If

    End Sub

    Private Sub BtTestInjectResponder_Click(sender As Object, e As EventArgs) Handles BtTestInjectResponder.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.InjectResponder(Masterkeys(0), ULong.Parse(TBTestResponder.Text),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestOpenDispute_Click(sender As Object, e As EventArgs) Handles BtTestOpenDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.OpenDispute(Masterkeys(0),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestMediateDispute_Click(sender As Object, e As EventArgs) Handles BtTestMediateDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            Dim TXID As String = CurrentContract.MediateDispute(Masterkeys(0), NUDTestMediateAmount.Value,, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestAppeal_Click(sender As Object, e As EventArgs) Handles BtTestAppeal.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.Appeal(Masterkeys(0),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestCheckCloseDispute_Click(sender As Object, e As EventArgs) Handles BtTestCheckCloseDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.CheckCloseDispute(Masterkeys(0),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestFinish_Click(sender As Object, e As EventArgs) Handles BtTestFinish.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.FinishOrder(Masterkeys(0),, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With
        End If
    End Sub

    Private Sub BtTestInjectChainSwapHash_Click(sender As Object, e As EventArgs) Handles BtTestInjectChainSwapHash.Click

        If Not CurrentContract Is Nothing Then

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            If TBTestChainSwapHashLong1.Text.Trim = "" Then
                Exit Sub
            End If

            Dim TXID As String = CurrentContract.InjectChainSwapHash(Masterkeys(0), TBTestChainSwapHashLong1.Text,, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With

        End If
    End Sub

    Private Sub BtTestFinishWithChainSwapKey_Click(sender As Object, e As EventArgs) Handles BtTestFinishWithChainSwapKey.Click
        If Not CurrentContract Is Nothing Then

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            If TBTestChainSwapKeyULong1.Text.Trim = "" Or TBTestChainSwapKeyULong2.Text.Trim = "" Or TBTestChainSwapKeyULong3.Text.Trim = "" Then

                If TBTestChainSwapKey.Text.Trim = "" Then
                    Exit Sub
                End If

                Dim SecretKeyList As List(Of ULong) = HEXStringToULongList(TBTestChainSwapKey.Text)

                If SecretKeyList.Count >= 1 Then
                    TBTestChainSwapKeyULong1.Text = SecretKeyList(0).ToString
                    TBTestChainSwapKeyULong2.Text = ""
                    TBTestChainSwapKeyULong3.Text = ""
                End If

                If SecretKeyList.Count >= 2 Then
                    TBTestChainSwapKeyULong2.Text = SecretKeyList(1).ToString
                    TBTestChainSwapKeyULong3.Text = ""
                End If

                If SecretKeyList.Count >= 3 Then
                    TBTestChainSwapKeyULong3.Text = SecretKeyList(2).ToString
                End If

            End If

            Dim TXID As String = CurrentContract.FinishOrderWithChainSwapKey(Masterkeys(0), TBTestChainSwapKey.Text,, Masterkeys(1))

            RTBTestDebug.AppendText(TXID + vbCrLf + vbCrLf)

            'With LVTestDEXContractBasic.Items.Add("TXID")
            '    .SubItems.Add("" + TXID)
            'End With

        End If
    End Sub

    Private Sub CoBxTestATComATID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxTestATComATID.SelectedIndexChanged

        If Not CoBxTestATComATID.SelectedItem Is Nothing Then
            Dim ItemAddress As String = CoBxTestATComATID.SelectedItem.ToString
            For Each T_Contract As ClsDEXContract In DEXContractList
                If T_Contract.Address.Trim.Contains(ItemAddress.Trim) Then
                    CurrentContract = T_Contract
                    Exit For
                End If
            Next
        End If

    End Sub

#End Region

#Region "Multithreading-Tests"

    Private Sub BtTestMultiRefresh_Click(sender As Object, e As EventArgs) Handles BtTestMultiRefresh.Click


        Dim ActiveNodes As Integer = C_MainForm.NodeList.Count
        LabActiveNodes.Text = ActiveNodes.ToString

        LVActiveNodes.Items.Clear()

        For i As Integer = 0 To C_MainForm.NodeList.Count - 1
            Dim NodeName As String = C_MainForm.NodeList(i)
            LVActiveNodes.Items.Add(NodeName)
        Next



        Dim APIReqList As List(Of SnipSwapForm.S_APIRequest) = New List(Of SnipSwapForm.S_APIRequest)(C_MainForm.APIRequestList.ToArray)

        Dim LVIList As List(Of ListViewItem) = New List(Of ListViewItem)

        For i As Integer = 0 To APIReqList.Count - 1
            Dim APIRequest As SnipSwapForm.S_APIRequest = APIReqList(i)

            Dim T_LVI As ListViewItem = New ListViewItem
            T_LVI.Tag = APIRequest.Command
            T_LVI.Text = APIRequest.Node
            T_LVI.SubItems.Add(APIRequest.Command) 'Command

            If Not APIRequest.RequestThread Is Nothing Then
                T_LVI.SubItems.Add(APIRequest.RequestThread.ManagedThreadId.ToString) 'ThreadID
            Else
                T_LVI.SubItems.Add("no Thread")
            End If

            T_LVI.SubItems.Add(APIRequest.Status) 'Status
            If APIRequest.Status = "Ready" Then

                If Not APIRequest.Result Is Nothing Then
                    T_LVI.SubItems.Add(APIRequest.Result.ToString) 'Result
                Else
                    T_LVI.SubItems.Add("Nothing") 'Result
                End If

            Else
                T_LVI.SubItems.Add("no Result")
            End If

            LVIList.Add(T_LVI)

        Next

        LVTestMulti.Items.Clear()
        LVTestMulti.Items.AddRange(LVIList.ToArray)

    End Sub

    Private Sub BtTestExit_Click(sender As Object, e As EventArgs) Handles BtTestExit.Click
        Dim TestMulti As SnipSwapForm.S_APIRequest = New SnipSwapForm.S_APIRequest
        TestMulti.Command = "Exit()"
        SnipSwapForm.APIRequestList.Add(TestMulti)
    End Sub

#End Region

#Region "TCP API tests"
    Private Sub ChBxTestTCPAPIEnable_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTestTCPAPIEnable.CheckedChanged

        If ChBxTestTCPAPIEnable.Checked Then
            If Not C_MainForm.TCPAPI.AlreadyStarted Then
                C_MainForm.TCPAPI.StartAPIServer()
            End If
        Else
            C_MainForm.TCPAPI.StopAPIServer()
        End If

        SetINISetting(E_Setting.TCPAPIEnable, ChBxTestTCPAPIEnable.Checked)

    End Sub

    Private Sub ChBxTestTCPAPIShowStatus_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTestTCPAPIShowStatus.CheckedChanged

        If ChBxTestTCPAPIShowStatus.Checked Then
            C_MainForm.TCPAPI.API_ShowStatusMSG = True
        Else
            C_MainForm.TCPAPI.API_ShowStatusMSG = False
        End If

        SetINISetting(E_Setting.TCPAPIShowStatus, ChBxTestTCPAPIShowStatus.Checked)

    End Sub

#End Region

#Region "Multiinvoke tests"
    Private Sub BtTestMultiInvokeSetInLV_Click(sender As Object, e As EventArgs) Handles BtTestMultiInvokeSetInLV.Click

        Dim LVI As ListViewItem = New ListViewItem

        LVI.Text = "a"
        LVI.SubItems.Add("b")
        LVI.SubItems.Add("c")
        LVI.BackColor = Color.Crimson
        LVI.ForeColor = Color.White

        SnipSwapForm.MultiInvoker(LVTestMultiInvoke, "Items", New List(Of Object)({"Add", LVI}))

    End Sub

#End Region

#Region "DEXNET"
    Private Sub ChBxTestDEXNETEnable_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTestDEXNETEnable.CheckedChanged

        SetINISetting(E_Setting.DEXNETEnable, ChBxTestDEXNETEnable.Checked)

        If ChBxTestDEXNETEnable.Checked Then

            If Not C_MainForm.DEXNET Is Nothing Then
                If C_MainForm.DEXNET.DEXNETClose Then
                    C_MainForm.InitiateDEXNET()
                End If
            Else
                C_MainForm.InitiateDEXNET()
            End If

        Else
            If Not C_MainForm.DEXNET Is Nothing Then
                C_MainForm.DEXNET.StopServer()
            End If
        End If

    End Sub

    Private Sub ChBxTestDEXNETShowStatus_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTestDEXNETShowStatus.CheckedChanged

        SetINISetting(E_Setting.DEXNETShowStatus, ChBxTestDEXNETShowStatus.Checked)

        If Not C_MainForm.DEXNET Is Nothing Then
            If ChBxTestDEXNETShowStatus.Checked Then
                C_MainForm.DEXNET.ShowStatus = True
            Else
                C_MainForm.DEXNET.ShowStatus = False
            End If
        End If

    End Sub

    Private Sub LiBoDEXNETStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LiBoDEXNETStatus.DoubleClick
        ShowListBoxEntry(LiBoDEXNETStatus)
    End Sub

    Private Sub LiBoTestRelMsgs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LiBoTestRelMsgs.DoubleClick
        ShowListBoxEntry(LiBoTestRelMsgs)
    End Sub

    Private Sub LiBoDEXNETPublicKeys_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LiBoDEXNETPublicKeys.DoubleClick
        ShowListBoxEntry(LiBoDEXNETPublicKeys)
    End Sub

    Private Sub ShowListBoxEntry(ByVal LiBo As ListBox)

        If Not LiBo.SelectedItem Is Nothing Then

            Dim T_Frm As Form = New Form With {.Name = "FrmMessage", .Text = "FrmMessage", .StartPosition = FormStartPosition.CenterScreen}
            Dim RTB As RichTextBox = New RichTextBox
            RTB.Dock = DockStyle.Fill
            RTB.AppendText(LiBo.SelectedItem.ToString)
            T_Frm.Controls.Add(RTB)
            T_Frm.Show()

        End If

    End Sub

    Private Sub TBTestPeerPort_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBTestPeerPort.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Select Case keys
            Case 48 To 57, 8
                ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                e.Handled = True
            Case Else
                ' alle anderen Eingaben unterdrücken
                e.Handled = True
        End Select

    End Sub

    Private Sub BtTestConnect_Click(sender As Object, e As EventArgs) Handles BtTestConnect.Click

        If Not TBTestNewPeer.Text.Trim = "" And Not TBTestPeerPort.Text.Trim = "" Then
            C_MainForm.DEXNET.Connect(TBTestNewPeer.Text, Integer.Parse(TBTestPeerPort.Text))
        End If

    End Sub
    Private Sub BtTestBroadcastMsg_Click(sender As Object, e As EventArgs) Handles BtTestBroadcastMsg.Click

        'DEXNET.BroadcastMessage("<SCID>0</SCID><Answer>request rejected</Answer>", MasterKeys(1), MasterKeys(2), MasterKeys(0), RM_AccountPublicKey)

        If Not TBTestBroadcastMessage.Text.Trim = "" Then
            If ChBxDEXNETEncryptMsg.Checked Then

                Dim MasterKeys As List(Of String) = GetPassPhrase()
                '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
                If MasterKeys.Count > 0 Then
                    C_MainForm.DEXNET.BroadcastMessage(TBTestBroadcastMessage.Text.Trim, MasterKeys(1), MasterKeys(2), MasterKeys(0), TBTestRecipientPubKey.Text)
                End If

            Else
                C_MainForm.DEXNET.BroadcastMessage(TBTestBroadcastMessage.Text.Trim)
            End If
        End If

    End Sub

    Private Sub BtTestAddRelKey_Click(sender As Object, e As EventArgs) Handles BtTestAddRelKey.Click
        C_MainForm.DEXNET.AddRelevantKey(TBTestAddRelKey.Text)
    End Sub



    Private Sub LiBoTestRelKeys_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LiBoTestRelKeys.DoubleClick

        If Not LiBoTestRelKeys.SelectedItem Is Nothing Then
            Dim T_RelKey As String = LiBoTestRelKeys.SelectedItem.ToString
            Dim T_Key As String = T_RelKey.Remove(T_RelKey.IndexOf("("))
            Dim T_SubKey As String = GetStringBetween(T_RelKey, "(", ")")
            C_MainForm.DEXNET.DelRelevantKey(T_Key, T_SubKey)
            LiBoTestRelKeys.Items.Remove(LiBoTestRelKeys.SelectedItem)
        End If

    End Sub



#End Region

#Region "Test"

    Private Sub ClearEntryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearEntryToolStripMenuItem.Click

        If LiBoDEXNETStatus.SelectedItems.Count > 0 Then
            LiBoDEXNETStatus.Items.Remove(LiBoDEXNETStatus.SelectedItem)
        End If

    End Sub

    Private Sub ClearAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearAllToolStripMenuItem.Click
        LiBoDEXNETStatus.Items.Clear()
    End Sub

    Private Sub BtTestHex2ULng_Click(sender As Object, e As EventArgs) Handles BtTestHex2ULng.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.HEX2ULng(TBTestConvert.Text).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub

    Private Sub BtTestLoadDEXContract_Click(sender As Object, e As EventArgs) Handles BtTestLoadDEXContract.Click

        If Not CurrentContract Is Nothing Then

            CurrentContract.Refresh(False)

            'Dim C_SignumAPI = New ClsSignumAPI("")
            Dim CurrentBlockHeight As ULong = C_SignumAPI.GetCurrentBlock()


            LVTestDEXContractBasic.Items.Clear()
            LVTestCurrentOrder.Items.Clear()

            With LVTestDEXContractBasic.Items.Add("Node")
                .SubItems.Add(CurrentContract.Node)
            End With

            With LVTestDEXContractBasic.Items.Add("CreatorID")
                .SubItems.Add("" + CurrentContract.CreatorID.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("CreatorAddress")
                .SubItems.Add("" + CurrentContract.CreatorAddress)
            End With
            With LVTestDEXContractBasic.Items.Add("ID")
                .SubItems.Add("" + CurrentContract.ID.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("Address")
                .SubItems.Add("" + CurrentContract.Address)
            End With
            With LVTestDEXContractBasic.Items.Add("Name")
                .SubItems.Add("" + CurrentContract.Name)
            End With
            With LVTestDEXContractBasic.Items.Add("Description")
                .SubItems.Add("" + CurrentContract.Description)
            End With

            With LVTestDEXContractBasic.Items.Add("Balance")
                .SubItems.Add("" + CurrentContract.CurrentBalance.ToString())
            End With

            With LVTestDEXContractBasic.Items.Add("isDEXContract")
                .SubItems.Add("" + CurrentContract.IsDEXContract.ToString())
            End With

            With LVTestDEXContractBasic.Items.Add("Deniability")
                .SubItems.Add("" + CurrentContract.Deniability.ToString())
            End With

            With LVTestDEXContractBasic.Items.Add("HistoryOrders")
                .SubItems.Add("" + CurrentContract.ContractOrderHistoryList.Count.ToString())
            End With

            With LVTestDEXContractBasic.Items.Add("")
                .BackColor = Color.LightGray
            End With

            With LVTestDEXContractBasic.Items.Add("isFrozen")
                .SubItems.Add("" + CurrentContract.IsFrozen.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("isRunning")
                .SubItems.Add("" + CurrentContract.IsRunning.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("isStopped")
                .SubItems.Add("" + CurrentContract.IsStopped.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("isFinished")
                .SubItems.Add("" + CurrentContract.IsFinished.ToString())
            End With
            With LVTestDEXContractBasic.Items.Add("isDead")
                .SubItems.Add("" + CurrentContract.IsDead.ToString())
            End With

            'LVTestDEXContractBasic.Items.Add("--------------------")

            With LVTestCurrentOrder.Items.Add("CurrentCreationTransaction")
                .SubItems.Add("" + CurrentContract.CurrentCreationTransaction.ToString())
            End With
            With LVTestCurrentOrder.Items.Add("CurrentConfirmations")
                .SubItems.Add("" + CurrentContract.CurrentConfirmations.ToString())
            End With

            With LVTestCurrentOrder.Items.Add("isCurrentSellOrder")
                .SubItems.Add("" + CurrentContract.IsSellOrder.ToString())
            End With

            With LVTestCurrentOrder.Items.Add("isInDispute")
                .SubItems.Add("" + CurrentContract.Dispute.ToString())
            End With

            With LVTestCurrentOrder.Items.Add("CurrentSeller")
                .SubItems.Add("" + CurrentContract.CurrentSellerAddress)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentBuyer")
                .SubItems.Add("" + CurrentContract.CurrentBuyerAddress)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentInitiatorsCollateral")

                .SubItems.Add("" + Math.Round(CurrentContract.CurrentInitiatorsCollateral, 8).ToString() + " Signa")
            End With
            With LVTestCurrentOrder.Items.Add("CurrentRespondersCollateral")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentRespondersCollateral, 8).ToString() + " Signa")
            End With

            If CurrentContract.IsSellOrder Then
                With LVTestCurrentOrder.Items.Add("CurrentSellAmount")
                    .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentBuySellAmount) + " Signa")
                End With
            Else
                With LVTestCurrentOrder.Items.Add("CurrentBuyAmount")
                    .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentBuySellAmount) + " Signa")
                End With
            End If

            With LVTestCurrentOrder.Items.Add("CurrentMediatorsDeposit")
                .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentMediatorsDeposit) + " Signa")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentDisputeTimeoutBlock")

                Dim diffblock As Long = 0

                If CurrentContract.CurrentDisputeTimeout <> 0UL Then
                    diffblock = CLng(CurrentContract.CurrentDisputeTimeout) - CLng(CurrentBlockHeight)
                End If

                .SubItems.Add(CurrentBlockHeight.ToString() + " / " + CurrentContract.CurrentDisputeTimeout.ToString() + " (in " + diffblock.ToString() + " Blocks)")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentObjection")
                .SubItems.Add("" + CurrentContract.CurrentObjection.ToString())
            End With


            With LVTestCurrentOrder.Items.Add("CurrentConciliationAmount")

                Dim percentage As Double = 100 / CurrentContract.CurrentBuySellAmount * CurrentContract.CurrentConciliationAmount

                .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentConciliationAmount) + " Signa / " + percentage.ToString() + "%")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentChainSwapHashLong1")
                .SubItems.Add("" + CurrentContract.CurrentChainSwapHashULong1.ToString())
            End With
            With LVTestCurrentOrder.Items.Add("CurrentChainSwapHashLong2")
                .SubItems.Add("" + CurrentContract.CurrentChainSwapHashULong2.ToString())
            End With
            With LVTestCurrentOrder.Items.Add("CurrentChainSwapHashLong3")
                .SubItems.Add("" + CurrentContract.CurrentChainSwapHashULong3.ToString())
            End With
            With LVTestCurrentOrder.Items.Add("CurrentChainSwapHashLong4")
                .SubItems.Add("" + CurrentContract.CurrentChainSwapHashULong4.ToString())
            End With

            With LVTestCurrentOrder.Items.Add("CurrentXItem")

                .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentXAmount) + " " + CurrentContract.CurrentXItem)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentPrice")

                .SubItems.Add("" + SnipSwapForm.Dbl2LVStr(CurrentContract.CurrentPrice) + " " + CurrentContract.CurrentXItem)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentStatus")
                .SubItems.Add("" + CurrentContract.Status.ToString())
            End With

            If CurrentContract.IsSellOrder Then
                NUDTestCollateral.Value = CDec(CurrentContract.CurrentInitiatorsCollateral)
            Else
                NUDTestAmount.Value = CDec(CurrentContract.CurrentBuySellAmount)
                NUDTestCollateral.Value = CDec(CurrentContract.CurrentInitiatorsCollateral)
            End If

            LVTestDEXContractBasic.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
            LVTestCurrentOrder.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

            'LiBoHistoryOrders.Items.Clear()

            'For Each HisOrd As S_Order In T_SignumContract.ContractOrderHistoryList

            '    LiBoHistoryOrders.Items.Add("CreationTransaction= " + HisOrd.CreationTransaction.ToString)
            '    LiBoHistoryOrders.Items.Add("Confirmations= " + HisOrd.Confirmations.ToString)
            '    LiBoHistoryOrders.Items.Add("StartTimestamp= " + HisOrd.StartTimestamp.ToString)
            '    LiBoHistoryOrders.Items.Add("EndTimestamp= " + HisOrd.EndTimestamp.ToString)
            '    LiBoHistoryOrders.Items.Add("Seller= " + HisOrd.SellerRS)
            '    LiBoHistoryOrders.Items.Add("Buyer= " + HisOrd.BuyerRS)
            '    LiBoHistoryOrders.Items.Add("Collateral= " + HisOrd.Collateral.ToString)
            '    LiBoHistoryOrders.Items.Add("Amount= " + Math.Round(HisOrd.Amount, 2).ToString("0.00") + " Signa")
            '    LiBoHistoryOrders.Items.Add("XItem= " + Math.Round(HisOrd.XAmount, 2).ToString("0.00") + " " + HisOrd.XItem)
            '    LiBoHistoryOrders.Items.Add("Price= " + Math.Round(HisOrd.Price, 2).ToString("0.00") + " " + HisOrd.XItem)
            '    LiBoHistoryOrders.Items.Add("Status= " + HisOrd.Status.ToString)
            '    LiBoHistoryOrders.Items.Add("--------------------")

            'Next
        End If
    End Sub

    Private Sub BtTestChainSwapKeyToHash_Click(sender As Object, e As EventArgs) Handles BtTestChainSwapKeyToHash.Click

        'Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")
        Dim ChainSwapKeyList As List(Of ULong) = HEXStringToULongList(TBTestChainSwapKey.Text)
        Dim Hash As String = GetSHA256HashString(TBTestChainSwapKey.Text)
        Dim ChainSwapHashList As List(Of ULong) = HEXStringToULongList(Hash)

        If ChainSwapKeyList.Count >= 1 Then
            TBTestChainSwapKeyULong1.Text = ChainSwapKeyList(0).ToString
            TBTestChainSwapKeyULong2.Text = ""
            TBTestChainSwapKeyULong3.Text = ""
        End If

        If ChainSwapKeyList.Count >= 2 Then
            TBTestChainSwapKeyULong2.Text = ChainSwapKeyList(1).ToString
            TBTestChainSwapKeyULong3.Text = ""
        End If

        If ChainSwapKeyList.Count >= 3 Then
            TBTestChainSwapKeyULong3.Text = ChainSwapKeyList(2).ToString
        End If

        If ChainSwapHashList.Count >= 3 Then
            TBTestChainSwapHashLong1.Text = ChainSwapHashList(0).ToString
            TBTestChainSwapHashLong2.Text = ChainSwapHashList(1).ToString
            TBTestChainSwapHashLong3.Text = ChainSwapHashList(2).ToString
        End If

    End Sub

    Private Sub BtExport_Click(sender As Object, e As EventArgs) Handles BtExport.Click

        'Dim contractIDAddressList As List(Of List(Of String)) = New List(Of List(Of String))
        Dim out As ClsOut = New ClsOut()
        Dim str As String = ""
        For Each T_Contract As ClsDEXContract In DEXContractList
            str += T_Contract.ID.ToString + ";" + T_Contract.Address + vbCrLf
            'contractIDAddressList.Add(New List(Of String)({T_Contract.ID.ToString, T_Contract.Name}))
        Next

        out.Info2File(str)

    End Sub

    Private Sub BtTestRefreshLiBoRelMsgs_Click(sender As Object, e As EventArgs) Handles BtTestRefreshLiBoRelMsgs.Click
        LiBoTestRelMsgs.Items.Clear()
    End Sub

    Private Sub BtCSKConvertback_Click(sender As Object, e As EventArgs) Handles BtCSKConvertback.Click

        Dim FirstChainSwapKeyLong As ULong = 0UL
        If TBTestChainSwapKeyULong1.Text.Trim <> "" Then
            FirstChainSwapKeyLong = CULng(TBTestChainSwapKeyULong1.Text)
        End If

        Dim SecondChainSwapKeyLong As ULong = 0UL
        If TBTestChainSwapKeyULong2.Text.Trim <> "" Then
            SecondChainSwapKeyLong = CULng(TBTestChainSwapKeyULong2.Text)
        End If

        Dim ThirdChainSwapKeyLong As ULong = 0UL
        If TBTestChainSwapKeyULong3.Text.Trim <> "" Then
            ThirdChainSwapKeyLong = CULng(TBTestChainSwapKeyULong3.Text)
        End If

        Dim ChainSwpKey As String = ULongListToHEXString(New List(Of ULong)({FirstChainSwapKeyLong, SecondChainSwapKeyLong, ThirdChainSwapKeyLong}))

        MsgBox(ChainSwpKey)

    End Sub

    Private Sub BtTestHash_Click(sender As Object, e As EventArgs) Handles BtTestHash.Click

        '00000000000000a100000000000000b200000000000000c300000000000000d4

        If MessageIsHEXString(TBTestKeyString.Text.Trim) Then
            TBTestKeyHEX.Text = TBTestKeyString.Text.Trim
        Else
            TBTestKeyHEX.Text = ByteArrayToHEXString(System.Text.Encoding.ASCII.GetBytes(TBTestKeyString.Text.Trim))
        End If

        TBTestHashHEX.Text = GetSHA256HashString(TBTestKeyHEX.Text.Trim).ToLower


        Dim ULKeyList As List(Of ULong) = HEXStringToULongList(TBTestKeyHEX.Text)
        'ULKeyList.Reverse()

        'For i As Integer = ULKeyList.Count To 3
        '    ULKeyList.Add(0UL)
        'Next

        If ULKeyList.Count >= 1 Then
            TBTestKeyUL1.Text = ULKeyList(0).ToString
            TBTestKeyUL2.Text = ""
            TBTestKeyUL3.Text = ""
            TBTestKeyUL4.Text = ""
        End If

        If ULKeyList.Count >= 2 Then
            TBTestKeyUL2.Text = ULKeyList(1).ToString
            TBTestKeyUL3.Text = ""
            TBTestKeyUL4.Text = ""
        End If

        If ULKeyList.Count >= 3 Then
            TBTestKeyUL3.Text = ULKeyList(2).ToString
            TBTestKeyUL4.Text = ""
        End If

        If ULKeyList.Count >= 4 Then
            TBTestKeyUL4.Text = ULKeyList(3).ToString
        End If


        Dim ULHashList As List(Of ULong) = GetSHA256HashULongs(ULKeyList, False)
        'ULHashList.Reverse()

        If ULHashList.Count >= 4 Then
            TBTestHashUL1.Text = ULHashList(0).ToString
            TBTestHashUL2.Text = ULHashList(1).ToString
            TBTestHashUL3.Text = ULHashList(2).ToString
            TBTestHashUL4.Text = ULHashList(3).ToString
        End If

        TBTestHashULHEX.Text = ULongListToHEXString(ULHashList)

    End Sub

    Private Sub BtTestBitcoinGetInfo_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetInfo.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        RTBTestBitcoin.AppendText(BitNet.GetMiningInfo() + vbCrLf)

    End Sub

    Private Sub BtTestBitcoinGetUTXOSet_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetUTXOSet.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()

        Dim Result As String = BitNet.ScanTXOUTSet(TBTestBitcoinAddress.Text) + vbCrLf

        Result = Result.Replace("\n", vbCrLf)
        Result = Result.Replace("\""", """")

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetWalletInfo.Click

        Dim XItem As ClsBitcoin = New ClsBitcoin
        Dim WalletInfo As String = XItem.GetWalletInfo()

        RTBTestBitcoin.AppendText(WalletInfo + vbCrLf)

    End Sub

    Private Sub BtTestBitcoinImportDescriptor_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinImportDescriptor.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.ImportDescriptor(TBTestBitcoinAddress.Text, "0") + vbCrLf

        Result = Result.Replace("\n", vbCrLf)
        Result = Result.Replace("\""", """")

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinWalletName_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinWalletName.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.CreateNewWallet(TBTestBitcoinWalletName.Text).ToString() + vbCrLf

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoionLoadWallet_Click(sender As Object, e As EventArgs) Handles BtTestBitcoionLoadWallet.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.LoadWalletRaw(TBTestBitcoinWalletName.Text).ToString() + vbCrLf

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetBalances_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetBalances.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.GetBalances()

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetBalance_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetBalance.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.GetBalance()

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetDescriptorInfo_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetDescriptorInfo.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.GetDescriptorInfo(TBTestBitcoinAddress.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetRawTransaction_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetRawTransaction.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.GetRawTransaction(TBTestBitcoinTransactionID.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetTransaction_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetTransaction.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As String = BitNet.GetTransaction(TBTestBitcoinTransactionID.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinListUnspent_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinListUnspent.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result = BitNet.ListUnspentRaw(TBTestBitcoinAddress.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinGetReceivedByAddress_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinGetReceivedByAddress.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result = BitNet.GetReceivedByAddress(TBTestBitcoinAddress.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Private Sub BtTestBitcoinScanBlocks_Click(sender As Object, e As EventArgs) Handles BtTestBitcoinScanBlocks.Click

        Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result = BitNet.ScanBlocks(TBTestBitcoinAddress.Text)

        RTBTestBitcoin.AppendText(Result)

    End Sub

    Dim SigTX As ClsSignumTransaction

    Private Sub BtTestSigTX_ReadTX_Click(sender As Object, e As EventArgs) Handles BtTestSigTX_ReadTX.Click
        SigTX = New ClsSignumTransaction(Convert.ToUInt64(TBTestSigTX_TXID.Text))
        SetSigTXInLV()
    End Sub

    Private Sub BtTestSigTX_Decrypt_Click(sender As Object, e As EventArgs) Handles BtTestSigTX_Decrypt.Click

        If Not IsNothing(SigTX) Then
            SigTX.DecryptAttachment(TBTestSigTX_PassPhrase.Text)
            SetSigTXInLV()
        End If

    End Sub

    Private Sub SetSigTXInLV()

        LVTestSigTX.Items.Clear()

        With LVTestSigTX.Items.Add("Timestamp")
            .SubItems.Add(SigTX.DateTimeStamp.ToString())
        End With

        With LVTestSigTX.Items.Add("Block")
            .SubItems.Add(SigTX.Block.ToString())
        End With

        With LVTestSigTX.Items.Add("Height")
            .SubItems.Add(SigTX.Height.ToString())
        End With

        With LVTestSigTX.Items.Add("Confirmations")
            .SubItems.Add(SigTX.Confirmations.ToString())
        End With

        With LVTestSigTX.Items.Add("SenderPublicKey")
            .SubItems.Add(SigTX.SenderPublicKey)
        End With

        With LVTestSigTX.Items.Add("SenderID")
            .SubItems.Add(SigTX.SenderID.ToString())
        End With

        With LVTestSigTX.Items.Add("SenderAddress")
            .SubItems.Add(SigTX.SenderAddress)
        End With

        With LVTestSigTX.Items.Add("RecipientPublicKey")
            .SubItems.Add(SigTX.RecipientPublicKey)
        End With

        With LVTestSigTX.Items.Add("RecipientID")
            .SubItems.Add(SigTX.RecipientID.ToString())
        End With

        With LVTestSigTX.Items.Add("RecipientAddress")
            .SubItems.Add(SigTX.RecipientAddress)
        End With

        With LVTestSigTX.Items.Add("Amount")
            .SubItems.Add(SigTX.AmountString)
        End With

        With LVTestSigTX.Items.Add("Fee")
            .SubItems.Add(SigTX.FeeString)
        End With

        With LVTestSigTX.Items.Add("Balance")
            .SubItems.Add(SigTX.BalanceString)
        End With

        With LVTestSigTX.Items.Add("Attachment")
            .SubItems.Add(SigTX.Attachment)
        End With

        With LVTestSigTX.Items.Add("Type")
            .SubItems.Add(SigTX.Type.ToString())
        End With

        With LVTestSigTX.Items.Add("Message")
            .SubItems.Add(SigTX.Message)
        End With

        With LVTestSigTX.Items.Add("Command")
            .SubItems.Add(SigTX.ReferenceCommand.ToString())
        End With

        RTBTestSigTX.Clear()
        RTBTestSigTX.AppendText(SigTX.Message)

        LVTestSigTX.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        LVTestSigTX.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

    End Sub

#End Region

End Class