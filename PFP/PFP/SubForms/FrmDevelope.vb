Option Strict On
Option Explicit On

Public Class FrmDevelope

    Dim C_MainForm As PFPForm = CType(Me.ParentForm, PFPForm)
    Dim SpecialTimer As Integer = 0

    Dim CurrentContract As ClsDEXContract = Nothing

    Sub New(ByVal MainForm As PFPForm)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_MainForm = MainForm


        CoBxTestATComATID.Items.Clear()

        For Each x In C_MainForm.DEXContractList
            CoBxTestATComATID.Items.Add(x.Address)
        Next

        If CoBxTestATComATID.Items.Count > 0 Then
            CoBxTestATComATID.SelectedItem = CoBxTestATComATID.Items(0)
            CurrentContract = C_MainForm.DEXContractList(0)
        End If

    End Sub
    Private Sub FrmDevelope_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CoBxTestATComATID.Items.Clear()
        For i As Integer = 0 To C_MainForm.DEXSmartContractList.Count - 1
            Dim DEXAT As String = C_MainForm.DEXSmartContractList(i)
            CoBxTestATComATID.Items.Add(ClsReedSolomon.Encode(ULong.Parse(DEXAT)))
        Next

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

        Dim PPAPI As ClsPayPal = New ClsPayPal
        CoBxTestPPCurrency.Items.AddRange(PPAPI.Currencys.ToArray)

        CoBxTestPPCurrency.SelectedItem = "USD"

    End Sub
    Private Sub DevTimer_Tick(sender As Object, e As EventArgs) Handles DevTimer.Tick

        If Not C_MainForm.DEXNET Is Nothing Then

            Dim DEXNETStatusMsgs As List(Of String) = C_MainForm.DEXNET.GetStatusMessages

            If DEXNETStatusMsgs.Count > 0 Then
                LiBoDEXNETStatus.Items.AddRange(DEXNETStatusMsgs.ToArray)
            End If

            Dim RelMsgs As List(Of ClsDEXNET.S_RelevantMessage) = C_MainForm.DEXNET.GetRelevantMsgs

            LiBoTestRelMsgs.Items.Clear()

            For Each RelMsg As ClsDEXNET.S_RelevantMessage In RelMsgs

                Dim FoundedKey As Boolean = False

                For Each LiBoRelKey As String In LiBoTestRelKeys.Items
                    If LiBoRelKey = RelMsg.RelevantKey Then
                        FoundedKey = True
                        Exit For
                    End If
                Next

                If Not FoundedKey Then
                    LiBoTestRelKeys.Items.Add(RelMsg.RelevantKey)
                End If


                'Dim FoundedMsg As Boolean = False

                'For Each LiBoRelMsg As String In LiBoTestRelMsgs.Items

                '    If LiBoRelMsg = RelMsg.RelevantMessage Then
                '        FoundedMsg = True
                '        Exit For
                '    End If

                'Next

                'If Not FoundedMsg Then
                LiBoTestRelMsgs.Items.Add(RelMsg.RelevantMessage)
                'End If

            Next

            LiBoTestPeers.Items.Clear()

            Dim Peers As List(Of ClsDEXNET.S_Peer) = C_MainForm.DEXNET.GetPeers

            For Each Peer As ClsDEXNET.S_Peer In Peers
                LiBoTestPeers.Items.Add(Peer.PossibleHost + ":" + Peer.Port.ToString)
            Next

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



                Dim APIReqList As List(Of PFPForm.S_APIRequest) = New List(Of PFPForm.S_APIRequest)(C_MainForm.APIRequestList.ToArray)

                Dim LVIList As List(Of ListViewItem) = New List(Of ListViewItem)


                For i As Integer = 0 To APIReqList.Count - 1
                    Dim APIRequest As PFPForm.S_APIRequest = APIReqList(i)

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


