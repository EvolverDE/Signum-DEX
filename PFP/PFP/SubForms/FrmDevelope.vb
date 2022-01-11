Option Strict On
Option Explicit On

Public Class FrmDevelope

    Dim C_MainForm As PFPForm = CType(Me.ParentForm, PFPForm)
    Dim SpecialTimer As Integer = 0

    Sub New(ByVal MainForm As PFPForm)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_MainForm = MainForm

    End Sub
    Private Sub FrmDevelope_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CoBxTestATComATID.Items.Clear()
        For i As Integer = 0 To C_MainForm.DEXATList.Count - 1
            Dim DEXAT As String = C_MainForm.DEXATList(i)
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

        If Not IsNothing(C_MainForm.DEXNET) Then

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

                    If Not IsNothing(APIRequest.RequestThread) Then
                        T_LVI.SubItems.Add(APIRequest.RequestThread.ManagedThreadId.ToString) 'ThreadID
                    Else
                        T_LVI.SubItems.Add("no Thread")
                    End If

                    T_LVI.SubItems.Add(APIRequest.Status) 'Status
                    If APIRequest.Status = "Ready" Then

                        If Not IsNothing(APIRequest.Result) Then
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

        If Not IsNothing(LiBoPayPalComs.SelectedItem) Then

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
    Private Sub BtTestCreate_Click(sender As Object, e As EventArgs) Handles BtTestCreate.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceCreateOrder, ULong.Parse(TBTestATComCollateral.Text), 100000000, ClsSignumAPI.String2ULng("USD")})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + TBTestATComAmount.Text + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub
    Private Sub BtTestAccept_Click(sender As Object, e As EventArgs) Handles BtTestAccept.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceAcceptOrder})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + TBTestATComAmount.Text + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub
    Private Sub BtTestFinish_Click(sender As Object, e As EventArgs) Handles BtTestFinish.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceFinishOrder})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + "100000000" + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub
    Private Sub BtTestInject_Click(sender As Object, e As EventArgs) Handles BtTestInject.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceInjectResponder, ULong.Parse(TBTestResponder.Text)})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + "100000000" + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub

    Private Sub BtTestATComInjectFWDKey_Click(sender As Object, e As EventArgs) Handles BtTestATComInjectFWDKey.Click
        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)
        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        If TBTestATComSHA256Key.Text.Trim = "" Then

            If TBTestATComTempSecretKey.Text.Trim = "" Then
                Exit Sub
            End If

            Dim SecretKeyList As List(Of ULong) = ClsSignumAPI.GetSHA256_64(TBTestATComTempSecretKey.Text)
            TBTestATComSHA256Key.Text = SecretKeyList(0).ToString
        End If

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceInjectChainSwapHash, ULong.Parse(TBTestATComSHA256Key.Text)})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + "100000000" + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub

    'Private Sub BtTestATComKeyOK_Click(sender As Object, e As EventArgs) Handles BtTestATComKeyOK.Click

    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

    '    Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

    '    Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceAcceptChainSwapHash, 1UL})
    '    Dim MsgStr As String = SignumAPI.ULngList2DataStr(ULngList)
    '    Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
    '    Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem).ToString.Trim + "&amountNQT=" + "100000000" + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

    '    Dim Response As String = SignumAPI.SignumRequest(postDataRL)

    '    LiBoATComms.Items.Add(Response)

    'End Sub

    Private Sub BtTestATComConvertTempSecretKey_Click(sender As Object, e As EventArgs) Handles BtTestATComConvertTempSecretKey.Click

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)
        Dim SecretKeyList As List(Of ULong) = ClsSignumAPI.GetSHA256_64(TBTestATComTempSecretKey.Text)

        TBTestATComFinishULong1.Text = SecretKeyList(0).ToString
        TBTestATComFinishULong2.Text = SecretKeyList(1).ToString
        TBTestATComSHA256Key.Text = SecretKeyList(2).ToString

    End Sub

    Private Sub BtTestATComFinishKey_Click(sender As Object, e As EventArgs) Handles BtTestATComFinishKey.Click

        'TODO: it will be marked as CANCELED if the Buyer sends ChainSwapKey to Contract

        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI(C_MainForm.PrimaryNode)

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(SignumAPI.GetTXFee) '.GetSlotFee)

        If TBTestATComFinishULong1.Text.Trim = "" Or TBTestATComFinishULong2.Text.Trim = "" Then

            If TBTestATComTempSecretKey.Text.Trim = "" Then
                Exit Sub
            End If

            Dim SecretKeyList As List(Of ULong) = ClsSignumAPI.GetSHA256_64(TBTestATComTempSecretKey.Text)
            TBTestATComFinishULong1.Text = SecretKeyList(0).ToString
            TBTestATComFinishULong2.Text = SecretKeyList(1).ToString
        End If

        Dim Finish As List(Of ULong) = ClsSignumAPI.GetSHA256_64(TBTestATComSHA256Key.Text)

        Dim ULngList As List(Of ULong) = New List(Of ULong)({SignumAPI.ReferenceFinishOrderWithChainSwapKey, ULong.Parse(TBTestATComFinishULong1.Text), ULong.Parse(TBTestATComFinishULong2.Text)})
        Dim MsgStr As String = ClsSignumAPI.ULngList2DataStr(ULngList)
        Dim TextMsg As String = "&message=" + MsgStr.Trim + "&messageIsText=False"
        Dim postDataRL As String = "requestType=sendMoney&recipient=" + ClsReedSolomon.Decode(CoBxTestATComATID.SelectedItem.ToString).ToString.Trim + "&amountNQT=" + "100000000" + "&secretPhrase=" + TBTestPP.Text + "&feeNQT=" + FeeNQT.ToString.Trim + "&deadline=60" + TextMsg

        Dim Response As String = SignumAPI.SignumRequest(postDataRL)

        LiBoATComms.Items.Add(Response)

    End Sub

    Private Sub TBTestATID_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBTestResponder.KeyPress

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

            If Not IsNothing(APIRequest.RequestThread) Then
                T_LVI.SubItems.Add(APIRequest.RequestThread.ManagedThreadId.ToString) 'ThreadID
            Else
                T_LVI.SubItems.Add("no Thread")
            End If

            T_LVI.SubItems.Add(APIRequest.Status) 'Status
            If APIRequest.Status = "Ready" Then

                If Not IsNothing(APIRequest.Result) Then
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

            If Not IsNothing(C_MainForm.DEXNET) Then
                If C_MainForm.DEXNET.DEXNETClose Then
                    C_MainForm.InitiateDEXNET()
                End If
            Else
                C_MainForm.InitiateDEXNET()
            End If

        Else
            If Not IsNothing(C_MainForm.DEXNET) Then
                C_MainForm.DEXNET.StopServer()
            End If
        End If

    End Sub

    Private Sub ChBxTestDEXNETShowStatus_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxTestDEXNETShowStatus.CheckedChanged

        SetINISetting(E_Setting.DEXNETShowStatus, ChBxTestDEXNETShowStatus.Checked)

        If Not IsNothing(C_MainForm.DEXNET) Then
            If ChBxTestDEXNETShowStatus.Checked Then
                C_MainForm.DEXNET.ShowStatus = True
            Else
                C_MainForm.DEXNET.ShowStatus = False
            End If
        End If

    End Sub

    Private Sub LiBoDEXNETStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LiBoDEXNETStatus.DoubleClick

        If Not IsNothing(LiBoDEXNETStatus.SelectedItem) Then

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

        If Not IsNothing(LiBoTestRelKeys.SelectedItem) Then
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





#End Region

End Class