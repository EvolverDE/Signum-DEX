
Public Class FrmMyOrderSettings

    Dim C_MainForm As PFPForm

    'Public Function GetPayTypes() As List(Of String)
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

    Private Sub FrmMyOrdersettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim T_OSList As List(Of ClsOrderSettings) = GetOrderSettings()

        For Each T_OS As ClsOrderSettings In T_OSList
            With LVOrders.Items.Add("TS-" + ClsReedSolomon.Encode(T_OS.ATID)) 'TODO: change TS- prefix
                .SubItems.Add(T_OS.TXID) 'TX
                .SubItems.Add(T_OS.Type) 'Type
                .SubItems.Add(T_OS.PaytypeString) 'Paytype
                .SubItems.Add(T_OS.Infotext) 'Infotext
                .SubItems.Add(T_OS.AutoSendInfotext) 'Autosendinfotext
                .SubItems.Add(T_OS.AutoCompleteAT) 'AutocompleteAT
                .SubItems.Add(T_OS.Status) 'Status
            End With

        Next

        LVOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

    End Sub

    Private Sub LVOrders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LVOrders.SelectedIndexChanged

        If LVOrders.SelectedItems.Count > 0 Then
            Dim SelectedItem As ListViewItem = LVOrders.SelectedItems(0)

            'Dim AT As String = GetLVColNameFromSubItem(LVOrders, "TX", SelectedItem)
            'Dim Type As String = GetLVColNameFromSubItem(LVOrders, "Type", SelectedItem)
            Dim Paytype As String = GetLVColNameFromSubItem(LVOrders, "Paytype", SelectedItem)
            Dim Infotext As String = GetLVColNameFromSubItem(LVOrders, "Infotext", SelectedItem)
            Dim AutoSendInfotext As String = GetLVColNameFromSubItem(LVOrders, "Autosend Infotext", SelectedItem)
            Dim AutoCompleteAT As String = GetLVColNameFromSubItem(LVOrders, "Autocomplete AT", SelectedItem)
            'Dim Status As String = GetLVColNameFromSubItem(LVOrders, "Status", SelectedItem)

            Dim Autosendbool As Boolean = False
            Try
                Autosendbool = CBool(AutoSendInfotext)
            Catch ex As Exception

            End Try
            ChBxAutosendInfo.Checked = Autosendbool


            Dim Autocompletebool As Boolean = False
            Try
                Autocompletebool = CBool(AutoCompleteAT)
            Catch ex As Exception

            End Try
            ChBxAutoCompleteAT.Checked = Autocompletebool


            CoBxPayType.SelectedItem = Paytype
            TBInfotext.Text = Infotext


        End If

    End Sub

    Private Sub BtSave_Click(sender As Object, e As EventArgs) Handles BtSave.Click

        If LVOrders.SelectedItems.Count > 0 Then
            Dim SelectedItem As ListViewItem = LVOrders.SelectedItems(0)

            Dim ATRS As String = GetLVColNameFromSubItem(LVOrders, "AT", SelectedItem)
            Dim ATID As ULong = ClsReedSolomon.Decode(ATRS)

            Dim TXID As ULong = CULng(GetLVColNameFromSubItem(LVOrders, "Transaction", SelectedItem))
            Dim Type As String = GetLVColNameFromSubItem(LVOrders, "Type", SelectedItem)
            'Dim Paytype As String = GetLVColNameFromSubItem(LVOrders, "Paytype", SelectedItem)
            'Dim Infotext As String = GetLVColNameFromSubItem(LVOrders, "Infotext", SelectedItem)
            'Dim AutoSendInfotext As String = GetLVColNameFromSubItem(LVOrders, "Autosend Infotext", SelectedItem)
            'Dim AutoCompleteAT As String = GetLVColNameFromSubItem(LVOrders, "Autocomplete AT", SelectedItem)
            Dim Status As String = GetLVColNameFromSubItem(LVOrders, "Status", SelectedItem)

            Dim Paytype As String = CoBxPayType.SelectedItem

            SetLVColName2SubItem(LVOrders, SelectedItem, "Paytype", Paytype)
            SetLVColName2SubItem(LVOrders, SelectedItem, "Infotext", TBInfotext.Text.Trim)
            SetLVColName2SubItem(LVOrders, SelectedItem, "Autosend Infotext", ChBxAutosendInfo.Checked.ToString)
            SetLVColName2SubItem(LVOrders, SelectedItem, "Autocomplete AT", ChBxAutoCompleteAT.Checked.ToString)

            LVOrders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

