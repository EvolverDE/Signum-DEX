Option Strict On
Option Explicit On

Public Class ClsSignumNET

    'Public Property C_PromptPIN() As Boolean = True

    'Sub New(ByVal PromptPIN As Boolean)
    '    C_PromptPIN = PromptPIN
    'End Sub

    '''' <summary>
    '''' Generates the Masterkeys from PassPhrase 0=PublicKey; 1=SignKey; 2=AgreementKey
    '''' </summary>
    '''' <returns>List of Masterkeys</returns>
    'Function GenerateMasterKeys() As List(Of Byte())

    '    Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
    '    Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)

    '    Dim Keys()() As Byte = Keygen(HashPhrase)

    '    Return Keys.ToList

    'End Function



    Public Structure S_Signature
        Dim AgreementKey As String
        Dim SignatureKey As String
        Dim PublicKey As String

        Dim UnsignedTransaction As String
        Dim SignedTransaction As String
        Dim SignatureHash As String
        Dim Sign As String
        Dim FullHash As String

    End Structure

    Function SignHelper(ByVal UnsignedMessageHex As String, ByVal SignKeyHEX As String) As S_Signature

        Dim Signature As S_Signature = New S_Signature


        If UnsignedMessageHex.Trim = "" Then
            Return Signature
        End If

        'Dim PassPhraseHash As List(Of Byte()) = GenerateMasterKeys()

        'Signature.UnsignedTransaction = UnsignedMessageHex
        'Signature.PublicKey = ByteArrayToHEXString(PassPhraseHash(0))
        'Signature.SignatureKey = ByteArrayToHEXString(PassPhraseHash(1))
        'Signature.AgreementKey = ByteArrayToHEXString(PassPhraseHash(2))


        Dim Sign As String = GenerateSignature(UnsignedMessageHex, SignKeyHEX)

        Signature.Sign = Sign

        Dim TransactionHEX As String = UnsignedMessageHex

        Dim TXBEnd As String = TransactionHEX.Substring(320)
        TransactionHEX = TransactionHEX.Remove(192)
        TransactionHEX &= Sign & TXBEnd
        Signature.SignedTransaction = TransactionHEX

        Dim cSHA256_sigHash As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim signatureHash As Byte() = cSHA256_sigHash.ComputeHash(HEXStringToByteArray(Sign))
        Signature.SignatureHash = ByteArrayToHEXString(signatureHash)

        Signature.FullHash = CalculateFullHash(UnsignedMessageHex, Signature.SignatureHash)

        Return Signature

    End Function

    Function CalculateFullHash(ByVal UnsignedMessageHex As String, ByVal SignHash As String) As String

        Dim SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
        Dim temp1 As Byte() = HEXStringToByteArray(UnsignedMessageHex)
        Dim temp2 As Byte() = HEXStringToByteArray(SignHash)
        Dim temp3 As Byte() = SHA256.ComputeHash(temp1.Concat(temp2).ToArray)

        Dim ret As String = ByteArrayToHEXString(temp3)
        Return ret

    End Function

    ''' <summary>
    ''' Encrypts Message from Parameters 0=Data; 1=Nonce
    ''' </summary>
    ''' <param name="Plaintext"></param>
    ''' <param name="RecipientPublicKeyHex"></param>
    ''' <param name="SenderAgreementKeyHex"></param>
    ''' <returns></returns>
    Function EncryptMessage(ByVal Plaintext As String, ByVal RecipientPublicKeyHex As String, ByVal SenderAgreementKeyHex As String) As String()

        Dim PlaintextBytes As Byte() = HEXStringToByteArray(StringToHEXString(Plaintext))
        Dim EncryptedMessage_Nonce As String() = EncryptData(PlaintextBytes, RecipientPublicKeyHex, SenderAgreementKeyHex)

        Return EncryptedMessage_Nonce

    End Function

    ''' <summary>
    ''' Encrypt Data from Parameters 0=Data; 1=Nonce
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <param name="RecipientPublicKeyHex"></param>
    ''' <param name="SenderAgreementKeyHex"></param>
    ''' <returns></returns>
    Function EncryptData(ByVal Data As Byte(), ByVal RecipientPublicKeyHex As String, ByVal SenderAgreementKeyHex As String) As String()

        Dim Curve As ClsCurve25519 = New ClsCurve25519
        Dim SharedKeyBytes As Byte() = New Byte(31) {}
        Curve.GetSharedSecret(SharedKeyBytes, HEXStringToByteArray(SenderAgreementKeyHex), HEXStringToByteArray(RecipientPublicKeyHex))
        Data = ClsGZip.Compress(Data)

        Dim Nonce As Byte() = RandomBytes(31)
        Dim NonceHexStr As String = ByteArrayToHEXString(Nonce)

        Dim SharedKey(31) As Byte
        For i As Integer = 0 To 32 - 1
            SharedKey(i) = SharedKeyBytes(i) Xor Nonce(i)
        Next


        Dim AESH As ClsAES = New ClsAES
        Dim OutputBytes As Byte() = AESH.AES_Encrypt(Data, SharedKey)
        Dim OutputHexStr As String = ByteArrayToHEXString(OutputBytes)

        Return {OutputHexStr, NonceHexStr}

    End Function


    'Public Function DecryptFrom(ByVal AccountID As ULong, ByVal data As String, ByVal nonce As String) As String
    '    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI()
    '    Dim PublicKey As String = SignumAPI.GetAccountPublicKeyFromAccountID_RS(AccountID.ToString)
    '    Return DecryptFrom(PublicKey, data, nonce)
    'End Function

    Public Function DecryptFrom(ByVal PublicKey As String, ByVal data As String, ByVal nonce As String) As String

        Dim MasterKeyList As List(Of String) = GetPassPhrase()
        '0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
        If MasterKeyList.Count > 0 Then

            Dim AgreementKey As String = MasterKeyList(2)

            Dim DecryptedMsg As String = DecryptMessage(data, nonce, PublicKey, AgreementKey)

            If DecryptedMsg.Contains(Application.ProductName + "-error") Then
                Return Application.ProductName + "-error in DecryptFrom(): -> " + vbCrLf + DecryptedMsg
            Else

                If Not MessageIsHEXString(DecryptedMsg) Then
                    Return DecryptedMsg
                Else
                    Return Application.ProductName + "-error in DecryptFrom(): -> " + vbCrLf + DecryptedMsg
                End If

            End If

        Else
            Return Application.ProductName + "-warning in DecryptFrom(): -> no Keys"
        End If

    End Function

    Function DecryptMessage(ByVal EncryptedMessage As String, ByVal Nonce As String, ByVal SenderPublicKeyHex As String, ByVal RecipientAgreementKeyHex As String) As String
        Try

            Dim EncryptedMessageBytes As Byte() = HEXStringToByteArray(EncryptedMessage)
            Dim NonceBytes As Byte() = HEXStringToByteArray(Nonce)

            Dim PlainText As String = DecryptData(EncryptedMessageBytes, NonceBytes, SenderPublicKeyHex, RecipientAgreementKeyHex)

            Return PlainText

        Catch ex As Exception
            Return Application.ProductName + "-error in SignumNET.DecryptMessage(): " + ex.Message
        End Try

    End Function

    Function DecryptData(ByVal Data As Byte(), ByVal Nonce As Byte(), ByVal SenderPublicKeyHex As String, RecipientAgreementKeyHex As String) As String

        Dim Curve As ClsCurve25519 = New ClsCurve25519
        Dim SharedKeyBytes As Byte() = New Byte(31) {}
        Curve.GetSharedSecret(SharedKeyBytes, HEXStringToByteArray(RecipientAgreementKeyHex), HEXStringToByteArray(SenderPublicKeyHex))

        Dim CompressedPlaintext As Byte() = Decrypt(Data, Nonce, SharedKeyBytes)

        Dim PlainText As String = ClsGZip.Inflate(CompressedPlaintext)

        Return PlainText

    End Function

    Function Decrypt(ByVal ivCiphertext As Byte(), nonce As Byte(), sharedKeyOrig As Byte()) As Byte()

        'If ivCiphertext.Length < 16 Or ivCiphertext.Length Mod 16 <> 0 Then
        '    Return Nothing
        'End If

        Dim SharedKey As Byte() = sharedKeyOrig.ToArray

        For i As Integer = 0 To 32 - 1
            SharedKey(i) = SharedKey(i) Xor nonce(i)
        Next

        Dim SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim Key As Byte() = SHA256.ComputeHash(SharedKey)
        Dim IV(15) As Byte
        Dim Buffer(ivCiphertext.Length - 1 - 16) As Byte

        Array.Copy(ivCiphertext, IV, 16)
        Array.Copy(ivCiphertext, 16, Buffer, 0, ivCiphertext.Length - 16)


        Dim AESH As ClsAES = New ClsAES
        Dim DecryptBytes = AESH.AES_Decrypt(Buffer, SharedKey, IV)

        Return DecryptBytes

    End Function

End Class