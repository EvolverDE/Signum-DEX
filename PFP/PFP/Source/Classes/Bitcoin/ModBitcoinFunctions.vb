
Option Strict On
Option Explicit On

Imports System.Numerics
Imports System.Security.Cryptography

Module ModBitcoinFunctions

    Property BitcoinAddressPrefix As String = "6f" '00=main; 6f=testnet
    Property BitcoinConfigFile As String = Application.StartupPath + "\Settings.ini"
    Property BitcoinConfigFileSection As String = "Bitcoin"

    Property BitcoinNodeRequestList As List(Of ClsBitcoinNET.S_ThreadedMethod) = New List(Of ClsBitcoinNET.S_ThreadedMethod) ' TODO: this list has to be cleared on program shutdown

    Public Function IntToHex(ByVal Int As Integer, Optional ByVal Length As Integer = -1, Optional BigEndian As Boolean = True) As String
        Dim BI As BigInteger = New BigInteger(Int)
        Return BigIntToHEX(BI, Length, BigEndian)
    End Function

    Public Function ULongToHex(ByVal ULo As ULong, Optional ByVal Length As Integer = -1, Optional BigEndian As Boolean = True) As String
        Dim BI As Numerics.BigInteger = New Numerics.BigInteger(ULo)
        Return BigIntToHEX(BI, Length, BigEndian)
    End Function

    Public Function BigIntToHEX(ByVal BigInt As Numerics.BigInteger, Optional ByVal Length As Integer = -1, Optional BigEndian As Boolean = True) As String

        Dim HEXList As List(Of String) = New List(Of String)({"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"})

        If BigEndian Then
            Return ChangeBaseSystem(BigInt, HEXList, Length)
        Else
            Return ChangeHEXStrEndian(ChangeBaseSystem(BigInt, HEXList, Length))
        End If

    End Function

    Public Function ChangeBaseSystem(ByVal Input As Numerics.BigInteger, ByVal BaseSystem As List(Of String), Optional Length As Integer = -1) As String

        Dim BS As ClsBaseX = New ClsBaseX(BaseSystem)
        Dim Converted As String = BS.BigIntToWordString(Input, "")

        If Not Length = -1 Then
            If Converted.Length < Length Then
                Dim T_Len As Integer = Converted.Length

                For i As Integer = T_Len To Length - 1
                    Converted = "0" + Converted
                Next

            End If
        End If

        If Converted.Length Mod 2 <> 0 Then
            Converted = "0" + Converted
        End If

        Return Converted
    End Function

    Public Function ChangeHEXStrEndian(ByVal InputHEX As String) As String

        Dim StartEndian As List(Of Byte) = New List(Of Byte)(HEXStringToByteArray(InputHEX))
        StartEndian.Reverse()
        Dim EndEndian As String = ByteArrayToHEXString(StartEndian.ToArray)

        Return EndEndian

    End Function

    Function GetBitcoinMainPrivateKey(ByVal ShowPINForm As Boolean) As String

        If Not CheckPIN() Then
            If ShowPINForm Then
                Dim T_PINForm As FrmEnterPIN = New FrmEnterPIN(FrmEnterPIN.E_Mode.EnterPINOnly)
                Dim Result As DialogResult = T_PINForm.ShowDialog()
            End If

            If GlobalPIN.Trim = "" Then
                Return ""
            End If
        End If

        Dim PINFingerprint As String = GetINISetting(E_Setting.PINFingerPrint, "")
        Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")

        If Not PINFingerprint = "" Then

            If T_Accounts.Contains(";") Then

                Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts.Split(";"c))

                Dim T_Account As String = T_AccList(0)

                If T_Account.Contains(":") Then
                    Dim AESMnemonic As String = T_Account.Split(":"c)(0)
                    Dim DecryptedMnemonic As String = AESDecrypt(AESMnemonic, GlobalPIN)
                    Dim T_PrivateKey As String = GetSHA256HashString(DecryptedMnemonic)
                    Return T_PrivateKey
                End If

            Else
                If T_Accounts.Contains(":") Then
                    Dim AESMnemonic As String = T_Accounts.Split(":"c)(0)
                    Dim DecryptedMnemonic As String = AESDecrypt(AESMnemonic, GlobalPIN)
                    Dim T_PrivateKey As String = GetSHA256HashString(DecryptedMnemonic)
                    Return T_PrivateKey
                End If

            End If

        Else

            If T_Accounts.Contains(";") Then

                Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts.Split(";"c))

                Dim T_Account As String = T_AccList(0)

                If T_Account.Contains(":") Then
                    Dim T_Mnemonic As String = T_Account.Split(":"c)(0)
                    Dim T_PrivateKey As String = GetSHA256HashString(T_Mnemonic)
                    Return T_PrivateKey
                End If

            Else

                If T_Accounts.Contains(":") Then
                    Dim T_Mnemonic As String = T_Accounts.Split(":"c)(0)
                    Dim T_PrivateKey As String = GetSHA256HashString(T_Mnemonic)
                    Return T_PrivateKey
                End If

            End If

        End If

        Return ""

    End Function

    Function GetBitcoinMainAddress() As String

        Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")

        If Not T_Accounts.Trim = "" Then

            If T_Accounts.Contains(";") Then

                Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts.Split(";"c))

                Dim T_Account As String = T_AccList(0)

                If T_Account.Contains(":") Then
                    Dim T_PublicKey As String = T_Account.Split(":"c)(1)
                    Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                    Return T_Address
                End If

            Else

                If T_Accounts.Contains(":") Then
                    Dim T_PublicKey As String = T_Accounts.Split(":"c)(1)
                    Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                    Return T_Address
                End If

            End If

        End If

        Return ""

    End Function

    Function GetBitcoinAddresses(Optional ByVal AddressCount As Integer = -1) As List(Of String)

        Dim T_Accounts As String = GetINISetting(E_Setting.BitcoinAccounts, "")

        If Not T_Accounts.Trim = "" Then

            If T_Accounts.Contains(";") Then

                Dim T_AccList As List(Of String) = New List(Of String)(T_Accounts.Split(";"c))

                Dim T_Accs As List(Of String) = New List(Of String)
                For Each TAcc As String In T_AccList

                    If TAcc.Contains(":") Then
                        Dim T_PublicKey As String = TAcc.Split(":"c)(1)
                        Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)

                        T_Accs.Add(T_Address)

                    End If

                Next

                Return T_Accs

            Else

                If T_Accounts.Contains(":") Then
                    Dim T_PublicKey As String = T_Accounts.Split(":"c)(1)
                    Dim T_Address As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)
                    Return New List(Of String)({T_Address})
                End If

            End If

        End If

        Return New List(Of String)

    End Function

    Function PrivKeyToPubKey(ByVal PrivKey As String) As String

        Dim PubKeyAry As Byte() = New Byte(63) {}
        Dim FullPubKey As Byte() = New Byte(64) {}
        Dim CompressedPubKey As Byte() = New Byte(32) {}

        Secp256k1Vb.Secp256k1.Start()

        Dim PubKey As Secp256k1Vb.PublicKey = Secp256k1Vb.Secp256k1.CreatePublicKey(HEXStringToByteArray(PrivKey.ToLower()))
        CompressedPubKey = PubKey.Serialize(True)

        Return ByteArrayToHEXString(CompressedPubKey)

    End Function

    Function PubKeyToRipe160(ByVal PubKey As String) As String

        Dim RIPEMD160 As RIPEMD160Managed = New RIPEMD160Managed()
        Dim SHA256 As SHA256Cng = New SHA256Cng()

        Dim Fullhash As String = ByteArrayToHEXString(SHA256.ComputeHash(HEXStringToByteArray(PubKey)))
        Dim Ripe160Hash As String = ByteArrayToHEXString(RIPEMD160.ComputeHash(HEXStringToByteArray(Fullhash)))

        Return Ripe160Hash

    End Function

    Function PubKeyToAddress(ByVal PubKey As String, Optional Prefix As String = "00") As String
        Dim Ripe160Hash As String = PubKeyToRipe160(PubKey)
        Return RIPE160ToAddress(Ripe160Hash, Prefix)
    End Function

    Function RIPE160ToAddress(ByVal Ripe160 As String, Optional Prefix As String = "00") As String

        Dim SHA256 As SHA256Cng = New SHA256Cng()

        Dim Ripe160HashPrefix As String = Prefix + Ripe160
        Dim Ripe160HashPrefixFirstHash As String = ByteArrayToHEXString(SHA256.ComputeHash(HEXStringToByteArray(Ripe160HashPrefix)))
        Dim Ripe160HashPrefixFullHash As String = ByteArrayToHEXString(SHA256.ComputeHash(HEXStringToByteArray(Ripe160HashPrefixFirstHash)))
        Dim Ripe160PrefixChecksum As String = Ripe160HashPrefix + Ripe160HashPrefixFullHash.Remove(8)
        Dim Base58Adress As String = Ripe160PreCheckToAddress(Ripe160PrefixChecksum)

        Return Base58Adress

    End Function

    Function Ripe160PreCheckToAddress(ByVal Ripe160 As String) As String
        Dim Address As String = ClsBase58.EncodeHexToBase58(Ripe160)
        Return Address
    End Function

    Function AddressToRipe160(ByVal Address As String, Optional ByVal Shorten As Boolean = True) As String

        Dim Ripe160 As String = ClsBase58.DecodeBase58ToHex(Address)

        If Ripe160.Trim.Length > 8 Then
            If Shorten Then
                Ripe160 = Ripe160.Substring(2)
                Ripe160 = Ripe160.Remove(Ripe160.Length - 8)
            End If
        End If

        Return Ripe160

    End Function

    Function Dbl2Satoshi(ByVal Bitcoin As Double) As ULong

        If Double.IsInfinity(Bitcoin) Then
            Bitcoin = 0.0
        End If

        Dim Satoshi As ULong = Convert.ToUInt64(Bitcoin * 100000000UL)
        Return Satoshi

    End Function
    Function Satoshi2Dbl(ByVal Satoshi As ULong) As Double

        Dim Bitcoin As Double = Satoshi / 100000000UL
        Return Bitcoin

    End Function

End Module