#Region "refresh Sellorders"
            For i As Integer = 0 To C_MainForm.LVSellorders.Items.Count - 1
                Dim LVISO As ListViewItem = C_MainForm.LVSellorders.Items(i)

                Dim PFPAT As PFPForm.S_PFPAT = LVISO.Tag

                If PFPAT.ATID = ATID Then

                    SetLVColName2SubItem(C_MainForm.LVSellorders, C_MainForm.LVSellorders.Items(i), "Method", Paytype)
                    SetLVColName2SubItem(C_MainForm.LVSellorders, C_MainForm.LVSellorders.Items(i), "Autoinfo", ChBxAutosendInfo.Checked.ToString)
                    SetLVColName2SubItem(C_MainForm.LVSellorders, C_MainForm.LVSellorders.Items(i), "Autofinish", ChBxAutoCompleteAT.Checked.ToString)

                    Exit For
                End If
            Next
#End Region

#Region "refreh Buyorders"
            For i As Integer = 0 To C_MainForm.LVBuyorders.Items.Count - 1
                Dim LVISO As ListViewItem = C_MainForm.LVBuyorders.Items(i)

                Dim PFPAT As PFPForm.S_PFPAT = LVISO.Tag

                If PFPAT.ATID = ATID Then

                    SetLVColName2SubItem(C_MainForm.LVBuyorders, C_MainForm.LVBuyorders.Items(i), "Method", Paytype)
                    SetLVColName2SubItem(C_MainForm.LVBuyorders, C_MainForm.LVBuyorders.Items(i), "Autoinfo", ChBxAutosendInfo.Checked.ToString)
                    SetLVColName2SubItem(C_MainForm.LVBuyorders, C_MainForm.LVBuyorders.Items(i), "Autofinish", ChBxAutoCompleteAT.Checked.ToString)

                    Exit For
                End If
            Next
#End Region

#Region "refresh Myorders"
            For i As Integer = 0 To C_MainForm.LVMyOpenOrders.Items.Count - 1
                Dim LVISO As ListViewItem = C_MainForm.LVMyOpenOrders.Items(i)

                Dim PFPOrder As PFPForm.S_Order = LVISO.Tag

                If PFPOrder.ATID = ATID Then

                    SetLVColName2SubItem(C_MainForm.LVMyOpenOrders, C_MainForm.LVMyOpenOrders.Items(i), "Method", Paytype)
                    SetLVColName2SubItem(C_MainForm.LVMyOpenOrders, C_MainForm.LVMyOpenOrders.Items(i), "Autoinfo", ChBxAutosendInfo.Checked.ToString)
                    SetLVColName2SubItem(C_MainForm.LVMyOpenOrders, C_MainForm.LVMyOpenOrders.Items(i), "Autofinish", ChBxAutoCompleteAT.Checked.ToString)

                    Exit For
                End If
            Next
#End Region

#Region "refresh OrderSettingsBuffer"
            For i As Integer = 0 To OrderSettingsBuffer.Count - 1
                Dim TT_OS As ClsOrderSettings = OrderSettingsBuffer(i)

                If TT_OS.ATID = ATID And TT_OS.TXID = TXID Then

                    TT_OS.Infotext = TBInfotext.Text.Trim
                    TT_OS.AutoSendInfotext = ChBxAutosendInfo.Checked.ToString
                    TT_OS.AutoCompleteAT = ChBxAutoCompleteAT.Checked.ToString
                    TT_OS.PaytypeString = Paytype
                    TT_OS.SetPayType()

                    Exit For
                End If

            Next
#End Region


#Region "refresh CSV"
            Dim T_OS As ClsOrderSettings = New ClsOrderSettings(ATID, TXID, Type, Status)

            T_OS.PaytypeString = Paytype
            T_OS.Infotext = TBInfotext.Text.Trim
            T_OS.AutoSendInfotext = ChBxAutosendInfo.Checked
            T_OS.AutoCompleteAT = ChBxAutoCompleteAT.Checked

            Dim Wait As Boolean = SaveOrderSettingsToCSV(New List(Of ClsOrderSettings)({T_OS}))
#End Region

        End If

    End Sub

    Private Sub CoBxPayType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CoBxPayType.SelectedIndexChanged

        If Not IsNothing(CoBxPayType.SelectedItem) Then

            Select Case CoBxPayType.SelectedItem
                Case ClsOrderSettings.E_PayType.Bankaccount.ToString
                    LabInfo.Visible = True
                    TBInfotext.Visible = True
                    LabInfo.Text = "Bankdata:"
                Case ClsOrderSettings.E_PayType.PayPal_E_Mail.ToString.Replace("_", "-")
                    LabInfo.Visible = True
                    TBInfotext.Visible = True
                    LabInfo.Text = "PayPal E-Mail:"
                Case ClsOrderSettings.E_PayType.PayPal_Order.ToString.Replace("_", "-")
                    LabInfo.Visible = False
                    TBInfotext.Visible = False
                    LabInfo.Text = "PayPal-Order:"
                Case ClsOrderSettings.E_PayType.Self_Pickup.ToString.Replace("_", "-")
                    LabInfo.Visible = True
                    TBInfotext.Visible = True
                    LabInfo.Text = "Location:"
                Case ClsOrderSettings.E_PayType.Other.ToString

            End Select

        End If

    End Sub

End Class