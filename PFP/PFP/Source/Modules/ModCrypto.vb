
Module ModCrypto


    Function GetSHA256HashString(ByVal Input As String) As String

        Dim InputBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(Input)
        Dim SHA256 As System.Security.Cryptography.SHA256Managed = New System.Security.Cryptography.SHA256Managed()
        InputBytes = SHA256.ComputeHash(InputBytes)
        Dim HashString As String = ""

        For i As Integer = 0 To InputBytes.Length - 1 'Step 2
            Dim T_Byte As Byte = InputBytes(i)

            Dim T_HEXString As String = Conversion.Hex(T_Byte)

            If T_HEXString.Length = 1 Then
                T_HEXString = "0" + T_HEXString
            End If

            HashString += T_HEXString

        Next

        Return HashString

    End Function

    Function GetPubKeyHEX(ByVal PassPhrase As String) As String

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)

        Dim Keys()() As Byte = Keygen(HashPhrase)

        Dim Pubkey As String = ByteArrayToHEXString(Keys(0))

        Return Pubkey

    End Function

    Function GetSignKeyHEX(ByVal PassPhrase As String) As String

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)

        Dim Keys()() As Byte = Keygen(HashPhrase)

        Dim SignKey As String = ByteArrayToHEXString(Keys(1))

        Return SignKey

    End Function

    Function GetAgreeKeyHEX(ByVal PassPhrase As String) As String

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)

        Dim Keys()() As Byte = Keygen(HashPhrase)

        Dim AgreeKey As String = ByteArrayToHEXString(Keys(2))

        Return AgreeKey

    End Function



    ''' <summary>
    ''' Generates the Masterkeys from PassPhrase 0=PublicKey; 1=SignKey; 2=AgreementKey
    ''' </summary>
    ''' <returns>List of Masterkeys</returns>
    Function GetMasterKeys(ByVal PassPhrase As String) As List(Of String)

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)
        Dim Keys()() As Byte = Keygen(HashPhrase)
        Dim KeyHESList As List(Of String) = New List(Of String)({ByteArrayToHEXString(Keys(0)), ByteArrayToHEXString(Keys(1)), ByteArrayToHEXString(Keys(2))})
        Return KeyHESList

    End Function

    ''' <summary>
    ''' Generates the Masterkeys from KeyHash
    ''' </summary>
    ''' <param name="KeyHash">The 32 Byte hashed PassPhrase</param>
    ''' <returns>Array of Masterkeys</returns>
    Function Keygen(KeyHash As Byte()) As Byte()()
        Dim PublicKey(31) As Byte
        Dim SignKey(31) As Byte

        Dim Curve As ClsCurve25519 = New ClsCurve25519
        Curve.Clamp(KeyHash)
        Curve.Core(PublicKey, SignKey, KeyHash, Nothing)

        Return {PublicKey, SignKey, KeyHash}

    End Function

    ''' <summary>
    ''' Generates the Signature of the MessageHEX
    ''' </summary>
    ''' <param name="MessageHEX">Message as HEX String</param>
    ''' <param name="PrivateKey">The Private Key to Sign with</param>
    ''' <returns>Signature in HEX String</returns>
    Function GenerateSignature(ByVal MessageHEX As String, ByVal PrivateKey As String) As String

        Dim ECKCDSA As EC_KCDSA = New EC_KCDSA
        Dim Secure As Byte() = HEXStringToByteArray(PrivateKey)
        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim MessageHash As Byte() = Managed_SHA256.ComputeHash(HEXStringToByteArray(MessageHEX))
        Dim Message_Secure_Array() As Byte = MessageHash.Concat(Secure).ToArray
        Dim Message_Secure_Hash() As Byte = Managed_SHA256.ComputeHash(Message_Secure_Array)
        Dim MessageSecureKey_Hash As Byte() = Keygen(Message_Secure_Hash)(0)
        Dim MessageHash_MSKey As Byte() = MessageHash.Concat(MessageSecureKey_Hash).ToArray
        Dim MH_MSKey_Hash As Byte() = Managed_SHA256.ComputeHash(MessageHash_MSKey)
        Dim MH_MSKey_Hash_Copy As Byte() = MH_MSKey_Hash.ToArray
        Dim SignValue As Byte() = ECKCDSA.Sign(MH_MSKey_Hash, Message_Secure_Hash, Secure)
        Dim SignValue_MHMSKeyHash As Byte() = SignValue.Concat(MH_MSKey_Hash_Copy).ToArray

        Return ByteArrayToHEXString(SignValue_MHMSKeyHash)

    End Function

    ''' <summary>
    ''' Verify the Signature of the UnsignedMessageHEX String
    ''' </summary>
    ''' <param name="Signature">The Signature as HEX String</param>
    ''' <param name="UnsignedMessageHex">The Unsigned Message as HEX String</param>
    ''' <param name="PublicKey">The Public Key as HEX String</param>
    ''' <returns></returns>
    Function VerifySignature(ByVal Signature As String, ByVal UnsignedMessageHex As String, ByVal PublicKey As String) As Boolean

        If Not MessageIsHEXString(Signature) Or Not MessageIsHEXString(UnsignedMessageHex) Or Not MessageIsHEXString(PublicKey) Then
            Return False
        End If

        Dim publicKeyBytes As Byte() = HEXStringToByteArray(PublicKey)
        Dim SignValue As Byte() = HEXStringToByteArray(Signature.Substring(0, 64))
        Dim SignHash As Byte() = HEXStringToByteArray(Signature.Substring(64))

        Dim ECKCDSA As EC_KCDSA = New EC_KCDSA
        Dim VerifyHash As Byte() = ECKCDSA.Verify(SignValue, SignHash, publicKeyBytes)

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = New Security.Cryptography.SHA256Managed
        Dim MessageHash As Byte() = Managed_SHA256.ComputeHash(HEXStringToByteArray(UnsignedMessageHex))
        Dim MessageHash_VerifyHash As Byte() = MessageHash.Concat(VerifyHash).ToArray

        Managed_SHA256 = New Security.Cryptography.SHA256Managed
        Dim MessageVerify_Hash As Byte() = Managed_SHA256.ComputeHash(MessageHash_VerifyHash)

        Dim SignHashHEXString As String = ByteArrayToHEXString(SignHash)
        Dim MessageVerifyHashHEXString As String = ByteArrayToHEXString(MessageVerify_Hash)

        If SignHashHEXString = MessageVerifyHashHEXString Then
            Return True
        Else
            Return False
        End If

    End Function


    Function AESEncrypt2HEXStr(ByVal Input As String, ByVal Password As String) As String

        Dim T_AES As ClsAES = New ClsAES

        Dim T_DecryptedBytes() As Byte = T_AES.AES_Encrypt(System.Text.Encoding.UTF8.GetBytes(Input), System.Text.Encoding.UTF8.GetBytes(Password))

        Return ByteArrayToHEXString(T_DecryptedBytes)

    End Function


    Function AESEncrypt2ByteArray(ByVal Input As String, ByVal Password As String) As Byte()

        Dim T_AES As ClsAES = New ClsAES

        Dim T_DecryptedBytes() As Byte = T_AES.AES_Encrypt(System.Text.Encoding.UTF8.GetBytes(Input), System.Text.Encoding.UTF8.GetBytes(Password))

        Return T_DecryptedBytes

    End Function


    Function AESDecrypt(ByVal Input As String, ByVal Password As String) As String

        If Input.Trim = "" Then
            Return ""
        End If

        Dim T_AES As ClsAES = New ClsAES

        Dim T_EncryptedBytes() As Byte = T_AES.AES_Decrypt(HEXStringToByteArray(Input), System.Text.Encoding.UTF8.GetBytes(Password))

        If IsNothing(T_EncryptedBytes) Then
            Return Input
        Else
            Return System.Text.Encoding.UTF8.GetString(T_EncryptedBytes)
        End If

    End Function

    Function AESDecrypt(ByVal Input As Byte(), ByVal Password As String) As String

        If Input.Length = 0 Then
            Return ""
        End If

        Dim T_AES As ClsAES = New ClsAES

        Dim T_EncryptedBytes() As Byte = T_AES.AES_Decrypt(Input, System.Text.Encoding.UTF8.GetBytes(Password))

        If IsNothing(T_EncryptedBytes) Then
            Return ""
        Else
            Return System.Text.Encoding.UTF8.GetString(T_EncryptedBytes)
        End If

    End Function

End Module