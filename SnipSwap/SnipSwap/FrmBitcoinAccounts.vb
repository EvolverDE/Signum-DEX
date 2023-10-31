Imports System.Security.Cryptography

Public Class FrmBitcoinAccounts

    Private Sub FrmBitcoinAccounts_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TBWallet.Text = GetINISetting(E_Setting.BitcoinWallet, If(GlobalInDevelope, "TESTWALLET", "DEXWALLET"))

        Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")

        If Not T_Accounts.Trim = "" Then

            LVAddresses.Items.Clear()

            If T_Accounts.Contains(";") Then

                Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts.Split(";"c))

                For Each T_KeyPair As String In T_AccList

                    If T_KeyPair.Contains(":") Then

                        'Dim T_Mnemonic As String = T_KeyPair.Split(":"c)(0)
                        Dim T_PublicKey As String = T_KeyPair.Split(":"c)(1)
                        Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                        LVAddresses.Items.Add(T_Address)

                    End If

                Next

            Else

                If T_Accounts.Contains(":") Then

                    'Dim T_Mnemonic As String = T_Accounts.Split(":"c)(0)
                    Dim T_PublicKey As String = T_Accounts.Split(":"c)(1)
                    Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                    LVAddresses.Items.Add(T_Address)

                End If

            End If

        End If

        CheckPIN()
        GetMnemonic()

    End Sub


    Function GetMnemonic() As List(Of String)

        If Not CheckPIN() Then
            Return New List(Of String)
        End If

        Dim PINFingerprint As String = GetINISetting(E_Setting.PINFingerPrint, "")

        If PINFingerprint = "" Then
            Dim PlainMnemonic As String = GetINISetting(E_Setting.BitcoinAccounts, "")

            'TODO: load mnemonics

            'Dim MasterKeys As List(Of String) = GetMasterKeys(PlainPassPhrase)
            'MasterKeys.Add(PlainPassPhrase)

            'Return MasterKeys
        Else
            Dim AESMnemonic As String = GetINISetting(E_Setting.PassPhrase, "")
            Dim DecryptedMnemonic As String = AESDecrypt(AESMnemonic, GlobalPIN)

            'If DecryptedPassPhrase = AESPassPhrase Or DecryptedPassPhrase.Trim = "" Then
            '    Return New List(Of String)
            'End If

            'Dim MasterKeys As List(Of String) = GetMasterKeys(DecryptedPassPhrase)
            'MasterKeys.Add(DecryptedPassPhrase)

            'Return MasterKeys
        End If

        Return New List(Of String)

    End Function


    Private Function GetMnemonics() As List(Of List(Of String))

        Dim T_Mnemonics As List(Of String) = New List(Of String)
        Dim T_Addresses As List(Of String) = New List(Of String)
        Dim T_MnemonicsAddresses As String = GetINISetting(E_Setting.BitcoinAccounts, "")

        If T_MnemonicsAddresses.Contains(";") Then
            Dim TList_MnemonicsAddresses As List(Of String) = New List(Of String)(T_MnemonicsAddresses.Split(";"))

            For Each MnemonicAddress As String In TList_MnemonicsAddresses
                If MnemonicAddress.Contains(":") Then
                    T_Mnemonics.Add(MnemonicAddress.Split(":")(0))
                    T_Addresses.Add(MnemonicAddress.Split(":")(1))
                End If
            Next
        End If

        Return New List(Of List(Of String))({T_Mnemonics, T_Addresses})

    End Function


    Private Sub BtShowHideMnemonic_Click(sender As Object, e As EventArgs) Handles BtShowHideMnemonic.Click

        If TBBitcoinMnemonic.UseSystemPasswordChar Then
            TBBitcoinMnemonic.UseSystemPasswordChar = Not TBBitcoinMnemonic.UseSystemPasswordChar
            BtShowHideMnemonic.Text = "hide"
        Else
            TBBitcoinMnemonic.UseSystemPasswordChar = Not TBBitcoinMnemonic.UseSystemPasswordChar
            BtShowHideMnemonic.Text = "show"
        End If

    End Sub

    Private Sub BtShowHidePrivateKey_Click(sender As Object, e As EventArgs) Handles BtShowHidePrivateKey.Click

        If TBPrivateKey.UseSystemPasswordChar Then
            TBPrivateKey.UseSystemPasswordChar = Not TBPrivateKey.UseSystemPasswordChar
            BtShowHidePrivateKey.Text = "hide"
        Else
            TBPrivateKey.UseSystemPasswordChar = Not TBPrivateKey.UseSystemPasswordChar
            BtShowHidePrivateKey.Text = "show"
        End If

    End Sub

    Private Sub BtGenerateMnemonic_Click(sender As Object, e As EventArgs) Handles BtGenerateMnemonic.Click

        Dim sha As SHA256 = SHA256.Create()

        TBPrivateKey.Text = ByteArrayToHEXString(sha.ComputeHash(HEXStringToByteArray(StringToHEXString(TBBitcoinMnemonic.Text))))
        TBPublicKey.Text = PrivKeyToPubKey(TBPrivateKey.Text)
        TBAddress.Text = PubKeyToAddress(TBPublicKey.Text, BitcoinAddressPrefix)

    End Sub

    Private Sub BtAbortScan_Click(sender As Object, e As EventArgs) Handles BtAbortScan.Click

        Dim XItem As ClsBitcoin = New ClsBitcoin
        XItem.AbortReScan()

    End Sub

    Private Sub BtAdd_Click(sender As Object, e As EventArgs) Handles BtAdd.Click

        Dim T_BitAddress As String = PubKeyToAddress(TBPublicKey.Text, BitcoinAddressPrefix)

        If ChBxReScan.Checked Then
            Dim Result As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("The (Re)scanning process will take some Time" + vbCrLf + "Do you really want to continue?", "Add Address", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No), , ClsMsgs.Status.Question)

            If Result = ClsMsgs.CustomDialogResult.No Or Result = ClsMsgs.CustomDialogResult.Close Then
                Exit Sub
            End If

        End If

        If AddAddress(T_BitAddress) Then

            Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")
            Dim Fingerprint As String = GetINISetting(E_Setting.PINFingerPrint, "")

            If Fingerprint.Trim = "" Then

                If T_Accounts.Contains(";") Then
                    T_Accounts += ";" + TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
                Else
                    If T_Accounts.Trim = "" Then
                        T_Accounts += TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
                    Else
                        T_Accounts += ";" + TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
                    End If
                End If

            Else

                If GlobalPIN.Trim = "" Then
                    Dim PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.EnterPINOnly)
                    Dim Result As DialogResult = PINForm.ShowDialog()

                    If Result = DialogResult.Abort Or Result = DialogResult.Cancel Or Result = DialogResult.None Then
                        Exit Sub
                    End If

                End If

                Dim EncryptedMnemonic As String = AESEncrypt2HEXStr(TBBitcoinMnemonic.Text, GlobalPIN)

                GlobalPIN = ""

                If T_Accounts.Contains(";") Then
                    T_Accounts += ";" + EncryptedMnemonic + ":" + TBPublicKey.Text
                Else
                    If T_Accounts.Trim = "" Then
                        T_Accounts += EncryptedMnemonic + ":" + TBPublicKey.Text
                    Else
                        T_Accounts += ";" + EncryptedMnemonic + ":" + TBPublicKey.Text
                    End If

                End If

            End If

            SetINISetting(E_Setting.BitcoinAccounts, T_Accounts)

            ClsMsgs.MBox(PubKeyToAddress(TBPublicKey.Text, BitcoinAddressPrefix) + " successfully added to " + GetINISetting(E_Setting.BitcoinWallet, ""))

            Dim T_Accounts2 As String = GetINISetting(E_Setting.BitcoinAccounts, "")

            If Not T_Accounts2.Trim = "" Then

                LVAddresses.Items.Clear()

                If T_Accounts2.Contains(";") Then

                    Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts2.Split(";"c))

                    For Each T_KeyPair As String In T_AccList

                        If T_KeyPair.Contains(":") Then

                            'Dim T_Mnemonic As String = T_KeyPair.Split(":"c)(0)
                            Dim T_PublicKey As String = T_KeyPair.Split(":"c)(1)
                            Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                            LVAddresses.Items.Add(T_Address)

                        End If

                    Next

                Else

                    If T_Accounts2.Contains(":") Then

                        'Dim T_Mnemonic As String = T_Accounts2.Split(":"c)(0)
                        Dim T_PublicKey As String = T_Accounts2.Split(":"c)(1)
                        Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                        LVAddresses.Items.Add(T_Address)

                    End If

                End If

            End If

        End If

    End Sub

    Private Function AddAddress(ByVal Address As String) As Boolean

        Dim XItem As ClsBitcoin = New ClsBitcoin
        If ChBxReScan.Checked Then
            Return XItem.ImportBitcoinDescriptor(Address, "0")
        Else
            Return XItem.ImportBitcoinDescriptor(Address)
        End If

        Return False

    End Function

    Private Sub BtLoadWallet_Click(sender As Object, e As EventArgs) Handles BtLoadWallet.Click
        Dim XItem As ClsBitcoin = New ClsBitcoin
        XItem.LoadBitcoinWallet(TBWallet.Text)
    End Sub

    Private Sub BtCreateWallet_Click(sender As Object, e As EventArgs) Handles BtCreateWallet.Click
        Dim XItem As ClsBitcoin = New ClsBitcoin
        If XItem.CreateNewBitcoinWallet(TBWallet.Text) Then
            SetINISetting(E_Setting.BitcoinWallet, TBWallet.Text)
        End If
    End Sub

    Private Sub BtUnloadWallet_Click(sender As Object, e As EventArgs) Handles BtUnloadWallet.Click
        Dim XItem As ClsBitcoin = New ClsBitcoin
        XItem.UnloadBitcoinWallet(TBWallet.Text)
    End Sub

    Private Sub ScanningTime_Tick(sender As Object, e As EventArgs) Handles ScanningTime.Tick

        Dim XItem As ClsBitcoin = New ClsBitcoin
        Dim WalletInfo As String = XItem.GetWalletInfo()

        Dim Scanning As String = GetStringBetween(WalletInfo, "<scanning>", "</scanning>")
        Dim Progress As Double = GetDoubleBetween(Scanning, "<progress>", "</progress>")

        If Progress = 0.0 Then
            StatusLabel.Text = ""
            StatusBar.Value = 0
            StatusBar.Visible = False
            BtAbortScan.Enabled = False
            If LVAddresses.SelectedItems.Count > 0 Then
                BtRescan.Enabled = True
                BtRefresh.Enabled = True
            End If
        Else
            StatusLabel.Text = "(Re)scanning... " + Math.Round(Progress * 100, 2).ToString() + " %"
            StatusBar.Visible = True
            StatusBar.Value = Progress * 100
            BtAbortScan.Enabled = True
            BtRescan.Enabled = False
            BtRefresh.Enabled = False
        End If

    End Sub

    Private Sub LVAddresses_MouseDown(sender As Object, e As MouseEventArgs) Handles LVAddresses.MouseDown
        LVAddresses.ContextMenuStrip = Nothing
    End Sub

    Private Sub LVAddresses_MouseUp(sender As Object, e As MouseEventArgs) Handles LVAddresses.MouseUp
        LVAddresses.ContextMenuStrip = Nothing

        If LVAddresses.SelectedItems.Count > 0 Then

            Dim LVi As ListViewItem = LVAddresses.SelectedItems(0)

            Dim Address As String = Convert.ToString(GetLVColNameFromSubItem(LVAddresses, "Address", LVi))

            Dim LVContextMenu As ContextMenuStrip = New ContextMenuStrip

            Dim LVCMItem As ToolStripMenuItem = New ToolStripMenuItem
            LVCMItem.Text = "copy address"
            LVCMItem.Tag = Address
            AddHandler LVCMItem.Click, AddressOf Copy2CB
            LVContextMenu.Items.Add(LVCMItem)

            LVAddresses.ContextMenuStrip = LVContextMenu

        End If
    End Sub

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

    Private Sub BtRescan_Click(sender As Object, e As EventArgs) Handles BtRescan.Click
        BtRescan.Enabled = False

        If LVAddresses.SelectedItems.Count > 0 Then

            Dim Address As String = GetLVColNameFromSubItem(LVAddresses, "Address", LVAddresses.SelectedItems(0))

            Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
            Dim Result As String = BitNet.ImportDescriptor(Address, "0")
            Result = Result

        End If

    End Sub

    Private Sub LVAddresses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LVAddresses.SelectedIndexChanged

        BtRescan.Enabled = False
        BtRefresh.Enabled = False

        If LVAddresses.SelectedItems.Count > 0 Then
            BtRescan.Enabled = True
            BtRefresh.Enabled = True
        End If

    End Sub

    Private Sub BtRefresh_Click(sender As Object, e As EventArgs) Handles BtRefresh.Click
        BtRefresh.Enabled = False

        If LVAddresses.SelectedItems.Count > 0 Then

            Dim Address As String = GetLVColNameFromSubItem(LVAddresses, "Address", LVAddresses.SelectedItems(0))

            Dim BitNet As ClsBitcoinNET = New ClsBitcoinNET()
            Dim Result As String = BitNet.ImportDescriptor(Address)
            Result = Result

        End If
    End Sub

End Class