#Region "Convertings"

    Private Sub BtTestDatStr2ULngList_Click(sender As Object, e As EventArgs) Handles BtTestDatStr2ULngList.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({ClsSignumAPI.String2ULng(TBTestConvert.Text.Trim)})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)

        TBTestConvert.Text = MsgStr

    End Sub
    Private Sub BtTestConvert_Click(sender As Object, e As EventArgs) Handles BtTestConvert.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.String2ULng(TBTestConvert.Text).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub
    Private Sub BtTestConvert2_Click(sender As Object, e As EventArgs) Handles BtTestConvert2.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.ULng2String(Convert.ToUInt64(TBTestConvert.Text)).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub
    Private Sub BtTestTimeConvert_Click(sender As Object, e As EventArgs) Handles BtTestTimeConvert.Click
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)
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
        Dim JSON As ClsJSON = New ClsJSON
        Dim JSONList As List(Of Object) = JSON.JSONRecursive(TBTestJSONInput.Text)
        Dim XMLStr As String = JSON.JSONListToXMLRecursive(JSONList)
        TBTestXMLOutput.Text = XMLStr
    End Sub

    Private Sub BtTestRecursiveXMLSearch_Click(sender As Object, e As EventArgs) Handles BtTestRecursiveXMLSearch.Click
        Dim JSON As ClsJSON = New ClsJSON
        Dim JSONList As List(Of Object) = JSON.JSONRecursive(TBTestJSONInput.Text)
        Dim XMLStr As String = JSON.JSONListToXMLRecursive(JSONList)
        XMLStr = JSON.RecursiveXMLSearch(XMLStr, TBTestRecursiveXMLSearch.Text)
        TBTestXMLOutput.Text = XMLStr
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

        Dim PPAPI As ClsPayPal = New ClsPayPal
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

        Dim PPAPI As ClsPayPal = New ClsPayPal
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

        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")

        Dim PPOrderID As List(Of String) = PPAPI.CreateOrder(TBTestPPXItem.Text, Double.Parse(TBTestPPXAmount.Text), Double.Parse(TBTestPPPrice.Text), CoBxTestPPCurrency.SelectedItem.ToString)

        LiBoPayPalComs.Items.Clear()

        For Each TX As String In PPOrderID
            LiBoPayPalComs.Items.Add(TX)
        Next

    End Sub

    Private Sub BtTestPPPayout_Click(sender As Object, e As EventArgs) Handles BtTestPPPayout.Click

        Dim PPAPI As ClsPayPal = New ClsPayPal
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
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
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

            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With

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

            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If

    End Sub

    Private Sub BtTestInjectResponder_Click(sender As Object, e As EventArgs) Handles BtTestInjectResponder.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.InjectResponder(Masterkeys(0), ULong.Parse(TBTestResponder.Text),, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestOpenDispute_Click(sender As Object, e As EventArgs) Handles BtTestOpenDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.OpenDispute(Masterkeys(0),, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestMediateDispute_Click(sender As Object, e As EventArgs) Handles BtTestMediateDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            Dim TXID As String = CurrentContract.MediateDispute(Masterkeys(0), NUDTestMediateAmount.Value,, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestAppeal_Click(sender As Object, e As EventArgs) Handles BtTestAppeal.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.Appeal(Masterkeys(0),, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestCheckCloseDispute_Click(sender As Object, e As EventArgs) Handles BtTestCheckCloseDispute.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.CheckCloseDispute(Masterkeys(0),, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestFinish_Click(sender As Object, e As EventArgs) Handles BtTestFinish.Click
        If Not CurrentContract Is Nothing Then
            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)
            Dim TXID As String = CurrentContract.FinishOrder(Masterkeys(0),, Masterkeys(1))
            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With
        End If
    End Sub

    Private Sub BtTestInjectChainSwapHash_Click(sender As Object, e As EventArgs) Handles BtTestInjectChainSwapHash.Click

        If Not CurrentContract Is Nothing Then

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            If TBTesChainSwapHash.Text.Trim = "" Then
                Exit Sub
            End If

            Dim ChainSwapHash As ULong = ULong.Parse(TBTesChainSwapHash.Text)

            Dim TXID As String = CurrentContract.InjectChainSwapHash(Masterkeys(0), ChainSwapHash,, Masterkeys(1))

            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With

        End If
    End Sub

    Private Sub BtTestFinishWithChainSwapKey_Click(sender As Object, e As EventArgs) Handles BtTestFinishWithChainSwapKey.Click
        If Not CurrentContract Is Nothing Then

            Dim Masterkeys As List(Of String) = GetMasterKeys(TBTestPP.Text)

            If TBTestChainSwapULong1.Text.Trim = "" Or TBTestChainSwapULong2.Text.Trim = "" Then

                If TBTestChainSwapKey.Text.Trim = "" Then
                    Exit Sub
                End If

                Dim SecretKeyList As List(Of ULong) = ClsSignumAPI.GetSHA256_64(TBTestChainSwapKey.Text)
                TBTestChainSwapULong1.Text = SecretKeyList(0).ToString
                TBTestChainSwapULong2.Text = SecretKeyList(1).ToString

            End If

            Dim TXID As String = CurrentContract.FinishOrderWithChainSwapKey(Masterkeys(0), ULong.Parse(TBTestChainSwapULong1.Text), ULong.Parse(TBTestChainSwapULong2.Text),, Masterkeys(1))

            With LVTestDEXContractBasic.Items.Add("TXID")
                .SubItems.Add("" + TXID)
            End With

        End If
    End Sub

    Private Sub CoBxTestATComATID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxTestATComATID.SelectedIndexChanged

        If Not CoBxTestATComATID.SelectedItem Is Nothing Then
            Dim ItemAddress As String = CoBxTestATComATID.SelectedItem.ToString
            For Each T_Contract As ClsDEXContract In C_MainForm.DEXContractList
                If T_Contract.Address.Trim = ItemAddress.Trim Then
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



        Dim APIReqList As List(Of PFPForm.S_APIRequest) = New List(Of PFPForm.S_APIRequest)(C_MainForm.APIRequestList.ToArray)

        Dim LVIList As List(Of ListViewItem) = New List(Of ListViewItem)

        For i As Integer = 0 To APIReqList.Count - 1
            Dim APIRequest As PFPForm.S_APIRequest = APIReqList(i)

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
        Dim TestMulti As PFPForm.S_APIRequest = New PFPForm.S_APIRequest
        TestMulti.Command = "Exit()"
        PFPForm.APIRequestList.Add(TestMulti)
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

        PFPForm.MultiInvoker(LVTestMultiInvoke, "Items", New List(Of Object)({"Add", LVI}))

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

        If Not LiBoDEXNETStatus.SelectedItem Is Nothing Then

            Dim T_Frm As Form = New Form With {.Name = "FrmMessage", .Text = "FrmMessage", .StartPosition = FormStartPosition.CenterScreen}
            Dim RTB As RichTextBox = New RichTextBox
            RTB.Dock = DockStyle.Fill
            RTB.AppendText(LiBoDEXNETStatus.SelectedItem.ToString)
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
            C_MainForm.DEXNET.DelRelevantKey(LiBoTestRelKeys.SelectedItem.ToString)
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

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Try
            TBTestConvert.Text = ClsSignumAPI.HEX2ULng(TBTestConvert.Text).ToString
        Catch ex As Exception
            TBTestConvert.Text = "error"
        End Try

    End Sub

    Private Sub BtTestLoadDEXContract_Click(sender As Object, e As EventArgs) Handles BtTestLoadDEXContract.Click

        If Not CurrentContract Is Nothing Then

            CurrentContract.Refresh(False)

            Dim k = New ClsSignumAPI
            Dim CurrentBlockHeight As Integer = k.GetCurrentBlock()


            LVTestDEXContractBasic.Items.Clear()
            LVTestCurrentOrder.Items.Clear()

            With LVTestDEXContractBasic.Items.Add("Node")
                .SubItems.Add(CurrentContract.Node)
            End With

            With LVTestDEXContractBasic.Items.Add("CreatorID")
                .SubItems.Add("" + CurrentContract.CreatorID.ToString)
            End With
            With LVTestDEXContractBasic.Items.Add("CreatorAddress")
                .SubItems.Add("" + CurrentContract.CreatorAddress)
            End With
            With LVTestDEXContractBasic.Items.Add("ID")
                .SubItems.Add("" + CurrentContract.ID.ToString)
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
                .SubItems.Add("" + CurrentContract.CurrentBalance.ToString)
            End With

            With LVTestDEXContractBasic.Items.Add("isDEXContract")
                .SubItems.Add("" + CurrentContract.IsDEXContract.ToString)
            End With

            With LVTestDEXContractBasic.Items.Add("Deniability")
                .SubItems.Add("" + CurrentContract.Deniability.ToString)
            End With

            With LVTestDEXContractBasic.Items.Add("HistoryOrders")
                .SubItems.Add("" + CurrentContract.ContractOrderHistoryList.Count.ToString)
            End With

            With LVTestDEXContractBasic.Items.Add("")
                .BackColor = Color.LightGray
            End With

            With LVTestDEXContractBasic.Items.Add("isFrozen")
                .SubItems.Add("" + CurrentContract.IsFrozen.ToString)
            End With
            With LVTestDEXContractBasic.Items.Add("isRunning")
                .SubItems.Add("" + CurrentContract.IsRunning.ToString)
            End With
            With LVTestDEXContractBasic.Items.Add("isStopped")
                .SubItems.Add("" + CurrentContract.IsStopped.ToString)
            End With
            With LVTestDEXContractBasic.Items.Add("isFinished")
                .SubItems.Add("" + CurrentContract.IsFinished.ToString)
            End With
            With LVTestDEXContractBasic.Items.Add("isDead")
                .SubItems.Add("" + CurrentContract.IsDead.ToString)
            End With

            'LVTestDEXContractBasic.Items.Add("--------------------")

            With LVTestCurrentOrder.Items.Add("CurrentCreationTransaction")
                .SubItems.Add("" + CurrentContract.CurrentCreationTransaction.ToString)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentConfirmations")
                .SubItems.Add("" + CurrentContract.CurrentConfirmations.ToString)
            End With

            With LVTestCurrentOrder.Items.Add("isCurrentSellOrder")
                .SubItems.Add("" + CurrentContract.IsSellOrder.ToString)
            End With

            With LVTestCurrentOrder.Items.Add("isInDispute")
                .SubItems.Add("" + CurrentContract.Dispute.ToString)
            End With

            With LVTestCurrentOrder.Items.Add("CurrentSeller")
                .SubItems.Add("" + CurrentContract.CurrentSellerAddress)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentBuyer")
                .SubItems.Add("" + CurrentContract.CurrentBuyerAddress)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentInitiatorsCollateral")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentInitiatorsCollateral, 2).ToString("0.00") + " Signa")
            End With
            With LVTestCurrentOrder.Items.Add("CurrentRespondersCollateral")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentRespondersCollateral, 2).ToString("0.00") + " Signa")
            End With

            If CurrentContract.IsSellOrder Then
                With LVTestCurrentOrder.Items.Add("CurrentSellAmount")
                    .SubItems.Add("" + Math.Round(CurrentContract.CurrentBuySellAmount, 2).ToString("0.00") + " Signa")
                End With
            Else
                With LVTestCurrentOrder.Items.Add("CurrentBuyAmount")
                    .SubItems.Add("" + Math.Round(CurrentContract.CurrentBuySellAmount, 2).ToString("0.00") + " Signa")
                End With
            End If

            With LVTestCurrentOrder.Items.Add("CurrentMediatorsDeposit")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentMediatorsDeposit, 2).ToString("0.00") + " Signa")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentDisputeTimeoutBlock")

                Dim diffblock As Long = 0

                If CurrentContract.CurrentDisputeTimeout <> 0UL Then
                    diffblock = CLng(CurrentContract.CurrentDisputeTimeout) - CLng(CurrentBlockHeight)
                End If

                .SubItems.Add(CurrentBlockHeight.ToString + " / " + CurrentContract.CurrentDisputeTimeout.ToString + " (in " + diffblock.ToString + " Blocks)")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentObjection")
                .SubItems.Add("" + CurrentContract.CurrentObjection.ToString)
            End With


            With LVTestCurrentOrder.Items.Add("CurrentConciliationAmount")

                Dim percentage As Double = 100 / CurrentContract.CurrentBuySellAmount * CurrentContract.CurrentConciliationAmount

                .SubItems.Add("" + Math.Round(CurrentContract.CurrentConciliationAmount, 2).ToString("0.00") + " Signa / " + percentage.ToString + "%")
            End With

            With LVTestCurrentOrder.Items.Add("CurrentChainSwapHash")
                .SubItems.Add("" + CurrentContract.CurrentChainSwapHash.ToString)
            End With

            With LVTestCurrentOrder.Items.Add("CurrentXItem")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentXAmount, 2).ToString("0.00") + " " + CurrentContract.CurrentXItem)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentPrice")
                .SubItems.Add("" + Math.Round(CurrentContract.CurrentPrice, 2).ToString("0.00") + " " + CurrentContract.CurrentXItem)
            End With
            With LVTestCurrentOrder.Items.Add("CurrentStatus")
                .SubItems.Add("" + CurrentContract.Status.ToString)
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

#End Region

End Class