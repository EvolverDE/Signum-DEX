Imports System.Security.Cryptography

Public Class FrmBitcoinAccounts

    Private Sub FrmBitcoinAccounts_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TBWallet.Text = GetINISetting(E_Setting.BitcoinWallet, "DEXWALLET")

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
            If XItem.LoadBitcoinWallet(GetINISetting(E_Setting.BitcoinWallet, "")) Then
            End If

            Dim Result As Boolean = False
            Dim LambdaThread As New Threading.Thread(
                Sub()
                    Result = XItem.ImportNewBitcoinAddress(Address)
                End Sub
            )
            LambdaThread.Start()

            'While LambdaThread.IsAlive
            '    Application.DoEvents()

            'Dim WalletInfo As String = XItem.GetWalletInfo()
            ''<walletname>DEXWALLET</walletname><walletversion>169900</walletversion><format>bdb</format><balance>0.00000000</balance><unconfirmed_balance>0.00000000</unconfirmed_balance><immature_balance>0.00000000</immature_balance><txcount>46</txcount><keypoololdest>1673007808</keypoololdest><keypoolsize>1000</keypoolsize><hdseedid>d3eb50b74aab6b30fc1ef5de130dfff614170668</hdseedid><keypoolsize_hd_internal>1000</keypoolsize_hd_internal><paytxfee>0.00000000</paytxfee><private_keys_enabled>true</private_keys_enabled><avoid_reuse>false</avoid_reuse><scanning><duration>16</duration><progress>0.004873486990590367</progress></scanning><descriptors>false</descriptors>
            'WalletInfo = WalletInfo

            'Dim Scanning As String = GetStringBetween(WalletInfo, "<scanning>", "</scanning>")
            'Dim Progress As Double = GetDoubleBetween(Scanning, "<progress>", "</progress>")


            'For i As Integer = 0 To 100
            '    Threading.Thread.Sleep(1)
            '    Application.DoEvents()
            'Next

            'End While


            'If XItem.ImportNewBitcoinAddress(Address) Then
            '    ClsMsgs.MBox(Address + " successfully added to " + GetINISetting(E_Setting.BitcoinWallet, ""))
            Return True
            'End If

        Else

            If XItem.LoadBitcoinWallet(GetINISetting(E_Setting.BitcoinWallet, "")) Then
            End If
            If XItem.CreateNewBitcoinAddress(Address) Then
                'ClsMsgs.MBox(Address + " successfully added to " + GetINISetting(E_Setting.BitcoinWallet, ""))
                Return True
            End If

        End If

        Return False

    End Function

    Private Sub BtLoadWallet_Click(sender As Object, e As EventArgs) Handles BtLoadWallet.Click
        Dim XItem As ClsBitcoin = New ClsBitcoin
        XItem.LoadBitcoinWallet(TBWallet.Text)
    End Sub

    Private Sub BtCreateWallet_Click(sender As Object, e As EventArgs) Handles BtCreateWallet.Click
        Dim XItem As ClsBitcoin = New ClsBitcoin
        XItem.CreateNewBitcoinWallet(TBWallet.Text)
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
        Else
            StatusLabel.Text = "(Re)scanning... " + Math.Round(Progress * 100, 2).ToString() + " %"
            StatusBar.Visible = True
            StatusBar.Value = Progress * 100
            BtAbortScan.Enabled = True
        End If

    End Sub

End Class