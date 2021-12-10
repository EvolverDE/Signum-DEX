Imports System.Security.Cryptography

Public Class FrmEnterPIN

    Public Property C_Mode As E_Mode = E_Mode.SignMessage

    Private C_PublicKey As String = ""
    Public ReadOnly Property PublicKey As String
        Get
            Return C_PublicKey
        End Get
    End Property

    Private C_SignKey As String = ""
    Public ReadOnly Property SignKey As String
        Get
            Return C_SignKey
        End Get
    End Property

    Private C_AgreeKey As String = ""
    Public ReadOnly Property AgreeKey() As String
        Get
            Return C_AgreeKey
        End Get
    End Property

    Public Enum E_Mode
        EnterPINOnly = 0
        ChangePIN = 1
        SignMessage = 2
    End Enum



    Sub New(ByVal Mode As E_Mode)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_Mode = Mode
        ChBxPIN.Checked = True

        Dim PINFingerPrint As String = GetINISetting(E_Setting.PINFingerPrint, "")

        If PINFingerPrint.Trim = "" Then
            SplitContainer1.Panel1Collapsed = True
        End If

        If C_Mode = E_Mode.ChangePIN Then
            Me.Text = "Change PIN"
            BtOK.Text = "change"
            Label3.Visible = True
            TBOldPIN.Visible = True
            ChBxPIN.Enabled = True
            SplitContainer1.Panel2Collapsed = True

            If Not GlobalPIN = "" Then
                TBOldPIN.Text = GlobalPIN
            End If

        ElseIf C_Mode = E_Mode.EnterPINOnly Then
            Me.Text = "Enter PIN"
            BtOK.Text = "OK"
            Label3.Visible = False
            TBOldPIN.Visible = False
            ChBxPIN.Enabled = False
            SplitContainer1.Panel2Collapsed = True
        Else 'If C_Mode = E_Mode.SignMessage Then
            Me.Text = "Enter PIN"
            BtOK.Text = "OK"
            Label3.Visible = False
            TBOldPIN.Visible = False
            ChBxPIN.Enabled = False
            SplitContainer1.Panel2Collapsed = False 'TODO: Sign Unsigned Transaction Bytes
        End If

    End Sub

    Private Sub BtOK_Click(sender As Object, e As EventArgs) Handles BtOK.Click

        BtOK.Enabled = False

        Dim OldPINFingerPrint As String = GetINISetting(E_Setting.PINFingerPrint, "")

        If C_Mode = E_Mode.ChangePIN Then
            'change PIN

            Dim SHA512 As SHA512 = SHA512Managed.Create()
            Dim HashOldPIN() As Byte = SHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(TBOldPIN.Text).ToArray)
            Dim HashOldPINHEX As String = ByteArrayToHEXString(HashOldPIN)

            If OldPINFingerPrint = HashOldPINHEX Then
                Dim EncryptedPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")
                Dim DecryptedPassPhrase As String = AESDecrypt(EncryptedPassPhrase, TBOldPIN.Text)

                If TBPIN.Text = "" Then
                    Dim Res As ClsMsgs.CustomDialogResult = ClsMsgs.MBox("The PassPhrase will shown as plaintext in Settings.ini" + vbCrLf + "do you really want to do that?", "Warning", ClsMsgs.DefaultButtonMaker(ClsMsgs.DBList.Yes_No),, ClsMsgs.Status.Warning)

                    If Res = ClsMsgs.CustomDialogResult.Yes Then
                        SetINISetting(E_Setting.PassPhrase, DecryptedPassPhrase)
                        SetINISetting(E_Setting.PINFingerPrint, "")
                        GlobalPIN = ""
                    Else
                        BtOK.Enabled = True
                        Exit Sub
                    End If

                Else
                    EncryptedPassPhrase = AESEncrypt2HEXStr(DecryptedPassPhrase, TBPIN.Text)
                    SetINISetting(E_Setting.PassPhrase, EncryptedPassPhrase)

                    Dim SHA512a As SHA512 = SHA512Managed.Create()
                    Dim HashNewPIN() As Byte = SHA512a.ComputeHash(System.Text.Encoding.UTF8.GetBytes(TBPIN.Text).ToArray)
                    Dim HashNewPINHEX As String = ByteArrayToHEXString(HashNewPIN)
                    SetINISetting(E_Setting.PINFingerPrint, HashNewPINHEX)

                End If

            ElseIf OldPINFingerPrint = "" And TBPIN.Text <> "" Then

                Dim SHA512a As SHA512 = SHA512Managed.Create()
                Dim HashNewPIN() As Byte = SHA512a.ComputeHash(System.Text.Encoding.UTF8.GetBytes(TBPIN.Text).ToArray)
                Dim HashNewPINHEX As String = ByteArrayToHEXString(HashNewPIN)
                SetINISetting(E_Setting.PINFingerPrint, HashNewPINHEX)

                Dim PlainPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")
                Dim EncryptedPassPhrase As String = AESEncrypt2HEXStr(PlainPassPhrase, TBPIN.Text)
                SetINISetting(E_Setting.PassPhrase, EncryptedPassPhrase)

            Else
                'wrong PIN
                ClsMsgs.MBox("The Old PIN you entered is wrong!", "Wrong PIN",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                BtOK.Enabled = True
                Exit Sub
            End If

        ElseIf C_Mode = E_Mode.EnterPINOnly Then

            Dim SHA512 As SHA512 = SHA512Managed.Create()
            Dim HashOldPIN() As Byte = SHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(TBPIN.Text).ToArray)
            Dim HashOldPINHEX As String = ByteArrayToHEXString(HashOldPIN)

            If OldPINFingerPrint = HashOldPINHEX Then
                GlobalPIN = TBPIN.Text
            Else
                'wrong PIN
                ClsMsgs.MBox("The PIN you entered is wrong!", "Wrong PIN",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                BtOK.Enabled = True
                Exit Sub
            End If

        Else 'If C_Mode = E_Mode.SignMessage Then

            Dim EncryptedPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

            If Not EncryptedPassPhrase = "" Then

                Dim DecryptedPassPhrase As String = AESDecrypt(EncryptedPassPhrase, TBPIN.Text)

                If Not DecryptedPassPhrase = EncryptedPassPhrase Then
                    Dim Masterkeys As List(Of String) = GetMasterKeys(DecryptedPassPhrase)

                    C_PublicKey = Masterkeys(0)
                    C_SignKey = Masterkeys(1)
                    C_AgreeKey = Masterkeys(2)
                Else
                    'wrong PIN
                    ClsMsgs.MBox("The PIN you entered is wrong!", "Wrong PIN",,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
                    BtOK.Enabled = True
                    Exit Sub
                End If

            Else
                'no passphrase
            End If

        End If

        Me.DialogResult = DialogResult.OK

        Me.Close()

    End Sub

    Private Sub TBPIN_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TBSignedBytes.KeyPress, TBPIN.KeyPress

        Dim keys As Integer = Asc(e.KeyChar)

        Select Case keys
                'Case 48 To 57, 44, 8
                    ' Zahlen, 8=Backspace und 32=Space 46=Punkt 44=Komma zulassen
            Case 13
                ' ENTER
                BtOK.PerformClick()
                e.Handled = True
            Case Else
                'alle anderen Eingaben unterdrücken
                'e.Handled = True
        End Select

    End Sub

End Class