Imports System.Net
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

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
        TBPrivateKey.Text = ByteArrayToHEXString(sha.ComputeHash(HEXStringToByteArray(TBBitcoinMnemonic.Text)))
        TBPublicKey.Text = PrivKeyToPubKey(TBPrivateKey.Text)
        TBAddress.Text = PubKeyToAddress(TBPublicKey.Text, BitcoinAddressPrefix)

    End Sub

    Private Sub BtAdd_Click(sender As Object, e As EventArgs) Handles BtAdd.Click

        Dim T_BitAddress As String = PubKeyToAddress(TBPublicKey.Text, BitcoinAddressPrefix)

        If AddAddress(T_BitAddress) Then

            Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")

            If T_Accounts.Contains(";") Then
                T_Accounts += ";" + TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
            Else
                If T_Accounts.Trim = "" Then
                    T_Accounts += TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
                Else
                    T_Accounts += ";" + TBBitcoinMnemonic.Text + ":" + TBPublicKey.Text
                End If

            End If

            SetINISetting(E_Setting.BitcoinAccounts, T_Accounts)

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

        If ChBxReScan.Checked Then
            If LoadBitcoinWallet(GetINISetting(E_Setting.BitcoinWallet, "")) Then
            End If
            If ImportNewBitcoinAddress(Address) Then
                ClsMsgs.MBox(Address + " successfully added to " + GetINISetting(E_Setting.BitcoinWallet, ""))
                Return True
            End If

        Else
            If LoadBitcoinWallet(GetINISetting(E_Setting.BitcoinWallet, "")) Then
            End If
            If CreateNewBitcoinAddress(Address) Then
                ClsMsgs.MBox(Address + " successfully added to " + GetINISetting(E_Setting.BitcoinWallet, ""))
                Return True
            End If

        End If

        Return False

    End Function

    Private Sub BtLoadWallet_Click(sender As Object, e As EventArgs) Handles BtLoadWallet.Click
        LoadBitcoinWallet(TBWallet.Text)
    End Sub

    Private Sub BtCreateWallet_Click(sender As Object, e As EventArgs) Handles BtCreateWallet.Click
        CreateNewBitcoinWallet(TBWallet.Text)
    End Sub

    Private Sub BtUnloadWallet_Click(sender As Object, e As EventArgs) Handles BtUnloadWallet.Click
        UnloadBitcoinWallet(TBWallet.Text)
    End Sub
End Class