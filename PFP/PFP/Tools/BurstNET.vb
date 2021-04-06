Public Class BurstNET

    ''' <summary>
    ''' Generates the Masterkeys from PassPhrase 0=PublicKey; 1=SignKey; 2=AgreementKey
    ''' </summary>
    ''' <param name="PassPhrase">The PassPhrase as String</param>
    ''' <returns>List of Masterkeys</returns>
    Function GenerateMasterKeys(ByVal PassPhrase As String) As List(Of Byte())

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim HashPhrase() As Byte = Managed_SHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PassPhrase).ToArray)

        Dim Keys()() As Byte = Keygen(HashPhrase)

        Return Keys.ToList

    End Function

    ''' <summary>
    ''' Generates the Masterkeys from KeyHash
    ''' </summary>
    ''' <param name="KeyHash">The 32 Byte hashed PassPhrase</param>
    ''' <returns>Array of Masterkeys</returns>
    Function Keygen(KeyHash As Byte()) As Byte()()
        Dim PublicKey(31) As Byte
        Dim SignKey(31) As Byte

        Dim Curve As Curve25519 = New Curve25519
        Curve.Clamp(KeyHash)
        Curve.Core(PublicKey, SignKey, KeyHash, Nothing)

        Return {PublicKey, SignKey, KeyHash}

    End Function



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

    Function SignHelper(ByVal UnsignedMessageHex As String, ByVal PassPhrase As String) As S_Signature

        Dim Signature As S_Signature = New S_Signature

        Dim PassPhraseHash As List(Of Byte()) = GenerateMasterKeys(PassPhrase)

        Signature.UnsignedTransaction = UnsignedMessageHex
        Signature.PublicKey = ByteAry2HEX(PassPhraseHash(0))
        Signature.SignatureKey = ByteAry2HEX(PassPhraseHash(1))
        Signature.AgreementKey = ByteAry2HEX(PassPhraseHash(2))


        Dim Sign As String = GenerateSignature(UnsignedMessageHex, ByteAry2HEX(PassPhraseHash(1)))

        Signature.Sign = Sign

        Dim TransactionHEX As String = UnsignedMessageHex

        Dim TXBEnd As String = TransactionHEX.Substring(TransactionHEX.Length - 32)
        TransactionHEX = TransactionHEX.Remove(TransactionHEX.Length - 160)
        TransactionHEX += Sign + TXBEnd
        Signature.SignedTransaction = TransactionHEX

        Dim cSHA256_sigHash As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim signatureHash As Byte() = cSHA256_sigHash.ComputeHash(HEXStr2ByteAry(Sign))
        Signature.SignatureHash = ByteAry2HEX(signatureHash)

        Signature.FullHash = CalculateFullHash(UnsignedMessageHex, Signature.SignatureHash)

        Return Signature

    End Function


    ''' <summary>
    ''' Generates the Signature of the MessageHEX
    ''' </summary>
    ''' <param name="MessageHEX">Message as HEX String</param>
    ''' <param name="PrivateKey">The Private Key to Sign with</param>
    ''' <returns>Signature in HEX String</returns>
    Function GenerateSignature(ByVal MessageHEX As String, ByVal PrivateKey As String) As String

        Dim ECKCDSA As EC_KCDSA = New EC_KCDSA
        Dim Secure As Byte() = HEXStr2ByteAry(PrivateKey)
        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256Managed.Create()
        Dim MessageHash As Byte() = Managed_SHA256.ComputeHash(HEXStr2ByteAry(MessageHEX))
        Dim Message_Secure_Array() As Byte = MessageHash.Concat(Secure).ToArray
        Dim Message_Secure_Hash() As Byte = Managed_SHA256.ComputeHash(Message_Secure_Array)
        Dim MessageSecureKey_Hash As Byte() = Keygen(Message_Secure_Hash)(0)
        Dim MessageHash_MSKey As Byte() = MessageHash.Concat(MessageSecureKey_Hash).ToArray
        Dim MH_MSKey_Hash As Byte() = Managed_SHA256.ComputeHash(MessageHash_MSKey)
        Dim MH_MSKey_Hash_Copy As Byte() = MH_MSKey_Hash.ToArray
        Dim SignValue As Byte() = ECKCDSA.Sign(MH_MSKey_Hash, Message_Secure_Hash, Secure)
        Dim SignValue_MHMSKeyHash As Byte() = SignValue.Concat(MH_MSKey_Hash_Copy).ToArray

        Return ByteAry2HEX(SignValue_MHMSKeyHash)

    End Function

    ''' <summary>
    ''' Verify the Signature of the UnsignedMessageHEX String
    ''' </summary>
    ''' <param name="Signature">The Signature as HEX String</param>
    ''' <param name="UnsignedMessageHex">The Unsigned Message as HEX String</param>
    ''' <param name="PublicKey">The Public Key as HEX String</param>
    ''' <returns></returns>
    Function VerifySignature(ByVal Signature As String, ByVal UnsignedMessageHex As String, ByVal PublicKey As String) As Boolean

        Dim publicKeyBytes As Byte() = HEXStr2ByteAry(PublicKey)
        Dim SignValue As Byte() = HEXStr2ByteAry(Signature.Substring(0, 64))
        Dim SignHash As Byte() = HEXStr2ByteAry(Signature.Substring(64))

        Dim ECKCDSA As EC_KCDSA = New EC_KCDSA
        Dim VerifyHash As Byte() = ECKCDSA.Verify(SignValue, SignHash, publicKeyBytes)

        Dim Managed_SHA256 As System.Security.Cryptography.SHA256 = New Security.Cryptography.SHA256Managed
        Dim MessageHash As Byte() = Managed_SHA256.ComputeHash(HEXStr2ByteAry(UnsignedMessageHex))
        Dim MessageHash_VerifyHash As Byte() = MessageHash.Concat(VerifyHash).ToArray

        Managed_SHA256 = New Security.Cryptography.SHA256Managed
        Dim MessageVerify_Hash As Byte() = Managed_SHA256.ComputeHash(MessageHash_VerifyHash)

        Dim SignHashHEXString As String = ByteAry2HEX(SignHash)
        Dim MessageVerifyHashHEXString As String = ByteAry2HEX(MessageVerify_Hash)

        If SignHashHEXString = MessageVerifyHashHEXString Then
            Return True
        Else
            Return False
        End If

    End Function


    Function CalculateFullHash(ByVal UnsignedMessageHex As String, ByVal SignHash As String) As String

        Dim SHA256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
        Dim temp1 As Byte() = HEXStr2ByteAry(UnsignedMessageHex)
        Dim temp2 As Byte() = HEXStr2ByteAry(SignHash)
        Dim temp3 As Byte() = SHA256.ComputeHash(temp1.Concat(temp2).ToArray)

        Dim ret As String = ByteAry2HEX(temp3)
        Return ret

    End Function









    Function EncryptMessage(ByVal Plaintext As String, ByVal RecipientPublicKeyHex As String, ByVal SenderAgreementKeyHex As String) As String()

        Dim PlaintextBytes As Byte() = HEXStr2ByteAry(Str2Hex(Plaintext))
        Dim EncryptedMessage_Nonce As String() = EncryptData(PlaintextBytes, RecipientPublicKeyHex, SenderAgreementKeyHex)

        Return EncryptedMessage_Nonce

    End Function


    Function EncryptData(ByVal Data As Byte(), ByVal RecipientPublicKeyHex As String, ByVal SenderAgreementKeyHex As String) As String()

        Dim Curve As Curve25519 = New Curve25519
        Dim SharedKeyBytes As Byte() = New Byte(31) {}
        Curve.GetSharedSecret(SharedKeyBytes, HEXStr2ByteAry(SenderAgreementKeyHex), HEXStr2ByteAry(RecipientPublicKeyHex))
        Data = GZip.Compress(Data)

        Dim Nonce As Byte() = RandomBytes(31)
        Dim NonceHexStr As String = ByteAry2HEX(Nonce)

        Dim SharedKey(31) As Byte
        For i As Integer = 0 To 32 - 1
            SharedKey(i) = SharedKeyBytes(i) Xor Nonce(i)
        Next


        Dim AESH As AES = New AES
        Dim OutputBytes As Byte() = AESH.AES_Encrypt(Data, SharedKey)
        Dim OutputHexStr As String = ByteAry2HEX(OutputBytes)

        Return {OutputHexStr, NonceHexStr}

    End Function


    Function DecryptMessage(ByVal EncryptedMessage As String, ByVal Nonce As String, ByVal SenderPublicKeyHex As String, ByVal RecipientAgreementKeyHex As String) As String

        Dim EncryptedMessageBytes As Byte() = HEXStr2ByteAry(EncryptedMessage)
        Dim NonceBytes As Byte() = HEXStr2ByteAry(Nonce)

        Dim PlainText As String = DecryptData(EncryptedMessageBytes, NonceBytes, SenderPublicKeyHex, RecipientAgreementKeyHex)

        Return PlainText

    End Function


    Function DecryptData(ByVal Data As Byte(), ByVal Nonce As Byte(), ByVal SenderPublicKeyHex As String, RecipientAgreementKeyHex As String) As String

        Dim Curve As Curve25519 = New Curve25519
        Dim SharedKeyBytes As Byte() = New Byte(31) {}
        Curve.GetSharedSecret(SharedKeyBytes, HEXStr2ByteAry(RecipientAgreementKeyHex), HEXStr2ByteAry(SenderPublicKeyHex))

        Dim CompressedPlaintext As Byte() = Decrypt(Data, Nonce, SharedKeyBytes)

        Dim PlainText As String = GZip.Inflate(CompressedPlaintext)

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


        Dim AESH As AES = New AES
        Dim DecryptBytes = AESH.AES_Decrypt(Buffer, SharedKey, IV)

        Return DecryptBytes

    End Function










End Class
